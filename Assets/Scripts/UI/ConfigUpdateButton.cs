using UnityEngine;
using UnityEngine.UI;
using System.IO;

/// <summary>
/// Simple UI component for config update functionality
/// Attach this to a GameObject with a Button to enable config updates
/// </summary>
public class ConfigUpdateButton : MonoBehaviour
{
    [Header("UI References (Optional - will search if not set)")]
    public Button updateButton;
    public Text statusText;
    
    [Header("Config Update Manager")]
    public ConfigUpdateManager configUpdateManager;
    
    [Header("Settings")]
    public bool showDebugInfo = true;
    
    private bool _isInitialized = false;

    private void Start()
    {
        InitializeComponents();
    }

    private void InitializeComponents()
    {
        if (_isInitialized) return;

        // Find update button if not assigned
        if (updateButton == null)
        {
            updateButton = GetComponent<Button>();
        }

        // Find or create ConfigUpdateManager
        if (configUpdateManager == null)
        {
            // Try to find existing manager (including in DontDestroyOnLoad scene)
            // This prevents creating duplicates on scene reloads
            var existingManagers = FindObjectsOfType<ConfigUpdateManager>();
            if (existingManagers != null && existingManagers.Length > 0)
            {
                configUpdateManager = existingManagers[0];
                Debug.Log("[ConfigUpdateButton] Found existing ConfigUpdateManager");
                
                // Clean up any duplicates
                for (int i = 1; i < existingManagers.Length; i++)
                {
                    Debug.LogWarning($"[ConfigUpdateButton] Destroying duplicate ConfigUpdateManager #{i}");
                    Destroy(existingManagers[i].gameObject);
                }
            }
            else
            {
                // Create new manager if none exists
                GameObject managerObj = new GameObject("ConfigUpdateManager");
                configUpdateManager = managerObj.AddComponent<ConfigUpdateManager>();
                // Note: Making this persistent. Consider manually adding ConfigUpdateManager
                // to a persistent manager object to avoid potential duplicates on scene reloads
                DontDestroyOnLoad(managerObj);
                Debug.Log("[ConfigUpdateButton] Created persistent ConfigUpdateManager");
            }
        }

        // Set up button click handler
        if (updateButton != null)
        {
            updateButton.onClick.AddListener(OnUpdateButtonClicked);
            Debug.Log("[ConfigUpdateButton] Initialized update button");
        }
        else
        {
            Debug.LogWarning("[ConfigUpdateButton] No button found! Attach this component to a Button GameObject.");
        }

        _isInitialized = true;
    }

    private void OnUpdateButtonClicked()
    {
        if (configUpdateManager == null || configUpdateManager.IsUpdating())
        {
            UpdateStatus("Update already in progress...");
            return;
        }

        UpdateStatus("Downloading config...");
        
        // Disable button during update
        if (updateButton != null)
        {
            updateButton.interactable = false;
        }

        // Start update
        configUpdateManager.UpdateConfig(OnUpdateComplete);
    }

    private void OnUpdateComplete(bool success, string message)
    {
        // Re-enable button
        if (updateButton != null)
        {
            updateButton.interactable = true;
        }

        // Update status
        UpdateStatus(success ? $"✓ {message}" : $"✗ {message}");

        // Log result
        if (success)
        {
            Debug.Log($"[ConfigUpdateButton] {message}");
        }
        else
        {
            Debug.LogWarning($"[ConfigUpdateButton] {message}");
        }
    }

    private void UpdateStatus(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
        }
        
        if (showDebugInfo)
        {
            Debug.Log($"[ConfigUpdateButton] {message}");
        }
    }

    /// <summary>
    /// Public method that can be called from Unity button events
    /// </summary>
    public void UpdateConfigClick()
    {
        if (!_isInitialized)
        {
            InitializeComponents();
        }
        OnUpdateButtonClicked();
    }

    /// <summary>
    /// Display current config info
    /// </summary>
    public void ShowConfigInfo()
    {
        if (configUpdateManager != null)
        {
            // Show local info immediately
            string info = configUpdateManager.GetConfigInfo();
            UpdateStatus(info);

            // Then check remote metadata and update status with a human-friendly message
            configUpdateManager.CheckRemoteStatus((success, message) =>
            {
                UpdateStatus(message);
            });
        }
        else
        {
            UpdateStatus("Config manager not initialized");
        }
    }

    // Editor helper - show status in inspector
    #if UNITY_EDITOR
    private void OnValidate()
    {
        // This runs in the editor when values change
        if (Application.isPlaying && showDebugInfo)
        {
            string configPath = ConfigLoader.GetConfigPath();
            if (File.Exists(configPath))
            {
                FileInfo fileInfo = new FileInfo(configPath);
                Debug.Log($"[ConfigUpdateButton] Config exists: {fileInfo.Length} bytes, modified {fileInfo.LastWriteTime}");
            }
            else
            {
                Debug.Log($"[ConfigUpdateButton] No downloaded config at: {configPath}");
            }
        }
    }
    #endif

        /// <summary>
        /// Clear status text on all ConfigUpdateButton instances.
        /// Useful for hiding messages when the menu is closed or other menu buttons are pressed.
        /// </summary>
        public static void ClearAllStatuses()
        {
            var all = FindObjectsOfType<ConfigUpdateButton>();
            foreach (var c in all)
            {
                if (c != null && c.statusText != null)
                    c.statusText.text = string.Empty;
            }
        }

    }
