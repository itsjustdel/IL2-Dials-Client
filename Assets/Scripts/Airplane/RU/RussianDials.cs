using UnityEngine;

public class RussianDials : MonoBehaviour
{
    public static Quaternion AirspeedTarget(float airspeed)
    {
        if (airspeed == 0)
            return Quaternion.identity;

        //airspeed dial has three gears
        //below 100
        Quaternion target;// = Quaternion.identity;
        if (airspeed < 100)
            //if 0 -> 0
            //if 50 -> 15
            //if 100 -> 30
            target = Quaternion.Euler(0, 0, -((airspeed / 50f) * 15f));

        //below 300
        else if (airspeed < 300)
            //for every 5 kmh, move 30 degrees            
            //if 100 -> 30
            //if 150 -> 60
            //if 200 -> 90
            target = Quaternion.Euler(0, 0, -((airspeed / 50f) * 30f) + 30f);
        else
            //over 300
            //for evry 10, move 40, but starts at 300 (150degrees)
            //if 600 -> 270
            //if 700 -> 310            
            target = Quaternion.Euler(0, 0, -((airspeed / 10f) * 4) - 30);
        return target;
    }

    public static Quaternion AltitudeTargetSmall(float altitude)
    {

        Quaternion altitudeSmallTarget = Quaternion.Euler(0, 0, -(altitude / 10000f) * 360);

        return altitudeSmallTarget;

    }

    public static Quaternion AltitudeTargetLarge(float altitude)
    {
        Quaternion altitudeLargeTarget = Quaternion.Euler(0, 0, -(altitude / 1000f) * 360);
        return altitudeLargeTarget;

    }

    public static Quaternion MmhgTarget(float mmhg)
    {

        float z = ((760f - mmhg) / 100f) * 300;

        Quaternion mmhgTarget = Quaternion.identity;

        //catch bad value
        if (!float.IsNaN(z))
            mmhgTarget = Quaternion.Euler(0, 0, z);

        return mmhgTarget;
    }

    public static Vector3 HeadingIndicatorPosition(float heading, float trackLength)
    {
        //check for Nan
        if (float.IsNaN(heading))
            return Vector3.zero;

        //range is 0 to pi*2
        float ratio = Mathf.PI * heading;
        //adjust for arbitry render camera position
        ratio *= 10.99f;

        Vector3 pos = Vector3.right * ratio;
        //track length
        pos += Vector3.right * trackLength;


        return pos;
    }

    public static Quaternion TurnAndBankPlaneRotation(float roll, float climb, float rollMultiplier, float xRotation)
    {
        //rotate plane
        //clamp roll , in game cockpit stop rotation at just under 90 degrees - this happens when roll rate is ~1.7
        float tempRoll = -roll;
        Mathf.Clamp(tempRoll, -1.7f, 1.7f);
        Quaternion t = Quaternion.Euler(0, 0, tempRoll * rollMultiplier);
        Quaternion r = t;

        //for x rotatin we need to rotate around global x after z rot
        r *= Quaternion.Euler(climb * xRotation, 0, 0);

        return r;
    }

    public static Vector3 TurnAndBankPlanePosition(float climb, float pitchMultiplier)
    {
        //move plane up and down
        return new Vector3(0, climb * pitchMultiplier, 0);
    }

    public static Vector3 TurnAndBankNumberTrackPosition(float climb, float pitchMultiplier)
    {
        //number track
        return new Vector3(0, climb * pitchMultiplier, 1.5f);
    }


    public static Quaternion TurnCoordinatorNeedleTarget(float v)
    {
        // ~.63 is max value in game data (on russian needle) so pi*2 perhaps - still confused with what's going on here
        // we get the the value from 0 to 1. 0.5 is centre, range of degrees is 60 (?)
        float z = (v - .5f) * Mathf.PI * 60f;
        z = Mathf.Clamp(z, -30, 30);
        Quaternion target = Quaternion.Euler(0, 0, -z);

        return target;
    }


    public static Quaternion TurnCoordinatorBallTarget(float ball)
    {
        //indicates whether the aircraft is in coordinated flight, showing the slip or skid of the turn. 
        float z = ball * 550;
        z = Mathf.Clamp(z, -19.5f, 19.5f);//test
        Quaternion target = Quaternion.Euler(0, 0, z);

        return target;
    }

