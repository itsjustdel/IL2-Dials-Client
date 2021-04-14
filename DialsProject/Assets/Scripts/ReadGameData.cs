using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using UnityEngine;

public class ReadGameData : MonoBehaviour
{
    //server will run this script and take data from IL-2, making it available to send to the client


    public float altitude;
    public float mmhg;
    public float airspeed;
    public float climbRate;
    public float rollRate;

    //program memory address (IL-2.exe)
    IntPtr baseAddressIL2 = IntPtr.Zero;
    //RSE.dll address - moved everything to il2.exe
    //IntPtr baseAddressRSE = IntPtr.Zero;

    //offsets found from pointer search 
    private int[] offsetsAltitude = new int[] { 0x148, 0x138, 0xF18, 0x60, 0xD0};
    private int[] offsetsMMHG = new int[] { 0x148, 0x1A0, 0xEB0, 0x900, 0x628, 0x40 };
    //private int[] offsetsAirspeed = new int[] { 0x198, 0x1A0, 0xEB0, 0x60, 0x68 }; //Note: Slightly different offsets for a back up route in case anythin stops working
    private int[] offsetsAirspeed = new int[] { 0x148, 0x138, 0xF18, 0x60, 0x70 }; //Note: Slightly different offsets for a back up route in case anythin stops working

    //from ref'd dll to help read data from memory
    private VAMemory vam;
    private Process[] processes;

    public BuildControl buildControl;
    // Start is called before the first frame update

    private void Awake()
    {
        //only server stuff in this script
        if (buildControl.isClient)
            enabled = false;
    }
    void Start()
    {
        vam = new VAMemory("Il-2");        
        processes = Process.GetProcessesByName("Il-2");
    }

    // Update is called once per frame
    void Update()
    {
        
          CheckForBaseAddresses();
          ReadValuesFromMemory();
    }


    void ReadValuesFromMemory()
    {
        
        altitude = ReadFromOffsets(baseAddressIL2 + 0x00F0A368, offsetsAltitude);
        mmhg = ReadFromOffsets(baseAddressIL2 + 0x00F0A368, offsetsMMHG);

        airspeed = ReadFromOffsets(baseAddressIL2 + 0x00F0A368, offsetsAirspeed); //stop if plane is broken, need new offset?
    }

    void CheckForBaseAddresses()
    {

        //IL2.exe
        //if we have not located a base address
        if (baseAddressIL2 == IntPtr.Zero)
        {
            baseAddressIL2 = GetBaseAddressIL2();

            if (baseAddressIL2 == IntPtr.Zero)
            {
                UnityEngine.Debug.Log("No process found - IL2");
                //if we still haven't found an address, wait for next loop
                return;
            }
            else
                UnityEngine.Debug.Log("IL2 found");
        }
    }

    IntPtr GetBaseAddressIL2()
    {

        //Seartch windows process for Il-2
        IntPtr baseAddressLocal = IntPtr.Zero;

        if (processes.Length > 0)
        {
            Process MyProc = processes[0];

            foreach (ProcessModule module in MyProc.Modules)
            {
                if (module.ModuleName.Contains("Il-2"))
                {
                    //UnityEngine.Debug.Log("Il2 found base address found");
                    baseAddressLocal = module.BaseAddress;

                }
            }
        }

        return baseAddressLocal;
    }

    public float ReadFromOffsets(IntPtr address, int[] offsets)
    {
        //write here... //factor below - change Altitde() to something more general
        //we start here
        long ptr = vam.ReadInt64(address);

        //and add offsets. Need to "dereference" each offset to read value, and then add from there
        for (int i = 0; i < offsets.Length; i++)
        {
            ptr = vam.ReadInt64((IntPtr)ptr + offsets[i]);
            //var hexValue = ptr.ToString("X");
            //Console.WriteLine(hexValue);
        }
        //read last value as string
        string final = ptr.ToString("X");
        //so we can convert it to a double using...
        var hex = final;
        var int64Val = Convert.ToInt64(hex, 16);
        var doubleVal = BitConverter.Int64BitsToDouble(int64Val);

        //voila!
        return (float)doubleVal;

    }
}
