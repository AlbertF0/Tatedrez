using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DG.Tweening;
using Events;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

public class BoardManager : MonoBehaviour
{
    private const float TILESIZE = 1; // WE WONT GO THAT FAR SO FOR NOW IS A CONST
    private const int MINSIZE = 2;


    [SerializeField]
    private Transform _tilesContainer;
    [SerializeField]
    private Transform _edgesContainer;
    [SerializeField]
    private TileView _tile;
    [SerializeField]
    private GameObject _tileEdge;
    [SerializeField]
    private GameObject _tileCorner;
    [SerializeField]
    private Camera _camera;


    private GameDataSo _gameData;
    private Vector2Int _boardSize = new Vector2Int(3, 3);  

    private bool _deploymentPhase;
    private List<TileController> _validTiles = new List<TileController>();
    private TileController[,] _tileControllers;
    private GameController _gameController;
    private List<PlayerPieceView> _playerPieceViews = new List<PlayerPieceView>();

    private EventDispatcher _eventDispatcher;

    private void Awake()
    {
        if(ServiceLocator.GetService<GameConfigService>()==null)
            SceneManager.LoadScene("PersistentLogicScene");
    }

    void Start()
    {
        _gameData = ServiceLocator.GetService<GameConfigService>().GameData;
        _eventDispatcher = ServiceLocator.GetService<EventDispatcher>();
        _eventDispatcher.AddListener<PieceSelectedEvent>(PlayerHighlightValidTiles);
        _eventDispatcher.AddListener<PieceReleasedEvent>(ValidateMovement);
        _eventDispatcher.AddListener<IAPieceReleasedEvent>(PerformIaMovement);
        _eventDispatcher.AddListener<EndOfDeploymentEvent>(FinishDeploymentPhase);
        _eventDispatcher.AddListener<PlayButtonClickedEvent>(StartGame);
        _eventDispatcher.AddListener<CleanBoardEvent>(CleanBoard);

        InsantiateEdges();
        SetupCamera();
    }
    
    private void StartGame(PlayButtonClickedEvent ev)
    {
        float timeToFinishAnimations = (_boardSize.x * _boardSize.y * 0.05f) + 0.8f + _playerPieceViews.Count / 5f;
        _deploymentPhase = true;
        _gameController = new GameController(this, _eventDispatcher, _gameData);
        InitializeTiles();
        GameController.Team starting = _gameController.Init();
        InitializePieces(starting);
        AnimatePieces();
        StartCoroutine(WaitForAnimationsToFinish(timeToFinishAnimations));
    }

    #region Anmations and Initializations

    private void SetupCamera()
    {
        float cameraVerticalOffset = Mathf.Max((_boardSize.x - MINSIZE) * 2, (_boardSize.y - MINSIZE - 2) * 2) + 7;
        float cameraYOffset = _boardSize.y + 2;
        _camera.transform.position = new Vector3(_boardSize.x / 2f - TILESIZE / 2, cameraVerticalOffset, (_boardSize.y / 2f - TILESIZE / 2) - cameraYOffset);
    }

    private void CleanBoard(CleanBoardEvent ev)
    {
        for (int i = _tilesContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(_tilesContainer.GetChild(i).gameObject);
        }
        for (int i = _playerPieceViews.Count - 1; i >= 0; i--)
        {
            Destroy(_playerPieceViews[i].gameObject);
        }
        _playerPieceViews.Clear();
        var aiPieces = _gameController.GetAIPieces();
        for (int i = aiPieces.Count - 1; i >= 0; i--)
        {
            Destroy(aiPieces[i].gameObject);
        }
    }

    private void InitializePieces(GameController.Team starting)
    {
        float center = _boardSize.x / 2f - TILESIZE / 2;
        float distanceFromBoard = -0.9f;
        float yOffset = -0.2f;

        for (int i = 0; i < _gameData.PlayerPieces.Count; i++)
        {
            float xPos = center + (i - (_gameData.PlayerPieces.Count - 1) / 2f);
            PlayerPieceView pb = Instantiate(_gameData.PlayerPieces[i], new Vector3(xPos, yOffset, distanceFromBoard), Quaternion.Euler(0, -90, 0), _tilesContainer)
                .GetComponent<PlayerPieceView>();
            _playerPieceViews.Add(pb);
            pb.Initialize(Vector2Int.one * -1, starting);
        }
    }

