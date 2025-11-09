using UnityEngine;
using LinuxLoginService.Models;
using LinuxLoginService.Services;

/// <summary>
/// Controller for Lock and Unlock button functionality.
/// Manages sending lock/unlock messages to the server via MessageSender.
/// </summary>
public class LockUnlockController : MonoBehaviour
{
    #region Serialized Fields
    [Header("Message Sender Reference")]
    [Tooltip("Reference to the MessageSender component")]
    [SerializeField] private MessageSender _messageSender;

    [Header("Endpoint Configuration")]
    [Tooltip("Endpoint path for lock action")]
    [SerializeField] private string _lockEndpoint = "/api/lock";

    [Tooltip("Endpoint path for unlock action")]
    [SerializeField] private string _unlockEndpoint = "/api/unlock";

    [Header("Message Payloads")]
    [Tooltip("JSON payload for lock message")]
    [TextArea(2, 5)]
    [SerializeField] private string _lockPayload = "{\"action\": \"lock\"}";

    [Tooltip("JSON payload for unlock message")]
    [TextArea(2, 5)]
    [SerializeField] private string _unlockPayload = "{\"action\": \"unlock\"}";
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        // Auto-find MessageSender if not assigned
        if (_messageSender == null)
        {
            _messageSender = GetComponent<MessageSender>();

            if (_messageSender == null)
            {
                Debug.LogError("[LockUnlockController] MessageSender component not found! Please assign it in the Inspector.");
            }
        }
    }

    private void OnValidate()
    {
        // Validate endpoints
        if (string.IsNullOrWhiteSpace(_lockEndpoint))
        {
            Debug.LogWarning("[LockUnlockController] Lock endpoint is empty!");
        }

        if (string.IsNullOrWhiteSpace(_unlockEndpoint))
        {
            Debug.LogWarning("[LockUnlockController] Unlock endpoint is empty!");
        }
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Sends a lock message to the server.
    /// This method can be called from UI Button onClick events.
    /// </summary>
    public void SendLockMessage()
    {
        if (_messageSender == null)
        {
            Debug.LogError("[LockUnlockController] Cannot send lock message - MessageSender is null!");
            return;
        }

        // Update endpoint for lock action
        _messageSender.Connection.Endpoint = _lockEndpoint;

        // Update payload for lock action
        _messageSender.Message.Payload = _lockPayload;

        // Send the message
        _messageSender.SendMessage();
    }

    /// <summary>
    /// Sends an unlock message to the server.
    /// This method can be called from UI Button onClick events.
    /// </summary>
    public void SendUnlockMessage()
    {
        if (_messageSender == null)
        {
            Debug.LogError("[LockUnlockController] Cannot send unlock message - MessageSender is null!");
            return;
        }

        Debug.Log($"[LockUnlockController] Sending UNLOCK message to endpoint: {_unlockEndpoint}");

        // Update endpoint for unlock action
        _messageSender.Connection.Endpoint = _unlockEndpoint;

        // Update payload for unlock action
        _messageSender.Message.Payload = _unlockPayload;

        // Send the message
        _messageSender.SendMessage();
    }

    /// <summary>
    /// Updates the connection settings (IP, Port, API Key).
    /// This will be called from the Settings UI in the future.
    /// </summary>
    /// <param name="targetIp">Target server IP address</param>
    /// <param name="port">Target server port</param>
    /// <param name="apiKey">API key for authentication</param>
    public void UpdateConnectionSettings(string targetIp, int port, string apiKey)
    {
        if (_messageSender == null)
        {
            Debug.LogError("[LockUnlockController] Cannot update connection - MessageSender is null!");
            return;
        }

        _messageSender.Connection.TargetIp = targetIp;
        _messageSender.Connection.Port = port;
        _messageSender.Connection.ApiKey = apiKey;

        Debug.Log($"[LockUnlockController] Connection settings updated: {_messageSender.Connection.TargetIp}:{_messageSender.Connection.Port}");
    }

    /// <summary>
    /// Updates the lock and unlock endpoints.
    /// </summary>
    /// <param name="lockEndpoint">New lock endpoint path</param>
    /// <param name="unlockEndpoint">New unlock endpoint path</param>
    public void UpdateEndpoints(string lockEndpoint, string unlockEndpoint)
    {
        _lockEndpoint = lockEndpoint;
        _unlockEndpoint = unlockEndpoint;

        Debug.Log($"[LockUnlockController] Endpoints updated - Lock: {_lockEndpoint}, Unlock: {_unlockEndpoint}");
    }

    /// <summary>
    /// Gets the current MessageSender reference (for external access if needed).
    /// </summary>
    /// <returns>MessageSender instance</returns>
    public MessageSender GetMessageSender()
    {
        return _messageSender;
    }
    #endregion
}
