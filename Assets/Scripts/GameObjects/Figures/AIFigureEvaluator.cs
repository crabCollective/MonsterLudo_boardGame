using System.Collections;
using System.Collections.Generic;
using Scripts.Core;
using Scripts.Core.Input;
using Scripts.Playboard.Tiles;
using UnityEngine;

namespace Scripts.GameObjects.Figures
{
    /// <summary>
    /// Part of the AI logic used for evaluation of figure position.
    /// Scores for individual tiles are saved in AIBrainSO scriptable object
    /// </summary>
    public class AIFigureEvaluator : MonoBehaviour
    {
        private const int SCORE_MAX_MULTIPLIER = 2;

        private AIBrainSO _AIBrain;
        private GameDirector _gameDirector;
        private FigureController _assignedFigure;

        /// <summary>
        /// Score after evaluation in given turn
        /// </summary>
        private int _currentTurnScore = 0;
        public int CurrentTurnScore { get => _currentTurnScore; private set => _currentTurnScore = value; }

        /// <summary>
        /// How many steps figure has to do to reach highest value spot in front
        /// </summary>
        [SerializeField] private int _currentNeededSteps = 0;
        public int CurrentNeededSteps { get => _currentNeededSteps; private set => _currentNeededSteps = value; }

        private int _highestObtainedScore = 0;

        /// <summary>
        /// Assign references right after instantiation
        /// </summary>
        public void Initialize(GameDirector director, FigureController figure, AIBrainSO AIBrain)
        {
            _gameDirector = director;
            _assignedFigure = figure;
            _AIBrain = AIBrain;
        }

        /// <summary>
        /// Use this method every turn for evaluation of the figure position
        /// </summary>
        /// <param name="actualIndex">Actual figure index</param>
        /// <param name="superdiceActive">Is superdice active</param>
        public void Evaluate(int actualIndex, bool superdiceActive)
        {
            _currentTurnScore = 0;
            //do not evaluate figures on the start ramp
            if (_assignedFigure.CurrentState == eFigureState.ON_START_RAMP)
            {
                return;
            }

            _highestObtainedScore = 0;
            _currentNeededSteps = 0;
            int checkIndex = _gameDirector.Playboard.GetNormalizedIndex(actualIndex + 1);
            //go through all reachable tiles and create score for given figure
            for (int i = 0; i < DiceController.MAX_DICE_VALUE; i++)
            {
                TileController tile = _gameDirector.Playboard.GetTile(checkIndex);
                //first check tile type
                switch (tile.TileType)
                {
                    case eTileType.FINAL_SPOT:
                        FinalSpotTileController fsTile = (FinalSpotTileController)tile;
                        if (fsTile.FinalSpot.AssignedTeam != _assignedFigure.Team || fsTile.FinalSpot.Occupied)
                        {
                            checkIndex = _gameDirector.Playboard.GetNormalizedIndex(checkIndex + 1);
                            continue;
                        }

                        //when superdice is active, make the final spot highest priority 
                        int scoreAdded = superdiceActive ? _AIBrain.ScoreValFinalSpot * SCORE_MAX_MULTIPLIER : _AIBrain.ScoreValFinalSpot;
                        _currentTurnScore += scoreAdded;
                        CheckForCurrentNeededStepsChange(_AIBrain.ScoreValFinalSpot, i);

                        break;

                    case eTileType.TURN_AGAIN:
                        _currentTurnScore += _AIBrain.ScoreValTurnAgain;
                        CheckForCurrentNeededStepsChange(_AIBrain.ScoreValTurnAgain, i);
                        break;

                    case eTileType.SPIKES:
                        _currentTurnScore += _AIBrain.ScoreValSpikes;
                        break;

                    default: break;
                }

                //now check tile for assigned figure
                if (tile.AssignedFigure != null)
                {
                    //same team players
                    if (tile.AssignedFigure.Team == _assignedFigure.Team)
                        _currentTurnScore += _AIBrain.ScoreValSameTeamPlayer;
                    //enemy team players when not standing on their start index
                    else if (tile.AssignedFigure.CurrentBoardIndex != _gameDirector.Playboard.GetStartTileIndex(tile.AssignedFigure.Team))
                    {
                        _currentTurnScore += _AIBrain.ScoreValOtherTeamPlayer;
                        CheckForCurrentNeededStepsChange(_AIBrain.ScoreValOtherTeamPlayer, i);
                    }
                }

                checkIndex = _gameDirector.Playboard.GetNormalizedIndex(checkIndex + 1);
            }
        }

        private void CheckForCurrentNeededStepsChange(int scoreToCheck, int stepsNeeded)
        {
            if (scoreToCheck > _highestObtainedScore)
            {
                _highestObtainedScore = scoreToCheck;
                _currentNeededSteps = stepsNeeded;
            }
        }
    }
}