    private void InitializeTiles()
    {
        _tileControllers = new TileController[_boardSize.x, _boardSize.y];
        Material startingMaterial = _gameData.Color1;
        int index = 0;
        for (int x = 0; x < _boardSize.x; x++)
        {
            Material material = startingMaterial;
            for (int z = 0; z < _boardSize.y; z++)
            {
                index++;
                material = material == _gameData.Color1 ? material = _gameData.Color2 : material = _gameData.Color1;
                TileView tile = Instantiate(_tile, new Vector3(x, 0, z), Quaternion.identity, _tilesContainer);
                tile.Initialize(new Vector2Int(x, z), material, index);
                _tileControllers[x, z] = new TileController(x, z, tile);
            }
            startingMaterial = startingMaterial == _gameData.Color1 ? startingMaterial = _gameData.Color2 : startingMaterial = _gameData.Color1;
        }
    }

    private void InsantiateEdges()
    {

        _boardSize = _gameData.BoardSize;
        _boardSize.x = Mathf.Max(MINSIZE, _boardSize.x);
        _boardSize.y = Mathf.Max(MINSIZE, _boardSize.y);
        for (int x = 0; x < _boardSize.x; x++)
        {
            for (int z = 0; z < _boardSize.y; z++)
            {
                if (x == 0)
                {
                    if (z == 0)
                    {
                        Instantiate(_tileCorner, new Vector3(x - 1, -1, z - 1), Quaternion.Euler(0, 180, 0), _edgesContainer);
                    }
                    else if (z == _boardSize.y - 1)
                    {
                        Instantiate(_tileCorner, new Vector3(x - 1, -1, z + 1), Quaternion.Euler(0, -90, 0), _edgesContainer);
                    }
                    else
                    {
                        Instantiate(_tileEdge, new Vector3(x - 1, -1, z), Quaternion.Euler(0, -90, 0), _edgesContainer);
                    }
                }
                else if (x == _boardSize.x - 1)
                {
                    if (z == 0)
                    {
                        Instantiate(_tileCorner, new Vector3(x + 1, -1, z - 1), Quaternion.Euler(0, 90, 0), _edgesContainer);
                    }
                    else if (z == _boardSize.y - 1)
                    {
                        Instantiate(_tileCorner, new Vector3(x + 1, -1, z + 1), Quaternion.Euler(0, 0, 0), _edgesContainer);
                    }
                    else
                    {
                        Instantiate(_tileEdge, new Vector3(x + 1, -1, z), Quaternion.Euler(0, 90, 0), _edgesContainer);
                    }
                }
                else if (z == 0)
                {
                    Instantiate(_tileEdge, new Vector3(x, -1, z - 1), Quaternion.Euler(0, 180, 0), _edgesContainer);
                }
                else if (z == _boardSize.y - 1)
                {
                    Instantiate(_tileEdge, new Vector3(x, -1, z + 1), Quaternion.Euler(0, 0, 0), _edgesContainer);
                }
            }
        }
    }

    private void AnimatePieces()
    {
        int index = 0;
        foreach(var piece in _playerPieceViews)
        {
            piece.SpawnAnimation((_boardSize.x * _boardSize.y * 0.05f) + 0.8f + index / 5f);
            index++;
        }
        index = 0;
        foreach(var piece in _gameController.GetAIPieces())
        {
            piece.SpawnAnimation((_boardSize.x * _boardSize.y * 0.05f) + 0.8f + index / 5f);
            index++;
        }
    }

