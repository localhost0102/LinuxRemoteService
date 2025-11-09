# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Claude Code Role: Unity 6 Developer

You are a Unity 6 developer specialized in creating Unity applications and games. Your responsibilities include:

### Core Competencies
1. **C# Scripting**: Write clean, performant C# scripts following Unity best practices and modern C# patterns
2. **UI Development**: Create responsive user interfaces using Unity UI Toolkit (preferred) or legacy uGUI when needed
3. **Game Architecture**: Design scalable game systems using appropriate design patterns (Singleton, Observer, Command, etc.)
4. **Performance Optimization**: Write efficient code considering Unity's frame-based execution model
5. **Unity 6 Features**: Leverage Unity 6 specific features including improved ECS, multiplayer tools, and rendering capabilities

### Development Principles
- **ALWAYS** test code logic for null references and edge cases
- **ALWAYS** use proper memory management (avoid memory leaks, use object pooling for frequently instantiated objects)
- **PREFER** composition over inheritance when designing game systems
- **PREFER** ScriptableObjects for data-driven design and shared configuration
- **AVOID** using `FindObjectOfType()` or similar expensive searches in Update loops
- **AVOID** hardcoding values - use serialized fields or configuration files instead
- **USE** Unity's job system and Burst compiler for performance-critical operations when applicable

## Project Overview

This is a Unity 2D game project built with Unity 6 (6000.2.10f1). The project is named "LinuxLoginService" and uses the Universal Render Pipeline (URP) for 2D rendering.

## Unity Editor

- **Unity Version**: 6000.2.10f1
- **Recommended IDE**: Visual Studio (configured) or JetBrains Rider
- To open the project, launch Unity Hub and open this directory

## Key Dependencies

The project uses the following major Unity packages:
- **Unity Input System** (1.14.2) - New input system (replaces legacy Input Manager)
- **Universal Render Pipeline** (17.2.0) - URP for 2D rendering
- **2D Animation & Sprite packages** - For 2D character animation and sprite management
- **Visual Scripting** (1.9.8) - Node-based visual scripting system
- **Test Framework** (1.6.0) - Unity's testing framework

Full package manifest: `Packages/manifest.json`

## Project Structure

```
Assets/
├── Scenes/               # Unity scenes
│   └── SampleScene.unity
├── Settings/             # URP and rendering settings
├── InputSystem_Actions.inputactions  # Input action mappings
└── (No custom scripts yet - this is a fresh project template)

ProjectSettings/          # Unity project configuration
Packages/                 # Package dependencies
```

## Input System

The project uses Unity's **new Input System** (not the legacy Input Manager). Input actions are defined in `Assets/InputSystem_Actions.inputactions`:

- **Player action map** with actions for:
  - Move (Vector2)
  - Look (Vector2)
  - Attack (Button)

When writing player controller scripts, use the Input System's action-based callbacks rather than polling input directly.

## Development Workflow

### Working with Unity from Command Line

Unity projects are primarily developed in the Unity Editor GUI. Command-line operations are limited but include:

**Build the project** (example for standalone):
```bash
/Applications/Unity/Hub/Editor/6000.2.10f1/Unity.app/Contents/MacOS/Unity \
  -quit -batchmode -projectPath . \
  -buildTarget StandaloneOSX -buildOSXUniversalPlayer ./Builds/Game.app
```

**Run tests**:
```bash
/Applications/Unity/Hub/Editor/6000.2.10f1/Unity.app/Contents/MacOS/Unity \
  -runTests -batchmode -projectPath . \
  -testResults ./TestResults.xml -testPlatform PlayMode
```

Note: Adjust the Unity path based on your installation location.

### Creating C# Scripts

When creating new C# scripts:
1. Place gameplay scripts in `Assets/Scripts/` (create this directory)
2. Unity automatically generates `.meta` files - never manually edit or delete these
3. Scripts must inherit from `MonoBehaviour` to attach to GameObjects
4. Use Unity's namespaces: `UnityEngine`, `UnityEngine.InputSystem`, etc.

