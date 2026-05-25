# The Frozen Cage — Technical Overview

## Project Info

- **Тема:** Разработка 3D компьютерной игры в жанре детективный хоррор с применением методов визуальной стилизации под ретро-графику
- **Движок:** Unity 6000.3.7f1
- **Язык:** C#

---

## 1. Архитектура приложения

### 1.1 Service Locator (DependencyContainer)

`DependencyContainer` — центральный реестр сервисов. Все ключевые системы доступны статически через него:

```
DependencyContainer.Instance
├── ClientSettings    — настройки (графика, звук, управление)
├── DialogSystem      — база диалоговых узлов
├── GraphicsMaster    — управление графикой (разрешение, VSync, яркость)
├── AudioMaster       — управление звуком (музыка, SFX)
├── GameTime          — игровое время (паузится при открытии меню)
├── InputHandler      — обёртка над InputSystem
├── ItemsLibrary      — база предметов
├── PhotoGallery      — коллекция фотоснимков
├── Player            — компонент игрока (lazy)
└── Inventory         — инвентарь (Id + Amount)
```

### 1.2 Event Bus (EventManager)

Типобезопасная шина событий. События — plain structs, подписчики — `Action<T>`.

#### События приложения (AppEvents)
| Событие | Описание |
|---------|----------|
| `Save { int Slot }` | Сохранить игру |
| `Load { int Slot }` | Загрузить игру |

#### Игровые события (GameEvent)
| Событие | Описание |
|---------|----------|
| `Pause` | Пауза |
| `InteractHover { Interactable }` | Наведение на интерактивный объект |
| `InteractHoverEnd { Interactable }` | Снятие наведения |
| `AddItem { int Id, int Amount }` | Добавление предмета |
| `InnerDialogue { string Text }` | Внутренний монолог (субтитры) |
| `LoadNextScene` | Загрузка следующей сцены |

#### События диалогов (DialogEvent)
| Событие | Описание |
|---------|----------|
| `OpenDialog { string NodeID }` | Открыть диалог |
| `OnChoice { string ChoiceID }` | Выбор реплики |
| `CloseDialog` | Закрыть диалог |

#### События UI (UIEvents)
| Событие | Описание |
|---------|----------|
| `OpenWindow { string Name }` | Открыть окно |
| `OpenWindowWithContext { string Name, object Context }` | Открыть с контекстом |
| `CloseWindow { string Name }` | Закрыть окно |
| `CloseLastWindow` | Закрыть последнее |
| `QuitGame` | Выход |
| `StartNewGame` | Новая игра |
| `ExitToMainMenu` | В главное меню |

### 1.3 Window System (MVC-like)

- `WindowPanel` — абстрактная база всех окон (MainMenu, Pause, Inventory, Dialog, Save, Settings, Gallery, PlayerGUI)
- `WindowSwitcher` — управляет стэком окон, инстанциирует/уничтожает панели, сортирует по priority
- Окна открываются/закрываются через события `UIEvents`

### 1.4 Save System (Strategy)

- `BaseSaver` — абстрактный класс: каждый сохраняемый компонент реализует свой `Key`, `Save()`, `Load()`
- `SaveSystem` — собирает все `BaseSaver` в сцене, сериализует в JSON (Newtonsoft)
- Саверы:
  - `PlayerSaver` — позиция, поворот, инвентарь, поворот камеры
  - `NPCDialogSaver` — прогресс диалогов NPC (текущий start node)
  - `DisappearObjectSaver` — был ли подобран предмет
  - `SceneTriggersSaver` — состояние триггеров сцены

---

## 2. Система скриптов (по папкам)

### 2.1 App — ядро приложения

