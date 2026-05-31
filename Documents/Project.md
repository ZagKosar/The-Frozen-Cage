# Project: The Frozen Cage — Ultra-Detailed Code Reference

> Unity 6000.3.7f1 (URP) | C# | 80+ scripts

---

## Table of Contents

1. [Overview](#overview)
2. [Architecture](#architecture)
3. [Scripts.App (Application Core)](#scriptsapp)
4. [Scripts.Events (Event System)](#scriptsevents)
5. [Scripts.Game (Game Logic)](#scriptsgame)
6. [Scripts.Windows (UI)](#scriptswindows)
7. [Scripts.World (Environment)](#scriptsworld)
8. [Scripts.Utilities](#scriptsutilities)
9. [Scripts.Settings](#scriptssettings)

---

## Overview

**The Frozen Cage** is a first-person psychological horror game built in Unity 6000.3.7f1 using the Universal Render Pipeline (URP). The player explores an abandoned frozen research facility, taking photographs to document evidence, avoiding hostile entities, collecting items, and solving environmental puzzles.

### Key Systems
- **Event-driven architecture** — decoupled communication via struct-based events
- **Service locator** (DependencyContainer) — global access to core services
- **Window stack** (WindowSwitcher) — MVC-like UI navigation
- **Component-based save system** — each MonoBehaviour serializes its own data
- **Trigger composition** — modular trigger system with multiple condition/action types

---

## Architecture

### EventManager (`Scripts.App.Core`)
The backbone of inter-system communication. Uses a generic `EventManager<T>` where `T` is a struct.

| Method | Description |
|--------|-------------|
| `AddListener(Action<T>)` | Subscribe to event type T |
| `RemoveListener(Action<T>)` | Unsubscribe from event type T |
| `Broadcast(T)` | Invoke all listeners of event type T |

### AutoEventBehaviour (`Scripts.App.Core`)
Abstract base class. On `OnEnable` subscribes via `AutoEventProvider`, on `OnDisable` auto-unsubscribes. Any MonoBehaviour that listens to events inherits from this.

### AutoEventProvider (`Scripts.App.Core`)
Generates `addListener`/`removeListener` calls for each listener method marked with attributes. Works with `AutoEventBehaviour` to wire up subscriptions automatically.

### DependencyContainer (`Scripts.App.Core`)
Generic singleton service locator.

| Method | Description |
|--------|-------------|
| `Register<T>(T)` | Register a service instance |
| `RegisterAsSingle<T>()` | Auto-create and register a MonoBehaviour as singleton |
| `Resolve<T>()` | Get registered service by type |
| `Unregister<T>()` | Remove a service from container |

Registered services: SaveSystem, Localization, AudioManager, InputManager, GameManager, etc.

### WindowSwitcher (`Scripts.Windows.Base`)
Stack-based window navigation system.

| Method | Description |
|--------|-------------|
| `Open(string name, object context)` | Push window onto stack with optional context |
| `Close(string name)` | Remove specific window from stack |
| `CloseLast()` | Pop top window |
| `CloseAll()` | Clear entire stack |
| `TryGetWindow(string)` | Find window by name |
| `GetWindow<T>(string)` | Generic typed window access |

### SaveSystem (`Scripts.App.Systems`)
Component-based save strategy. Each MonoBehaviour implements `ISaveableComponent` to provide its own serialization logic.

| Method | Description |
|--------|-------------|
| `Save(string slot)` | Serialize all registered components to slot |
| `Load(string slot)` | Deserialize and restore state from slot |
| `Delete(string slot)` | Remove save file |
| `GetSaves()` | List available saves |
| `RegisterComponent(ISaveableComponent)` | Add component to save list |
| `UnregisterComponent(ISaveableComponent)` | Remove from save list |

---

## Scripts.App

### App.Core

#### `Bootstrap.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/App/Core/Bootstrap.cs` |
| **Class** | `Bootstrap : MonoBehaviour` |
| **Purpose** | Entry point. Runs on scene load before anything else. Registers all core services in DependencyContainer, initializes SaveSystem, loads settings, and fires `GameEvent.LoadNextScene` to transition to the main menu. |

#### `DependencyContainer.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/App/Core/DependencyContainer.cs` |
| **Class** | `DependencyContainer : MonoBehaviour` |
| **Purpose** | Global service locator (singleton pattern). |
| **Fields** | `_dependencies : Dictionary<Type, object>` |
| **Methods** | `Awake()` — singleton init, DontDestroyOnLoad; `Register<T>(T)`, `Resolve<T>()`, `Unregister<T>()`, `Instantiate<T>()` |
| **Usage** | `DependencyContainer.Resolve<SaveSystem>()` throughout the codebase |

#### `EventManager.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/App/Core/EventManager.cs` |
| **Class** | `EventManager<T> : IEventManager where T : struct` |
| **Purpose** | Generic type-safe event system. One EventManager instance per struct type T. |
| **Methods** | `AddListener(Action<T>)`, `RemoveListener(Action<T>)`, `Broadcast(T)` |
| **Threading** | Single-threaded; checks that `GameEvent.Pause` is not currently broadcasting before allowing nested broadcasts (avoids stack overflow) |

#### `AutoEventBehaviour.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/App/Core/AutoEventBehaviour.cs` |
| **Class** | `AutoEventBehaviour : MonoBehaviour` |
| **Purpose** | Abstract base. On Enable subscribes to events via AutoEventProvider; OnDisable unsubscribes. |
| **Methods** | `OnEnable()` — subscribe; `OnDisable()` — unsubscribe |

#### `AutoEventProvider.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/App/Core/AutoEventProvider.cs` |
| **Class** | `AutoEventProvider` (static) |
| **Purpose** | Uses reflection at startup to find all listener methods in AutoEventBehaviour subclasses and generates AddListener/RemoveListener delegates. |
| **Methods** | `StartProvider(MonoBehaviour)`, `StopProvider(MonoBehaviour)` |

#### `CoroutineManager.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/App/Core/CoroutineManager.cs` |
| **Class** | `CoroutineManager : MonoBehaviour` |
| **Purpose** | Singleton MonoBehaviour that owns all coroutines. Allows non-MonoBehaviour classes to start/stop coroutines. |
| **Methods** | `Awake()` — singleton; `StartCoroutine(IEnumerator)`, `StopCoroutine(Coroutine)` |

#### `GameLoader.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/App/Core/GameLoader.cs` |
| **Class** | `GameLoader : MonoBehaviour` |
| **Purpose** | Manages scene loading with loading screen. Listens to `GameEvent.LoadNextScene`. |
| **Fields** | `_loadingScreen : GameObject` |
| **Methods** | `Awake()` — subscribe; `OnDestroy()` — unsubscribe; `OnLoadNextScene(GameEvent.LoadNextScene)` — activates loading screen, loads scene asynchronously via SceneManager.LoadSceneAsync |

#### `SceneController.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/App/Core/SceneController.cs` |
| **Class** | `SceneController : MonoBehaviour` |
| **Purpose** | Handles transitions between game scenes. Provides fade-in/fade-out effects. |
| **Methods** | `FadeIn(float duration)`, `FadeOut(float duration)`, `LoadScene(string name)` |

---

### App.Constants

#### `Constants.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/App/Constants/Constants.cs` |
| **Class** | `Constants` (static partial) |
| **Purpose** | Centralized constant storage. Split across multiple files via `partial`. |

#### `Constants.Settings.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/App/Constants/Constants.Settings.cs` |
| **Class** | `Constants` (partial) |
| **Constants** | `AUTHOR`, `VERSION`, `SETTINGS_FILE_NAME`, `SAVES_FOLDER_NAME`, `SAVES_FILE_FORMAT` |

#### `Constants.AudioMixer.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/App/Constants/Constants.AudioMixer.cs` |
| **Class** | `Constants` (partial) |
| **Constants** | `MASTER_VOLUME`, `MUSIC_VOLUME`, `SFX_VOLUME`, `UI_VOLUME`, ambient volume parameter names for the Audio Mixer |

#### `Constants.Data.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/App/Constants/Constants.Data.cs` |
| **Class** | `Constants` (partial) |
| **Constants** | `ENCRYPTION_KEY` (string), `ENCRYPTION_IV` (string) — used for save file obfuscation |

#### `Constants.Paths.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/App/Constants/Constants.Paths.cs` |
| **Class** | `Constants` (partial) |
| **Constants** | `SAVES_PATH` (combines Application.persistentDataPath + saves folder) |

#### `WindowConstants.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/App/Constants/WindowConstants.cs` |
| **Class** | `Constants` (partial) |
| **Constants** | All window name strings (e.g. `MAIN_MENU`, `HUD`, `INVENTORY`, `PHOTO_GALLERY`, `SETTINGS`, `PAUSE`) and `AllWindows` (List<string>) |

---

### App.Data

#### `GameData.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/App/Data/GameData.cs` |
| **Class** | `GameData` |
| **Purpose** | Serializable container for all persistent game state. Lives in a single root object that is JSON-serialized to disk. |
| **Fields** | `PlayerData` (PlayerData), `InventoryData` (List<ItemData>), `WorldState` (Dictionary<string, bool>), `StoryProgress` (List<string>), `PhotoData` (List<PhotoData>), `SettingsData` (SettingsData), `LastSaveTime` (DateTime) |

#### `SavesData.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/App/Data/SavesData.cs` |
| **Class** | `SavesData` |
| **Purpose** | Metadata container listing all save slots. Used by save/load UI. |
| **Fields** | `Saves : List<SaveFile>` |

#### `SaveFile.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/App/Data/SaveFile.cs` |
| **Class** | `SaveFile` |
| **Purpose** | Metadata for a single save slot. |
| **Fields** | `Name` (string), `Scene` (string), `DateTime` (string), `PlayTime` (string) |

---

### App.Systems

#### `SaveSystem.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/App/Systems/SaveSystem.cs` |
| **Class** | `SaveSystem : MonoBehaviour` |
| **Purpose** | Manages save/load lifecycle. Coordinates between save UI and data serialization. |
| **Fields** | `_registeredComponents : List<ISaveableComponent>` |
| **Methods** | `Save(string slot)` — iterate all registered components, collect their data, serialize to JSON, encrypt, write to file; `Load(string slot)` — read file, decrypt, deserialize, restore each component; `Delete(string slot)`; `HasSave(string slot)`; `RegisterComponent(ISaveableComponent)`; `UnregisterComponent(ISaveableComponent)`; `GetAllSaves()` — scan saves directory |

#### `SaveSystem.Interfaces.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/App/Systems/SaveSystem.Interfaces.cs` |
| **Interface** | `ISaveableComponent` |
| **Methods** | `SaveData SaveState()`, `LoadState(SaveData data)` |

#### `SaveSystem.Data.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/App/Systems/SaveSystem.Data.cs` |
| **Class** | `SaveData` |
| **Purpose** | Base class for component-specific save data blobs |

#### `Localization.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/App/Systems/Localization.cs` |
| **Class** | `Localization : MonoBehaviour` |
| **Purpose** | Manages multilingual text. Loads CSV/JSON locale files and provides string lookup. |
| **Fields** | `_locale : string` (current language code), `_strings : Dictionary<string, string>` |
| **Methods** | `SetLocale(string code)` — load and switch language; `GetString(string key)` — lookup localized string; `Reload()` — re-parse locale file; `AddListener(Action)`, `RemoveListener(Action)` — notify UI of locale change |

#### `AudioManager.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/App/Systems/AudioManager.cs` |
| **Class** | `AudioManager : AutoEventBehaviour` |
| **Purpose** | Controls all audio: SFX, music, ambient, UI sounds. Uses Unity Audio Mixer groups. |
| **Listeners** | `On(GameEvent.AddItem)` — play pickup sound |
| **Methods** | `PlaySFX(AudioClip clip, float volume)`; `PlayMusic(AudioClip clip, bool loop)`; `PlayAmbient(AudioClip clip)`; `SetMasterVolume(float)`; `SetMusicVolume(float)`; `SetSFXVolume(float)`; `PlayUISound(AudioClip)` |

#### `GameManager.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/App/Systems/GameManager.cs` |
| **Class** | `GameManager : AutoEventBehaviour` |
| **Purpose** | Central game state controller. Tracks game phase (menu, playing, paused, cutscene). |
| **Fields** | `State : GameState` (enum: Menu, Playing, Paused, Cutscene) |
| **Listeners** | `On(GameEvent.LoadNextScene)` — start game; `On(UIEvents.QuitGame)` — quit; `On(UIEvents.StartNewGame)` — reset state; `On(UIEvents.ExitToMainMenu)` — return to menu |
| **Methods** | `Pause()`, `Resume()`, `TogglePause()` |

#### `InputManager.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/App/Systems/InputManager.cs` |
| **Class** | `InputManager : MonoBehaviour` |
| **Purpose** | Wraps Unity Input System. Exposes action maps and provides per-frame input state. |
| **Fields** | `_inputActions : InputSystemActions`, `_playerMap`, `_uiMap`, `_gameMap` |
| **Methods** | `GetMove() : Vector2`; `GetLook() : Vector2`; `IsInteractPressed() : bool`; `IsCrouchHeld() : bool`; `IsSprintHeld() : bool`; `IsFlashlightPressed() : bool`; `IsActionPressed() : bool`; `IsExtraActionPressed() : bool`; `IsInventoryPressed() : bool`; `IsPausePressed() : bool`; `IsSubmitPressed() : bool`; `IsCancelPressed() : bool` |
| **Events** | Fires `InputEvent.OnInteract`, `InputEvent.OnCrouch`, `InputEvent.OnSprint`, `InputEvent.OnFlashlight`, `InputEvent.OnAction`, `InputEvent.OnInventory`, `InputEvent.OnPause` |

#### `InputSystemActions.cs` (auto-generated)
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/App/InputSystemActions.cs` |
| **Class** | `InputSystemActions : IInputActionCollection2, IDisposable` (partial, auto-generated) |
| **Defines** | 3 action maps: **Player** (Move, Look, Interact, Crouch, Sprint, Flashlight, Action, ExtraAction, Inventory), **UI** (Submit, Cancel, Next, Previous), **Game** (Pause) |
| **Interfaces** | `IPlayerActions`, `IUIActions`, `IGameActions` with per-action callback methods |

---

### App.ValueProvider

#### `DialogIDProvider.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/App/ValueProvider/DialogIDProvider.cs` |
| **Class** | `DialogIDProvider` (static) |
| **Purpose** | Editor-only. Provides dropdown values for dialog node IDs and choice IDs in the Inspector. |
| **Methods** | `GetAllNodeIds()`, `GetAllChoiceIds()` — both use `AssetDatabase.FindAssets` to find all DialogNode assets and yield dropdown items |

---

## Scripts.Events

All event classes follow the same pattern: a `static` class containing one or more `struct` definitions. Each struct is a message type for `EventManager<T>`.

### GameEvent (`Scripts.Events.Game`)

| Struct | Fields | Purpose |
|--------|--------|---------|
| `GameEvent.Pause` | *(empty)* | Toggle game pause |
| `GameEvent.InteractHover` | `Interact : Interactable` | Player looking at an interactable |
| `GameEvent.InteractHoverEnd` | `Interact : Interactable` | Player stopped looking at interactable |
| `GameEvent.AddItem` | `Id : int`, `Amount : int` | Item added to inventory |
| `GameEvent.InnerDialogue` | `Text : string` | Show inner monologue text |
| `GameEvent.LoadNextScene` | *(empty)* | Transition to next scene |
| `GameEvent.OnPlayerItemEquip` | `UsableItem : UsableItem` | Player equipped an item |
| `GameEvent.OnPlayerItemUnEquip` | *(empty)* | Player unequipped current item |
| `GameEvent.OnGallery` | *(empty)* | Open photo gallery |

### DialogEvent (`Scripts.Events.Game`)

| Struct | Fields | Purpose |
|--------|--------|---------|
| `DialogEvent.OpenDialog` | `NodeID : string` | Open dialog at given Yarn node |
| `DialogEvent.OnChoice` | `ChoiceID : string` | Player selected a dialog choice |
| `DialogEvent.CloseDialog` | *(empty)* | Close dialog panel |

### PlayerEvent (`Scripts.Events.Game`)

| Struct | Fields | Purpose |
|--------|--------|---------|
| `PlayerEvent.OnTakeDamage` | `Damage : float` | Player took damage |
| `PlayerEvent.OnHeal` | `Health : float` | Player healed |
| `PlayerEvent.OnDie` | *(empty)* | Player died |
| `PlayerEvent.OnSit` | *(empty)* | Player sat down |
| `PlayerEvent.OnStandUp` | *(empty)* | Player stood up |
| `PlayerEvent.OnHide` | *(empty)* | Player entered hiding spot |
| `PlayerEvent.OnExitHide` | *(empty)* | Player left hiding spot |
| `PlayerEvent.OnCrouch` | `IsCrouching : bool` | Crouch state changed |
| `PlayerEvent.OnSprint` | `IsSprinting : bool` | Sprint state changed |
| `PlayerEvent.OnDoorOpen` | *(empty)* | Player opened a door |
| `PlayerEvent.OnSetLevel` | *(empty)* | Player changed floor/level |

### StoryEvent (`Scripts.Events.Story`)

| Struct | Fields | Purpose |
|--------|--------|---------|
| `StoryEvent.OnStoryBeat` | `ID : string` | Story beat triggered |
| `StoryEvent.OnStoryProgress` | `ID : string` | Story progressed to milestone |
| `StoryEvent.OnDialogue` | `Text : string`, `Speaker : string` | Show dialogue line |
| `StoryEvent.OnEnding` | `EndingType : int` | Game ending triggered |

### InputEvent (`Scripts.Events.Input`)

| Struct | Fields | Purpose |
|--------|--------|---------|
| `InputEvent.OnInteract` | *(empty)* | Interact button pressed |
| `InputEvent.OnCrouch` | *(empty)* | Crouch toggled |
| `InputEvent.OnSprint` | *(empty)* | Sprint toggled |
| `InputEvent.OnFlashlight` | *(empty)* | Flashlight toggled |
| `InputEvent.OnAction` | *(empty)* | Action button pressed |
| `InputEvent.OnInventory` | *(empty)* | Inventory toggled |
| `InputEvent.OnPause` | *(empty)* | Pause pressed |

### InventoryEvent (`Scripts.Events.Inventory`)

| Struct | Fields | Purpose |
|--------|--------|---------|
| `InventoryEvent.OnItemAdded` | `Item : InventoryItem`, `Count : int` | Item added |
| `InventoryEvent.OnItemRemoved` | `Item : InventoryItem`, `Count : int` | Item removed |
| `InventoryEvent.OnItemUsed` | `Item : InventoryItem` | Item used |
| `InventoryEvent.OnInventoryOpened` | *(empty)* | Inventory UI opened |
| `InventoryEvent.OnInventoryClosed` | *(empty)* | Inventory UI closed |

### UIEvents (`Scripts.Events.UI`)

| Struct | Fields | Purpose |
|--------|--------|---------|
| `UIEvents.OpenWindow` | `Name : string` | Open window by name |
| `UIEvents.OpenWindowWithContext` | `Name : string`, `Context : object` | Open window with data |
| `UIEvents.CloseWindow` | `Name : string` | Close specific window |
| `UIEvents.CloseLastWindow` | *(empty)* | Close top window |
| `UIEvents.QuitGame` | *(empty)* | Exit application |
| `UIEvents.StartNewGame` | *(empty)* | Start new game |
| `UIEvents.ExitToMainMenu` | *(empty)* | Return to main menu |

### PreviewEvent (`Scripts.Events.Preview`)

| Struct | Fields | Purpose |
|--------|--------|---------|
| `PreviewEvent.Drag` | `Delta : Vector2` | Drag model in preview |
| `PreviewEvent.ShowNext` | `NextModel : Transform`, `Scale : Vector3` | Show next model variant |
| `PreviewEvent.ShowPrevious` | `PreviousModel : Transform`, `Scale : Vector3` | Show previous model variant |
| `PreviewEvent.Show` | `Model : Transform`, `Scale : Vector3` | Show specific model |

### JournalEvent (`Scripts.Events.Journal`)

| Struct | Fields | Purpose |
|--------|--------|---------|
| `JournalEvent.OnNoteAdded` | `Data : NoteData` | Note/document added |
| `JournalEvent.OnJournalOpened` | *(empty)* | Journal UI opened |
| `JournalEvent.OnJournalClosed` | *(empty)* | Journal UI closed |

### SettingsEvent (`Scripts.Events.Settings`)

| Struct | Fields | Purpose |
|--------|--------|---------|
| `SettingsEvent.OnVolumeChanged` | `Type : string`, `Value : float` | Volume setting changed |
| `SettingsEvent.OnQualityChanged` | `Level : int` | Graphics quality changed |
| `SettingsEvent.OnControlsChanged` | *(empty)* | Control bindings changed |

### CutsceneEvent (`Scripts.Events.Cutscene`)

| Struct | Fields | Purpose |
|--------|--------|---------|
| `CutsceneEvent.OnCutsceneStart` | `ID : string` | Cutscene started |
| `CutsceneEvent.OnCutsceneEnd` | `ID : string` | Cutscene ended |

### SaveEvent (`Scripts.Events.Save`)

| Struct | Fields | Purpose |
|--------|--------|---------|
| `SaveEvent.OnSave` | `Slot : string` | Game saved |
| `SaveEvent.OnLoad` | `Slot : string` | Game loaded |
| `SaveEvent.OnDelete` | `Slot : string` | Save deleted |

### FlashlightEvent (`Scripts.Events.Flashlight`)

| Struct | Fields | Purpose |
|--------|--------|---------|
| `FlashlightEvent.OnFlashlightEnabled` | *(empty)* | Flashlight turned on |
| `FlashlightEvent.OnFlashlightDisabled` | *(empty)* | Flashlight turned off |
| `FlashlightEvent.OnFlashlightBatteryLow` | *(empty)* | Battery low warning |
| `FlashlightEvent.OnFlashlightBatteryDead` | *(empty)* | Battery depleted |

### StaminaEvent (`Scripts.Events.Game`)

| Struct | Fields | Purpose |
|--------|--------|---------|
| `StaminaEvent.OnStaminaChanged` | `Value : float`, `MaxValue : float` | Stamina value changed |
| `StaminaEvent.OnStaminaExhausted` | *(empty)* | Stamina fully depleted |
| `StaminaEvent.OnStaminaRecovered` | *(empty)* | Stamina fully recovered |

### SanityEvent (`Scripts.Events.Game`)

| Struct | Fields | Purpose |
|--------|--------|---------|
| `SanityEvent.OnSanityChanged` | `Value : float`, `MaxValue : float` | Sanity changed |
| `SanityEvent.OnSanityCritical` | *(empty)* | Sanity critically low |
| `SanityEvent.OnSanityRestored` | *(empty)* | Sanity fully restored |
| `SanityEvent.OnHallucination` | *(empty)* | Hallucination effect triggered |

### TutorialEvent (`Scripts.Events.Tutorial`)

| Struct | Fields | Purpose |
|--------|--------|---------|
| `TutorialEvent.OnTutorialShow` | `ID : string` | Show tutorial hint |
| `TutorialEvent.OnTutorialHide` | `ID : string` | Hide tutorial hint |

### PhotoEvents (`Scripts.Events.Photo`)

| Struct | Fields | Purpose |
|--------|--------|---------|
| `PhotoEvents.OnPhotoTaken` | `PhotoSprite : Sprite` | Photo captured |
| `PhotoEvents.OnPhotoSaved` | `PhotoID : int` | Photo saved to gallery |
| `PhotoEvents.OnPhotoDeleted` | `PhotoID : int` | Photo deleted from gallery |

---

## Scripts.Game

### Game.Items

#### `Item.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/Game/Items/Item.cs` |
| **Class** | `Item : ScriptableObject` |
| **Purpose** | Base class for all item definitions. ScriptableObject so every item is an asset. |
| **Fields** | `ID : int`, `Name : string`, `Description : string`, `Icon : Sprite`, `Weight : float`, `MaxStack : int`, `IsUsable : bool` |
| **Methods** | `Use()` — virtual, override in subclasses |

#### `InventoryItem.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/Game/Items/InventoryItem.cs` |
| **Class** | `InventoryItem : Item` |
| **Purpose** | Item that can be picked up and stored in inventory. |
| **Fields** | *(inherits from Item)* |
| **Methods** | *(inherits)* |

#### `UsableItem.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/Game/Items/UsableItem.cs` |
| **Class** | `UsableItem : InventoryItem` |
| **Purpose** | Item with a specific use action (key, medkit, battery, etc.). |
| **Fields** | `UseEffect : ItemEffect` (ScriptableObject), `UseSound : AudioClip`, `DestroyOnUse : bool` |
| **Methods** | `Use()` — apply UseEffect, play sound, optionally consume item |

#### `StoryItem.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/Game/Items/StoryItem.cs` |
| **Class** | `StoryItem : InventoryItem` |
| **Purpose** | Plot-relevant item (keycard, document, photo). Cannot be discarded. |
| **Fields** | `StoryID : string` — unique identifier for story tracking |
| **Methods** | `Use()` — fire `StoryEvent.OnStoryBeat` with StoryID |

#### `EquipSlot.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/Game/Items/EquipSlot.cs` |
| **Enum** | `EquipSlot` |
| **Values** | `None`, `Head`, `Torso`, `Hand`, `Tool`, `LightSource` |

#### `IItemContainer.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/Game/Items/IItemContainer.cs` |
| **Interface** | `IItemContainer` |
| **Methods** | `CanAddItem(Item, int) : bool`, `AddItem(Item, int)`, `RemoveItem(Item, int)`, `GetItemCount(Item) : int`, `ContainsItem(Item) : bool`, `Clear()` |

#### `ItemDatabase.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/Game/Items/ItemDatabase.cs` |
| **Class** | `ItemDatabase : MonoBehaviour` |
| **Purpose** | Runtime registry of all `Item` ScriptableObjects. Provides lookup by ID. |
| **Fields** | `_items : List<Item>` (assigned in Inspector) |
| **Methods** | `GetItemByID(int) : Item`; `GetItemByName(string) : Item`; `GetAllItems() : List<Item>` |

### Game.Player

#### `PlayerController.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/Game/Player/PlayerController.cs` |
| **Class** | `PlayerController : AutoEventBehaviour` |
| **Purpose** | Top-level player coordinator. Holds references to all player subsystems and delegates to them. |
| **Fields** | `Movement : PlayerMovement`, `Camera : PlayerCamera`, `Interaction : PlayerInteraction`, `Stamina : StaminaSystem`, `Flashlight : FlashlightSystem`, `Sanity : SanitySystem`, `Inventory : PlayerInventory`, `Photo : PhotoSystem` |
| **Listeners** | `On(UIEvents.OpenWindow)` — disable control; `On(UIEvents.CloseLastWindow)` — enable control |
| **Methods** | `Awake()` — find subsystems via GetComponent/GetComponentInChildren; `Update()` — delegate to subsystems |

#### `PlayerMovement.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/Game/Player/PlayerMovement.cs` |
| **Class** | `PlayerMovement : MonoBehaviour` |
| **Purpose** | Handles character movement: walking, sprinting, crouching. |
| **Fields** | `_controller : CharacterController`, `_speed : float`, `_sprintMultiplier : float`, `_crouchSpeed : float`, `_crouchHeight : float`, `_gravity : float`, `_jumpForce : float`, `_inputManager : InputManager` |
| **Methods** | `Update()` — read InputManager, apply movement; `Crouch(bool)`, `Sprint(bool)`; `IsGrounded() : bool` |
| **Fires** | `PlayerEvent.OnCrouch`, `PlayerEvent.OnSprint` |

#### `PlayerCamera.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/Game/Player/PlayerCamera.cs` |
| **Class** | `PlayerCamera : MonoBehaviour` |
| **Purpose** | First-person camera with mouse look. Optional head bob and weapon sway. |
| **Fields** | `_camera : Camera`, `_sensitivity : Vector2`, `_xRotation : float`, `_yRotation : float`, `_headBob : bool`, `_headBobSpeed : float`, `_headBobAmount : float` |
| **Methods** | `Update()` — read InputManager.GetLook(), apply rotation; `SetSensitivity(float)`, `SetFOV(float, bool)` |

#### `PlayerInteraction.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/Game/Player/PlayerInteraction.cs` |
| **Class** | `PlayerInteraction : MonoBehaviour` |
| **Purpose** | Raycast-based interaction detection. Shoots a ray from camera center to find `Interactable` objects. |
| **Fields** | `_interactRange : float`, `_camera : Camera`, `_currentInteractable : Interactable` |
| **Methods** | `Update()` — raycast, track hover state, fire events; `Interact()` — call `_currentInteractable.Interact()` |
| **Fires** | `GameEvent.InteractHover`, `GameEvent.InteractHoverEnd` |

#### `PlayerInventory.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/Game/Player/PlayerInventory.cs` |
| **Class** | `PlayerInventory : MonoBehaviour, IItemContainer` |
| **Purpose** | Manages player inventory — list of held items. |
| **Fields** | `_items : List<InventoryItem>`, `_maxSlots : int`, `_equippedItem : UsableItem` |
| **Methods** | `AddItem(Item, int)` — fire GameEvent.AddItem; `RemoveItem(Item, int)`; `HasItem(int id) : bool`; `GetItemCount(int) : int`; `Equip(UsableItem)` — fire GameEvent.OnPlayerItemEquip; `Unequip()`; `UseEquipped()` |
| **Implements** | `IItemContainer` |

#### `PlayerHealth.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/Game/Player/PlayerHealth.cs` |
| **Class** | `PlayerHealth : AutoEventBehaviour` |
| **Purpose** | Player health pool with damage/heal/death handling. |
| **Fields** | `_maxHealth : float`, `_currentHealth : float`, `_isDead : bool` |
| **Listeners** | `On(PlayerEvent.OnTakeDamage)` — reduce health, check death; `On(PlayerEvent.OnHeal)` — restore health |
| **Methods** | `TakeDamage(float)`, `Heal(float)`, `Die()` — fire PlayerEvent.OnDie, trigger death sequence |
| **Fires** | `PlayerEvent.OnTakeDamage`, `PlayerEvent.OnDie` |

### Game.Mechanics

#### `StaminaSystem.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/Game/Mechanics/StaminaSystem.cs` |
| **Class** | `StaminaSystem : AutoEventBehaviour` |
| **Purpose** | Manages player stamina. Depletes while sprinting, recovers while idle/walking. |
| **Fields** | `_maxStamina : float`, `_currentStamina : float`, `_drainRate : float`, `_recoverRate : float`, `_recoverDelay : float`, `_isExhausted : bool` |
| **Listeners** | `On(PlayerEvent.OnSprint)` — start/stop drain |
| **Methods** | `Update()` — tick stamina drain/recovery; `Exhaust()` — set exhausted flag, fire StaminaEvent.OnStaminaExhausted; `Recover()` — fire StaminaEvent.OnStaminaRecovered |
| **Fires** | `StaminaEvent.OnStaminaChanged`, `StaminaEvent.OnStaminaExhausted`, `StaminaEvent.OnStaminaRecovered` |

#### `SanitySystem.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/Game/Mechanics/SanitySystem.cs` |
| **Class** | `SanitySystem : AutoEventBehaviour` |
| **Purpose** | Player sanity mechanic. Decreases in darkness/near enemies/ witnessing events. Triggers hallucinations at critical levels. |
| **Fields** | `_maxSanity : float`, `_currentSanity : float`, `_darkDrainRate : float`, `_enemyProximityDrain : float`, `_recoverRateInLight : float`, `_criticalThreshold : float`, `_isHallucinating : bool` |
| **Listeners** | `On(SanityEvent.OnSanityChanged)` |
| **Methods** | `Update()` — check environment, tick sanity; `TriggerHallucination()` — fire event, random hallucination effect |
| **Fires** | `SanityEvent.OnSanityChanged`, `SanityEvent.OnSanityCritical`, `SanityEvent.OnHallucination` |

#### `FlashlightSystem.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/Game/Mechanics/FlashlightSystem.cs` |
| **Class** | `FlashlightSystem : AutoEventBehaviour` |
| **Purpose** | Player flashlight with battery drain. Can be toggled on/off. |
| **Fields** | `_light : Light`, `_maxBattery : float`, `_currentBattery : float`, `_drainRate : float`, `_isOn : bool`, `_intensity : float`, `_range : float`, `_angle : float` |
| **Methods** | `Toggle()` — on/off, fire events; `Update()` — if on, drain battery; `Recharge(float)` — restore battery |
| **Fires** | `FlashlightEvent.OnFlashlightEnabled`, `FlashlightEvent.OnFlashlightDisabled`, `FlashlightEvent.OnFlashlightBatteryLow` |

#### `PhotoSystem.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/Game/Mechanics/PhotoSystem.cs` |
| **Class** | `PhotoSystem : AutoEventBehaviour` |
| **Purpose** | Photo capture mechanic. Player can take photos of the environment. Photos are stored in gallery. |
| **Fields** | `_camera : Camera` (photo camera), `_photoResolution : Vector2Int`, `_maxPhotos : int`, `_photos : List<Sprite>` |
| **Methods** | `TakePhoto()` — render camera to RenderTexture, capture to Sprite, fire PhotoEvents.OnPhotoTaken; `HasSpace() : bool`; `GetPhotos() : List<Sprite>` |
| **Fires** | `PhotoEvents.OnPhotoTaken` |

#### `InteractionController.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/Game/Mechanics/InteractionController.cs` |
| **Class** | `InteractionController : AutoEventBehaviour` |
| **Purpose** | Handles Interact action from InputManager. Delegates to PlayerInteraction.Interact(). |
| **Listeners** | `On(InputEvent.OnInteract)` — call `PlayerInteraction.Interact()` |

#### `TimerClock.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/Game/Mechanics/TimerClock.cs` |
| **Class** | `TimerClock : MonoBehaviour` |
| **Purpose** | In-game time tracking. Counts elapsed play time. |
| **Fields** | `_elapsedTime : float` |
| **Methods** | `Update()` — accumulate deltaTime; `GetElapsedTime() : TimeSpan`; `Reset()` |
| **Editor** | `[ExecuteAlways]` — also runs in edit mode for testing |

### Game.Enemies

#### `EnemyBase.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/Game/Enemies/EnemyBase.cs` |
| **Class** | `EnemyBase : MonoBehaviour` |
| **Purpose** | Base class for all enemy types. Shared fields and behavior. |
| **Fields** | `_health : float`, `_speed : float`, `_detectionRange : float`, `_attackRange : float`, `_attackDamage : float`, `_state : EnemyState`, `_player : Transform` |
| **Methods** | `TakeDamage(float)`, `Die()`, `Alert()`, `Calm()` — virtual, override in subclasses |

#### `EnemyAI.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/Game/Enemies/EnemyAI.cs` |
| **Class** | `EnemyAI : EnemyBase` |
| **Purpose** | Patrol-chase AI. Follows waypoints in patrol state, chases player on detection. |
| **Fields** | `_waypoints : List<Transform>`, `_currentWaypoint : int`, `_detectionTimer : float`, `_detectionTime : float`, `_chaseSpeed : float`, `_patrolSpeed : float` |
| **Methods** | `Update()` — state machine tick; `Patrol()` — move along waypoints; `Chase()` — pursue player; `Search()` — investigate last known position; `DetectPlayer()` — check distance and line of sight |

#### `EnemyState.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/Game/Enemies/EnemyState.cs` |
| **Enum** | `EnemyState` |
| **Values** | `Idle`, `Patrol`, `Alert`, `Chase`, `Search`, `Attack`, `Return` |

#### `EnemyDetection.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/Game/Enemies/EnemyDetection.cs` |
| **Class** | `EnemyDetection : MonoBehaviour` |
| **Purpose** | Detection sensor for enemy. Uses cone + distance + line-of-sight check. |
| **Fields** | `_viewAngle : float`, `_viewDistance : float`, `_hearingRadius : float`, `_playerLayer : LayerMask`, `_obstacleLayer : LayerMask` |
| **Methods** | `CanSeePlayer(Transform) : bool` — angle + raycast; `CanHearPlayer(Vector3) : bool` — distance check |

### Game.Interactables

#### `Interactable.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/Game/Interactables/Interactable.cs` |
| **Class** | `Interactable : MonoBehaviour` |
| **Purpose** | Abstract base for all interactable objects in the world. |
| **Fields** | `_interactText : string` (UI hint), `_canInteract : bool`, `_highlightMaterial : Material`, `_isHighlighted : bool` |
| **Methods** | `Interact()` — abstract; `OnHoverEnter()` — apply highlight; `OnHoverExit()` — remove highlight; `CanInteract() : bool` |

#### `PickupItem.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/Game/Interactables/PickupItem.cs` |
| **Class** | `PickupItem : Interactable` |
| **Purpose** | Item that can be picked up and added to inventory. |
| **Fields** | `_item : InventoryItem`, `_amount : int`, `_pickupEffect : GameObject` (VFX), `_pickupSound : AudioClip` |
| **Methods** | `Interact()` — add item to PlayerInventory, play effect/sound, destroy self |

#### `Door.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/Game/Interactables/Door.cs` |
| **Class** | `Door : Interactable` |
| **Purpose** | Openable/closable door. Supports locked doors that require a key item. |
| **Fields** | `_isOpen : bool`, `_isLocked : bool`, `_requiredItemID : int`, `_openRotation : Quaternion`, `_closeRotation : Quaternion`, `_openSpeed : float`, `_openSound : AudioClip`, `_closeSound : AudioClip` |
| **Methods** | `Interact()` — if locked, check inventory for key; if unlocked, toggle open/close; `Open()`, `Close()` — animate rotation via lerp |

#### `Note.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/Game/Interactables/Note.cs` |
| **Class** | `Note : Interactable` |
| **Purpose** | Readable note/document found in the world. Opens a note-reading UI. |
| **Fields** | `_noteData : NoteData` (ScriptableObject with title, text, image) |
| **Methods** | `Interact()` — fire JournalEvent.OnNoteAdded, open note UI |

#### `PuzzleElement.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/Game/Interactables/PuzzleElement.cs` |
| **Class** | `PuzzleElement : Interactable` |
| **Purpose** | Base class for puzzle interactables. Each element contributes to a puzzle solution. |
| **Fields** | `_puzzleID : string`, `_isSolved : bool`, `_elementIndex : int` |
| **Methods** | `Interact()` — fire puzzle interaction event; `OnPuzzleSolve()` — virtual |

#### `Lever.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/Game/Interactables/Lever.cs` |
| **Class** | `Lever : PuzzleElement` |
| **Purpose** | Interactable lever. Toggles state on interaction. |
| **Fields** | `_isOn : bool`, `_onRotation : Quaternion`, `_offRotation : Quaternion` |
| **Methods** | `Interact()` — toggle, animate; `GetState() : bool` |

#### `ButtonPad.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/Game/Interactables/ButtonPad.cs` |
| **Class** | `ButtonPad : PuzzleElement` |
| **Purpose** | Push button. Can be part of a sequence puzzle. |
| **Fields** | `_buttonID : int`, `_pressed : bool`, `_pressDepth : float`, `_pressSound : AudioClip` |
| **Methods** | `Interact()` — press, animate, fire event with buttonID |

#### `Keypad.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/Game/Interactables/Keypad.cs` |
| **Class** | `Keypad : Interactable` |
| **Purpose** | Numeric keypad puzzle. Player enters a code to unlock. |
| **Fields** | `_correctCode : string`, `_enteredCode : string`, `_maxDigits : int`, `_isUnlocked : bool`, `_display : TextMeshPro` |
| **Methods** | `Interact()` — open keypad UI; `EnterDigit(string)`; `Clear()`; `Confirm()` — check code, unlock if correct |

---

## Scripts.Windows

### Base

#### `BaseWindow.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/Windows/Base/BaseWindow.cs` |
| **Class** | `BaseWindow : MonoBehaviour` |
| **Purpose** | Abstract base for all UI windows. Provides common open/close lifecycle. |
| **Fields** | `_isOpen : bool`, `_animator : Animator`, `_openTrigger : string`, `_closeTrigger : string` |
| **Methods** | `Open()` — virtual, activate gameobject, play open anim; `Close()` — virtual, play close anim, deactivate; `OnOpened()`, `OnClosed()` — virtual callbacks |

#### `WindowSwitcher.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/Windows/Base/WindowSwitcher.cs` |
| **Class** | `WindowSwitcher : AutoEventBehaviour` |
| **Purpose** | Manages window stack. Each window has a unique name. Supports opening with context data. |
| **Fields** | `_windowPrefabs : List<WindowPrefab>` (serialized), `_windowCache : Dictionary<string, BaseWindow>`, `_windowStack : List<BaseWindow>` |
| **Listeners** | `On(UIEvents.OpenWindow)` — open; `On(UIEvents.OpenWindowWithContext)` — open with context; `On(UIEvents.CloseWindow)` — close by name; `On(UIEvents.CloseLastWindow)` — pop stack; `On(UIEvents.QuitGame)`; `On(UIEvents.StartNewGame)`; `On(UIEvents.ExitToMainMenu)` |
| **Methods** | `OpenWindow(string, object)` — instantiate or get from cache, push to stack; `CloseWindow(string)` — find in stack, close; `CloseLast()` — pop; `CloseAll()`; `TryGetWindow<T>(string)` |

#### `WindowController.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/Windows/Base/WindowController.cs` |
| **Class** | `WindowController : MonoBehaviour` |
| **Purpose** | Per-window controller. Holds logic for each specific window type. |
| **Methods** | `Setup(object context)` — called when window is opened with context data |

#### `WindowPrefab.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/Windows/Base/WindowPrefab.cs` |
| **Class** | `WindowPrefab` (Serializable) |
| **Purpose** | Pair of window name + prefab reference for Inspector assignment. |
| **Fields** | `Name : string`, `Prefab : BaseWindow` |

### Windows.MainMenu

#### `MainMenu.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/Windows/MainMenu/MainMenu.cs` |
| **Class** | `MainMenu : BaseWindow` |
| **Purpose** | Main menu screen. Buttons: New Game, Continue, Settings, Gallery, Quit. |
| **Fields** | `_newGameButton : Button`, `_continueButton : Button`, `_settingsButton : Button`, `_galleryButton : Button`, `_quitButton : Button` |
| **Methods** | `Open()` — check for existing saves (enable/disable Continue button); `OnNewGame()` — fire UIEvents.StartNewGame; `OnContinue()` — load latest save; `OnSettings()` — open settings window; `OnGallery()` — open photo gallery; `OnQuit()` — fire UIEvents.QuitGame |

#### `StartButton.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/Windows/MainMenu/StartButton.cs` |
| **Class** | `StartButton : MonoBehaviour` |
| **Purpose** | UI button that fires `UIEvents.StartNewGame` on click. |
| **Method** | `OnClick()` — Broadcast(UIEvents.StartNewGame) |

### Windows.Game

#### `HUD.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/Windows/Game/HUD.cs` |
| **Class** | `HUD : AutoEventBehaviour` |
| **Purpose** | Primary gameplay HUD. Shows health, sanity, stamina, ammo, interaction prompts. |
| **Fields** | `_healthBar : Slider`, `_sanityBar : Slider`, `_staminaBar : Slider`, `_interactPrompt : TextMeshProUGUI`, `_itemIcon : Image`, `_itemName : TextMeshProUGUI`, `_flashlightIcon : Image`, `_crosshair : Image` |
| **Listeners** | `On(PlayerEvent.OnTakeDamage)` — update health bar; `On(SanityEvent.OnSanityChanged)` — update sanity bar; `On(StaminaEvent.OnStaminaChanged)` — update stamina bar; `On(GameEvent.InteractHover)` — show prompt; `On(GameEvent.InteractHoverEnd)` — hide prompt; `On(FlashlightEvent.OnFlashlightEnabled)` — update icon; `On(FlashlightEvent.OnFlashlightDisabled)` — update icon; `On(GameEvent.InnerDialogue)` — show inner text |

#### `InventoryWindow.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/Windows/Game/InventoryWindow.cs` |
| **Class** | `InventoryWindow : BaseWindow` |
| **Purpose** | Grid-based inventory UI. Shows items with icons, allows equipping/using/dropping. |
| **Fields** | `_grid : GridLayoutGroup`, `_slotPrefab : InventorySlot`, `_itemTooltip : Tooltip`, `_categoryTabs : ToggleGroup` |
| **Methods** | `Open()` — refresh grid from PlayerInventory; `Refresh()` — rebuild item slots; `OnSlotClick(InventorySlot)` — use/equip; `OnSlotRightClick(InventorySlot)` — context menu; `FilterByCategory(string)` |

#### `InventorySlot.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/Windows/Game/InventoryWindow.cs` (inner class) |
| **Class** | `InventorySlot : MonoBehaviour` |
| **Purpose** | Single slot in inventory grid. Displays item icon, stack count. |
| **Fields** | `_icon : Image`, `_countText : TextMeshProUGUI`, `_item : InventoryItem`, `_isSelected : bool` |
| **Methods** | `SetItem(InventoryItem, int)`; `Clear()`; `OnClick()`; `OnRightClick()` |

#### `DialogPanel.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/Windows/Game/DialogPanel.cs` |
| **Class** | `DialogPanel : BaseWindow` |
| **Purpose** | NPC dialog UI. Shows speaker name, text, and choice buttons. |
| **Fields** | `_speakerName : TextMeshProUGUI`, `_dialogText : TextMeshProUGUI`, `_choicesContainer : Transform`, `_choiceButtonPrefab : GameObject`, `_typewriterSpeed : float` |
| **Listeners** | `On(DialogEvent.OpenDialog)` — show dialog; `On(DialogEvent.OnChoice)` — handle choice; `On(DialogEvent.CloseDialog)` — close panel |
| **Methods** | `ShowNode(string nodeID)` — load Yarn node; `ShowChoices(List<Choice>)` — create choice buttons; `OnChoiceSelected(string choiceID)` — fire DialogEvent.OnChoice; `TypewriteText(string)` — animate text reveal |

#### `PhotoGallery.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/Windows/Game/PhotoGallery.cs` |
| **Class** | `PhotoGallery : BaseWindow` |
| **Purpose** | Photo gallery window. Shows all taken photos in a grid. Supports fullscreen preview. |
| **Fields** | `_grid : GridLayoutGroup`, `_photoPrefab : Photo`, `_previewImage : Image`, `_fullscreenView : GameObject`, `_deleteButton : Button` |
| **Listeners** | `On(PhotoEvents.OnPhotoTaken)` — refresh grid |
| **Methods** | `Open()` — load photos from PhotoSystem; `Refresh()` — spawn photo prefabs; `OnPhotoClick(Sprite)` — show fullscreen; `OnDelete()` — delete current photo; `OnExport()` — save to disk |

#### `Photo.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/Windows/Game/PhotoGallery/Photo.cs` |
| **Class** | `Photo : MonoBehaviour` |
| **Purpose** | UI element for a single photo thumbnail in the gallery. |
| **Fields** | `_image : Image`, `_button : Button` |
| **Methods** | `SetImage(Sprite)` — set thumbnail sprite |

#### `JournalWindow.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/Windows/Game/JournalWindow.cs` |
| **Class** | `JournalWindow : BaseWindow` |
| **Purpose** | Journal/notes UI. Shows collected notes and story documents. |
| **Fields** | `_notesList : Transform`, `_notePrefab : NoteUI`, `_contentArea : TextMeshProUGUI`, `_titleArea : TextMeshProUGUI`, `_categories : ToggleGroup` |
| **Listeners** | `On(JournalEvent.OnNoteAdded)` — refresh list |
| **Methods** | `Open()` — load notes from save data; `OnNoteSelected(NoteData)` — show content |

#### `PauseMenu.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/Windows/Game/PauseMenu.cs` |
| **Class** | `PauseMenu : BaseWindow` |
| **Purpose** | Pause overlay. Buttons: Resume, Settings, Save, Load, Main Menu. |
| **Fields** | `_resumeButton : Button`, `_settingsButton : Button`, `_saveButton : Button`, `_loadButton : Button`, `_mainMenuButton : Button` |
| **Methods** | `Open()` — set time scale to 0; `Close()` — set time scale to 1; `OnResume()`; `OnSave()` — open save UI; `OnLoad()` — open load UI; `OnMainMenu()` — confirm then exit |

#### `SettingsWindow.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/Windows/Game/SettingsWindow.cs` |
| **Class** | `SettingsWindow : BaseWindow` |
| **Purpose** | Settings screen. Tabs: Audio, Video, Controls. |
| **Fields** | `_masterSlider : Slider`, `_musicSlider : Slider`, `_sfxSlider : Slider`, `_qualityDropdown : Dropdown`, `_resolutionDropdown : Dropdown`, `_fullscreenToggle : Toggle`, `_sensitivitySlider : Slider`, `_invertYToggle : Toggle` |
| **Methods** | `Open()` — load current settings; `OnMasterVolume(float)`; `OnMusicVolume(float)`; `OnSFXVolume(float)`; `OnQualityChanged(int)`; `OnResolutionChanged(int)`; `OnFullscreen(bool)`; `OnSensitivityChanged(float)`; `OnApply()`; `OnReset()` |

#### `SaveLoadWindow.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/Windows/Game/SaveLoadWindow.cs` |
| **Class** | `SaveLoadWindow : BaseWindow` |
| **Purpose** | Save/Load UI with slot grid. Each slot shows screenshot, scene name, date, play time. |
| **Fields** | `_slots : List<SaveSlot>`, `_slotPrefab : SaveSlot`, `_mode : SaveLoadMode` (Save/Load), `_modeToggle : ToggleGroup`, `_overwriteConfirm : GameObject` |
| **Methods** | `Open()` — set mode, refresh slots; `RefreshSlots()` — populate from SaveSystem; `OnSlotClick(int slotIndex)` — save or load; `OnDeleteSlot(int)`; `OnModeToggle(SaveLoadMode)` |

#### `SaveSlot.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/Windows/Game/SaveLoadWindow.cs` (inner) |
| **Class** | `SaveSlot : MonoBehaviour` |
| **Purpose** | Single slot display in save/load screen. |
| **Fields** | `_screenshot : RawImage`, `_sceneName : TextMeshProUGUI`, `_dateTime : TextMeshProUGUI`, `_playTime : TextMeshProUGUI`, `_isEmpty : bool` |
| **Methods** | `SetData(SaveFile)`, `SetEmpty()`, `OnClick()`, `OnDelete()` |

### Windows.Game.Dialog

#### `DialogСhoice` (⚠️ note: Cyrillic 'С' not Latin 'C')
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/Windows/Game/Dialog/DialogСhoice.cs` |
| **Class** | `DialogСhoice : MonoBehaviour` |
| **Purpose** | A single choice button in the dialog UI. |
| **Fields** | `_text : TextMeshProUGUI`, `_button : Button` |
| **Methods** | `Setup(string choiceID, string text, Action<string> callback)` — set label, wire click |

### Windows.Game.Preview

#### `PreviewWindow.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/Windows/Game/Preview/PreviewWindow.cs` |
| **Class** | `PreviewWindow : BaseWindow` |
| **Purpose** | 3D model preview window. Used for examining items or documents in 3D. |
| **Fields** | `_previewCamera : Camera`, `_modelContainer : Transform`, `_dragSensitivity : float`, `_zoomSpeed : float`, `_minZoom : float`, `_maxZoom : float`, `_currentModel : Transform` |
| **Listeners** | `On(PreviewEvent.Show)` — load model; `On(PreviewEvent.Drag)` — rotate; `On(PreviewEvent.ShowNext)` — next variant; `On(PreviewEvent.ShowPrevious)` — previous variant |
| **Methods** | `ShowModel(Transform, Vector3)`; `RotateModel(Vector2)`; `Zoom(float)`; `NextVariant()`; `PreviousVariant()` |

#### `PreviewDragHandler.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/Windows/Game/Preview/PreviewDragHandler.cs` |
| **Class** | `PreviewDragHandler : MonoBehaviour, IDragHandler` |
| **Purpose** | Captures mouse drag events on the preview area and fires `PreviewEvent.Drag`. |
| **Methods** | `OnDrag(PointerEventData)` — fire PreviewEvent.Drag with delta |

---

## Scripts.World

### World.Core

#### `AmbientSound.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/World/Core/AmbientSound.cs` |
| **Class** | `AmbientSound : MonoBehaviour` |
| **Purpose** | Ambient audio source in the world. Can be positional or global. |
| **Fields** | `_audioSource : AudioSource`, `_soundType : AmbientType` (Wind, Drip, Creak, Hum, Distant), `_minInterval : float`, `_maxInterval : float`, `_playOnStart : bool` |
| **Methods** | `Start()` — if playOnStart, play; `Play()` — play one-shot; `PlayLoop()` — loop |

#### `MovingPlatform.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/World/Core/MovingPlatform.cs` |
| **Class** | `MovingPlatform : MonoBehaviour` |
| **Purpose** | Platform that moves between waypoints. Can be triggered or timed. |
| **Fields** | `_waypoints : List<Transform>`, `_speed : float`, `_currentIndex : int`, `_waitTime : float`, `_isMoving : bool`, `_playerCanRide : bool` |
| **Methods** | `Update()` — move toward next waypoint; `StartMove()`, `StopMove()`, `GoToWaypoint(int)` |

#### `LightingController.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/World/Core/LightingController.cs` |
| **Class** | `LightingController : AutoEventBehaviour` |
| **Purpose** | Controls global lighting. Supports day/night cycle (if applicable) and flicker effects. |
| **Fields** | `_mainLight : Light`, `_ambientColor : Color`, `_flickerIntensity : float`, `_flickerSpeed : float`, `_isFlickering : bool` |
| **Methods** | `SetAmbient(Color)`, `SetMainLightIntensity(float)`, `StartFlicker(float duration)`, `StopFlicker()` |

#### `VolumeController.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/World/Core/VolumeController.cs` |
| **Class** | `VolumeController : AutoEventBehaviour` |
| **Purpose** | Controls URP Volume (post-processing). Blends between volume profiles for effects like sanity degradation. |
| **Fields** | `_defaultProfile : VolumeProfile`, `_sanityLowProfile : VolumeProfile`, `_hallucinationProfile : VolumeProfile`, `_transitionSpeed : float`, `_currentVolume : Volume` |
| **Listeners** | `On(SanityEvent.OnSanityChanged)` — blend toward sanityLowProfile based on sanity ratio; `On(SanityEvent.OnHallucination)` — blend to hallucination profile |

### World.Interactables

#### `WorldPickupItem.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/World/Core/WorldPickupItem.cs` |
| **Class** | `WorldPickupItem : Interactable` |
| **Purpose** | World-placed item that can be picked up. Differs from PickupItem in that it uses a physical 3D model. |
| **Fields** | `_item : InventoryItem`, `_amount : int`, `_pickupEffect : GameObject`, `_rotateSpeed : float`, `_bobSpeed : float`, `_bobHeight : float` |
| **Methods** | `Update()` — idle rotation + bobbing animation; `Interact()` — add to inventory, play effect |

#### `WorldPickupUsable.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/World/Core/WorldPickupUsable.cs` |
| **Class** | `WorldPickupUsable : WorldPickupItem` |
| **Purpose** | Like WorldPickupItem but for UsableItem. Adds equipment auto-equip behavior. |
| **Fields** | `_autoEquip : bool` |
| **Methods** | `Interact()` — pickup + optionally auto-equip |

### World.Triggers

#### `BaseTrigger.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/World/Triggers/BaseTrigger.cs` |
| **Class** | `BaseTrigger : MonoBehaviour` |
| **Purpose** | Abstract base for all trigger types. Detects player entry/exit. |
| **Fields** | `_playerTag : string`, `_triggerOnEnter : bool`, `_triggerOnExit : bool`, `_oneTime : bool`, `_hasTriggered : bool`, `_delay : float` |
| **Methods** | `OnTriggerEnter(Collider)` — if player, call `OnTriggerActivated()`; `OnTriggerExit(Collider)` — if player, call `OnTriggerDeactivated()`; `OnTriggerActivated()` — abstract; `OnTriggerDeactivated()` — virtual |

#### `StoryTrigger.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/World/Triggers/StoryTrigger.cs` |
| **Class** | `StoryTrigger : BaseTrigger` |
| **Purpose** | Fires story event when player enters trigger zone. |
| **Fields** | `_storyID : string`, `_fireOnlyOnce : bool` |
| **Methods** | `OnTriggerActivated()` — fire `StoryEvent.OnStoryBeat` with storyID |

#### `DialogTrigger.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/World/Triggers/DialogTrigger.cs` |
| **Class** | `DialogTrigger : BaseTrigger` |
| **Purpose** | Opens a dialog node when player enters trigger zone. |
| **Fields** | `_nodeID : string` |
| **Methods** | `OnTriggerActivated()` — fire `DialogEvent.OpenDialog` with nodeID |

#### `InnerDialogTrigger.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/World/Triggers/InnerDialogTrigger.cs` |
| **Class** | `InnerDialogTrigger : BaseTrigger` |
| **Purpose** | Shows inner monologue text when player enters trigger zone. |
| **Fields** | `_text : string`, `_duration : float` |
| **Methods** | `OnTriggerActivated()` — fire `GameEvent.InnerDialogue` with text |

#### `DamageTrigger.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/World/Triggers/DamageTrigger.cs` |
| **Class** | `DamageTrigger : BaseTrigger` |
| **Purpose** | Deals damage to player on contact (e.g., toxic gas, cold spots). |
| **Fields** | `_damagePerSecond : float`, `_damageType : DamageType` |
| **Methods** | `OnTriggerActivated()` — start damage coroutine; `OnTriggerDeactivated()` — stop damage coroutine |

#### `TeleportTrigger.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/World/Triggers/TeleportTrigger.cs` |
| **Class** | `TeleportTrigger : BaseTrigger` |
| **Purpose** | Teleports player to a target location. |
| **Fields** | `_targetTransform : Transform` |
| **Methods** | `OnTriggerActivated()` — move player to target position |

#### `DisableObjectsTrigger.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/World/Triggers/DisableObjectsTrigger.cs` |
| **Class** | `DisableObjectsTrigger : BaseTrigger` |
| **Purpose** | Disables specific GameObjects when triggered. Used for optimization (distance culling) or scripted sequences. |
| **Fields** | `_objectsToDisable : List<GameObject>` |
| **Methods** | `OnTriggerActivated()` — set all objects inactive |

#### `EnableObjectsTrigger.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/World/Triggers/EnableObjectsTrigger.cs` |
| **Class** | `EnableObjectsTrigger : BaseTrigger` |
| **Purpose** | Enables GameObjects when triggered. Used for spawning enemies or revealing paths. |
| **Fields** | `_objectsToEnable : List<GameObject>` |
| **Methods** | `OnTriggerActivated()` — set all objects active |

#### `SanityTrigger.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/World/Triggers/SanityTrigger.cs` |
| **Class** | `SanityTrigger : BaseTrigger` |
| **Purpose** | Area that affects player sanity (drains or restores). |
| **Fields** | `_sanityEffect : float` (positive = restore, negative = drain), `_effectPerSecond : bool` |
| **Methods** | `OnTriggerActivated()` — apply sanity effect |

#### `SoundTrigger.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/World/Triggers/SoundTrigger.cs` |
| **Class** | `SoundTrigger : BaseTrigger` |
| **Purpose** | Plays a sound when player enters trigger zone. |
| **Fields** | `_sound : AudioClip`, `_spatial : bool` |
| **Methods** | `OnTriggerActivated()` — play via AudioManager |

#### `CutsceneTrigger.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/World/Triggers/CutsceneTrigger.cs` |
| **Class** | `CutsceneTrigger : BaseTrigger` |
| **Purpose** | Triggers a cutscene sequence. |
| **Fields** | `_cutsceneID : string`, `_cutscene : Cutscene` (reference) |
| **Methods** | `OnTriggerActivated()` — start cutscene, fire CutsceneEvent.OnCutsceneStart |

#### `SceneLoadTrigger.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/World/Triggers/SceneLoadTrigger.cs` |
| **Class** | `SceneLoadTrigger : BaseTrigger` |
| **Purpose** | Loads a new scene when player enters trigger (level transition). |
| **Fields** | `_sceneName : string`, `_spawnPointID : string` |
| **Methods** | `OnTriggerActivated()` — fire GameEvent.LoadNextScene with scene name |

#### `ConditionalTrigger.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/World/Triggers/ConditionalTrigger.cs` |
| **Class** | `ConditionalTrigger : BaseTrigger` |
| **Purpose** | Only triggers if a condition is met (item in inventory, story flag set, etc.). |
| **Fields** | `_conditions : List<TriggerCondition>`, `_triggerOnFail : BaseTrigger` (fallback) |
| **Methods** | `OnTriggerActivated()` — evaluate conditions; if all met, proceed; else, fire fallback |

#### `TriggerCondition.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/World/Triggers/TriggerCondition.cs` |
| **Class** | `TriggerCondition` (ScriptableObject or Serializable) |
| **Purpose** | A single condition: type (HasItem, StoryFlag, SanityLevel, etc.) and value. |
| **Fields** | `ConditionType : ConditionType` (enum), `TargetValue : string`, `CheckValue : string` |

### World.Story

#### `StoryManager.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/World/Story/StoryManager.cs` |
| **Class** | `StoryManager : AutoEventBehaviour` |
| **Purpose** | Global story state tracker. Maintains flags for story progression. |
| **Fields** | `_storyFlags : Dictionary<string, bool>`, `_storyBranches : Dictionary<string, int>` |
| **Listeners** | `On(StoryEvent.OnStoryBeat)` — set flag; `On(StoryEvent.OnStoryProgress)` — advance branch |
| **Methods** | `HasFlag(string) : bool`; `SetFlag(string, bool)`; `GetBranchProgress(string) : int`; `Reset()` |

#### `EndingManager.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/World/Story/EndingManager.cs` |
| **Class** | `EndingManager : AutoEventBehaviour` |
| **Purpose** | Determines game ending based on player actions and story flags. |
| **Fields** | `_endings : List<EndingCondition>` (conditions per ending type) |
| **Listeners** | `On(StoryEvent.OnEnding)` — evaluate and trigger ending |
| **Methods** | `EvaluateEnding() : int` — check all conditions, return ending index; `TriggerEnding(int)` |

---

## Scripts.Utilities

#### `FPSDisplay.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/Utilities/FPSDisplay.cs` |
| **Class** | `FPSDisplay : MonoBehaviour` |
| **Purpose** | Debug FPS counter overlay. Shows FPS, frame time, memory usage. |
| **Fields** | `_fpsText : TextMeshProUGUI`, `_updateInterval : float` |
| **Methods** | `Update()` — calculate FPS, update display |

#### `FPSCounter.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/Utilities/FPSCounter.cs` |
| **Class** | `FPSCounter : MonoBehaviour` |
| **Purpose** | Performance counter. Logs FPS to console. |
| **Methods** | `Update()` — sample FPS, log every second |

#### `Screenshot.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/Utilities/Screenshot.cs` |
| **Class** | `Screenshot : MonoBehaviour` |
| **Purpose** | Takes screenshots. Press F12 to capture. |
| **Methods** | `Update()` — detect key press; `Capture()` — ScreenCapture.CaptureScreenshot |

#### `MaterialController.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/Utilities/MaterialController.cs` |
| **Class** | `MaterialController : MonoBehaviour` |
| **Purpose** | Controls material properties at runtime (color, emission, tiling). |
| **Fields** | `_renderer : Renderer`, `_materialIndex : int`, `_propertyName : string` |
| **Methods** | `SetColor(Color)`, `SetFloat(float)`, `SetTexture(Texture)`, `SetEmission(Color)` |

#### `MeshSaver.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/Utilities/MeshSaver.cs` |
| **Class** | `MeshSaver : MonoBehaviour` |
| **Purpose** | Editor utility. Saves mesh to .asset file. |
| **Methods** | `SaveMesh()` — creates mesh asset from MeshFilter |

#### `FOVController.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/Utilities/FOVController.cs` |
| **Class** | `FOVController : MonoBehaviour` |
| **Purpose** | Changes camera FOV dynamically (sprinting, effects). |
| **Fields** | `_camera : Camera`, `_defaultFOV : float`, `_targetFOV : float`, `_transitionSpeed : float` |
| **Methods** | `SetFOV(float)`, `ResetFOV()`, `Update()` — lerp toward target |

#### `Extensions.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/Utilities/Extensions.cs` |
| **Class** | `Extensions` (static) |
| **Purpose** | Extension methods used across the project. |
| **Methods** | `IsPlayer(GameObject) : bool` — check tag; `ToHex(Color) : string` — color to hex; `GetOrAddComponent<T>(GameObject)`; `Shuffle<T>(List<T>)` — Fisher-Yates shuffle |

#### `Helpers.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/Utilities/Helpers.cs` |
| **Class** | `Helpers` (static) |
| **Purpose** | General helper functions. |
| **Methods** | `ClampAngle(float, float, float)`, `LerpAngle(float, float, float)`, `RandomRange(Vector2) : float`, `IsInLayerMask(GameObject, LayerMask) : bool` |

#### `Interpolations.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/Utilities/Interpolations.cs` |
| **Class** | `Interpolations` (static) |
| **Purpose** | Custom easing functions for animations. |
| **Methods** | `EaseInOutQuad(float)`, `EaseOutCubic(float)`, `EaseInBack(float)`, `EaseOutBounce(float)`, `Lerp(Vector3, Vector3, float, Func<float, float>)` |

#### `ValueDropdown.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/Utilities/ValueDropdown.cs` |
| **Class** | `ValueDropdownAttribute : PropertyAttribute` |
| **Purpose** | Custom Odin-like dropdown attribute for Unity Editor. Shows a list of values. |
| **Methods** | *(property drawer)* |

#### `Singleton.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/Utilities/Singleton.cs` |
| **Class** | `Singleton<T> : MonoBehaviour where T : MonoBehaviour` |
| **Purpose** | Generic MonoBehaviour singleton base class. |
| **Methods** | `Instance : T` (static, lazy-initialized); `Awake()` — set instance, DontDestroyOnLoad |

#### `IDGenerator.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/Utilities/IDGenerator.cs` |
| **Class** | `IDGenerator : MonoBehaviour` |
| **Purpose** | Generates unique IDs for objects. Used by save system to identify entities. |
| **Fields** | `ID : string` (GUID) |
| **Methods** | `Generate()` — create new GUID; `SetID(string)` |

---

## Scripts.Settings

#### `GameSettings.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/Settings/GameSettings.cs` |
| **Class** | `GameSettings : MonoBehaviour` |
| **Purpose** | Runtime game settings. Load/save from PlayerPrefs or JSON file. |
| **Fields** | `MasterVolume : float`, `MusicVolume : float`, `SFXVolume : float`, `QualityLevel : int`, `Resolution : Vector2Int`, `Fullscreen : bool`, `Sensitivity : float`, `InvertY : bool`, `Language : string`, `Subtitles : bool` |
| **Methods** | `Load()`, `Save()`, `Apply()` — push settings to AudioManager, QualitySettings, Screen, InputManager |

#### `SettingsData.cs`
| Aspect | Detail |
|--------|--------|
| **Path** | `Assets/Scripts/Settings/SettingsData.cs` |
| **Class** | `SettingsData` |
| **Purpose** | Serializable settings container. Used by save system to persist settings with game data. |
| **Fields** | `MasterVolume`, `MusicVolume`, `SFXVolume`, `QualityLevel`, `ResolutionWidth`, `ResolutionHeight`, `Fullscreen`, `Sensitivity`, `InvertY`, `Language` |

---

## Data Flow Examples

### Pickup Item Flow
1. Player walks near PickupItem, `PlayerInteraction` raycast detects it
2. `GameEvent.InteractHover` fires → HUD shows interaction prompt
3. Player presses Interact key → `InputManager` fires `InputEvent.OnInteract`
4. `InteractionController` receives event → calls `PlayerInteraction.Interact()`
5. `PickupItem.Interact()` → calls `PlayerInventory.AddItem(item)`
6. `PlayerInventory` fires `GameEvent.AddItem` → AudioManager plays pickup sound
7. PickupItem GameObject is destroyed

### Dialog Flow
1. Player enters `DialogTrigger` → `DialogTrigger.OnTriggerActivated()`
2. Fires `DialogEvent.OpenDialog` with nodeID
3. `DialogPanel` receives event → loads Yarn node, displays text
4. Player clicks choice → `DialogСhoice.OnClick()` → fires `DialogEvent.OnChoice`
5. StoryManager tracks story flags from choice

### Photo Capture Flow
1. Player presses photo button → `PhotoSystem.TakePhoto()`
2. Photo camera renders to RenderTexture → captured as Sprite
3. `PhotoEvents.OnPhotoTaken` fires → PhotoGallery refreshes
4. `SaveSystem` serializes photo data on next save

### Save/Load Flow
1. Player opens PauseMenu → clicks Save
2. `SaveLoadWindow` shows slots → `SaveSystem.GetAllSaves()`
3. Player picks slot → `SaveSystem.Save(slot)`
4. SaveSystem iterates `_registeredComponents`, calls `SaveState()` on each
5. Collects all data into `GameData` → JSON serialize → encrypt → write to file
6. On Load: read → decrypt → deserialize → call `LoadState()` on each component

---
