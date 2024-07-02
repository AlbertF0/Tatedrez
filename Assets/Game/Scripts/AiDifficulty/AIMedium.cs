using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Logic AI", menuName = "ScriptableObjects/Logic AI", order = 1)]
public class AIMedium : AAILogic
{
    [SerializeField]
    private int _chanceToMiss;
 

    public override void PlacePiece(BoardManager boardManager , AiPieceView piece)
    {
    
        if(piece.name == "AIbishop(Clone)")
        {
            piece.MoveIAPiece(boardManager.GetTileByCoord(new Vector2Int(1, 0)));
        }
        if (piece.name == "AIknight(Clone)")
        {
            piece.MoveIAPiece(boardManager.GetTileByCoord(new Vector2Int(2, 1)));
        }
        if (piece.name == "AIrook(Clone)")
        {
            piece.MoveIAPiece(boardManager.GetTileByCoord(new Vector2Int(0, 0)));
        }
        
        
         // List<TileController> validTiles = boardManager.GetEmptyTiles();
         // ListUtilities.ShuffleList(validTiles);
         // TileController bestTile = null;
         // int bestScore = int.MinValue;
         // 
         // foreach (TileController tile in validTiles)
         // {
         //     int score = boardManager.EvaluateBoard(tile);
         //     if (Random.Range(0, 100) < _chanceToMiss)
         //         score = Random.Range(0, 3);
         //     if (score > bestScore)
         //     {
         //         bestScore = score;
         //         bestTile = tile;
         //         if (score == 3)
         //             break;
         //     }
         // }
         // piece.MoveIAPiece(bestTile);
        
    }

    public override bool MovePiece(List<AiPieceView> piecesSpawned, BoardManager boardManager)
    {
        ListUtilities.ShuffleList(piecesSpawned);
        List<TileController> validMovements = new List<TileController>();
        TileController bestTile = null;
        AiPieceView bestPiece = null;
        int bestScore = int.MinValue;
        foreach (AiPieceView piece in piecesSpawned)
        {
            TileController currentPieceTile = boardManager.GetTileByCoord(piece.TileCoords);
            validMovements.Clear();
            validMovements = boardManager.CheckValidIATiles(piece.TileCoords, piece.MovementType);
            ListUtilities.ShuffleList(validMovements);
            if (validMovements.Count > 0)
            {
                foreach (TileController tile in validMovements)
                {
                    int score = boardManager.EvaluateBoard(tile, currentPieceTile);
                    if (Random.Range(0, 100) < _chanceToMiss)
                        score = Random.Range(0, 3);
                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestTile = tile;
                        bestPiece = piece;
                        if (score == 3)
                            break;
                    }
                }
            }
        }
        if (bestTile != null)
        {
            bestPiece.MoveIAPiece(bestTile);
            return true;
        }
        else
            return false;
    }
}