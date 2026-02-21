using System;
using UnityEngine;

/// <summary>
/// Periodically checks the local `plane-config.json` version and reloads the in-memory config
/// when a change is detected. This enables runtime updates without restarting the client.
/// </summary>
public class ConfigAutoReloader : MonoBehaviour
{
    [Tooltip("Check interval in seconds")] public float checkInterval = 5.0f;
    [Tooltip("Enable auto-reload at runtime")] public bool enabledAtStart = true;

    private string _currentVersion = null;
    private float _timer = 0f;

    void Awake()
    {
        // Initialize current version from loaded config (or bundled/local)
        _currentVersion = ConfigLoader.GetConfigVersion() ?? ConfigLoader.GetLocalConfigVersion() ?? ConfigLoader.GetBundledConfigVersion();

        if (enabledAtStart)
        {
            _timer = checkInterval;
        }

        // Optional: subscribe to reload events for logging or extra UI hooks
        ConfigLoader.OnConfigReloaded += OnConfigReloadedHandler;
    }

    void OnDestroy()
    {
        ConfigLoader.OnConfigReloaded -= OnConfigReloadedHandler;
    }

    void Update()
    {
        if (!enabledAtStart) return;

        _timer -= Time.unscaledDeltaTime;
        if (_timer > 0f) return;
        _timer = checkInterval;

        try
        {
            string local = ConfigLoader.GetLocalConfigVersion();
            string bundled = ConfigLoader.GetBundledConfigVersion();
            string inMemory = ConfigLoader.GetConfigVersion();

            // Prefer downloaded local version when present, otherwise bundled; compare to in-memory
            string newest = local ?? bundled;

            if (!string.IsNullOrEmpty(newest) && newest != inMemory && newest != _currentVersion)
            {
                Debug.Log($"[ConfigAutoReloader] Detected config version change (was: {_currentVersion ?? "<none>"}, now: {newest}). Reloading...");
                ConfigLoader.ReloadConfig();
                PlaneLists.Reload();
                _currentVersion = newest;
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"[ConfigAutoReloader] Error checking config version: {ex.Message}");
        }
    }

    private void OnConfigReloadedHandler()
    {
        Debug.Log("[ConfigAutoReloader] Config reloaded (event)");
    }
}
