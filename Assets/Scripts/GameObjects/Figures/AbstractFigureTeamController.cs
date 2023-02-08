using System;
using System.Collections.Generic;
using Scripts.Core;
using Scripts.Playboard;
using UnityEngine;

namespace Scripts.GameObjects.Figures
{
    public enum eTeamName
    {
        RED,
        BLUE
    }

    /// <summary>
    /// Abstract class for the figure teams used in game. Needs to be overriden to implement full functionality.
    /// </summary>
    public abstract class AbstractFigureTeamController : MonoBehaviour
    {
        [SerializeField] protected eTeamName _teamName;
        public eTeamName TeamName { get => _teamName; protected set => _teamName = value; }
        [Space(5)]
        [SerializeField] protected List<FigureController> _teamMembers;
        [SerializeField] protected GameDirector _gameDirector;
        [SerializeField] protected int _superDiceMaxVal = 3;
        public int SuperDiceMaxVal { get => _superDiceMaxVal; protected set => _superDiceMaxVal = value; }

        protected bool _superDiceReady = false;
        public bool SuperDiceReady { get => _superDiceReady; private set => _superDiceReady = value; }

        protected int _superDiceStatus = 0;
        public int SuperDiceStatus { get => _superDiceStatus; protected set => _superDiceStatus = value; }

        protected int numOfMembersInportal = 0;
        public int NumOfMembersInportal { get => numOfMembersInportal; set => numOfMembersInportal = value; }

        protected List<FigureController> _figuresInGame = new List<FigureController>();
        protected List<FigureController> _availableForSelection;
        protected FigureController _selectedFigure = null;
        protected int _figureSelectedIndex = 0;
        protected bool _canSelectFromTheRamp = true;
        protected FigureController _figureFromTheRamp;
        public FigureController SelectedFigure { get => _selectedFigure; protected set => _selectedFigure = value; }

        //figure selection related events
        public event Action<FigureController> onFigureSelectionChanged;
        public event Action onFigureSelectionConfirmed;
        //figure movement related events
        public event Action<FigureController> onFigureMovementEnd;
        public event Action<FigureController, int> onFigureWaypointChange;
        //dice roll related events
        public event Action onLeftPressed;
        public event Action onRightPressed;
        public event Action onConfirmPressed;

        protected virtual void Awake() { }
        protected virtual void OnEnable()
        {
            foreach (FigureController figure in _teamMembers)
            {
                figure.onWaypointReached += OnFigureMovementChangedCb;
                figure.onCurrentMoveEnd += OnFigureMovementEndCb;
                figure.onDeathEvent += OnRemoveFigureFromPlayfield;
                figure.onFinishReachedEvent += OnRemoveFigureFromPlayfield;
            }
        }

        protected virtual void OnDisable()
        {
            foreach (FigureController figure in _teamMembers)
            {
                figure.onWaypointReached -= OnFigureMovementChangedCb;
                figure.onCurrentMoveEnd -= OnFigureMovementEndCb;
                figure.onDeathEvent -= OnRemoveFigureFromPlayfield;
                figure.onFinishReachedEvent -= OnRemoveFigureFromPlayfield;
            }
        }

        /// <summary>
        /// Fills the list with figures that can be selected on the start of the turn
        /// </summary>
        protected void GetFiguresAvailableForSelection()
        {
            _availableForSelection = new List<FigureController>(_figuresInGame);
            _figureFromTheRamp = GetFigureFromTheStartRamp();

            //make sure we cannot select figure on the ramp when there is already figure on the start
            _canSelectFromTheRamp = _figureFromTheRamp != null;
            foreach (FigureController figure in _figuresInGame)
            {
                if (figure.Team == _teamName && figure.CurrentBoardIndex == _gameDirector.Playboard.GetStartTileIndex(_teamName))
                    _canSelectFromTheRamp = false;
            }

            if (_canSelectFromTheRamp && _figureFromTheRamp != null)
                _availableForSelection.Add(_figureFromTheRamp);
        }

        /**
        * Methods for event invokings follows
        */
        // Figure selection
        protected void InvokeOnFigureSelectionChanged(FigureController selectedFigure)
        {
            onFigureSelectionChanged?.Invoke(selectedFigure);
        }

        protected void InvokeOnFigureSelectionConfirmed()
        {
            onFigureSelectionConfirmed?.Invoke();
        }

        // Controls
        protected void InvokeOnLeftPressedEvent()
        {
            onLeftPressed?.Invoke();
        }

        protected void InvokeOnRightPressedEvent()
        {
            onRightPressed?.Invoke();
        }

        protected void InvokeOnConfirmPressedEvent()
        {
            onConfirmPressed?.Invoke();
        }

        public void UpdateSuperDiceStatus()
        {
            if (_superDiceReady)
                return;

            _superDiceStatus++;
            _superDiceReady = _superDiceStatus >= _superDiceMaxVal;
        }

        public void ResetSuperDice()
        {
            _superDiceStatus = 0;
            _superDiceReady = false;
        }

        /// <summary>
        /// Used for the figures starting from the start ramp
        /// </summary>
        public void DoTheStartMovement()
        {
            if (_selectedFigure != null)
                _selectedFigure.GoToStart(_gameDirector.Playboard.GetStartRamp(_teamName).PathToMainBoard);
            else Debug.LogError(gameObject.name + ": DoTheStartMovement, no figure selected!");
        }

        protected FigureController GetFigureFromTheStartRamp()
        {
            FigureStartPoint? firstOccupied = _gameDirector.Playboard.GetStartRamp(TeamName).GetFirstOccupiedPosition();
            return firstOccupied != null ? firstOccupied.Value.AssignedFigure : null;
        }

        /// <summary>
        /// Called after figure leaves the start ramp and is on the main board
        /// </summary>
        /// <param name="figure">Given figure</param>
        private void PutFigureInGame(FigureController figure)
        {
            figure.SetToInGame(_gameDirector.Playboard.MainBoardPath, _gameDirector.Playboard.GetStartTileIndex(_teamName));
            _figuresInGame.Add(figure);
        }

        /***
        * Events binded to figures
        **/
        private void OnRemoveFigureFromPlayfield(FigureController figure)
        {
            _figuresInGame.Remove(figure);
        }

        private void OnFigureMovementChangedCb(FigureController figure, int newIndex)
        {
            onFigureWaypointChange?.Invoke(figure, newIndex);
        }

        private void OnFigureMovementEndCb(FigureController figure)
        {
            onFigureMovementEnd?.Invoke(figure);
            if (figure.CurrentState == eFigureState.ON_START_RAMP)
                PutFigureInGame(figure);
        }

        /***
        * Abstract methods
        **/
        public abstract void ChooseFigureToTurnWith();
        public abstract void HandleDiceInteraction();
    }
}
