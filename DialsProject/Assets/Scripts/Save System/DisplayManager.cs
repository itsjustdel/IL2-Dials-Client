using System;
using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;

//Class to access windows info on display device info

public class DisplayManager : MonoBehaviour
{
    public static List<DISPLAY_DEVICE> Devices()
    {
        List<DISPLAY_DEVICE> devices = new List<DISPLAY_DEVICE>();
        var dd = new DISPLAY_DEVICE();
        dd.cb = Marshal.SizeOf(dd);
        uint devNum = 0;
        while (EnumDisplayDevices(null, devNum, ref dd, 0))
        {
            uint devMon = 0;
            var ddMon = new DISPLAY_DEVICE();
            ddMon.cb = Marshal.SizeOf(ddMon);
            while (EnumDisplayDevices(dd.DeviceName, devMon, ref ddMon, 0))
            {
                devices.Add(dd);

                devMon++;
                ddMon.cb = Marshal.SizeOf(ddMon);
            }
            devNum++;
            dd.cb = Marshal.SizeOf(dd);
        }
        return devices;
    }

    [DllImport("user32.dll")]
    public static extern bool EnumDisplayDevices(string lpDevice, uint iDevNum, ref DISPLAY_DEVICE lpDisplayDevice, uint dwFlags);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct DISPLAY_DEVICE
    {
        [MarshalAs(UnmanagedType.U4)] public int cb;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)] public string DeviceName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)] public string DeviceString;
        [MarshalAs(UnmanagedType.U4)] public DisplayDeviceStateFlags StateFlags;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)] public string DeviceID;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)] public string DeviceKey;
    }

    [Flags()]
    public enum DisplayDeviceStateFlags : int
    {
        AttachedToDesktop = 0x1,
        MultiDriver = 0x2,
        PrimaryDevice = 0x4,
        MirroringDriver = 0x8,
        VGACompatible = 0x16,
        Removable = 0x20,
        ModesPruned = 0x8000000,
        Remote = 0x4000000,
        Disconnect = 0x2000000
    }
}