    public static Quaternion VerticalSpeedTargetLarge(float verticalSpeed)
    {
        //vsi
        //start at 9 o'clock
        verticalSpeed = 90f - verticalSpeed * 6f;
        //clamp to "30"
        verticalSpeed = Mathf.Clamp(verticalSpeed, -90, 270);

        Quaternion target = Quaternion.Euler(0, 0, verticalSpeed);

        return target;
    }

    public static Quaternion VerticalSpeedTargetSmall(float verticalSpeed)
    {
        //vsi
        //start at 9 o'clock
        verticalSpeed = 90f - verticalSpeed * 18f;
        //clamp to "10"
        verticalSpeed = Mathf.Clamp(verticalSpeed, -90, 270);

        Quaternion target = Quaternion.Euler(0, 0, verticalSpeed);

        return target;
    }

    public static Quaternion RPMALargeTarget(float rpm)
    {
        float r = rpm * -0.036f;
        Quaternion target = Quaternion.Euler(0, 0, r);

        return target;
    }

    //2nd needle on two needle rpm
    public static Quaternion RPMASmallTarget(float rpm)
    {
        float r = rpm * -0.36f;
        Quaternion target = Quaternion.Euler(0, 0, r);

        return target;
    }


    public static Quaternion RPMBTarget(float rpm, float scalar, float scalar2)
    {
        // -0.12275*
        //209.135
        float start = 209.135f;
        float r = rpm * -0.12275f + (start);

        //clamp low is actually high, rotation are negative
        r = Mathf.Clamp(r, -180, 160);

        Quaternion target = Quaternion.Euler(0, 0, r);

        return target;
    }


    public static Quaternion RPMCSmallTarget(float rpm)
    {
        float r = rpm * -0.036f;
        Quaternion target = Quaternion.Euler(0, 0, r);

        return target;
    }

    public static Quaternion RPMCLargeTarget(float rpm)
    {
        float r = rpm * -0.36f;
        Quaternion target = Quaternion.Euler(0, 0, r);

        return target;
    }

    public static Quaternion ManifoldTargetAB(float manifold, float scalar, float scalarB)
    {
        //km/cm2 to mm of Hg
        //manifold *= 7.35592400690826f;
        //manifold *= scalar;
        float m = 0;
        // resting position in game
        if (manifold <= 315)
            m = 111f;
        else
        {
            m = (manifold) * -0.25f;
            m += 190;
        }

        Quaternion target = Quaternion.Euler(0, 0, m);

        return target;
    }

    public static Quaternion ManifoldTargetC(float manifold, float scalar)
    {
        //km/cm2 to mm of Hg
        //manifold *= 7.35592400690826f;
        float m = 0;
        if (manifold <= 300)
            m = 135;
        else
        {
            m = (manifold) * -.3f;
            m += 225;
        }

        Quaternion target = Quaternion.Euler(0, 0, m);

        return target;
    }

    internal static Quaternion WaterTempTargetA(float v, float scalar0, float scalar1)
    {
        v = Mathf.Clamp(v, 0, 120);
        float r = v * -.8f;
        Quaternion target = Quaternion.Euler(0, 0, r + 48);

        return target;
    }

    internal static Quaternion WaterTempTargetB(float v, float scalar0, float scalar1, string name)
    {
        if (name == "Yak-9 ser.1")// || name == "Yak-9T ser.1")
        {
            v *= 1.03f; // Don't know why
        }

        v = Mathf.Clamp(v, 0, 125);
        float r = v * -2.4f;
        Quaternion target = Quaternion.Euler(0, 0, r + 150);

        return target;
    }

    internal static Quaternion WaterTempTargetC(float v, float scalar0, float scalar1)
    {
        v = Mathf.Clamp(v, 0, 160);
        float r = v * -.6f;
        Quaternion target = Quaternion.Euler(0, 0, r + 48);

        return target;
    }

    internal static Quaternion OilTempCombo(float v, float scalar0, float scalar1)
    {
        v = Mathf.Clamp(v, 0, 125);
        float r = v * -1.438f;
        Quaternion target = Quaternion.Euler(0, 0, r + 90);

        return target;
    }

    internal static Quaternion OilTempInA(float v, float scalar0, float scalar1)
    {
        v = Mathf.Clamp(v, 0, 120);
        float r = v * -.8f;
        Quaternion target = Quaternion.Euler(0, 0, r + 48);

        return target;
    }

    internal static Quaternion CylinderHeadTempTargetA(float v, float scalar0, float scalar1)
    {
        v = Mathf.Clamp(v, 0, 350f);
        float r = v * -0.27f;
        Quaternion target = Quaternion.Euler(0, 0, r + 47.25f);

        return target;
    }
}
