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
        Quaternion mmhgTarget = Quaternion.Euler(0, 0, ((760f - mmhg) / 100f) * 300);

        return mmhgTarget;
    }

    public static Vector3 HeadingIndicatorPosition(float heading)
    {
        


       // heading -= Mathf.PI;

        //check for Nan
        if (float.IsNaN(heading) || heading == 0f)
            return Vector3.zero;

        //range is 0 to pi*2
        float ratio = Mathf.PI * heading;
        //adjust for arbitry render camera position
        ratio *= 1.5855f;

        Vector3 pos = Vector3.right*ratio;
        //arbitry multiplier due to blender camera settings for render
        //pos *= 0.916f;

        //pos *= .66666f;

        return pos;
    }

    public static Quaternion TurnAndBankPlaneRotation(float roll,float climb, float rollMultiplier, float climbMultiplier)
    {        
        //rotate plane
        //clamp roll , in game cockpit stop rotation at just under 90 degrees - this happens when roll rate is ~1.7
        float tempRoll = -roll;
        Mathf.Clamp(tempRoll, -1.7f, 1.7f);
        Quaternion t = Quaternion.Euler(0, 0, tempRoll * rollMultiplier);
        Quaternion r = t;

        //for x rotatin we need to rotate around global x after z rot
        r *= Quaternion.Euler(climb * climbMultiplier, 0, 0);

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

    public static Quaternion TurnCoordinatorNeedleTarget(float  currentHeading, float previousHeading, float lastMessageReceivedTime, float previousMessageTime)
    {
        //stop heasding crossing over 0
        currentHeading += 100;
        previousHeading += 100;
        
        
        //indicates the rate of turn, or the rate of change in the aircraft's heading;



        //time diff
        float delta = lastMessageReceivedTime - previousMessageTime; //?

        //heading diff between last two frames
        float diff = currentHeading - previousHeading;

        diff /= delta;

        diff *= -200;

        
        Debug.Log("curr heading = " + currentHeading);
        Debug.Log("prev heading = " + previousHeading);

        Debug.Log("dif =" + diff);
        Debug.Log("delta =" + delta);
        
        
        Quaternion target = Quaternion.Euler(0,0,diff);

        return target;
    }

    public static Quaternion TurnCoordinatorBallTarget(float ball)//?
    {
        //indicates whether the aircraft is in coordinated flight, showing the slip or skid of the turn. 

        Quaternion target = Quaternion.Euler(0, 0, ball* 5);

        return target;
    }
}
