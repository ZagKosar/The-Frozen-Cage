using Scripts.WindowSwitcher;
using UnityEngine;

public class SettingsPopUp : WindowPanel
{
    public override void Open()
    {
        gameObject.SetActive(true);
    }

    public override void Close()
    {
        gameObject.SetActive(false);
    }
}
