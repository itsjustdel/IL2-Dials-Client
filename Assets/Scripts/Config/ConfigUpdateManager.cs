using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Manages downloading and updating plane configuration from remote URL
/// </summary>
public class ConfigUpdateManager : MonoBehaviour
{
    public delegate void UpdateCallback(bool success, string message);
    
    [Header("Settings")]
    [Tooltip("Network timeout in seconds for config download")]
    public int networkTimeoutSeconds = 30;
    
    private bool _isUpdating = false;

    /// <summary>
    /// Download and update the plane configuration
    /// </summary>
    public void UpdateConfig(UpdateCallback callback = null)
    {
        if (_isUpdating)
        {
            callback?.Invoke(false, "Update already in progress");
            return;
        }

        StartCoroutine(UpdateConfigCoroutine(callback));
    }

    private IEnumerator UpdateConfigCoroutine(UpdateCallback callback)
    {
        _isUpdating = true;
        
        string url = ConfigLoader.GetConfigUrl();
        Debug.Log($"[ConfigUpdateManager] Downloading config from: {url}");

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            // Set timeout (configurable)
            request.timeout = networkTimeoutSeconds;

            // Send request
            yield return request.SendWebRequest();

            // Check for errors
            #if UNITY_2020_1_OR_NEWER
            if (request.result != UnityWebRequest.Result.Success)
            #else
            if (request.isNetworkError || request.isHttpError)
            #endif
            {
                string errorMsg = $"Failed to download config: {request.error}";
                Debug.LogError($"[ConfigUpdateManager] {errorMsg}");
                callback?.Invoke(false, errorMsg);
                _isUpdating = false;
                yield break;
            }

            // Get downloaded data
            string jsonData = request.downloadHandler.text;

            // Validate JSON
            if (string.IsNullOrEmpty(jsonData))
            {
                string errorMsg = "Downloaded config is empty";
                Debug.LogError($"[ConfigUpdateManager] {errorMsg}");
                callback?.Invoke(false, errorMsg);
                _isUpdating = false;
                yield break;
            }

            // Before saving, check versions (if present) to avoid pointless overwrite
            try
            {
                string remoteVersion = ConfigLoader.ExtractVersionFromJson(jsonData);
                string localVersion = ConfigLoader.GetLocalConfigVersion();

                if (!string.IsNullOrEmpty(remoteVersion) && !string.IsNullOrEmpty(localVersion) && remoteVersion == localVersion)
                {
                    string msg = $"Already up-to-date (version {localVersion})";
                    Debug.Log($"[ConfigUpdateManager] {msg}");
                    callback?.Invoke(true, msg);
                    _isUpdating = false;
                    yield break;
                }

                string configPath = ConfigLoader.GetConfigPath();

                // Ensure directory exists
                string directory = Path.GetDirectoryName(configPath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // Write file
                File.WriteAllText(configPath, jsonData);

                Debug.Log($"[ConfigUpdateManager] Config saved to: {configPath}");

                // Reload config
                ConfigLoader.ReloadConfig();
                PlaneLists.Reload();

                string successMsg = "Config updated successfully";
                Debug.Log($"[ConfigUpdateManager] {successMsg}");
                callback?.Invoke(true, successMsg);
            }
            catch (Exception e)
            {
                string errorMsg = $"Failed to save config: {e.Message}";
                Debug.LogError($"[ConfigUpdateManager] {errorMsg}");
                callback?.Invoke(false, errorMsg);
            }
        }

        _isUpdating = false;
    }

    /// <summary>
    /// Check remote config metadata (HEAD request) and compare to local file to provide a human-friendly status.
    /// Calls callback with (success, message).
    /// </summary>
    public void CheckRemoteStatus(Action<bool, string> callback = null)
    {
        StartCoroutine(CheckRemoteStatusCoroutine(callback));
    }

    private IEnumerator CheckRemoteStatusCoroutine(Action<bool, string> callback)
    {
        string url = ConfigLoader.GetConfigUrl();

        // Try to GET remote JSON and parse its version field
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.timeout = networkTimeoutSeconds;
            yield return request.SendWebRequest();

            #if UNITY_2020_1_OR_NEWER
            if (request.result != UnityWebRequest.Result.Success)
            #else
            if (request.isNetworkError || request.isHttpError)
            #endif
            {
                callback?.Invoke(false, $"Failed to contact server: {request.error}");
                yield break;
            }

            string remoteJson = request.downloadHandler.text;
            string remoteVersion = ConfigLoader.ExtractVersionFromJson(remoteJson);
            string localVersion = ConfigLoader.GetLocalConfigVersion();

            // If no local file exists, report remote version (if any)
            string configPath = ConfigLoader.GetConfigPath();
            if (!File.Exists(configPath))
            {
                if (!string.IsNullOrEmpty(remoteVersion))
                    callback?.Invoke(true, $"No downloaded config present (remote version: {remoteVersion})");
                else
                    callback?.Invoke(true, "No downloaded config present");
                yield break;
            }

            // If both have versions, compare
            if (!string.IsNullOrEmpty(remoteVersion) && !string.IsNullOrEmpty(localVersion))
            {
                if (remoteVersion == localVersion)
                {
                    callback?.Invoke(true, $"Config is up-to-date (version {localVersion})");
                }
                else
                {
                    callback?.Invoke(true, $"Remote version {remoteVersion} differs from local {localVersion}");
                }
                yield break;
            }

            // If remote has version but local doesn't
            if (!string.IsNullOrEmpty(remoteVersion) && string.IsNullOrEmpty(localVersion))
            {
                callback?.Invoke(true, $"Remote version {remoteVersion} available (local has no version)");
                yield break;
            }

            // No version info available: fallback to headers / size comparison
            string remoteLastModified = request.GetResponseHeader("Last-Modified");
            string remoteLength = request.GetResponseHeader("Content-Length");

            FileInfo fi = new FileInfo(configPath);

            if (!string.IsNullOrEmpty(remoteLastModified) && DateTime.TryParse(remoteLastModified, out DateTime remoteDt))
            {
                DateTime localDt = fi.LastWriteTime.ToUniversalTime();
                if (localDt >= remoteDt.ToUniversalTime())
                {
                    callback?.Invoke(true, "Config is up-to-date (local newer or equal).");
                }
                else
                {
                    callback?.Invoke(true, $"Remote config newer (remote: {remoteDt:u}, local: {fi.LastWriteTime:u})");
                }
                yield break;
            }

            if (!string.IsNullOrEmpty(remoteLength) && long.TryParse(remoteLength, out long remoteLen))
            {
                if (fi.Length == remoteLen)
                    callback?.Invoke(true, "Config appears up-to-date (matching size).");
                else
                    callback?.Invoke(true, $"Remote config size differs (remote: {remoteLen} bytes, local: {fi.Length} bytes)");
                yield break;
            }

            callback?.Invoke(true, $"Local: {fi.Length} bytes, modified {fi.LastWriteTime}");
        }
    }

    /// <summary>
    /// Check if update is currently in progress
    /// </summary>
    public bool IsUpdating()
    {
        return _isUpdating;
    }

    /// <summary>
    /// Get current config info
    /// </summary>
    public string GetConfigInfo()
    {
        string configPath = ConfigLoader.GetConfigPath();
        
        if (File.Exists(configPath))
        {
            FileInfo fileInfo = new FileInfo(configPath);
            return $"Config: {fileInfo.Length} bytes\nLast modified: {fileInfo.LastWriteTime}";
        }
        
        return "No downloaded config found";
    }
}
