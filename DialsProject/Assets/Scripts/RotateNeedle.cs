using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using UnityEngine;

public class RotateNeedle : MonoBehaviour
{
    public bool testValues;
   

    public BuildControl buildControl;
    public AirplaneData iL2GameDataClient;
    public TCPClient tcpClient;
    public GameObject altitudeNeedleSmall;
    public GameObject altitudeNeedleSmallest;//UK
    public GameObject altitudeNeedleLarge;
    public GameObject mmhgDial;
    public GameObject airspeedNeedle;
    public GameObject airspeedNeedleTest;
    public GameObject turnAndBankNumberTrack;    
    public GameObject turnAndBankPlane;
    public GameObject headingIndicator;
    public GameObject headingIndicatorChild0;
    private GameObject headingIndicatorActive;
    public GameObject turnCoordinatorNeedle;
    public GameObject turnCoordinatorBall;
    //public bool tcpReceived = false; //moved to tcpClient, multiple instances of Rotate Needle for each country, only single instance of tcpclient
    public float lastMessageReceivedTime;//two ways of doing the same thing
    public float previousMessageTime;
    public float maxSpin =1f;
    public float turnAndBankPitchMultiplier = 5f;
    public float turnAndBankRollMultiplier = 5f;
    public float turnAndBankPlaneXMultiplier = 5f;

    //previous frame positions for client prediction -- rotations
    private List<Quaternion> quaternionsAltitudeLarge = new List<Quaternion>() { Quaternion.identity, Quaternion.identity };
    private List<Quaternion> quaternionsAltitudeSmall = new List<Quaternion>() { Quaternion.identity, Quaternion.identity };
    private List<Quaternion> quaternionsAltitudeSmallest = new List<Quaternion>() { Quaternion.identity, Quaternion.identity };
    public List<Quaternion> quaternionsAirspeed = new List<Quaternion>() { Quaternion.identity, Quaternion.identity };
    private List<Quaternion> quaternionsMmhg = new List<Quaternion>() { Quaternion.identity, Quaternion.identity };
    private List<Quaternion> quaternionsTurnAndBankPlane = new List<Quaternion>() { Quaternion.identity, Quaternion.identity };
    private List<Quaternion> quaternionsTurnCoordinatorNeedle = new List<Quaternion>() { Quaternion.identity, Quaternion.identity };
    private List<Quaternion> quaternionsTurnCoordinatorBall = new List<Quaternion>() { Quaternion.identity, Quaternion.identity };
    // -- positions
    private List<Vector3> positionsHeading = new List<Vector3>() { Vector3.zero, Vector3.zero };
    private List<Vector3> positionsTurnAndBankPlane = new List<Vector3>() { Vector3.zero, Vector3.zero };
    private List<Vector3> positionsTurnAndBankNumberTrack = new List<Vector3>() { Vector3.zero, Vector3.zero };

    //  private bool saveForPredictions -- rotations
    public Quaternion airspeedStart; // -- "Start" needed, just use object transform loation/rotation
    public  Quaternion airspeedTarget;
    private Quaternion altitudeLargeStart;
    private Quaternion altitudeLargeTarget;
    private Quaternion altitudeSmallStart;
    private Quaternion altitudeSmallTarget;
    private Quaternion altitudeSmallestStart;
    private Quaternion altitudeSmallestTarget;
    private Quaternion mmhgStart;
    private Quaternion mmhgTarget;
    private Quaternion turnAndBankPlaneRotationStart;
    private Quaternion turnAndBankPlaneRotationTarget;
    private Quaternion turnCoordinatorNeedleStart;
    private Quaternion turnCoordinatorNeedleTarget;
    private Quaternion turnCoordinatorBallStart;
    private Quaternion turnCoordinatorBallTarget;

    // -- positions
    //heading is on a track, we move along the x, we don't rotate
    private Vector3 headingIndicatorStart;
    private Vector3 headingIndicatorTarget;
    private Vector3 turnAndBankPlanePositionStart;
    private Vector3 turnAndBankPlanePositionTarget;
    private Vector3 turnAndBankNumberTrackStart;
    private Vector3 turnAndBankNumberTrackTarget;

    //public bool testPrediction = false;//moved to tcp client


    public float heading;
    public float prevHeading;

    public float trackLength = -15.64f;


    // Start is called before the first frame update
    void Start()
    {

        headingIndicatorActive = headingIndicator;

     
        
    }


