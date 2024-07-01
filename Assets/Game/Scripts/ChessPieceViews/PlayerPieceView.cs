using System;
using DG.Tweening;
using Events;
using UnityEngine;
using Utils;

public class PlayerPieceView : APieceView
    {
    [SerializeField]
    protected Collider _collider;

    private float _startTime;
    private float _angle;
    private float _height;

    private Vector3 _oldMousePosition;

    private bool _holding;
    private bool _interactable;
    private bool _playerTurn;

    private float _delayCounter;

    private LayerMask _layerMask = ~(1 << 3);
    private TileView _highlightedTile;

    public void Initialize(Vector2Int coords, GameController.Team starting)
    {
        base.Initialize(coords);

        _eventDispatcher.AddListener<ChangeTurnEvent>(ChangeTurn);

        _releaseEvent.PieceID = gameObject.GetInstanceID();
        _releaseEvent.Callback = Release;

        SetUpPiece(starting);
        _interactable = false;
    }

    public void GameStarts()
    {
        _interactable = true;
    }

    private void ChangeTurn(ChangeTurnEvent ev)
    {
        _playerTurn = ev.Team == GameController.Team.Player1;
    }

    private void SetUpPiece(GameController.Team  starting)
    {
        transform.localScale = Vector3.zero;
        _renderer.material = _data.Player1Color;
        _playerTurn = starting == GameController.Team.Player1;
        _selectedEvent.PieceBehaviour = this;
        _basePosition = _piece.position;
        _initialRotation = _piece.rotation;
        ResetPosition();
    }

    public void ResetPosition()
    {
        _interactable = true;
        _holding = false;
        _piece.DOMove(_basePosition, 0.3f);
        _piece.DORotateQuaternion(_initialRotation, 0.3f);
    }
   
    private void Update()
    {
        if (!_playerTurn)
            return;
        if (_holding)
        {
            if (_delayCounter < 10)
            {
                _delayCounter++;
            }
            else
            {
                _delayCounter = 0;
            }
            _newPosition = Input.mousePosition;
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit _hit;
            if (Physics.Raycast(ray, out _hit, 100f, _layerMask) && _hit.transform.tag.Equals("BoardTile"))
            {
                var actualTile = _hit.transform.gameObject.GetComponent<TileView>();
                if (_highlightedTile != actualTile)
                {
                    _highlightedTile?.ToggleSelectedHighlightTile(false);
                    _highlightedTile = actualTile;
                    _highlightedTile.ToggleSelectedHighlightTile(true);
                }

            }
            else if(_highlightedTile != null)
            {
                _highlightedTile?.ToggleSelectedHighlightTile(false);
                _highlightedTile = null;
            }
            Pickup();
        }
        if (!_interactable)
            return;
        if (Input.GetMouseButtonDown(0))
        {
            if (_deploymentPhase && _tileCoords.x != -1)
                return;
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit _hit;

            if (!Physics.Raycast(ray, out _hit, 100f) || _hit.transform != _piece)
            {
                return;
            }
            _basePosition = _piece.position;
            _piece.DOKill();
            _newPosition = Input.mousePosition;
            _startTime = Time.time;
            _delayCounter = 0;
            _holding = true;
            _selectedEvent.Position = _tileCoords;
            _eventDispatcher.Raise(_selectedEvent);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (!_holding)
                return;
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit _hit;
            _releaseEvent.Valid = true;
            if (!Physics.Raycast(ray, out _hit, 100f, _layerMask) || !_hit.transform.tag.Equals("BoardTile"))
            {
                _releaseEvent.Valid = false;
            }
            else
            {
                _releaseEvent.TargetTile = _hit.transform.gameObject.GetComponent<TileView>();
            }
            _releaseEvent.CurrentTileCoords = _tileCoords;
            _eventDispatcher.Raise(_releaseEvent);
        }
    }

    void Release(bool valid,TileView tile)
    {
        if (valid)
        {
            _interactable = true;
            _holding = false;
            _piece.DOMove(tile.transform.position,0.3f);
            _piece.DORotateQuaternion(_initialRotation,0.3f);
            _basePosition = _piece.position;
            _tileCoords = tile.Coords;
            return;
        }
        ResetPosition();
    }

    void Pickup()
    {
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        float rayDistance;
        Vector3 _piecePosition = _piece.localPosition;

        if (groundPlane.Raycast(ray, out rayDistance))
        {
            _piecePosition = ray.GetPoint(rayDistance);
        }

        _piece.localPosition = Vector3.Lerp(_piece.localPosition, _piecePosition + Vector3.up*0.3f, 10f * Time.deltaTime);

        Vector3 direction = Input.mousePosition - _oldMousePosition;
        float magnitude = direction.magnitude;

        direction.Normalize();

        float rotationAngleX = -direction.x * magnitude * 2;
        float rotationAngleZ = -direction.y * magnitude * 2;

        Quaternion rotation = Quaternion.Euler(rotationAngleX, -90f, rotationAngleZ);

        _piece.rotation = Quaternion.Slerp(_piece.rotation, rotation, 5 * Time.deltaTime);

        _oldMousePosition = Input.mousePosition;

    }
}

