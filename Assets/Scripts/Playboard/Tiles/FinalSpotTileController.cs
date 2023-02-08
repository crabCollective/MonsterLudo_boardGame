
using UnityEngine;

namespace Scripts.Playboard.Tiles
{
    public class FinalSpotTileController : TileController
    {
        [SerializeField] private FinalSpotController _finalSpot;
        public FinalSpotController FinalSpot { get => _finalSpot; private set => _finalSpot = value; }
    }
}
