using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Handles loading and caching of plane configuration data
/// Loads from: 1) Downloaded config, 2) Bundled Resources, 3) Hardcoded fallback
/// </summary>
public static class ConfigLoader
{
    private static PlaneConfigRoot _cachedConfig;
    private static bool _isLoaded = false;
    
    // TODO: Replace with production config URL before deployment
    // This is a placeholder URL - should point to a stable GitHub release or CDN
    private static string _configUrl = "https://github.com/user-attachments/files/25318490/plane-config.json";
    
    // Storage paths
    private static string PersistentConfigPath
    {
        get
        {
            #if UNITY_ANDROID && !UNITY_EDITOR
                return Path.Combine(Application.persistentDataPath, "plane-config.json");
            #elif UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
                string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string il2DialsPath = Path.Combine(appDataPath, "IL2Dials");
                if (!Directory.Exists(il2DialsPath))
                    Directory.CreateDirectory(il2DialsPath);
                return Path.Combine(il2DialsPath, "plane-config.json");
            #else
                return Path.Combine(Application.persistentDataPath, "plane-config.json");
            #endif
        }
    }

    /// <summary>
    /// Get cached config or load if not loaded yet
    /// </summary>
    public static PlaneConfigRoot GetConfig()
    {
        if (!_isLoaded)
        {
            LoadConfig();
        }
        return _cachedConfig;
    }

    /// <summary>
    /// Force reload of config
    /// </summary>
    public static void ReloadConfig()
    {
        _isLoaded = false;
        LoadConfig();
    }

    /// <summary>
    /// Load config with priority: downloaded → bundled → empty fallback
    /// </summary>
    private static void LoadConfig()
    {
        string jsonText = null;
        
        // Priority 1: Try to load downloaded config from persistent storage
        if (File.Exists(PersistentConfigPath))
        {
            try
            {
                jsonText = File.ReadAllText(PersistentConfigPath);
                Debug.Log($"[ConfigLoader] Loaded config from: {PersistentConfigPath}");
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[ConfigLoader] Failed to load downloaded config: {e.Message}");
            }
        }
        
        // Priority 2: Try to load bundled config from Resources
        if (string.IsNullOrEmpty(jsonText))
        {
            TextAsset configAsset = Resources.Load<TextAsset>("plane-config");
            if (configAsset != null)
            {
                jsonText = configAsset.text;
                Debug.Log("[ConfigLoader] Loaded bundled config from Resources");
            }
            else
            {
                Debug.LogWarning("[ConfigLoader] No bundled config found in Resources");
            }
        }
        
        // Parse JSON if we have any
        if (!string.IsNullOrEmpty(jsonText))
        {
            try
            {
                _cachedConfig = ParseConfig(jsonText);
                _isLoaded = true;
                Debug.Log($"[ConfigLoader] Successfully parsed {_cachedConfig.planes.Count} planes");
            }
            catch (Exception e)
            {
                Debug.LogError($"[ConfigLoader] Failed to parse config JSON: {e.Message}");
                _cachedConfig = new PlaneConfigRoot();
                _isLoaded = true;
            }
        }
        else
        {
            // No config found, create empty
            Debug.LogWarning("[ConfigLoader] No config loaded, using empty config (will fall back to hardcoded data)");
            _cachedConfig = new PlaneConfigRoot();
            _isLoaded = true;
        }
    }

