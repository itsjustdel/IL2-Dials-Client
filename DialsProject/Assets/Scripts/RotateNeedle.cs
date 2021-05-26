using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using UnityEngine;

public class RotateNeedle : MonoBehaviour
{
    public BuildControl buildControl;
    public AirplaneData iL2GameDataClient;
    public TCPClient tcpClient;
    public GameObject altitudeNeedleSmall;
    public GameObject altitudeNeedleSmallest;//UK
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
    private List<Quaternion> quaternionsAltitudeSmallest = new List<Quaternion>() { Quaternion.identity, Quaternion.identity };
    public List<Quaternion> quaternionsAirspeed = new List<Quaternion>() { Quaternion.identity, Quaternion.identity };
    private List<Quaternion> quaternionsMmhg = new List<Quaternion>() { Quaternion.identity, Quaternion.identity };
  //  private bool saveForPredictions;
    public Quaternion airspeedStart;
    public  Quaternion airspeedTarget;
    private Quaternion altitudeLargeStart;
    private Quaternion altitudeLargeTarget;
    private Quaternion altitudeSmallStart;
    private Quaternion altitudeSmallTarget;
    private Quaternion altitudeSmallestStart;
    private Quaternion altitudeSmallestTarget;
    private Quaternion mmhgStart;
    private Quaternion mmhgTarget;

    public bool testPrediction = false;
    public bool testValues = true;



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

        if(testValues)
        {
            //this makes the network code think we received a message on the last frame
            //note- need seperate mmhg test
            lastMessageReceivedTime = -Time.deltaTime;
            SetRotationTargets();
            NeedleRotations();

            return;
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

        //only UK has the smallest needle
        if(iL2GameDataClient.country == AirplaneData.Country.UK)
            AddRotationToList(quaternionsAltitudeSmallest, altitudeNeedleSmallest.transform.rotation);

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


        //smallest altutude (if UK)

        if (iL2GameDataClient.country == AirplaneData.Country.UK)
        {
            difference = quaternionsAltitudeSmallest[0].eulerAngles.z - quaternionsAltitudeSmallest[1].eulerAngles.z;
            //keep moving at client send rate at previous known step        
            //set start point
            altitudeSmallestStart = altitudeNeedleSmallest.transform.rotation;
            //and end point
            altitudeSmallestTarget = altitudeNeedleSmallest.transform.rotation * Quaternion.Euler(0, 0, difference); ;

        }


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

        //set where we rotate from
        AirspeedStart();
        //find where we are rotating to
        airspeedTarget = AirspeedTarget(iL2GameDataClient.country, iL2GameDataClient.airspeed);

        //set where we rotate from
        AltitudeStarts();
        //find where we are rotating to for each needle

        //only UK has this
        if (iL2GameDataClient.country == AirplaneData.Country.UK)
            altitudeSmallestTarget = AltitudeTargetSmallest(iL2GameDataClient.country, iL2GameDataClient.altitude);

        altitudeSmallTarget = AltitudeTargetSmall(iL2GameDataClient.country, iL2GameDataClient.altitude);
        altitudeLargeTarget = AltitudeTargetLarge(iL2GameDataClient.country, iL2GameDataClient.altitude);

        //set where we rotate from
        MmhgStart();
        //set where we are rotating to
        RussianDials.MmhgTarget(iL2GameDataClient.mmhg);

     
    }
    void AirspeedStart()
    {
        airspeedStart = quaternionsAirspeed[0];
    }

    static Quaternion AirspeedTarget(AirplaneData.Country country, float airspeed)
    {
        Quaternion airspeedTarget = Quaternion.identity;

        //each country has slightly different dials, we need to work out rotations individually for each
        switch (country)
        {
            case AirplaneData.Country.RU:
                airspeedTarget = RussianDials.AirspeedTarget(airspeed);
                break;

            case AirplaneData.Country.GER:
                airspeedTarget = GermanDials.AirspeedTarget(airspeed);
                break;

            case AirplaneData.Country.US:
                airspeedTarget = USDials.AirspeedTarget(airspeed);
                break;

            case AirplaneData.Country.UK:
                airspeedTarget = UKDials.AirspeedTarget(airspeed);
                break;

            case AirplaneData.Country.ITA:
                airspeedTarget = ITADials.AirspeedTarget(airspeed);
                break;
        }
         

        return airspeedTarget;
    }

    static Quaternion AltitudeTargetLarge(AirplaneData.Country country, float altitude)
    {
        Quaternion target = Quaternion.identity;

        //each country has slightly different dials, we need to work out rotations individually for each
        switch (country)
        {
            case AirplaneData.Country.RU:
                
                target = RussianDials.AltitudeTargetLarge(altitude);
                break;

            case AirplaneData.Country.GER:
                target = GermanDials.AltitudeTargetLarge(altitude);
                break;

            case AirplaneData.Country.US:
                target= USDials.AltitudeTargetLarge(altitude);
                break;

            case AirplaneData.Country.UK:
                target = UKDials.AltitudeTargetLarge(altitude);
                break;

            case AirplaneData.Country.ITA:
                target = ITADials.AltitudeTargetLarge(altitude);
                break;
        }


        return target;
    }

    static Quaternion AltitudeTargetSmall(AirplaneData.Country country, float altitude)
    {
        Quaternion target = Quaternion.identity;

        //each country has slightly different dials, we need to work out rotations individually for each
        switch (country)
        {
            case AirplaneData.Country.RU:

                target = RussianDials.AltitudeTargetSmall(altitude);
                break;

            case AirplaneData.Country.GER:
                target = GermanDials.AltitudeTargetSmall(altitude);
                break;

            case AirplaneData.Country.US:
                target = USDials.AltitudeTargetSmall(altitude);
                break;

            case AirplaneData.Country.UK:
                target = UKDials.AltitudeTargetSmall(altitude);
                break;

            case AirplaneData.Country.ITA:
                target = ITADials.AltitudeTargetSmall(altitude);
                break;
        }


        return target;
    }

    static Quaternion AltitudeTargetSmallest(AirplaneData.Country country, float altitude)
    {
        Quaternion target = Quaternion.identity;

        //each country has slightly different dials, we need to work out rotations individually for each
        switch (country)
        {
            //only UK has smallest dial

            case AirplaneData.Country.UK:
                target = UKDials.AltitudeTargetSmallest(altitude);
                break;
        }


        return target;
    }



    void AltitudeStarts()
    {
        altitudeSmallStart = quaternionsAltitudeSmall[0];

        //UK has small needle
        if (iL2GameDataClient.country == AirplaneData.Country.UK)        
            altitudeSmallestStart = quaternionsAltitudeSmallest[0];       


        altitudeLargeStart = quaternionsAltitudeLarge[0];
    }

    void MmhgStart()
    {
        mmhgStart = quaternionsMmhg[0];
    }



    void NeedleRotations()
    {
      //  Debug.Log("rotating needles");
        AirspeedNeedleRotation();
        AltitudeNeedleRotations();
        MmhgNeedleRotation();

        if (!buildControl.freeVersion)
        {
            TurnAndBank();
        }
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
        //only UK has the smallest needle
        if (iL2GameDataClient.country == AirplaneData.Country.UK)
            altitudeNeedleSmallest.transform.rotation = Quaternion.Slerp(altitudeSmallestStart, altitudeSmallestTarget, (Time.time - lastMessageReceivedTime) / (Time.fixedDeltaTime));


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
