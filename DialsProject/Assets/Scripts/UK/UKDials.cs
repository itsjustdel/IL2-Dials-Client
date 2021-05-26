using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UKDials : MonoBehaviour
{
    public static Quaternion AirspeedTarget(float airspeed)
    {
        //airspeed dial has two gears
        Quaternion target = Quaternion.identity;
        if (airspeed <= 50)
        {
            target = Quaternion.Euler(0, 0, -((airspeed) * Easing.Exponential.In(.825f)));
        }
        else if (airspeed >= 50 && airspeed < 100)
        {
            target = Quaternion.Euler(0, 0, -((airspeed - 50) * Easing.Exponential.In(.985f)) - 15); //Jesus Christi
        }
        else if (airspeed >= 100 && airspeed < 300)
        {
            //100 = 60*
            //150 = 100*            
            //200 = 140*
            //250 = 180
            target = Quaternion.Euler(0, 0, -((airspeed - 100) * .8f) - 60);//60* is at 100
        }
        else
        {
            target = Quaternion.Euler(0, 0, -((airspeed - 300) * .3f) + 140);//140* is at 300
        }

        return target;
    }

    public static Quaternion AltitudeTargetSmall(float altitude)
    {

        Quaternion altitudeSmallTarget = Quaternion.Euler(0, 0, -(altitude * .036f));

        return altitudeSmallTarget;

    }

    public static Quaternion AltitudeTargetSmallest(float altitude)
    {

        Quaternion altitudeSmallTarget = Quaternion.Euler(0, 0, -(altitude * .0036f));

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
}
