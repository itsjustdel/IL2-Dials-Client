using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using UnityEngine;

public class RotateNeedle : MonoBehaviour
{

    
    public float altitude;
    public float mmhg;

    public float airspeed;

    public GameObject altitudeNeedleSmall;
    public GameObject altitudeNeedleLarge;
    public GameObject mmhgDial;

    public GameObject airspeedNeedle;

    //program memory address (IL-2.exe)
    IntPtr baseAddressIL2 = IntPtr.Zero;
    //RSE.dll address - moved everything to il2.exe
    //IntPtr baseAddressRSE = IntPtr.Zero;

    //offsets found from pointer search 
    int[] offsetsAltitude = new int[] { 0x198, 0xB60, 0x4F0, 0x900, 0x628, 0x28 };
    int[] offsetsMMHG = new int[] { 0x198, 0xB60, 0x4F0, 0x900, 0x628, 0x40 };
    int[] offsetsAirspeed = new int[] { 0x198, 0x1A0, 0xEB0, 0x60, 0x68 }; //Note: Slightly different offsets for a back up route in case anythin stops working

    public VAMemory vam;
    public Process[] processes;
    // Start is called before the first frame update
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

        RotateNeedles();

        RotateMMHG();
    }

    void RotateNeedles()
    {
        AltitudeNeedles();

        AirspeedNeedle();
    }

    void RotateMMHG()
    {
        //barometer/pressure dial inside main dial

        //sea level is 760
        //zero rotation on 760

        mmhgDial.transform.rotation = Quaternion.Euler(0, 0, ((760f - mmhg) / 100f) * 300) ;

    }

    void AltitudeNeedles()
    {
        //small hand gose to thousands
        altitudeNeedleSmall.transform.rotation = Quaternion.Euler(0, 0, -(altitude / 10000f) * 360);
        //large hand to hundreds
        altitudeNeedleLarge.transform.rotation = Quaternion.Euler(0, 0, -(altitude / 1000f) * 360);
    }

    void AirspeedNeedle()
    {
        //airspeed dial has three gears
        //below 100
        if(airspeed < 100)
            //if 0 -> 0
            //if 50 -> 15
            //if 100 -> 30
            airspeedNeedle.transform.rotation = Quaternion.Euler(0, 0, -((airspeed / 50f) * 15f));
        //below 300
        else if (airspeed < 300)
            //for every 5 kmh, move 30 degrees            
            //if 100 -> 30
            //if 150 -> 60
            //if 200 -> 90
            airspeedNeedle.transform.rotation = Quaternion.Euler(0, 0,  -((airspeed / 50f) * 30f) + 30f );
        else
            //over 300
            //for evry 10, move 40, but starts at 300 (150degrees)
            //if 600 -> 270
            //if 700 -> 310            
            airspeedNeedle.transform.rotation = Quaternion.Euler(0, 0, -((airspeed / 10f) * 4) - 30 );
        
    }
    void ReadValuesFromMemory()
    {  
         altitude = ReadFromOffsets(baseAddressIL2 + 0x00F031B8, offsetsAltitude);         
         mmhg = ReadFromOffsets(baseAddressIL2 + 0x00F031B8, offsetsMMHG);

        airspeed = ReadFromOffsets(baseAddressIL2 + 0x00F031B8, offsetsAirspeed); //stop if plane is broken, need new offset?
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

        //RSE.dll
        /*
        if (baseAddressRSE == IntPtr.Zero)
        {
            baseAddressRSE = GetBaseAddressRSE();

            if (baseAddressRSE == IntPtr.Zero)
            {
                UnityEngine.Debug.Log("No process found - RSE");
                //if we still haven't found an address, wait for next loop
                return;
            }
            else
                UnityEngine.Debug.Log("RSE found");
        }
        */
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
                    //Console.WriteLine("Il2 found");
                    baseAddressLocal = module.BaseAddress;

                }
            }
        }

        return baseAddressLocal;
    }

    IntPtr GetBaseAddressRSE()
    {

        //Seartch windows process for Il-2
        IntPtr baseAddressLocal = IntPtr.Zero;

        if (processes.Length > 0)
        {
            Process MyProc = processes[0];

            foreach (ProcessModule module in MyProc.Modules)
            {
                if (module.ModuleName.Contains("RSE.dll"))
                {
                    //Console.WriteLine("Il2 found");
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
