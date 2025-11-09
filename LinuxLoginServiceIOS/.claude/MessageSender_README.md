# MessageSender Component - Usage Guide

## Overview
The **MessageSender** component is a flexible, reusable script that allows UI buttons to send HTTP POST requests with configurable parameters. Each button can use the same script with different configurations for target IP, port, and x-api-key.

## Features
✅ Configurable target IP, port, and API key
✅ Support for HTTP/HTTPS
✅ Custom endpoints and payloads
✅ Event-driven success/failure callbacks
✅ Runtime parameter modification
✅ Automatic timeout handling
✅ Multiple instances with different configs

---

## Quick Start

### 1. Setup in Unity Editor

#### For Each Button:
1. Create a UI Button in your scene
2. Add an **empty GameObject** as a child or sibling
3. Attach the **MessageSender** component to this GameObject
4. Configure the MessageSender in the Inspector:
   - **Target IP**: e.g., `192.168.1.100`
   - **Port**: e.g., `8080`
   - **API Key**: e.g., `your-api-key-here`
   - **Message Payload**: JSON message to send
5. In the Button's **OnClick()** event:
   - Drag the MessageSender GameObject
   - Select `MessageSender.SendMessage()`

### 2. Example Scene Setup

```
Canvas
├── Button1 (UI Button)
│   └── MessageSender1 (GameObject with MessageSender component)
│       - Target IP: 192.168.1.100
│       - Port: 8080
│       - API Key: key-for-server-1
│
├── Button2 (UI Button)
│   └── MessageSender2 (GameObject with MessageSender component)
│       - Target IP: 10.0.0.50
│       - Port: 9000
│       - API Key: key-for-server-2
│
└── Button3 (UI Button)
    └── MessageSender3 (GameObject with MessageSender component)
        - Target IP: 127.0.0.1
        - Port: 3000
        - API Key: localhost-key
```

---

## Configuration Options

### Inspector Settings

#### Connection Settings
- **Target IP**: IP address of the target server (e.g., `192.168.1.100`, `127.0.0.1`)
- **Port**: Port number (1-65535, default: 8080)
- **API Key**: Authentication key sent in `x-api-key` header

#### Request Settings
- **Use HTTPS**: Toggle between HTTP and HTTPS
- **Endpoint**: API endpoint path (e.g., `/api/message`, `/webhook`)
- **Timeout Seconds**: Request timeout in seconds (default: 10)

#### Message Content
- **Message Payload**: JSON string to send in the request body

#### Events
- **OnMessageSent**: Triggered on successful response (returns response body)
- **OnMessageFailed**: Triggered on error (returns error message)

---

## Usage Methods

### Method 1: Direct Inspector Configuration (Recommended)
Each button has its own MessageSender with pre-configured settings.

**Setup:**
```
1. Attach MessageSender to a GameObject
2. Configure Target IP, Port, API Key in Inspector
3. Connect Button.OnClick to MessageSender.SendMessage()
```

**Pros:**
- Visual configuration in Unity Editor
- No code required
- Easy to modify per button

---

### Method 2: Runtime Parameter Override
Configure in Inspector but override at runtime.

**C# Example:**
```csharp
public class MyController : MonoBehaviour
{
    [SerializeField] private MessageSender messageSender;
    [SerializeField] private Button myButton;

    private void Start()
    {
        myButton.onClick.AddListener(() =>
        {
            messageSender.SendMessageWithParams(
                targetIp: "192.168.1.100",
                port: 9000,
                apiKey: "runtime-key-123"
            );
        });
    }
}
```

---

### Method 3: Update Then Send
Update parameters programmatically before sending.

**C# Example:**
```csharp
private void OnButtonClick()
{
    messageSender.UpdateConnectionParams(
        targetIp: "10.0.0.50",
        port: 8443,
        apiKey: "new-api-key"
    );

    messageSender.SendMessage();
}
```

---

### Method 4: Runtime Property Modification
Directly modify properties at runtime.

**C# Example:**
```csharp
private void ConfigureForProduction()
{
    messageSender.TargetIp = "production.server.com";
    messageSender.Port = 443;
    messageSender.ApiKey = "prod-key-xyz";
    messageSender.MessagePayload = "{\"env\": \"production\"}";
}
```

---

## Event Handling

### Subscribe to Success/Failure Events

**C# Example:**
```csharp
public class EventHandler : MonoBehaviour
{
    [SerializeField] private MessageSender messageSender;

    private void Start()
    {
        // Subscribe to events
        messageSender.OnMessageSent.AddListener(OnSuccess);
        messageSender.OnMessageFailed.AddListener(OnFailure);
    }

    private void OnSuccess(string response)
    {
        Debug.Log($"Success! Server responded: {response}");
        // Update UI, show notification, etc.
    }

    private void OnFailure(string error)
    {
        Debug.LogError($"Failed: {error}");
        // Show error dialog, retry, etc.
    }
}
```

### Inspector Event Setup
You can also connect events in the Inspector:
1. Expand the **OnMessageSent** or **OnMessageFailed** event
2. Click **+** to add a listener
3. Drag your GameObject with a receiver script
4. Select the method to call (e.g., `MyScript.OnSuccess`)

---

## Advanced Examples

### Example 1: Multiple Servers with Different Keys

