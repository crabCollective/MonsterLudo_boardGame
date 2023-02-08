using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Scripts.Core.Input;
using Scripts.Playboard.Tiles;
using UnityEngine;

namespace Scripts.GameObjects.Figures
{
    /// <summary>
    /// Class representing AI-controlled team of figures. Includes one part of the AI logic.
    /// Another part is represented by AIFigureEvaluator class
    /// </summary>
    public class AIFigureTeamController : AbstractFigureTeamController
    {
        [SerializeField] private AIFigureEvaluator _AIEvaluatorPrefab;
        [SerializeField] private AIBrainSO _usedAIBrain;
        [SerializeField] private List<FigureController> _highestScoreFigures;
        [SerializeField] private float _diceStopDelay = 0.5f;
        [SerializeField] private float _superDiceSelectionDelay = 0.3f;
        private int _turnHighestScore;

        protected override void Awake()
        {
            base.Awake();
            //instantiate AI evaluator on awake
            foreach (FigureController figure in _teamMembers)
            {
                GameObject AIEvaluator = Instantiate(_AIEvaluatorPrefab.gameObject, figure.transform);
                AIFigureEvaluator ai = AIEvaluator.GetComponent<AIFigureEvaluator>();
                ai.Initialize(_gameDirector, figure, _usedAIBrain);

                figure.Init(_teamName, ai);
            }
        }

        /// <summary>
        /// AI logic for figure selection
        /// </summary>
        public override void ChooseFigureToTurnWith()
        {
            GetFiguresAvailableForSelection();

            //Evaluate all the selectable figures by AIEvaluator - higher score means higher priority
            _turnHighestScore = System.Int32.MinValue;
            foreach (FigureController figure in _availableForSelection)
            {
                figure.AIEvaluator.Evaluate(figure.CurrentBoardIndex, _superDiceReady);
                if (figure.AIEvaluator.CurrentTurnScore > _turnHighestScore)
                    _turnHighestScore = figure.AIEvaluator.CurrentTurnScore;
            }

            //Highest priority - if there is enemy on the start tile, always kick him off
            TileController startTile = _gameDirector.Playboard.GetTile(_gameDirector.Playboard.GetStartTileIndex(_teamName));
            if (_canSelectFromTheRamp && (startTile.AssignedFigure != null && startTile.AssignedFigure.Team != _teamName))
            {
                _selectedFigure = _figureFromTheRamp;
            }
            //This part selects figure with the highest priority. Used when score is very high, when superdice is ready, 
            //when enough figures are on the board or when there are no figures on the ramp.
            else if (_superDiceReady || _turnHighestScore >= _usedAIBrain.HighPriorityScore
            || _figuresInGame.Count >= _usedAIBrain.NumOfPrefferedPlayersIngame || !_canSelectFromTheRamp)
            {
                //always get list of all the figures with the highest priority and then choose randomly from them
                _highestScoreFigures = _availableForSelection.Where(item => item.AIEvaluator.CurrentTurnScore == _turnHighestScore).ToList();
                _selectedFigure = _highestScoreFigures[Random.Range(0, _highestScoreFigures.Count)];
            }
            //else try to put as much figures to the game as possible
            else
            {
                _selectedFigure = _figureFromTheRamp;
            }
            InvokeOnFigureSelectionConfirmed();
        }

        /// <summary>
        /// Simple logic for using/not using superdice. Simulates player input
        /// </summary>
        public override void HandleDiceInteraction()
        {
            //Use super dice if available
            if (_superDiceReady)
            {
                //But do not use it when the figures has low score - happening rarely
                if (_turnHighestScore <= _usedAIBrain.LowPriorityScore)
                {
                    //1- confirm selection of regular dice, 2 - stop dice after chosen time
                    InvokeOnConfirmPressedEvent();
                    Invoke("LateOnConfirmInvoke", _diceStopDelay);

                    return;
                }

                //1 - push right, 2-confirm superdice selection, 3-slowly select needed number
                InvokeOnRightPressedEvent();
                InvokeOnConfirmPressedEvent();
                if (_selectedFigure.AIEvaluator.CurrentTurnScore >= 1)
                {
                    //if the figure has higher score (0 is default), make sure we are going straight to the needed index
                    StartCoroutine(SuperDiceSelectionCoroutine(_selectedFigure.AIEvaluator.CurrentNeededSteps));
                }
                else
                {
                    //in case when no figure has good position just select 6
                    StartCoroutine(SuperDiceSelectionCoroutine(DiceController.MAX_DICE_VALUE - 1));
                }
            }
            //Regular dice - stop dice after chosen time
            else Invoke("LateOnConfirmInvoke", _diceStopDelay);
        }

        private void LateOnConfirmInvoke()
        {
            InvokeOnConfirmPressedEvent();
        }

        private IEnumerator SuperDiceSelectionCoroutine(int neededNumber)
        {
            for (int i = 1; i <= neededNumber; i++)
            {
                InvokeOnRightPressedEvent();
                yield return new WaitForSeconds(_superDiceSelectionDelay);
            }
            InvokeOnConfirmPressedEvent();
        }
    }
}