| Файл | Класс | Назначение |
|------|-------|------------|
| `AppManager.cs` | `AppManager` | Главный контроллер: загрузка сцен, управление окнами, save/load, new game |
| `DependencyContainer.cs` | `DependencyContainer` | Service locator, singleton |
| `InputHandler.cs` | `InputHandler` | Обёртка InputSystem, C#-события на действия игрока |
| `InputSystemActions.cs` | `InputSystemActions` | Авто-генерация из .inputactions (3 маппера: Player, UI, Game) |
| `AudioManager.cs` | `AudioManager` | Воспроизведение музыки/SFX, громкость |
| `GraphicsManager.cs` | `GraphicsManager` | Разрешение, полноэкранный режим, VSync, яркость через URP Volume |
| `ClientSettings.cs` | `ClientSettings` | Сериализуемые настройки (JSON) — вложенные GameSettings, GraphicsSettings, AudioSettings |
| `GameTime.cs` | `GameTime` | Кастомное время (паузится при открытии окон) |
| `Constants/WindowConstants.cs` | — | Константы имён окон |

### 2.2 Events — шина событий

| Файл | Назначение |
|------|------------|
| `EventManager.cs` | Generic pub/sub, Subscribe<T>/Invoke<T> |
| `App/AppEvents.cs` | Save, Load |
| `Game/GameEvent.cs` | Pause, InteractHover, AddItem, InnerDialogue, LoadNextScene |
| `Game/DialogEvent.cs` | OpenDialog, OnChoice, CloseDialog |
| `UI/UIEvents.cs` | OpenWindow, CloseWindow, QuitGame, StartNewGame |
| `Preview/PreviewEvent.cs` | Drag, Show, ShowNext, ShowPrevious |

### 2.3 Game — игровая логика

#### 2.3.1 Основные компоненты

| Файл | Класс | Назначение |
|------|-------|------------|
| `GameManager.cs` | `GameManager` | Контроллер геймплея: пауза, инвентарь, взаимодействие, подписки на события |
| `CameraController.cs` | `CameraController` | Камера от первого лица: mouse look (Y -80/+80), raycast на интерактивные объекты, head bob |
| `PlayerMoveSystem.cs` | `PlayerMoveSystem` | Передвижение: Rigidbody, ходьба/бег/присед (DOTween твининг высоты коллайдера), шаги |
| `Interactable.cs` | `Interactable` | Абстрактный класс: InteractDescription, Interact() |
| `ColliderDetector.cs` | `ColliderDetector` | Прокидывает OnTriggerEnter/Exit в C#-события |
| `Player.cs` | `Player` | Хранит ссылку на Inventory |

#### 2.3.2 Диалоговая система (ScriptableObject-based directed graph)

| Файл | Класс | Назначение |
|------|-------|------------|
| `Dialog/DialogNode.cs` | `DialogNode` (SO) | Узел диалога: ID, текст, список выборов |
| `Dialog/DialogСhoice.cs` | `DialogСhoice` | Вариант ответа: ID, текст, NextNodeID, NextIsStart, Actions |
| `Dialog/DialogSystem.cs` | `DialogSystem` (SO) | База узлов: словарь ID→Node, Initialize(), TryGetNode/Choice |
| `Dialog/NPCDialog.cs` | `NPCDialog` | NPC (Interactable): при взаимодействии открывает диалог, управляет аниматором |
| `Dialog/DialogActions/IDialogAction.cs` | `IDialogAction` | Интерфейс действий при выборе реплики |

#### 2.3.3 Система предметов

| Файл | Класс | Назначение |
|------|-------|------------|
| `Items/Item.cs` | `Item` | Данные предмета: Id, Name, Description, Model |
| `Items/ItemsLibrary.cs` | `ItemsLibrary` (SO) | База предметов: ID→Item |
| `Items/UsableItem.cs` | `UsableItem` | Абстракт: IsEquiped, Pickup, Unequipe, Use, AltUse |
| `Items/PhotoCamera.cs` | `PhotoCamera` | Фотоаппарат: создаёт RenderTexture 512×512, рендерит, сохраняет в PhotoGallery (base64 PNG) |
| `Items/PickableItem.cs` | `PickableItem` | Предмет в мире: Interact → AddItem в инвентарь, уничтожается |

#### 2.3.4 Триггерная система (Composition)

Условия (`ICondition`):

