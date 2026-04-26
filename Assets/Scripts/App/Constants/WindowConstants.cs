using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Scripts.App.Constants
{
    public static partial class Constants
    {
        public static readonly string MainMenuWindow = "main_menu_panel";
        public static readonly string SettingsPopUp = "settings_panel";
        public static readonly string InventoryWindow = "inventory_window";
        public static readonly string PauseWindow = "pause_window";
        public static readonly string PlayerGUI = "player_gui";
        public static readonly string SaveWindow = "save_window";
        public static readonly string LoadingWindow = "loading_window";

        public static IReadOnlyList<string> AllWindows = new List<string>()
        {
            MainMenuWindow,
            SettingsPopUp,
            InventoryWindow,
            PauseWindow,
            PlayerGUI,
            SaveWindow,
            LoadingWindow
        };
    }
}
