using UnityEngine;

namespace Events
{
    public class IAPieceReleasedEvent
    {
        public TileView TargetTile;
        public int PieceID;
        public Vector2Int CurrentTileCoords;
    }
}