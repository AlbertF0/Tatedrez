using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "ScriptableObjects/GameData", order = 1)]
public class GameDataSo : ScriptableObject
{

    [SerializeField]
    private AAILogic _aiLogic;
    [SerializeField]
    List<GameObject> _iAPieces;
    [SerializeField]
    List<GameObject> _playerPieces;
    [SerializeField]
    private Vector2Int _boardSize = new Vector2Int(3, 3);
    [SerializeField]
    private int _consecutivePiecesToWin = 3;
    [SerializeField]
    private Material _player1Color;
    [SerializeField]
    private Material _player2Color;
    [SerializeField]
    private Material _boardColor1;
    [SerializeField]
    private Material _boardColor2;

    public AAILogic AILogic => _aiLogic;
    public List<GameObject> IAPieces => _iAPieces;
    public List<GameObject> PlayerPieces => _playerPieces;
    public Vector2Int BoardSize => _boardSize;
    public int ConsecutivePiecesToWin => _consecutivePiecesToWin;
    public Material Color1 => _boardColor1;
    public Material Color2 => _boardColor2;
    public Material Player1Color => _player1Color;
    public Material Player2Color => _player2Color;

}
