using UnityEngine;

public class GermanDials : MonoBehaviour
{
    public static Quaternion AirspeedTarget(float airspeed)
    {
        if (float.IsNaN(airspeed))
            return Quaternion.identity;


        if (airspeed == 0)
            return Quaternion.Euler(0, 0, 180);

        //airspeed dial has two gears
        Quaternion target = Quaternion.identity;
        if (airspeed < 100)
        {
            target = Quaternion.Euler(0, 0, -((airspeed) * 0.15f) - 180);//smaller step size
        }
        else
        {
            target = Quaternion.Euler(0, 0, -((airspeed) * 0.5f) - 195 + 50);//195 is wher "100" is and 50 is the step
        }

        return target;
    }

    public static Quaternion AltitudeTargetSmall(float altitude)
    {
        if (altitude == 0f)
            return Quaternion.identity;

        Quaternion altitudeSmallTarget = Quaternion.Euler(0, 0, -(altitude / 10000f) * 360);

        return altitudeSmallTarget;

    }

    public static Quaternion AltitudeTargetLarge(float altitude)
    {
        if (altitude == 0f)
            return Quaternion.identity;

        Quaternion altitudeLargeTarget = Quaternion.Euler(0, 0, -((altitude / 1000f) * 360) + 180);
        return altitudeLargeTarget;

    }

    public static Quaternion MmhgTarget(float mmhg)
    {


        //mmhg to mbar
        float input = mmhg * 1.333f;
        float z = ((1013.25f - input) * 1f) - 3.25f;
        Quaternion mmhgTarget = Quaternion.identity;

        //catch bad value
        try
        {
            if (!float.IsNaN(z))
                mmhgTarget = Quaternion.Euler(0, 0, z); // 0 is 1013.25 mbar //0 degrees // bit more confusing because of asset rotation
        }
        catch
        {
            return Quaternion.identity;
        }

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
        ratio *= 11.89f;

        Vector3 pos = Vector3.right * ratio;
        pos += Vector3.right * trackLength;

        return pos;
    }


    //turn and bank
    public static Quaternion TurnAndBankPlaneRotation(float roll, float climb, float rollMultiplier, float climbMultiplier)
    {
        //rotate plane
        //clamp roll , in game cockpit stop rotation at just under 90 degrees - this happens when roll rate is ~1.7
        float tempRoll = -roll;
        Mathf.Clamp(tempRoll, -1.7f, 1.7f);
        Quaternion t = Quaternion.Euler(0, 0, tempRoll * rollMultiplier);
        Quaternion r = t;

        //for x rotatin we need to rotate around global x after z rot
        //r *= Quaternion.Euler(climb * climbMultiplier, 0, 0);

        return r;
    }

    public static Vector3 TurnAndBankPlanePosition(float climb, float pitchMultiplier)
    {
        //move plane up and down
        return new Vector3(0, climb * pitchMultiplier - 3.61f, 0);
    }

    //turn co
    public static Quaternion TurnCoordinatorNeedleTarget(float v, string planeType)
    {

        // ~.63 is max value in game data (on russian needle) so pi*2 perhaps - still confused with what's going on here
        // we get the the value from 0 to 1. 0.5 is centre, range of degrees is 60 (?)
        float z = (v - .5f) * Mathf.PI * 60f;

        if (planeType == "Hs 129 B-2")
            z *= 0.333f;

        z = Mathf.Clamp(z, -30, 30);
        Quaternion target = Quaternion.Euler(0, 0, -z);

        return target;
    }


    public static Quaternion TurnCoordinatorBallTarget(float ball, float mod)
    {
        //indicates whether the aircraft is in coordinated flight, showing the slip or skid of the turn. 
        float z = ball * mod;
        z = Mathf.Clamp(z, -12f, 12f);
        Quaternion target = Quaternion.Euler(0, 0, z);

        return target;
    }

    public static Quaternion TurnAndBankBallTarget(float ball, float multiplier)
    {
        //indicates whether the aircraft is in coordinated flight, showing the slip or skid of the turn. 
        float z = ball * multiplier;
        z = Mathf.Clamp(z, -15f, 15f);
        Quaternion target = Quaternion.Euler(0, 0, z);

        return target;
    }


    public static Quaternion RepeaterCompassTarget(float heading, float rimSpin)
    {
        //number passed is rotation in rads, pi = 180 degrees
        //rim spin is how much of an offset to add if user has spun the outer rim to navigate
        Quaternion target = Quaternion.Euler(0, 0, -heading * Mathf.Rad2Deg + rimSpin);

        return target;
    }

    public static Quaternion RepeaterCompassAlternateTarget(float heading)
    {
        //number passed is rotation in rads, pi = 180 degrees
        Quaternion target = Quaternion.Euler(0, 0, heading * Mathf.Rad2Deg);

        return target;
    }

