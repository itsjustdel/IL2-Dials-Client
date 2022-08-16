using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using UnityEngine;

public enum Country
{
    RU,
    GER,
    US,
    UK,
    ITA,
    FR,
    UNDEFINED

}

public class AirplaneData : MonoBehaviour
{
    public bool tests;
    public float scalar0;
    public float scalar1;
    public bool useTestPlane;
    public GameObject testPlane;

    public float clientVersion = 0.3f; //manually update this
    public float serverVersion;


    public string planeType;
    public string planeTypePrevious;
    //stores name avaialble dials

    public PlaneDataFromName.PlaneAttributes planeAttributes;

    public float altitude;
    public float mmhg;
    public float airspeed;
    public float pitch;
    public float roll;
    public float rollPrev;
    public float verticalSpeed;
    public float turnCoordinatorBall;
    public float turnCoordinatorNeedle;
    public float heading;
    public float headingPrevious;
    public float headingPreviousPrevious;
    public List<float> rpms;
    public List<float> manifolds;
    public List<float> waterTemps;
    public List<float> oilTemps;
    public int engineModification;

    public BuildControl buildControl;
    public MenuHandler menuHandler;
    public UDPClient udpClient;
    

    public void Start()
    {
        //initialise list with empty
        rpms = new List<float> { 0f, 0f, 0f, 0f };
        manifolds = new List<float> { 0f, 0f, 0f, 0f };
        waterTemps = new List<float> { 0f, 0f};
        oilTemps = new List<float> { 0f, 0f };
    }

    //fixed update is enough for checking status
    void FixedUpdate()
    {
        //check client version against incoming server message
        CheckVersion();


    }


    void CheckVersion()
    {

        //checks version and shows message if mismatch (if connected)
        if (udpClient.connected)
        {
            if (serverVersion != clientVersion)
            {
                //show server message
                menuHandler.ServerMessageOpen();
            }
            else
            {
                //all good
                menuHandler.ServerMessageClosed();
            }
        }
        else
        {
            //we are not connected to anything we don't know anything about version numbersd
            menuHandler.ServerMessageClosed();
        }
    }

    internal void setPlaneType(string planeType)
    {
        //do not set if menu open
        if (!menuHandler.layoutOpen && !menuHandler.tigerMothSelected)
            this.planeType = planeType;
    }

}
