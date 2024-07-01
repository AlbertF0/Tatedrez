using System;
using UnityEngine;

namespace Events
{
    public class PieceReleasedEvent
    {
        public bool Valid;
        public TileView TargetTile;
        public Vector2Int CurrentTileCoords;
        public int PieceID;
        public Action<bool,TileView> Callback;
    }
}