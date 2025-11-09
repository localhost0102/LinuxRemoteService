# MessageSender - OOP Architecture Guide

## Overview
The MessageSender system has been refactored following Object-Oriented Programming (OOP) principles. Instead of exposing individual properties, it now uses **model classes** to encapsulate related configuration data.

---

## Architecture

### Class Structure

```
Models/
├── ConnectionConfig.cs       - Connection parameters (IP, port, API key, endpoint)
└── MessageConfig.cs          - Message parameters (payload, timeout, HTTP method)

Services/
└── MessageSender.cs          - Main service using the model classes
```

---

## Model Classes

### 1. ConnectionConfig (`Models/ConnectionConfig.cs`)

**Purpose**: Encapsulates all connection-related parameters.

**Properties**:
- `TargetIp` (string) - Target IP address
- `Port` (int) - Target port (validated: 1-65535)
- `ApiKey` (string) - API key for x-api-key header
- `UseHttps` (bool) - Use HTTPS instead of HTTP
- `Endpoint` (string) - API endpoint path
- `Protocol` (string) - Returns "http" or "https" based on UseHttps

**Methods**:
- `ConstructUrl()` - Builds full URL from configuration
- `IsValid()` - Validates the configuration
- `Clone()` - Creates a deep copy
- `CopyFrom(ConnectionConfig)` - Copies values from another config
- `ToString()` - Returns formatted string representation

**Constructors**:
```csharp
// Default constructor
new ConnectionConfig()

// With basic parameters
new ConnectionConfig(targetIp, port, apiKey)

// With all parameters
new ConnectionConfig(targetIp, port, apiKey, useHttps, endpoint)
```

**Example Usage**:
```csharp
var config = new ConnectionConfig("192.168.1.100", 8080, "my-api-key");
config.UseHttps = true;
config.Endpoint = "/api/v2/message";

Debug.Log(config.ConstructUrl());
// Output: https://192.168.1.100:8080/api/v2/message
```

---

### 2. MessageConfig (`Models/MessageConfig.cs`)

**Purpose**: Encapsulates all message-related parameters.

**Properties**:
- `Payload` (string) - JSON message payload
- `TimeoutSeconds` (float) - Request timeout (validated: > 0)
- `ContentType` (string) - HTTP content type
- `Method` (HttpMethod enum) - HTTP method (GET, POST, PUT, DELETE, PATCH)

**Methods**:
- `IsValid()` - Validates the configuration
- `Clone()` - Creates a deep copy
- `CopyFrom(MessageConfig)` - Copies values from another config
- `ToString()` - Returns formatted string representation

**Constructors**:
```csharp
// Default constructor
new MessageConfig()

// With payload
new MessageConfig(payload)

// With all parameters
new MessageConfig(payload, timeoutSeconds, contentType, httpMethod)
```

**Example Usage**:
```csharp
var message = new MessageConfig("{\"action\": \"test\"}");
message.TimeoutSeconds = 15f;
message.Method = MessageConfig.HttpMethod.POST;
message.ContentType = "application/json";
```

---

## MessageSender Service

### Refactored Design

**Before (Old Approach)**:
```csharp
public class MessageSender
{
    [SerializeField] private string _targetIp;
    [SerializeField] private int _port;
    [SerializeField] private string _apiKey;
    [SerializeField] private string _endpoint;
    [SerializeField] private string _messagePayload;
    [SerializeField] private float _timeoutSeconds;
    // ... many individual properties
}
```

**After (OOP Approach)**:
```csharp
public class MessageSender
{
    [SerializeField] private ConnectionConfig _connectionConfig;
    [SerializeField] private MessageConfig _messageConfig;
    // Clean, organized, maintainable!
}
```

---

## Benefits of OOP Refactoring

### 1. **Encapsulation**
- Related properties grouped into cohesive model classes
- Validation logic contained within model classes
- Single Responsibility Principle (SRP) followed

