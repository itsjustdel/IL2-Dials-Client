using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GermanDials : MonoBehaviour
{
    public static Quaternion AirspeedTarget(float airspeed)
    {
        if (airspeed == 0)
            return Quaternion.Euler(0, 0, 180);

        //airspeed dial has two gears
        Quaternion target = Quaternion.identity;
        if(airspeed<100)
        {
            target = Quaternion.Euler(0, 0, -((airspeed) * 0.15f) - 180 );//smaller step size
        }
        else
        {
            target = Quaternion.Euler(0, 0, -((airspeed) * 0.5f) - 195 + 50);//195 is wher "100" is and 50 is the step
        }

        return target;
    }

    public static Quaternion AltitudeTargetSmall(float altitude)
    {

        Quaternion altitudeSmallTarget = Quaternion.Euler(0, 0, -(altitude / 10000f) * 360);

        return altitudeSmallTarget;

    }

    public static Quaternion AltitudeTargetLarge(float altitude)
    {
        Quaternion altitudeLargeTarget = Quaternion.Euler(0, 0, -((altitude / 1000f) * 360) + 180);
        return altitudeLargeTarget;

    }

    public static Quaternion MmhgTarget(float mmhg)
    {
        

        //mmhg to mbar
        float input = mmhg * 1.333f;        
        Quaternion mmhgTarget = Quaternion.Euler(0, 0, ((1013.25f -input) *1f) - 3.25f); // 0 is 1013.25 mbar //0 degrees // bit more confusing because of asset rotation

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
        ratio *= 11.93f;

        Vector3 pos = Vector3.right * ratio;


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

    public static Quaternion TurnCoordinatorNeedleTarget(float v)
    {
        Quaternion target = Quaternion.Euler(0, 0, v);

        return target;
    }

    public static Quaternion TurnCoordinatorBallTarget(float ball,float mod)
    {
        //indicates whether the aircraft is in coordinated flight, showing the slip or skid of the turn. 
        Quaternion target = Quaternion.Euler(0, 0, ball * mod);

        return target;
    }

    public static Quaternion TurnAndBankBallTarget(float ball,float multiplier)
    {
        //indicates whether the aircraft is in coordinated flight, showing the slip or skid of the turn. 
        Quaternion target = Quaternion.Euler(0, 0, ball * multiplier);

        return target;
    }


    public static Quaternion RepeaterCompassTarget(float heading)
    {
        //number passed is rotation in rads, pi = 180 degrees
        Quaternion target = Quaternion.Euler(0, 0, heading * Mathf.Rad2Deg);

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

        if (verticalSpeed <= 5)
        {
            //start at 9 o'clock
            verticalSpeed = 90f - verticalSpeed * 9f;
        }
        else if (verticalSpeed > 5 && verticalSpeed < 10f)
        {
            //create rotation by creating new start point ( verticalSpeed - 5 degrees )
            verticalSpeed = 90f - ((verticalSpeed-5f) * 6f) - 45f;
        }
        else
        {
            // > 10
            verticalSpeed = 90f - (verticalSpeed * 3f) -45f;

        }
        //clamp to "10"
        verticalSpeed = Mathf.Clamp(verticalSpeed, -90, 270);

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


}
