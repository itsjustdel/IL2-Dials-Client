using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class USDials : MonoBehaviour
{
    public static Quaternion AirspeedTarget(float airspeed)
    {
        //airspeed dial has two gears
        Quaternion target = Quaternion.identity;

        //convert mph
        airspeed /= 1.609f;

        if (airspeed <= 50)
		{
			target = Quaternion.Euler(0, 0, -((airspeed) * Easing.Exponential.In(.825f))); 
		}
		else if (airspeed >= 50 && airspeed < 100)
        {
			target = Quaternion.Euler(0, 0, -((airspeed -50) * Easing.Exponential.In(.985f)) -15); //Jesus Christi
        }
        else if(airspeed >=100 && airspeed < 300)
        {
            //100 = 60*
            //150 = 100*            
            //200 = 140*
            //250 = 180
            target = Quaternion.Euler(0, 0, -((airspeed - 100) * .8f  ) -60);//60* is at 100
        }
        else
        {
            target = Quaternion.Euler(0, 0, -((airspeed - 300) * .3f) + 140);//140* is at 300
        }

        return target;
    }

    public static Quaternion AltitudeTargetSmall(float altitude)
    {
        //convert to feet
        altitude *= 3.281f;
        Quaternion altitudeSmallTarget = Quaternion.Euler(0, 0, -(altitude *.018f));

        return altitudeSmallTarget;

    }

    public static Quaternion AltitudeTargetLarge(float altitude)
    {
        //convert to feet
        altitude *= 3.281f;

        Quaternion altitudeLargeTarget = Quaternion.Euler(0, 0, -(altitude / 1000f) * 360);
        return altitudeLargeTarget;

    }

    public static Quaternion MmhgTarget(float mmhg)
    {
        //mmhg to inches of mercury
        float input = mmhg / 25.4f;
        float seaLevelInHg = 760f / 25.4f;//29.921
        Quaternion mmhgTarget = Quaternion.Euler(0, 0, ((input - seaLevelInHg) * 148f) + 3f); //zero degree at 29.921 (slight ofset in asset rotation 3 degrees on blender)

       // Debug.Log("inches of merc = " + input);

        return mmhgTarget;
    }
}




