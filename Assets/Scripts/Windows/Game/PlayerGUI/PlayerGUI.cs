using DG.Tweening;
using Scripts.Events.Game;
using Scripts.WindowSwitcher;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerGUI : WindowPanel
{
    [SerializeField] private Image _crosshair;
    [SerializeField] private Sprite _defaultCrosshair;
    [SerializeField] private Sprite _hoverCrosshair;
    [SerializeField] private TMP_Text _interactionDescription;
    [SerializeField] private TMP_Text _dialogSubtitles;
    [SerializeField] private GameObject _subtitlesContainer;

    public override int Priority => 1;

    public override void Open(object context = null)
    {
        EventManager.Instance.Subscribe<GameEvent.InteractHover>(OnItemHover);
        EventManager.Instance.Subscribe<GameEvent.InteractHoverEnd>(OnItemHoverEnd);
        EventManager.Instance.Subscribe<GameEvent.InnerDialogue>(OnInnerDialogue);

        gameObject.SetActive(true);
    }

    public override void Close()
    {
        EventManager.Instance.Unsubscribe<GameEvent.InteractHover>(OnItemHover);
        EventManager.Instance.Unsubscribe<GameEvent.InteractHoverEnd>(OnItemHoverEnd);
        EventManager.Instance.Unsubscribe<GameEvent.InnerDialogue>(OnInnerDialogue);

        gameObject.SetActive(false);
    }

    public override void Load()
    {
        
    }

    public override void Destroy()
    {
        
    }

    private void OnItemHover(GameEvent.InteractHover data)
    {
        _crosshair.transform.localScale = Vector3.one * 4;
        _crosshair.sprite = _hoverCrosshair;

        _interactionDescription.text = data.Interact.interactDescription;
        _interactionDescription.gameObject.SetActive(true);
    }

    private void OnItemHoverEnd(GameEvent.InteractHoverEnd data)
    {
        _crosshair.transform.localScale = Vector3.one;
        _crosshair.sprite = _defaultCrosshair;

        _interactionDescription.gameObject.SetActive(false);
    }

    private void OnInnerDialogue(GameEvent.InnerDialogue data)
    {
        _subtitlesContainer.SetActive(true);

        _dialogSubtitles.text = data.Text;
        _dialogSubtitles.maxVisibleCharacters = 0;
        DOTween.To(
            () => _dialogSubtitles.maxVisibleCharacters,
            mvc => _dialogSubtitles.maxVisibleCharacters = mvc,
            data.Text.Length,
            3f
            ).SetEase(Ease.Linear)
            .OnComplete(() => DOVirtual.DelayedCall(4f, () => {
                _dialogSubtitles.text = "";
                _subtitlesContainer.SetActive(false);
            }));
    }
}
