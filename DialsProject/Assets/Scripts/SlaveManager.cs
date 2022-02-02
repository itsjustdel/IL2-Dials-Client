using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System;
using System.Runtime.InteropServices;

public class SlaveManager : MonoBehaviour
{
    public AirplaneData airplaneData;
    public bool createNew;
    public bool slave = false;
    public int id;
    public int monintorNumber = -1;


    [DllImport("user32.dll")]
    static extern IntPtr GetForegroundWindow();
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
         

            id = int.Parse( args[2] );
            UnityEngine.Debug.Log("id = " + id);

        }
        else
        {
            UnityEngine.Debug.Log("This is the master");
            //the master spawns all slave windows on startup
            //the amount of slavesuser has crated is stored in player prefs
            int slaves = PlayerPrefs.GetInt("slaves");
            for (int i = 0; i < slaves; i++)
            {
                //spawn a client window for each slave and pass the id (will be plus 1 of index - master is 0)
                SpawnOldSlave(i + 1);
            }

        }
    }


    // Update is called once per frame
    void Update()
    {
        
        //UnityEngine.Debug.ClearDeveloperConsole();
        //IntPtr handle = GetForegroundWindow();
    }


  


    //new slave
    public static void SpawnNewSlave()
    {

        //create id and pass as arg
        //id is slave count + 1
        int id = PlayerPrefs.GetInt("slaves");
        id++;
        PlayerPrefs.SetInt("slaves", id);
        UnityEngine.Debug.Log("Slaves = " + id);

        string args = "Slave " + id;// System.DateTime.Now.ToString("hh.mm.ss.ffffff");

        var process = Process.GetCurrentProcess();
        string fullPath = process.MainModule.FileName;

        var myProcess = new Process();

        //window size and position -ise when loading
        //DisplayManager.SetWindowPosition(myProcess.MainWindowHandle, layout.rect.Top, layout.rect.Left);

        myProcess.StartInfo.FileName = fullPath;
        myProcess.StartInfo.Arguments = args;
        myProcess.Start();
    }

    public static void SpawnOldSlave(int id)
    {
        UnityEngine.Debug.Log("Spawnding Old slave, id = " + id);

        string args = "Slave " + id;// System.DateTime.Now.ToString("hh.mm.ss.ffffff");

        var process = Process.GetCurrentProcess();
        string fullPath = process.MainModule.FileName;

        var myProcess = new Process();

        //window size and position -ise when loading
        //DisplayManager.SetWindowPosition(myProcess.MainWindowHandle, layout.rect.Top, layout.rect.Left);

        myProcess.StartInfo.FileName = fullPath;
        myProcess.StartInfo.Arguments = args;
        myProcess.Start();
    }



}