### 2. **Reusability**
- Model classes can be used across different services
- Easy to create configuration presets
- Serializable for saving/loading configurations

### 3. **Maintainability**
- Changes to connection logic only affect ConnectionConfig
- Changes to message logic only affect MessageConfig
- Cleaner, more organized codebase

### 4. **Flexibility**
- Easy to swap entire configurations
- Support for configuration presets
- Runtime configuration changes simplified

### 5. **Testability**
- Models can be unit tested independently
- Mock configurations easy to create
- Better separation of concerns

---

## Usage Examples

### Example 1: Inspector Configuration (Simplest)

**Setup in Unity Inspector**:
1. Attach MessageSender to GameObject
2. Expand "Connection Config" in Inspector
   - Set Target IP: `192.168.1.100`
   - Set Port: `8080`
   - Set API Key: `your-key-here`
3. Expand "Message Config" in Inspector
   - Set Payload: `{"message": "Hello"}`
4. Connect Button.OnClick to `MessageSender.SendMessage()`

**No code required!**

---

### Example 2: Programmatic Configuration

```csharp
public class Example : MonoBehaviour
{
    [SerializeField] private MessageSender _messageSender;

    private void Start()
    {
        // Configure using properties
        _messageSender.Connection.TargetIp = "192.168.1.100";
        _messageSender.Connection.Port = 8080;
        _messageSender.Connection.ApiKey = "my-key";

        _messageSender.Message.Payload = "{\"data\": \"test\"}";

        // Send message
        _messageSender.SendMessage();
    }
}
```

---

### Example 3: Using Model Objects

```csharp
public class Example : MonoBehaviour
{
    [SerializeField] private MessageSender _messageSender;

    private void Start()
    {
        // Create connection config
        var connection = new ConnectionConfig(
            targetIp: "api.example.com",
            port: 443,
            apiKey: "secure-key"
        )
        {
            UseHttps = true,
            Endpoint = "/api/v2/data"
        };

        // Create message config
        var message = new MessageConfig(
            payload: "{\"action\": \"create\", \"item\": \"test\"}",
            timeoutSeconds: 20f
        );

        // Send with custom configs
        _messageSender.SendMessage(connection, message);
    }
}
```

---

### Example 4: Configuration Presets

```csharp
public class MultiEnvironment : MonoBehaviour
{
    [SerializeField] private MessageSender _messageSender;

    [Header("Environment Configurations")]
    [SerializeField] private ConnectionConfig _developmentConfig;
    [SerializeField] private ConnectionConfig _stagingConfig;
    [SerializeField] private ConnectionConfig _productionConfig;

    public void SendToDevelopment()
    {
        _messageSender.SendMessage(_developmentConfig);
    }

    public void SendToStaging()
    {
        _messageSender.SendMessage(_stagingConfig);
    }

    public void SendToProduction()
    {
        _messageSender.SendMessage(_productionConfig);
    }

    public void SwitchEnvironment(EnvironmentType env)
    {
        ConnectionConfig config = env switch
        {
            EnvironmentType.Development => _developmentConfig,
            EnvironmentType.Staging => _stagingConfig,
            EnvironmentType.Production => _productionConfig,
            _ => _developmentConfig
        };

        _messageSender.UpdateConnectionConfig(config);
    }
}

public enum EnvironmentType { Development, Staging, Production }
```

---

### Example 5: Multiple Buttons with Different Configs

