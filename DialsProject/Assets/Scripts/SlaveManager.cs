using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System;
using System.Runtime.InteropServices;

public class SlaveManager : MonoBehaviour
{
    public AirplaneData airplaneData;
    public GameObject menuPanel;
    public GameObject displayPanel;
    public GameObject confirmObject;
    public GameObject masterClientText;
    public bool createNew;
    public bool slave = false;
    public int id;
    public int monintorNumber = -1;
    


    [DllImport("user32.dll", SetLastError = true)]
    internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);
    // Start is called before the first frame update

    //when a slave is launched by the master, the master sets its window size and position
    //when a slave loads for the first time, it runs Start
    //it should check if its id matches any of the keys in player prefs

    void Start()
    {
        string[] args = Environment.GetCommandLineArgs();

        UnityEngine.Debug.Log("args = ");
        foreach(string s in args)
            UnityEngine.Debug.Log(s);


        //get args and save them to public variables for easy access
        //first arg is file location of exe
        if (args.Length == 3)
        {
            if (args[1] == "Slave")
            {
                UnityEngine.Debug.Log("This is a slave");

                slave = true;
            }


            id = int.Parse(args[2]);
            UnityEngine.Debug.Log("id = " + id);

        }
        else
        {
            UnityEngine.Debug.Log("This is the master");
            //the master spawns all slave windows on startup
            string[] keys = PlayerPrefsHelper.GetRegistryValues();            
            foreach (string key in keys)
            {
                //layout keys are saved with id then plane type e.g (0 il2 mod 1942), (1 spitfire-123)
                string[] subs = key.Split(' ');

                //we are looking for window Info keys
                if (subs[0] == "windowInfo")
                {
                    //strip id number from registry key which has _hxxxxxxx after it
                    string idString = "";
                    int start = subs[0].Length + 1;
                    for (int i = start; i < key.Length - 1; i++)
                    {
                        //look for handle "_h" - we don't need values after key[x] is a char so do conversion                        
                        if (key[i].ToString() == "_" && key[i + 1].ToString() == "h")
                            break;

                        idString += key[i];
                    }

                    int _id = int.Parse(idString);

                    //0 is the master
                    if(_id > 0)
                        //use this if to spawn a previous slave
                        SpawnOldSlave(_id);
                }
            }
        }
    }

    //new slave
    public  void SpawnNewSlave()
    {

        //create id and pass as arg
        //find the highest previous id - all salves have a windowInfo key attached find them
        //get all player prefs that start with this plane type
        string[] keys = PlayerPrefsHelper.GetRegistryValues();
        int highest = 0;
        foreach (string key in keys)
        {
            //layout keys are saved with id then plane type e.g (0 il2 mod 1942), (1 spitfire-123)
            string[] subs = key.Split(' ');

            //we are looking for a key to find the highest id
            if  (subs[0] == "windowInfo")
            {

                //strip id number from registry key which has _hxxxxxxx after it
                string idString = "";
                int start = subs[0].Length + 1;
                for (int i = start; i < key.Length - 1; i++)
                {
                    //look for handle "_h" - we don't need values after key[x] is a char so do conversion                        
                    if (key[i].ToString() == "_" && key[i + 1].ToString() == "h")
                        break;

                    idString += key[i];
                }

                int _id = int.Parse(idString);
                if (_id > highest)
                    highest = _id;

            }
        }

        highest += 1;
        string args = "Slave " + highest.ToString();

        var process = Process.GetCurrentProcess();
        string fullPath = process.MainModule.FileName;

        var myProcess = new Process();

        myProcess.StartInfo.FileName = fullPath;
        myProcess.StartInfo.Arguments = args;
        myProcess.Start();
    }

    public static void SpawnOldSlave(int id)
    {
        string args = "Slave " + id;

        var process = Process.GetCurrentProcess();
        string fullPath = process.MainModule.FileName;

        var myProcess = new Process();

     
        //window size and position -use when loading - 
        //SetWindowPos(myProcess.MainWindowHandle, layout.rect.Top, layout.rect.Left);

        myProcess.StartInfo.FileName = fullPath;
        myProcess.StartInfo.Arguments = args;
        myProcess.Start();
                
    }

    public void AddSlaveWindow()
    {
       SpawnNewSlave();
    }

    public void DeleteScreen()
    {
        if(id == 0)
        {
            UnityEngine.Debug.Log("Can't delete main client");
        //    return;
        }

        //get all player prefs that start with this plane type
        string[] keys = PlayerPrefsHelper.GetRegistryValues();

        foreach (string key in keys)
        {
            //layout keys are saved with id then plane type e.g (0 il2 mod 1942), (1 spitfire-123)
            string[] subs = key.Split(' ');

            //we are looking for a layout etc
            if (subs[0] == "layout" || subs[0] == "fullscreen" || subs[0] == "windowInfo")
            {            

                //strip id number from registry key which has _hxxxxxxx after it
                string idString = "";
                int start = subs[0].Length + 1;
                for (int i = start; i < key.Length - 1; i++)
                {
                    //look for handle "_h" - we don't need values after key[x] is a char so do conversion                        
                    if (key[i].ToString() == "_" && key[i + 1].ToString() == "h")
                        break;

                    idString += key[i];
                }

                if (int.Parse(idString) == id)
                {
                    string keyToDelete = subs[0] + " " + idString;
                    UnityEngine.Debug.Log("deleting key = " + keyToDelete);
                    PlayerPrefs.DeleteKey(keyToDelete);
                }
            }
        }

        //and close the app
        Application.Quit();

    }

    public void DeleteConfirm()
    {
        if (slave)
            confirmObject.SetActive(true);
        else
            masterClientText.SetActive(true);
            
    }


    public void ScreensPanelBack()
    {
        displayPanel.SetActive(false);
        menuPanel.SetActive(true);

        //reset text
        confirmObject.SetActive(false);
        masterClientText.SetActive(false);
    }

}