| Файл | Класс | Назначение |
|------|-------|------------|
| `Triggers/Conditions/ICondition.cs` | `ICondition` | Интерфейс: Initialize(), Complete event |
| `Triggers/Conditions/OnTriggerEnterCondition.cs` | `OnTriggerEnterCondition` | Вход в триггер-зону (по тегу) |
| `Triggers/Conditions/ItemPickedUp.cs` | `ItemPickedUp` | Подбор конкретного PickableItem |
| `Triggers/Conditions/OrCondition.cs` | `OrCondition` | OR: любое из дочерних условий |
| `Triggers/Conditions/Dialog/DialogChoiceCondition.cs` | `DialogChoiceCondition` | Выбор конкретной реплики в диалоге |

События триггеров (`ITriggerEvent`):

| Файл | Класс | Назначение |
|------|-------|------------|
| `Triggers/Events/ITriggerEvent.cs` | `ITriggerEvent` | Интерфейс: Run() |
| `Triggers/Events/EnableTriggerEvent.cs` | `EnableTriggerEvent` | Включить другой триггер (цепочки) |
| `Triggers/Events/InnerDialogueTriggerEvent.cs` | `InnerDialogueTriggerEvent` | Показать внутренний монолог |
| `Triggers/Events/LoadNextSceneTriggerEvent.cs` | `LoadNextSceneTriggerEvent` | Загрузить следующую сцену |
| `Triggers/Events/SetActiveObjectTriggerEvent.cs` | `SetActiveObjectTriggerEvent` | Включить/выключить GameObject |
| `Triggers/Events/Dialog/SetStartNode.cs` | `SetStartNode` | Сменить start node NPC (прогресс сюжета) |

Управление:

| Файл | Класс | Назначение |
|------|-------|------------|
| `Triggers/Trigger.cs` | `Trigger` | Композиция условий + событий. GUID для save/load |
| `Triggers/SceneTriggers.cs` | `SceneTriggers` | MonoBehaviour: список триггеров сцены, enable на старте, run/remove |

#### 2.3.5 Компоненты игрока

| Файл | Класс | Назначение |
|------|-------|------------|
| `Player/Flashlight.cs` | `Flashlight` | Фонарик: toggle Light.enabled |
| `Player/Inventory.cs` | `Inventory` | Инвентарь: List<(Id, Amount)>, JSON-сериализуемый |
| `Player/PhotoGallery.cs` | `PhotoGallery` | Фотоальбом: base64 PNG + Sprite |
| `Player/Player.cs` | `Player` | Хранит Inventory |

#### 2.3.6 Save

| Файл | Класс | Key | Назначение |
|------|-------|-----|------------|
| `Save/SaveSystem.cs` | `SaveSystem` | — | Собирает/восстанавливает все BaseSaver, пишет/читает JSON |
| `Save/BaseSaver.cs` | `BaseSaver` | — | Абстракт: Key, Save(), Load() |
| `Save/Utils.cs` | SerializableVector3/Quaternion | — | JSON-сериализация Vector3/Quaternion |
| `Save/Player/PlayerSaver.cs` | `PlayerSaver` | "Player" | Позиция, поворот, инвентарь, камера |
| `Save/Dialog/NPCDialogSaver.cs` | `NPCDialogSaver` | "NPCDialog_{name}" | Текущий start node NPC |
| `Save/Items/DisappearObjectSaver.cs` | `DisappearObjectSaver` | "DisappearObject_{ID}" | Подобран ли предмет |
| `Save/Triggers/SceneTriggersSaver.cs` | `SceneTriggersSaver` | "SceneTriggers_{type}" | Состояние триггеров |

### 2.4 UI — окна и интерфейс

#### 2.4.1 Окна

