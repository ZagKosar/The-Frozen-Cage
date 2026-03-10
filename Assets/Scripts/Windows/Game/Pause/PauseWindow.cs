using Scripts.WindowSwitcher;
using UnityEngine;

public class PauseWindow : WindowPanel
{
    public override void Open()
    {
        gameObject.SetActive(true);
    }

    public override void Close()
    {
        gameObject.SetActive(false);
    }

    public override void Load()
    {
        
    }

    public override void Destroy()
    {
        
    }
}
