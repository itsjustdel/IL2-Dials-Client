# Config System Integration Guide

This document explains how to use the new dynamic config loading system for IL-2 Dials Client.

## Overview

The config system replaces hardcoded plane data with dynamic loading from JSON files. This allows:
- Easy updates to plane data without code changes
- Client and server using the same plane data source
- Offline functionality with bundled config
- Online updates via download

## Components

### 1. ConfigLoader.cs
Static class that handles loading and caching plane configuration:
- Loads from downloaded config (highest priority)
- Falls back to bundled Resources/plane-config.json
- Falls back to hardcoded data (backward compatibility)

### 2. PlaneConfig.cs
Data models for plane configuration JSON structure.

### 3. ConfigUpdateManager.cs
MonoBehaviour that handles downloading config updates:
- Downloads from configurable URL
- Saves to persistent storage
- Handles errors and timeouts
- Provides callback for UI updates

### 4. ConfigUpdateButton.cs
Optional UI component for easy integration:
- Can be attached to any Button GameObject
- Handles download progress and status
- Shows user feedback

## Usage

### For Developers

#### Using ConfigLoader directly:
```csharp
// Get plane attributes
var attributes = ConfigLoader.GetPlaneAttributes("Bf 109 F-4");

// Check if config is loaded
if (ConfigLoader.HasConfigData()) {
    // Use config data
}

// Get planes grouped by country
var planesByCountry = ConfigLoader.GetPlanesByCountry();
```

#### Using updated PlaneLists:
```csharp
// These now automatically load from config with fallback to hardcoded
List<string> germanPlanes = PlaneLists.GerPlanes;
List<string> russianPlanes = PlaneLists.RuPlanes;

// Force reload after config update
PlaneLists.Reload();
```

#### Using PlaneDataFromName:
```csharp
// No changes needed - it automatically tries config first, then hardcoded
var attributes = PlaneDataFromName.AttributesFromName(planeName);
```

### For Unity Editor Users

#### Adding Update Button to UI:

1. **Simple Method (Using Unity Events):**
   - Add a Button to your scene
   - Create an empty GameObject named "ConfigUpdateManager"
   - Add `ConfigUpdateManager` component to it
   - Create another GameObject and add `ConfigUpdateButton` component
   - In Button's OnClick event, drag the ConfigUpdateButton GameObject
   - Select `ConfigUpdateButton.UpdateConfigClick()` function

2. **Manual Method:**
   - Add `ConfigUpdateManager` component to any GameObject (recommended: make it persistent)
   - Add `ConfigUpdateButton` component to a Button GameObject
   - Link references in inspector if needed

#### Testing Config Loading:

1. **Test with bundled config:**
   - Config is automatically loaded from `Assets/Resources/plane-config.json`
   - Should see console message: "[ConfigLoader] Loaded bundled config from Resources"

2. **Test offline fallback:**
   - Remove or rename `Assets/Resources/plane-config.json`
   - Should see console message: "[PlaneLists] Initialized from fallback hardcoded lists"

3. **Test download:**
   - Click update button in game
   - Check persistent path:
     - Windows: `%APPDATA%/IL2Dials/plane-config.json`
     - Android: `Application.persistentDataPath/plane-config.json`

## Configuration

### Changing Config URL:

In ConfigLoader.cs, modify:
```csharp
private static string _configUrl = "https://your-url-here/plane-config.json";
```

Or dynamically:
```csharp
ConfigLoader.SetConfigUrl("https://new-url/plane-config.json");
```

### Config File Format:

See `Assets/Resources/plane-config.json` for the full structure. Example:
```json
{
  "planes": {
    "LaGG-3 ser.29": {
      "country": "RU",
      "altimeter": true,
      "engines": 1,
      ...
    }
  }
}
```

## Storage Paths

### Windows:
- Downloaded config: `%APPDATA%/IL2Dials/plane-config.json`
- Bundled config: `Resources/plane-config.json` (read-only)

### Android:
- Downloaded config: `Application.persistentDataPath/plane-config.json`
- Bundled config: Embedded in APK (read-only)

## Backward Compatibility

The system maintains full backward compatibility:
1. If no config is available, hardcoded data is used
2. All existing code continues to work without changes
3. PlaneDataFromName still has all hardcoded switch cases as fallback
4. PlaneLists still has all hardcoded lists as fallback

## Testing

### Manual Testing Steps:

1. **Start without downloaded config:**
   ```
   - Delete %APPDATA%/IL2Dials/plane-config.json
   - Launch game
   - Should load from bundled Resources/plane-config.json
   - Check console for: "[ConfigLoader] Loaded bundled config from Resources"
   ```

2. **Test update download:**
   ```
   - Click "Update Config" button
   - Should see download progress
   - Check file created at persistent path
   - Next launch should use downloaded config
   ```

3. **Test offline fallback:**
   ```
   - Remove Resources/plane-config.json from project
   - Delete downloaded config
   - Launch game
   - Should fall back to hardcoded data
   - Check console for: "[PlaneLists] Initialized from fallback hardcoded lists"
   ```

## Troubleshooting

### Config not loading:
- Check console for error messages
- Verify plane-config.json is in `Assets/Resources/` folder
- Check JSON syntax is valid

### Update button not working:
- Ensure ConfigUpdateManager exists in scene
- Check network connectivity
- Verify config URL is accessible
- Check console for detailed error messages

### Planes missing:
- Verify plane names match exactly (case-sensitive)
- Check config has all required planes
- Fall back to hardcoded data if needed

## Future Enhancements

Possible improvements:
- Version checking (compare local vs remote)
- Automatic updates on launch
- Config validation UI
- Plane preview/selection improvements
- Delta updates (only download changes)

## Notes

- The JSON parser in ConfigLoader is custom-built because Unity's JsonUtility doesn't support dictionaries
- Config is cached in memory after first load for performance
- All config loading happens synchronously on first access
- Update downloads happen asynchronously with callbacks
