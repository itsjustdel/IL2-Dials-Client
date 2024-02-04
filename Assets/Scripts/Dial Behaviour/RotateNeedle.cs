using System.Collections.Generic;
using UnityEngine;

public class RotateNeedle : MonoBehaviour
{
    public BuildControl buildControl;
    public AirplaneData airplaneData;
    public UDPClient udpClient;
    public float smoothing = 3f;
    public float manifoldSmoothing = 1f;
    public float turnNeedleSmoothing = 1f;

    public GameObject altitudeNeedleSmall;
    public GameObject altitudeNeedleSmallest;//UK//US
    public GameObject altitudeNeedleLarge;
    public GameObject mmhgDial;
    public GameObject airspeedNeedle;
    public GameObject turnAndBankNumberTrack;
    public GameObject turnAndBankPlane;
    public GameObject headingIndicator;
    public GameObject headingIndicatorBall;
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
    public List<GameObject> manifoldNeedlesSmall = new List<GameObject>();
    public List<GameObject> manifoldNeedlesLarge = new List<GameObject>();
    public List<GameObject> waterTempNeedles = new List<GameObject>();
    public List<GameObject> oilTempInNeedles = new List<GameObject>();
    public List<GameObject> oilTempOutNeedles = new List<GameObject>();
    public List<GameObject> oilTempPressureNeedles = new List<GameObject>();
    public List<GameObject> oilTempComboNeedles = new List<GameObject>();
    public List<GameObject> cylinderHeadNeedles = new List<GameObject>();
    public List<GameObject> carbTempNeedles = new List<GameObject>();

    public float previousMessageTime;
    public float maxSpin = 1f;
    public float turnAndBankPitchMultiplier = 5f;
    public float turnAndBankRollMultiplier = 5f;
    public float turnAndBankPlaneXMultiplier = 5f;

    // -- positions
    private List<Vector3> positionsHeading = new List<Vector3>() { Vector3.zero, Vector3.zero };    //needed?

    //  private bool saveForPredictions -- rotations

    private Quaternion airspeedTarget;
    private Quaternion altitudeLargeTarget;
    private Quaternion altitudeSmallTarget;
    private Quaternion altitudeSmallestTarget;
    private Quaternion mmhgTarget;
    private Quaternion turnAndBankPlaneRotationTarget;
    private Quaternion turnCoordinatorNeedleTarget;
    private Quaternion turnCoordinatorBallTarget;
    private Quaternion headingIndicatorBallTarget;
    private Quaternion vsiNeedleTarget;
    private Quaternion repeaterCompassTarget;
    private Quaternion repeaterCompassAlternateTarget;
    private Quaternion artificialHorizonRotationTarget;
    private Quaternion artificialHorizonNeedleTarget;
    private Quaternion artificialHorizonChevronTarget;
    private Quaternion artificialHorizonRotationPlaneTarget;//if dial has seperate background and plane
    public List<Quaternion> rpmLargeTargets = new List<Quaternion>();
    public List<Quaternion> rpmSmallTargets = new List<Quaternion>();
    public List<Quaternion> manifoldLargeTargets = new List<Quaternion>();
    public List<Quaternion> waterTempTargets = new List<Quaternion>();
    public List<Quaternion> oilTempInTargets = new List<Quaternion>();
    public List<Quaternion> oilTempOutTargets = new List<Quaternion>();
    public List<Quaternion> oilTempPressureTargets = new List<Quaternion>();
    public List<Quaternion> oilTempComboTargets = new List<Quaternion>();
    public List<Quaternion> cylinderHeadTargets = new List<Quaternion>();
    public List<Quaternion> carbAirTargets = new List<Quaternion>();
    private Quaternion turnAndBankBallTarget;

    // -- positions
    //heading is on a track, we move along the x, we don't rotate
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
    public AnimationCurve animationCurveVSIB;

    public AnimationCurve animationCurveRPMA;
    public AnimationCurve animationCurveRPMB;
    public AnimationCurve animationCurveRPMC;
    public AnimationCurve animationCurveRPMD;
    public AnimationCurve animationCurveSpeedometerA;
    public AnimationCurve animationCurveWaterTempA;
    public AnimationCurve animationCurveOilTempF;
    private bool headingIndicatorTest;

    public bool germanWaterOilSwitch;
    public GameObject waterTempButton;

