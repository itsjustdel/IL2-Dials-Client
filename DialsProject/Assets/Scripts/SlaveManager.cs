using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System;
using System.Runtime.InteropServices;

public class SlaveManager : MonoBehaviour
{
    public bool createNew;
    public bool slave = false;
    public string id;
    public int monintorNumber = -1;
    // Start is called before the first frame update



    //https://stackoverflow.com/questions/27977924/in-windows-10-how-can-we-determine-which-virtual-desktop-a-window-belongs-to   ????

    //https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-monitorfromwindow
    void Start()
    {

        //DisplayManager dM = new DisplayManager();

        string[] args = Environment.GetCommandLineArgs();

        //List<DisplayManager.DISPLAY_DEVICE> ddd = DisplayManager.Devices();

        //foreach(string s in args)
        //UnityEngine.Debug.Log(s);

        //get args and save them to public variables for easy access
        if (args.Length == 3)
        {
            if (args[1] == "Slave")
                slave = true;

            id = args[2];
        }

    }

    [DllImport("user32.dll")]
    static extern IntPtr GetForegroundWindow();

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            RunFile();
            createNew = false;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            Camera.main.targetDisplay = 2;
            
        }

        UnityEngine.Debug.ClearDeveloperConsole();
        IntPtr handle = GetForegroundWindow();
    }

    private static void RunFile()
    {
        //craete id and pass as arg
        string args = "Slave " + DateTime.Now.ToString("hh.mm.ss.ffffff");

        var process = Process.GetCurrentProcess(); 
        string fullPath = process.MainModule.FileName;

        var myProcess = new Process();
       
        myProcess.StartInfo.FileName = fullPath;
        myProcess.StartInfo.Arguments = args;
        myProcess.Start();

    }


}
