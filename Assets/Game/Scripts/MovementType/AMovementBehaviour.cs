
using System.Collections.Generic;
using UnityEngine;

public abstract class AMovementBehaviour
{   
    public abstract List<Vector2Int> MovementBehaviour(Vector2Int position, Vector2Int matrixSize, List<TileController> validTiles, bool canJumpOver);
}