    // Update is called once per frame
    void Update()
    {

        if(testValues)
        {
            //this makes the network code think we received a message on the last frame
            //note- need seperate mmhg test


            previousMessageTime = lastMessageReceivedTime;//using?
            lastMessageReceivedTime = Time.time -Time.deltaTime;


            prevHeading = heading;
            iL2GameDataClient.headingPrevious = iL2GameDataClient.heading;
            heading = iL2GameDataClient.heading;

            if (iL2GameDataClient.heading > Mathf.PI*2)
                iL2GameDataClient.heading = 0;

            if (iL2GameDataClient.heading < 0)
                iL2GameDataClient.heading = Mathf.PI * 2;

            ////////////



            //////////

            //PredictRotations();
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
            if (tcpClient.tcpReceived)
            {

                //flag set by tcp client in async thread

                lastMessageReceivedTime = Time.time;
                SetRotationTargets();

                tcpClient.tcpReceived = false;


            }

            //check to see if we need to predict or if we received a new update recently
            else if (Time.time - lastMessageReceivedTime > Time.fixedDeltaTime)//we send and receive on fixed time step
            {
                tcpClient.tcpReceived = false;

                //   Debug.Log("0");
                lastMessageReceivedTime += Time.fixedDeltaTime;

                //  SavePreviousRotations(); -- don't save roation,s we will just use the last one to continue with until real update occurs
                PredictRotations();
            }
        }


        //float difference = quaternionsAirspeed[0].eulerAngles.z - quaternionsAirspeed[1].eulerAngles.z;
        //Debug.Log(difference);


        //best place to put this?
        //flicks between different 2d number tracks to create smooth wrap around
        HeadingIndicatorSwitch();

        NeedleRotations();


    }

    void SavePreviousRotations()
    {
        //used for prediction - save previous position

        //airspeed
        AddRotationToList(quaternionsAirspeed, airspeedNeedle.transform.rotation);

        //altimeter
        AddRotationToList(quaternionsAltitudeSmall, altitudeNeedleSmall.transform.rotation);
        //only UK has the smallest needle
        if (iL2GameDataClient.country == AirplaneData.Country.UK)
            AddRotationToList(quaternionsAltitudeSmallest, altitudeNeedleSmallest.transform.rotation);        
        AddRotationToList(quaternionsAltitudeLarge, altitudeNeedleLarge.transform.rotation);
        AddRotationToList(quaternionsMmhg, mmhgDial.transform.rotation);

        //heading
        AddPositionToList(positionsHeading, headingIndicator.transform.localPosition);

        //turn and bank
        //plane position
        AddPositionToList(positionsTurnAndBankPlane, turnAndBankPlane.transform.localPosition);
        //plane rotations
        AddRotationToList(quaternionsTurnAndBankPlane, turnAndBankPlane.transform.rotation);
        //number track
        AddPositionToList(positionsTurnAndBankNumberTrack, turnAndBankNumberTrack.transform.localPosition);


        //turn co-ord
        AddRotationToList(quaternionsTurnCoordinatorNeedle, turnCoordinatorNeedle.transform.rotation);
        //AddRotationToList(quaternionsTurnCoordinatorBall, turnCoordinatorBall.transform.rotation);

    }


    List<Quaternion> AddRotationToList(List<Quaternion> qList, Quaternion toAdd)
    {
        //method to insert quatenions in to a list of size 2

        //add at start
        qList.Insert(0, toAdd);
        //and cap length, we only need to do simple prediction
        if (qList.Count > 2)
            qList.RemoveAt(2);

        return qList;
    }
    List<Vector3> AddPositionToList(List<Vector3> v3List, Vector3 toAdd)
    {
        //method to insert quatenions in to a list of size 2

        //add at start
        v3List.Insert(0, toAdd);
        //and cap length, we only need to do simple prediction
        if (v3List.Count > 2)
            v3List.RemoveAt(2);

        return v3List;
    }

