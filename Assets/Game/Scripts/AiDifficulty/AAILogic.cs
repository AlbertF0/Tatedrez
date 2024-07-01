using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AAILogic : ScriptableObject
{
   public abstract void PlacePiece(BoardManager _boardManager , AiPieceView piece);
   public abstract bool MovePiece(List<AiPieceView> piecesSpawned, BoardManager boardManager);
}
