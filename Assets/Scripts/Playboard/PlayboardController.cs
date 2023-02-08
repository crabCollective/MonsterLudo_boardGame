using System;
using System.Collections;
using System.Collections.Generic;
using Scripts.GameObjects.Figures;
using Scripts.Playboard.Tiles;
using SWS;
using UnityEngine;

namespace Scripts.Playboard
{
    /// <summary>
    /// Class representing playboard
    /// </summary>
    public class PlayboardController : MonoBehaviour
    {
        [Tooltip("Assign all the board tiles to this list. The order must be the same as the order of the waypoints in main game path")]
        [SerializeField] private List<TileController> _tiles = new List<TileController>();
        [SerializeField] private PathManager _mainBoardPath;
        [SerializeField] private StartRampController _startRampRed;
        [SerializeField] private StartRampController _startRampBlue;
        [Tooltip("Index of the start on main board path for the RED team")]
        [SerializeField] private int _startTileIndexRed;

        [Tooltip("Index of the start on main board path for the BLUE team")]
        [SerializeField] private int _startTileIndexBlue;

        public PathManager MainBoardPath { get => _mainBoardPath; set => _mainBoardPath = value; }

        public TileController GetTile(int index)
        {
            TileController tile = null;
            try
            {
                tile = _tiles[index];
            }
            catch (Exception e)
            {
                Debug.LogError("Error: Cannot get playboard tile on index " + index);
                Debug.LogError("Exception: " + e.Message);
            }

            return tile;
        }

        public void SetTileAsOccupied(int index, FigureController figure)
        {
            TileController tile = GetTile(index);
            tile.AssignFigure(figure);
        }

        public void SetTileAsFree(int index)
        {
            TileController tile = GetTile(index);
            tile.FreeAssignFigure();
        }

        public StartRampController GetStartRamp(eTeamName teamName)
        {
            return teamName == eTeamName.RED ? _startRampRed : _startRampBlue;
        }

        public int GetStartTileIndex(eTeamName teamName)
        {
            return teamName == eTeamName.RED ? _startTileIndexRed : _startTileIndexBlue;
        }

        public TileController GetNextTile(int currentIndex)
        {
            return _tiles[GetNormalizedIndex(currentIndex + 1)];
        }

        public int GetNormalizedIndex(int targetIndex)
        {
            return targetIndex <= _tiles.Count - 1 ? targetIndex : targetIndex - _tiles.Count;
        }
    }
}