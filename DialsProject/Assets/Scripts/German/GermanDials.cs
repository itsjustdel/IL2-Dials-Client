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

    public static Quaternion VerticalSpeedTarget15(float verticalSpeed)
    {
        //vsi
        //start at 9 o'clock
        verticalSpeed = 90f - verticalSpeed * 9f;
        //clamp to "10"
        verticalSpeed = Mathf.Clamp(verticalSpeed, -90, 270);

        Quaternion target = Quaternion.Euler(0, 0, verticalSpeed);

        return target;
    }

    public static Quaternion TurnCoordinatorNeedleTarget(float v)
    {
        Quaternion target = Quaternion.Euler(0, 0, v);

        return target;
    }

    public static Quaternion TurnCoordinatorBallTarget(float ball)
    {
        //indicates whether the aircraft is in coordinated flight, showing the slip or skid of the turn. 
        Quaternion target = Quaternion.Euler(0, 0, ball );

        return target;
    }

    public static Quaternion TurnAndBankBallTarget(float ball)
    {
        //indicates whether the aircraft is in coordinated flight, showing the slip or skid of the turn. 
        Quaternion target = Quaternion.Euler(0, 0, ball );

        return target;
    }


    public static Quaternion RepeaterCompassTarger(float heading)
    {
        Quaternion target = Quaternion.Euler(0, 0, Mathf.PI * heading);

        return target;
    }

    public static Quaternion VerticalSpeedTarget30(float verticalSpeed)
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
}
