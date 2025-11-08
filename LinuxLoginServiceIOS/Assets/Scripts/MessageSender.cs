using System;
using System.Collections;
using System.Net.Http;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using LinuxLoginService.Models;

namespace LinuxLoginService.Services
{
    /// <summary>
    /// Configurable HTTP message sender that can be attached to UI buttons.
    /// Each instance can have different connection and message configurations.
    /// Uses model classes for better OOP design and maintainability.
    /// </summary>
    public class MessageSender : MonoBehaviour
    {
        #region Serialized Fields
        [Header("Configuration")]
        [Tooltip("Connection configuration (IP, port, API key, endpoint)")]
        [SerializeField] private ConnectionConfig _connectionConfig = new ConnectionConfig();

        [Tooltip("Message configuration (payload, timeout, content type)")]
        [SerializeField] private MessageConfig _messageConfig = new MessageConfig();

        [Header("Events")]
        [Tooltip("Event triggered when message is sent successfully")]
        public UnityEvent<string> OnMessageSent;

        [Tooltip("Event triggered when message sending fails")]
        public UnityEvent<string> OnMessageFailed;
        #endregion

        #region Private Fields
        private static readonly HttpClient _httpClient = new HttpClient();
        private bool _isSending = false;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the connection configuration
        /// </summary>
        public ConnectionConfig Connection => _connectionConfig;

        /// <summary>
        /// Gets the message configuration
        /// </summary>
        public MessageConfig Message => _messageConfig;

        /// <summary>
        /// Gets whether a message is currently being sent
        /// </summary>
        public bool IsSending => _isSending;

        /// <summary>
        /// Gets the full URL constructed from current settings
        /// </summary>
        public string FullUrl => _connectionConfig.ConstructUrl();
        #endregion

