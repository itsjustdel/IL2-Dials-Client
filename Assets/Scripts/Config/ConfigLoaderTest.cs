using UnityEngine;

/// <summary>
/// Simple test script to validate config loading system
/// Attach to a GameObject and check console logs
/// </summary>
public class ConfigLoaderTest : MonoBehaviour
{
    [Header("Run tests on Start")]
    public bool runTestsOnStart = true;
    
    [Header("Test Results")]
    public bool testsCompleted = false;
    public int testsPassed = 0;
    public int testsFailed = 0;

    private void Start()
    {
        if (runTestsOnStart)
        {
            RunTests();
        }
    }

    [ContextMenu("Run Config Tests")]
    public void RunTests()
    {
        Debug.Log("=== Config Loader Tests Starting ===");
        testsCompleted = false;
        testsPassed = 0;
        testsFailed = 0;

        // Test 1: ConfigLoader loads config
        Test_ConfigLoaderLoadsConfig();

        // Test 2: Get plane attributes
        Test_GetPlaneAttributes();

        // Test 3: PlaneLists loads from config
        Test_PlaneListsLoadFromConfig();

        // Test 4: PlaneDataFromName uses config
        Test_PlaneDataFromNameUsesConfig();

        // Test 5: Get planes by country
        Test_GetPlanesByCountry();

        Debug.Log($"=== Tests Complete: {testsPassed} passed, {testsFailed} failed ===");
        testsCompleted = true;
    }

    private void Test_ConfigLoaderLoadsConfig()
    {
        Debug.Log("\n--- Test 1: ConfigLoader loads config ---");
        
        var config = ConfigLoader.GetConfig();
        
        if (config != null && config.planes != null)
        {
            Debug.Log($"✓ Config loaded with {config.planes.Count} planes");
            testsPassed++;
        }
        else
        {
            Debug.LogError("✗ Failed to load config");
            testsFailed++;
        }
    }

    private void Test_GetPlaneAttributes()
    {
        Debug.Log("\n--- Test 2: Get plane attributes ---");
        
        string[] testPlanes = { "LaGG-3 ser.29", "Bf 109 F-4", "P-51D-15" };
        
        foreach (string planeName in testPlanes)
        {
            var attributes = ConfigLoader.GetPlaneAttributes(planeName);
            
            if (attributes != null)
            {
                Debug.Log($"✓ Got attributes for '{planeName}' - Country: {attributes.country}, Engines: {attributes.engines}");
                testsPassed++;
            }
            else
            {
                // Try fallback to hardcoded
                attributes = PlaneDataFromName.AttributesFromName(planeName);
                if (attributes != null)
                {
                    Debug.LogWarning($"⚠ '{planeName}' not in config, used hardcoded fallback");
                    testsPassed++;
                }
                else
                {
                    Debug.LogError($"✗ Failed to get attributes for '{planeName}'");
                    testsFailed++;
                }
            }
        }
    }

    private void Test_PlaneListsLoadFromConfig()
    {
        Debug.Log("\n--- Test 3: PlaneLists loads from config ---");
        
        var ruPlanes = PlaneLists.RuPlanes;
        var gerPlanes = PlaneLists.GerPlanes;
        var usPlanes = PlaneLists.UsPlanes;
        
        Debug.Log($"Russian planes: {ruPlanes.Count}");
        Debug.Log($"German planes: {gerPlanes.Count}");
        Debug.Log($"US planes: {usPlanes.Count}");
        
        if (ruPlanes.Count > 0 && gerPlanes.Count > 0 && usPlanes.Count > 0)
        {
            Debug.Log("✓ PlaneLists successfully loaded");
            testsPassed++;
        }
        else
        {
            Debug.LogError("✗ PlaneLists failed to load");
            testsFailed++;
        }
    }

    private void Test_PlaneDataFromNameUsesConfig()
    {
        Debug.Log("\n--- Test 4: PlaneDataFromName uses config ---");
        
        string testPlane = "LaGG-3 ser.29";
        var attributes = PlaneDataFromName.AttributesFromName(testPlane);
        
        if (attributes != null)
        {
            Debug.Log($"✓ PlaneDataFromName returned attributes for '{testPlane}'");
            Debug.Log($"  Country: {attributes.country}");
            Debug.Log($"  Altimeter: {attributes.altimeter}");
            Debug.Log($"  Engines: {attributes.engines}");
            testsPassed++;
        }
        else
        {
            Debug.LogError($"✗ PlaneDataFromName failed for '{testPlane}'");
            testsFailed++;
        }
    }

    private void Test_GetPlanesByCountry()
    {
        Debug.Log("\n--- Test 5: Get planes by country ---");
        
        var planesByCountry = ConfigLoader.GetPlanesByCountry();
        
        Debug.Log($"Countries found: {planesByCountry.Count}");
        
        foreach (var kvp in planesByCountry)
        {
            Debug.Log($"  {kvp.Key}: {kvp.Value.Count} planes");
        }
        
        if (planesByCountry.Count > 0)
        {
            Debug.Log("✓ GetPlanesByCountry successful");
            testsPassed++;
        }
        else
        {
            Debug.LogError("✗ GetPlanesByCountry returned no data");
            testsFailed++;
        }
    }

    [ContextMenu("Show Config Info")]
    public void ShowConfigInfo()
    {
        Debug.Log("\n=== Config Information ===");
        Debug.Log($"Config path: {ConfigLoader.GetConfigPath()}");
        Debug.Log($"Config URL: {ConfigLoader.GetConfigUrl()}");
        Debug.Log($"Has config data: {ConfigLoader.HasConfigData()}");
        
        var config = ConfigLoader.GetConfig();
        if (config != null && config.planes != null)
        {
            Debug.Log($"Planes in config: {config.planes.Count}");
            
            // Show first few planes
            int count = 0;
            Debug.Log("\nSample planes:");
            foreach (var kvp in config.planes)
            {
                if (count++ >= 5) break;
                Debug.Log($"  - {kvp.Key} ({kvp.Value.country})");
            }
        }
    }

    [ContextMenu("Force Reload Config")]
    public void ForceReload()
    {
        Debug.Log("\n=== Force Reloading Config ===");
        ConfigLoader.ReloadConfig();
        PlaneLists.Reload();
        Debug.Log("Config and PlaneLists reloaded");
    }
}