| Файл | Класс | Priority | Назначение |
|------|-------|----------|------------|
| `Windows/MainMenu/MainMenuWindow.cs` | `MainMenuWindow` | 1 | Главное меню |
| `Windows/Game/Pause/PauseWindow.cs` | `PauseWindow` | 2 | Пауза: продолжить, сохранить, загрузить, настройки, выход |
| `Windows/Game/Inventory/InventoryWindow.cs` | `InventoryWindow` | 2 | Инвентарь: название, описание, 3D-превью (Q/E листание) |
| `Windows/Game/PlayerGUI/PlayerGUI.cs` | `PlayerGUI` | 1 | HUD: прицел, текст взаимодействия, субтитры |
| `Windows/Game/PhotoGallery/PhotoGalleryWindow.cs` | `PhotoGalleryWindow` | 2 | Галерея фото (сетка) |
| `Windows/Dialog/DialogWindow.cs` | `DialogWindow` | 2 | Диалоговое окно: текст NPC + кнопки выбора |
| `Windows/Dialog/DialogWindowChoice.cs` | `DialogWindowChoice` | — | Кнопка выбора реплики |
| `Windows/Save/SaveWindow.cs` | `SaveWindow` | 3 | Окно сохранения/загрузки |
| `Windows/Settings/SettingsPopUp.cs` | `SettingsPopUp` | 3 | Настройки (игра, графика, аудио) |
| `Windows/App/Loading/LoadingWindow.cs` | `LoadingWindow` | 99 | Экран загрузки |

#### 2.4.2 Вспомогательные UI-компоненты

| Файл | Класс | Назначение |
|------|-------|------------|
| `UI/YesNoPopup.cs` | `YesNoPopup` | Popup подтверждения (Да/Нет) |
| `UI/DragbleUIElement.cs` | `DragbleUIElement` | Drag-события для UI |
| `UI/Switcher.cs` | `Switcher` | Анимированный переключатель on/off (DOTween) |

### 2.5 Прочие утилиты

| Файл | Класс | Назначение |
|------|-------|------------|
| `Animations/BlinkingAnimation.cs` | `BlinkingAnimation` | Мигание объекта (toggle вкл/выкл) |
| `SlidePresenter/Presenter.cs` | `Presenter` | Спрайтовая анимация (flipbook, FPS) |
| `HealsBar/ScrollHealth.cs` | `ScrollHealth` | Полоска здоровья (scroll texture) |
| `Editor/Anchor.cs` | `Anchor` | Утилита: подогнать anchors под offsets (Ctrl+L) |
| `Game/Preview/PreviewShower.cs` | `PreviewShower` | 3D-превью предметов: drag-вращение, swipe-переключение (DOTween) |
| `WindowSwitcher/WindowSwitcher.cs` | `WindowSwitcher` | Менеджер окон: стэк, инстанциирование по имени префаба |
| `WindowSwitcher/WindowPanel.cs` | `WindowPanel` | Абстрактная база окна |

---

## 3. Scene Flow

| Build Index | Сцена | Описание |
|-------------|-------|----------|
| 1 | MainMenu | Главное меню |
| 2 | Game (Лес) | Первая локация: место автокатастрофы |
| 3 | Game (Забегаловка) | Вторая локация |
| 4+ | Game (Завод) | Третья локация, финал |

`AppManager.LoadNextGameScene()` инкрементирует build index (SceneManager.LoadScene(current + 1)). Загрузка асинхронная через UniTask.

---

## 4. Зависимости (external packages)

| Пакет | Назначение |
|-------|------------|
| Unity URP | Рендеринг |
| Unity InputSystem | Обработка ввода |
| Unity TextMeshPro | UI-текст |
| Newtonsoft.Json | Сериализация |
| DOTween (Demigiant) | Анимации (crouch, head bob, loading, UI) |
| Sirenix Odin Inspector | Инспектор/Serialize/ValueDropdown |
| Cysharp.Threading.Tasks (UniTask) | Асинхронная загрузка сцен |

---

## 5. Ключевые архитектурные решения

- **Событийно-ориентированная архитектура**: системы общаются через EventManager, а не прямыми ссылками
- **Composition в триггерах**: любое сочетание условий (AND по умолчанию, OR через OrCondition) + любое событие
- **ScriptableObject как база данных**: DialogNode, DialogSystem, ItemsLibrary — SO, редактируются в инспекторе без кодогенерации
- **Component-based Save**: каждый модуль сам отвечает за свою сериализацию через BaseSaver
- **DOTween для кастомных анимаций**: приседание (твининг высоты коллайдера), head bob, инвентарный swipe, loading circle