```csharp
public class MultiServerController : MonoBehaviour
{
    [SerializeField] private MessageSender serverA;
    [SerializeField] private MessageSender serverB;
    [SerializeField] private MessageSender serverC;

    private void Start()
    {
        // Configure each server
        serverA.UpdateConnectionParams("192.168.1.10", 8080, "key-A");
        serverB.UpdateConnectionParams("192.168.1.20", 8080, "key-B");
        serverC.UpdateConnectionParams("192.168.1.30", 9000, "key-C");
    }

    public void SendToAllServers()
    {
        serverA.SendMessage();
        serverB.SendMessage();
        serverC.SendMessage();
    }
}
```

---

### Example 2: Dynamic Payload Based on User Input

```csharp
public class DynamicMessageController : MonoBehaviour
{
    [SerializeField] private MessageSender messageSender;
    [SerializeField] private TMP_InputField inputField;

    public void SendCustomMessage()
    {
        string customPayload = $"{{\"user_input\": \"{inputField.text}\"}}";
        messageSender.SendCustomMessage(customPayload);
    }
}
```

---

### Example 3: Environment-Based Configuration

```csharp
public class EnvironmentConfig : MonoBehaviour
{
    [SerializeField] private MessageSender messageSender;

    public enum Environment { Development, Staging, Production }
    [SerializeField] private Environment currentEnvironment;

    private void Start()
    {
        ConfigureForEnvironment();
    }

    private void ConfigureForEnvironment()
    {
        switch (currentEnvironment)
        {
            case Environment.Development:
                messageSender.UpdateConnectionParams("127.0.0.1", 8080, "dev-key");
                break;

            case Environment.Staging:
                messageSender.UpdateConnectionParams("staging.myapp.com", 8080, "staging-key");
                break;

            case Environment.Production:
                messageSender.UpdateConnectionParams("api.myapp.com", 443, "prod-key");
                break;
        }
    }
}
```

---

## Troubleshooting

### Issue: Button doesn't send message
**Solution:**
- Verify MessageSender component is attached
- Check Button.OnClick is connected to `MessageSender.SendMessage()`
- Check console for error messages

### Issue: Request times out
**Solution:**
- Verify target IP and port are correct
- Check if server is running and accessible
- Increase **Timeout Seconds** in Inspector

### Issue: "Already sending a message" warning
**Solution:**
- Wait for previous request to complete
- Check if you're clicking too fast
- Increase timeout or disable button during sending

### Issue: API key not working
**Solution:**
- Verify the API key is correct
- Check server logs to see if header is received
- Ensure no extra spaces in the API key field

---

## Best Practices

1. **One MessageSender per Button**: Each button should have its own MessageSender instance for clarity
2. **Use Inspector Configuration**: Configure static values (IP, port, key) in Inspector for easy modification
3. **Subscribe to Events**: Always handle success/failure events for better UX
4. **Validate Input**: Check `messageSender.IsSending` before sending to prevent spam
5. **Use HTTPS in Production**: Enable **Use HTTPS** for secure communication
6. **Handle Errors Gracefully**: Show user-friendly error messages, not technical details
7. **Test Timeout Values**: Set appropriate timeout based on expected server response time

---

## API Reference

### Public Methods

| Method | Description |
|--------|-------------|
| `SendMessage()` | Sends message with current configuration |
| `SendMessageWithParams(ip, port, key)` | Sends with temporary override parameters |
| `SendCustomMessage(payload)` | Sends custom payload with current settings |
| `UpdateConnectionParams(ip, port, key)` | Updates connection parameters permanently |

### Public Properties

| Property | Type | Description |
|----------|------|-------------|
| `TargetIp` | string | Get/set target IP address |
| `Port` | int | Get/set target port |
| `ApiKey` | string | Get/set API key |
| `MessagePayload` | string | Get/set message payload |
| `IsSending` | bool | Check if currently sending (read-only) |
| `FullUrl` | string | Get constructed full URL (read-only) |

### Events

| Event | Parameter | Description |
|-------|-----------|-------------|
| `OnMessageSent` | string response | Triggered on successful send |
| `OnMessageFailed` | string error | Triggered on failure |

---

## Example JSON Payloads

### Simple Message
```json
{
  "message": "Hello from Unity",
  "timestamp": "2025-01-08T12:00:00Z"
}
```

### Complex Data
```json
{
  "action": "player_action",
  "data": {
    "player_id": "12345",
    "position": { "x": 10.5, "y": 0, "z": -3.2 },
    "health": 100
  }
}
```

### Authentication Request
```json
{
  "username": "player1",
  "device_id": "unity-ios-device-001"
}
```

---

## Notes

- **Thread Safety**: HTTP requests run asynchronously but callbacks execute on Unity main thread
- **Static HttpClient**: All MessageSender instances share one HttpClient for efficiency
- **Error Handling**: All exceptions are caught and reported through OnMessageFailed event
- **Timeout Behavior**: Requests automatically cancel after specified timeout

---

## Support

For issues or questions, refer to Unity documentation:
- [UnityWebRequest](https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequest.html)
- [UnityEvents](https://docs.unity3d.com/ScriptReference/Events.UnityEvent.html)
- [UI Button](https://docs.unity3d.com/Packages/com.unity.ugui@1.0/manual/script-Button.html)
