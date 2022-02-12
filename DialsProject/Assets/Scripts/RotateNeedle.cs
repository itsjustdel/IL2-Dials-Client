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
    public UDPClient udpClient;
    public float smoothing = 3f;

    public GameObject altitudeNeedleSmall;
    public GameObject altitudeNeedleSmallest;//UK//US
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


    public float previousMessageTime;
    public float maxSpin =1f;
    public float turnAndBankPitchMultiplier = 5f;
    public float turnAndBankRollMultiplier = 5f;
    public float turnAndBankPlaneXMultiplier = 5f;


    // -- positions
    private List<Vector3> positionsHeading = new List<Vector3>() { Vector3.zero, Vector3.zero };    //needed?

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
    public List<Quaternion> manifoldLargeTargets = new List<Quaternion>();
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
            //fill empty so wecan asign to later
            rpmLargeTargets.Add(Quaternion.identity);
            rpmSmallTargets.Add(Quaternion.identity);

            //manifold
            manifoldLargeTargets.Add(Quaternion.identity);
        }

    }


    // Update is called once per frame
    void Update()
    { 
        if (airplaneData.tests)
        {
            Tests();   
            return;
        }
        

        SetRotationTargets();


        NeedleRotations();
    }


    void ResetNeedles()
    {
        //if we have completely lost connection reset needles, there is a 5 second grace period where prediction takes over            
        airplaneData.altitude = 0f;
        airplaneData.mmhg = 0f;
        airplaneData.airspeed = 0f;
        //add more? -TODO   
    }

    void Tests()
    {
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
      //  previousMessageTime = lastMessageReceivedTime;//using?
        //lastMessageReceivedTime = Time.time - Time.deltaTime;

        airplaneData.headingPrevious = airplaneData.heading;

        if (airplaneData.heading > Mathf.PI * 2)
        {
            airplaneData.heading -= Mathf.PI * 2;
        }

        else if (airplaneData.heading < 0)
        {
            airplaneData.heading += Mathf.PI * 2;
        }

        SetRotationTargets();

        NeedleRotations();
    }

    public void SetRotationTargets()
    {
        
        AirspeedTarget(this);

        AltimeterTargets();

        HeadingTarget(airplaneData.planeAttributes.country);

        TurnAndBankTargets(airplaneData.planeAttributes.country);

        TurnCoordinatorTarget(airplaneData.planeAttributes.country);

        VSITarget(airplaneData.planeAttributes.country);

        RepeaterCompassTarget(airplaneData.planeAttributes.country);

        ArtificialHorizonTargets(airplaneData.planeAttributes.country);

        RPMTarget(airplaneData.planeAttributes.country);

        ManifoldTarget(airplaneData.planeAttributes.country);
    }

    private void ManifoldTarget(Country country)
    {
        for (int i = 0; i < airplaneData.planeAttributes.engines; i++)
        {
            switch (country)
            {
                case (Country.GER):
                {
                        manifoldLargeTargets[i] = GermanDials.ManifoldTarget(airplaneData.manifolds[i],airplaneData.scalar0,airplaneData.scalar1);
                        break;
                }
            }
        }
    }

    void RPMTarget(Country country)
    {
     
        for (int i = 0; i < airplaneData.planeAttributes.engines; i++)
        {
            switch (country)
            {
                //RU
                case (Country.RU):
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
                case (Country.GER):
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
               case (Country.US):
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

                    
           case (Country.UK):
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

           case (Country.ITA):

               if (airplaneData.planeAttributes.rpmType == RpmType.A)
               {
                   rpmLargeTargets[i] = ITADials.RPMATarget(airplaneData.rpms[i], airplaneData.scalar0, airplaneData.scalar1);
               }
               break;
               
            }
        }

        
    }

    void ArtificialHorizonTargets(Country country)
    {
        switch (country)
        {
            //no RU

            //GER
            case (Country.GER):
                //rotation // roll
                artificialHorizonRotationTarget = GermanDials.ArtificialHorizon(airplaneData.roll, artificialHorizonRollMod);

                //position  
                artificialHorizonPositionTarget = GermanDials.ArtificialHorizonPosition(airplaneData.pitch, artificialHorizonMultiplier);

                //use the same function that turn co-ordinator uses
                if (artificialHorizonNeedle != null)
                    artificialHorizonNeedleTarget = GermanDials.TurnCoordinatorNeedleTarget(airplaneData.turnCoordinatorNeedle,airplaneData.planeType);

                break;


            //US
            case (Country.US):
                //rotation // roll
               
                artificialHorizonRotationTarget = USDials.ArtificialHorizonRotation(airplaneData.roll, artificialHorizonRollMod);

                //position  /pitch                
                artificialHorizonPositionTarget = USDials.ArtificialHorizonPosition(airplaneData.pitch, artificialHorizonMultiplier);
                

                //chevron
                artificialHorizonChevronTarget = USDials.ArtificialHorizonChevronRotation(airplaneData.roll, artificialHorizonRollMod);

                
                break;

            case (Country.UK):

                artificialHorizonRotationTarget = UKDials.ArtificialHorizonRotation(airplaneData.roll, artificialHorizonRollMod);

                //position  
                artificialHorizonPositionTarget = UKDials.ArtificialHorizonPosition(airplaneData.pitch, artificialHorizonMultiplier);
                //chevron
                artificialHorizonChevronTarget = UKDials.ArtificialHorizonChevronRotation(airplaneData.roll, artificialHorizonRollMod);
                break;

            case (Country.ITA):

                //rotation of plane
                artificialHorizonRotationPlaneTarget = ITADials.ArtificialHorizonRotation(airplaneData.roll, artificialHorizonRollMod);

                //position of moving track/ball, only moves on Y axis
                artificialHorizonPositionTarget = ITADials.ArtificialHorizonPosition(airplaneData.pitch, artificialHorizonMultiplier);
                break;
        }
    }

    void RepeaterCompassTarget(Country country)
    {

        if (country == Country.GER)
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

        else if (country == Country.US)
            repeaterCompassTarget = USDials.RepeaterCompassTarget(airplaneData.heading);

        else if (country == Country.UK)
            repeaterCompassTarget = UKDials.RepeaterCompassTarget(airplaneData.heading);


    }
    
    void VSITarget(Country country)
    {
        switch (country)
        {
            case (Country.RU):
                if(airplaneData.planeAttributes.vsiLarge)
                    vsiNeedleTarget = RussianDials.VerticalSpeedTargetLarge(airplaneData.verticalSpeed);
                
                else if(airplaneData.planeAttributes.vsiSmall)
                    vsiNeedleTarget = RussianDials.VerticalSpeedTargetSmall(airplaneData.verticalSpeed);

                break;

            case (Country.GER):

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
            case (Country.US):
                vsiNeedleTarget = USDials.VerticalSpeedTarget(airplaneData.verticalSpeed,animationCurveVSI);
                break;

            case (Country.UK):
                vsiNeedleTarget = UKDials.VerticalSpeedTarget(airplaneData.verticalSpeed);
                break;

            case (Country.ITA):
                vsiNeedleTarget = ITADials.VerticalSpeedTarget(airplaneData.verticalSpeed);
                break;

        }
    }

    void TurnCoordinatorTarget(Country country)
    {

        switch (country)
        {
            case (Country.RU):
                //RU
                //pendulum needle
                turnCoordinatorNeedleTarget = RussianDials.TurnCoordinatorNeedleTarget(airplaneData.turnCoordinatorNeedle, turnCoordinaterNeedleMod);

                //ball indicator                
                turnCoordinatorBallTarget = RussianDials.TurnCoordinatorBallTarget(airplaneData.turnCoordinatorBall);
                break;

            case (Country.GER):
                //RU
                //pendulum needle
                turnCoordinatorNeedleTarget = GermanDials.TurnCoordinatorNeedleTarget(airplaneData.turnCoordinatorNeedle,airplaneData.planeType);

                //ball indicator                
                turnCoordinatorBallTarget = GermanDials.TurnCoordinatorBallTarget(airplaneData.turnCoordinatorBall,turnCoordinaterBallMod);
                break;

            case (Country.US):
                if(airplaneData.planeType == "A-20B")
                    turnCoordinatorNeedleTarget = USDials.TurnCoordinatorNeedleTarget(airplaneData.turnCoordinatorNeedle, true);
                else 
                    turnCoordinatorNeedleTarget = USDials.TurnCoordinatorNeedleTarget(airplaneData.turnCoordinatorNeedle, false);

                //ball indicator                
                turnCoordinatorBallTarget = USDials.TurnCoordinatorBallTarget(airplaneData.turnCoordinatorBall, turnCoordinaterBallMod);
                break;


            case (Country.UK):
                turnCoordinatorNeedleTarget = UKDials.TurnCoordinatorNeedleTarget(airplaneData.turnCoordinatorNeedle, turnCoordinaterNeedleMod);

                //second needle        
                turnCoordinatorBallTarget = UKDials.TurnCoordinatorBallTarget(airplaneData.turnCoordinatorBall, turnCoordinaterBallMod);
                break;


            case (Country.ITA):
                turnCoordinatorNeedleTarget = ITADials.TurnCoordinatorNeedleTarget(airplaneData.turnCoordinatorNeedle);

                //second needle        
                turnCoordinatorBallTarget = ITADials.TurnCoordinatorBallTarget(airplaneData.turnCoordinatorBall, turnCoordinaterBallMod);
                break;

        }
       
    }

    //turn and bank is dial with artifical horizon and slip together
    void TurnAndBankTargets(Country country)
    {
        //plane or background pos

        switch (country)
        {
            case (Country.RU): 
                //note russian is quite different - more like an artifical horizon with plane moving instead of horizon
                turnAndBankPlanePositionTarget = RussianDials.TurnAndBankPlanePosition(airplaneData.pitch, turnAndBankPitchMultiplier);

                //plane or background rotation
                turnAndBankPlaneRotationTarget = RussianDials.TurnAndBankPlaneRotation(airplaneData.roll, airplaneData.pitch, turnAndBankRollMultiplier,turnAndBankPlaneXMultiplier);

                turnAndBankNumberTrackTarget = RussianDials.TurnAndBankNumberTrackPosition(airplaneData.pitch, turnAndBankPitchMultiplier);

                break;

                // with slip?
            case (Country.GER):
                turnAndBankPlanePositionTarget = GermanDials.TurnAndBankPlanePosition(airplaneData.pitch, turnAndBankPitchMultiplier);

                turnAndBankPlaneRotationTarget = GermanDials.TurnAndBankPlaneRotation(airplaneData.roll, airplaneData.pitch, turnAndBankRollMultiplier, turnAndBankRollMultiplier);

                turnAndBankBallTarget = GermanDials.TurnAndBankBallTarget(airplaneData.turnCoordinatorBall, turnAndBankBallMultiplier);

                break;

        }


        
    }

    void HeadingTarget(Country country)
    {
        
        switch (country)
        {
            case (Country.RU):
                headingIndicatorTarget = RussianDials.HeadingIndicatorPosition(airplaneData.heading,trackLength);
                break;

            case (Country.GER):
                headingIndicatorTarget = GermanDials.HeadingIndicatorPosition(airplaneData.heading,trackLength);
                break;

            case (Country.US):
                headingIndicatorTarget = USDials.HeadingIndicatorPosition(airplaneData.heading, trackLength);
                break;

            case (Country.UK):
                headingIndicatorTarget = UKDials.HeadingIndicatorPosition(airplaneData.heading ,trackLength);
                break;

            case (Country.ITA):
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
            altitudeSmallestTarget = AltitudeTargetSmallest(airplaneData.planeAttributes.country, airplaneData.altitude);

        altitudeSmallTarget = AltitudeTargetSmall(airplaneData.planeAttributes.country, airplaneData.altitude);
        altitudeLargeTarget = AltitudeTargetLarge(airplaneData.planeAttributes.country, airplaneData.altitude);

        PressureReferenceTargets();
    }

    void PressureReferenceTargets()
    {   //Pressure //mmhg

        //set where we rotate from
        //MmhgStart();

        //set where we are rotating to
        mmhgTarget = AtmosphericPressure(airplaneData.planeAttributes.country, airplaneData.mmhg);

    }

    static Quaternion AtmosphericPressure(Country country, float unit)
    {
        Quaternion target = Quaternion.identity;

        //each country has slightly different dials, we need to work out rotations individually for each
        switch (country)
        {
            case Country.RU:
                target = RussianDials.MmhgTarget(unit);
                break;

            case Country.GER:
                target = GermanDials.MmhgTarget(unit);
                break;

            case Country.US:
                target = USDials.MmhgTarget(unit);
                break;

            case Country.UK:
                target = UKDials.MmhgTarget(unit);
                break;

            case Country.ITA:
                target = ITADials.MmhgTarget(unit);
                break;
        }

        return target;

    }

    static void AirspeedTarget(RotateNeedle rN)
    {
       

        //each country has slightly different dials, we need to work out rotations individually for each
        switch (rN.airplaneData.planeAttributes.country)
        {
            case Country.RU:
                rN.airspeedTarget = RussianDials.AirspeedTarget(rN.airplaneData.airspeed);
                break;

            case Country.GER:
                rN.airspeedTarget = GermanDials.AirspeedTarget(rN.airplaneData.airspeed);
                break;

            case Country.US:
                if(rN.airplaneData.planeAttributes.speedometer == Speedometer.A)
                    rN.airspeedTarget = USDials.AirspeedTargetA(rN.airplaneData.airspeed);
                else
                    rN.airspeedTarget = USDials.AirspeedTargetB(rN.airplaneData.airspeed,rN.airplaneData.scalar0, rN.airplaneData.scalar1);
                break;

            case Country.UK:
                rN.airspeedTarget = UKDials.AirspeedTarget(rN.airplaneData.airspeed);
                break;

            case Country.ITA:
                rN.airspeedTarget = ITADials.AirspeedTarget(rN.airplaneData.airspeed);
                break;
        }
         
    }

    static Quaternion AltitudeTargetLarge(Country country, float altitude)
    {
        Quaternion target = Quaternion.identity;

        //each country has slightly different dials, we need to work out rotations individually for each
        switch (country)
        {
            case Country.RU:
                
                target = RussianDials.AltitudeTargetLarge(altitude);
                break;

            case Country.GER:
                target = GermanDials.AltitudeTargetLarge(altitude);
                break;

            case Country.US:
                target= USDials.AltitudeTargetLarge(altitude);
                break;

            case Country.UK:
                target = UKDials.AltitudeTargetLarge(altitude);
                break;

            case Country.ITA:
                target = ITADials.AltitudeTargetLarge(altitude);
                break;
        }


        return target;
    }

    static Quaternion AltitudeTargetSmall(Country country, float altitude)
    {
        Quaternion target = Quaternion.identity;

        //each country has slightly different dials, we need to work out rotations individually for each
        switch (country)
        {
            case Country.RU:
                target = RussianDials.AltitudeTargetSmall(altitude);
                break;

            case Country.GER:
                target = GermanDials.AltitudeTargetSmall(altitude);
                break;

            case Country.US:
                target = USDials.AltitudeTargetSmall(altitude);
                break;

            case Country.UK:
                target = UKDials.AltitudeTargetSmall(altitude);
                break;

            case Country.ITA:
                target = ITADials.AltitudeTargetSmall(altitude);
                break;
        }


        return target;
    }

    static Quaternion AltitudeTargetSmallest(Country country, float altitude)
    {
        Quaternion target = Quaternion.identity;

        //each country has slightly different dials, we need to work out rotations individually for each
        switch (country)
        {
            //only UK has smallest dial

            case Country.UK:
                target = UKDials.AltitudeTargetSmallest(altitude);
                break;

            case Country.US:
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
                rpmNeedlesLarge[i].transform.rotation = Quaternion.Slerp(rpmNeedlesLarge[i].transform.rotation, rpmLargeTargets[i], Time.deltaTime*smoothing);
        }


        for (int i = 0; i < rpmNeedlesSmall.Count; i++)
        {
            if (rpmNeedlesSmall[i].gameObject != null)
                rpmNeedlesSmall[i].transform.rotation = Quaternion.Slerp(rpmNeedlesSmall[i].transform.rotation, rpmSmallTargets[i], Time.deltaTime * smoothing);

        }
    }

    void AirspeedNeedleRotation()
    {
        //float d = Mathf.Abs( airspeedTarget.eulerAngles.z - quaternionsAirspeed[0].eulerAngles.z);
       // Debug.Log("air needle");
       if( airspeedNeedle != null)
            airspeedNeedle.transform.rotation = Quaternion.Slerp(airspeedNeedle.transform.rotation, airspeedTarget, Time.deltaTime * smoothing); 
        
    }

    void AltitudeNeedleRotations()
    {
        //only UK / US has the smallest needle
        if (altitudeNeedleSmallest != null)
            altitudeNeedleSmallest.transform.rotation = Quaternion.Slerp(altitudeNeedleSmallest.transform.rotation, altitudeSmallestTarget, Time.deltaTime * smoothing);

        if(altitudeNeedleSmall != null)
            altitudeNeedleSmall.transform.rotation = Quaternion.Slerp(altitudeNeedleSmall.transform.rotation, altitudeSmallTarget, Time.deltaTime * smoothing);

        if(altitudeNeedleLarge != null)
            altitudeNeedleLarge.transform.rotation = Quaternion.Slerp(altitudeNeedleLarge.transform.rotation, altitudeLargeTarget, Time.deltaTime * smoothing);
    }

    void MmhgNeedleRotation()
    {
        mmhgDial.transform.rotation = Quaternion.Slerp(mmhgDial.transform.rotation, mmhgTarget, Time.deltaTime * smoothing);
    }

    void TurnCoordinatorRotation()
    {
        //Needle        
        turnCoordinatorNeedle.transform.rotation = Quaternion.Slerp(turnCoordinatorNeedle.transform.rotation, turnCoordinatorNeedleTarget, Time.deltaTime * smoothing);


        //Ball
        turnCoordinatorBall.transform.rotation = Quaternion.Slerp(turnCoordinatorBall.transform.rotation, turnCoordinatorBallTarget, Time.deltaTime * smoothing); 
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
        HeadingTarget(airplaneData.planeAttributes.country);


        return false;
    }
    void HeadingIndicatorRotation()
    {
        HeadingIndicatorSwitch();

        //
        if (float.IsNaN(headingIndicatorTarget.y) || float.IsNaN(headingIndicatorTarget.z))
            return;

        headingIndicator.transform.localPosition = Vector3.Lerp(headingIndicator.transform.localPosition, headingIndicatorTarget, Time.deltaTime * smoothing);

    }

    void TurnAndBankRotations()
    {
        if (turnAndBankPlane != null)
            {
        
            //for x rotatin we need to rotate around global x after z rot
            turnAndBankPlane.transform.rotation = Quaternion.Slerp(turnAndBankPlane.transform.rotation, turnAndBankPlaneRotationTarget, Time.deltaTime * smoothing);

            //move plane up and down
            turnAndBankPlane.transform.localPosition = Vector3.Lerp(turnAndBankPlane.transform.localPosition, turnAndBankPlanePositionTarget, Time.deltaTime * smoothing);

        }

        //number track - only russian
        if(turnAndBankNumberTrack != null)
            turnAndBankNumberTrack.transform.localPosition = Vector3.Lerp(turnAndBankNumberTrack.transform.localPosition, turnAndBankNumberTrackTarget, Time.deltaTime * smoothing);

        //ball -- only german
        if(turnAndBankBall != null)
        {
            turnAndBankBall.transform.rotation = Quaternion.Slerp(turnAndBankBall.transform.rotation, turnAndBankBallTarget, Time.deltaTime * smoothing);
        }
    }

    void VSIRotation()
    {
        vsiNeedle.transform.rotation = Quaternion.Slerp(vsiNeedle.transform.rotation, vsiNeedleTarget,Time.deltaTime * smoothing);
    }

    void RepeaterCompassRotation()
    {
        if (airplaneData.planeAttributes.repeaterCompass)
        {
            if (repeaterCompassFace != null)
                repeaterCompassFace.transform.rotation = Quaternion.Slerp(repeaterCompassFace.transform.rotation, repeaterCompassTarget, Time.deltaTime * smoothing);
        }

        if(airplaneData.planeAttributes.repeaterCompassAlternate)
        {
            if(repeaterCompassAlternateFace != null)
                repeaterCompassAlternateFace.transform.rotation = Quaternion.Slerp(repeaterCompassAlternateFace.transform.rotation, repeaterCompassAlternateTarget, Time.deltaTime * smoothing);
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
            artificialHorizonPlane.transform.rotation = Quaternion.Slerp(artificialHorizonPlane.transform.rotation, artificialHorizonRotationPlaneTarget, Time.deltaTime * smoothing);

        else
            artificialHorizon.transform.rotation = Quaternion.Slerp(artificialHorizon.transform.rotation, artificialHorizonRotationTarget, Time.deltaTime * smoothing);

        //german plane also has co-ordinator needle on this dial
        if(artificialHorizonNeedle != null)
        {
            artificialHorizonNeedle.transform.rotation = Quaternion.Slerp(artificialHorizonNeedle.transform.rotation, artificialHorizonNeedleTarget, Time.deltaTime * smoothing);
        }
    }

    void ArtificialHorizonPosition()
    {

        artificialHorizon.transform.localPosition = Vector3.Lerp(artificialHorizon.transform.localPosition, artificialHorizonPositionTarget, Time.deltaTime * smoothing);

        //clamp plane to background
        if(artificialHorizonPlane != null) //ITA
            artificialHorizonPlane.transform.localPosition = artificialHorizon.transform.localPosition;
    }

    void ArtificialHorizonChevronRotation()
    {
        if(artificialHorizonChevron != null)
            artificialHorizonChevron.transform.rotation = Quaternion.Slerp(artificialHorizonChevron.transform.rotation, artificialHorizonChevronTarget, Time.deltaTime * smoothing);
    }
}
