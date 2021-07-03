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
}
