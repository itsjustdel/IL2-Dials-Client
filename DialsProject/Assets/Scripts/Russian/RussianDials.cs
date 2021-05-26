using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RussianDials : MonoBehaviour
{
   public static Quaternion AirspeedTarget(float airspeed)
    {
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
}