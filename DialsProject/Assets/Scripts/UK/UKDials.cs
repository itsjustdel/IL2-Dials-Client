using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UKDials : MonoBehaviour
{
    public static Quaternion AirspeedTarget(float airspeed)
    {
        if (airspeed == 0)
            return Quaternion.identity;

        //airspeed dial has two gears
        Quaternion target = Quaternion.identity;

        //convert mph
        airspeed /= 1.609f;

        if (airspeed < 60)
        {
            target = Quaternion.Euler(0, 0, -(airspeed) * .333f + 180);//60 at 220
        }
        else
        {
            target = Quaternion.Euler(0, 0, (60f - airspeed) * 1.4f + 160);//60 at 220
        }

        return target;
    }

    public static Quaternion AltitudeTargetSmall(float altitude)
    {
        //convert to feet
        altitude *= 3.2808f;
        Quaternion altitudeSmallTarget = Quaternion.Euler(0, 0, -(altitude * .036f));

        return altitudeSmallTarget;

    }

    public static Quaternion AltitudeTargetSmallest(float altitude)
    {
        //convert to feet
        altitude *= 3.2808f;
        Quaternion altitudeSmallTarget = Quaternion.Euler(0, 0, -(altitude * .0036f));

        return altitudeSmallTarget;

    }

    public static Quaternion AltitudeTargetLarge(float altitude)
    {
        //convert to feet
        altitude *= 3.2808f;

        Quaternion altitudeLargeTarget = Quaternion.Euler(0, 0, -(altitude / 1000f) * 360);
        return altitudeLargeTarget;

    }

    public static Quaternion MmhgTarget(float mmhg)
    {
        float input = mmhg * 1.333f;
        Quaternion mmhgTarget = Quaternion.Euler(0, 0, ((1013.25f - input) * 1f) - 13.25f);




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
        ratio *= 20.315f;

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

    public static Quaternion ArtificialHorizonChevronRotation(float roll, float rollMultiplier)
    {
        //rotate horizon        

        Quaternion t = Quaternion.Euler(0, 0, roll * rollMultiplier);

        //for x rotation we need to rotate around global x after z rot
        //t *= Quaternion.Euler(climb * climbMultiplier, 0, 0);

        return t;
    }

    public static Quaternion TurnCoordinatorNeedleTarget(float v, float mod)
    {
      

        Quaternion target = Quaternion.identity;
        if (Mathf.Abs( v ) < 10f)
             target = Quaternion.Euler(0, 0, v * 2f);
        //geared
        else if (Mathf.Abs( v )>= 10f )
            //take in to account if v is positive or negative
            if(v > 0)
                target = Quaternion.Euler(0, 0,  10 + ((v)));
            else
                 target = Quaternion.Euler(0, 0, -10 + ((v)));
        return target;
    }

    public static Quaternion TurnCoordinatorBallTarget(float ball, float multiplier)
    {
        //indicates whether the aircraft is in coordinated flight, showing the slip or skid of the turn. 
        Quaternion target = Quaternion.Euler(0, 0, ball * multiplier + 180f);

        return target;
    }


    public static Quaternion VerticalSpeedTarget(float verticalSpeed)
    {
        //vsi
        //start at 9 o'clock
        verticalSpeed = 90f - verticalSpeed * 36f;
        //clamp to "10"
        verticalSpeed = Mathf.Clamp(verticalSpeed, -90, 270);

        Quaternion target = Quaternion.Euler(0, 0, verticalSpeed);

        return target;
    }


}
