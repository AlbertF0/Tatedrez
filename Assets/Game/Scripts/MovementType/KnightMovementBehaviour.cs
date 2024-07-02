
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class KnightMovementBehaviour : AMovementBehaviour
{
    public override List<Vector2Int> MovementBehaviour(Vector2Int position, Vector2Int matrixSize, List<TileController> validTiles, bool canJumpOver)
    {
        List<Vector2Int> possiblePositions = new List<Vector2Int>();

        Vector2Int[] knightMoves = new Vector2Int[]
        {
        new Vector2Int(2, 1),
        new Vector2Int(2, -1),
        new Vector2Int(-2, 1),
        new Vector2Int(-2, -1),
        new Vector2Int(1, 2),
        new Vector2Int(1, -2),
        new Vector2Int(-1, 2),
        new Vector2Int(-1, -2)
        };

        foreach (var move in knightMoves)
        {
            Vector2Int newPosition = position + move;
            if (newPosition.x >= 0 && newPosition.x < matrixSize.x && newPosition.y >= 0 && newPosition.y < matrixSize.y && validTiles.Exists(t => t.TileCoords == newPosition))
            {
                possiblePositions.Add(newPosition);
            }
        }

        return possiblePositions;
    }
}