    // Start is called before the first frame update
    void Start()
    {
        //initialise lists
        rpmSmallTargets = new List<Quaternion>(new Quaternion[airplaneData.planeAttributes.engines]);
        rpmLargeTargets = new List<Quaternion>(new Quaternion[airplaneData.planeAttributes.engines]);
        manifoldLargeTargets = new List<Quaternion>(new Quaternion[airplaneData.planeAttributes.engines]);
        manifoldLargeTargets = new List<Quaternion>(new Quaternion[airplaneData.planeAttributes.engines]);
        waterTempTargets = new List<Quaternion>(new Quaternion[airplaneData.planeAttributes.engines]);
        oilTempInTargets = new List<Quaternion>(new Quaternion[airplaneData.planeAttributes.engines]);
        oilTempOutTargets = new List<Quaternion>(new Quaternion[airplaneData.planeAttributes.engines]);
        oilTempPressureTargets = new List<Quaternion>(new Quaternion[airplaneData.planeAttributes.engines]);
        cylinderHeadTargets = new List<Quaternion>(new Quaternion[airplaneData.planeAttributes.engines]);
        carbAirTargets = new List<Quaternion>(new Quaternion[airplaneData.planeAttributes.engines]);
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
        WaterTempButtonSize();
    }

    void WaterTempButtonSize()
    {
        if (waterTempButton == null)
            return;

        //show visual change
        if (germanWaterOilSwitch)
        {
            //if pressed, make smaller
            waterTempButton.transform.localScale = new Vector3(1.73791f * 0.95f, 1.73791f * 0.95f, 1f);
        }
        else
        {
            waterTempButton.transform.localScale = new Vector3(1.73791f, 1.73791f, 1f);//value direct from ui
        }
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

            //switch direction
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
        //initial refactoring, could go further, lots of parameters
        airspeedTarget = DialTargets.AirspeedTarget(airplaneData.planeAttributes.country, airplaneData, this);

        altitudeLargeTarget = DialTargets.AltimeterTargets(ref altitudeSmallTarget, ref altitudeSmallestTarget, altitudeNeedleSmallest, airplaneData);

        mmhgTarget = DialTargets.PressureReferenceTargets(airplaneData);

        headingIndicatorTarget = DialTargets.HeadingTarget(airplaneData.planeAttributes.country, airplaneData, trackLength);

        headingIndicatorBallTarget = DialTargets.HeadingIndicatorBallTarget(airplaneData);

        turnAndBankPlaneRotationTarget = DialTargets.TurnAndBankTargets(ref turnAndBankPlanePositionTarget, ref turnAndBankNumberTrackTarget, ref turnAndBankBallTarget,
                                                                        airplaneData, turnAndBankPitchMultiplier, turnAndBankRollMultiplier, turnAndBankPlaneXMultiplier,
                                                                        turnAndBankBallMultiplier, airplaneData.planeAttributes.country);

        turnCoordinatorNeedleTarget = DialTargets.TurnCoordinatorTarget(ref turnCoordinatorBallTarget, airplaneData, turnCoordinaterNeedleMod, turnCoordinaterBallMod, airplaneData.planeAttributes.country);

        vsiNeedleTarget = DialTargets.VSITarget(airplaneData, airplaneData.planeAttributes.country, this);

        repeaterCompassTarget = DialTargets.RepeaterCompassTarget(ref repeaterCompassAlternateTarget, airplaneData, compassRim, airplaneData.planeAttributes.country);

        artificialHorizonRotationTarget = DialTargets.ArtificialHorizonTargets(ref artificialHorizonNeedleTarget, ref artificialHorizonPositionTarget, ref artificialHorizonChevronTarget, ref artificialHorizonRotationPlaneTarget,
                                                                                airplaneData, artificialHorizonNeedle,
                                                                                artificialHorizonRollMod, artificialHorizonMultiplier, airplaneData.planeAttributes.country);

        //rpms 
        List<List<Quaternion>> rpmTargetsList = DialTargets.RPMTarget(airplaneData, airplaneData.planeAttributes.country, this);
        rpmSmallTargets = rpmTargetsList[0];
        rpmLargeTargets = rpmTargetsList[1];


        //manifolds
        List<List<Quaternion>> manifoldTargets = DialTargets.ManifoldTarget(airplaneData, airplaneData.planeAttributes.country);
        manifoldLargeTargets = manifoldTargets[1];

        //water temps
        waterTempTargets = DialTargets.WaterTempTargets(airplaneData, airplaneData.planeAttributes.country, this);

        //oil temps
        oilTempInTargets = DialTargets.OilTempInTargets(airplaneData, airplaneData.planeAttributes.country, this);
        oilTempOutTargets = DialTargets.OilTempOutTargets(airplaneData, airplaneData.planeAttributes.country, this);
        oilTempPressureTargets = DialTargets.OilTempPressureTargets(airplaneData, airplaneData.planeAttributes.country, this);
        oilTempComboTargets = DialTargets.OilTempComboTargets(airplaneData, airplaneData.planeAttributes.country, this);

        //cylinder head
        cylinderHeadTargets = DialTargets.CylinderHeadTargets(airplaneData, airplaneData.planeAttributes.country);

        //carb air
        carbAirTargets = DialTargets.CarbAirTargets(airplaneData, airplaneData.planeAttributes.country);
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

        ManifoldRotations();

        WaterTempRotations();

        OilTempInRotations();

        OilTempOutRotations();

        OilTempPressureRotations();

        OilTempComboRotations();

        CylinderHeadRotations();

        CarbAirRotations();
    }

