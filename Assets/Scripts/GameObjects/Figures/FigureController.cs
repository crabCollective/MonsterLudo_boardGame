using System;
using System.Collections;
using System.Collections.Generic;
using SWS;
using UnityEngine;
using MoreMountains.Feedbacks;
using Scripts.Playboard;

namespace Scripts.GameObjects.Figures
{
    public enum eFigureState
    {
        ON_START_RAMP,
        IN_GAME,
        FINISHED
    }

    /// <summary>
    /// Basic class for in-game figures
    /// </summary>
    public class FigureController : MonoBehaviour
    {
        [SerializeField] private eTeamName _team;
        public eTeamName Team { get => _team; private set => _team = value; }

        private eFigureState _currentState;
        public eFigureState CurrentState { get => _currentState; private set => _currentState = value; }

        private int _currentBoardIndex = -1;
        public int CurrentBoardIndex { get => _currentBoardIndex; set => _currentBoardIndex = value; }

        private int _targetBoardIndex = -1;
        public int TargetBoardIndex { get => _targetBoardIndex; set => _targetBoardIndex = value; }

        [SerializeField] private float _afterDeadInitDelay = 3f;
        public float AfterDeadInitDelay { get => _afterDeadInitDelay; set => _afterDeadInitDelay = value; }

        [SerializeField] private float _afterAttackDelay = 1.5f;
        [SerializeField] private float _afterJumpDelay = 0.5f;

        [SerializeField] private AIFigureEvaluator _AIEvaluator;
        public AIFigureEvaluator AIEvaluator { get => _AIEvaluator; private set => _AIEvaluator = value; }

        [Space(5)]
        [SerializeField] private MMF_Player _idleFeedback;
        [SerializeField] private MMF_Player _walkFeedback;
        [SerializeField] private MMF_Player _jumpFeedback;
        [SerializeField] private MMF_Player _attackFeedback;
        [SerializeField] private MMF_Player _deadFeedback;
        [SerializeField] private MMF_Player _startAgainFeedback;

        public event Action<FigureController> onCurrentMoveEnd;
        public event Action<FigureController> onDeathEvent;
        public event Action<FigureController> onFinishReachedEvent;
        public event Action<FigureController, int> onWaypointReached;

        private splineMove _splineMoveController;
        private bool _wasOnStart = true;
        private int _lastVisitedIndex = -1;
        //start point on the start ramp
        private FigureStartPoint _assignedStartPoint;
        private bool _isAttacking = false;

        private bool _isJumping = false;
        public bool IsJumping { get => _isJumping; private set => _isJumping = value; }

        private void Awake()
        {
            _splineMoveController = GetComponent<splineMove>();
            Debug.Assert(_splineMoveController);
        }

        private void OnEnable()
        {
            _splineMoveController.movementChangeEvent += OnWaypointReached;
        }

        private void OnDisable()
        {
            _splineMoveController.movementChangeEvent -= OnWaypointReached;
        }

        void Start()
        {
            _currentState = eFigureState.ON_START_RAMP;
        }

        public void Init(eTeamName teamName, AIFigureEvaluator AIevaluator = null)
        {
            _team = teamName;
            _AIEvaluator = AIevaluator;
        }

        public void AssignStartPoint(FigureStartPoint startPoint)
        {
            _assignedStartPoint = startPoint;
        }

        /// <summary>
        /// Go to the main board start tile from start ramp
        /// </summary>
        /// <param name="path">Path leading to the main board</param>
        public void GoToStart(PathManager path)
        {
            AssignPath(path);

            _lastVisitedIndex = -1;
            _targetBoardIndex = path.waypoints.Length - 1;
            _splineMoveController.startPoint = 0;
            _splineMoveController.StartMove();

            _walkFeedback?.PlayFeedbacks();
        }

