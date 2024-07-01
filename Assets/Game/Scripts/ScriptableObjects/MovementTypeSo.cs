using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MovementType", menuName = "ScriptableObjects/MovementTypeData", order = 1)]
public class MovementTypeSo : ScriptableObject
{
    [SerializeReference]
    public AMovementBehaviour Behaviour;
    [SerializeField]
    private bool _canJumpOver;


    public List<Vector2Int> GetValidPositions(Vector2Int position, Vector2Int matrixSize, List<TileController> validTiles)
    {
        return Behaviour.MovementBehaviour(position, matrixSize, validTiles, _canJumpOver);
    }
}
