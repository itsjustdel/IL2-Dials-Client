using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using UnityEngine;

public class RotateNeedle : MonoBehaviour
{
    
    public ReadGameData iL2GameDataClient;
    public TCPClient tcpClient;
    public GameObject altitudeNeedleSmall;
    public GameObject altitudeNeedleLarge;
    public GameObject mmhgDial;
    public GameObject airspeedNeedle;
    public GameObject airspeedNeedleTest;
    public GameObject turnTrack;
    public GameObject turnPlane;
    public bool tcpReceived = false;
    public float lastMessageReceivedTime;//two ways of doing the same thing
    public float maxSpin =1f;
    public float turnAndBankPitchMultiplier = 5f;
    public float turnAndBankRollMultiplier = 5f;
    public float turnAndBankPlaneXrotation = 5f;


    //previous frame positions for client prediction
    private List<Quaternion> quaternionsAltitudeLarge = new List<Quaternion>() { Quaternion.identity, Quaternion.identity };
    private List<Quaternion> quaternionsAltitudeSmall = new List<Quaternion>() { Quaternion.identity, Quaternion.identity };
    public List<Quaternion> quaternionsAirspeed = new List<Quaternion>() { Quaternion.identity, Quaternion.identity };
    private List<Quaternion> quaternionsMmhg = new List<Quaternion>() { Quaternion.identity, Quaternion.identity };
  //  private bool saveForPredictions;
    public Quaternion airspeedStart;
    public  Quaternion airspeedTarget;
    private Quaternion altitudeLargeStart;
    private Quaternion altitudeLargeTarget;
    private Quaternion altitudeSmallStart;
    private Quaternion altitudeSmallTarget;
    private Quaternion mmhgStart;
    private Quaternion mmhgTarget;

    public bool testPrediction = false;




