using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using Scripts.Core.UI;
using Scripts.GameObjects;
using Scripts.GameObjects.Figures;
using Scripts.Playboard;
using Scripts.Playboard.Tiles;
using UnityEngine;

namespace Scripts.Core
{
    public enum eGAMESTATE
    {
        GAME_START,
        ROUND_START,
        FIGURE_SELECTION,
        DICE_ROLL,
        FIGURE_MOVEMENT,
        ROUND_END,
        GAME_OVER
    }

    /// <summary>
    /// Main script driving the game - Game Director. Based on simple FSM represented by various game states
    /// </summary>
    public class GameDirector : MonoBehaviour
    {
        [Header("Basic references")]
        [SerializeField] private PlayboardController _playboard;
        public PlayboardController Playboard { get => _playboard; private set => _playboard = value; }
        //Player team
        [SerializeField] private AbstractFigureTeamController _team1;
        //AI team
        [SerializeField] private AbstractFigureTeamController _team2;
        [SerializeField] private UIManager _UIManager;
        [SerializeField] private DiceSelectorController _dice;
        [SerializeField] private GameObject _selectionArrow;

        [Header("Feedbacks")]
        [SerializeField] private MMF_Player _figureSelectionFeedback;
        [SerializeField] private MMF_Player _figureConfirmationFeedback;
        [SerializeField] private MMF_Player _diceSelectionFeedback;
        [SerializeField] private MMF_Player _diceConfirmationFeedback;
        [SerializeField] private MMF_Player _diceTypeConfirmationFeedback;

        private eGAMESTATE _currentState;
        public eGAMESTATE CurrentState { get => _currentState; set => _currentState = value; }

        private delegate void StateInitMethod();
        private Dictionary<eGAMESTATE, StateInitMethod> StatesInitMethods;

        private int _currentRound = 1;
        private int _lastDiceNumber = -1;
        private int _goalBoardIndex = -1;
        private AbstractFigureTeamController _teamPlaying;
        private bool _isTeam1Playing = true;
        private bool _gameEnded = false;
        private bool _teamTurningAgain = false;
        private bool _justStarted = true;

        /***********************************
        * UNITY METHODS
        ************************************/
        void Start()
        {
            _selectionArrow.SetActive(false);

            StatesInit();
            GameInit();
        }

        private void OnDisable()
        {
            RemoveTeamControllerEvents(_teamPlaying);
        }

        private void AssignTeamControllerEvents(AbstractFigureTeamController team)
        {
            team.onFigureMovementEnd += OnFigureMovementEnd;
            team.onFigureWaypointChange += OnFigureWaypointChange;
            team.onFigureSelectionChanged += OnFigureSelectionChange;
            team.onFigureSelectionConfirmed += OnFigureSelectionConfirmed;
            team.onLeftPressed += OnLeftPressed;
            team.onRightPressed += OnRightPressed;
            team.onConfirmPressed += OnConfirmPressed;
        }

        private void RemoveTeamControllerEvents(AbstractFigureTeamController team)
        {
            team.onFigureMovementEnd -= OnFigureMovementEnd;
            team.onFigureWaypointChange -= OnFigureWaypointChange;
            team.onFigureSelectionChanged -= OnFigureSelectionChange;
            team.onFigureSelectionConfirmed -= OnFigureSelectionConfirmed;
            team.onLeftPressed -= OnLeftPressed;
            team.onRightPressed -= OnRightPressed;
            team.onConfirmPressed -= OnConfirmPressed;
        }

        /***********************************
        * GAME DIRECTOR HANDLING METHODS
        ************************************/
        /// <summary>
        /// Assign states init methods to the dictionary using delegates
        /// </summary>
        private void StatesInit()
        {
            StatesInitMethods = new Dictionary<eGAMESTATE, StateInitMethod>();
            StatesInitMethods.Add(eGAMESTATE.ROUND_START, StartRound);
            StatesInitMethods.Add(eGAMESTATE.FIGURE_SELECTION, HandleFigureSelection);
            StatesInitMethods.Add(eGAMESTATE.DICE_ROLL, HandleDiceInteraction);
            StatesInitMethods.Add(eGAMESTATE.FIGURE_MOVEMENT, DoTheMovement);
            StatesInitMethods.Add(eGAMESTATE.ROUND_END, EndRound);
            StatesInitMethods.Add(eGAMESTATE.GAME_OVER, GameOver);

            _currentState = eGAMESTATE.GAME_START;
        }