    private IEnumerator WaitForAnimationsToFinish(float t)
    {
        yield return new WaitForSeconds(1 + t);
        _eventDispatcher.Raise<BoardFinishedAnimationEvent>();
        yield return new WaitForSeconds(3);
        foreach (PlayerPieceView piece in _playerPieceViews)
        {
            piece.GameStarts();
        }
        _gameController.StartGame();
    }

    public void EndGameAnimations(GameController.Result result)
    {
        GameController.Team team = GameController.Team.None;
        int i = 0;
        switch (result)
        {
            case GameController.Result.Victory:
                team = GameController.Team.Player1;
                foreach (AiPieceView piece in _gameController.GetAIPieces())
                {
                    piece.DespawnAnimation(i);
                    i++;
                }
                foreach (PlayerPieceView piece in _playerPieceViews)
                {
                    piece.VictoryAnimation();
                    i++;
                }
                break;
            case GameController.Result.Defeat:
                team = GameController.Team.Player2;
                foreach (PlayerPieceView piece in _playerPieceViews)
                {
                    piece.DespawnAnimation(i);
                    i++;
                }
                foreach (AiPieceView piece in _gameController.GetAIPieces())
                {
                    piece.VictoryAnimation();
                    i++;
                }
                break;
            case GameController.Result.Draw:
                team = GameController.Team.None;
                foreach (PlayerPieceView piece in _playerPieceViews)
                {
                    piece.DespawnAnimation(i);
                    i++;
                }
                foreach (AiPieceView piece in _gameController.GetAIPieces())
                {
                    piece.DespawnAnimation(i);
                    i++;
                }
                break;
        }
        i = 0;
        foreach (TileController tileController in _tileControllers)
        {
            if(tileController.Team != team || tileController.Team == GameController.Team.None )
            {
                tileController.View.PlayFallAnimation(i);
                i++;
            }
            
        }
    }

    #endregion

    #region logic

    public TileController GetTileByCoord(Vector2Int coords)
    {
        return _tileControllers[coords.x, coords.y];
    }

    public bool CanMove()
    {
        List<TileController> validTiles = GetEmptyTiles();
        foreach (PlayerPieceView pp in _playerPieceViews)
        {
            List<TileController> validMovementTiles = new List<TileController>();
            var movementValidTiles = pp.MovementType.GetValidPositions(pp.TileCoords, _boardSize, validTiles);
            if(movementValidTiles.Count > 0)
            {
                return true;
            }
                
        }
        return false;
    }

    public void  PlayerHighlightValidTiles(PieceSelectedEvent ev)
    {
        _validTiles = new List<TileController>(CheckValidTiles(ev.PieceBehaviour));
        HighlightValidTiles(_validTiles);
    }

    private List<TileController> CheckValidTiles(PlayerPieceView pb)
    {
        var validTiles = GetEmptyTiles();
        if (_deploymentPhase)
        {
            return validTiles;
        }
        else
        {
            List<TileController> _validMovementTiles = new List<TileController>();
            var _movementValidTiles = pb.MovementType.GetValidPositions(pb.TileCoords, _boardSize, validTiles);
            foreach (var _movement in _movementValidTiles)
            {
                _validMovementTiles.Add(_tileControllers[_movement.x, _movement.y]);
            }
            validTiles = validTiles.Intersect(_validMovementTiles).ToList();
        }
        return validTiles;
    }

    public List<TileController> GetEmptyTiles()
    {
        List<TileController> validTiles = new List<TileController>();
        for (int x = 0; x < _boardSize.x; x++)
        {
            for (int z = 0; z < _boardSize.y; z++)
            {
                if (_tileControllers[x, z].Team == GameController.Team.None)
                    validTiles.Add(_tileControllers[x, z]);
            }
        }
        return validTiles;
    }

    public int GetNumberOfOccupiedTiles()
    {
        int i = 0;
        for (int x = 0; x < _boardSize.x; x++)
        {
            for (int z = 0; z < _boardSize.y; z++)
            {
                if (_tileControllers[x, z].Team != GameController.Team.None)
                    i++;
            }
        }
        return i;
    }