    /// <summary>
    /// Parse JSON manually since Unity's JsonUtility doesn't support dictionaries
    /// </summary>
    private static PlaneConfigRoot ParseConfig(string jsonText)
    {
        var config = new PlaneConfigRoot();
        
        // Simple JSON parsing for the planes dictionary
        // Find the "planes" section
        int planesIndex = jsonText.IndexOf("\"planes\"");
        if (planesIndex < 0)
        {
            Debug.LogError("[ConfigLoader] Invalid config format: 'planes' section not found");
            return config;
        }
        
        // Find the opening brace of planes object
        int planesStart = jsonText.IndexOf('{', planesIndex);
        if (planesStart < 0) return config;
        
        // Parse each plane entry
        int currentPos = planesStart + 1;
        while (currentPos < jsonText.Length)
        {
            // Skip whitespace
            while (currentPos < jsonText.Length && char.IsWhiteSpace(jsonText[currentPos]))
                currentPos++;
            
            if (currentPos >= jsonText.Length) break;
            
            // Check for end of planes object
            if (jsonText[currentPos] == '}') break;
            
            // Find plane name (quoted string)
            if (jsonText[currentPos] == '"')
            {
                int nameStart = currentPos + 1;
                int nameEnd = jsonText.IndexOf('"', nameStart);
                if (nameEnd < 0) break;
                
                string planeName = jsonText.Substring(nameStart, nameEnd - nameStart);
                
                // Find plane data object
                int dataStart = jsonText.IndexOf('{', nameEnd);
                if (dataStart < 0) break;
                
                int dataEnd = FindMatchingBrace(jsonText, dataStart);
                if (dataEnd < 0) break;
                
                string planeJson = jsonText.Substring(dataStart, dataEnd - dataStart + 1);
                
                try
                {
                    PlaneConfigData planeData = JsonUtility.FromJson<PlaneConfigData>(planeJson);
                    config.planes[planeName] = planeData;
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"[ConfigLoader] Failed to parse plane '{planeName}': {e.Message}");
                }
                
                currentPos = dataEnd + 1;
            }
            else
            {
                currentPos++;
            }
        }
        
        return config;
    }

    /// <summary>
    /// Find matching closing brace for an opening brace
    /// </summary>
    private static int FindMatchingBrace(string text, int openBraceIndex)
    {
        int depth = 1;
        int pos = openBraceIndex + 1;
        bool inString = false;
        
        while (pos < text.Length && depth > 0)
        {
            char c = text[pos];
            
            // Handle string literals
            if (c == '"' && (pos == 0 || text[pos - 1] != '\\'))
            {
                inString = !inString;
            }
            else if (!inString)
            {
                if (c == '{') depth++;
                else if (c == '}') depth--;
            }
            
            pos++;
        }
        
        return depth == 0 ? pos - 1 : -1;
    }

    /// <summary>
    /// Get plane attributes by name from config
    /// Returns null if plane not found in config
    /// </summary>
    public static PlaneDataFromName.PlaneAttributes GetPlaneAttributes(string planeName)
    {
        var config = GetConfig();
        
        if (config.planes.TryGetValue(planeName, out PlaneConfigData planeData))
        {
            return planeData.ToPlaneAttributes();
        }
        
        return null;
    }

    /// <summary>
    /// Get all plane names grouped by country
    /// </summary>
    public static Dictionary<Country, List<string>> GetPlanesByCountry()
    {
        var config = GetConfig();
        var planesByCountry = new Dictionary<Country, List<string>>();
        
        foreach (var kvp in config.planes)
        {
            string planeName = kvp.Key;
            PlaneConfigData planeData = kvp.Value;
            
            // Parse country
            Country country = Country.UNDEFINED;
            if (!string.IsNullOrEmpty(planeData.country))
            {
                switch (planeData.country.ToUpper())
                {
                    case "RU": country = Country.RU; break;
                    case "GER": country = Country.GER; break;
                    case "US": country = Country.US; break;
                    case "UK": country = Country.UK; break;
                    case "ITA": country = Country.ITA; break;
                    case "FR": country = Country.FR; break;
                }
            }
            
            if (!planesByCountry.ContainsKey(country))
            {
                planesByCountry[country] = new List<string>();
            }
            
            planesByCountry[country].Add(planeName);
        }
        
        return planesByCountry;
    }

    /// <summary>
    /// Check if config has any planes loaded
    /// </summary>
    public static bool HasConfigData()
    {
        return GetConfig().planes.Count > 0;
    }

    /// <summary>
    /// Get persistent config file path for external use
    /// </summary>
    public static string GetConfigPath()
    {
        return PersistentConfigPath;
    }

    /// <summary>
    /// Get config URL for downloading
    /// </summary>
    public static string GetConfigUrl()
    {
        return _configUrl;
    }

    /// <summary>
    /// Set custom config URL (useful for testing or using different sources)
    /// </summary>
    public static void SetConfigUrl(string url)
    {
        _configUrl = url;
    }
}
