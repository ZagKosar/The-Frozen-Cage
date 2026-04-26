using Scripts.App;
using Scripts.Game.Items;
using UnityEngine;

public class DependencyContainer : MonoBehaviour
{
    [SerializeField] private ClientSettings _clientSettings = new();
    [SerializeField] private GraphicsManager _graphicsMaster;
    [SerializeField] private AudioManager _audioMaster;
    [SerializeField] private GameTime _gameTime;
    [SerializeField] private InputHandler _inputHandler;
    [SerializeField] private ItemsLibrary _itemsLibrary;

    private Inventory _playerInventory;

    public static ClientSettings ClientSettings
    {
        get
        {
            return Instance._clientSettings;
        }
    }

    public static GraphicsManager GraphicsMaster
    {
        get
        {
            return Instance._graphicsMaster;
        }
    }

    public static AudioManager AudioMaster
    {
        get 
        {
            return Instance._audioMaster;
        }
    }

    public static GameTime GameTime
    {
        get
        {
            return Instance._gameTime;
        } 
    }

    public static InputHandler InputHandler
    {
        get
        {
            return Instance._inputHandler;
        }
    }

    public static ItemsLibrary ItemsLibrary
    {
        get
        {
            return Instance._itemsLibrary;
        }
    }

    public static Inventory Inventory
    {
        get
        {
            return Instance._playerInventory;
        }
    }

    private static DependencyContainer s_instance;
    public static DependencyContainer Instance
    {
        get
        {
            s_instance ??= FindFirstObjectByType<DependencyContainer>();
            
            return s_instance;
        }
    }

    private void Awake()
    {
        s_instance = this;
    }

    public void Initialize()
    {
        _gameTime = new();

        _inputHandler.Initialize();
        _itemsLibrary.Initialize();
    }

    public void SetInventory(Inventory inventory)
    {
        _playerInventory = inventory;
    }
}
