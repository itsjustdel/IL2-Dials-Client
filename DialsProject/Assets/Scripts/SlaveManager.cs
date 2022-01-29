using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System;

public class SlaveManager : MonoBehaviour
{
    public bool createNew;
    // Start is called before the first frame update
    void Start()
    {
        string[] args = Environment.GetCommandLineArgs();

        foreach(string s in args)
        UnityEngine.Debug.Log(s);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RunFile();
            createNew = false;
        }
    }

    private static void RunFile()
    {
        //craete id and pass as arg
        string args = DateTime.Now.ToString("hh.mm.ss.ffffff");

        var process = Process.GetCurrentProcess(); // Or whatever method you are using
        string fullPath = process.MainModule.FileName;

        var myProcess = new Process();
       
        myProcess.StartInfo.FileName = fullPath;
        myProcess.StartInfo.Arguments = args;
        myProcess.Start();

    }
}
