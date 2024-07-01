using DG.Tweening;
using Events;
using UnityEngine;
using Utils;

public class APieceView : MonoBehaviour
{
    [SerializeField]
    protected Transform _piece;
    [SerializeField]
    protected MovementTypeSo _movementType;
    [SerializeField]
    protected MeshRenderer _renderer;

    protected Camera _camera;
    protected Vector2Int _tileCoords = new Vector2Int(-1, -1);
    protected bool _deploymentPhase = true;

    protected Vector3 _newPosition;
    protected Vector3 _basePosition;
    protected Quaternion _initialRotation;

    protected EventDispatcher _eventDispatcher;
    protected GameDataSo _data;
    protected PieceSelectedEvent _selectedEvent = new PieceSelectedEvent();
    protected PieceReleasedEvent _releaseEvent = new PieceReleasedEvent();
    protected IAPieceReleasedEvent _iaMovementEvent = new IAPieceReleasedEvent();

    public MovementTypeSo MovementType => _movementType;
    public Vector2Int TileCoords => _tileCoords;

    public virtual void Initialize(Vector2Int coords)
    {
        _eventDispatcher = ServiceLocator.GetService<EventDispatcher>();
        _data = ServiceLocator.GetService<GameConfigService>().GameData;
        _camera = Camera.main;

        transform.localScale = Vector3.zero;
        _tileCoords = coords;
        
        _eventDispatcher.AddListener<EndOfDeploymentEvent>(EndOfDeployment);
    }

    public void SpawnAnimation(float delay)
    {
        transform.DOScale(Vector3.one * 0.75f, 0.3f).SetDelay(delay).SetEase(Ease.OutBounce);
        transform.DOMoveY(transform.localPosition.y, 0.6f).From(transform.localPosition.y + 0.75f).SetDelay(delay).SetEase(Ease.OutBounce);
    }

    public void DespawnAnimation(float delay)
    {
        transform.DOScale(Vector3.zero, 0.5f).SetDelay(delay*0.2f);
    }

    public void VictoryAnimation()
    {
        transform.DOMoveY(transform.position.y + 0.7f, 0.5f).SetEase(Ease.OutQuad).SetLoops(-1, LoopType.Yoyo);
        transform.DORotate(transform.rotation.eulerAngles + new Vector3(0, 360, 0), 0.5f, RotateMode.FastBeyond360).SetEase(Ease.InOutSine).SetLoops(-1);
    }

    private void EndOfDeployment(EndOfDeploymentEvent ev)
    {
        _deploymentPhase = false;
        _eventDispatcher.RemoveListener<EndOfDeploymentEvent>(EndOfDeployment);
    }



}

