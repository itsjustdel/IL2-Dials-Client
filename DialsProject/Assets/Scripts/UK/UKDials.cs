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

        float z = ((1013.25f - input) * 1f) - 13.25f;


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
        ratio *= 20.32f;

        Vector3 pos = Vector3.right * ratio;
        //add track length
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

    public static Quaternion ArtificialHorizonChevronRotation(float roll, float rollMultiplier)
    {
        //rotate horizon        

        Quaternion t = Quaternion.Euler(0, 0, roll * rollMultiplier);

        //for x rotation we need to rotate around global x after z rot
        //t *= Quaternion.Euler(climb * climbMultiplier, 0, 0);

        return t;
    }

    public static Quaternion TurnCoordinatorNeedleTarget(float v, float mod)//bottom needle
    {

        v *= -.62f; //clamped at 31 in game sp double that?
        Quaternion target = Quaternion.identity;
        if (Mathf.Abs( v ) < 10f)
             target = Quaternion.Euler(0, 0, v * 2f + 180);
        //geared
        else if (Mathf.Abs( v )>= 10f )
            //take in to account if v is positive or negative
            if(v > 0)
                target = Quaternion.Euler(0, 0,  10 + ((v)) + 180);
            else
                 target = Quaternion.Euler(0, 0, -10 + ((v)) + 180);
        return target;
    }

    public static Quaternion TurnCoordinatorBallTarget(float v, float multiplier)//top needle
    {
        v *= -1000;
        Debug.Log(v);
        float gearChange = 10f;
        Quaternion target = Quaternion.identity;
        if (Mathf.Abs(v) < gearChange)
            target = Quaternion.Euler(0, 0, v);
        //geared
        else if (Mathf.Abs(v) >= gearChange)
            //take in to account if v is positive or negative
            if (v > 0)
                target = Quaternion.Euler(0, 0, (( gearChange + v)));
            else
                target = Quaternion.Euler(0, 0, -(gearChange - v));


        
        return target;

    }


    public static Quaternion VerticalSpeedTarget(float verticalSpeed)
    {
        //vsi
        //start at 9 o'clock
        verticalSpeed = 90f - verticalSpeed* 7.2f;


        //clamp to 90 degrees /TODO dont clamp hee, clamp after thre rotations applied to its hitsd the pin opn the dial rather coming to a smooth stop
        verticalSpeed = Mathf.Clamp(verticalSpeed, -90, 270);

        Quaternion target = Quaternion.Euler(0, 0, verticalSpeed);

        return target;
    }

    //repeater compass

    public static Quaternion RepeaterCompassTarget(float heading)
    {
        //number passed is rotation in rads, pi = 180 degrees
        Quaternion target = Quaternion.Euler(0, 0, -heading * Mathf.Rad2Deg);

        return target;
    }


    public static Quaternion RPMATarget(float rpm, float scalar, float scalar2)
    {
        float r = rpm * -scalar + (scalar2);

        //clamp low is actually high, rotation are negative
        //r = Mathf.Clamp(r, -180, 164);

        Quaternion target = Quaternion.Euler(0, 0, r);

        return target;
    }

    public static Quaternion RPMBTarget(float rpm, float scalar, float scalar2)
    {
        float r = rpm * -scalar + (scalar2);

        //clamp low is actually high, rotation are negative
        //r = Mathf.Clamp(r, -180, 164);

        Quaternion target = Quaternion.Euler(0, 0, r);

        return target;
    }

}