    void PredictRotations()
    {
        //simulate a tcp event

        if (quaternionsAirspeed.Count < 2)
            return;


        
        Debug.Log("predicting");
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

        //heading
        Vector3 differenceV3 = positionsHeading[0] - positionsHeading[1];
        headingIndicatorStart = headingIndicator.transform.localPosition;
        headingIndicatorTarget = headingIndicator.transform.localPosition + differenceV3;

        //turn and bank
        // - plane position
        differenceV3 = positionsTurnAndBankPlane[0] - positionsTurnAndBankPlane[1];
        turnAndBankPlanePositionStart = turnAndBankPlane.transform.localPosition;
        turnAndBankPlanePositionTarget = turnAndBankPlane.transform.localPosition + differenceV3;
        // - plane rotation
        difference = quaternionsTurnAndBankPlane[0].z - quaternionsTurnAndBankPlane[1].z;
        turnAndBankPlaneRotationStart = turnAndBankPlane.transform.rotation;
        turnAndBankPlaneRotationTarget = turnAndBankPlane.transform.rotation * Quaternion.Euler(0, 0, difference);
        // - number track
        differenceV3 = positionsTurnAndBankNumberTrack[0] - positionsTurnAndBankNumberTrack[1];
        turnAndBankNumberTrackStart = turnAndBankNumberTrack.transform.localPosition;
        turnAndBankNumberTrackTarget = turnAndBankNumberTrack.transform.localPosition + differenceV3;

        //turn co-ordinator
        // - needle
        difference = quaternionsTurnCoordinatorNeedle[0].z - quaternionsTurnCoordinatorNeedle[1].z;
        turnCoordinatorNeedleStart = turnCoordinatorNeedle.transform.rotation;
        turnCoordinatorNeedleTarget = turnCoordinatorNeedle.transform.rotation * Quaternion.Euler(0, 0, difference);
        // - ball
      //  difference = quaternionsTurnCoordinatorBall[0].z - quaternionsTurnCoordinatorBall[1].z;
       // turnCoordinatorBallStart = turnCoordinatorBall.transform.rotation;
       // turnCoordinatorBallTarget = turnCoordinatorBall.transform.rotation * Quaternion.Euler(0, 0, difference);

        //TODO VSI
    }

    public void SetRotationTargets()
    {
        //called when tcp client receives update
        SavePreviousRotations();

        AirspeedTarget();

        AltimeterTargets();

        HeadingTarget();

        TurnAndBankTargets();

        TurnCoordinatorTarget();               
    }

    
    void TurnCoordinatorTarget()
    {
        //RU
        //pendulum needle
        turnCoordinatorNeedleTarget = RussianDials.TurnCoordinatorNeedleTarget(heading, prevHeading, lastMessageReceivedTime, previousMessageTime);

        //ball indicator
        Vector3 velocity = Vector3.zero;// to get
        turnCoordinatorBallTarget = RussianDials.TurnCoordinatorBallTarget(iL2GameDataClient.heading, velocity);
    }

    void TurnAndBankTargets()
    {
        //plane pos
        turnAndBankPlanePositionTarget = RussianDials.TurnAndBankPlanePosition(iL2GameDataClient.pitch, turnAndBankPitchMultiplier);

        //plane rotation
        turnAndBankPlaneRotationTarget = RussianDials.TurnAndBankPlaneRotation(iL2GameDataClient.roll, iL2GameDataClient.pitch, turnAndBankRollMultiplier, turnAndBankPitchMultiplier);

        //number track
        turnAndBankNumberTrackTarget = RussianDials.TurnAndBankNumberTrackPosition(iL2GameDataClient.pitch, turnAndBankPitchMultiplier);
    }

    void HeadingTarget()
    {
        //RU
        headingIndicatorTarget = RussianDials.HeadingIndicatorPosition(iL2GameDataClient.heading);
    }

    void AltimeterTargets()
    {  
        //Altitude
        //set where we rotate from
        AltitudeStarts();
        //find where we are rotating to for each needle

        //only UK has this
        if (iL2GameDataClient.country == AirplaneData.Country.UK)
            altitudeSmallestTarget = AltitudeTargetSmallest(iL2GameDataClient.country, iL2GameDataClient.altitude);

        altitudeSmallTarget = AltitudeTargetSmall(iL2GameDataClient.country, iL2GameDataClient.altitude);
        altitudeLargeTarget = AltitudeTargetLarge(iL2GameDataClient.country, iL2GameDataClient.altitude);

        PressureReferenceTargets();
    }

    void PressureReferenceTargets()
    {   //Pressure //mmhg

        //set where we rotate from
        MmhgStart();

        //set where we are rotating to
        mmhgTarget = AtmosphericPressure(iL2GameDataClient.country, iL2GameDataClient.mmhg);

    }

    void AirspeedTarget()
    {
        // Airspeed
        //set where we rotate from
        AirspeedStart();
        //find where we are rotating to
        airspeedTarget = AirspeedTarget(iL2GameDataClient.country, iL2GameDataClient.airspeed);
    }

    void AirspeedStart()
    {
        airspeedStart = quaternionsAirspeed[0];
    }

