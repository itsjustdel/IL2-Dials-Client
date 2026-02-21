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
    private static string _configVersion = null;
    public static event Action OnConfigReloaded;
    // Use the GitHub releases "latest" download URL so the app can fetch the current release asset
    private static string _configUrl = "https://github.com/itsjustdel/IL2-Dials-Server/releases/latest/download/plane-config.json";
    
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
        try { OnConfigReloaded?.Invoke(); } catch { }
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
            // extract version if present
            _configVersion = ExtractVersionFromJson(jsonText);
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
    /// Extract a top-level "version" field value from a JSON string.
    /// Returns null if not present.
    /// </summary>
    public static string ExtractVersionFromJson(string jsonText)
    {
        if (string.IsNullOrEmpty(jsonText)) return null;

        int idx = jsonText.IndexOf("\"version\"");
        if (idx < 0) return null;

        int colon = jsonText.IndexOf(':', idx);
        if (colon < 0) return null;

        int p = colon + 1;
        // skip whitespace
        while (p < jsonText.Length && char.IsWhiteSpace(jsonText[p])) p++;

        if (p >= jsonText.Length) return null;

        // if quoted string
        if (jsonText[p] == '"')
        {
            int start = p + 1;
            int end = jsonText.IndexOf('"', start);
            if (end > start)
                return jsonText.Substring(start, end - start);
            return null;
        }

        // unquoted token (number etc.) - read until comma or brace
        int endPos = p;
        while (endPos < jsonText.Length && jsonText[endPos] != ',' && jsonText[endPos] != '}' && !char.IsWhiteSpace(jsonText[endPos])) endPos++;
        if (endPos > p)
            return jsonText.Substring(p, endPos - p).Trim();

        return null;
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
            
            // Handle string literals with proper escape sequence checking
            if (c == '"')
            {
                // Count consecutive backslashes before the quote
                int backslashCount = 0;
                int checkPos = pos - 1;
                while (checkPos >= 0 && text[checkPos] == '\\')
                {
                    backslashCount++;
                    checkPos--;
                }
                
                // If even number of backslashes (including 0), they escape each other (e.g., \\\\ becomes \\),
                // leaving the quote unescaped, so it's a string terminator
                if (backslashCount % 2 == 0)
                {
                    inString = !inString;
                }
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
    /// Get the version string of the currently loaded config (downloaded or bundled).
    /// Returns null if no version present.
    /// </summary>
    public static string GetConfigVersion()
    {
        if (!_isLoaded) GetConfig();
        return _configVersion;
    }

    /// <summary>
    /// Get the version string from the local downloaded config file without affecting in-memory cache.
    /// Returns null if no local file or no version field present.
    /// </summary>
    public static string GetLocalConfigVersion()
    {
        try
        {
            string path = PersistentConfigPath;
            if (!File.Exists(path)) return null;
            string json = File.ReadAllText(path);
            return ExtractVersionFromJson(json);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Read the bundled Resources/plane-config.json asset and return its top-level version string if present.
    /// Returns null if no bundled asset or no version field.
    /// </summary>
    public static string GetBundledConfigVersion()
    {
        try
        {
            TextAsset ta = Resources.Load<TextAsset>("plane-config");
            if (ta == null) return null;
            return ExtractVersionFromJson(ta.text);
        }
        catch
        {
            return null;
        }
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
