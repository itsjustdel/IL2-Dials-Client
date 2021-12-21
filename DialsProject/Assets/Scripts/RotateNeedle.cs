using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using UnityEngine;

public class RotateNeedle : MonoBehaviour
{
    //
    public BuildControl buildControl;
    public AirplaneData airplaneData;
    public TCPClient tcpClient;

    public GameObject altitudeNeedleSmall;
    public GameObject altitudeNeedleSmallest;//UK
    public GameObject altitudeNeedleLarge;
    public GameObject mmhgDial;
    public GameObject airspeedNeedle; 
    public GameObject turnAndBankNumberTrack;    
    public GameObject turnAndBankPlane;
    public GameObject headingIndicator;
    public GameObject turnCoordinatorNeedle;
    public GameObject turnCoordinatorBall;
    public GameObject vsiNeedle;
    public GameObject repeaterCompassFace;
    public GameObject repeaterCompassAlternateFace;
    public GameObject artificialHorizon;
    public GameObject artificialHorizonChevron;
    public GameObject artificialHorizonPlane;//for ITA
    public GameObject artificialHorizonNeedle;//GER
    public GameObject turnAndBankBall;
    public GameObject compassRim;//GER
    public List<GameObject> rpmNeedlesLarge = new List<GameObject>();
    public List<GameObject> rpmNeedlesSmall = new List<GameObject>();

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
    
    private List<Quaternion> quaternionsTurnAndBankBall = new List<Quaternion>() { Quaternion.identity, Quaternion.identity };

    private List<Quaternion> quaternionsTurnCoordinatorNeedle = new List<Quaternion>() { Quaternion.identity, Quaternion.identity };
    private List<Quaternion> quaternionsTurnCoordinatorBall = new List<Quaternion>() { Quaternion.identity, Quaternion.identity };
    private List<Quaternion> quaternionsVSI = new List<Quaternion>() { Quaternion.identity, Quaternion.identity };
    private List<Quaternion> quaternionsRepeaterCompass = new List<Quaternion>() { Quaternion.identity, Quaternion.identity };
    private List<Quaternion> quaternionsRepeaterCompassAlternate = new List<Quaternion>() { Quaternion.identity, Quaternion.identity };
    private List<Quaternion> quaternionsArtificialHorizon = new List<Quaternion>() { Quaternion.identity, Quaternion.identity };
    private List<Quaternion> quaternionsArtificialHorizonPlane = new List<Quaternion>() { Quaternion.identity, Quaternion.identity };
    private List<Quaternion> quaternionsArtificialHorizonNeedle = new List<Quaternion>() { Quaternion.identity, Quaternion.identity };
    private List<Quaternion> quaternionsArtificialHorizonChevron = new List<Quaternion>() { Quaternion.identity, Quaternion.identity };
    private List<List<Quaternion>> quaternionsRPMLarge = new List<List<Quaternion>>();// = new List<Quaternion>() { Quaternion.identity, Quaternion.identity };
    private List<List<Quaternion>> quaternionsRPMSmall = new List<List<Quaternion>>();// = new List<Quaternion>() { Quaternion.identity, Quaternion.identity };

    // -- positions
    private List<Vector3> positionsHeading = new List<Vector3>() { Vector3.zero, Vector3.zero };
    private List<Vector3> positionsTurnAndBankPlane = new List<Vector3>() { Vector3.zero, Vector3.zero };
    private List<Vector3> positionsTurnAndBankNumberTrack = new List<Vector3>() { Vector3.zero, Vector3.zero };
    private List<Vector3> positionsArtificialHorizon = new List<Vector3>() { Vector3.zero, Vector3.zero };
    

    //  private bool saveForPredictions -- rotations

    //public Quaternion airspeedStart; // -- "Start" needed, just use object transform loation/rotation
    public  Quaternion airspeedTarget;
    private Quaternion altitudeLargeTarget;
    private Quaternion altitudeSmallTarget;
    private Quaternion altitudeSmallestTarget;
    private Quaternion mmhgTarget;
    private Quaternion turnAndBankPlaneRotationTarget;    
    private Quaternion turnCoordinatorNeedleTarget;    
    private Quaternion turnCoordinatorBallTarget;
    private Quaternion vsiNeedleTarget;
    private Quaternion repeaterCompassTarget;
    private Quaternion repeaterCompassAlternateTarget;
    private Quaternion artificialHorizonRotationTarget;
    private Quaternion artificialHorizonNeedleTarget;
    private Quaternion artificialHorizonChevronTarget;
    private Quaternion artificialHorizonRotationPlaneTarget;//if dial has seperate background and plane
    public List<Quaternion> rpmLargeTargets = new List<Quaternion>();
    public  List<Quaternion> rpmSmallTargets = new List<Quaternion>();

    private Quaternion turnAndBankBallTarget;

