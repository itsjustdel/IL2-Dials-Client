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
    public GameObject turnAndBankNumberTrack;    
    public GameObject turnAndBankPlane;
    public GameObject headingIndicator;
  //  public GameObject headingIndicatorChild0;
  //  public GameObject headingIndicator;
    public GameObject turnCoordinatorNeedle;
    public GameObject turnCoordinatorBall;
    public GameObject vsiNeedle;
    public GameObject repeaterCompassFace;
    public GameObject artificialHorizonPlane;//ITA
    public GameObject artificialHorizonBackground;
    public GameObject artificialHorizonChevron;
    public GameObject turnAndBankBall;
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
    private List<Quaternion> quaternionsVSI = new List<Quaternion>() { Quaternion.identity, Quaternion.identity };
    private List<Quaternion> quaternionsRepeaterCompass = new List<Quaternion>() { Quaternion.identity, Quaternion.identity };    
    private List<Quaternion> quaternionsArtificialHorizon = new List<Quaternion>() { Quaternion.identity, Quaternion.identity };
    private List<Quaternion> quaternionsArtificialHorizonChevron = new List<Quaternion>() { Quaternion.identity, Quaternion.identity };
    private List<Quaternion> quaternionsTurnAndBankBall = new List<Quaternion>() { Quaternion.identity, Quaternion.identity };
    
    // -- positions
    private List<Vector3> positionsHeading = new List<Vector3>() { Vector3.zero, Vector3.zero };
    private List<Vector3> positionsTurnAndBankPlane = new List<Vector3>() { Vector3.zero, Vector3.zero };
    private List<Vector3> positionsTurnAndBankNumberTrack = new List<Vector3>() { Vector3.zero, Vector3.zero };
    private List<Vector3> positionsArtificialHorizonBomber = new List<Vector3>() { Vector3.zero, Vector3.zero };

    //  private bool saveForPredictions -- rotations
    
    //public Quaternion airspeedStart; // -- "Start" needed, just use object transform loation/rotation
    public  Quaternion airspeedTarget;
    //private Quaternion altitudeLargeStart;
    private Quaternion altitudeLargeTarget;
    //private Quaternion altitudeSmallStart;
    private Quaternion altitudeSmallTarget;
    //private Quaternion altitudeSmallestStart;
    private Quaternion altitudeSmallestTarget;
    //private Quaternion mmhgStart;
    private Quaternion mmhgTarget;
    //private Quaternion turnAndBankPlaneRotationStart;
    private Quaternion turnAndBankPlaneRotationTarget;
    //private Quaternion turnCoordinatorNeedleStart;
    private Quaternion turnCoordinatorNeedleTarget;
    //private Quaternion turnCoordinatorBallStart;
    private Quaternion turnCoordinatorBallTarget;
    private Quaternion vsiNeedleTarget;
    private Quaternion repeaterCompassTarget;
    private Quaternion artificialHorizonRotationTarget;
    private Quaternion artificialHorizonChevronTarget;
    private Quaternion turnAndBankBallTarget;

    // -- positions
    //heading is on a track, we move along the x, we don't rotate
    //private Vector3 headingIndicatorStart;
    private Vector3 headingIndicatorTarget;
    //private Vector3 turnAndBankPlanePositionStart;
    private Vector3 turnAndBankPlanePositionTarget;
    //private Vector3 turnAndBankNumberTrackStart;
    private Vector3 turnAndBankNumberTrackTarget;
    private Vector3 artificialHorizonPositionTarget;

    // -- modifiers
    public float trackLength = -15.64f;
    public float trackLengthForSwitch = 200.3f;
    private bool headingIndicatorCrossoverRight = false;
    private bool headingIndicatorCrossoverLeft = false;
    private bool headingIndicatorJump = false;



    public float turnCoordinaterNeedleMod = 1f;
    public float turnCoordinaterBallMod = 1f;
    public float turnCoordinaterMultiplier = 20f;
    public float artificialHorizonBomberRollMod = 1f;
    public float artificialHorizonBomberMultiplier = 20f;
    public float turnAndBankBallMultiplier = 1f;

    public AnimationCurve animationCurveVSI;
  
    private bool headingIndicatorTest;

    // Start is called before the first frame update
    void Start()
    {
        


    }


    // Update is called once per frame
    void Update()
    {

        

        if (testValues)
        {
            //this makes the network code think we received a message on the last frame
            //note- need seperate mmhg test
            //if (iL2GameDataClient.heading > .2f && iL2GameDataClient.heading < 6.2f)
            //headingIndicatorShift = !headingIndicatorShift;
            bool autoMove = false;
            if (autoMove)
            {


                float h = iL2GameDataClient.heading;
                float a = .4f;
                float b = 6f;

                //swithc direction
                if (h > a && h < b)
                {
                    //                Debug.Log(headingIndicatorShift);
                    headingIndicatorTest = !headingIndicatorTest;
                }
                float speed = .001f;
                if (headingIndicatorTest)
                    iL2GameDataClient.heading += speed;
                else
                    iL2GameDataClient.heading -= speed;

            }
            previousMessageTime = lastMessageReceivedTime;//using?
            lastMessageReceivedTime = Time.time -Time.deltaTime;


            iL2GameDataClient.headingPrevious= iL2GameDataClient.heading;
           
            //iL2GameDataClient.heading = iL2GameDataClient.heading;

            
            if (iL2GameDataClient.heading > Mathf.PI * 2)
            {
                iL2GameDataClient.heading -= Mathf.PI * 2;
               // iL2GameDataClient.headingPrevious -= Mathf.PI * 2;

             //   headingIndicatorCrossoverLeft = true;
            }

            else if (iL2GameDataClient.heading < 0)
            {
                iL2GameDataClient.heading += Mathf.PI * 2;
              //  iL2GameDataClient.headingPrevious += Mathf.PI * 2;

              //  headingIndicatorCrossoverRight = true;
            }

            

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
                //save two steps of time
                
                previousMessageTime = lastMessageReceivedTime;
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
        //HeadingIndicatorSwitch();

        NeedleRotations();


    }

    

    void SavePreviousRotationsAndPositions()
    {
        //used for prediction - save previous position

        //airspeed
        AddRotationToList(quaternionsAirspeed, airspeedNeedle.transform.rotation);

        //altimeter
        AddRotationToList(quaternionsAltitudeSmall, altitudeNeedleSmall.transform.rotation);
        //only UK / US has the smallest needle
        if (altitudeNeedleSmallest != null)
            AddRotationToList(quaternionsAltitudeSmallest, altitudeNeedleSmallest.transform.rotation);

        AddRotationToList(quaternionsAltitudeLarge, altitudeNeedleLarge.transform.rotation);
        AddRotationToList(quaternionsMmhg, mmhgDial.transform.rotation);

        //heading
        AddPositionToList(positionsHeading, headingIndicator.transform.localPosition );

        //turn and bank
        if (turnAndBankPlane != null)
        {
            //plane position
            AddPositionToList(positionsTurnAndBankPlane, turnAndBankPlane.transform.localPosition);

            //plane rotations
            AddRotationToList(quaternionsTurnAndBankPlane, turnAndBankPlane.transform.rotation);
        }

        //number track
        if(turnAndBankNumberTrack != null)
            AddPositionToList(positionsTurnAndBankNumberTrack, turnAndBankNumberTrack.transform.localPosition);

        //turn co-ord
        AddRotationToList(quaternionsTurnCoordinatorNeedle, turnCoordinatorNeedle.transform.rotation);
        AddRotationToList(quaternionsTurnCoordinatorBall, turnCoordinatorBall.transform.rotation);

        //vsi
        AddRotationToList(quaternionsVSI, vsiNeedle.transform.rotation);

        //ger repeater compass
        if (iL2GameDataClient.country == AirplaneData.Country.GER)
            AddRotationToList(quaternionsRepeaterCompass, repeaterCompassFace.transform.rotation);

        //ger bomber horizon
        if (iL2GameDataClient.country == AirplaneData.Country.GER)
            AddRotationToList(quaternionsArtificialHorizon, artificialHorizonBackground.transform.rotation);

        //ger turn and bank ball
        if (iL2GameDataClient.country == AirplaneData.Country.GER)
            AddRotationToList(quaternionsTurnAndBankBall, turnAndBankBall.transform.rotation);


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

     //   Debug.Log("predicting");        
        

        //airspeed - prediction doesn't take in to account gearing on speedometer
        //last known difference
        float difference = quaternionsAirspeed[0].eulerAngles.z - quaternionsAirspeed[1].eulerAngles.z;        
        //keep moving at client send rate at previous known step        
        //set start point
        //airspeedStart =  airspeedNeedle.transform.rotation;
        //and end point
        airspeedTarget = airspeedNeedle.transform.rotation * Quaternion.Euler(0, 0, difference); ;

        //altitude
        //last known difference
        difference = quaternionsAltitudeLarge[0].eulerAngles.z - quaternionsAltitudeLarge[1].eulerAngles.z;
        //keep moving at client send rate at previous known step
        //set start point
       // altitudeLargeStart = altitudeNeedleLarge.transform.rotation;
        //and end point
        altitudeLargeTarget = altitudeNeedleLarge.transform.rotation * Quaternion.Euler(0, 0, difference); ;

        //small altutude
        difference = quaternionsAltitudeSmall[0].eulerAngles.z - quaternionsAltitudeSmall[1].eulerAngles.z;
        //keep moving at client send rate at previous known step        
        //set start point
       // altitudeSmallStart = altitudeNeedleSmall.transform.rotation;
        //and end point
        altitudeSmallTarget = altitudeNeedleSmall.transform.rotation * Quaternion.Euler(0, 0, difference); ;


        //smallest altutude (if UK)

        if (iL2GameDataClient.country == AirplaneData.Country.UK)
        {
            difference = quaternionsAltitudeSmallest[0].eulerAngles.z - quaternionsAltitudeSmallest[1].eulerAngles.z;
            //keep moving at client send rate at previous known step        
            //set start point
        //    altitudeSmallestStart = altitudeNeedleSmallest.transform.rotation;
            //and end point
            altitudeSmallestTarget = altitudeNeedleSmallest.transform.rotation * Quaternion.Euler(0, 0, difference); ;

        }

        difference = quaternionsMmhg[0].eulerAngles.z - quaternionsMmhg[1].eulerAngles.z;
        //keep moving at client send rate at previous known step        
        //set start point
      //  mmhgStart = mmhgDial.transform.rotation;
        //and end point
        mmhgTarget = mmhgDial.transform.rotation * Quaternion.Euler(0, 0, difference);

        //heading
        Vector3 differenceV3 = positionsHeading[0] - positionsHeading[1];
      //  headingIndicatorStart = headingIndicator.transform.localPosition;
        headingIndicatorTarget = headingIndicator.transform.localPosition + differenceV3;
       
        //turn and bank
        // - plane position
        if (turnAndBankPlane != null)
        {
            differenceV3 = positionsTurnAndBankPlane[0] - positionsTurnAndBankPlane[1];
          //  turnAndBankPlanePositionStart = turnAndBankPlane.transform.localPosition;
            turnAndBankPlanePositionTarget = turnAndBankPlane.transform.localPosition + differenceV3;
            // - plane rotation
            difference = quaternionsTurnAndBankPlane[0].z - quaternionsTurnAndBankPlane[1].z;
           // turnAndBankPlaneRotationStart = turnAndBankPlane.transform.rotation;
            turnAndBankPlaneRotationTarget = turnAndBankPlane.transform.rotation * Quaternion.Euler(0, 0, difference);
            // - number track
            if (turnAndBankNumberTrack != null)
            {
                differenceV3 = positionsTurnAndBankNumberTrack[0] - positionsTurnAndBankNumberTrack[1];
          //      turnAndBankNumberTrackStart = turnAndBankNumberTrack.transform.localPosition;
                turnAndBankNumberTrackTarget = turnAndBankNumberTrack.transform.localPosition + differenceV3;
            }
        }
        if (turnCoordinatorNeedle != null)
        {
            //turn co-ordinator
            // - needle
            difference = quaternionsTurnCoordinatorNeedle[0].z - quaternionsTurnCoordinatorNeedle[1].z;
         //   turnCoordinatorNeedleStart = turnCoordinatorNeedle.transform.rotation;
            turnCoordinatorNeedleTarget = turnCoordinatorNeedle.transform.rotation * Quaternion.Euler(0, 0, difference);
            // - ball
            difference = quaternionsTurnCoordinatorBall[0].z - quaternionsTurnCoordinatorBall[1].z;
          //  turnCoordinatorBallStart = turnCoordinatorBall.transform.rotation;
            turnCoordinatorBallTarget = turnCoordinatorBall.transform.rotation * Quaternion.Euler(0, 0, difference);
        }

        if (vsiNeedle != null)
        {
            //VSI
            difference = quaternionsVSI[0].z - quaternionsVSI[1].z;
            vsiNeedleTarget = vsiNeedle.transform.rotation * Quaternion.Euler(0, 0, difference);
        }

        //ger repeater
        if (iL2GameDataClient.country == AirplaneData.Country.GER)
        {
            difference = quaternionsRepeaterCompass[0].z - quaternionsRepeaterCompass[1].z;
            repeaterCompassTarget = repeaterCompassFace.transform.rotation * Quaternion.Euler(0, 0, difference);
        }

        //artificial horizon
        //background        
        //rotation
        if (artificialHorizonBackground != null)
        {
            difference = quaternionsArtificialHorizon[0].z - quaternionsArtificialHorizon[1].z;
            artificialHorizonRotationTarget = artificialHorizonBackground.transform.rotation * Quaternion.Euler(0, 0, difference);
            //pos
            differenceV3 = positionsArtificialHorizonBomber[0] - positionsArtificialHorizonBomber[1];
            artificialHorizonPositionTarget = artificialHorizonBackground.transform.localPosition + differenceV3;

        }

        //chevron
        if (artificialHorizonChevron != null)
        {
            difference = quaternionsArtificialHorizonChevron[0].z - quaternionsArtificialHorizonChevron[1].z;
            artificialHorizonChevronTarget = artificialHorizonChevron.transform.rotation * Quaternion.Euler(0, 0, difference);
        }
        if (turnAndBankBall != null)
        {
            //ger turn and bank ball
            difference = quaternionsTurnAndBankBall[0].z - quaternionsTurnAndBankBall[1].z;
            turnAndBankBallTarget = turnAndBankBall.transform.rotation * Quaternion.Euler(0, 0, difference);
        }
     
    }

    public void SetRotationTargets()
    {
        //called when tcp client receives update
        SavePreviousRotationsAndPositions();

        AirspeedTarget();

        AltimeterTargets();

        HeadingTarget(iL2GameDataClient.country);

        TurnAndBankTargets(iL2GameDataClient.country);

        TurnCoordinatorTarget(iL2GameDataClient.country);

        VSITarget(iL2GameDataClient.country);

        RepeaterCompassTarget(iL2GameDataClient.country);

        ArtificialHorizonTargets(iL2GameDataClient.country);
    }

    void ArtificialHorizonTargets(AirplaneData.Country country)
    {
        switch (country)
        {
            //no RU

            //GER
            case (AirplaneData.Country.GER):
                //rotation // roll
                artificialHorizonRotationTarget = GermanDials.ArtificialHorizon(iL2GameDataClient.roll, artificialHorizonBomberRollMod);

                //position  
                artificialHorizonPositionTarget = GermanDials.ArtificialHorizonPosition(iL2GameDataClient.pitch, artificialHorizonBomberMultiplier);

                break;


            //US
            case (AirplaneData.Country.US):
                //rotation // roll
                //adjustemnts for certain planes
                
                    artificialHorizonRotationTarget = USDials.ArtificialHorizonRotation(iL2GameDataClient.roll, artificialHorizonBomberRollMod);


                //position  /pitch
                
                artificialHorizonPositionTarget = USDials.ArtificialHorizonPosition(iL2GameDataClient.pitch, artificialHorizonBomberMultiplier);
                

                //chevron
                artificialHorizonChevronTarget = USDials.ArtificialHorizonChevronRotation(iL2GameDataClient.roll, artificialHorizonBomberRollMod);

                
                break;

            case (AirplaneData.Country.UK):

                artificialHorizonRotationTarget = UKDials.ArtificialHorizonRotation(iL2GameDataClient.roll, artificialHorizonBomberRollMod);

                //position  
                artificialHorizonPositionTarget = UKDials.ArtificialHorizonPosition(iL2GameDataClient.pitch, artificialHorizonBomberMultiplier);
                //chevron
                artificialHorizonChevronTarget = UKDials.ArtificialHorizonChevronRotation(iL2GameDataClient.roll, artificialHorizonBomberRollMod);
                break;

            case (AirplaneData.Country.ITA):

                artificialHorizonRotationTarget = ITADials.ArtificialHorizonRotation(iL2GameDataClient.roll, artificialHorizonBomberRollMod);

                //position  
                artificialHorizonPositionTarget = ITADials.ArtificialHorizonPosition(iL2GameDataClient.pitch, artificialHorizonBomberMultiplier);
                break;
        }
    }

    void RepeaterCompassTarget(AirplaneData.Country country)
    {
        
        if (country == AirplaneData.Country.GER)
            repeaterCompassTarget = GermanDials.RepeaterCompassTarget(iL2GameDataClient.heading);

        else if (country == AirplaneData.Country.US)
            repeaterCompassTarget = USDials.RepeaterCompassTarget(iL2GameDataClient.heading);


    }
    
    void VSITarget(AirplaneData.Country country)
    {
        switch (country)
        {
            case (AirplaneData.Country.RU):
                if(iL2GameDataClient.planeAttributes.vsiLarge)
                    vsiNeedleTarget = RussianDials.VerticalSpeedTargetLarge(iL2GameDataClient.verticalSpeed);
                
                else if(iL2GameDataClient.planeAttributes.vsiSmall)
                    vsiNeedleTarget = RussianDials.VerticalSpeedTargetSmall(iL2GameDataClient.verticalSpeed);

                break;

            case (AirplaneData.Country.GER):

                if (iL2GameDataClient.planeAttributes.vsiLarge)
                    vsiNeedleTarget = GermanDials.VerticalSpeedTargetLarge(iL2GameDataClient.verticalSpeed);
                
                else if (iL2GameDataClient.planeAttributes.vsiSmall)
                    vsiNeedleTarget = GermanDials.VerticalSpeedTargetSmall(iL2GameDataClient.verticalSpeed);

                break;

            //these countries only have one vsi (so far)
            case (AirplaneData.Country.US):
                vsiNeedleTarget = USDials.VerticalSpeedTarget(iL2GameDataClient.verticalSpeed,animationCurveVSI);
                break;

            case (AirplaneData.Country.UK):
                vsiNeedleTarget = UKDials.VerticalSpeedTarget(iL2GameDataClient.verticalSpeed);
                break;

            case (AirplaneData.Country.ITA):
                vsiNeedleTarget = ITADials.VerticalSpeedTarget(iL2GameDataClient.verticalSpeed);
                break;

        }
    }

    void TurnCoordinatorTarget(AirplaneData.Country country)
    {

        switch (country)
        {
            case (AirplaneData.Country.RU):
                //RU
                //pendulum needle
                turnCoordinatorNeedleTarget = RussianDials.TurnCoordinatorNeedleTarget(iL2GameDataClient.turnCoordinatorNeedle, turnCoordinaterNeedleMod);

                //ball indicator                
                turnCoordinatorBallTarget = RussianDials.TurnCoordinatorBallTarget(iL2GameDataClient.turnCoordinatorBall);
                break;

            case (AirplaneData.Country.GER):
                //RU
                //pendulum needle
                turnCoordinatorNeedleTarget = GermanDials.TurnCoordinatorNeedleTarget(iL2GameDataClient.turnCoordinatorNeedle);

                //ball indicator                
                turnCoordinatorBallTarget = GermanDials.TurnCoordinatorBallTarget(iL2GameDataClient.turnCoordinatorBall,turnCoordinaterBallMod);
                break;

            case (AirplaneData.Country.US):
                if(iL2GameDataClient.planeType == "A-20B")
                    turnCoordinatorNeedleTarget = USDials.TurnCoordinatorNeedleTarget(iL2GameDataClient.turnCoordinatorNeedle, true);
                else 
                    turnCoordinatorNeedleTarget = USDials.TurnCoordinatorNeedleTarget(iL2GameDataClient.turnCoordinatorNeedle, false);

                //ball indicator                
                turnCoordinatorBallTarget = USDials.TurnCoordinatorBallTarget(iL2GameDataClient.turnCoordinatorBall, turnCoordinaterBallMod);
                break;


            case (AirplaneData.Country.UK):
                turnCoordinatorNeedleTarget = UKDials.TurnCoordinatorNeedleTarget(iL2GameDataClient.turnCoordinatorNeedle, turnCoordinaterNeedleMod);

                //second needle        
                turnCoordinatorBallTarget = UKDials.TurnCoordinatorBallTarget(iL2GameDataClient.turnCoordinatorBall, turnCoordinaterBallMod);
                break;


            case (AirplaneData.Country.ITA):
                turnCoordinatorNeedleTarget = ITADials.TurnCoordinatorNeedleTarget(iL2GameDataClient.turnCoordinatorNeedle);

                //second needle        
                turnCoordinatorBallTarget = ITADials.TurnCoordinatorBallTarget(iL2GameDataClient.turnCoordinatorBall, turnCoordinaterBallMod);
                break;

        }
       
    }

    //turn and bank is dial with artifical horizon and slip together
    void TurnAndBankTargets(AirplaneData.Country country)
    {
        //plane or background pos

        switch (country)
        {
            case (AirplaneData.Country.RU): 
                //note russian is quite different - more like an artifical horizon with plane moving instead of horizon
                turnAndBankPlanePositionTarget = RussianDials.TurnAndBankPlanePosition(iL2GameDataClient.pitch, turnAndBankPitchMultiplier);

                //plane or background rotation
                turnAndBankPlaneRotationTarget = RussianDials.TurnAndBankPlaneRotation(iL2GameDataClient.roll, iL2GameDataClient.pitch, turnAndBankRollMultiplier,turnAndBankPlaneXMultiplier);

                turnAndBankNumberTrackTarget = RussianDials.TurnAndBankNumberTrackPosition(iL2GameDataClient.pitch, turnAndBankPitchMultiplier);

                break;

                // with slip?
            case (AirplaneData.Country.GER):
                turnAndBankPlanePositionTarget = GermanDials.TurnAndBankPlanePosition(iL2GameDataClient.pitch, turnAndBankPitchMultiplier);

                turnAndBankPlaneRotationTarget = GermanDials.TurnAndBankPlaneRotation(iL2GameDataClient.roll, iL2GameDataClient.pitch, turnAndBankRollMultiplier, turnAndBankRollMultiplier);

                turnAndBankBallTarget = GermanDials.TurnAndBankBallTarget(iL2GameDataClient.turnCoordinatorBall, turnAndBankBallMultiplier);

                break;

        }


        
    }

    void HeadingTarget(AirplaneData.Country country)
    {
        
        switch (country)
        {
            case (AirplaneData.Country.RU):
                headingIndicatorTarget = RussianDials.HeadingIndicatorPosition(iL2GameDataClient.heading,trackLength);
                break;

            case (AirplaneData.Country.GER):
                headingIndicatorTarget = GermanDials.HeadingIndicatorPosition(iL2GameDataClient.heading,trackLength);
                break;

            case (AirplaneData.Country.US):
                headingIndicatorTarget = USDials.HeadingIndicatorPosition(iL2GameDataClient.heading, trackLength);
                break;

            case (AirplaneData.Country.UK):
                headingIndicatorTarget = UKDials.HeadingIndicatorPosition(iL2GameDataClient.heading ,trackLength);
                break;

            case (AirplaneData.Country.ITA):
                headingIndicatorTarget = ITADials.HeadingIndicatorPosition(iL2GameDataClient.heading, trackLength);
                break;
        }
        
    }

    void AltimeterTargets()
    {  
        //Altitude
        //set where we rotate from
        //AltitudeStarts();
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
        //MmhgStart();

        //set where we are rotating to
        mmhgTarget = AtmosphericPressure(iL2GameDataClient.country, iL2GameDataClient.mmhg);

    }

    void AirspeedTarget()
    {
        // Airspeed
        //set where we rotate from
        //AirspeedStart();
        //find where we are rotating to
        airspeedTarget = AirspeedTarget(iL2GameDataClient.country, iL2GameDataClient.airspeed);
    }

    void AirspeedStart()
    {
        //airspeedStart = quaternionsAirspeed[0];
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

    /*
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
    */

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

        VSIRotation();

        RepeaterCompassRotation();

        ArtificialHorizonTranslations();
    }

    void AirspeedNeedleRotation()
    {
        //float d = Mathf.Abs( airspeedTarget.eulerAngles.z - quaternionsAirspeed[0].eulerAngles.z);
       // Debug.Log("air needle");

        airspeedNeedle.transform.rotation = Quaternion.Slerp(airspeedNeedle.transform.rotation, airspeedTarget, Time.fixedDeltaTime); 
        
    }

    void AltitudeNeedleRotations()
    {
        //only UK has the smallest needle
        if (iL2GameDataClient.country == AirplaneData.Country.UK)
            altitudeNeedleSmallest.transform.rotation = Quaternion.Slerp(altitudeNeedleSmallest.transform.rotation, altitudeSmallestTarget, Time.fixedDeltaTime);


        altitudeNeedleSmall.transform.rotation = Quaternion.Slerp(altitudeNeedleSmall.transform.rotation, altitudeSmallTarget, Time.fixedDeltaTime);
        altitudeNeedleLarge.transform.rotation = Quaternion.Slerp(altitudeNeedleLarge.transform.rotation, altitudeLargeTarget, Time.fixedDeltaTime);
    }

    void MmhgNeedleRotation()
    {
        mmhgDial.transform.rotation = Quaternion.Slerp(mmhgDial.transform.rotation, mmhgTarget, (Time.time - lastMessageReceivedTime) / (Time.fixedDeltaTime));
    }

    void TurnCoordinatorRotation()
    {
        //Needle        
        turnCoordinatorNeedle.transform.rotation = Quaternion.Slerp(turnCoordinatorNeedle.transform.rotation, turnCoordinatorNeedleTarget, Time.fixedDeltaTime);//already used time difference in calculation


        //Ball
        turnCoordinatorBall.transform.rotation = Quaternion.Slerp(turnCoordinatorBall.transform.rotation, turnCoordinatorBallTarget,  (Time.fixedDeltaTime)); //smoother without time.time - last message time
    }

    bool HeadingIndicatorSwitch()
    {
        //check for when heading rolls over 0
        if (iL2GameDataClient.headingPrevious - iL2GameDataClient.heading > Mathf.PI)
        {
            //adjust previous heading so that's its close to new heading
            iL2GameDataClient.headingPrevious -= Mathf.PI * 2;

            //move track position without smoothing to other side
            Vector3 _trackLength = Vector3.right * trackLength * 2 * transform.localScale.x;
            headingIndicator.transform.localPosition += _trackLength;

            //necessary?
            for (int i = 0; i < 2; i++)
            {
                positionsHeading[i] += _trackLength;
            }

            /*
            Debug.Log("A");

            Debug.Log("heading = " + iL2GameDataClient.heading);
            Debug.Log("heading Prev = " + iL2GameDataClient.headingPrevious);
            */
        }
        //check for rolling over 0 from the oter direction
        else if (iL2GameDataClient.heading - iL2GameDataClient.headingPrevious > Mathf.PI)
        {
            //  iL2GameDataClient.heading += Mathf.PI * 2;
            iL2GameDataClient.headingPrevious += Mathf.PI * 2;

            //move track position without smoothing to other side
            Vector3 _trackLength = Vector3.right * trackLength * 2 * transform.localScale.x;
            headingIndicator.transform.localPosition -= _trackLength;

            //necessary?
            for (int i = 0; i < 2; i++)
            {
                positionsHeading[i] -= _trackLength;
            }

            /*
            Debug.Log("B");

            Debug.Log("heading = " + iL2GameDataClient.heading);
            Debug.Log("heading Prev = " + iL2GameDataClient.headingPrevious);
            */
        }



        //now rework target
        HeadingTarget(iL2GameDataClient.country);


        return false;
    }
    void HeadingIndicatorRotation()
    {
        HeadingIndicatorSwitch();

        //
                headingIndicator.transform.localPosition = Vector3.Lerp(headingIndicator.transform.localPosition, headingIndicatorTarget, Time.fixedDeltaTime);

        //headingIndicator.transform.localPosition = headingIndicatorTarget;

        Debug.DrawLine(headingIndicator.transform.localPosition, headingIndicator.transform.localPosition + Vector3.up*100);
        Debug.DrawLine(headingIndicatorTarget, headingIndicatorTarget + Vector3.up * 100, Color.red);


    }

    void TurnAndBankRotations()
    {
        if (turnAndBankPlane != null)
            {
        
            //for x rotatin we need to rotate around global x after z rot
            turnAndBankPlane.transform.rotation = Quaternion.Slerp(turnAndBankPlane.transform.rotation, turnAndBankPlaneRotationTarget, (Time.time - lastMessageReceivedTime) / Time.fixedDeltaTime);//?

            //move plane up and down
            turnAndBankPlane.transform.localPosition = Vector3.Lerp(turnAndBankPlane.transform.localPosition, turnAndBankPlanePositionTarget, (Time.time - lastMessageReceivedTime) / Time.fixedDeltaTime);

        }

        //number track - only russian
        if(turnAndBankNumberTrack != null)
            turnAndBankNumberTrack.transform.localPosition = Vector3.Lerp(turnAndBankNumberTrack.transform.localPosition, turnAndBankNumberTrackTarget, (Time.time - lastMessageReceivedTime) / Time.fixedDeltaTime);

        //ball -- only german
        if(turnAndBankBall != null)
        {
            turnAndBankBall.transform.rotation = Quaternion.Slerp(turnAndBankBall.transform.rotation, turnAndBankBallTarget, Time.fixedDeltaTime);
        }
    }

    void VSIRotation()
    {
        vsiNeedle.transform.rotation = Quaternion.Slerp(vsiNeedle.transform.rotation, vsiNeedleTarget,  Time.fixedDeltaTime);
    }

    void RepeaterCompassRotation()
    {
        if(repeaterCompassFace != null)
            repeaterCompassFace.transform.rotation = Quaternion.Slerp(repeaterCompassFace.transform.rotation, repeaterCompassTarget, Time.fixedDeltaTime);
    }

    void ArtificialHorizonTranslations()
    {
        if (artificialHorizonBackground == null)
            return;

        ArtificialHorizonPosition();
        ArtificialHorizonRotation();
        ArtificialHorizonChevronRotation();
    }

    void ArtificialHorizonRotation()
    {
        if (artificialHorizonPlane != null)
            //ITA plane rotates whilst background doesn't
            artificialHorizonPlane.transform.rotation = Quaternion.Slerp(artificialHorizonPlane.transform.rotation, artificialHorizonRotationTarget, Time.fixedDeltaTime);

        else
            artificialHorizonBackground.transform.rotation = Quaternion.Slerp(artificialHorizonBackground.transform.rotation, artificialHorizonRotationTarget, Time.fixedDeltaTime);
    }

    void ArtificialHorizonPosition()
    {
        artificialHorizonBackground.transform.localPosition = Vector3.Lerp(artificialHorizonBackground.transform.localPosition, artificialHorizonPositionTarget, Time.fixedDeltaTime);

        //clamp plane to background
        if(artificialHorizonPlane != null)
            artificialHorizonPlane.transform.localPosition = artificialHorizonBackground.transform.localPosition;
    }

    void ArtificialHorizonChevronRotation()
    {
        if(artificialHorizonChevron != null)
            artificialHorizonChevron.transform.rotation = Quaternion.Slerp(artificialHorizonChevron.transform.rotation, artificialHorizonChevronTarget, Time.fixedDeltaTime);
    }
}
