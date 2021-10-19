using System.Collections;
using System.Collections.Generic;
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

        Vector3 pos = Vector3.right*ratio;
        //track length
        pos += Vector3.right * trackLength;
        

        return pos;
    }

    public static Quaternion TurnAndBankPlaneRotation(float roll,float climb, float rollMultiplier, float xRotation)
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


    public static Quaternion TurnCoordinatorNeedleTarget(float v, float mod)
    {
        Quaternion target = Quaternion.Euler(0, 0, v * mod);

        return target;
    }


    public static Quaternion TurnCoordinatorBallTarget(float ball)
    {
        //indicates whether the aircraft is in coordinated flight, showing the slip or skid of the turn. 
        float z = ball * 420;
        z = Mathf.Clamp(z,-15f, 15f);
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

        Quaternion target = Quaternion.Euler(0, 0,  verticalSpeed);

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
}
