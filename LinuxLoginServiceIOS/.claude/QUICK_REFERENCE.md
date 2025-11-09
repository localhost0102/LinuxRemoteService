# MessageSender - Quick Reference Card

## 🚀 Quick Start (30 seconds)

### Setup
1. Attach `MessageSender` component to a GameObject
2. In Inspector, configure:
   - Connection Config → Target IP, Port, API Key
   - Message Config → Payload
3. Connect Button.OnClick to `MessageSender.SendMessage()`

Done! Button now sends HTTP requests.

---

## 📋 Common Usage Patterns

### 1️⃣ Simple Button Click (No Code)
```
Inspector Setup:
- MessageSender.Connection.TargetIp = "192.168.1.100"
- MessageSender.Connection.Port = 8080
- MessageSender.Connection.ApiKey = "my-key"
- Button.OnClick → MessageSender.SendMessage()
```

### 2️⃣ Multiple Buttons, Different Servers
```csharp
[SerializeField] private MessageSender _sender;

void Start() {
    var serverA = new ConnectionConfig("192.168.1.10", 8080, "key-A");
    var serverB = new ConnectionConfig("192.168.1.20", 8080, "key-B");

    _buttonA.onClick.AddListener(() => _sender.SendMessage(serverA));
    _buttonB.onClick.AddListener(() => _sender.SendMessage(serverB));
}
```

### 3️⃣ Runtime Configuration
```csharp
// Change properties directly
_sender.Connection.TargetIp = "10.0.0.50";
_sender.Connection.Port = 9000;
_sender.Connection.ApiKey = "new-key";
_sender.SendMessage();
```

### 4️⃣ Environment Switching
```csharp
[SerializeField] private ConnectionConfig _devConfig;
[SerializeField] private ConnectionConfig _prodConfig;

public void SwitchToProd() {
    _sender.UpdateConnectionConfig(_prodConfig);
}
```

### 5️⃣ Custom Message Per Button
```csharp
_button1.onClick.AddListener(() => {
    _sender.Message.Payload = "{\"action\": \"start\"}";
    _sender.SendMessage();
});

_button2.onClick.AddListener(() => {
    _sender.Message.Payload = "{\"action\": \"stop\"}";
    _sender.SendMessage();
});
```

---

## 🔧 API Cheat Sheet

### ConnectionConfig
```csharp
// Create
var conn = new ConnectionConfig("192.168.1.100", 8080, "api-key");

// Properties
conn.TargetIp = "192.168.1.100";
conn.Port = 8080;
conn.ApiKey = "my-key";
conn.UseHttps = true;
conn.Endpoint = "/api/v2/message";

// Methods
string url = conn.ConstructUrl();        // Build full URL
bool valid = conn.IsValid();              // Validate config
var copy = conn.Clone();                  // Deep copy
conn.CopyFrom(otherConfig);              // Copy values
```

### MessageConfig
```csharp
// Create
var msg = new MessageConfig("{\"test\": true}");

// Properties
msg.Payload = "{\"action\": \"test\"}";
msg.TimeoutSeconds = 15f;
msg.ContentType = "application/json";
msg.Method = MessageConfig.HttpMethod.POST;

// Methods
bool valid = msg.IsValid();               // Validate config
var copy = msg.Clone();                   // Deep copy
msg.CopyFrom(otherMsg);                   // Copy values
```

### MessageSender
```csharp
// Send Methods
_sender.SendMessage();                                    // Use Inspector config
_sender.SendMessage(connectionConfig);                    // Custom connection
_sender.SendMessage(connectionConfig, messageConfig);     // Both custom
_sender.SendMessageWithParams(ip, port, key);            // Quick params
_sender.SendCustomMessage(jsonPayload);                   // Custom payload

// Update Methods
_sender.UpdateConnectionConfig(config);                   // Update connection
_sender.UpdateConnectionParams(ip, port, key);           // Quick update
_sender.UpdateMessageConfig(msgConfig);                   // Update message

// Properties
_sender.Connection         // Access ConnectionConfig
_sender.Message           // Access MessageConfig
_sender.IsSending         // Check if sending
_sender.FullUrl           // Get constructed URL

// Events
_sender.OnMessageSent.AddListener((response) => { });
_sender.OnMessageFailed.AddListener((error) => { });
```

