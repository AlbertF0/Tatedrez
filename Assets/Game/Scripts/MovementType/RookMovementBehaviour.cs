
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RookMovementBehaviour : AMovementBehaviour
{
    public override List<Vector2Int> MovementBehaviour(Vector2Int position, Vector2Int matrixSize, List<TileController> validTiles, bool canJumpOver)
    {
        List<Vector2Int> possiblePositions = new List<Vector2Int>();
        int x = position.x;
        int y = position.y;
        Vector2Int coords;

        for (int i = 1; i < matrixSize.x; i++)
        {
            coords = new Vector2Int(x + i, y);
            if (!canJumpOver && !validTiles.Exists(t => t.TileCoords == coords))
                break;
            if (x + i < matrixSize.x)
                possiblePositions.Add(coords);
            else
                break;
        }
         for (int i = 1; i < matrixSize.x; i++)
        {
            coords = new Vector2Int(x - i, y);
            if (!canJumpOver && !validTiles.Exists(t => t.TileCoords == coords))
                break;
            if (x - i >= 0)
                possiblePositions.Add(coords);
            else
                break;
        }
        for (int i = 1; i < matrixSize.x; i++)
        {
            coords = new Vector2Int(x, y + i);
            if (!canJumpOver && !validTiles.Exists(t => t.TileCoords == coords))
                break;
            if (y + i < matrixSize.y)
                possiblePositions.Add(coords);
            else
                break;
        }
         for (int i = 1; i < matrixSize.x; i++)
        {
            coords = new Vector2Int(x, y - i);
            if (!canJumpOver && !validTiles.Exists(t => t.TileCoords == coords))
                break;
            if (y - i >= 0)
                possiblePositions.Add(coords);
            else
                break;
        }

        return possiblePositions;
    }
}
