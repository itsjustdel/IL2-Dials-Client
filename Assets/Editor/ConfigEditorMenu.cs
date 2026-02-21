using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// Unity Editor menu items for config system debugging and testing
/// </summary>
public class ConfigEditorMenu
{
    [MenuItem("IL2 Dials/Config/Show Config Info")]
    public static void ShowConfigInfo()
    {
        Debug.Log("=== Config System Information ===");
        Debug.Log($"Config URL: {ConfigLoader.GetConfigUrl()}");
        Debug.Log($"Persistent Path: {ConfigLoader.GetConfigPath()}");
        Debug.Log($"Has Config Data: {ConfigLoader.HasConfigData()}");
        
        // Check if downloaded config exists
        string persistentPath = ConfigLoader.GetConfigPath();
        if (File.Exists(persistentPath))
        {
            FileInfo fileInfo = new FileInfo(persistentPath);
            Debug.Log($"✓ Downloaded config exists: {fileInfo.Length} bytes, modified {fileInfo.LastWriteTime}");
        }
        else
        {
            Debug.Log("✗ No downloaded config found");
        }
        
        // Check if bundled config exists
        TextAsset bundled = Resources.Load<TextAsset>("plane-config");
        if (bundled != null)
        {
            Debug.Log($"✓ Bundled config exists: {bundled.text.Length} bytes");
        }
        else
        {
            Debug.Log("✗ No bundled config in Resources");
        }
        
        // Show plane count
        var config = ConfigLoader.GetConfig();
        if (config != null && config.planes != null)
        {
            Debug.Log($"✓ Config loaded with {config.planes.Count} planes");
            
            // Group by country
            var planesByCountry = ConfigLoader.GetPlanesByCountry();
            Debug.Log("\nPlanes by country:");
            foreach (var kvp in planesByCountry)
            {
                Debug.Log($"  {kvp.Key}: {kvp.Value.Count} planes");
            }
        }
    }

    [MenuItem("IL2 Dials/Config/Test Config Loading")]
    public static void TestConfigLoading()
    {
        Debug.Log("=== Testing Config Loading ===");
        
        // Test a few planes
        string[] testPlanes = { "LaGG-3 ser.29", "Bf 109 F-4", "P-51D-15", "Spitfire Mk.Vb" };
        
        foreach (string planeName in testPlanes)
        {
            var attributes = ConfigLoader.GetPlaneAttributes(planeName);
            
            if (attributes != null)
            {
                Debug.Log($"✓ {planeName}");
                Debug.Log($"  Country: {attributes.country}");
                Debug.Log($"  Engines: {attributes.engines}");
                Debug.Log($"  Altimeter: {attributes.altimeter}");
            }
            else
            {
                Debug.LogWarning($"✗ Could not load attributes for: {planeName}");
            }
        }
    }

    [MenuItem("IL2 Dials/Config/Force Reload Config")]
    public static void ForceReloadConfig()
    {
        Debug.Log("=== Force Reloading Config ===");
        ConfigLoader.ReloadConfig();
        PlaneLists.Reload();
        Debug.Log("✓ Config and PlaneLists reloaded");
        ShowConfigInfo();
    }

    [MenuItem("IL2 Dials/Config/Delete Downloaded Config")]
    public static void DeleteDownloadedConfig()
    {
        string path = ConfigLoader.GetConfigPath();
        
        if (File.Exists(path))
        {
            if (EditorUtility.DisplayDialog(
                "Delete Downloaded Config",
                $"Delete config file at:\n{path}\n\nThis will force the game to use bundled config.",
                "Delete", "Cancel"))
            {
                File.Delete(path);
                Debug.Log($"✓ Deleted downloaded config: {path}");
                ConfigLoader.ReloadConfig();
                PlaneLists.Reload();
                Debug.Log("✓ Config reloaded from bundled source");
            }
        }
        else
        {
            EditorUtility.DisplayDialog("No Downloaded Config", "No downloaded config file exists.", "OK");
        }
    }

    [MenuItem("IL2 Dials/Config/Open Config Folder")]
    public static void OpenConfigFolder()
    {
        string path = ConfigLoader.GetConfigPath();
        string folder = Path.GetDirectoryName(path);
        
        if (Directory.Exists(folder))
        {
            EditorUtility.RevealInFinder(folder);
        }
        else
        {
            if (EditorUtility.DisplayDialog(
                "Config Folder Not Found",
                $"Config folder does not exist:\n{folder}\n\nCreate it?",
                "Create", "Cancel"))
            {
                Directory.CreateDirectory(folder);
                EditorUtility.RevealInFinder(folder);
            }
        }
    }

    [MenuItem("IL2 Dials/Config/Validate All Planes")]
    public static void ValidateAllPlanes()
    {
        Debug.Log("=== Validating All Planes ===");
        
        var config = ConfigLoader.GetConfig();
        if (config == null || config.planes == null || config.planes.Count == 0)
        {
            Debug.LogError("✗ No config data loaded!");
            return;
        }
        
        int validPlanes = 0;
        int invalidPlanes = 0;
        
        foreach (var kvp in config.planes)
        {
            string planeName = kvp.Key;
            PlaneConfigData planeData = kvp.Value;
            
            // Basic validation
            bool isValid = true;
            
            if (string.IsNullOrEmpty(planeData.country))
            {
                Debug.LogWarning($"✗ Plane '{planeName}' has no country");
                isValid = false;
            }
            
            if (planeData.engines < 1 || planeData.engines > 4)
            {
                Debug.LogWarning($"✗ Plane '{planeName}' has invalid engine count: {planeData.engines}");
                isValid = false;
            }
            
            if (isValid)
            {
                validPlanes++;
            }
            else
            {
                invalidPlanes++;
            }
        }
        
        Debug.Log($"\n=== Validation Complete ===");
        Debug.Log($"✓ Valid planes: {validPlanes}");
        if (invalidPlanes > 0)
        {
            Debug.LogWarning($"✗ Invalid planes: {invalidPlanes}");
        }
        else
        {
            Debug.Log("✓ All planes valid!");
        }
    }

    [MenuItem("IL2 Dials/Config/Compare Hardcoded vs Config")]
    public static void CompareHardcodedVsConfig()
    {
        Debug.Log("=== Comparing Hardcoded vs Config ===");
        
        // This would require more complex analysis
        // For now, just show counts
        
        var config = ConfigLoader.GetConfig();
        Debug.Log($"Config planes: {config.planes.Count}");
        
        Debug.Log($"RU planes (PlaneLists): {PlaneLists.RuPlanes.Count}");
        Debug.Log($"GER planes (PlaneLists): {PlaneLists.GerPlanes.Count}");
        Debug.Log($"US planes (PlaneLists): {PlaneLists.UsPlanes.Count}");
        Debug.Log($"UK planes (PlaneLists): {PlaneLists.UkPlanes.Count}");
        Debug.Log($"ITA planes (PlaneLists): {PlaneLists.ItaPlanes.Count}");
        Debug.Log($"FR planes (PlaneLists): {PlaneLists.FrPlanes.Count}");
        
        int totalFromLists = PlaneLists.RuPlanes.Count + PlaneLists.GerPlanes.Count + 
                            PlaneLists.UsPlanes.Count + PlaneLists.UkPlanes.Count +
                            PlaneLists.ItaPlanes.Count + PlaneLists.FrPlanes.Count;
        
        Debug.Log($"Total from PlaneLists: {totalFromLists}");
    }
}
