# Lock/Unlock Button Setup - Implementation Summary

## ✅ Implementation Complete

All Lock and Unlock button functionality has been successfully implemented and connected to MessageSender.

---

## Changes Made

### 1. Created LockUnlockController Script
**File**: `Assets/Scripts/LockUnlockController.cs`

A controller script that manages Lock and Unlock message sending via a shared MessageSender component.

**Features**:
- ✅ Single shared MessageSender instance
- ✅ Separate endpoints for Lock (`/api/lock`) and Unlock (`/api/unlock`)
- ✅ Configurable payloads for each action
- ✅ Auto-finds MessageSender component if not assigned
- ✅ Public method `UpdateConnectionSettings()` for Settings UI (future)
- ✅ Validation and error handling

**Public Methods**:
```csharp
void SendLockMessage()          // Sends lock request to /api/lock
void SendUnlockMessage()        // Sends unlock request to /api/unlock
void UpdateConnectionSettings(string targetIp, int port, string apiKey)
void UpdateEndpoints(string lockEndpoint, string unlockEndpoint)
MessageSender GetMessageSender()
```

**Inspector Configuration**:
- **Message Sender**: Auto-assigned to MessageSender component on same GameObject
- **Lock Endpoint**: `/api/lock` (configurable)
- **Unlock Endpoint**: `/api/unlock` (configurable)
- **Lock Payload**: `{"action": "lock"}` (configurable)
- **Unlock Payload**: `{"action": "unlock"}` (configurable)

---

### 2. Added Components to MainCanvas

**Components Added to MainCanvas GameObject**:

#### **MessageSender Component** (ID: 1200936855)
- **Target URL**: Empty (placeholder - will be configured via Settings UI)
- **Port**: 8080 (default)
- **API Key**: Empty (placeholder - will be configured via Settings UI)
- **Use HTTPS**: False (default)
- **Default Endpoint**: `/api/lock`
- **Default Payload**: `{"action": "lock"}`
- **Timeout**: 10 seconds
- **Content Type**: `application/json`
- **HTTP Method**: POST

#### **LockUnlockController Component** (ID: 1200936856)
- **Message Sender Reference**: Points to MessageSender component (1200936855)
- **Lock Endpoint**: `/api/lock`
- **Unlock Endpoint**: `/api/unlock`
- **Lock Payload**: `{"action": "lock"}`
- **Unlock Payload**: `{"action": "unlock"}`

---

### 3. Connected Button OnClick Events

**File**: `Assets/Scenes/SampleScene.unity`

#### **UnlockButton**
- **OnClick Event**: Connected to `LockUnlockController.SendUnlockMessage()`
- **GameObject ID**: 1166767197
- **Position**: (-334, 887) - Left side
- **Label**: "Unlock"

#### **LockButton**
- **OnClick Event**: Connected to `LockUnlockController.SendLockMessage()`
- **GameObject ID**: 1131382882
- **Position**: (132, 887) - Middle
- **Label**: "Lock"

#### **SettingsButton**
- **OnClick Event**: Empty (reserved for future Settings UI)
- **GameObject ID**: 1227941231
- **Position**: (464.031, 887) - Right side
- **Label**: "Settings"

---

## Current UI Structure

```
MainCanvas
├─ Components:
│  ├─ Canvas
│  ├─ CanvasScaler
│  ├─ GraphicRaycaster
│  ├─ SimpleButtonController (debug logging)
│  ├─ MessageSender ✅ NEW (shared instance)
│  └─ LockUnlockController ✅ NEW
│
└─ ActionButtons
   ├─ UnlockButton → LockUnlockController.SendUnlockMessage() ✅
   ├─ LockButton → LockUnlockController.SendLockMessage() ✅
   └─ SettingsButton → (Not connected - future implementation)
```

---

## How It Works

### Lock Button Flow:
1. User clicks **LockButton**
2. `LockUnlockController.SendLockMessage()` is called
3. Controller updates MessageSender endpoint to `/api/lock`
4. Controller updates MessageSender payload to `{"action": "lock"}`
5. Controller calls `MessageSender.SendMessage()`
6. MessageSender sends HTTP POST request to configured server

### Unlock Button Flow:
1. User clicks **UnlockButton**
2. `LockUnlockController.SendUnlockMessage()` is called
3. Controller updates MessageSender endpoint to `/api/unlock`
4. Controller updates MessageSender payload to `{"action": "unlock"}`
5. Controller calls `MessageSender.SendMessage()`
6. MessageSender sends HTTP POST request to configured server

---

## Current Configuration Status

### ⚠️ Connection Settings (Placeholder - To Be Configured via Settings UI)

The following settings are currently **empty/placeholder** and will be configured when the Settings UI is implemented:

- **Target IP**: Empty string `""`
  - Needs to be set via Settings UI
  - Example: `192.168.1.100`

- **Port**: 8080 (default)
  - Can be changed via Settings UI
  - Example: `8080`, `443`

- **API Key**: Empty string `""`
  - Needs to be set via Settings UI
  - Example: `your-api-key-here`

### ✅ Endpoint Configuration (Pre-configured)

