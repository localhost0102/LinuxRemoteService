using System;
using UnityEngine;

namespace LinuxLoginService.Models
{
    /// <summary>
    /// Configuration model for message request settings.
    /// Contains payload, timeout, and HTTP method settings.
    /// </summary>
    [Serializable]
    public class MessageConfig
    {
        #region Serialized Fields
        [Tooltip("Message payload to send (JSON format)")]
        [TextArea(3, 10)]
        [SerializeField] private string _payload = "{\"message\": \"Hello from Unity\"}";

        [Tooltip("Request timeout in seconds")]
        [SerializeField] private float _timeoutSeconds = 10f;

        [Tooltip("Content type for the request")]
        [SerializeField] private string _contentType = "application/json";

        [Tooltip("HTTP method to use")]
        [SerializeField] private HttpMethod _httpMethod = HttpMethod.POST;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the message payload
        /// </summary>
        public string Payload
        {
            get => _payload;
            set => _payload = value;
        }

        /// <summary>
        /// Gets or sets the request timeout in seconds
        /// </summary>
        public float TimeoutSeconds
        {
            get => _timeoutSeconds;
            set
            {
                if (value <= 0)
                {
                    Debug.LogWarning($"[MessageConfig] Timeout must be positive. Value not changed.");
                    return;
                }
                _timeoutSeconds = value;
            }
        }

        /// <summary>
        /// Gets or sets the content type
        /// </summary>
        public string ContentType
        {
            get => _contentType;
            set => _contentType = value;
        }

        /// <summary>
        /// Gets or sets the HTTP method
        /// </summary>
        public HttpMethod Method
        {
            get => _httpMethod;
            set => _httpMethod = value;
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new MessageConfig with default values
        /// </summary>
        public MessageConfig()
        {
        }

        /// <summary>
        /// Creates a new MessageConfig with specified payload
        /// </summary>
        /// <param name="payload">Message payload</param>
        public MessageConfig(string payload)
        {
            _payload = payload;
        }

        /// <summary>
        /// Creates a new MessageConfig with all parameters
        /// </summary>
        /// <param name="payload">Message payload</param>
        /// <param name="timeoutSeconds">Request timeout</param>
        /// <param name="contentType">Content type</param>
        /// <param name="httpMethod">HTTP method</param>
        public MessageConfig(string payload, float timeoutSeconds, string contentType = "application/json", HttpMethod httpMethod = HttpMethod.POST)
        {
            _payload = payload;
            TimeoutSeconds = timeoutSeconds; // Use property for validation
            _contentType = contentType;
            _httpMethod = httpMethod;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Validates the configuration
        /// </summary>
        /// <returns>True if configuration is valid</returns>
        public bool IsValid()
        {
            if (string.IsNullOrWhiteSpace(_payload))
            {
                Debug.LogWarning("[MessageConfig] Payload is empty");
                return false;
            }

            if (_timeoutSeconds <= 0)
            {
                Debug.LogError($"[MessageConfig] Timeout {_timeoutSeconds} must be positive");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Creates a deep copy of this configuration
        /// </summary>
        /// <returns>New MessageConfig instance with same values</returns>
        public MessageConfig Clone()
        {
            return new MessageConfig(_payload, _timeoutSeconds, _contentType, _httpMethod);
        }

        /// <summary>
        /// Copies values from another configuration
        /// </summary>
        /// <param name="other">Configuration to copy from</param>
        public void CopyFrom(MessageConfig other)
        {
            if (other == null)
            {
                Debug.LogWarning("[MessageConfig] Cannot copy from null configuration");
                return;
            }

            _payload = other._payload;
            _timeoutSeconds = other._timeoutSeconds;
            _contentType = other._contentType;
            _httpMethod = other._httpMethod;
        }
        #endregion

        #region Overrides
        public override string ToString()
        {
            return $"MessageConfig[Method: {_httpMethod}, Timeout: {_timeoutSeconds}s, ContentType: {_contentType}, PayloadSize: {_payload?.Length ?? 0} chars]";
        }
        #endregion

        #region Nested Types
        /// <summary>
        /// Supported HTTP methods
        /// </summary>
        public enum HttpMethod
        {
            GET,
            POST,
            PUT,
            DELETE,
            PATCH
        }
        #endregion
    }
}
