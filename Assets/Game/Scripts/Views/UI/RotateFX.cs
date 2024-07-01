using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class RotateFX : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private RectTransform _transform;
    [SerializeField]
    private float  _speed;
    [SerializeField]
    private int direction = 1;

    void Start()
    {
        _transform.DORotate(new Vector3(0, 0, 360*direction), _speed, RotateMode.FastBeyond360)
                    .SetEase(Ease.Linear)
                    .SetLoops(-1, LoopType.Restart);
    }

}