    public static Quaternion VerticalSpeedTargetSmallest(float verticalSpeed)
    {
        //vsi
        //start at 9 o'clock
        verticalSpeed = 90f - verticalSpeed * 18f;
        //clamp to "10"
        verticalSpeed = Mathf.Clamp(verticalSpeed, -90, 270);

        Quaternion target = Quaternion.Euler(0, 0, verticalSpeed);

        return target;
    }

    public static Quaternion VerticalSpeedTargetSmall(float verticalSpeed)
    {
        //vsi
        //start at 9 o'clock
        verticalSpeed = 90f - verticalSpeed * 9f;
        //clamp to "10"
        verticalSpeed = Mathf.Clamp(verticalSpeed, -90, 270);

        Quaternion target = Quaternion.Euler(0, 0, verticalSpeed);

        return target;
    }


    public static Quaternion VerticalSpeedTargetLarge(float verticalSpeed)
    {
        //vsi

        //geared

        if (Mathf.Abs(verticalSpeed) <= 5)
        {
            //start at 9 o'clock
            verticalSpeed = 90f - verticalSpeed * 9f;
        }
        else if (Mathf.Abs(verticalSpeed) > 5 && Mathf.Abs(verticalSpeed) < 10f)
        {
            //create rotation by creating new start point ( verticalSpeed - 5 degrees )
            if (verticalSpeed > 0)
                verticalSpeed = 90f - ((verticalSpeed - 5f) * 6f) - 45f;
            else
                verticalSpeed = 90f - ((verticalSpeed + 5f) * 6f) + 45f;
        }
        else
        {
            if (verticalSpeed > 0)
                verticalSpeed = 90f - (verticalSpeed * 3f) - 45f;
            else
                verticalSpeed = 90f - (verticalSpeed * 3f) + 45f;

        }
        //clamp to "30"
        verticalSpeed = Mathf.Clamp(verticalSpeed, -45, 225);

        Quaternion target = Quaternion.Euler(0, 0, verticalSpeed);

        return target;
    }

    public static Quaternion ArtificialHorizon(float roll, float rollMultiplier)
    {
        //rotate horizon        

        Quaternion t = Quaternion.Euler(0, 0, roll * rollMultiplier);

        //for x rotation we need to rotate around global x after z rot
        //t *= Quaternion.Euler(climb * climbMultiplier, 0, 0);

        return t;
    }

    public static Vector3 ArtificialHorizonPosition(float climb, float pitchMultiplier)
    {
        //move plane up and down
        return new Vector3(0, climb * pitchMultiplier, 0);
    }

    public static Quaternion RPMATarget(float rpm, float scalar, float scalar2)
    {
        float r = rpm * -0.11f + (230);

        //clamp low is actually high, rotation are negative
        r = Mathf.Clamp(r, -180, 164);//164 i "6" on dial if want to clamp to that

        Quaternion target = Quaternion.Euler(0, 0, r);

        return target;
    }

    public static Quaternion RPMBTarget(float rpm, float scalar, float scalar1, AnimationCurve curve)
    {
        //-315 full needle spin to 3000
        //and work out percentage to use 0-1 scale for curve
        float highest = 3000;
        float percentage = (Mathf.Abs(rpm / highest));

        //multiply by half a dial of spin (180 degrees)
        float angleToSpin = curve.Evaluate(percentage);
        // Debug.Log(angleToSpin);
        angleToSpin *= -315;// scalar1;

        //put negative back?


        // if (negative)
        //   angleToSpin *= -1;

        angleToSpin -= 180;

        //offset by 90 degrees - vsi starts at 9 0'clock on the dial
        //verticalSpeed = 90f - angleToSpin;

        //set to quaternion
        Quaternion target = Quaternion.Euler(0, 0, angleToSpin);


        return target;
    }


    public static Quaternion RPMCTarget(float rpm, float scalar, float scalar1, AnimationCurve curve)
    {
        //-315 full needle spin to 3000
        //and work out percentage to use 0-1 scale for curve
        float highest = 3500;
        float percentage = (Mathf.Abs(rpm / highest));

        //multiply by half a dial of spin (180 degrees)
        float angleToSpin = curve.Evaluate(percentage);
        // Debug.Log(angleToSpin);
        angleToSpin *= -315;

        //put negative back?


        // if (negative)
        //   angleToSpin *= -1;

        angleToSpin -= 180;

        //offset by 90 degrees - vsi starts at 9 0'clock on the dial
        //verticalSpeed = 90f - angleToSpin;

        //set to quaternion
        Quaternion target = Quaternion.Euler(0, 0, angleToSpin);


        return target;
    }

