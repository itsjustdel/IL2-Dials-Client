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
        Quaternion altitudeSmallTarget = Quaternion.Euler(0, 0, -(altitude * .0018f));

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
}