    private void ValidateMovement(PieceReleasedEvent ev)
    {
        if (!ev.Valid)
        {
            ev.Callback.Invoke(false, ev.TargetTile);
            UnHighlightTiles(_validTiles);
            _validTiles.Clear();
            return;
        }

        TileController targetTileController = _validTiles.Find(c => c.View == ev.TargetTile);
        bool valid = targetTileController != null;
        ev.Callback.Invoke(valid, ev.TargetTile);

        if (valid)
        {
            if (!_deploymentPhase)
                _tileControllers[ev.CurrentTileCoords.x, ev.CurrentTileCoords.y].TakeTile(GameController.Team.None);
            targetTileController.TakeTile(GameController.Team.Player1);
            _gameController.LogMovement(ev.TargetTile.Coords, ev.PieceID, GameController.Team.Player1);
            if (CheckForConsecutives(GameController.Team.Player1))
            {
                _gameController.GameFinished(GameController.Result.Victory);
            }
            else
            {
                _gameController.NextTurn(GameController.Team.Player2, GameController.Team.Player1);
            }
           
        }

        UnHighlightTiles(_validTiles);
        _validTiles.Clear();
    }

    private bool CheckForConsecutives(GameController.Team teamToCheck)
    {
        int consecutivePiecesToWin = _gameData.ConsecutivePiecesToWin;
        int consecutives = 0;
        // we could add a boolean to make sure we dont do redundant searches BUT if we want to make a bigger board with more pieces and still check for 3 in a row that would be a problem.
        //Check Vertical
        for(int x = 0; x < _boardSize.x; x++)
        {
            consecutives = 0;
            for (int y = 0; y < _boardSize.y; y++)
            {
                if (_tileControllers[x, y].Team == teamToCheck)
                {
                    consecutives++;
                    if (consecutives == consecutivePiecesToWin)
                        return true;
                }
                else if (consecutives > 0)
                    consecutives = 0;
            }
        }
        //Check Horizontal
        for (int y = 0; y < _boardSize.y; y++)
        {
            consecutives = 0;
            for (int x = 0; x < _boardSize.x; x++)
            {
                if (_tileControllers[x, y].Team == teamToCheck)
                {
                    consecutives++;
                    if (consecutives == consecutivePiecesToWin)
                        return true;
                }
                else if (consecutives > 0)
                    consecutives = 0;
            }
        }
        // diagonal left up - right down
        for (int x = 0; x < _boardSize.x - (consecutivePiecesToWin - 1); x++)
        {
            for (int y = 0; y < _boardSize.y - (consecutivePiecesToWin - 1); y++)
            {
                consecutives = 0;
                for (int i = 0; i < consecutivePiecesToWin; i++)
                {
                    if (_tileControllers[x + i, y + i].Team == teamToCheck)
                    {
                        consecutives++;
                        if (consecutives == consecutivePiecesToWin)
                            return true;
                    }
                    else
                    {
                        consecutives = 0;
                    }
                }
            }
        }
        // diagonal left down - right up
        for (int x = 0; x < _boardSize.x - (consecutivePiecesToWin - 1); x++)
        {
            for (int y = consecutivePiecesToWin - 1; y < _boardSize.y; y++)
            {
                consecutives = 0;
                for (int i = 0; i < consecutivePiecesToWin; i++)
                {
                    if (_tileControllers[x + i, y - i].Team == teamToCheck)
                    {
                        consecutives++;
                        if (consecutives == consecutivePiecesToWin)
                            return true;
                    }
                    else
                    {
                        consecutives = 0;
                    }
                }
            }
        }

        return false;
    }

    private void HighlightValidTiles(List<TileController> _validTiles)
    {
        foreach (var _tileController in _validTiles)
        {
            _tileController.View.ToggleHighlightTile(true);
        }
    }

    private void UnHighlightTiles(List<TileController> _validTiles)
    {
        foreach (var _tileController in _validTiles)
        {
            _tileController.View.ToggleHighlightTile(false);
        }
    }

    public void FinishDeploymentPhase(EndOfDeploymentEvent ev)
    {
        _deploymentPhase = false;
    }

