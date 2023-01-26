using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildControl : MonoBehaviour
{
    
    public bool isServer = false;
    public bool isClient = false;
    public bool freeVersion;
    public void Awake()
    {

        Application.targetFrameRate = 60;

        //stop screen from turnign off
        if (Application.platform == RuntimePlatform.Android)
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }



        if (isServer)
            Debug.Log("This is the server");
        
       // if(isClient)
       //     Debug.Log("This is the client");

        if(isClient && isServer)
        {
            Debug.Log("WARNING");
        }

        if(!isClient && !isServer)
        {
            Debug.Log("Build control needs selected");
        }
    }
}
