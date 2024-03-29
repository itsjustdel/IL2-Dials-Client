using UnityEngine;

public class ITADials : MonoBehaviour
{
    public static Quaternion AirspeedTarget(float airspeed)
    {

        if (airspeed == 0 || float.IsNaN(airspeed))
            return Quaternion.Euler(0, 0, 90);

        //airspeed dial has three gears
        Quaternion target = Quaternion.identity;
        if (airspeed < 100)
        {
            target = Quaternion.Euler(0, 0, ((airspeed) * -0.36f) + 90);//
        }
        else if (airspeed >= 100 && airspeed < 300)
        {
            target = Quaternion.Euler(0, 0, ((airspeed - 100) * -0.72f) + 54);//
        }
        else
        {
            target = Quaternion.Euler(0, 0, -((airspeed - 300) * 0.4f) - 90);//
        }


        return target;
    }

    public static Quaternion AltitudeTargetSmall(float altitude)
    {

        Quaternion altitudeSmallTarget = Quaternion.Euler(0, 0, -(altitude / 10000f) * 360 + 180);

        return altitudeSmallTarget;

    }

    public static Quaternion AltitudeTargetLarge(float altitude)
    {
        Quaternion altitudeLargeTarget = Quaternion.Euler(0, 0, -((altitude / 1000f) * 360) + 180);
        return altitudeLargeTarget;

    }

    public static Quaternion MmhgTarget(float mmhg)
    {

        //convert to german format
        float input = mmhg * 1.333f;
        float z = (-(1013.25f - input) * 1f) + 13.25f;

        Quaternion mmhgTarget = Quaternion.identity;

        //catch bad value
        if (!float.IsNaN(z))
            mmhgTarget = Quaternion.Euler(0, 0, z); // 0 is 1013.25 mbar //0 degrees // bit more confusing because of asset rotation

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
        ratio *= 11.945f;
        Vector3 pos = Vector3.right * ratio;
        pos += Vector3.right * trackLength;

        return pos;
    }

    public static Quaternion ArtificialHorizonRotation(float roll, float rollMultiplier)
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

    public static Quaternion TurnCoordinatorNeedleTarget(float v)
    {

        // ~.63 is max value in game data (on russian needle) so pi*2 perhaps - still confused with what's going on here
        // we get the the value from 0 to 1. 0.5 is centre, range of degrees is 60 (?)
        float z = (v - .5f) * Mathf.PI * 60f;

        z = Mathf.Clamp(z, -30, 30);
        Quaternion target = Quaternion.Euler(0, 0, -z);

        return target;
    }

    public static Quaternion TurnCoordinatorBallTarget(float ball, float multiplier)
    {
        //indicates whether the aircraft is in coordinated flight, showing the slip or skid of the turn. 
        float z = ball * multiplier;
        z = Mathf.Clamp(z, -3.75f, 3.75f); //high parent!
        Quaternion target = Quaternion.Euler(0, 0, z);

        return target;
    }

    public static Quaternion VerticalSpeedTarget(float verticalSpeed)
    {
        //vsi
        //start at 9 o'clock
        verticalSpeed = 90f - verticalSpeed * 7.2f;
        //clamp to "10"
        verticalSpeed = Mathf.Clamp(verticalSpeed, -90, 270);

        Quaternion target = Quaternion.Euler(0, 0, verticalSpeed);

        return target;
    }

    public static Quaternion RPMATarget(float rpm, float scalar, float scalar2)
    {
        // -0.12275*
        //209.135
        float start = -30;
        float r = rpm * -0.1f + (start);

        //clamp low is actually high, rotation are negative
        //r = Mathf.Clamp(r, -180, 160);

        Quaternion target = Quaternion.Euler(0, 0, r);

        return target;
    }

    public static Quaternion ManifoldTargetA(float manifold, float scalar, float s2)
    {
        //in gamerotations differ from pure data from game
        //manifold *= 1.0225f;
        float m = 0;
        if (manifold <= 500)
            m = 150;
        else
        {
            m = (manifold - 500) * -0.2f;
            m += 150;
        }

        Quaternion target = Quaternion.Euler(0, 0, m);

        return target;
    }

    internal static Quaternion WaterTempTargetA(float v, float scalar, float scalar1, AnimationCurve curve)
    {
        //-315 full needle spin to 3000
        //and work out percentage to use 0-1 scale for curve
        float highest = 130;
        float percentage = (Mathf.Abs(v / highest));

        //Debug.Log(percentage);

        //multiply by half a dial of spin (180 degrees)
        float angleToSpin = curve.Evaluate(percentage);

        angleToSpin *= -294;
        angleToSpin -= -149;

        Quaternion target = Quaternion.Euler(0, 0, angleToSpin);

        return target;
    }
}
