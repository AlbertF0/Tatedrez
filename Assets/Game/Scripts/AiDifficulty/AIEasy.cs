using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Random AI", menuName = "ScriptableObjects/Random AI", order = 1)]
public class AIEasy : AAILogic
{
    public override void PlacePiece(BoardManager boardManager , AiPieceView piece)
    {
        List<TileController> validTiles = boardManager.GetEmptyTiles();
        piece.MoveIAPiece(validTiles[UnityEngine.Random.Range(0, validTiles.Count)]);
    }

    public override bool MovePiece(List<AiPieceView> piecesSpawned, BoardManager boardManager)
    {
        ListUtilities.ShuffleList(piecesSpawned);
        List<TileController> validMovements = new List<TileController>();
        foreach (AiPieceView piece in piecesSpawned)
        {
            validMovements.Clear();
            validMovements = boardManager.CheckValidIATiles(piece.TileCoords, piece.MovementType);
            ListUtilities.ShuffleList(validMovements);
            if (validMovements.Count > 0)
            {

                piece.MoveIAPiece(validMovements[UnityEngine.Random.Range(0, validMovements.Count)]);
                return true;
            }
        }
        return false;
    }
}