        private void NextState()
        {
            switch (_currentState)
            {
                case eGAMESTATE.GAME_START:
                    _currentState = eGAMESTATE.ROUND_START;
                    break;
                case eGAMESTATE.ROUND_START:
                    _currentState = eGAMESTATE.FIGURE_SELECTION;
                    break;
                case eGAMESTATE.FIGURE_SELECTION:
                    //if we play with figure on the start, there is no dice roll
                    if (_teamPlaying.SelectedFigure.CurrentState == eFigureState.ON_START_RAMP)
                    {
                        _currentState = eGAMESTATE.FIGURE_MOVEMENT;
                    }
                    else
                    {
                        _currentState = eGAMESTATE.DICE_ROLL;
                    }
                    break;
                case eGAMESTATE.DICE_ROLL:
                    _currentState = eGAMESTATE.FIGURE_MOVEMENT;
                    break;
                case eGAMESTATE.FIGURE_MOVEMENT:
                    _currentState = eGAMESTATE.ROUND_END;
                    break;
                case eGAMESTATE.ROUND_END:
                    if (_gameEnded)
                    {
                        _currentState = eGAMESTATE.GAME_OVER;
                    }
                    else
                    {
                        _currentState = eGAMESTATE.ROUND_START;
                    }
                    break;
                default:
                    Debug.LogError("Error! UNKNNOWN STATE DETECTED!");
                    break;
            }

            //Call state init method
            StatesInitMethods[_currentState]();
        }

        private void SetTeamPlaying()
        {
            _teamPlaying = _isTeam1Playing ? _team1 : _team2;
        }

        private void SetTheUI()
        {
            _UIManager.SetRoundNumber(_currentRound);
            _UIManager.SetTeamPlayingSign(_teamPlaying.TeamName);
        }

        /***********************************
        * STATE - GAME_START RELATED METHODS
        ************************************/
        private void GameInit()
        {
            _currentRound = 1;
            _justStarted = true;
            _isTeam1Playing = true;
            _UIManager.ActivateTitleScreen(true);

            SetTeamPlaying();
            SetTheUI();
            AssignTeamControllerEvents(_teamPlaying);
        }

        /// <summary>
        /// Called after the title screen has been shown
        /// </summary>
        private void OnTitleSignShown()
        {
            _UIManager.ActivateTitleScreen(false);
            NextState();
        }

        /***********************************
        * STATE - ROUND_START RELATED METHODS
        ************************************/
        private void StartRound()
        {
            if (!_justStarted)
            {
                _currentRound++;
                SetTeamPlaying();
                SetTheUI();
                AssignTeamControllerEvents(_teamPlaying);
            }
            else _justStarted = false;

            _teamPlaying.UpdateSuperDiceStatus();
            _UIManager.SetSuperDiceSliderBar(_teamPlaying.TeamName, _teamPlaying.SuperDiceStatus);

            NextState();
        }

        /***********************************
        * STATE - ROUND_END RELATED METHODS
        ************************************/
        private void EndRound()
        {
            if (!_teamTurningAgain)
                _isTeam1Playing = !_isTeam1Playing;
            else _teamTurningAgain = false;

            RemoveTeamControllerEvents(_teamPlaying);

            if (_teamPlaying.NumOfMembersInportal >= 4)
                _gameEnded = true;

            NextState();
        }

        /***********************************
        * STATE - GAME_OVER RELATED METHODS
        ************************************/
        private void GameOver()
        {
            _UIManager.ShowFinalScreen(_teamPlaying.TeamName);
        }

        /***********************************
        * STATE - FIGURE_SELECTION RELATED METHODS
        ************************************/
        private void HandleFigureSelection()
        {
            EnableSelectionArrow();
            _teamPlaying.ChooseFigureToTurnWith();
        }

