using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class TileView : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer _renderer;
    [SerializeField]
    private GameObject _highLightedEffect;
    [SerializeField]
    private SpriteRenderer _circle;
    [SerializeField]
    private SpriteRenderer _border;
    [SerializeField]
    private Color _normalColor;
    [SerializeField]
    private Color _selectedColor;

    private Vector2Int _coords;
    private bool selected;

    public Vector2Int Coords => _coords;


    public void Initialize(Vector2Int coords, Material mat, int index)
    {
        _coords = coords;
        _renderer.material = mat;
        _circle.transform.DOScale(Vector3.one * 0.7f, 0.5f).SetEase(Ease.InOutCubic)
              .SetLoops(-1, LoopType.Yoyo);
        PlayShowAnimation(index);
    }

    private void PlayShowAnimation(int index)
    {
        Vector3 scale = transform.localScale;
        transform.localScale = Vector3.zero;
        transform.DOScale(scale, 0.2f).From(Vector3.zero).SetDelay(index*0.05f);
        transform.DOMoveY(transform.localPosition.y, 1f).From(transform.localPosition.y + 1).SetDelay(index * 0.05f);
        _renderer.material.DOFade(1, 0.5f).From(0).SetDelay(index * 0.05f);
    }

    public void PlayFallAnimation(int index)
    {
        transform.DOScale(Vector3.zero, 0.5f).SetDelay(index * 0.2f);
        transform.DOMoveY(transform.localPosition.y -1 , 1f).SetDelay(index * 0.2f);
        _renderer.material.DOFade(0, 0.5f).From(1).SetDelay(index * 0.2f);
    }

    public void ToggleSelectedHighlightTile(bool isSelected)
    {

        if (selected == isSelected)
            return;
        Color targetColor = isSelected ? _selectedColor : _normalColor;

        _circle.DOColor(targetColor,0.2f);
        _border.DOColor(targetColor, 0.2f);

        selected = isSelected;
    }

    public void ToggleHighlightTile(bool toggle)
    {
        _circle.DOKill();
        _border.DOKill();
        if (toggle)
        {
            _highLightedEffect.SetActive(true);
            _circle.DOFade(0.75f, 0.2f);
            _circle.DOFade(0.75f, 0.2f);
            _border.DOFade(0.75f, 0.2f);
        }
        else
        {
            _circle.DOFade(0, 0.2f);
            _border.DOFade(0, 0.2f).OnComplete(() => _highLightedEffect.SetActive(false));
        }
    }
    public Vector3 GetTileWorldPositon()
    {
        return transform.position;
    }
}
