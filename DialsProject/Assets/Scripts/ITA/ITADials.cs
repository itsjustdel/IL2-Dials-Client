using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ITADials : MonoBehaviour
{
    public static Quaternion AirspeedTarget(float airspeed)
    {

        if (airspeed == 0)
            return Quaternion.Euler(0,0,90);

        //airspeed dial has three gears
        Quaternion target = Quaternion.identity;
        if (airspeed < 100)
        {
            target = Quaternion.Euler(0, 0, ((airspeed) * -0.36f) + 90);//
        }
        else if (airspeed >= 100 && airspeed < 300)
        {
            target = Quaternion.Euler(0, 0, ((airspeed -100) * -0.72f) + 54);//
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
        Quaternion mmhgTarget = Quaternion.Euler(0, 0, (-(1013.25f - input) * 1f) + 13.25f); // 0 is 1013.25 mbar //0 degrees // bit more confusing because of asset rotation
        

        return mmhgTarget;
    }


    public static Vector3 HeadingIndicatorPosition(float heading)
    {
        //check for Nan
        if (float.IsNaN(heading) || heading == 0f)
            return Vector3.zero;

        //range is 0 to pi*2
        float ratio = Mathf.PI * heading;
        //adjust for arbitry render camera position
        ratio *= 11.945f;
        Vector3 pos = Vector3.right * ratio;


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
        Quaternion target = Quaternion.Euler(0, 0, v);

        return target;
    }

    public static Quaternion TurnCoordinatorBallTarget(float ball, float multiplier)
    {
        //indicates whether the aircraft is in coordinated flight, showing the slip or skid of the turn. 
        Quaternion target = Quaternion.Euler(0, 0, ball * multiplier);

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
}
