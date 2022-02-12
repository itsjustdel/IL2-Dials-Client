using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefsHelper : MonoBehaviour
{

    // Start is called before the first frame update
    public static string[] GetRegistryValues()
    {
        string companyName = "DellyWellySoftware"; //how to get dynamically?
        string productName = "IL-2 Dials";

        //add editor? https://github.dev/sabresaurus/PlayerPrefsEditor/blob/main/Editor/PlayerPrefsEditor.cs -- a how to

        //TOD add if editor string op


        Microsoft.Win32.RegistryKey registryKey;

#if UNITY_EDITOR
       registryKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Unity\\UnityEditor\\DellyWellySoftware\\IL-2 Dials");
#elif UNITY_STANDALONE_WIN
       registryKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software\\" + companyName + "\\" + productName);
#endif

        //grab all keyss
        string[] valueNames = registryKey.GetValueNames();

        //foreach (string s in valueNames)
        //    Debug.Log(s);

        return valueNames;

    }
}
