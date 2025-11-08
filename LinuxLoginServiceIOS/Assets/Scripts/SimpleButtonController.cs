using UnityEngine;

/// <summary>
/// Simple button controller for testing UI button functionality.
/// Logs button clicks to the Unity console.
/// </summary>
public class SimpleButtonController : MonoBehaviour
{
    #region Serialized Fields
    [Header("Debug Settings")]
    [Tooltip("Prefix to add to console messages")]
    [SerializeField] private string _logPrefix = "[Button]";

    [Tooltip("Include timestamp in console messages")]
    [SerializeField] private bool _includeTimestamp = true;
    #endregion

    #region Private Fields
    private int _clickCount = 0;
    #endregion

    #region Unity Lifecycle
    private void Start()
    {
        Debug.Log($"{_logPrefix} SimpleButtonController initialized and ready.");
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Called when the button is clicked. Logs to console.
    /// This method can be connected to UI Button OnClick events in the Inspector.
    /// </summary>
    public void OnButtonClick()
    {
        _clickCount++;

        string timestamp = _includeTimestamp ? System.DateTime.Now.ToString("HH:mm:ss.fff") : "";
        string message = _includeTimestamp
            ? $"{_logPrefix} Button clicked! [Click #{_clickCount}] at {timestamp}"
            : $"{_logPrefix} Button clicked! [Click #{_clickCount}]";

        Debug.Log(message);
    }

    /// <summary>
    /// Called when a named button is clicked. Logs to console with button name.
    /// </summary>
    /// <param name="buttonName">Name of the button that was clicked</param>
    public void OnNamedButtonClick(string buttonName)
    {
        _clickCount++;

        string timestamp = _includeTimestamp ? System.DateTime.Now.ToString("HH:mm:ss.fff") : "";
        string message = _includeTimestamp
            ? $"{_logPrefix} Button '{buttonName}' clicked! [Click #{_clickCount}] at {timestamp}"
            : $"{_logPrefix} Button '{buttonName}' clicked! [Click #{_clickCount}]";

        Debug.Log(message);
    }

    /// <summary>
    /// Resets the click counter.
    /// </summary>
    public void ResetClickCount()
    {
        _clickCount = 0;
        Debug.Log($"{_logPrefix} Click counter reset.");
    }

    /// <summary>
    /// Logs a custom message to console.
    /// </summary>
    /// <param name="customMessage">Message to log</param>
    public void LogCustomMessage(string customMessage)
    {
        string timestamp = _includeTimestamp ? System.DateTime.Now.ToString("HH:mm:ss.fff") : "";
        string message = _includeTimestamp
            ? $"{_logPrefix} {customMessage} at {timestamp}"
            : $"{_logPrefix} {customMessage}";

        Debug.Log(message);
    }
    #endregion
}
