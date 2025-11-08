using UnityEngine;
using UnityEngine.UI;
using LinuxLoginService.Models;
using LinuxLoginService.Services;

/// <summary>
/// Example demonstrating how to use MessageSender with multiple buttons.
/// This shows different ways to configure and use the MessageSender component
/// with the new OOP model-based approach.
/// </summary>
public class MessageSenderExample : MonoBehaviour
{
    #region Serialized Fields
    [Header("Button References")]
    [SerializeField] private Button _button1;
    [SerializeField] private Button _button2;
    [SerializeField] private Button _button3;
    [SerializeField] private Button _button4;

    [Header("MessageSender References")]
    [SerializeField] private MessageSender _messageSender1;
    [SerializeField] private MessageSender _messageSender2;
    [SerializeField] private MessageSender _messageSender3;
    [SerializeField] private MessageSender _messageSender4;

    [Header("Configuration Presets")]
    [SerializeField] private ConnectionConfig _developmentConfig;
    [SerializeField] private ConnectionConfig _productionConfig;
    #endregion

    #region Unity Lifecycle
    private void Start()
    {
        SetupButtons();
        ConfigureMessageSenders();
    }
    #endregion

    #region Private Methods
    private void SetupButtons()
    {
        // Method 1: Direct connection (configure MessageSender in Inspector)
        if (_button1 != null && _messageSender1 != null)
        {
            _button1.onClick.AddListener(_messageSender1.SendMessage);
        }

        // Method 2: Use with custom parameters at runtime
        if (_button2 != null && _messageSender2 != null)
        {
            _button2.onClick.AddListener(() =>
            {
                _messageSender2.SendMessageWithParams(
                    targetIp: "192.168.1.100",
                    port: 9000,
                    apiKey: "my-custom-api-key-123"
                );
            });
        }

        // Method 3: Use with custom ConnectionConfig model
        if (_button3 != null && _messageSender3 != null)
        {
            _button3.onClick.AddListener(() =>
            {
                var customConfig = new ConnectionConfig(
                    targetUrl: "10.0.0.50",
                    port: 8443,
                    apiKey: "production-key-456"
                );
                _messageSender3.SendMessage(customConfig);
            });
        }

        // Method 4: Use with both custom ConnectionConfig and MessageConfig
        if (_button4 != null && _messageSender4 != null)
        {
            _button4.onClick.AddListener(() =>
            {
                var connectionConfig = new ConnectionConfig("api.example.com", 443, "secure-key")
                {
                    UseHttps = true,
                    Endpoint = "/api/v2/data"
                };

                var messageConfig = new MessageConfig(
                    payload: "{\"action\": \"custom_action\", \"data\": {\"value\": 42}}",
                    timeoutSeconds: 15f
                );

                _messageSender4.SendMessage(connectionConfig, messageConfig);
            });
        }
    }

    private void ConfigureMessageSenders()
    {
        // Method 1: Configure using individual properties
        if (_messageSender1 != null)
        {
            _messageSender1.Connection.TargetIp = "127.0.0.1";
            _messageSender1.Connection.Port = 8080;
            _messageSender1.Connection.ApiKey = "dev-key-789";

            _messageSender1.Message.Payload = "{\"environment\": \"development\"}";

            // Subscribe to events
            _messageSender1.OnMessageSent.AddListener(OnMessageSuccess);
            _messageSender1.OnMessageFailed.AddListener(OnMessageError);
        }

        // Method 2: Configure using model objects
        if (_messageSender2 != null)
        {
            var config = new ConnectionConfig("192.168.1.200", 8080, "staging-key");
            _messageSender2.UpdateConnectionConfig(config);

            _messageSender2.OnMessageSent.AddListener(OnMessageSuccess);
            _messageSender2.OnMessageFailed.AddListener(OnMessageError);
        }

        // Method 3: Use preset configurations
        if (_messageSender3 != null && _developmentConfig != null)
        {
            _messageSender3.UpdateConnectionConfig(_developmentConfig);

            _messageSender3.OnMessageSent.AddListener(OnMessageSuccess);
            _messageSender3.OnMessageFailed.AddListener(OnMessageError);
        }

        if (_messageSender4 != null)
        {
            _messageSender4.OnMessageSent.AddListener(OnMessageSuccess);
            _messageSender4.OnMessageFailed.AddListener(OnMessageError);
        }
    }

    private void OnMessageSuccess(string response)
    {
        Debug.Log($"[Example] Message sent successfully! Response: {response}");
        // Update UI, show success notification, etc.
    }

    private void OnMessageError(string error)
    {
        Debug.LogError($"[Example] Message failed: {error}");
        // Show error dialog, retry logic, etc.
    }

    /// <summary>
    /// Example: Switch between development and production configurations
    /// </summary>
    public void SwitchToProduction()
    {
        if (_messageSender1 != null && _productionConfig != null)
        {
            _messageSender1.UpdateConnectionConfig(_productionConfig);
            Debug.Log("[Example] Switched to production configuration");
        }
    }

    /// <summary>
    /// Example: Create and use a configuration on the fly
    /// </summary>
    public void SendToCustomServer(string ip, int port, string apiKey)
    {
        if (_messageSender1 != null)
        {
            var customConfig = new ConnectionConfig(ip, port, apiKey);
            _messageSender1.SendMessage(customConfig);
        }
    }
    #endregion
}
