using UnityEngine;

public class PlayerPrefsHelper : MonoBehaviour
{

    // Start is called before the first frame update
    public static string[] GetRegistryValues()
    {


        //add editor? https://github.dev/sabresaurus/PlayerPrefsEditor/blob/main/Editor/PlayerPrefsEditor.cs -- a how to

        //TOD add if editor string op

#if UNITY_EDITOR && !UNITY_ANDROID
        string companyName = "DellyWellySoftware"; //how to get dynamically?
        string productName = "IL-2 Dials";
        Microsoft.Win32.RegistryKey registryKey;
        registryKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Unity\\UnityEditor\\DellyWellySoftware\\IL-2 Dials");
        string[] valueNames = registryKey.GetValueNames();
        return valueNames;

#elif UNITY_STANDALONE_WIN
        string companyName = "DellyWellySoftware"; //how to get dynamically?
        string productName = "IL-2 Dials";
        Microsoft.Win32.RegistryKey registryKey;
       registryKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software\\" + companyName + "\\" + productName);
       string[] valueNames = registryKey.GetValueNames();
       return valueNames;

#else
        return null;
#endif



    }
}