- **Lock Endpoint**: `/api/lock` ✅
- **Unlock Endpoint**: `/api/unlock` ✅

### ✅ Message Payloads (Pre-configured)

- **Lock Payload**: `{"action": "lock"}` ✅
- **Unlock Payload**: `{"action": "unlock"}` ✅

---

## Testing the Buttons (Without Server)

### In Unity Editor:
1. Open `Assets/Scenes/SampleScene.unity`
2. Click Play ▶️
3. Click the **UnlockButton** or **LockButton**
4. Check the Console window

### Expected Console Output:

**When clicking UnlockButton**:
```
[LockUnlockController] Sending UNLOCK message to endpoint: /api/unlock
[MessageSender] Configuration is invalid. Cannot send message.
```

**When clicking LockButton**:
```
[LockUnlockController] Sending LOCK message to endpoint: /api/lock
[MessageSender] Configuration is invalid. Cannot send message.
```

⚠️ **Note**: Messages will fail validation because Target IP is empty. This is expected! The IP, Port, and API Key will be configured via the Settings UI in the future.

---

## Testing with a Server

To test with an actual server, you can temporarily set the connection settings:

### Option 1: Set in Inspector (Unity Editor)
1. Select **MainCanvas** in Hierarchy
2. In Inspector, find **MessageSender** component
3. Expand **Connection Config**
4. Set **Target URL**: Your server IP (e.g., `192.168.1.100`)
5. Set **API Key**: Your API key
6. Click Play and test buttons

### Option 2: Set Programmatically (For Testing)
Add this temporary code to `LockUnlockController.Awake()`:
```csharp
private void Awake()
{
    // Auto-find MessageSender if not assigned
    if (_messageSender == null)
    {
        _messageSender = GetComponent<MessageSender>();
    }

    // TEMPORARY: Set connection for testing
    UpdateConnectionSettings("192.168.1.100", 8080, "test-api-key");
}
```

**Expected Output with Valid Server**:
```
[LockUnlockController] Sending UNLOCK message to endpoint: /api/unlock
[MessageSender] Sending message to: http://192.168.1.100:8080/api/unlock
[MessageSender] Success! Status: 200, Response: {"result": "unlocked"}
```

---

## Future: Settings UI Integration

When the Settings UI is implemented, it should call `LockUnlockController.UpdateConnectionSettings()`:

```csharp
// Example Settings UI code (future implementation)
public class SettingsUI : MonoBehaviour
{
    [SerializeField] private LockUnlockController _lockUnlockController;
    [SerializeField] private TMP_InputField _ipField;
    [SerializeField] private TMP_InputField _portField;
    [SerializeField] private TMP_InputField _apiKeyField;

    public void OnSaveButtonClick()
    {
        string ip = _ipField.text;
        int port = int.Parse(_portField.text);
        string apiKey = _apiKeyField.text;

        _lockUnlockController.UpdateConnectionSettings(ip, port, apiKey);

        Debug.Log("Connection settings saved!");
    }
}
```

---

## HTTP Request Details

### Lock Request
```http
POST http://{targetIp}:{port}/api/lock
Content-Type: application/json
x-api-key: {apiKey}

{"action": "lock"}
```

### Unlock Request
```http
POST http://{targetIp}:{port}/api/unlock
Content-Type: application/json
x-api-key: {apiKey}

{"action": "unlock"}
```

---

## Files Modified/Created

| File | Status | Description |
|------|--------|-------------|
| `Assets/Scripts/LockUnlockController.cs` | ✅ Created | Controller for Lock/Unlock functionality |
| `Assets/Scenes/SampleScene.unity` | ✅ Modified | Added components, connected button events |

---

## Component References

### GameObject IDs:
- **MainCanvas**: 1200936849
- **MessageSender Component**: 1200936855
- **LockUnlockController Component**: 1200936856
- **UnlockButton**: 1166767197
- **LockButton**: 1131382882
- **SettingsButton**: 1227941231

### Script GUIDs:
- **MessageSender**: `35e7a503025614060964995517abd0c3`
- **LockUnlockController**: `d0cbbcb7d8f3443f68a730af8559f6fe`

---

## Summary

### ✅ Completed:
1. ✅ Created LockUnlockController script
2. ✅ Added MessageSender component to MainCanvas
3. ✅ Added LockUnlockController component to MainCanvas
4. ✅ Connected UnlockButton to SendUnlockMessage()
5. ✅ Connected LockButton to SendLockMessage()
6. ✅ Configured endpoints: `/api/lock` and `/api/unlock`
7. ✅ Configured payloads for both actions
8. ✅ Single shared MessageSender instance

### ⏳ Future Implementation:
- Settings UI to configure IP, Port, and API Key
- SettingsButton onClick functionality
- Save/Load connection settings
- Visual feedback for button states
- Error handling UI (success/failure notifications)

---

## Status: ✅ Lock and Unlock Functionality Ready!

The Lock and Unlock buttons are now fully functional and connected to MessageSender. They are ready to send HTTP requests once connection settings (IP, Port, API Key) are configured via the Settings UI (to be implemented).

**Next Step**: Implement Settings UI panel to allow users to input and save connection parameters.