---

## 📝 Code Snippets

### Complete Example
```csharp
using UnityEngine;
using UnityEngine.UI;
using LinuxLoginService.Models;
using LinuxLoginService.Services;

public class MyController : MonoBehaviour
{
    [SerializeField] private MessageSender _sender;
    [SerializeField] private Button _button;

    private void Start()
    {
        // Configure
        var config = new ConnectionConfig("192.168.1.100", 8080, "key");
        _sender.UpdateConnectionConfig(config);

        // Setup button
        _button.onClick.AddListener(OnButtonClick);

        // Subscribe to events
        _sender.OnMessageSent.AddListener(OnSuccess);
        _sender.OnMessageFailed.AddListener(OnError);
    }

    private void OnButtonClick()
    {
        _sender.Message.Payload = "{\"action\": \"click\"}";
        _sender.SendMessage();
    }

    private void OnSuccess(string response)
    {
        Debug.Log($"Success: {response}");
    }

    private void OnError(string error)
    {
        Debug.LogError($"Error: {error}");
    }
}
```

### Dynamic Configuration from UI
```csharp
[SerializeField] private TMP_InputField _ipField;
[SerializeField] private TMP_InputField _portField;
[SerializeField] private TMP_InputField _keyField;

public void SendWithUserInput()
{
    var config = new ConnectionConfig(
        _ipField.text,
        int.Parse(_portField.text),
        _keyField.text
    );

    if (config.IsValid())
        _sender.SendMessage(config);
}
```

### Configuration Presets
```csharp
[Header("Presets")]
[SerializeField] private ConnectionConfig _localConfig;
[SerializeField] private ConnectionConfig _stagingConfig;
[SerializeField] private ConnectionConfig _productionConfig;

public void SendToLocal() => _sender.SendMessage(_localConfig);
public void SendToStaging() => _sender.SendMessage(_stagingConfig);
public void SendToProduction() => _sender.SendMessage(_productionConfig);
```

---

## 🎯 Common Scenarios

### Scenario: Login Button
```csharp
public void OnLoginButtonClick()
{
    string username = _usernameField.text;
    string password = _passwordField.text;

    string payload = $"{{\"username\": \"{username}\", \"password\": \"{password}\"}}";

    _sender.Message.Payload = payload;
    _sender.Connection.Endpoint = "/api/login";
    _sender.SendMessage();
}
```

### Scenario: Different Actions, Same Server
```csharp
private void SetupButtons()
{
    _startButton.onClick.AddListener(() => SendAction("start"));
    _stopButton.onClick.AddListener(() => SendAction("stop"));
    _pauseButton.onClick.AddListener(() => SendAction("pause"));
}

private void SendAction(string action)
{
    _sender.Message.Payload = $"{{\"action\": \"{action}\"}}";
    _sender.SendMessage();
}
```

### Scenario: Retry Logic
```csharp
private int _retryCount = 0;
private const int MAX_RETRIES = 3;

private void Start()
{
    _sender.OnMessageFailed.AddListener(OnFailedWithRetry);
}

private void OnFailedWithRetry(string error)
{
    if (_retryCount < MAX_RETRIES)
    {
        _retryCount++;
        Debug.Log($"Retry {_retryCount}/{MAX_RETRIES}");
        Invoke(nameof(RetryMessage), 2f);
    }
    else
    {
        Debug.LogError("Max retries reached");
        _retryCount = 0;
    }
}

private void RetryMessage()
{
    _sender.SendMessage();
}
```

### Scenario: Loading Indicator
```csharp
[SerializeField] private GameObject _loadingSpinner;

private void Start()
{
    _sender.OnMessageSent.AddListener(_ => HideLoading());
    _sender.OnMessageFailed.AddListener(_ => HideLoading());
}

public void SendWithLoading()
{
    _loadingSpinner.SetActive(true);
    _sender.SendMessage();
}

private void HideLoading()
{
    _loadingSpinner.SetActive(false);
}
```

---

## ⚙️ Configuration Examples