    // -- positions
    //heading is on a track, we move along the x, we don't rotate
    //private Vector3 headingIndicatorStart;
    private Vector3 headingIndicatorTarget;    
    private Vector3 turnAndBankPlanePositionTarget;    
    private Vector3 turnAndBankNumberTrackTarget;
    private Vector3 artificialHorizonPositionTarget;

    // -- modifiers
    public float trackLength = -15.64f;
    public float trackLengthForSwitch = 200.3f;
    

    public float turnCoordinaterNeedleMod = 1f;
    public float turnCoordinaterBallMod = 1f;    
    public float turnCoordinaterMultiplier = 20f;
    public float artificialHorizonRollMod = 1f;
    public float artificialHorizonMultiplier = 20f;
    public float turnAndBankBallMultiplier = 1f;

    public AnimationCurve animationCurveVSI;
    
    public AnimationCurve animationCurveRPMA;
    public AnimationCurve animationCurveRPMB;
    public AnimationCurve animationCurveRPMC;
    public AnimationCurve animationCurveRPMD;
    private bool headingIndicatorTest;

    // Start is called before the first frame update
    void Start()
    {

        for (int i = 0; i < airplaneData.planeAttributes.engines; i++)
        {
            //initialise lists with zero rotations
            quaternionsRPMLarge.Add(new List<Quaternion>() { Quaternion.identity, Quaternion.identity });
            quaternionsRPMSmall.Add(new List<Quaternion>() { Quaternion.identity, Quaternion.identity });

            //fill empty so wecan asign to later
            rpmLargeTargets.Add(Quaternion.identity);
            rpmSmallTargets.Add(Quaternion.identity);
        }

    }


