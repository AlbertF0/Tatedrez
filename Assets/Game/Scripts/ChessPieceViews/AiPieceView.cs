using DG.Tweening;
using TMPro;
using UnityEngine;

public class AiPieceView : APieceView
{
    public override void Initialize(Vector2Int coords)
    {
        base.Initialize(coords);
        _initialRotation = Quaternion.Euler(0,90,0);
        _releaseEvent.PieceID = gameObject.GetInstanceID();
        _renderer.material = _data.Player2Color;
    }

    public void MoveIAPiece(TileController targetTile)
    {
        _iaMovementEvent.TargetTile = targetTile.View;
        _iaMovementEvent.CurrentTileCoords = _tileCoords;
        _eventDispatcher.Raise(_iaMovementEvent);

        float liftHeight = 0.5f; 
        var initialPosition = targetTile.View.transform.position;

        Vector3 midPosition = new Vector3(
            (initialPosition.x + targetTile.View.transform.position.x) / 2,
            initialPosition.y + liftHeight,
            (initialPosition.z + targetTile.View.transform.position.z) / 2
        );

        Sequence sequence = DOTween.Sequence();
        sequence.Append(_piece.transform.DOJump(targetTile.View.transform.position, liftHeight, 1, 0.5f));
        sequence.Join(_piece.transform.DORotateQuaternion(_initialRotation, 0.5f));
        sequence.Play();


        _tileCoords = targetTile.View.Coords;
    }
}

