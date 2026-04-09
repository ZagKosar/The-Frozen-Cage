using UnityEngine;

public class UIEvents
{
    public struct OpenWindow
    {
        public string Name;
    }

    public struct OpenWindowWithContext
    {
        public string Name;
        public object Context;
    }


    public struct CloseWindow
    {
        public string Name;
    }

    public struct CloseLastWindow
    {

    }

    public struct QuitGame
    {

    }

    public struct StartNewGame
    {

    }

    public struct ExitToMainMenu
    {

    }
}
