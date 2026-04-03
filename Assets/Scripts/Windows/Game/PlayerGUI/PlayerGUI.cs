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

    public override void Open()
    {
        EventManager.Instance.Subscribe<GameEvent.InteractHover>(OnItemHover);
        EventManager.Instance.Subscribe<GameEvent.InteractHoverEnd>(OnItemHoverEnd);

        gameObject.SetActive(true);
    }

    public override void Close()
    {
        EventManager.Instance.Unsubscribe<GameEvent.InteractHover>(OnItemHover);
        EventManager.Instance.Unsubscribe<GameEvent.InteractHoverEnd>(OnItemHoverEnd);

        gameObject.SetActive(false);
    }

    public override void Load()
    {
        
    }

    public override void Destroy()
    {
        
    }

    void Start()
    {
        
    }

    void Update()
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
}
