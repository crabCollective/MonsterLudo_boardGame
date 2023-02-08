using System;
using System.Collections;
using System.Collections.Generic;
using Scripts.GameObjects.Figures;
using SWS;
using UnityEngine;

namespace Scripts.Playboard
{
    [Serializable]
    public struct FigureStartPoint
    {
        public FigureController AssignedFigure;
        public Transform StartPos;
    }

    /// <summary>
    /// Class representing start ramp. Assign figures in the correct order from the editor
    /// </summary>
    public class StartRampController : MonoBehaviour
    {
        [SerializeField] private List<FigureStartPoint> _startPoints;
        [SerializeField] private PathManager _pathToMainBoard;
        public PathManager PathToMainBoard { get => _pathToMainBoard; private set => _pathToMainBoard = value; }

        private void Start()
        {
            //Assign start points to the figures on the start
            foreach (FigureStartPoint startPoint in _startPoints)
            {
                if (startPoint.AssignedFigure != null)
                    startPoint.AssignedFigure.AssignStartPoint(startPoint);
            }
        }

        /// <summary>
        /// Use this method to find the figure closest to the ramp exit.
        /// </summary>
        /// <returns></returns>
        public FigureStartPoint? GetFirstOccupiedPosition()
        {
            foreach (FigureStartPoint startPoint in _startPoints)
            {
                if (startPoint.AssignedFigure != null
                && startPoint.AssignedFigure.CurrentState == eFigureState.ON_START_RAMP)
                {
                    return startPoint;
                }
            }

            return null;
        }
    }
}
