using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DG.Tweening;
using Events;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class UIView: MonoBehaviour
{

    [SerializeField]
    private CanvasGroup _mainMenu;
    [SerializeField]
    private GameFinishedPanelView _resultView;
    [SerializeField]
    private HUDView _hudView;
    [SerializeField]
    private Button _playButton;

    private EventDispatcher _eventDispatcher;

    void Start()
    {
        _eventDispatcher = ServiceLocator.GetService<EventDispatcher>();
        _eventDispatcher.AddListener<GameFinishedEvent>(DisplayResult);
        _playButton.onClick.AddListener(Play);
        _resultView.Init(this, _eventDispatcher);
        _hudView.Init(_eventDispatcher);
    }

    public void ShowMainMenu()
    {
        _mainMenu.gameObject.SetActive(true);
        _mainMenu.DOFade(1, 0.5f).OnComplete(
           () => _mainMenu.interactable = true);
    }

    private void Play()
    {
        _mainMenu.interactable = false;
        _mainMenu.DOFade(0, 0.5f).OnComplete(
            () => Transition());
    }

    private void Transition()
    {
        _eventDispatcher.Raise<PlayButtonClickedEvent>();
        _mainMenu.gameObject.SetActive(false);
    }
   
    private void DisplayResult(GameFinishedEvent ev)
    {
        _resultView.gameObject.SetActive(true);
        _resultView.DisplayResult(ev.Result, ev.Time);
    }

}