    // Start is called before the first frame update
    void Start()
    {

        Application.targetFrameRate = 60;

        //stop screen from turnign off
        if (Application.platform == RuntimePlatform.Android)
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }
        
    }


    // Update is called once per frame
    void Update()
    {
        bool test = false;
        if (test)
        {
            iL2GameDataClient.airspeed -= 10;
            iL2GameDataClient.altitude += 10;
            iL2GameDataClient.mmhg += 10;
        }


        if (!tcpClient.connected)
        {
            //if we have completely lost connection reset needles, there is a 5 second grace period where prediction takes over            
            iL2GameDataClient.altitude = 0f;
            iL2GameDataClient.mmhg = 0f;
            iL2GameDataClient.airspeed = 0f;
        }
        else //we are connected
        {
            if (tcpReceived)
            {

                //flag set by tcp client in async thread

                lastMessageReceivedTime = Time.time;
                SetRotationTargets();

                tcpReceived = false;


            }

            //check to see if we need to predict or if we received a new update recently
            else if (Time.time - lastMessageReceivedTime > Time.fixedDeltaTime)//we send and receive on fixed time step
            {
                tcpReceived = false;

                //   Debug.Log("0");
                lastMessageReceivedTime += Time.fixedDeltaTime;

                //  SavePreviousRotations(); -- don't save roation,s we will just use the last one to continue with until real update occurs
                PredictRotations();
            }
        }


        //float difference = quaternionsAirspeed[0].eulerAngles.z - quaternionsAirspeed[1].eulerAngles.z;
        //Debug.Log(difference);


        NeedleRotations();


    }

    void SavePreviousRotations()
    {
        //used for prediction - save previous position

        AddRotationToList(quaternionsAirspeed, airspeedNeedle.transform.rotation);
        AddRotationToList(quaternionsAltitudeSmall, altitudeNeedleSmall.transform.rotation);
        AddRotationToList(quaternionsAltitudeLarge, altitudeNeedleLarge.transform.rotation);
        AddRotationToList(quaternionsMmhg, mmhgDial.transform.rotation);

    }
    List<Quaternion> AddRotationToList(List<Quaternion> qList, Quaternion toAdd)
    {
        //add at start
        qList.Insert(0, toAdd);
        //and cap length, we only need to do simple prediction
        if (qList.Count > 2)
            qList.RemoveAt(2);

        return qList;
    }

    void PredictRotations()
    {

        //simulate a tcp event

        if (quaternionsAirspeed.Count < 2)
            return;
        
       // Debug.Log("predicting");
        //airspeed - prediction doesn't take in to account gearing on speedometer
        //last known difference
        float difference = quaternionsAirspeed[0].eulerAngles.z - quaternionsAirspeed[1].eulerAngles.z;        
        //keep moving at client send rate at previous known step        
        //set start point
        airspeedStart =  airspeedNeedle.transform.rotation;
        //and end point
        airspeedTarget = airspeedNeedle.transform.rotation * Quaternion.Euler(0, 0, difference); ;

        //altitude
        //last known difference
        difference = quaternionsAltitudeLarge[0].eulerAngles.z - quaternionsAltitudeLarge[1].eulerAngles.z;
        //keep moving at client send rate at previous known step
        //set start point
        altitudeLargeStart = altitudeNeedleLarge.transform.rotation;
        //and end point
        altitudeLargeTarget = altitudeNeedleLarge.transform.rotation * Quaternion.Euler(0, 0, difference); ;

        //small altutude
        difference = quaternionsAltitudeSmall[0].eulerAngles.z - quaternionsAltitudeSmall[1].eulerAngles.z;
        //keep moving at client send rate at previous known step        
        //set start point
        altitudeSmallStart = altitudeNeedleSmall.transform.rotation;
        //and end point
        altitudeSmallTarget = altitudeNeedleSmall.transform.rotation * Quaternion.Euler(0, 0, difference); ;

        difference = quaternionsMmhg[0].eulerAngles.z - quaternionsMmhg[1].eulerAngles.z;
        //keep moving at client send rate at previous known step        
        //set start point
        mmhgStart = mmhgDial.transform.rotation;
        //and end point
        mmhgTarget = mmhgDial.transform.rotation * Quaternion.Euler(0, 0, difference);
    }

    public void SetRotationTargets()
    {
        //called when tcp client receives update
        SavePreviousRotations();

        AirspeedStart();
        AirspeedTarget();

        AltitudeStarts();
        AltitudeTargets();

        MmhgStart();
        MmhgTarget();
     
    }
    void AirspeedStart()
    {
        airspeedStart = quaternionsAirspeed[0];
    }
    void AltitudeStarts()
    {
        altitudeSmallStart = quaternionsAltitudeSmall[0];
        altitudeLargeStart = quaternionsAltitudeLarge[0];
    }

    void MmhgStart()
    {
        mmhgStart = quaternionsMmhg[0];
    }

    void AirspeedTarget()
    {
        //airspeed dial has three gears
        //below 100
        Quaternion target;// = Quaternion.identity;
        if (iL2GameDataClient.airspeed < 100)
            //if 0 -> 0
            //if 50 -> 15
            //if 100 -> 30
            target = Quaternion.Euler(0, 0, -((iL2GameDataClient.airspeed / 50f) * 15f));

        //below 300
        else if (iL2GameDataClient.airspeed < 300)
            //for every 5 kmh, move 30 degrees            
            //if 100 -> 30
            //if 150 -> 60
            //if 200 -> 90
            target = Quaternion.Euler(0, 0, -((iL2GameDataClient.airspeed / 50f) * 30f) + 30f);
        else
            //over 300
            //for evry 10, move 40, but starts at 300 (150degrees)
            //if 600 -> 270
            //if 700 -> 310            
            target = Quaternion.Euler(0, 0, -((iL2GameDataClient.airspeed / 10f) * 4) - 30);

        airspeedTarget = target;
    }

    void AltitudeTargets()
    {

        altitudeSmallTarget = Quaternion.Euler(0, 0, -(iL2GameDataClient.altitude / 10000f) * 360);

        altitudeLargeTarget = Quaternion.Euler(0, 0, -(iL2GameDataClient.altitude / 1000f) * 360);


    }

    void MmhgTarget()
    {
        mmhgTarget = Quaternion.Euler(0, 0, ((760f - iL2GameDataClient.mmhg) / 100f) * 300);
    }

    void NeedleRotations()
    {
      //  Debug.Log("rotating needles");
        AirspeedNeedleRotation();
        AltitudeNeedleRotations();
        MmhgNeedleRotation();
        TurnAndBank();
    }

    void AirspeedNeedleRotation()
    {
        

        //float d = Mathf.Abs( airspeedTarget.eulerAngles.z - quaternionsAirspeed[0].eulerAngles.z);
       // Debug.Log("air needle");

        airspeedNeedle.transform.rotation = Quaternion.Slerp(airspeedStart, airspeedTarget, (Time.time - lastMessageReceivedTime)/(Time.fixedDeltaTime));
        
        //test needle
        airspeedNeedleTest.transform.rotation = airspeedTarget;

    }

    void AltitudeNeedleRotations()
    {
        altitudeNeedleSmall.transform.rotation = Quaternion.Slerp(altitudeSmallStart, altitudeSmallTarget, (Time.time - lastMessageReceivedTime) / (Time.fixedDeltaTime));
        altitudeNeedleLarge.transform.rotation = Quaternion.Slerp(altitudeLargeStart, altitudeLargeTarget, (Time.time - lastMessageReceivedTime) / (Time.fixedDeltaTime));
    }

    void MmhgNeedleRotation()
    {
        mmhgDial.transform.rotation = Quaternion.Slerp(mmhgStart, mmhgTarget, (Time.time - lastMessageReceivedTime) / (Time.fixedDeltaTime));
    }

    void TurnAndBank()
    {
        //rotate plane
        //clamp roll , in game cockpit stop rotation at just under 90 degrees - this happens when roll rate is ~1.7
        float tempRoll = -iL2GameDataClient.rollRate;
        Mathf.Clamp(tempRoll, -1.7f, 1.7f);
        Quaternion t = Quaternion.Euler(0, 0, tempRoll*turnAndBankRollMultiplier);
        turnPlane.transform.rotation = t;

        //for x rotatin we need to rotate around global x after z rot
        turnPlane.transform.rotation *= Quaternion.Euler(iL2GameDataClient.climbRate * turnAndBankPlaneXrotation, 0, 0);

        //move plane up and down
        turnPlane.transform.localPosition = new Vector3(0, iL2GameDataClient.climbRate*turnAndBankPitchMultiplier, 0);
    }
   
}
