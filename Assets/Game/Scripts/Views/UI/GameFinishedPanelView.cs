using DG.Tweening;
using Events;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using static ResultScreenCatalog;

public class GameFinishedPanelView : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup _canvasGroup;
    [SerializeField]
    private Image _background;
    [SerializeField]
    private TextMeshProUGUI _title;
    [SerializeField]
    private TextMeshProUGUI _subtitle;
    [SerializeField]
    private TextMeshProUGUI _buttonText;

    [SerializeField]
    private ResultScreenCatalog _catalog;

    [SerializeField]
    private Button _play;
    [SerializeField]
    private Button _mainMenu;

    private UIView _view;
    private EventDispatcher _eventDispatcher;

    public void Init(UIView view , EventDispatcher eventDispatcher)
    {
        _canvasGroup.interactable = false;
        _canvasGroup.alpha = 0f;
        _view = view;
        _eventDispatcher = eventDispatcher;
        _play.onClick.AddListener(PlayAgain);
        _mainMenu.onClick.AddListener(BackToMenu);
        gameObject.SetActive(false);
    }

    public void DisplayResult(GameController.Result result, float delay)
    {
        ResultElements resultElem = _catalog.ReturnResult(result);
        _background.sprite = resultElem.Background;
        _title.text = resultElem.Title;
        _title.color = resultElem.TitleColor;
        _subtitle.text = resultElem.Description;
        _subtitle.color = resultElem.DescriptionColor;
        _buttonText.text = resultElem.ButtonText;

        _canvasGroup.alpha = 0;
        _canvasGroup.interactable = true;
        _canvasGroup.DOFade(1, 0.5f).SetDelay(delay);
    }

    private void PlayAgain()
    {
        _eventDispatcher.Raise<CleanBoardEvent>();
        _canvasGroup.interactable = false;
        _canvasGroup.DOFade(0, 0.5f).OnComplete(
            () => DeactivateAndStartGame());
    }
    private void DeactivateAndStartGame()
    {
        _eventDispatcher.Raise<PlayButtonClickedEvent>();
        gameObject.SetActive(false);
    }

    private void BackToMenu()
    {
        _eventDispatcher.Raise<CleanBoardEvent>();
        _canvasGroup.interactable = false;
        _canvasGroup.DOFade(0, 0.5f).OnComplete(()=> gameObject.SetActive(false));
        _view.ShowMainMenu();
    }
}