    public static Quaternion RPMDTarget(float rpm, float scalar, float scalar1, AnimationCurve curve)
    {
        //-315 full needle spin to 3000
        //and work out percentage to use 0-1 scale for curve
        float highest = 14000;
        float percentage = (Mathf.Abs(rpm / highest));

        //multiply by half a dial of spin (180 degrees)
        float angleToSpin = curve.Evaluate(percentage);

        angleToSpin *= -307;

        angleToSpin -= 180;

        //set to quaternion
        Quaternion target = Quaternion.Euler(0, 0, angleToSpin);


        return target;
    }


    public static Quaternion ManifoldTargetA(float manifold, string planeType, int engineMod, float s)
    {
        //planes need nudged, not consistent to manifold value
        //   if (planeType == "Bf 109 E-7" || planeType == "Bf-110 E2")
        //     manifold *= 1.0225f;
        //else
        //  manifold *= 0.9875f;


        //account for different engines
        float upperLimit = 1800;
        if (planeType == "Bf 109 K-4" && engineMod == 1)
            upperLimit = 2000;

        float m = 0;
        if (manifold <= 600)
            m = 166.156f;

        else if (upperLimit == 1800 && manifold > upperLimit)
            m = -166.156f;

        else if (manifold <= upperLimit)
        {
            m = (manifold - 600) * -0.2766667f;//can be same as below now for k-4?
            m += 166.156f;
        }
        else
        {
            m = (manifold - 600) * -0.275f;
            m += 166.156f;
        }

        Quaternion target = Quaternion.Euler(0, 0, m);

        return target;
    }


    public static Quaternion ManifoldTargetD(float manifold, string planeType, float s)
    {

        float m = 0;
        if (manifold <= 500)
            m = 160;
        else
        {
            m = (manifold - 500) * -0.16f;
            m += 160;
        }

        Quaternion target = Quaternion.Euler(0, 0, m);

        return target;
    }

    public static Quaternion ManifoldTargetE(float manifold, string planeType, float s)
    {
        //if (planeType == "He 111 H-6")
        //manifold *= 0.9875f; //assumed
        float m = 0;
        if (manifold <= 600)
            m = 160;
        else
        {
            m = (manifold - 600) * -0.26667f;
            m += 160;
        }

        Quaternion target = Quaternion.Euler(0, 0, m);

        return target;
    }

    internal static Quaternion WaterTempTargetA(float v, float scalar0, float scalar1)
    {
        v *= -0.735f;
        v -= -47.35f;
        v = Mathf.Clamp(v, -50f, 50f);

        return Quaternion.Euler(0, 0, v);
    }

    internal static Quaternion WaterTempTargetB(float v, float scalar0, float scalar1)
    {
        v *= -.74f;
        v -= -43.5f;
        v = Mathf.Clamp(v, -50f, 50f);

        return Quaternion.Euler(0, 0, v);
    }

    internal static Quaternion WaterTempTargetC(float v, float scalar0, float scalar1)
    {
        v = Mathf.Clamp(v, -30f, 160f);
        v *= -0.686f;
        v -= -134;


        return Quaternion.Euler(0, 0, v);
    }

    internal static Quaternion WaterTempTargetD(float v, float scalar0, float scalar1)
    {
        v *= -0.42f;
        v -= -33.33334f;
        v = Mathf.Clamp(v, -34, 34);

        return Quaternion.Euler(0, 0, v);
    }

    //unused now
    internal static Quaternion OilTempFW190(float inRad, float outRad, float scalar0, float scalar1)
    {
        float diff = (outRad - inRad);
        float modDiff = diff * 0.45f;
        float preDiv = outRad - modDiff;
        float v = preDiv / 120f;
        v = Mathf.Lerp(-48f, 51f, v);//skewed model, or maybe not correct formula, good enough for now
        return Quaternion.Euler(0, 0, -v);
    }

    internal static Quaternion OilTempTargetF(float v, float scalar0, float scalar1, AnimationCurve curve)
    {
        //and work out percentage to use 0-1 scale for curve
        float highest = 160;
        float percentage = (Mathf.Abs(v / highest));
        //multiply by half a dial of spin (180 degrees)
        float angleToSpin = curve.Evaluate(percentage);

        angleToSpin *= -287.5f;
        angleToSpin -= -145;

        Quaternion target = Quaternion.Euler(0, 0, angleToSpin);

        return target;
    }

    internal static Quaternion OilTempTargetC(float v, float scalar0, float scalar1)
    {
        v = Mathf.Clamp(v, -30f, 160f);
        v *= -0.686f;
        v -= -134;

        return Quaternion.Euler(0, 0, v);
    }
}