```csharp
public class ButtonManager : MonoBehaviour
{
    [SerializeField] private MessageSender _messageSender;

    [Header("Buttons")]
    [SerializeField] private Button _serverAButton;
    [SerializeField] private Button _serverBButton;
    [SerializeField] private Button _serverCButton;

    private ConnectionConfig _serverAConfig;
    private ConnectionConfig _serverBConfig;
    private ConnectionConfig _serverCConfig;

    private void Awake()
    {
        // Initialize configurations
        _serverAConfig = new ConnectionConfig("192.168.1.10", 8080, "key-A");
        _serverBConfig = new ConnectionConfig("192.168.1.20", 8080, "key-B");
        _serverCConfig = new ConnectionConfig("192.168.1.30", 9000, "key-C");

        // Setup buttons
        _serverAButton.onClick.AddListener(() =>
            _messageSender.SendMessage(_serverAConfig));

        _serverBButton.onClick.AddListener(() =>
            _messageSender.SendMessage(_serverBConfig));

        _serverCButton.onClick.AddListener(() =>
            _messageSender.SendMessage(_serverCConfig));
    }
}
```

---

### Example 6: Dynamic Configuration from User Input

```csharp
public class DynamicConfig : MonoBehaviour
{
    [SerializeField] private MessageSender _messageSender;

    [Header("UI Input Fields")]
    [SerializeField] private TMP_InputField _ipInput;
    [SerializeField] private TMP_InputField _portInput;
    [SerializeField] private TMP_InputField _apiKeyInput;
    [SerializeField] private TMP_InputField _payloadInput;
    [SerializeField] private Button _sendButton;

    private void Start()
    {
        _sendButton.onClick.AddListener(SendWithUserInput);
    }

    private void SendWithUserInput()
    {
        // Create config from user input
        var connection = new ConnectionConfig(
            targetIp: _ipInput.text,
            port: int.Parse(_portInput.text),
            apiKey: _apiKeyInput.text
        );

        var message = new MessageConfig(_payloadInput.text);

        // Validate before sending
        if (connection.IsValid() && message.IsValid())
        {
            _messageSender.SendMessage(connection, message);
        }
        else
        {
            Debug.LogError("Invalid configuration from user input");
        }
    }
}
```

---

### Example 7: Configuration Persistence (Save/Load)

```csharp
public class ConfigPersistence : MonoBehaviour
{
    [SerializeField] private MessageSender _messageSender;

    private const string CONFIG_KEY = "SavedConnectionConfig";

    // Save current configuration
    public void SaveConfiguration()
    {
        var config = _messageSender.Connection;
        string json = JsonUtility.ToJson(config);
        PlayerPrefs.SetString(CONFIG_KEY, json);
        PlayerPrefs.Save();

        Debug.Log("Configuration saved!");
    }

    // Load saved configuration
    public void LoadConfiguration()
    {
        if (PlayerPrefs.HasKey(CONFIG_KEY))
        {
            string json = PlayerPrefs.GetString(CONFIG_KEY);
            var config = JsonUtility.FromJson<ConnectionConfig>(json);

            _messageSender.UpdateConnectionConfig(config);

            Debug.Log($"Configuration loaded: {config}");
        }
        else
        {
            Debug.LogWarning("No saved configuration found");
        }
    }
}
```

---

## API Reference

### MessageSender Public API

#### Send Methods
```csharp
// Send with current Inspector configuration
void SendMessage()

// Send with custom connection config
void SendMessage(ConnectionConfig connectionConfig)

// Send with custom connection and message configs
void SendMessage(ConnectionConfig connectionConfig, MessageConfig messageConfig)

// Convenience: Send with individual parameters
void SendMessageWithParams(string targetIp, int port, string apiKey)

// Send custom payload with current connection
void SendCustomMessage(string customPayload)
```

#### Update Methods
```csharp
// Update entire connection configuration
void UpdateConnectionConfig(ConnectionConfig connectionConfig)

// Update individual connection parameters
void UpdateConnectionParams(string targetIp, int port, string apiKey)

// Update entire message configuration
void UpdateMessageConfig(MessageConfig messageConfig)
```

#### Properties
```csharp
ConnectionConfig Connection { get; }  // Get connection config
MessageConfig Message { get; }        // Get message config
bool IsSending { get; }               // Check if currently sending
string FullUrl { get; }               // Get constructed URL
```

#### Events
```csharp
UnityEvent<string> OnMessageSent     // Success event (response body)
UnityEvent<string> OnMessageFailed   // Failure event (error message)
```

