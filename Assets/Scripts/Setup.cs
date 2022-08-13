using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Setup : MonoBehaviour
{    
    // Start is called before the first frame update
    void Start()
    {
        //breaking layout changes were introduced in 0.6
        if (PlayerPrefs.GetInt("0.6 update") == 0)
        {
            string[] keys = PlayerPrefsHelper.GetRegistryValues();
            for (int i = 0; i < keys.Length; i++)
            {
                string[] split = keys[i].Split(' ');
                if (split[0] == "layout")
                {
                    Debug.Log("Deleting pref key:" + keys[i]);
                    PlayerPrefs.DeleteKey(keys[i]);
                } 
            }

            PlayerPrefs.SetInt("0.6 update", 1);
        }
    }
}