### Development Server
```csharp
var devConfig = new ConnectionConfig()
{
    TargetIp = "127.0.0.1",
    Port = 8080,
    ApiKey = "dev-key-12345",
    UseHttps = false,
    Endpoint = "/api/test"
};
```

### Production Server
```csharp
var prodConfig = new ConnectionConfig()
{
    TargetIp = "api.myapp.com",
    Port = 443,
    ApiKey = "prod-secure-key-xyz",
    UseHttps = true,
    Endpoint = "/api/v2/data"
};
```

### Custom HTTP Method
```csharp
var msg = new MessageConfig()
{
    Payload = "{\"id\": 123}",
    Method = MessageConfig.HttpMethod.DELETE,
    TimeoutSeconds = 5f
};
```

---

## 🐛 Troubleshooting

### Problem: "Invalid configuration" error
**Solution**: Check that IP is not empty, port is 1-65535, endpoint is not empty
```csharp
if (!_sender.Connection.IsValid())
    Debug.LogError("Connection config invalid!");
```

### Problem: Request times out
**Solution**: Increase timeout or check server availability
```csharp
_sender.Message.TimeoutSeconds = 30f;  // Increase timeout
```

### Problem: "Already sending" warning
**Solution**: Check IsSending before sending again
```csharp
if (!_sender.IsSending)
    _sender.SendMessage();
```

### Problem: API key not working
**Solution**: Verify API key is correct and has no spaces
```csharp
Debug.Log($"Using API Key: {_sender.Connection.ApiKey}");
Debug.Log($"Full URL: {_sender.FullUrl}");
```

---

## 📚 File Locations

```
Assets/Scripts/
├── Models/
│   ├── ConnectionConfig.cs       ← Connection parameters
│   └── MessageConfig.cs          ← Message parameters
│
├── MessageSender.cs              ← Main service
├── MessageSenderExample.cs       ← Full examples
├── MessageSender_OOP_Guide.md    ← Complete guide
├── QUICK_REFERENCE.md            ← This file
└── CLASS_DIAGRAM.md              ← Architecture diagrams
```

---

## 🔍 Inspector Layout

```
MessageSender Component
│
├─ Configuration
│  ├─ Connection Config
│  │  ├─ Target Ip: [192.168.1.100]
│  │  ├─ Port: [8080]
│  │  ├─ Api Key: [my-api-key]
│  │  ├─ Use Https: [ ]
│  │  └─ Endpoint: [/api/message]
│  │
│  └─ Message Config
│     ├─ Payload: [{"message": "Hello"}]
│     ├─ Timeout Seconds: [10]
│     ├─ Content Type: [application/json]
│     └─ Http Method: [POST ▼]
│
└─ Events
   ├─ On Message Sent (String)
   │  └─ [+] Add Event
   │
   └─ On Message Failed (String)
      └─ [+] Add Event
```

---

## 🎓 Best Practices

✅ **DO:**
- Validate configs before sending: `config.IsValid()`
- Subscribe to events for user feedback
- Use configuration presets for different environments
- Check `IsSending` to prevent spam
- Use HTTPS in production

❌ **DON'T:**
- Hardcode sensitive API keys (use config files)
- Send requests in Update() loop
- Ignore validation errors
- Forget to unsubscribe from events

---

## 💡 Pro Tips

1. **Create ScriptableObjects for configs**
   ```csharp
   [CreateAssetMenu]
   public class ConnectionConfigSO : ScriptableObject
   {
       public ConnectionConfig config;
   }
   ```

2. **Use enums for server selection**
   ```csharp
   public enum ServerType { Development, Staging, Production }
   private ConnectionConfig GetConfig(ServerType type) { }
   ```

3. **Log full URLs for debugging**
   ```csharp
   Debug.Log($"Sending to: {_sender.FullUrl}");
   ```

4. **Chain messages with events**
   ```csharp
   _sender.OnMessageSent.AddListener(response => SendNextMessage());
   ```

---

## 📞 Need More Help?

- **Full Documentation**: See `MessageSender_OOP_Guide.md`
- **Architecture**: See `CLASS_DIAGRAM.md`
- **Examples**: See `MessageSenderExample.cs`
- **Original README**: See `MessageSender_README.md`

---

**Version**: 2.0 (OOP Refactored)
**Last Updated**: 2025-01-08