    #endregion

    #region AI Functions

    public void SpawnPiece(List<GameObject> aiPieces, List<AiPieceView> aiPiecesSpawned)
    {
        float center = _boardSize.x / 2f - TILESIZE / 2;
        float distanceFromBoard = _boardSize.y - 0.1f;
        float yOffset = -0.2f;
        for (int i = 0; i < aiPieces.Count; i++)
        {
            float xPos = center + (i - (_gameData.IAPieces.Count - 1) / 2f);
            AiPieceView piece = Instantiate(aiPieces[i], new Vector3(xPos, yOffset, distanceFromBoard), Quaternion.Euler(0, 90, 0), _tilesContainer)
                .GetComponent<AiPieceView>();
            piece.Initialize(Vector2Int.one * -1);
            aiPiecesSpawned.Add(piece);
        }
    }

    public List<TileController> CheckValidIATiles(Vector2Int position, MovementTypeSo movementBehaviour)
    {
        List<TileController> validTiles = GetEmptyTiles();
        List<TileController> _validMovementTiles = new List<TileController>();
        var _movementValidTiles = movementBehaviour.GetValidPositions(position, _boardSize, validTiles);
        foreach (var _movement in _movementValidTiles)
        {
            _validMovementTiles.Add(_tileControllers[_movement.x, _movement.y]);
        }
        validTiles = validTiles.Intersect(_validMovementTiles).ToList();

        return validTiles;
    }

    private void PerformIaMovement(IAPieceReleasedEvent ev)
    {
        if (ev.CurrentTileCoords.x != -1)
            _tileControllers[ev.CurrentTileCoords.x, ev.CurrentTileCoords.y].TakeTile(GameController.Team.None);
        _tileControllers[ev.TargetTile.Coords.x, ev.TargetTile.Coords.y].TakeTile(GameController.Team.Player2);
        _gameController.LogMovement(ev.TargetTile.Coords, ev.PieceID, GameController.Team.Player2);
        if (CheckForConsecutives(GameController.Team.Player2))
        {
            _gameController.GameFinished(GameController.Result.Defeat);
        }
        else
        {
            _gameController.NextTurn(GameController.Team.Player1, GameController.Team.Player2);
        }
    }

    public int EvaluateBoard(TileController tile, TileController oldTile = null)
    {
       
        oldTile?.TakeTile(GameController.Team.None);
        tile.TakeTile(GameController.Team.Player2);
        bool ConsecutivePieces = CheckForConsecutives(GameController.Team.Player2);
        tile.TakeTile(GameController.Team.None);
        oldTile?.TakeTile(GameController.Team.Player2);
        if (ConsecutivePieces)
            return 3;

        if (oldTile != null)
        {
            oldTile.TakeTile(GameController.Team.Player1);
            ConsecutivePieces = CheckForConsecutives(GameController.Team.Player1);
            oldTile.TakeTile(GameController.Team.Player2);
            if (ConsecutivePieces)
                return 0;
        }

        tile.TakeTile(GameController.Team.Player1);
        ConsecutivePieces = CheckForConsecutives(GameController.Team.Player1);
        tile.TakeTile(GameController.Team.None);
        if (ConsecutivePieces && CanTileBeReached(tile))
            return 2;

        return 1;
    }

    public bool CanTileBeReached(TileController tile)
    {
        foreach (PlayerPieceView pb in _playerPieceViews)
        {
            if (CheckValidTiles(pb).Contains(tile))
                return true;
        }
        return false;
    }

    public bool CanAIMove()
    {
        List<TileController> validTiles = GetEmptyTiles();
        foreach (AiPieceView pb in _gameController.GetAIPieces())
        {
            List<TileController> validMovementTiles = new List<TileController>();
            var movementValidTiles = pb.MovementType.GetValidPositions(pb.TileCoords, _boardSize, validTiles);
            if (movementValidTiles.Count > 0)
                return true;
        }
        return false;
    }

    #endregion

}