    // Update is called once per frame
    void Update()
    {

        

        if (airplaneData.tests)
        {
            //this makes the network code think we received a message on the last frame
            //note- need seperate mmhg test
            //if (airplaneData.heading > .2f && airplaneData.heading < 6.2f)
            //headingIndicatorShift = !headingIndicatorShift;
            bool autoMove = false;
            if (autoMove)
            {


                float h = airplaneData.heading;
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
                    airplaneData.heading += speed;
                else
                    airplaneData.heading -= speed;

            }
            previousMessageTime = lastMessageReceivedTime;//using?
            lastMessageReceivedTime = Time.time -Time.deltaTime;


            airplaneData.headingPrevious= airplaneData.heading;
           
            //airplaneData.heading = airplaneData.heading;

            
            if (airplaneData.heading > Mathf.PI * 2)
            {
                airplaneData.heading -= Mathf.PI * 2;
               // airplaneData.headingPrevious -= Mathf.PI * 2;

             //   headingIndicatorCrossoverLeft = true;
            }

            else if (airplaneData.heading < 0)
            {
                airplaneData.heading += Mathf.PI * 2;
              //  airplaneData.headingPrevious += Mathf.PI * 2;

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
            airplaneData.altitude = 0f;
            airplaneData.mmhg = 0f;
            airplaneData.airspeed = 0f;
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

        //repeater compass
        if (repeaterCompassFace != null)
            AddRotationToList(quaternionsRepeaterCompass, repeaterCompassFace.transform.rotation);

        //repeater compass alt
        if (repeaterCompassAlternateFace != null)
            AddRotationToList(quaternionsRepeaterCompassAlternate, repeaterCompassAlternateFace.transform.rotation);

        //artificial horizon
        if (artificialHorizon)
            AddRotationToList(quaternionsArtificialHorizon, artificialHorizon.transform.rotation);

        //artificial horizon plane for ITA
        if (artificialHorizonPlane)
            AddRotationToList(quaternionsArtificialHorizonPlane, artificialHorizonPlane.transform.rotation);

        //middle needle for tnb ger
        if(artificialHorizonNeedle)
            AddRotationToList(quaternionsArtificialHorizonNeedle, artificialHorizonNeedle.transform.rotation);

        //turn and bank ball
        if (turnAndBankBall != null)
            AddRotationToList(quaternionsTurnAndBankBall, turnAndBankBall.transform.rotation);

        //rpm
        for (int i = 0; i < rpmNeedlesLarge.Count; i++)
        {
            if (rpmNeedlesLarge[i] != null)
                AddRotationToList(quaternionsRPMLarge[i], rpmNeedlesLarge[i].transform.rotation);
        }

        for (int i = 0; i < rpmNeedlesSmall.Count; i++)
        {
            if (rpmNeedlesSmall[i] != null)
                AddRotationToList(quaternionsRPMSmall[i], rpmNeedlesSmall[i].transform.rotation);
        }
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
        //end point
        airspeedTarget = airspeedNeedle.transform.rotation * Quaternion.Euler(0, 0, difference); ;

        //altitude
        //last known difference
        difference = quaternionsAltitudeLarge[0].eulerAngles.z - quaternionsAltitudeLarge[1].eulerAngles.z;
        //keep moving at client send rate at previous known step    
        //and end point
        altitudeLargeTarget = altitudeNeedleLarge.transform.rotation * Quaternion.Euler(0, 0, difference); ;

        //small altutude
        difference = quaternionsAltitudeSmall[0].eulerAngles.z - quaternionsAltitudeSmall[1].eulerAngles.z;
        //keep moving at client send rate at previous known step       
        //and end point
        altitudeSmallTarget = altitudeNeedleSmall.transform.rotation * Quaternion.Euler(0, 0, difference); ;

        //smallest altutude (if UK)

        if (airplaneData.country == AirplaneData.Country.UK)
        {
            difference = quaternionsAltitudeSmallest[0].eulerAngles.z - quaternionsAltitudeSmallest[1].eulerAngles.z;
            //keep moving at client send rate at previous known step           
            //and end point
            altitudeSmallestTarget = altitudeNeedleSmallest.transform.rotation * Quaternion.Euler(0, 0, difference); ;

        }

        difference = quaternionsMmhg[0].eulerAngles.z - quaternionsMmhg[1].eulerAngles.z;
        //keep moving at client send rate at previous known step   
        //and end point
        mmhgTarget = mmhgDial.transform.rotation * Quaternion.Euler(0, 0, difference);

        //heading
        Vector3 differenceV3 = positionsHeading[0] - positionsHeading[1];   
        headingIndicatorTarget = headingIndicator.transform.localPosition + differenceV3;
       
        //turn and bank
        // - plane position
        if (turnAndBankPlane != null)
        {
            differenceV3 = positionsTurnAndBankPlane[0] - positionsTurnAndBankPlane[1];         
            turnAndBankPlanePositionTarget = turnAndBankPlane.transform.localPosition + differenceV3;
            // - plane rotation
            difference = quaternionsTurnAndBankPlane[0].z - quaternionsTurnAndBankPlane[1].z;          
            turnAndBankPlaneRotationTarget = turnAndBankPlane.transform.rotation * Quaternion.Euler(0, 0, difference);
            // - number track
            if (turnAndBankNumberTrack != null)
            {
                differenceV3 = positionsTurnAndBankNumberTrack[0] - positionsTurnAndBankNumberTrack[1];
                turnAndBankNumberTrackTarget = turnAndBankNumberTrack.transform.localPosition + differenceV3;
            }
        }

        if (turnAndBankBall != null)
        {
            difference = quaternionsTurnAndBankBall[0].z - quaternionsTurnAndBankBall[1].z;
            turnAndBankBallTarget = turnAndBankBall.transform.rotation * Quaternion.Euler(0, 0, difference);
        }

        if (turnCoordinatorNeedle != null)
        {
            //turn co-ordinator
            // - needle
            difference = quaternionsTurnCoordinatorNeedle[0].z - quaternionsTurnCoordinatorNeedle[1].z;
            turnCoordinatorNeedleTarget = turnCoordinatorNeedle.transform.rotation * Quaternion.Euler(0, 0, difference);
            // - ball
            difference = quaternionsTurnCoordinatorBall[0].z - quaternionsTurnCoordinatorBall[1].z;
            turnCoordinatorBallTarget = turnCoordinatorBall.transform.rotation * Quaternion.Euler(0, 0, difference);
        }

        if (vsiNeedle != null)
        {
            //VSI
            difference = quaternionsVSI[0].z - quaternionsVSI[1].z;
            vsiNeedleTarget = vsiNeedle.transform.rotation * Quaternion.Euler(0, 0, difference);
        }

        //ger repeater / US repeater
        if (repeaterCompassFace != null)
        {
            difference = quaternionsRepeaterCompass[0].z - quaternionsRepeaterCompass[1].z;
            repeaterCompassTarget = repeaterCompassFace.transform.rotation * Quaternion.Euler(0, 0, difference);
        }

        if (repeaterCompassAlternateFace != null)
        {
            difference = quaternionsRepeaterCompassAlternate[0].z - quaternionsRepeaterCompassAlternate[1].z;
            repeaterCompassAlternateTarget = repeaterCompassAlternateFace.transform.rotation * Quaternion.Euler(0, 0, difference);
        }


        //artificial horizon        
        if (artificialHorizon!= null)
        {
            difference = quaternionsArtificialHorizon[0].z - quaternionsArtificialHorizon[1].z;
            artificialHorizonRotationTarget = artificialHorizon.transform.rotation * Quaternion.Euler(0, 0, difference);
            //pos
            differenceV3 = positionsArtificialHorizon[0] - positionsArtificialHorizon[1];
            artificialHorizonPositionTarget = artificialHorizon.transform.localPosition + differenceV3;
        }

        //artificial horizon ITA plane
        if (artificialHorizonPlane != null && airplaneData.planeAttributes.country == AirplaneData.Country.ITA)
        {
            difference = quaternionsArtificialHorizonPlane[0].z - quaternionsArtificialHorizonPlane[1].z;
            artificialHorizonRotationPlaneTarget = artificialHorizonPlane.transform.rotation * Quaternion.Euler(0, 0, difference);
        }

        //artificial horizon GER needle
        if (artificialHorizonPlane != null && airplaneData.planeAttributes.country == AirplaneData.Country.GER)
        {
            difference = quaternionsArtificialHorizonNeedle[0].z - quaternionsArtificialHorizonNeedle[1].z;
            artificialHorizonNeedleTarget = artificialHorizonNeedle.transform.rotation * Quaternion.Euler(0, 0, difference);
        }


        //chevron
        if (artificialHorizonChevron != null)
        {
            difference = quaternionsArtificialHorizonChevron[0].z - quaternionsArtificialHorizonChevron[1].z;
            artificialHorizonChevronTarget = artificialHorizonChevron.transform.rotation * Quaternion.Euler(0, 0, difference);
        }

        for (int i = 0; i < rpmNeedlesLarge.Count; i++)
        {
            if (rpmNeedlesLarge[i] != null)
            {
                difference = quaternionsRPMLarge[i][0].z - quaternionsRPMLarge[i][1].z;
                rpmLargeTargets[i] = rpmNeedlesLarge[i].transform.rotation * Quaternion.Euler(0, 0, difference);
            }
        }
        for (int i = 0; i < rpmNeedlesSmall.Count; i++)
        {
            if (rpmNeedlesSmall[i] != null)
            {
                difference = quaternionsRPMSmall[i][0].z - quaternionsRPMSmall[i][1].z;
                rpmSmallTargets[i] = rpmNeedlesSmall[i].transform.rotation * Quaternion.Euler(0, 0, difference);
            }
        }

    }

    public void SetRotationTargets()
    {
        //called when tcp client receives update
        SavePreviousRotationsAndPositions();

        AirspeedTarget();

        AltimeterTargets();

        HeadingTarget(airplaneData.country);

        TurnAndBankTargets(airplaneData.country);

        TurnCoordinatorTarget(airplaneData.country);

        VSITarget(airplaneData.country);

        RepeaterCompassTarget(airplaneData.country);

        ArtificialHorizonTargets(airplaneData.country);

        RPMTarget(airplaneData.country);
    }

    void RPMTarget(AirplaneData.Country country)
    {
     
        for (int i = 0; i < airplaneData.planeAttributes.engines; i++)
        {
            switch (country)
            {
                //RU
                case (AirplaneData.Country.RU):
                    if (airplaneData.planeAttributes.rpmType == RpmType.A)
                    {
                        rpmLargeTargets[i] = RussianDials.RPMALargeTarget(airplaneData.rpms[i]);
                        rpmSmallTargets[i] = RussianDials.RPMASmallTarget(airplaneData.rpms[i]);
                    }
                    else if (airplaneData.planeAttributes.rpmType == RpmType.B)
                    {
                        rpmLargeTargets[i] = RussianDials.RPMBTarget(airplaneData.rpms[i], airplaneData.scalar0, airplaneData.scalar1);
                    }
                    else if (airplaneData.planeAttributes.rpmType == RpmType.C)
                    {
                        rpmLargeTargets[i] = RussianDials.RPMCLargeTarget(airplaneData.rpms[i]);
                        rpmSmallTargets[i] = RussianDials.RPMCSmallTarget(airplaneData.rpms[i]);
                    }

                    break;
                    
                //GER
                case (AirplaneData.Country.GER):
                    if (airplaneData.planeAttributes.rpmType == RpmType.A)
                    {
                        rpmLargeTargets[i] = GermanDials.RPMATarget(airplaneData.rpms[i], airplaneData.scalar0, airplaneData.scalar1);
                    }
                    else if (airplaneData.planeAttributes.rpmType == RpmType.B)
                    {
                        rpmLargeTargets[i] = GermanDials.RPMBTarget(airplaneData.rpms[i], airplaneData.scalar0, airplaneData.scalar1, animationCurveRPMA);
                    }
                    else if (airplaneData.planeAttributes.rpmType == RpmType.C)
                    {
                        rpmLargeTargets[i] = GermanDials.RPMCTarget(airplaneData.rpms[i], airplaneData.scalar0, airplaneData.scalar1, animationCurveRPMC);
                    }
                    else
                        rpmLargeTargets[i] = GermanDials.RPMDTarget(airplaneData.rpms[i], airplaneData.scalar0, airplaneData.scalar1, animationCurveRPMD);

                    break;
                
               //US
               case (AirplaneData.Country.US):
                    if (airplaneData.planeAttributes.rpmType == RpmType.A)
                    {
                        rpmLargeTargets[i] = USDials.RPMATarget(airplaneData.rpms[i], airplaneData.scalar0, airplaneData.scalar1);
                        rpmSmallTargets[i] = USDials.RPMAInnerTarget(airplaneData.rpms[i], airplaneData.scalar0, airplaneData.scalar1);
                    }
                    else if (airplaneData.planeAttributes.rpmType == RpmType.B)
                    {
                        rpmLargeTargets[i] = USDials.RPMBTarget(airplaneData.rpms[i], airplaneData.scalar0, airplaneData.scalar1);
                    }
                    else if (airplaneData.planeAttributes.rpmType == RpmType.C)
                    {
                        rpmLargeTargets[i] = USDials.RPMCTarget(airplaneData.rpms[i], airplaneData.scalar0, airplaneData.scalar1);
                    }
                    else if (airplaneData.planeAttributes.rpmType == RpmType.D)
                    {
                        //we can use A for big needle
                        rpmLargeTargets[i] = USDials.RPMATarget(airplaneData.rpms[i], airplaneData.scalar0, airplaneData.scalar1);
                        rpmSmallTargets[i] = USDials.RPMDSmallTarget(airplaneData.rpms[i], airplaneData.scalar0, airplaneData.scalar1);
                    }
                    else if (airplaneData.planeAttributes.rpmType == RpmType.E)
                    {
                        //note we use hard indexes for rpms and engine lists. P38J is asigend "1 engine" because it only has 1 engine dial
                        rpmLargeTargets[0] = USDials.RPMCTarget(airplaneData.rpms[0], airplaneData.scalar0, airplaneData.scalar1);
                        rpmSmallTargets[0] = USDials.RPMCTarget(airplaneData.rpms[1], airplaneData.scalar0, airplaneData.scalar1);
                    }

                        break;

                    
           case (AirplaneData.Country.UK):
               if (airplaneData.planeAttributes.rpmType == RpmType.A)
               {
                   //A Taret is first needle
                   rpmLargeTargets[i] = UKDials.RPMATarget(airplaneData.rpms[i], airplaneData.scalar0, airplaneData.scalar1,animationCurveRPMA);

               }
               else if (airplaneData.planeAttributes.rpmType == RpmType.B)
               {
                   //"A" Target is first Needle - not the best naming
                   rpmLargeTargets[i] = UKDials.RPMBTarget(airplaneData.rpms[i], airplaneData.scalar0, airplaneData.scalar1);
               }
               break;

           case (AirplaneData.Country.ITA):

               if (airplaneData.planeAttributes.rpmType == RpmType.A)
               {
                   rpmLargeTargets[i] = ITADials.RPMATarget(airplaneData.rpms[i], airplaneData.scalar0, airplaneData.scalar1);
               }
               break;
               
            }
        }

        
    }

    void ArtificialHorizonTargets(AirplaneData.Country country)
    {
        switch (country)
        {
            //no RU

            //GER
            case (AirplaneData.Country.GER):
                //rotation // roll
                artificialHorizonRotationTarget = GermanDials.ArtificialHorizon(airplaneData.roll, artificialHorizonRollMod);

                //position  
                artificialHorizonPositionTarget = GermanDials.ArtificialHorizonPosition(airplaneData.pitch, artificialHorizonMultiplier);

                //use the same function that turn co-ordinator uses
                if (artificialHorizonNeedle != null)
                    artificialHorizonNeedleTarget = GermanDials.TurnCoordinatorNeedleTarget(airplaneData.turnCoordinatorNeedle,airplaneData.planeType);

                break;


            //US
            case (AirplaneData.Country.US):
                //rotation // roll
               
                artificialHorizonRotationTarget = USDials.ArtificialHorizonRotation(airplaneData.roll, artificialHorizonRollMod);

                //position  /pitch                
                artificialHorizonPositionTarget = USDials.ArtificialHorizonPosition(airplaneData.pitch, artificialHorizonMultiplier);
                

                //chevron
                artificialHorizonChevronTarget = USDials.ArtificialHorizonChevronRotation(airplaneData.roll, artificialHorizonRollMod);

                
                break;

            case (AirplaneData.Country.UK):

                artificialHorizonRotationTarget = UKDials.ArtificialHorizonRotation(airplaneData.roll, artificialHorizonRollMod);

                //position  
                artificialHorizonPositionTarget = UKDials.ArtificialHorizonPosition(airplaneData.pitch, artificialHorizonMultiplier);
                //chevron
                artificialHorizonChevronTarget = UKDials.ArtificialHorizonChevronRotation(airplaneData.roll, artificialHorizonRollMod);
                break;

            case (AirplaneData.Country.ITA):

                //rotation of plane
                artificialHorizonRotationPlaneTarget = ITADials.ArtificialHorizonRotation(airplaneData.roll, artificialHorizonRollMod);

                //position of moving track/ball, only moves on Y axis
                artificialHorizonPositionTarget = ITADials.ArtificialHorizonPosition(airplaneData.pitch, artificialHorizonMultiplier);
                break;
        }
    }

    void RepeaterCompassTarget(AirplaneData.Country country)
    {

        if (country == AirplaneData.Country.GER)
        {
            if (airplaneData.planeAttributes.repeaterCompass)
            {
                //if user spins rim
                float offset = compassRim.transform.eulerAngles.z;
                repeaterCompassTarget = GermanDials.RepeaterCompassTarget(airplaneData.heading, offset);
            }
           
            if(airplaneData.planeAttributes.repeaterCompassAlternate)
            {
                //Junkers unique dial
                repeaterCompassAlternateTarget = GermanDials.RepeaterCompassAlternateTarget(airplaneData.heading);
            }
        }

        else if (country == AirplaneData.Country.US)
            repeaterCompassTarget = USDials.RepeaterCompassTarget(airplaneData.heading);

        else if (country == AirplaneData.Country.UK)
            repeaterCompassTarget = UKDials.RepeaterCompassTarget(airplaneData.heading);


    }
    
    void VSITarget(AirplaneData.Country country)
    {
        switch (country)
        {
            case (AirplaneData.Country.RU):
                if(airplaneData.planeAttributes.vsiLarge)
                    vsiNeedleTarget = RussianDials.VerticalSpeedTargetLarge(airplaneData.verticalSpeed);
                
                else if(airplaneData.planeAttributes.vsiSmall)
                    vsiNeedleTarget = RussianDials.VerticalSpeedTargetSmall(airplaneData.verticalSpeed);

                break;

            case (AirplaneData.Country.GER):

                if (airplaneData.planeAttributes.vsiLarge)
                {
                    //need to clamp vertical speed - helps with coming back from over the limit
                    airplaneData.verticalSpeed = Mathf.Clamp(airplaneData.verticalSpeed, -30f, 30f);
                    vsiNeedleTarget = GermanDials.VerticalSpeedTargetLarge(airplaneData.verticalSpeed);
                }

                else if (airplaneData.planeAttributes.vsiSmall)
                {
                    airplaneData.verticalSpeed = Mathf.Clamp(airplaneData.verticalSpeed, -15f, 15f);
                    vsiNeedleTarget = GermanDials.VerticalSpeedTargetSmall(airplaneData.verticalSpeed);
                }

                else if (airplaneData.planeAttributes.vsiSmallest)
                {
                    airplaneData.verticalSpeed = Mathf.Clamp(airplaneData.verticalSpeed, -5f, 5f);
                    vsiNeedleTarget = GermanDials.VerticalSpeedTargetSmallest(airplaneData.verticalSpeed);
                }

                break;

            //these countries only have one vsi (so far)
            case (AirplaneData.Country.US):
                vsiNeedleTarget = USDials.VerticalSpeedTarget(airplaneData.verticalSpeed,animationCurveVSI);
                //vsiNeedleTarget = USDials.VerticalSpeedTargetSimple(airplaneData.verticalSpeed);
                break;

            case (AirplaneData.Country.UK):
                vsiNeedleTarget = UKDials.VerticalSpeedTarget(airplaneData.verticalSpeed);
                break;

            case (AirplaneData.Country.ITA):
                vsiNeedleTarget = ITADials.VerticalSpeedTarget(airplaneData.verticalSpeed);
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
                turnCoordinatorNeedleTarget = RussianDials.TurnCoordinatorNeedleTarget(airplaneData.turnCoordinatorNeedle, turnCoordinaterNeedleMod);

                //ball indicator                
                turnCoordinatorBallTarget = RussianDials.TurnCoordinatorBallTarget(airplaneData.turnCoordinatorBall);
                break;

            case (AirplaneData.Country.GER):
                //RU
                //pendulum needle
                turnCoordinatorNeedleTarget = GermanDials.TurnCoordinatorNeedleTarget(airplaneData.turnCoordinatorNeedle,airplaneData.planeType);

                //ball indicator                
                turnCoordinatorBallTarget = GermanDials.TurnCoordinatorBallTarget(airplaneData.turnCoordinatorBall,turnCoordinaterBallMod);
                break;

            case (AirplaneData.Country.US):
                if(airplaneData.planeType == "A-20B")
                    turnCoordinatorNeedleTarget = USDials.TurnCoordinatorNeedleTarget(airplaneData.turnCoordinatorNeedle, true);
                else 
                    turnCoordinatorNeedleTarget = USDials.TurnCoordinatorNeedleTarget(airplaneData.turnCoordinatorNeedle, false);

                //ball indicator                
                turnCoordinatorBallTarget = USDials.TurnCoordinatorBallTarget(airplaneData.turnCoordinatorBall, turnCoordinaterBallMod);
                break;


            case (AirplaneData.Country.UK):
                turnCoordinatorNeedleTarget = UKDials.TurnCoordinatorNeedleTarget(airplaneData.turnCoordinatorNeedle, turnCoordinaterNeedleMod);

                //second needle        
                turnCoordinatorBallTarget = UKDials.TurnCoordinatorBallTarget(airplaneData.turnCoordinatorBall, turnCoordinaterBallMod);
                break;


            case (AirplaneData.Country.ITA):
                turnCoordinatorNeedleTarget = ITADials.TurnCoordinatorNeedleTarget(airplaneData.turnCoordinatorNeedle);

                //second needle        
                turnCoordinatorBallTarget = ITADials.TurnCoordinatorBallTarget(airplaneData.turnCoordinatorBall, turnCoordinaterBallMod);
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
                turnAndBankPlanePositionTarget = RussianDials.TurnAndBankPlanePosition(airplaneData.pitch, turnAndBankPitchMultiplier);

                //plane or background rotation
                turnAndBankPlaneRotationTarget = RussianDials.TurnAndBankPlaneRotation(airplaneData.roll, airplaneData.pitch, turnAndBankRollMultiplier,turnAndBankPlaneXMultiplier);

                turnAndBankNumberTrackTarget = RussianDials.TurnAndBankNumberTrackPosition(airplaneData.pitch, turnAndBankPitchMultiplier);

                break;

                // with slip?
            case (AirplaneData.Country.GER):
                turnAndBankPlanePositionTarget = GermanDials.TurnAndBankPlanePosition(airplaneData.pitch, turnAndBankPitchMultiplier);

                turnAndBankPlaneRotationTarget = GermanDials.TurnAndBankPlaneRotation(airplaneData.roll, airplaneData.pitch, turnAndBankRollMultiplier, turnAndBankRollMultiplier);

                turnAndBankBallTarget = GermanDials.TurnAndBankBallTarget(airplaneData.turnCoordinatorBall, turnAndBankBallMultiplier);

                break;

        }


        
    }

    void HeadingTarget(AirplaneData.Country country)
    {
        
        switch (country)
        {
            case (AirplaneData.Country.RU):
                headingIndicatorTarget = RussianDials.HeadingIndicatorPosition(airplaneData.heading,trackLength);
                break;

            case (AirplaneData.Country.GER):
                headingIndicatorTarget = GermanDials.HeadingIndicatorPosition(airplaneData.heading,trackLength);
                break;

            case (AirplaneData.Country.US):
                headingIndicatorTarget = USDials.HeadingIndicatorPosition(airplaneData.heading, trackLength);
                break;

            case (AirplaneData.Country.UK):
                headingIndicatorTarget = UKDials.HeadingIndicatorPosition(airplaneData.heading ,trackLength);
                break;

            case (AirplaneData.Country.ITA):
                headingIndicatorTarget = ITADials.HeadingIndicatorPosition(airplaneData.heading, trackLength);
                break;
        }
        
    }

    void AltimeterTargets()
    {  
        //Altitude
        //set where we rotate from
        //AltitudeStarts();
        //find where we are rotating to for each needle

        //if mini needle
        if (altitudeNeedleSmallest != null)
            altitudeSmallestTarget = AltitudeTargetSmallest(airplaneData.country, airplaneData.altitude);

        altitudeSmallTarget = AltitudeTargetSmall(airplaneData.country, airplaneData.altitude);
        altitudeLargeTarget = AltitudeTargetLarge(airplaneData.country, airplaneData.altitude);

        PressureReferenceTargets();
    }

    void PressureReferenceTargets()
    {   //Pressure //mmhg

        //set where we rotate from
        //MmhgStart();

        //set where we are rotating to
        mmhgTarget = AtmosphericPressure(airplaneData.country, airplaneData.mmhg);

    }

    void AirspeedTarget()
    {
        // Airspeed
        //set where we rotate from
        //AirspeedStart();
        //find where we are rotating to
        airspeedTarget = AirspeedTarget(airplaneData.country, airplaneData.airspeed);
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

            case AirplaneData.Country.US:
                target = USDials.AltitudeTargetSmallest(altitude);
                break;
        }


        return target;
    }

    void NeedleRotations()
    {
        AirspeedNeedleRotation();

        AltitudeNeedleRotations();

        MmhgNeedleRotation();
        
        HeadingIndicatorRotation();
        
        TurnAndBankRotations();    

        TurnCoordinatorRotation();

        VSIRotation();

        RepeaterCompassRotation();

        ArtificialHorizonTranslations();

        RPMRotations();
    }

    void RPMRotations()
    {
        for (int i = 0; i < rpmNeedlesLarge.Count; i++)
        {
            if (rpmNeedlesLarge[i].gameObject != null)
                rpmNeedlesLarge[i].transform.rotation = Quaternion.Slerp(rpmNeedlesLarge[i].transform.rotation, rpmLargeTargets[i], Time.fixedDeltaTime);
        }


        for (int i = 0; i < rpmNeedlesSmall.Count; i++)
        {
            if (rpmNeedlesSmall[i].gameObject != null)
                rpmNeedlesSmall[i].transform.rotation = Quaternion.Slerp(rpmNeedlesSmall[i].transform.rotation, rpmSmallTargets[i], Time.fixedDeltaTime);

        }
    }

    void AirspeedNeedleRotation()
    {
        //float d = Mathf.Abs( airspeedTarget.eulerAngles.z - quaternionsAirspeed[0].eulerAngles.z);
       // Debug.Log("air needle");

        airspeedNeedle.transform.rotation = Quaternion.Slerp(airspeedNeedle.transform.rotation, airspeedTarget, Time.fixedDeltaTime); 
        
    }

    void AltitudeNeedleRotations()
    {
        //only UK / US has the smallest needle
        if (altitudeNeedleSmallest != null)
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
        if (airplaneData.headingPrevious - airplaneData.heading > Mathf.PI)
        {
            //adjust previous heading so that's its close to new heading
            airplaneData.headingPrevious -= Mathf.PI * 2;

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

            Debug.Log("heading = " + airplaneData.heading);
            Debug.Log("heading Prev = " + airplaneData.headingPrevious);
            */
        }
        //check for rolling over 0 from the oter direction
        else if (airplaneData.heading - airplaneData.headingPrevious > Mathf.PI)
        {
            //  airplaneData.heading += Mathf.PI * 2;
            airplaneData.headingPrevious += Mathf.PI * 2;

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

            Debug.Log("heading = " + airplaneData.heading);
            Debug.Log("heading Prev = " + airplaneData.headingPrevious);
            */
        }



        //now rework target
        HeadingTarget(airplaneData.country);


        return false;
    }
    void HeadingIndicatorRotation()
    {
        HeadingIndicatorSwitch();

        //
        if (float.IsNaN(headingIndicatorTarget.y) || float.IsNaN(headingIndicatorTarget.z))
            return;

        headingIndicator.transform.localPosition = Vector3.Lerp(headingIndicator.transform.localPosition, headingIndicatorTarget, Time.fixedDeltaTime);





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
        if (airplaneData.planeAttributes.repeaterCompass)
        {
            if (repeaterCompassFace != null)
                repeaterCompassFace.transform.rotation = Quaternion.Slerp(repeaterCompassFace.transform.rotation, repeaterCompassTarget, Time.fixedDeltaTime);
        }

        if(airplaneData.planeAttributes.repeaterCompassAlternate)
        {
            if(repeaterCompassAlternateFace != null)
                repeaterCompassAlternateFace.transform.rotation = Quaternion.Slerp(repeaterCompassAlternateFace.transform.rotation, repeaterCompassAlternateTarget, Time.fixedDeltaTime);
        }
    }

    void ArtificialHorizonTranslations()
    {
        if (artificialHorizon == null)
            return;

        ArtificialHorizonPosition();
        ArtificialHorizonRotation();
        ArtificialHorizonChevronRotation();
    }

    void ArtificialHorizonRotation()
    {
        if (artificialHorizonPlane != null)
            //ITA plane rotates whilst background doesn't
            artificialHorizonPlane.transform.rotation = Quaternion.Slerp(artificialHorizonPlane.transform.rotation, artificialHorizonRotationPlaneTarget, Time.fixedDeltaTime);

        else
            artificialHorizon.transform.rotation = Quaternion.Slerp(artificialHorizon.transform.rotation, artificialHorizonRotationTarget, Time.fixedDeltaTime);

        //german plane also has co-ordinator needle on this dial
        if(artificialHorizonNeedle != null)
        {
            artificialHorizonNeedle.transform.rotation = Quaternion.Slerp(artificialHorizonNeedle.transform.rotation, artificialHorizonNeedleTarget, Time.fixedDeltaTime);
        }
    }

    void ArtificialHorizonPosition()
    {
        artificialHorizon.transform.localPosition = Vector3.Lerp(artificialHorizon.transform.localPosition, artificialHorizonPositionTarget, Time.fixedDeltaTime);

        //clamp plane to background
        if(artificialHorizonPlane != null) //ITA
            artificialHorizonPlane.transform.localPosition = artificialHorizon.transform.localPosition;
    }

    void ArtificialHorizonChevronRotation()
    {
        if(artificialHorizonChevron != null)
            artificialHorizonChevron.transform.rotation = Quaternion.Slerp(artificialHorizonChevron.transform.rotation, artificialHorizonChevronTarget, Time.fixedDeltaTime);
    }
}
