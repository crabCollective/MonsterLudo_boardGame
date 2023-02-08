using Scripts.Core.Input;
using UnityEngine;

namespace Scripts.GameObjects.Figures
{
    /// <summary>
    /// Class representing human-controlled team of figures. Handles the player input
    /// </summary>
    public class HumanFigureTeamController : AbstractFigureTeamController
    {
        [SerializeField] private InputHandlerSO _inputHandler;

        protected override void Awake()
        {
            base.Awake();
            foreach (FigureController figure in _teamMembers)
                figure.Init(_teamName);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            _inputHandler.EnableInput();

            _inputHandler.onLeftPressed += OnLeftPressed;
            _inputHandler.onRightPressed += OnRightPressed;
            _inputHandler.onConfirmPressed += OnConfirmPressed;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _inputHandler.DisableInput();

            _inputHandler.onLeftPressed -= OnLeftPressed;
            _inputHandler.onRightPressed -= OnRightPressed;
            _inputHandler.onConfirmPressed -= OnConfirmPressed;
        }

        public override void ChooseFigureToTurnWith()
        {
            GetFiguresAvailableForSelection();

            _figureSelectedIndex = 0;
            _selectedFigure = _availableForSelection[_figureSelectedIndex];

            InvokeOnFigureSelectionChanged(_selectedFigure);
        }

        /**
        * Input-related methods
        */
        private void OnLeftPressed()
        {
            if (_gameDirector.CurrentState == Core.eGAMESTATE.FIGURE_SELECTION)
            {
                _figureSelectedIndex--;
                _figureSelectedIndex = (_figureSelectedIndex >= 0) ? _figureSelectedIndex : _availableForSelection.Count - 1;
                _selectedFigure = _availableForSelection[_figureSelectedIndex];

                InvokeOnFigureSelectionChanged(_selectedFigure);
            }
            else if (_gameDirector.CurrentState == Core.eGAMESTATE.DICE_ROLL)
            {
                InvokeOnLeftPressedEvent();
            }
        }

        private void OnRightPressed()
        {
            if (_gameDirector.CurrentState == Core.eGAMESTATE.FIGURE_SELECTION)
            {
                _figureSelectedIndex++;
                _figureSelectedIndex = (_figureSelectedIndex <= _availableForSelection.Count - 1) ? _figureSelectedIndex : 0;
                _selectedFigure = _availableForSelection[_figureSelectedIndex];

                InvokeOnFigureSelectionChanged(_selectedFigure);
            }
            else if (_gameDirector.CurrentState == Core.eGAMESTATE.DICE_ROLL)
            {
                InvokeOnRightPressedEvent();
            }
        }

        private void OnConfirmPressed()
        {
            if (_gameDirector.CurrentState == Core.eGAMESTATE.FIGURE_SELECTION)
                InvokeOnFigureSelectionConfirmed();
            else if (_gameDirector.CurrentState == Core.eGAMESTATE.DICE_ROLL || _gameDirector.CurrentState == Core.eGAMESTATE.GAME_START)
                InvokeOnConfirmPressedEvent();
        }

        //Nothing to do for human - controlled team. Working without some additional work
        public override void HandleDiceInteraction() { }
    }
}
