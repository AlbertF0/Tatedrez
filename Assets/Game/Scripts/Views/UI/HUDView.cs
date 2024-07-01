using System;
using System.Collections;
using DG.Tweening;
using Events;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class HUDView : MonoBehaviour
{
    [SerializeField]
    private Image _player1Picture;
    [SerializeField]
    private TextMeshProUGUI _player1Text;
    [SerializeField]
    private Image _player2Picture;
    [SerializeField]
    private TextMeshProUGUI _player2Text;
    [SerializeField]
    private CanvasGroup _choseCanvas;
    [SerializeField]
    private CanvasGroup _surrenderPanel;
    [SerializeField]
    private Button _surrenderButton;
    [SerializeField]
    private Button _yesButton;
    [SerializeField]
    private Button _noButton;
    [SerializeField]
    private TextMeshProUGUI _chosePlayerText;

    private Sequence sequencePlayer1;
    private Sequence sequencePlayer2;

    private EventDispatcher _eventDispatcher;
    private GameFinishedEvent _gameFinishedEvent = new GameFinishedEvent();

    public void Init(EventDispatcher eventDispatcher)
    {
        _surrenderButton.interactable = false;
        _surrenderPanel.gameObject.SetActive(false);

        _surrenderButton.onClick.AddListener(ShowSurrenderPanel);
        _yesButton.onClick.AddListener(Surrender);
        _noButton.onClick.AddListener(HideSurrenderPanel);

        _gameFinishedEvent.Result = GameController.Result.Defeat;
        _gameFinishedEvent.Time = 0;

        _eventDispatcher = eventDispatcher;
        _eventDispatcher.AddListener<ChangeTurnEvent>(ChangeTurn);
        _eventDispatcher.AddListener<BoardFinishedAnimationEvent>(ChoseTeamAnimation);
        _eventDispatcher.AddListener<CantMoveEvent>(CantMoveAnimation);
        _eventDispatcher.AddListener<CleanBoardEvent>(ResetHUD);
    }

    public void ResetHUD(CleanBoardEvent ev)
    {
        _player1Text.text = "Player 1";
        _player1Text.color = Color.white;
        _player2Text.text = "Player 2";
        _player2Text.color = Color.white;
        _surrenderButton.interactable = false;
        _surrenderPanel.gameObject.SetActive(false);
    }

    private void ChoseTeamAnimation(BoardFinishedAnimationEvent ev)
    {
        _chosePlayerText.text = "Choosing starting player";
        _player1Text.text = "Player 1";
        _player1Text.color = Color.white;
        _player2Text.text = "Player 2";
        _player2Text.color = Color.white;
        _choseCanvas.blocksRaycasts = true;
        _choseCanvas.DOFade(1, 0.5f);

        StartCoroutine(AddDots());

        float duration = 0.05f;
        float size = 0.7f;
        float t = 0;

        sequencePlayer1 = DOTween.Sequence();
        sequencePlayer2 = DOTween.Sequence();

        sequencePlayer1.Append(_player1Picture.DOColor(Color.gray, duration))
                .Join(_player1Picture.rectTransform.DOScale(Vector3.one * size, duration).From(Vector3.one))
                .Append(_player1Picture.DOColor(Color.white, duration))
                .Join(_player1Picture.rectTransform.DOScale(Vector3.one, duration).From(Vector3.one * size))
                .SetLoops(-1, LoopType.Restart)
                .OnStepComplete(() => {
                    sequencePlayer1.timeScale *= (float)Math.Exp(-0.1 * t);
                    t ++;
                }); ;

        sequencePlayer2.Append(_player2Picture.DOColor(Color.white, duration))
               .Join(_player2Picture.rectTransform.DOScale(Vector3.one, duration).From(Vector3.one * size))
               .Append(_player2Picture.DOColor(Color.gray, duration))
               .Join(_player2Picture.rectTransform.DOScale(Vector3.one * size, duration).From(Vector3.one))
               .SetLoops(-1, LoopType.Restart)
               .OnStepComplete(() => {
                   sequencePlayer2.timeScale *= (float)Math.Exp(-0.1 * t);
               }); ; ;
    }

    private IEnumerator AddDots()
    {
        yield return new WaitForSeconds(0.75f);
        _chosePlayerText.text = "Choosing starting player.";
        yield return new WaitForSeconds(0.75f);
        _chosePlayerText.text = "Choosing starting player..";
        yield return new WaitForSeconds(0.75f);
        _chosePlayerText.text = "Choosing starting player...";
    }

    private void ChangeTurn(ChangeTurnEvent ev)
    {
        if (sequencePlayer1.IsActive())
        {
            _surrenderButton.interactable = true;
            _choseCanvas.blocksRaycasts = false;
            sequencePlayer1.Kill();
            sequencePlayer2.Kill();
            _choseCanvas.DOFade(0, 0.5f);
        }

        if(ev.Team == GameController.Team.Player1)
        {
            _player1Text.text = "Player 1";
            _player1Text.color = Color.white;
        }
        else
        {
            _player2Text.text = "Player 2";
            _player2Text.color = Color.white;
        }

        Color color1 = ev.Team == GameController.Team.Player1 ? Color.white : Color.gray; 
        Color color2 = ev.Team == GameController.Team.Player2 ? Color.white : Color.gray; 

        _player1Picture.DOColor(color1, 0.5f);
        _player2Picture.DOColor(color2, 0.5f);

        float size1 = ev.Team == GameController.Team.Player1 ? 1 : 0.7f;
        float size2 = ev.Team == GameController.Team.Player2 ? 1 : 0.7f;

        _player1Picture.transform.DOScale(Vector3.one * size1, 0.5f);
        _player2Picture.transform.DOScale(Vector3.one * size2, 0.5f);
    }

    private void CantMoveAnimation(CantMoveEvent ev)
    {

        TextMeshProUGUI text = ev.PlayerWithoutMoves == GameController.Team.Player1? _player1Text : _player2Text;
        text.text = "NO MOVES";
        text.DOColor(Color.red, 0.3f);
        text.transform.localScale = Vector3.one;
        text.transform.DOPunchScale(Vector3.one, 0.3f, 1, 1).OnComplete(() => ev.Callback.Invoke(ev.NextPlayer, ev.PlayerWithoutMoves));
    }

    private void ShowSurrenderPanel()
    {
        _surrenderPanel.gameObject.SetActive(true);
        _surrenderPanel.DOFade(1, 0.5f).From(0);
    }

    private void Surrender()
    {
        _eventDispatcher.Raise(_gameFinishedEvent);
        HideSurrenderPanel();
    }

    public void HideSurrenderPanel()
    {
        _surrenderPanel.DOFade(0, 0.5f).OnComplete(()=> _surrenderPanel.gameObject.SetActive(false));
    }

}
