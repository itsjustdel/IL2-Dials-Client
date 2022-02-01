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

        Microsoft.Win32.RegistryKey registryKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software\\" + companyName + "\\" + productName);

        //grab all keyss
        string[] valueNames = registryKey.GetValueNames();

        //foreach (string s in valueNames)
        //    Debug.Log(s);

        return valueNames;

    }
}