        /// <summary>
        /// Go to given index on math playboard path
        /// </summary>
        /// <param name="index">Where to go</param>
        /// <param name="resumingAfterAttack">Is this called after resuming from attack?</param>
        public void GoToIndex(int index, bool resumingAfterAttack)
        {
            _lastVisitedIndex = -1;
            _targetBoardIndex = index;
            if (_wasOnStart && !resumingAfterAttack)
            {
                _wasOnStart = false;
                _splineMoveController.StartMove();
            }
            else _splineMoveController.Resume();

            _walkFeedback?.PlayFeedbacks();
        }

        /// <summary>
        /// Called when the figure is going to the final spot
        /// </summary>
        /// <param name="path">Path to final spot</param>
        public void GoToFinalSpot(PathManager path)
        {
            AssignPath(path);

            _currentState = eFigureState.FINISHED;
            _lastVisitedIndex = -1;
            _targetBoardIndex = path.waypoints.Length - 1;
            _splineMoveController.StartMove();
            ChangeToNonLoopMovement();

            _walkFeedback?.PlayFeedbacks();

            onFinishReachedEvent?.Invoke(this);
        }

        public void SetToInGame(PathManager path, int pathStartIndex)
        {
            _wasOnStart = true;
            AssignPath(path);
            _splineMoveController.moveToPath = false;
            _splineMoveController.startPoint = pathStartIndex;
            _currentState = eFigureState.IN_GAME;

            StartCoroutine(ChangeToLoopMovementDelayed());
        }

        private IEnumerator ChangeToLoopMovementDelayed()
        {
            yield return null;
            _splineMoveController.loopType = splineMove.LoopType.loop;
            _splineMoveController.closeLoop = true;
        }

        private void ChangeToNonLoopMovement()
        {
            _splineMoveController.loopType = splineMove.LoopType.none;
            _splineMoveController.closeLoop = false;
        }

        public void DoTheJump()
        {
            _jumpFeedback?.PlayFeedbacks();
            _isJumping = true;

            Invoke("AfterJump", _afterJumpDelay);
        }

        private void AfterJump()
        {
            _isJumping = false;
        }

        public void DoTheAttack()
        {
            _isAttacking = true;
            _splineMoveController.Pause();
            _attackFeedback?.PlayFeedbacks();

            Invoke("AfterAttack", _afterAttackDelay);
        }

        private void AfterAttack()
        {
            _isAttacking = false;
            GoToIndex(_targetBoardIndex, true);
        }

        public void Die()
        {
            _splineMoveController.Stop();
            _splineMoveController.pathContainer = null;

            _deadFeedback?.PlayFeedbacks();
            onDeathEvent?.Invoke(this);

            Invoke("ReturnBackToStart", _afterDeadInitDelay);
        }

        /// <summary>
        /// Called after death - returning back to the start ramp
        /// </summary>
        public void ReturnBackToStart()
        {
            transform.position = _assignedStartPoint.StartPos.position;
            transform.rotation = _assignedStartPoint.StartPos.rotation;

            _lastVisitedIndex = -1;
            ChangeToNonLoopMovement();
            _splineMoveController.moveToPath = true;
            _currentState = eFigureState.ON_START_RAMP;

            _startAgainFeedback?.PlayFeedbacks();
        }

        /// <summary>
        /// Assign path to spline script
        /// </summary>
        /// <param name="path"></param>
        private void AssignPath(PathManager path)
        {
            _splineMoveController.pathContainer = path;
        }

        private void OnWaypointReached(int index)
        {
            //SWS script starts to move again when loop path is reached, so we need to do this workaround to:
            //1) prevent onWaypointReached event being called twice
            //2) make sure player stops when needed
            if (_lastVisitedIndex == index)
            {
                if (_targetBoardIndex == index || _isAttacking)
                    _splineMoveController.Pause();

                return;
            }

            _lastVisitedIndex = index;
            _currentBoardIndex = index;
            onWaypointReached?.Invoke(this, index);

            if (_targetBoardIndex == index)
            {
                _splineMoveController.Pause();
                onCurrentMoveEnd?.Invoke(this);

                _idleFeedback?.PlayFeedbacks();
            }

        }
    }
}
