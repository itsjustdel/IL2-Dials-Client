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

            // Save to persistent storage
            try
            {
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
                
                string successMsg = $"Config updated successfully ({jsonData.Length} bytes)";
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
