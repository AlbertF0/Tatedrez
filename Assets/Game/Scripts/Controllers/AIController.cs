using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIController
{
    private GameController _gameController;
    private BoardManager _boardManager;
    private GameDataSo _gamedata;
    private List<GameObject> _piecesToSpawn;
    private List<AiPieceView> _piecesSpawned;
    private AAILogic _logic;

    public AIController(BoardManager boardManager, GameDataSo gameData, GameController gameController)
    {
        _logic = gameData.AILogic;
        _boardManager = boardManager;
        _gamedata = gameData;
        _gameController = gameController;
        _piecesToSpawn = new List<GameObject>(gameData.IAPieces);
        _piecesSpawned = new List<AiPieceView>(gameData.IAPieces.Count);
    }

    public List<AiPieceView> GetPieces()
    {
        return _piecesSpawned;
    }

    public void SpawnPieces()
    {
        _boardManager.SpawnPiece(_piecesToSpawn, _piecesSpawned);
    }

    public void PlacePiece()
    {
        List<AiPieceView> pieceBehaviours = _piecesSpawned.FindAll(w => w.TileCoords.x == -1);
        _logic.PlacePiece(_boardManager, pieceBehaviours[UnityEngine.Random.Range(0,pieceBehaviours.Count)]);
    }

    public void MovePiece()
    {
        if (_logic.MovePiece(_piecesSpawned, _boardManager))
            return;
        _gameController.SendCantMoveEvent(GameController.Team.Player2, GameController.Team.Player1);
    }
}