    private void CylinderHeadRotations()
    {
        for (int i = 0; i < cylinderHeadNeedles.Count; i++)
        {
            if (cylinderHeadNeedles[i].gameObject != null)
                cylinderHeadNeedles[i].transform.rotation = Quaternion.Slerp(cylinderHeadNeedles[i].transform.rotation, cylinderHeadTargets[i], Time.deltaTime * smoothing);
        }
    }

    private void CarbAirRotations()
    {
        for (int i = 0; i < carbTempNeedles.Count; i++)
        {
            if (carbTempNeedles[i].gameObject != null)
                carbTempNeedles[i].transform.rotation = Quaternion.Slerp(carbTempNeedles[i].transform.rotation, carbAirTargets[i], Time.deltaTime * smoothing);
        }
    }

    private void OilTempComboRotations()
    {
        for (int i = 0; i < oilTempComboNeedles.Count; i++)
        {
            if (oilTempComboNeedles[i].gameObject != null)
                oilTempComboNeedles[i].transform.rotation = Quaternion.Slerp(oilTempComboNeedles[i].transform.rotation, oilTempComboTargets[i], Time.deltaTime * smoothing);
        }
    }

    private void OilTempPressureRotations()
    {
        for (int i = 0; i < oilTempPressureNeedles.Count; i++)
        {
            if (oilTempPressureNeedles[i].gameObject != null)
                oilTempPressureNeedles[i].transform.rotation = Quaternion.Slerp(oilTempPressureNeedles[i].transform.rotation, oilTempPressureTargets[i], Time.deltaTime * smoothing);
        }
    }

    private void OilTempOutRotations()
    {
        for (int i = 0; i < oilTempOutNeedles.Count; i++)
        {
            if (oilTempOutNeedles[i].gameObject != null)
                oilTempOutNeedles[i].transform.rotation = Quaternion.Slerp(oilTempOutNeedles[i].transform.rotation, oilTempOutTargets[i], Time.deltaTime * smoothing);
        }
    }

    private void OilTempInRotations()
    {
        for (int i = 0; i < oilTempInNeedles.Count; i++)
        {
            if (oilTempInNeedles[i].gameObject != null)
                oilTempInNeedles[i].transform.rotation = Quaternion.Slerp(oilTempInNeedles[i].transform.rotation, oilTempInTargets[i], Time.deltaTime * smoothing);
        }
    }

    private void WaterTempRotations()
    {
        for (int i = 0; i < waterTempNeedles.Count; i++)
        {
            if (waterTempNeedles[i].gameObject != null)
            {
                if (!germanWaterOilSwitch)
                    waterTempNeedles[i].transform.rotation = Quaternion.Slerp(waterTempNeedles[i].transform.rotation, waterTempTargets[i], Time.deltaTime * smoothing);
                else
                    // bf 109 oil is inbound https://airpages.ru/eng/lw/109k_01.shtml
                    waterTempNeedles[i].transform.rotation = Quaternion.Slerp(waterTempNeedles[i].transform.rotation, oilTempInTargets[i], Time.deltaTime * smoothing);
            }
        }
    }

    private void ManifoldRotations()
    {
        for (int i = 0; i < manifoldNeedlesLarge.Count; i++)
        {
            if (manifoldNeedlesLarge[i].gameObject != null)
                manifoldNeedlesLarge[i].transform.rotation = Quaternion.Slerp(manifoldNeedlesLarge[i].transform.rotation, manifoldLargeTargets[i], Time.deltaTime * manifoldSmoothing);
        }
    }

    void RPMRotations()
    {
        for (int i = 0; i < rpmNeedlesLarge.Count; i++)
        {
            if (rpmNeedlesLarge[i].gameObject != null)
                rpmNeedlesLarge[i].transform.rotation = Quaternion.Slerp(rpmNeedlesLarge[i].transform.rotation, rpmLargeTargets[i], Time.deltaTime * smoothing);
        }


        for (int i = 0; i < rpmNeedlesSmall.Count; i++)
        {
            if (rpmNeedlesSmall[i].gameObject != null)
                rpmNeedlesSmall[i].transform.rotation = Quaternion.Slerp(rpmNeedlesSmall[i].transform.rotation, rpmSmallTargets[i], Time.deltaTime * smoothing);
        }
    }

    void AirspeedNeedleRotation()
    {
        if (airspeedNeedle != null)
            airspeedNeedle.transform.rotation = Quaternion.Slerp(airspeedNeedle.transform.rotation, airspeedTarget, Time.deltaTime * smoothing);
    }