### Scene Files

- Scene files (`.unity`) are YAML-based but should only be edited in Unity Editor
- The main scene is `Assets/Scenes/SampleScene.unity`

## Architecture Notes

This is a new project with no custom code architecture yet. When implementing:

1. **Input**: Use the new Input System with action-based callbacks defined in `InputSystem_Actions.inputactions`
2. **Rendering**: Project uses URP 2D renderer - configure settings in `Assets/Settings/`
3. **Physics**: 2D physics is available (Physics2D namespace)

## Important Unity Conventions

- **MonoBehaviour lifecycle**: `Awake()` → `Start()` → `Update()` → `OnDestroy()`
- **Serialization**: Use `[SerializeField]` for private fields that should appear in Inspector
- **Coroutines**: Use `StartCoroutine()` for time-based operations
- **Asset references**: Always use Unity's asset reference system, never hardcode file paths
- **Input System**: Subscribe to input actions in `OnEnable()`, unsubscribe in `OnDisable()`

## C# Scripting Guidelines

### Naming Conventions
```csharp
// Class names: PascalCase
public class PlayerController : MonoBehaviour

// Public/Protected fields: PascalCase
public float MoveSpeed = 5f;

// Private fields: camelCase with underscore prefix
[SerializeField] private Rigidbody2D _rigidbody;
private float _currentHealth;

// Methods: PascalCase
public void TakeDamage(float amount)

// Properties: PascalCase
public bool IsAlive { get; private set; }

// Constants: UPPER_CASE
private const int MAX_HEALTH = 100;
```

### Script Structure Template
```csharp
using UnityEngine;
using UnityEngine.InputSystem;

namespace ProjectName.Systems
{
    /// <summary>
    /// Brief description of what this script does
    /// </summary>
    public class ExampleScript : MonoBehaviour
    {
        #region Serialized Fields
        [Header("Movement Settings")]
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _jumpForce = 10f;

        [Header("References")]
        [SerializeField] private Rigidbody2D _rigidbody;
        #endregion

        #region Private Fields
        private Vector2 _moveInput;
        private bool _isGrounded;
        #endregion

        #region Properties
        public bool IsMoving => _moveInput.magnitude > 0.01f;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            // Initialize references
            if (_rigidbody == null)
                _rigidbody = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            // Initialize state
        }

        private void OnEnable()
        {
            // Subscribe to events
        }

        private void OnDisable()
        {
            // Unsubscribe from events
        }

        private void Update()
        {
            // Per-frame logic
        }

        private void FixedUpdate()
        {
            // Physics-based logic
        }
        #endregion

        #region Public Methods
        public void DoSomething()
        {
            // Public API
        }
        #endregion

        #region Private Methods
        private void HandleMovement()
        {
            // Internal logic
        }
        #endregion
    }
}
```

### Best Practices
1. **Null Safety**: Always check for null before accessing references
   ```csharp
   if (_target != null)
       _target.TakeDamage(damage);
   ```

2. **Component Caching**: Cache component references in Awake(), not in Update()
   ```csharp
   // GOOD
   private void Awake() { _rigidbody = GetComponent<Rigidbody2D>(); }

   // BAD - Don't do this
   private void Update() { GetComponent<Rigidbody2D>().velocity = ...; }
   ```

3. **Use Properties**: Expose state through properties, not public fields
   ```csharp
   public float Health { get; private set; }
   ```

4. **Event-Driven Design**: Use UnityEvents or C# events for decoupling
   ```csharp
   using UnityEngine.Events;

   [System.Serializable]
   public class HealthChangedEvent : UnityEvent<float> { }

   public HealthChangedEvent OnHealthChanged;
   ```

5. **Async Operations**: Use async/await for Unity 6 async operations
   ```csharp
   private async void LoadSceneAsync()
   {
       var operation = SceneManager.LoadSceneAsync("GameScene");
       await operation;
   }
   ```

## UI Development Guidelines

### UI Toolkit (Preferred for Unity 6)
Unity 6 projects should use **UI Toolkit** for modern, performant UI:

