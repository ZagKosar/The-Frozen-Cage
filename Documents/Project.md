# Project: The Frozen Cage — Code Reference

> Unity 6000.3.7f1 (URP) | C# | 82 files

---

## Table of Contents

1. [Overview](#overview)
2. [Architecture](#architecture)
3. [App Layer](#app-layer)
4. [Events System](#events-system)
5. [Game Layer](#game-layer)
6. [UI / Window Layer](#ui--window-layer)
7. [Other Components](#other-components)

---

## Overview

**The Frozen Cage** is a first-person psychological horror game. The player explores an abandoned frozen research facility, photographs evidence, avoids a patrolling NPC, collects items, and solves environmental puzzles.

### Key Systems

- **Event-driven communication** — singleton `EventManager` with generic `Subscribe<T>`/`Invoke<T>` pattern
- **Service locator** (`DependencyContainer`) — global static access to core services via named properties
- **Window stack** (`WindowSwitcher`) — `WindowPanel`-based UI with navigation stack and priority sorting
- **Component-based save system** — `BaseSaver` abstract class, each `MonoBehaviour` serializes its own `JObject`
- **Trigger composition** — `Trigger` with `ICondition` list (AND logic) + `ITriggerEvent` actions, supports `OrCondition`
- **Dialog system** — `DialogNode` ScriptableObjects with `DialogСhoice` list, navigated by `DialogWindow`

---

## Architecture

### EventManager (`Scripts.Events`)

Non-generic singleton. Stores subscribers in `Dictionary<Type, List<Delegate>>`.

| Method | Description |
|--------|-------------|
| `Subscribe<T>(Action<T>)` | Register callback for event type `T` |
| `Unsubscribe<T>(Action<T>)` | Remove callback |
| `Invoke<T>(T data)` | Invoke all callbacks for type `T` |
| `ClearAll()` | Clear all subscribers |
| `CleanupDestroyedSubscribers()` | Remove subscribers whose `MonoBehaviour` target was destroyed (called on scene unload) |

### DependencyContainer (`Scripts.App`)

Singleton service locator. All dependencies are wired in the Inspector and exposed as static read-only properties.

```csharp
DependencyContainer.ClientSettings
DependencyContainer.DialogSystem
DependencyContainer.GraphicsMaster
DependencyContainer.AudioMaster
DependencyContainer.GameTime
DependencyContainer.InputHandler
DependencyContainer.ItemsLibrary
DependencyContainer.PhotoGallery
DependencyContainer.Player
DependencyContainer.Inventory
```

### WindowSwitcher (`Scripts.WindowSwitcher`)

Stack-based window navigation. Maintains prefab dictionary, runtime instance dictionary, and navigation stack. All windows inherit from `WindowPanel`.

| Method | Description |
|--------|-------------|
| `ShowWindow(name, closePrevious, context)` | Show window by name, optionally close previous |
| `CloseWindow(name)` | Close window by name |
| `CloseLast()` | Pop top window from stack |

### SaveSystem (`Scripts.Game.Save`)

Discovers all `BaseSaver` components in the scene via `FindObjectsByType`, serializes each to a `JObject`, and writes to `Save_{slot}.json`. Uses Newtonsoft.Json.

| Method | Description |
|--------|-------------|
| `Save(slot)` | Collect all `BaseSaver` data, serialize to JSON file |
| `Load(slot)` | Read JSON file, deserialize, restore each `BaseSaver` |
| `GetSaveSceneIndex(slot)` | Read scene index from save file |

### Trigger System (`Scripts.Game.Triggers`)

Composable condition-action system. A `Trigger` monitors `List<ICondition>` — when ALL conditions are satisfied, it executes `List<ITriggerEvent>`.

| Component | Role |
|-----------|------|
| `Trigger` | Serializable condition/event container with GUID, play-once, enable-on-start |
| `SceneTriggers` | `MonoBehaviour` that manages all triggers in a scene |
| `ICondition` | Interface: `Complete` event + `Initialize()` |
| `ITriggerEvent` | Interface: `Run()` |
| `OrCondition` | Composite — completes when ANY child condition completes |

---

## App Layer

### AppManager (`App/AppManager.cs`)

| Aspect | Detail |
|--------|--------|
| **Class** | `AppManager : MonoBehaviour` |
| **Purpose** | Top-level application controller. Entry point. Initializes all systems on scene 1. |
| **Fields** | `_windowSwitcher`, `_saveSystem` |
| **Key Methods** | `Start()` — initialize DependencyContainer, load settings, open MainMenu, subscribe to all UI/App/Game events; `OpenWindow/CloseWindow/CloseLastWindow/QuitGame/StartNewGame/ExitToMainMenu/SaveGame/LoadGame/LoadNextGameScene` — event handlers; `LoadScene(int)` — async scene loading with `UniTask` |

### DependencyContainer (`App/DependencyContainer.cs`)

| Aspect | Detail |
|--------|--------|
| **Class** | `DependencyContainer : MonoBehaviour` |
| **Purpose** | Singleton service locator. All dependencies via Inspector. |
| **Pattern** | Singleton via `Instance` property (lazy `FindFirstObjectByType`). Static properties proxy to serialized fields. |
| **Properties** | `ClientSettings`, `DialogSystem`, `GraphicsMaster`, `AudioMaster`, `GameTime`, `InputHandler`, `ItemsLibrary`, `PhotoGallery`, `Player`, `Inventory` |

### AudioManager (`App/AudioManager.cs`)

| Aspect | Detail |
|--------|--------|
| **Class** | `AudioManager : MonoBehaviour` |
| **Purpose** | Music playback via named `SoundPack` dictionary. |
| **Fields** | `_musicSource`, `_audioSettings`, `_packs` (List<SoundPack>) |
| **Methods** | `Initialize()`, `PlaySound(name)`, `StopSound()`, `PauseSound()`, `ResumeSound()`, `SetMasterVolume(float)`, `SetMusicVolume(float)`, `SetSFXVolume(float)` |
| **Helper** | `SoundPack` — `Name`, `AudioClip`, `Loop` |

### ClientSettings (`App/ClientSettings.cs`)

| Aspect | Detail |
|--------|--------|
| **Class** | `ClientSettings [Serializable]` |
| **Purpose** | Persistent settings container. JSON save/load via Newtonsoft. |
| **Sub-classes** | `GameSettings` (MouseSensitivity, TextSpeed, Subtitles), `GraphicsSettings` (ScreenMode, ResolutionWidth/Height, Brightness, VSync), `AudioSettings` (MasterVolume, MusicVolume, SFXVolume) |
| **Methods** | `Save()`, `Load()`, `Clone()`, `CopyFrom()`, `EqualsTo()` |

### GraphicsManager (`App/GraphicsManager.cs`)

| Aspect | Detail |
|--------|--------|
| **Class** | `GraphicsManager : MonoBehaviour` |
| **Purpose** | Display settings control: screen mode, resolution, VSync, brightness via URP ColorAdjustments. |
| **Methods** | `Initialize()`, `ApplyAll()`, `ApplyScreenMode(FullScreenMode)`, `ApplyResolution(int, int)`, `ApplyVSync(bool)`, `ApplyBrightness(float)` |

### GameTime (`App/GameTime.cs`)

| Aspect | Detail |
|--------|--------|
| **Class** | `GameTime [Serializable]` |
| **Namespace** | `Scripts.App` |
| **Purpose** | Decoupled delta-time tracker. Updated each frame by external system. |
| **Properties** | `Time` (total elapsed), `DeltaTime` (last frame) |
| **Methods** | `Update(float deltaTime)`, `Reset()` |

### InputHandler (`App/InputHandler.cs`)

| Aspect | Detail |
|--------|--------|
| **Class** | `InputHandler : MonoBehaviour` |
| **Namespace** | `Scripts.App` |
| **Purpose** | Central input hub. Wraps auto-generated `InputSystemActions` and exposes C# events. |
| **Properties** | `EnablePlayer`, `EnableUI`, `EnableGame` — toggle action maps at runtime |
| **Events** | `OnMove(Vector2)`, `OnLook(Vector2)`, `OnInteract`, `OnCrouchStart/Stop`, `OnSprintStart/Stop`, `OnFlashlight`, `OnAction`, `OnExtraAction`, `OnSubmit`, `OnCancel`, `OnNext`, `OnPrevious`, `OnPause`, `OnInventory` |

### InputSystemActions (`App/InputSystemActions.cs`)

| Aspect | Detail |
|--------|--------|
| **Class** | `InputSystemActions : IInputActionCollection2, IDisposable` (auto-generated) |
| **Purpose** | Auto-generated wrapper for `.inputactions` asset. |
| **Action Maps** | **Player**: Move, Look, Interact, Crouch, Sprint, Flashlight, Action, ExtraAction, Inventory. **UI**: Submit, Cancel, Next, Previous. **Game**: Pause |
| **Interfaces** | `IPlayerActions`, `IUIActions`, `IGameActions` |

### Constants (`App/Constants/WindowConstants.cs`)

| Aspect | Detail |
|--------|--------|
| **Class** | `Constants` (static partial) |
| **Namespace** | `Scripts.App.Constants` |
| **Constants** | `MainMenuWindow`, `SettingsPopUp`, `InventoryWindow`, `PauseWindow`, `PlayerGUI`, `SaveWindow`, `LoadingWindow`, `DialogWindow`, `GalleryWindow` |
| **Property** | `AllWindows` — `IReadOnlyList<string>` of all nine window name constants |

### DialogIDProvider (`App/ValueProvider/DialogIDProvider.cs`)

| Aspect | Detail |
|--------|--------|
| **Class** | `DialogIDProvider` (static) |
| **Namespace** | `Scripts.App.ValueProvider` |
| **Purpose** | Editor-only. Provides `[ValueDropdown]` items for `DialogNode` IDs and choice IDs via `AssetDatabase.FindAssets`. |

---

## Events System

All event types in `Scripts.Events`. Each is a non-static class containing struct message types for `EventManager<T>`.

### GameEvent (`Events/Game/GameEvent.cs`)

| Struct | Fields | Purpose |
|--------|--------|---------|
| `Pause` | — | Toggle game pause |
| `InteractHover` | `Interactable Interact` | Player looking at an interactable |
| `InteractHoverEnd` | `Interactable Interact` | Player stopped looking at interactable |
| `AddItem` | `int Id`, `int Amount` | Item added to inventory |
| `InnerDialogue` | `string Text` | Show inner monologue |
| `LoadNextScene` | — | Transition to next scene |
| `OnPlayerItemEquip` | `UsableItem UsableItem` | Player equipped an item |
| `OnPlayerItemUnEquip` | — | Player unequipped |
| `OnGallery` | — | Open photo gallery |

### DialogEvent (`Events/Game/DialogEvent.cs`)

| Struct | Fields | Purpose |
|--------|--------|---------|
| `OpenDialog` | `string NodeID` | Open dialog at node |
| `OnChoice` | `string ChoiceID` | Player selected a choice |
| `CloseDialog` | — | Close dialog |

### UIEvents (`Events/UI/UIEvents.cs`)

| Struct | Fields | Purpose |
|--------|--------|---------|
| `OpenWindow` | `string Name` | Open window by name |
| `OpenWindowWithContext` | `string Name`, `object Context` | Open window with data |
| `CloseWindow` | `string Name` | Close specific window |
| `CloseLastWindow` | — | Close top window |
| `QuitGame` | — | Exit application |
| `StartNewGame` | — | Start new game |
| `ExitToMainMenu` | — | Return to main menu |

### PreviewEvent (`Events/Preview/PreviewEvent.cs`)

| Struct | Fields | Purpose |
|--------|--------|---------|
| `Drag` | `Vector2 Delta` | Drag model in preview |
| `ShowNext` | `Transform NextModel`, `Vector3 Scale` | Show next model variant |
| `ShowPrevious` | `Transform PreviousModel`, `Vector3 Scale` | Show previous variant |
| `Show` | `Transform Model`, `Vector3 Scale` | Show specific model |

### AppEvents (`Events/App/AppEvents.cs`)

| Struct | Fields | Purpose |
|--------|--------|---------|
| `Save` | `int Slot` | Save game to slot |
| `Load` | `int Slot` | Load game from slot |
| `StartSceneSwitching` | — | Scene transition started |

---

## Game Layer

### GameManager (`Game/GameManager.cs`)

| Aspect | Detail |
|--------|--------|
| **Class** | `GameManager : MonoBehaviour` |
| **Namespace** | `Scripts.Game` |
| **Purpose** | Central game loop. Subscribes to input/game events, manages pause, opens/closes UI windows. |
| **Fields** | `_cameraController`, `_player`, `_isPaused`, `_gameTime`, `_currentInteractable` |
| **Methods** | `Start()` — subscribe to events (OnPause, OnInventory, OnInteract, Pause, InteractHover, AddItem, OnGallery, OpenDialog, CloseDialog), open PlayerGUI. `OnPause()` — toggle pause. `OnInventory()` — show inventory. `OnInteract()` — interact with current. `OnOpenDialog/OnCloseDialog()` — dialog lifecycle. |

### CameraController (`Game/CameraController.cs`)

| Aspect | Detail |
|--------|--------|
| **Class** | `CameraController : MonoBehaviour` |
| **Purpose** | First-person camera: mouse look, head bob, interaction raycast, cursor management. |
| **Fields** | `_camera`, `_body`, `_playerCollider`, `_bodyMaxHeight`, `_cameraHeightOffset`, `_interactionDistance`, `_interactionsLayers`, `_amplitude`, `_frequency`, `_bobTimer` |
| **Methods** | `OnLook(Vector2)` — apply rotation with clamp (-80 to 80), `CheckInteractableHover()` — raycast center screen for `Interactable`, `UpdateCameraHeight()` — head bob + collider height following, `SetMouseLock(bool)` |

### Interactable (`Game/Interactable.cs`)

| Aspect | Detail |
|--------|--------|
| **Class** | `Interactable : MonoBehaviour` (abstract) |
| **Namespace** | `Scripts.Game` |
| **Abstract** | `string InteractDescription { get; }`, `void Interact()` |

### PlayerMoveSystem (`Game/PlayerMoveSystem.cs`)

| Aspect | Detail |
|--------|--------|
| **Class** | `PlayerMoveSystem : MonoBehaviour` |
| **Namespace** | `Scripts.Game` |
| **Purpose** | Walk, sprint, crouch with DOTween height transitions, step sounds, ceiling detection. |
| **Fields** | `_rigidbody`, `_collider`, `_model`, `_walkSpeed`, `_sprintSpeed`, `_crouchSpeed`, `_standHeight`, `_crouchHeight`, `_crouchDuration`, `_ceilingMask` |
| **Methods** | `ApplyMovement()`, `ApplyCrouch()`, `CeilingAbove()`, `GetCurrentSpeed()`, `HandleSteps()` |
| **Properties** | `IsSprinting`, `IsCrouching`, `IsMoving` |

### ColliderDetector (`Game/ColliderDetector.cs`)

| Aspect | Detail |
|--------|--------|
| **Class** | `ColliderDetector : MonoBehaviour` |
| **Namespace** | `Scripts.Game` |
| **Purpose** | Forwards Unity collision/trigger callbacks as C# events. |
| **Events** | `CollisionEnter`, `CollisionExit`, `TriggerEnter`, `TriggerExit` |

### Player (`Game/Player/Player.cs`)

| Aspect | Detail |
|--------|--------|
| **Class** | `Player : MonoBehaviour` |
| **Namespace** | `Scripts.Game` |
| **Purpose** | Player entity. Holds inventory, manages equipped item, forwards use/alt-use input. |
| **Fields** | `_inventory`, `_currentItem (UsableItem)` |
| **Methods** | `OnAction()` — call `_currentItem.Use()`. `OnExtraAction()` — call `_currentItem.AltUse()`. `SetInventory(Inventory)`. |
| **Properties** | `Inventory` |

### Inventory (`Game/Player/Inventory.cs`)

| Aspect | Detail |
|--------|--------|
| **Class** | `Inventory` (plain class) |
| **Purpose** | Serializable item list. Items stored as `InventoryItem` (id + amount). |
| **Methods** | `AddItem(int id, int amount)`, `RemoveItem(int id, int amount)` |
| **Properties** | `Items` (IReadOnlyList) |

### Flashlight (`Game/Player/Flashlight.cs`)

| Aspect | Detail |
|--------|--------|
| **Class** | `Flashlight : MonoBehaviour` |
| **Namespace** | `Scripts.Game` |
| **Purpose** | Toggleable flashlight parented to camera. |
| **Fields** | `_flashlight (Light)`, `_isFlashlightOn` |
| **Methods** | `OnFlashlight()` — toggle on/off |

### PhotoGallery (`Game/Player/PhotoGallery.cs`)

| Aspect | Detail |
|--------|--------|
| **Class** | `PhotoGallery` (plain class) |
| **Namespace** | `Scripts.Game` |
| **Purpose** | Stores photos as base64 strings (serialization) and Sprites (UI). |
| **Methods** | `Add(Texture2D)`, `SetPhotos(List<string>)` |
| **Properties** | `Photos`, `PhotosBase64` |

### Items / Item (`Game/Items/Item.cs`)

| Aspect | Detail |
|--------|--------|
| **Class** | `Item` (plain class) |
| **Namespace** | `Scripts.Game.Items` |
| **Fields** | `Id`, `Name`, `Description`, `Model (Transform)`, `PreviewScale (Vector3)` |

### ItemsLibrary (`Game/Items/ItemsLibrary.cs`)

| Aspect | Detail |
|--------|--------|
| **Class** | `ItemsLibrary : ScriptableObject` |
| **Namespace** | `Scripts.Game.Items` |
| **Purpose** | Central item registry. Maps ID to Item. |
| **Methods** | `Initialize()`, `TryGetItem(int, out Item)` |

### UsableItem (`Game/Items/UsableItem.cs`)

| Aspect | Detail |
|--------|--------|
| **Class** | `UsableItem : Item` (abstract) |
| **Namespace** | `Scripts.Game.Items` |
| **Abstract Methods** | `Initialize()`, `Pickup()`, `Unequipe()`, `Use()`, `AltUse()` |
| **Abstract Properties** | `IsEquiped (bool)` |

### PhotoCamera (`Game/Items/PhotoCamera.cs`)

| Aspect | Detail |
|--------|--------|
| **Class** | `PhotoCamera : UsableItem` |
| **Namespace** | `Scripts.Game.Items` |
| **Purpose** | Camera item. Captures 512x512 photos from a child camera render texture. |
| **Methods** | `Use()` — capture photo, add to `PhotoGallery`. `AltUse()` — fire `GameEvent.OnGallery` |

### PickableItem (`Game/Items/PickableItem.cs`)

| Aspect | Detail |
|--------|--------|
| **Class** | `PickableItem : Interactable` |
| **Purpose** | World pickup object. Adds item to inventory, destroys self. |
| **Fields** | `_id`, `_amount`, `_interactDescription` |
| **Events** | `PickedUp` (used by `ItemPickedUp` trigger condition) |

### NPC / PatrolNPC (`Game/NPC/PatrolNPC.cs` + partials)

| Aspect | Detail |
|--------|--------|
| **Class** | `PatrolNPC : MonoBehaviour` (partial, 4 files) |
| **Namespace** | `Scripts.Game.NPC` |
| **Purpose** | Patrolling AI with 3 states: Patrol, Chase, Search. Uses NavMeshAgent + SplineContainer. |
| **States** | **Patrol**: moves along spline path with configurable wait points. **Chase**: (stub). **Search**: (stub). |
| **Methods** | `CanSeePlayer()` — distance + angle + line-of-sight check |

### Dialog / NPCDialog (`Game/Dialog/NPCDialog.cs`)

| Aspect | Detail |
|--------|--------|
| **Class** | `NPCDialog : Interactable` |
| **Namespace** | `Scripts.Game.Dialog` |
| **Purpose** | NPC dialog trigger. Opens dialog tree at configurable start node. Subscribes to dialog events to track state. |
| **Methods** | `Interact()` — fire `OpenDialog`. `SetStartNode(string)` — change dialog start node |

### DialogSystem (`Game/Dialog/DialogSystem.cs`)

| Aspect | Detail |
|--------|--------|
| **Class** | `DialogSystem` (plain class) |
| **Namespace** | `Scripts.Game.Dialog` |
| **Purpose** | Dialog data container. Builds dictionaries from node/choice lists. |
| **Methods** | `Initialize()`, `TryGetNode(string, out DialogNode)`, `TryGetChoice(string, out DialogСhoice)` |

### DialogNode (`Game/Dialog/DialogNode.cs`)

| Aspect | Detail |
|--------|--------|
| **Class** | `DialogNode : ScriptableObject` |
| **Namespace** | `Scripts.Game.Dialog` |
| **Fields** | `ID`, `Text`, `DialogСhoice` (List) |

### DialogСhoice (`Game/Dialog/DialogСhoice.cs`)

| Aspect | Detail |
|--------|--------|
| **Class** | `DialogСhoice` (Cyrillic 'С') |
| **Namespace** | `Scripts.Game.Dialog` |
| **Fields** | `ID`, `Text`, `NextNodeID`, `NextIsStart`, `Actions` (List<IDialogAction>) |

### IDialogAction (`Game/Dialog/DialogActions/IDialogAction.cs`)

| Aspect | Detail |
|--------|--------|
| **Interface** | `IDialogAction` |
| **Namespace** | `Scripts.Game.Dialog.DialogActions` |
| **Methods** | `void Execute()` |

### Triggers / Trigger (`Game/Triggers/Trigger.cs`)

| Aspect | Detail |
|--------|--------|
| **Class** | `Trigger` (plain class) |
| **Namespace** | `Scripts.Game.Triggers` |
| **Purpose** | Condition-based trigger. Activates when all conditions satisfied. |
| **Fields** | `_guid`, `_conditions` (List<ICondition>), `_triggerEvents` (List<ITriggerEvent>), `_enableOnStart`, `_playOnce` |
| **Methods** | `Enable()`, `Disable()`, `Run()`, `UpdateData(Trigger)` |

### SceneTriggers (`Game/Triggers/SceneTriggers.cs`)

| Aspect | Detail |
|--------|--------|
| **Class** | `SceneTriggers : MonoBehaviour` |
| **Namespace** | `Scripts.Game.Triggers` |
| **Purpose** | Manages all triggers in a scene. Restores state from save data. |

### Conditions

| Class | Namespace | Purpose |
|-------|-----------|---------|
| `ICondition` | `Scripts.Game.Triggers` | Interface: `event Action Complete`, `void Initialize()` |
| `OnTriggerEnterCondition` | `Scripts.Game.Triggers.Conditions` | Completes when collider with tag enters trigger |
| `ItemPickedUp` | `Scripts.Game.Triggers.Conditions` | Completes when specific `PickableItem` picked up |
| `DialogChoiceCondition` | `Scripts.Game.Triggers.Conditions.Dialog` | Completes when specific dialog choice selected |
| `OrCondition` | `Scripts.Game.Triggers.Conditions` | Completes when ANY child condition completes |

### Trigger Events

| Class | Namespace | Purpose |
|-------|-----------|---------|
| `ITriggerEvent` | `Scripts.Game.Triggers` | Interface: `void Run()` |
| `SetActiveObjectTriggerEvent` | `Scripts.Game.Triggers.Events` | Enable/disable a GameObject |
| `LoadNextSceneTriggerEvent` | `Scripts.Game.Triggers.Events` | Load next scene |
| `EnableTriggerEvent` | `Scripts.Game.Triggers.Events` | Enable another trigger by GUID (chaining) |
| `InnerDialogueTriggerEvent` | `Scripts.Game.Triggers.Events` | Show inner monologue text |
| `SetStartNode` | `Scripts.Game.Triggers.Events.Dialog` | Change NPC's dialog start node |

### Save System

#### BaseSaver (`Game/Save/BaseSaver.cs`)

| Aspect | Detail |
|--------|--------|
| **Class** | `BaseSaver : MonoBehaviour` (abstract) |
| **Namespace** | `Scripts.Game.Save` |
| **Abstract** | `string Key { get; }`, `JObject Save()`, `bool Load(JObject)` |

#### SaveSystem (`Game/Save/SaveSystem.cs`)

| Aspect | Detail |
|--------|--------|
| **Class** | `SaveSystem` (plain class) |
| **Namespace** | `Scripts.Game.Save` |
| **Methods** | `Save(int slot)`, `Load(int slot)`, `GetSaveSceneIndex(int slot)` |
| **Storage** | `Application.persistentDataPath/Save/Save_{slot}.json` |

#### Savers

| Saver | Namespace | Saves |
|-------|-----------|-------|
| `PlayerSaver` | `Scripts.Game.Save.Player` | Inventory, position, rotation, camera rotation |
| `DisappearObjectSaver` | `Scripts.Game.Save.Items` | Whether a pickup object was taken |
| `NPCDialogSaver` | `Game.Save.Dialog` | NPC's dialog start node |
| `SceneTriggersSaver` | `Scripts.Game.Save.Triggers` | All trigger states (enabled, condition progress) |
| `PhotoGallerySaver` | `Game.Save.PhotoGallery` | Photo gallery base64 images |

#### Utils (`Game/Save/Utils.cs`)

| Aspect | Detail |
|--------|--------|
| **Namespace** | `Scripts.Game.Save.Utils` |
| **Classes** | `SerializableVector3` (x, y, z), `SerializableQuaternion` (x, y, z, w) — JSON-friendly wrappers |

### PreviewShower (`Game/Preview/PreviewShower.cs`)

| Aspect | Detail |
|--------|--------|
| **Class** | `PreviewShower : MonoBehaviour` |
| **Namespace** | `Scripts.Game.Preview` |
| **Purpose** | 3D item preview viewer. Supports drag-rotate and swipe-to-next/previous with DOTween animations. |
| **Subscribes** | `PreviewEvent.Drag`, `PreviewEvent.Show`, `PreviewEvent.ShowNext`, `PreviewEvent.ShowPrevious` |

---

## UI / Window Layer

### WindowPanel (`WindowSwitcher/WindowPanel.cs`)

| Aspect | Detail |
|--------|--------|
| **Class** | `WindowPanel : MonoBehaviour` (abstract) |
| **Namespace** | `Scripts.WindowSwitcher` |
| **Lifecycle** | `abstract void Load()`, `abstract void Destroy()`, `abstract void Open(object context)`, `abstract void Close()` |
| **Property** | `abstract int Priority { get; }` — sorting order in container |

### WindowSwitcher (`WindowSwitcher/WindowSwitcher.cs`)

| Aspect | Detail |
|--------|--------|
| **Class** | `WindowSwitcher : MonoBehaviour` |
| **Namespace** | `Scripts.WindowSwitcher` |
| **Purpose** | Central window manager. Prefab dictionary, instance cache, navigation stack. |
| **Methods** | `ShowWindow(name, closePrevious, context)`, `CloseWindow(name)`, `CloseLast()` |
| **Helper** | Nested `Window` class with `[ValueDropdown]` on `Name` from `Constants.AllWindows` |

### Windows

| Window | Namespace | Priority | Purpose |
|--------|-----------|----------|---------|
| `MainMenuWindow` | (global) | 1 | New Game, Continue, Settings, Quit |
| `PlayerGUI` | (global) | 1 | HUD: crosshair, interaction prompt, subtitles |
| `DialogWindow` | `Scripts.Windows.Dialog` | 2 | Dialog tree with choice buttons (object-pooled) |
| `InventoryWindow` | `Scripts.Windows.Inventory` | 2 | Item list with 3D preview, equip/unequip |
| `PauseWindow` | (global) | 2 | Continue, Save, Load, Settings, Exit (with confirmation popup) |
| `PhotoGalleryWindow` | `Windows.Game.PhotoGallery` | 2 | Photo grid display |
| `SaveWindow` | `Scripts.Windows.Save` | 3 | 5 save slots, save/load mode |
| `SettingsPopUp` | (global) | 3 | Game/Graphics/Audio tabs, apply/revert/reset |
| `LoadingWindow` | `Scripts.Windows.App.Loading` | 99 | Spinning loading animation |

### UI Components

| Component | Namespace | Purpose |
|-----------|-----------|---------|
| `DragbleUIElement` | `Scripts.UI` | `IDragHandler`, fires `Drag(Vector2)` event |
| `Switcher` | (global) | On/off toggle switch with DOTween animation |
| `YesNoPopup` | `Scripts.UI` | Yes/No confirmation popup with `Result(bool)` event |
| `DialogWindowChoice` | `Scripts.Windows.Dialog` | Single choice button (pooled prefab) |
| `DialogWindowContext` | `Scripts.Windows.Dialog` | Data: `NodeID` |
| `SaveWindowContext` | `Scripts.Windows.Save` | Data: `IsSaving` |
| `SaveSlot` | `Scripts.Windows.Save` | Single save slot button |
| `Photo` | `Windows.Game.PhotoGallery` | Single photo thumbnail |

---

## Other Components

### ScrollHealth (`HealsBar/ScrollHealth.cs`)

| Aspect | Detail |
|--------|--------|
| **Class** | `ScrollHealth : MonoBehaviour` |
| **Purpose** | Animated health bar with scrolling texture and status icons. Currently WIP — state change logic is disabled. |

### Presenter (`SlidePresenter/Presenter.cs`)

| Aspect | Detail |
|--------|--------|
| **Class** | `Presenter : MonoBehaviour` (requires `Image`) |
| **Purpose** | Sprite-based slideshow/animator. Cycles through `List<Sprite>` at configurable FPS. |
| **Methods** | `Play()`, `Stop()` |

### BlinkingAnimation (`Animations/BlinkingAnimation.cs`)

| Aspect | Detail |
|--------|--------|
| **Class** | `BlinkingAnimation : MonoBehaviour` |
| **Namespace** | `Scripts.Animations` |
| **Purpose** | Toggles a GameObject on/off at configurable intervals. |
| **Fields** | `_object`, `_onDelay`, `_offDelay`, `_playOnStart` |

### Anchor (`Editor/Anchor.cs`)

| Aspect | Detail |
|--------|--------|
| **Class** | `Anchor` (static) |
| **Purpose** | Editor utility (Ctrl+L). Bakes RectTransform offsets into anchor values for resolution-independent UI. |
