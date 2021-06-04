using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ITADials : MonoBehaviour
{
    public static Quaternion AirspeedTarget(float airspeed)
    {
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
}
