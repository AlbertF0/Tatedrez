
using System;
using System.Numerics;
using UnityEngine;


public class TileController
{
    private TileView _view;
    private Vector2Int _tileCoords;

    private GameController.Team _team = GameController.Team.None;

    public GameController.Team Team => _team;
    public TileView View => _view;
    public Vector2Int TileCoords  => _tileCoords;

    public TileController(int x, int y, TileView view)
    {
        _tileCoords = new Vector2Int(x, y);
        _view = view;
    }

    public void TakeTile(GameController.Team team = GameController.Team.None)
    {
        _team = team;
    }
}
