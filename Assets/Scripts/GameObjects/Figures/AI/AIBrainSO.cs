using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Core.Input
{
    [CreateAssetMenu(fileName = "AIBrainSO", menuName = "Game/AI_Brain")]
    public class AIBrainSO : ScriptableObject
    {
        [Header("General AI rules")]
        [Tooltip("At which score should player always turn with figure?")]
        [SerializeField] private int _highPriorityScore = 4;
        public int HighPriorityScore { get => _highPriorityScore; private set => _highPriorityScore = value; }

        [Tooltip("At which score should player shouldnt use superdice?")]
        private int _lowPriorityScore = -2;
        public int LowPriorityScore { get => _lowPriorityScore; private set => _lowPriorityScore = value; }

        [Tooltip("How many players should AI try to put in the game?")]
        private int _numOfPreferredPlayersIngame = 3;
        public int NumOfPrefferedPlayersIngame { get => _numOfPreferredPlayersIngame; private set => _numOfPreferredPlayersIngame = value; }

        [Header("Scores used by AI evaluator")]
        [SerializeField] private int _scoreValFinalSpot = 3;
        public int ScoreValFinalSpot { get => _scoreValFinalSpot; private set => _scoreValFinalSpot = value; }

        [SerializeField] private int _scoreValOtherTeamPlayer = 2;
        public int ScoreValOtherTeamPlayer { get => _scoreValOtherTeamPlayer; private set => _scoreValOtherTeamPlayer = value; }

        [SerializeField] private int _scoreValTurnAgain = 1;
        public int ScoreValTurnAgain { get => _scoreValTurnAgain; private set => _scoreValTurnAgain = value; }

        [SerializeField] private int _scoreValSpikes = -2;
        public int ScoreValSpikes { get => _scoreValSpikes; private set => _scoreValSpikes = value; }

        [SerializeField] private int _scoreValSameTeamPlayer = -2;
        public int ScoreValSameTeamPlayer { get => _scoreValSameTeamPlayer; private set => _scoreValSameTeamPlayer = value; }
    }
}