        #region Unity Lifecycle
        private void OnValidate()
        {
            if (_connectionConfig != null && !_connectionConfig.IsValid())
            {
                Debug.LogWarning("[MessageSender] Connection configuration is invalid");
            }

            if (_messageConfig != null && !_messageConfig.IsValid())
            {
                Debug.LogWarning("[MessageSender] Message configuration is invalid");
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Sends the configured message to the target server.
        /// This method can be called directly from UI Button onClick events.
        /// </summary>
        public void SendMessage()
        {
            if (_isSending)
            {
                Debug.LogWarning("[MessageSender] Already sending a message. Please wait.");
                return;
            }

            if (!_connectionConfig.IsValid() || !_messageConfig.IsValid())
            {
                Debug.LogError("[MessageSender] Configuration is invalid. Cannot send message.");
                OnMessageFailed?.Invoke("Invalid configuration");
                return;
            }

            StartCoroutine(SendMessageCoroutine(_connectionConfig, _messageConfig));
        }

        /// <summary>
        /// Sends a message with custom connection configuration.
        /// </summary>
        /// <param name="connectionConfig">Custom connection configuration</param>
        public void SendMessage(ConnectionConfig connectionConfig)
        {
            if (_isSending)
            {
                Debug.LogWarning("[MessageSender] Already sending a message. Please wait.");
                return;
            }

            if (connectionConfig == null || !connectionConfig.IsValid())
            {
                Debug.LogError("[MessageSender] Invalid connection configuration");
                OnMessageFailed?.Invoke("Invalid connection configuration");
                return;
            }

            StartCoroutine(SendMessageCoroutine(connectionConfig, _messageConfig));
        }

        /// <summary>
        /// Sends a message with custom connection and message configurations.
        /// </summary>
        /// <param name="connectionConfig">Custom connection configuration</param>
        /// <param name="messageConfig">Custom message configuration</param>
        public void SendMessage(ConnectionConfig connectionConfig, MessageConfig messageConfig)
        {
            if (_isSending)
            {
                Debug.LogWarning("[MessageSender] Already sending a message. Please wait.");
                return;
            }

            if (connectionConfig == null || !connectionConfig.IsValid())
            {
                Debug.LogError("[MessageSender] Invalid connection configuration");
                OnMessageFailed?.Invoke("Invalid connection configuration");
                return;
            }

            if (messageConfig == null || !messageConfig.IsValid())
            {
                Debug.LogError("[MessageSender] Invalid message configuration");
                OnMessageFailed?.Invoke("Invalid message configuration");
                return;
            }

            StartCoroutine(SendMessageCoroutine(connectionConfig, messageConfig));
        }

        /// <summary>
        /// Sends a message with custom parameters (convenience method).
        /// </summary>
        /// <param name="targetIp">Target IP address</param>
        /// <param name="port">Target port</param>
        /// <param name="apiKey">API key for authentication</param>
        public void SendMessageWithParams(string targetIp, int port, string apiKey)
        {
            var customConfig = new ConnectionConfig(targetIp, port, apiKey)
            {
                UseHttps = _connectionConfig.UseHttps,
                Endpoint = _connectionConfig.Endpoint
            };

            SendMessage(customConfig);
        }

        /// <summary>
        /// Sends a custom message payload with the configured connection settings.
        /// </summary>
        /// <param name="customPayload">Custom JSON payload to send</param>
        public void SendCustomMessage(string customPayload)
        {
            var customMessage = new MessageConfig(customPayload)
            {
                TimeoutSeconds = _messageConfig.TimeoutSeconds,
                ContentType = _messageConfig.ContentType,
                Method = _messageConfig.Method
            };

            SendMessage(_connectionConfig, customMessage);
        }

        /// <summary>
        /// Updates the connection configuration.
        /// </summary>
        /// <param name="connectionConfig">New connection configuration</param>
        public void UpdateConnectionConfig(ConnectionConfig connectionConfig)
        {
            if (connectionConfig == null)
            {
                Debug.LogWarning("[MessageSender] Cannot update with null configuration");
                return;
            }

            _connectionConfig.CopyFrom(connectionConfig);
            Debug.Log($"[MessageSender] Updated connection config: {_connectionConfig}");
        }

        /// <summary>
        /// Updates connection parameters at runtime.
        /// </summary>
        /// <param name="targetIp">New target IP</param>
        /// <param name="port">New port</param>
        /// <param name="apiKey">New API key</param>
        public void UpdateConnectionParams(string targetIp, int port, string apiKey)
        {
            _connectionConfig.TargetIp = targetIp;
            _connectionConfig.Port = port;
            _connectionConfig.ApiKey = apiKey;

            Debug.Log($"[MessageSender] Updated connection params: {FullUrl}");
        }

        /// <summary>
        /// Updates the message configuration.
        /// </summary>
        /// <param name="messageConfig">New message configuration</param>
        public void UpdateMessageConfig(MessageConfig messageConfig)
        {
            if (messageConfig == null)
            {
                Debug.LogWarning("[MessageSender] Cannot update with null configuration");
                return;
            }

            _messageConfig.CopyFrom(messageConfig);
            Debug.Log($"[MessageSender] Updated message config: {_messageConfig}");
        }
        #endregion

        #region Private Methods
        private IEnumerator SendMessageCoroutine(ConnectionConfig connectionConfig, MessageConfig messageConfig)
        {
            _isSending = true;
            string url = connectionConfig.ConstructUrl();

            Debug.Log($"[MessageSender] Sending message to: {url}");
            Debug.Log($"[MessageSender] Connection: {connectionConfig}");
            Debug.Log($"[MessageSender] Message: {messageConfig}");

            // Create HTTP request with the appropriate method
            HttpRequestMessage request = CreateHttpRequest(connectionConfig, messageConfig);

            // Set timeout
            _httpClient.Timeout = TimeSpan.FromSeconds(messageConfig.TimeoutSeconds);

            // Send request in a separate task
            var sendTask = _httpClient.SendAsync(request);

            // Wait for completion or timeout
            float elapsedTime = 0f;
            while (!sendTask.IsCompleted && elapsedTime < messageConfig.TimeoutSeconds)
            {
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Handle response
            if (sendTask.IsCompleted)
            {
                try
                {
                    HttpResponseMessage response = sendTask.Result;
                    string responseBody = response.Content.ReadAsStringAsync().Result;

                    if (response.IsSuccessStatusCode)
                    {
                        Debug.Log($"[MessageSender] Success! Status: {response.StatusCode}, Response: {responseBody}");
                        OnMessageSent?.Invoke(responseBody);
                    }
                    else
                    {
                        string errorMsg = $"HTTP {response.StatusCode}: {responseBody}";
                        Debug.LogError($"[MessageSender] Failed: {errorMsg}");
                        OnMessageFailed?.Invoke(errorMsg);
                    }
                }
                catch (Exception ex)
                {
                    string errorMsg = $"Exception: {ex.Message}";
                    Debug.LogError($"[MessageSender] Error: {errorMsg}");
                    OnMessageFailed?.Invoke(errorMsg);
                }
            }
            else
            {
                string errorMsg = "Request timeout";
                Debug.LogError($"[MessageSender] {errorMsg}");
                OnMessageFailed?.Invoke(errorMsg);

                // Cancel the task
                sendTask.Dispose();
            }

            _isSending = false;
        }

        private HttpRequestMessage CreateHttpRequest(ConnectionConfig connectionConfig, MessageConfig messageConfig)
        {
            string url = connectionConfig.ConstructUrl();

            // Convert MessageConfig.HttpMethod enum to System.Net.Http.HttpMethod
            System.Net.Http.HttpMethod method = messageConfig.Method switch
            {
                MessageConfig.HttpMethod.GET => System.Net.Http.HttpMethod.Get,
                MessageConfig.HttpMethod.POST => System.Net.Http.HttpMethod.Post,
                MessageConfig.HttpMethod.PUT => System.Net.Http.HttpMethod.Put,
                MessageConfig.HttpMethod.DELETE => System.Net.Http.HttpMethod.Delete,
                MessageConfig.HttpMethod.PATCH => new System.Net.Http.HttpMethod("PATCH"),
                _ => System.Net.Http.HttpMethod.Post
            };

            HttpRequestMessage request = new HttpRequestMessage(method, url);

            // Add API key header
            if (!string.IsNullOrEmpty(connectionConfig.ApiKey))
            {
                request.Headers.Add("x-api-key", connectionConfig.ApiKey);
            }

            // Add content for methods that support it
            if (messageConfig.Method != MessageConfig.HttpMethod.GET &&
                messageConfig.Method != MessageConfig.HttpMethod.DELETE)
            {
                request.Content = new StringContent(
                    messageConfig.Payload,
                    Encoding.UTF8,
                    messageConfig.ContentType
                );
            }

            return request;
        }
        #endregion
    }
}
