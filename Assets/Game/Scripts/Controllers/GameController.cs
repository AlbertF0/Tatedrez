using System;
using System.Collections;
using Events;
using UnityEngine;
using System.Collections.Generic;
using Utils;
using CoroutineRunner = Utils.CoroutineRunner;
using Random = UnityEngine.Random;


public class GameController
{

    const int REPEATEABLEMOVEMENTS = 3;
    public enum Team { Player1, Player2 , None };
    public enum Result { Victory, Defeat, Draw};

    private int totalNumberOfPieces;
    private bool _placementPhase;
    private bool _gameFinishedBool;
    private Team _currentPlayer;
    private Team _waitingPlayer;
    private BoardManager _boardManager;
    private GameDataSo _gamedata;
    private EventDispatcher _eventDispatcher;
    private AIController _aiController;
    private CoroutineRunner _coroutineRunner;
    private Coroutine _coroutine;

    private EndOfDeploymentEvent _endOfDeploymentEvent = new EndOfDeploymentEvent();
    private ChangeTurnEvent _changeTurnEvent = new ChangeTurnEvent();
    private GameFinishedEvent _gameFinished = new GameFinishedEvent();
    private CantMoveEvent _cantMove = new CantMoveEvent();
    private List<MovementLog> _movementLog = new List<MovementLog>();

    public struct MovementLog
    {
        public MovementLog(Vector2Int coord, int piece, Team team)
        {
            Coord = coord;
            Piece = piece;
            Team = team;
        }

        public Vector2Int Coord;
        public int Piece;
        public Team Team;
    }

    public GameController(BoardManager boardManager, EventDispatcher eventDispatcher, GameDataSo gameData)
    {
        _coroutineRunner = ServiceLocator.GetService<CoroutineRunner>();
        eventDispatcher.AddListener<GameFinishedEvent>(GameFinishedGlobal);
        _aiController = new AIController(boardManager, gameData,this);
        _boardManager = boardManager;
        _eventDispatcher = eventDispatcher;
        _gamedata = gameData;
        totalNumberOfPieces = _gamedata.PlayerPieces.Count + _gamedata.IAPieces.Count;
       
    }

    public List<AiPieceView> GetAIPieces()
    {
        return _aiController.GetPieces();
    }

    public Team Init()
    {
        _cantMove.Callback = NextTurn;
        _placementPhase = true;
        _gameFinishedBool = false;
        _aiController.SpawnPieces();
        return ChooseStartingTeam();
    }

    private Team ChooseStartingTeam()
    {
        int randomInt = Random.Range(0, 2);
        var team = (Team[])Enum.GetValues(typeof(Team));
        _currentPlayer = team[randomInt];
        _waitingPlayer = Team.Player1;
        if (_currentPlayer == Team.Player1)
            _waitingPlayer = Team.Player2;
        return _currentPlayer;
    }

    public void StartGame()
    {
        NextTurn(_currentPlayer, _waitingPlayer);
    }

    public void NextTurn(Team currentPlayer, Team waitingPlayer)
    {
        if (_gameFinishedBool)
            return;
     
        _changeTurnEvent.Team = _currentPlayer = currentPlayer;
        _waitingPlayer = waitingPlayer;
        _eventDispatcher.Raise(_changeTurnEvent);

        if (_placementPhase && _boardManager.GetNumberOfOccupiedTiles() == totalNumberOfPieces)
        {
            _eventDispatcher.Raise(_endOfDeploymentEvent);
            _placementPhase = false;
        }

        if (currentPlayer == Team.Player1)
        {
            if(!_placementPhase && !_boardManager.CanMove())
            {
                if (!_boardManager.CanAIMove())
                {
                    GameFinished(Result.Draw);
                    return;
                }
                SendCantMoveEvent(currentPlayer,waitingPlayer);
            }
            return;
        }
        _coroutine = _coroutineRunner.StartCoroutine(this,AITurn());
    }

    public void  SendCantMoveEvent(Team currentPlayer, Team waitingPlayer)
    {
        _cantMove.PlayerWithoutMoves = currentPlayer;
        _cantMove.NextPlayer = waitingPlayer;
        _eventDispatcher.Raise(_cantMove);
    }

    private IEnumerator AITurn()
    {
        yield return new WaitForSeconds(Random.Range(0.5f,1.5f));
        if (_placementPhase)
            _aiController.PlacePiece();
        else
            _aiController.MovePiece();
    }

    public void GameFinished(Result result)
    {
        _gameFinished.Result = result;
        _boardManager.EndGameAnimations(result);
        _eventDispatcher.Raise(_gameFinished);
    }

    private void GameFinishedGlobal(GameFinishedEvent ev)
    {
        _gameFinishedBool = true;
        _coroutineRunner.StopCoroutine(this, _coroutine);
    }

    public void LogMovement(Vector2Int coord, int ID, Team team)
    {
       
        _movementLog.Add(new MovementLog(coord, ID, team));
        if (_movementLog.Count == 17)
        {
            _movementLog.RemoveAt(0);
            if (CheckForRepeatingPattern())
                GameFinished(Result.Draw);
        }
    }

    private bool CheckForRepeatingPattern()
    {
        if (patternChecker(3, 9)) // ABC ABC ABC ABC
            return true;
        if (patternChecker(4, 8)) // ABCD ABCD ABCD
            return true;
        if (patternChecker(5, 5)) // ABCDE ABCDE
            return true;
        if (patternChecker(6, 6)) // ABCDEF ABCDEF 
            return true;
        return false;
    }

    private bool patternChecker(int patternLenght, int totalLenght)
    {
        for (int i = 15; i >= 16-totalLenght; i--)
        {
            if (!MovementLogEquals(_movementLog[i], _movementLog[i - patternLenght]))
            {
                return false;
            }
        }
        return true;
    }

    private static bool MovementLogEquals(MovementLog a, MovementLog b)
    {
        return a.Coord == b.Coord && a.Piece == b.Piece && a.Team == b.Team;
    }

}