    static Quaternion AtmosphericPressure(AirplaneData.Country country, float unit)
    {
        Quaternion target = Quaternion.identity;

        //each country has slightly different dials, we need to work out rotations individually for each
        switch (country)
        {
            case AirplaneData.Country.RU:
                target = RussianDials.MmhgTarget(unit);
                break;

            case AirplaneData.Country.GER:
                target = GermanDials.MmhgTarget(unit);
                break;

            case AirplaneData.Country.US:
                target = USDials.MmhgTarget(unit);
                break;

            case AirplaneData.Country.UK:
                target = UKDials.MmhgTarget(unit);
                break;

            case AirplaneData.Country.ITA:
                target = ITADials.MmhgTarget(unit);
                break;
        }

        return target;

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


        //TODO - don't calculate if dial not present // opto

        AirspeedNeedleRotation();

        AltitudeNeedleRotations();

        MmhgNeedleRotation();
        
        HeadingIndicatorRotation();
        
        TurnAndBankRotations();    

        TurnCoordinatorRotation();
    }

    void AirspeedNeedleRotation()
    {
        //float d = Mathf.Abs( airspeedTarget.eulerAngles.z - quaternionsAirspeed[0].eulerAngles.z);
       // Debug.Log("air needle");

        airspeedNeedle.transform.rotation = Quaternion.Slerp(airspeedStart, airspeedTarget, (Time.time - lastMessageReceivedTime)/(Time.fixedDeltaTime)); //fixed delta is step time for receiving messages - works well
        
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

    void TurnCoordinatorRotation()
    {
        //Needle        
        turnCoordinatorNeedle.transform.rotation = Quaternion.Slerp(turnCoordinatorNeedle.transform.rotation, turnCoordinatorNeedleTarget, (Time.deltaTime - lastMessageReceivedTime));


        //Ball
        //turnCoordinatorBall.transform.rotation = Quaternion.Slerp(turnCoordinatorBallStart, turnCoordinatorBallTarget, (Time.time - lastMessageReceivedTime) / (Time.fixedDeltaTime));

    }

    void HeadingIndicatorSwitch()
    {
        //We have two number tracks and we flick between them when heading crosses over 0. Heading range is 0 to pi * 2, but is cyclictic, so, after 6.24, it goes to 0
        if (iL2GameDataClient.headingPrevious > Mathf.PI && iL2GameDataClient.heading < Math.PI)
        {
            //switch
            if (headingIndicator.activeInHierarchy)
            {
                headingIndicator.SetActive(false);
                headingIndicatorChild0.SetActive(true);

                headingIndicatorActive = headingIndicatorChild0;


                headingIndicatorChild0.transform.localPosition = trackLength * Vector3.right;
            }
            else
            {
                headingIndicator.SetActive(true);
                headingIndicatorChild0.SetActive(false);

                headingIndicatorActive = headingIndicator;

                headingIndicator.transform.localPosition = trackLength * Vector3.right;
            }
        }

        if (iL2GameDataClient.headingPrevious < Mathf.PI && iL2GameDataClient.heading > Math.PI)
        {
            //switch
            if (!headingIndicator.activeInHierarchy)
            {
                headingIndicator.SetActive(false);
                headingIndicatorChild0.SetActive(true);

                headingIndicatorActive = headingIndicatorChild0;

                headingIndicatorChild0.transform.localPosition = -trackLength * Vector3.right;
            }
            else
            {
                headingIndicator.SetActive(true);
                headingIndicatorChild0.SetActive(false);

                headingIndicatorActive = headingIndicator;

                headingIndicator.transform.localPosition = -trackLength * Vector3.right;
            }
        }
    }

    void HeadingIndicatorRotation()
    {
        HeadingIndicatorSwitch();

        //add half a compass (centred on South) but we rotate over North
        Vector3 mod = (trackLength * Vector3.right);
        headingIndicatorActive.transform.localPosition = Vector3.Lerp(headingIndicatorActive.transform.localPosition , headingIndicatorTarget + mod, (Time.time - lastMessageReceivedTime) / Time.fixedDeltaTime);
        

    }

    void TurnAndBankRotations()
    {
        //for x rotatin we need to rotate around global x after z rot
        turnAndBankPlane.transform.rotation = Quaternion.Slerp(turnAndBankPlane.transform.rotation, turnAndBankPlaneRotationTarget, (Time.time - lastMessageReceivedTime) / Time.fixedDeltaTime);

        //move plane up and down
        turnAndBankPlane.transform.localPosition = Vector3.Lerp(turnAndBankPlane.transform.localPosition, turnAndBankPlanePositionTarget, (Time.time - lastMessageReceivedTime)/Time.fixedDeltaTime);

        //number track
        turnAndBankNumberTrack.transform.localPosition = Vector3.Lerp(turnAndBankNumberTrack.transform.localPosition, turnAndBankNumberTrackTarget, (Time.time - lastMessageReceivedTime) / Time.fixedDeltaTime);
    }
   
}