---

## Comparison: Before vs After

### Creating and Using Configuration

**Before (Scattered Properties)**:
```csharp
messageSender.TargetIp = "192.168.1.100";
messageSender.Port = 8080;
messageSender.ApiKey = "my-key";
messageSender.Endpoint = "/api/message";
messageSender.UseHttps = false;
messageSender.MessagePayload = "{\"test\": true}";
messageSender.TimeoutSeconds = 10f;
// Hard to group, reuse, or validate together
```

**After (Model Classes)**:
```csharp
var config = new ConnectionConfig("192.168.1.100", 8080, "my-key")
{
    Endpoint = "/api/message",
    UseHttps = false
};

var message = new MessageConfig("{\"test\": true}")
{
    TimeoutSeconds = 10f
};

// Clean, reusable, validatable, and follows OOP principles
messageSender.SendMessage(config, message);
```

---

## Design Patterns Used

### 1. **Data Transfer Object (DTO)**
- `ConnectionConfig` and `MessageConfig` act as DTOs
- Transfer configuration data between components
- Serializable for Inspector and persistence

### 2. **Encapsulation**
- Related properties grouped into cohesive classes
- Validation logic encapsulated within models
- Internal state hidden behind properties

### 3. **Builder Pattern (Partial)**
- Constructors provide different initialization options
- Object initializer syntax for optional parameters
- Fluent configuration possible

### 4. **Strategy Pattern (Implicit)**
- Different `ConnectionConfig` instances = different strategies
- Easy to swap configurations at runtime
- Supports multiple server targets

---

## Best Practices

1. ✅ **Use Model Classes for Configuration**
   - Create `ConnectionConfig` instances for different servers
   - Create `MessageConfig` instances for different message types

2. ✅ **Validate Configurations**
   - Always call `IsValid()` before using a config
   - Handle validation errors gracefully

3. ✅ **Reuse Configurations**
   - Create presets in Inspector or code
   - Use `Clone()` when you need variations

4. ✅ **Keep Models Simple**
   - Models should contain data and basic validation
   - Business logic stays in services (MessageSender)

5. ✅ **Use Serialization**
   - Mark models as `[Serializable]`
   - Enables Inspector editing and persistence

---

## Testing

### Unit Testing Models

```csharp
[Test]
public void ConnectionConfig_ValidatesPort()
{
    var config = new ConnectionConfig();

    config.Port = -1;
    Assert.IsFalse(config.IsValid());

    config.Port = 8080;
    Assert.IsTrue(config.IsValid());

    config.Port = 99999;
    Assert.IsFalse(config.IsValid());
}

[Test]
public void ConnectionConfig_ConstructsCorrectUrl()
{
    var config = new ConnectionConfig("192.168.1.100", 8080, "key")
    {
        UseHttps = true,
        Endpoint = "/api/test"
    };

    Assert.AreEqual("https://192.168.1.100:8080/api/test", config.ConstructUrl());
}

[Test]
public void ConnectionConfig_Clone_CreatesDeepCopy()
{
    var original = new ConnectionConfig("192.168.1.100", 8080, "key");
    var clone = original.Clone();

    clone.Port = 9000;

    Assert.AreEqual(8080, original.Port);
    Assert.AreEqual(9000, clone.Port);
}
```

---

## Summary

The refactored MessageSender system demonstrates proper OOP principles:

- **Separation of Concerns**: Models handle data, services handle logic
- **Encapsulation**: Related properties grouped into classes
- **Reusability**: Configuration objects can be reused across the application
- **Maintainability**: Changes to configuration structure are localized
- **Testability**: Models can be unit tested independently
- **Flexibility**: Easy to extend with new configuration types

This architecture makes it easy to:
- Create multiple buttons with different configurations
- Switch between environments (dev, staging, prod)
- Persist configurations
- Test individual components
- Extend functionality without breaking existing code