    void AltitudeNeedleRotations()
    {
        //only UK / US has the smallest needle
        if (altitudeNeedleSmallest != null)
            altitudeNeedleSmallest.transform.rotation = Quaternion.Slerp(altitudeNeedleSmallest.transform.rotation, altitudeSmallestTarget, Time.deltaTime * smoothing);

        if (altitudeNeedleSmall != null)
            altitudeNeedleSmall.transform.rotation = Quaternion.Slerp(altitudeNeedleSmall.transform.rotation, altitudeSmallTarget, Time.deltaTime * smoothing);

        if (altitudeNeedleLarge != null)
            altitudeNeedleLarge.transform.rotation = Quaternion.Slerp(altitudeNeedleLarge.transform.rotation, altitudeLargeTarget, Time.deltaTime * smoothing);
    }

    void MmhgNeedleRotation()
    {
        mmhgDial.transform.rotation = Quaternion.Slerp(mmhgDial.transform.rotation, mmhgTarget, Time.deltaTime * smoothing);
    }

    void TurnCoordinatorRotation()
    {
        //Needle        
        turnCoordinatorNeedle.transform.rotation = Quaternion.Slerp(turnCoordinatorNeedle.transform.rotation, turnCoordinatorNeedleTarget, Time.deltaTime * turnNeedleSmoothing);

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
        headingIndicatorTarget = DialTargets.HeadingTarget(airplaneData.planeAttributes.country, airplaneData, trackLength);


        return false;
    }

    void HeadingIndicatorRotation()
    {
        HeadingIndicatorSwitch();

        if (float.IsNaN(headingIndicatorTarget.y) || float.IsNaN(headingIndicatorTarget.z))
            return;

        headingIndicator.transform.localPosition = Vector3.Lerp(headingIndicator.transform.localPosition, headingIndicatorTarget, Time.deltaTime * smoothing);

        if (airplaneData.planeAttributes.headingIndicatorType == DialVariant.B)
        {
            //a-20 has combined ball and heading indicator
            headingIndicatorBall.transform.rotation = Quaternion.Slerp(headingIndicatorBall.transform.rotation, headingIndicatorBallTarget, Time.deltaTime * smoothing);
        }
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
        if (turnAndBankNumberTrack != null)
            turnAndBankNumberTrack.transform.localPosition = Vector3.Lerp(turnAndBankNumberTrack.transform.localPosition, turnAndBankNumberTrackTarget, Time.deltaTime * smoothing);

        //ball -- only german
        if (turnAndBankBall != null)
        {
            turnAndBankBall.transform.rotation = Quaternion.Slerp(turnAndBankBall.transform.rotation, turnAndBankBallTarget, Time.deltaTime * smoothing);
        }
    }

    void VSIRotation()
    {
        vsiNeedle.transform.rotation = Quaternion.Slerp(vsiNeedle.transform.rotation, vsiNeedleTarget, Time.deltaTime * smoothing);
    }

    void RepeaterCompassRotation()
    {
        if (airplaneData.planeAttributes.repeaterCompass)
        {
            if (repeaterCompassFace != null)
                repeaterCompassFace.transform.rotation = Quaternion.Slerp(repeaterCompassFace.transform.rotation, repeaterCompassTarget, Time.deltaTime * smoothing);
        }

        if (airplaneData.planeAttributes.repeaterCompassAlternate)
        {
            if (repeaterCompassAlternateFace != null)
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
        if (artificialHorizonNeedle != null)
        {
            artificialHorizonNeedle.transform.rotation = Quaternion.Slerp(artificialHorizonNeedle.transform.rotation, artificialHorizonNeedleTarget, Time.deltaTime * smoothing);
        }
    }

    void ArtificialHorizonPosition()
    {

        artificialHorizon.transform.localPosition = Vector3.Lerp(artificialHorizon.transform.localPosition, artificialHorizonPositionTarget, Time.deltaTime * smoothing);

        //clamp plane to background
        if (artificialHorizonPlane != null) //ITA
            artificialHorizonPlane.transform.localPosition = artificialHorizon.transform.localPosition;
    }

    void ArtificialHorizonChevronRotation()
    {
        if (artificialHorizonChevron != null)
            artificialHorizonChevron.transform.rotation = Quaternion.Slerp(artificialHorizonChevron.transform.rotation, artificialHorizonChevronTarget, Time.deltaTime * smoothing);
    }

    public void OpenKeyCodePanel()
    {
        //Inputs objects holds reference to de-activated key code panel. "Find" cannot find disactivated objects
        GameObject.FindGameObjectWithTag("MenuObject").GetComponent<MenuHandler>().EnableKeyCodePanel();
    }
}
