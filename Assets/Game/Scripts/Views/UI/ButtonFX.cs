using System.Collections;
using DG.Tweening;
using UnityEngine;

public class ButtonFX : MonoBehaviour
{
    [SerializeField]
    private Transform _button;
    [SerializeField]
    private Transform _sparkleFX;
   
  
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(MoveSparkle());
    }

    private IEnumerator MoveSparkle()
    {
        yield return new WaitForSeconds(2);
        _button.DOPunchScale(Vector3.one*0.1f,2,2,0.5f);
        yield return new WaitForSeconds(0.5f);
        _sparkleFX.DOLocalMoveX(-250,2).From(250).SetEase(Ease.InOutQuart);
        yield return new WaitForSeconds(2);
        StartCoroutine(MoveSparkle());
    }
}