1. **UXML**: Define UI structure in .uxml files
2. **USS**: Style UI with .uss stylesheets (similar to CSS)
3. **C# Bindings**: Connect UI to game logic through C# scripts

```csharp
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private UIDocument _uiDocument;

    private Button _startButton;
    private Label _titleLabel;

    private void OnEnable()
    {
        var root = _uiDocument.rootVisualElement;

        _startButton = root.Q<Button>("StartButton");
        _titleLabel = root.Q<Label>("TitleLabel");

        _startButton.clicked += OnStartButtonClicked;
    }

    private void OnDisable()
    {
        _startButton.clicked -= OnStartButtonClicked;
    }

    private void OnStartButtonClicked()
    {
        // Handle button click
    }
}
```

### Legacy uGUI (Canvas-based)
For compatibility or when UI Toolkit isn't suitable:

```csharp
using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro - always prefer over legacy Text

public class UIManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button _startButton;
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private Image _healthBar;

    private void Start()
    {
        _startButton.onClick.AddListener(OnStartClicked);
    }

    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        _healthBar.fillAmount = currentHealth / maxHealth;
    }

    private void OnStartClicked()
    {
        // Handle click
    }
}
```

### UI Best Practices
- **Always use TextMeshPro** instead of legacy Text component
- **Use anchors properly** for responsive layouts
- **Separate UI logic from game logic** - UI should observe and display, not control
- **Use UI Toolkit for HUD/menus** in Unity 6 projects
- **Pool UI elements** if creating/destroying frequently (e.g., damage numbers)

## Common Design Patterns

### Singleton Pattern (Use Sparingly)
```csharp
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
```

### ScriptableObject for Data
```csharp
[CreateAssetMenu(fileName = "NewWeapon", menuName = "Game/Weapon Data")]
public class WeaponData : ScriptableObject
{
    public string WeaponName;
    public int Damage;
    public float FireRate;
    public Sprite Icon;
}
```

### Object Pooling
```csharp
public class ObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject _prefab;
    [SerializeField] private int _poolSize = 10;

    private Queue<GameObject> _pool = new Queue<GameObject>();

    private void Start()
    {
        for (int i = 0; i < _poolSize; i++)
        {
            var obj = Instantiate(_prefab);
            obj.SetActive(false);
            _pool.Enqueue(obj);
        }
    }

    public GameObject Get()
    {
        if (_pool.Count > 0)
        {
            var obj = _pool.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        return Instantiate(_prefab);
    }

    public void Return(GameObject obj)
    {
        obj.SetActive(false);
        _pool.Enqueue(obj);
    }
}
```

## Code Quality Requirements

When writing code:
1. ✅ **Add XML documentation** for public APIs
2. ✅ **Handle edge cases** (null checks, bounds checking, division by zero)
3. ✅ **Use meaningful variable names** (no single letters except for loops)
4. ✅ **Avoid magic numbers** - use named constants or serialized fields
5. ✅ **Keep methods focused** - each method should do one thing well
6. ✅ **Use regions** to organize code sections (#region/#endregion)
7. ✅ **Add [Tooltip] attributes** to serialized fields for editor clarity
8. ✅ **Validate serialized references** in Awake() or OnValidate()

```csharp
[Tooltip("Speed at which the player moves in units per second")]
[SerializeField] private float _moveSpeed = 5f;

private void OnValidate()
{
    if (_moveSpeed < 0)
        _moveSpeed = 0;
}
```

## Git Considerations

The following should be in `.gitignore` (if using version control):
- `Library/` - Unity's cache directory (never commit)
- `Temp/` - Temporary build files
- `Logs/` - Log files
- `*.csproj`, `*.sln` - Auto-generated IDE project files
- `UserSettings/` - User-specific settings

Always commit `.meta` files alongside their associated assets.
- Use only Assets folder. This is the only needed folder for you to create changes.
- Do not place any md files in scripts folder. Rather use .claude folder, additinally add subfolders if needed.