        private void EnableSelectionArrow()
        {
            _selectionArrow.SetActive(true);
        }

        private void DisableSelectionArrow()
        {
            _selectionArrow.SetActive(false);
        }

        /// <summary>
        /// Handle arrow position change - invoked on FigureTeamController event
        /// </summary>
        /// <param name="selectedFigure">Currently selected figure</param>
        private void OnFigureSelectionChange(FigureController selectedFigure)
        {
            _selectionArrow.transform.position = new Vector3(selectedFigure.transform.position.x, selectedFigure.transform.position.y + 2, selectedFigure.transform.position.z);
            _figureSelectionFeedback?.PlayFeedbacks();
        }

        private void OnFigureSelectionConfirmed()
        {
            _figureConfirmationFeedback?.PlayFeedbacks();
            DisableSelectionArrow();
            NextState();
        }

        /***********************************
        * STATE - DICE_ROLL RELATED METHODS
        ************************************/
        /// <summary>
        /// Called first in dice roll state
        /// </summary>
        private void HandleDiceInteraction()
        {
            _dice.gameObject.SetActive(true);
            _dice.InitDiceSelector(!_teamPlaying.SuperDiceReady);

            _teamPlaying.HandleDiceInteraction();
        }

        private void OnLeftPressed()
        {
            if (_currentState != eGAMESTATE.DICE_ROLL)
                return;

            _dice.HandleInputLeft();
            _diceSelectionFeedback?.PlayFeedbacks();
        }

        private void OnRightPressed()
        {
            if (_currentState != eGAMESTATE.DICE_ROLL)
                return;

            _dice.HandleInputRight();
            _diceSelectionFeedback?.PlayFeedbacks();
        }

        private void OnConfirmPressed()
        {
            if (_currentState == eGAMESTATE.GAME_START)
            {
                OnTitleSignShown();
                return;
            }

            if (_currentState != eGAMESTATE.DICE_ROLL)
                return;

            //-1 returned means we only confirmed dice selection dialogue
            int diceResult = _dice.HandleInputConfirm();
            if (diceResult != -1)
            {
                if (_dice.CurrentlySelected == eDiceType.SUPERDICE)
                {
                    _teamPlaying.ResetSuperDice();
                    _UIManager.SetSuperDiceSliderBar(_teamPlaying.TeamName, 0);
                }
                _lastDiceNumber = diceResult;

                _dice.gameObject.SetActive(false);
                _diceConfirmationFeedback?.PlayFeedbacks();
                NextState();
            }
            else _diceTypeConfirmationFeedback?.PlayFeedbacks();
        }

        /***********************************
        * STATE - FIGURE_MOVEMENT RELATED METHODS
        ************************************/
        private void DoTheMovement()
        {
            if (_teamPlaying.SelectedFigure.CurrentState == eFigureState.ON_START_RAMP)
                DoTheStartMovement();
            else DoTheBoardMovement();
        }

        /// <summary>
        /// For the movement starting on start ramp - called when the figure is still on start ramp
        /// </summary>
        private void DoTheStartMovement()
        {
            _teamPlaying.DoTheStartMovement();
        }

        /// <summary>
        /// For the regular movement on the playboard
        /// </summary>
        private void DoTheBoardMovement()
        {
            _playboard.SetTileAsFree(_teamPlaying.SelectedFigure.CurrentBoardIndex);

            _goalBoardIndex = _playboard.GetNormalizedIndex(_teamPlaying.SelectedFigure.CurrentBoardIndex + _lastDiceNumber);
            _teamPlaying.SelectedFigure.GoToIndex(_goalBoardIndex, false);
            CheckTheNextTile(_teamPlaying.SelectedFigure, _teamPlaying.SelectedFigure.CurrentBoardIndex);
        }

