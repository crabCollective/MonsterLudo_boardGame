using System.Collections;
using System.Collections.Generic;
using Scripts.GameObjects.Figures;
using UnityEngine;

namespace Scripts.Playboard.Tiles
{
    public enum eTileType
    {
        STANDARD = 0,
        SPIKES = 1,
        TURN_AGAIN = 2,
        FINAL_SPOT = 3,
    }

    /// <summary>
    /// Basic class representing tile on the main path
    /// </summary>
    public class TileController : MonoBehaviour
    {
        [SerializeField] protected eTileType _tileType = eTileType.STANDARD;
        public eTileType TileType { get => _tileType; set => _tileType = value; }

        /// <summary>
        /// Reference to the figure standing on top of this tile. Null when the tile is empty.
        /// </summary>
        protected FigureController _assignedFigure;
        public FigureController AssignedFigure { get => _assignedFigure; protected set => _assignedFigure = value; }

        public void AssignFigure(FigureController figure)
        {
            _assignedFigure = figure;
        }

        public void FreeAssignFigure()
        {
            _assignedFigure = null;
        }
    }
}
