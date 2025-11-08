using System;
using UnityEngine;

namespace LinuxLoginService.Models
{
    /// <summary>
    /// Configuration model for HTTP connection parameters.
    /// Contains target IP, port, API key, and protocol settings.
    /// </summary>
    [Serializable]
    public class ConnectionConfig
    {
        #region Serialized Fields
        [Tooltip("Target IP address for the HTTP request")]
        [SerializeField] private string _targetUrl = "127.0.0.1";

        [Tooltip("Target port number")]
        [SerializeField] private int _port = 8080;

        [Tooltip("API key for authentication (x-api-key header)")]
        [SerializeField] private string _apiKey = "";

        [Tooltip("Use HTTPS instead of HTTP")]
        [SerializeField] private bool _useHttps = false;

        [Tooltip("Request endpoint path (e.g., /api/message)")]
        [SerializeField] private string _endpoint = "/api/message";
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the target IP address
        /// </summary>
        public string TargetIp
        {
            get => _targetUrl;
            set => _targetUrl = value;
        }

        /// <summary>
        /// Gets or sets the target port
        /// </summary>
        public int Port
        {
            get => _port;
            set
            {
                if (value < 1 || value > 65535)
                {
                    Debug.LogWarning($"[ConnectionConfig] Port {value} is out of valid range (1-65535). Value not changed.");
                    return;
                }
                _port = value;
            }
        }

        /// <summary>
        /// Gets or sets the API key
        /// </summary>
        public string ApiKey
        {
            get => _apiKey;
            set => _apiKey = value;
        }

        /// <summary>
        /// Gets or sets whether to use HTTPS
        /// </summary>
        public bool UseHttps
        {
            get => _useHttps;
            set => _useHttps = value;
        }

        /// <summary>
        /// Gets or sets the endpoint path
        /// </summary>
        public string Endpoint
        {
            get => _endpoint;
            set => _endpoint = value;
        }

        /// <summary>
        /// Gets the protocol (http or https)
        /// </summary>
        public string Protocol => _useHttps ? "https" : "http";
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new ConnectionConfig with default values
        /// </summary>
        public ConnectionConfig()
        {
        }

        /// <summary>
        /// Creates a new ConnectionConfig with specified parameters
        /// </summary>
        /// <param name="targetUrl">Target IP address</param>
        /// <param name="port">Target port</param>
        /// <param name="apiKey">API key for authentication</param>
        public ConnectionConfig(string targetUrl, int port, string apiKey)
        {
            _targetUrl = targetUrl;
            Port = port; // Use property for validation
            _apiKey = apiKey;
        }

        /// <summary>
        /// Creates a new ConnectionConfig with all parameters
        /// </summary>
        /// <param name="targetUrl">Target IP address</param>
        /// <param name="port">Target port</param>
        /// <param name="apiKey">API key for authentication</param>
        /// <param name="useHttps">Use HTTPS protocol</param>
        /// <param name="endpoint">API endpoint path</param>
        public ConnectionConfig(string targetUrl, int port, string apiKey, bool useHttps, string endpoint)
        {
            _targetUrl = targetUrl;
            Port = port; // Use property for validation
            _apiKey = apiKey;
            _useHttps = useHttps;
            _endpoint = endpoint;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Constructs the full URL from the configuration
        /// </summary>
        /// <returns>Full URL string</returns>
        public string ConstructUrl()
        {
            string endpoint = _endpoint.StartsWith("/") ? _endpoint : "/" + _endpoint;
            return $"{Protocol}://{_targetUrl}:{_port}{endpoint}";
        }

        /// <summary>
        /// Validates the configuration
        /// </summary>
        /// <returns>True if configuration is valid</returns>
        public bool IsValid()
        {
            if (string.IsNullOrWhiteSpace(_targetUrl))
            {
                Debug.LogError("[ConnectionConfig] Target IP is empty");
                return false;
            }

            if (_port < 1 || _port > 65535)
            {
                Debug.LogError($"[ConnectionConfig] Port {_port} is out of valid range");
                return false;
            }

            if (string.IsNullOrWhiteSpace(_endpoint))
            {
                Debug.LogError("[ConnectionConfig] Endpoint is empty");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Creates a deep copy of this configuration
        /// </summary>
        /// <returns>New ConnectionConfig instance with same values</returns>
        public ConnectionConfig Clone()
        {
            return new ConnectionConfig(_targetUrl, _port, _apiKey, _useHttps, _endpoint);
        }

        /// <summary>
        /// Copies values from another configuration
        /// </summary>
        /// <param name="other">Configuration to copy from</param>
        public void CopyFrom(ConnectionConfig other)
        {
            if (other == null)
            {
                Debug.LogWarning("[ConnectionConfig] Cannot copy from null configuration");
                return;
            }

            _targetUrl = other._targetUrl;
            _port = other._port;
            _apiKey = other._apiKey;
            _useHttps = other._useHttps;
            _endpoint = other._endpoint;
        }
        #endregion

        #region Overrides
        public override string ToString()
        {
            return $"ConnectionConfig[{Protocol}://{_targetUrl}:{_port}{_endpoint}, ApiKey: {(_apiKey.Length > 0 ? "***" : "none")}]";
        }
        #endregion
    }
}