        private void OnFigureMovementEnd(FigureController figure)
        {
            //Figure just came to the game from ramp
            if (figure.CurrentState == eFigureState.ON_START_RAMP)
            {
                _playboard.SetTileAsOccupied(_playboard.GetStartTileIndex(_teamPlaying.TeamName), figure);
                figure.CurrentBoardIndex = _playboard.GetStartTileIndex(_teamPlaying.TeamName);

                NextState();
            }
            //Figure has finished
            else if (figure.CurrentState == eFigureState.FINISHED)
            {
                _teamPlaying.NumOfMembersInportal++;
                _UIManager.SetInPortalText(_teamPlaying.TeamName, _teamPlaying.NumOfMembersInportal);

                NextState();
            }
            //typical situation - evaluate where the figure stepped up
            else
            {
                EvaluateEndTile(figure);
            }
        }

        /// <summary>
        /// Called every time the figure comes to the next waypoint(tile). Reacts to AbstractFigureTeamController events
        /// </summary>
        /// <param name="figure">Moving figure instance</param>
        /// <param name="newIndex">Current index on the playboard</param>
        private void OnFigureWaypointChange(FigureController figure, int newIndex)
        {
            //on the start ramp, we are only checking if there is figure of the other team on start index
            if (figure.CurrentState == eFigureState.ON_START_RAMP)
            {
                if (newIndex == 1)
                {
                    //Check if there is enemy figure and eventually kill it
                    TileController tile = _playboard.GetTile(_playboard.GetStartTileIndex(_teamPlaying.TeamName));
                    if (tile.AssignedFigure != null && tile.AssignedFigure.Team != _teamPlaying.TeamName)
                    {
                        figure.DoTheAttack();
                        tile.AssignedFigure.Die();
                        tile.FreeAssignFigure();
                    }
                }
            }
            //check what is one tile in front of figure
            else CheckTheNextTile(figure, newIndex);
        }

        /// <summary>
        /// Called during figure movement, check what is one tile ahead of figure.
        /// Used for attacking and jumping
        /// </summary>
        /// <param name="figure">Moving figure instance</param>
        /// <param name="currentIndex">Current index on the playboard</param>
        private void CheckTheNextTile(FigureController figure, int currentIndex)
        {
            if (currentIndex == _goalBoardIndex)
                return;

            //get tile in front of figure
            int nextRealIndex = _playboard.GetNormalizedIndex(currentIndex + 1);
            TileController tile = _playboard.GetTile(nextRealIndex);
            //attack another figure if there is one
            if (tile.AssignedFigure != null && _goalBoardIndex == nextRealIndex)
            {
                figure.DoTheAttack();
                tile.AssignedFigure.Die();
                tile.FreeAssignFigure();
            }
            //jump over if needed
            else if (!figure.IsJumping && _goalBoardIndex != nextRealIndex &&
                (tile.AssignedFigure != null || tile.TileType == eTileType.SPIKES))
                figure.DoTheJump();
        }

        /// <summary>
        /// Called on the end of the movement. Evaluates the tile where the figure stepped up.
        /// </summary>
        /// <param name="figure">Moving figure instance</param>
        private void EvaluateEndTile(FigureController figure)
        {
            TileController tile = _playboard.GetTile(figure.CurrentBoardIndex);
            //spikes - die
            if (tile.TileType == eTileType.SPIKES)
            {
                figure.Die();
                Invoke("NextState", figure.AfterDeadInitDelay);
            }
            //final spot - check if it is free and for the team of the figure
            else if (tile.TileType == eTileType.FINAL_SPOT)
            {
                FinalSpotTileController finalSpotTile = (FinalSpotTileController)tile;
                if (finalSpotTile.FinalSpot.AssignedTeam == figure.Team && !finalSpotTile.FinalSpot.Occupied)
                {
                    figure.GoToFinalSpot(finalSpotTile.FinalSpot.AssignedPath);
                    finalSpotTile.FinalSpot.SetOccupied(true);
                }
                else
                {
                    _playboard.SetTileAsOccupied(figure.CurrentBoardIndex, figure);
                    NextState();
                }
            }
            //turning again
            else if (tile.TileType == eTileType.TURN_AGAIN)
            {
                _playboard.SetTileAsOccupied(figure.CurrentBoardIndex, figure);
                _teamTurningAgain = true;
                _UIManager.ShowTurnAgainText();
                NextState();
            }
            //standard tile
            else
            {
                _playboard.SetTileAsOccupied(figure.CurrentBoardIndex, figure);
                NextState();
            }
        }
    }
}
