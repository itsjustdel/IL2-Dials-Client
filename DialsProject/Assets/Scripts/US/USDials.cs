using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class USDials : MonoBehaviour
{
    public static Quaternion AirspeedTargetA(float airspeed)
    {
        if (airspeed == 0)
            return Quaternion.identity;

        if (float.IsNaN(airspeed) || float.IsNegativeInfinity(airspeed) || float.IsPositiveInfinity(airspeed))
            return Quaternion.identity;

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

    public static Quaternion AirspeedTargetB(float airspeed)
    {
        if (airspeed == 0)
            return Quaternion.identity;

        //airspeed dial has two gears
        Quaternion target = Quaternion.identity;

        //convert mph
        airspeed /= 1.609f;

        if (airspeed <= 50)
        {
            target = Quaternion.Euler(0, 0, -(airspeed * .6f));
        }
        else if (airspeed > 50 && airspeed <= 200)
        {
            float r = airspeed - 50f;
            r *= 1.2f;
            r += 30f;//where gear starts in degrees
            target = Quaternion.Euler(0, 0, -r);
        }
        else
        {
            float r = airspeed - 200f;
            r *= .4f;
            r += 210;//where gear starts degrees
            target = Quaternion.Euler(0, 0, -r);
        }

        return target;
    }

    public static Quaternion AltitudeTargetSmall2(float altitude)
    {
        if (float.IsNaN(altitude))
            return Quaternion.identity;

        //convert to feet
        altitude *= 3.2808f;
       // Debug.Log("feet = " + altitude);
        Quaternion altitudeSmallTarget = Quaternion.Euler(0, 0, -(altitude *.018f));

        return altitudeSmallTarget;

    }

    public static Quaternion AltitudeTargetSmall(float altitude)
    {
        //convert to feet
        altitude *= 3.2808f;
        Quaternion altitudeSmallTarget = Quaternion.Euler(0, 0, -(altitude * .036f));

        return altitudeSmallTarget;

    }

    public static Quaternion AltitudeTargetLarge(float altitude)
    {
        if (float.IsNaN(altitude))
            return Quaternion.identity;

        //convert to feet
        altitude *= 3.2808f;

        Quaternion altitudeLargeTarget = Quaternion.Euler(0, 0, -(altitude / 1000f) * 360);
        return altitudeLargeTarget;

    }

    public static Quaternion AltitudeTargetSmallest(float altitude)
    {
        //convert to feet
        altitude *= 3.2808f;
        Quaternion altitudeSmallTarget = Quaternion.Euler(0, 0, -(altitude * .0036f));

        return altitudeSmallTarget;

    }

    public static Quaternion MmhgTarget(float mmhg)
    {
        //mmhg to inches of mercury
        float input = mmhg / 25.4f;
        float seaLevelInHg = 760f / 25.4f;//29.921
        float z = ((input - seaLevelInHg) * 148f) + 3f; //zero degree at 29.921 (slight ofset in asset rotation 3 degrees on blender)

        // Debug.Log("inches of merc = " + input);


        Quaternion mmhgTarget = Quaternion.identity;

        //catch bad value
        if (!float.IsNaN(z) && !float.IsNegativeInfinity(z) && !float.IsPositiveInfinity(z))
        {
            
            mmhgTarget = Quaternion.Euler(0, 0, z);
        }

        return mmhgTarget;

        
    }

    public static Vector3 HeadingIndicatorPosition(float heading,float trackLength)
    {

        //check for Nan
        if (float.IsNaN(heading) || float.IsNegativeInfinity(heading) || float.IsPositiveInfinity(heading))
            return Vector3.zero;

        //range is 0 to pi*2
        float ratio = Mathf.PI * heading;
        //adjust for arbitry render camera position
        ratio *= 18.235f;

        Vector3 pos = Vector3.right * ratio;
        pos += Vector3.right * trackLength;

       

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
        return new Vector3(0, climb * pitchMultiplier, 0);
    }

    public static Quaternion TurnAndBankRollMark(float roll)
    {
        //chevron that spins with roll
        Quaternion t = Quaternion.Euler(0, 0, -(roll));

        return t;

    }

    //repeater compass

    public static Quaternion RepeaterCompassTarget(float heading)
    {
        //number passed is rotation in rads, pi = 180 degrees
        Quaternion target = Quaternion.Euler(0, 0, -heading * Mathf.Rad2Deg);

        return target;
    }

    public static Quaternion VerticalSpeedTarget(float verticalSpeed,AnimationCurve curve)
    {
       
        //using animation curve to define angle to spin ( component on prefab)
        //animation curve needs positive value to work so save if negative
        bool negative = (verticalSpeed>= 0) ? false : true;

        //metres to feet
        //verticalSpeed *= 0.3048f;
        //verticalSpeed *= 3.048f;///test

        //and work out percentage to use 0-1 scale for curve
        float percentage = (Mathf.Abs(verticalSpeed/30f)); 
        //multiply by half a dial of spin (180 degrees)
        float angleToSpin = curve.Evaluate(percentage) ;
       // Debug.Log(angleToSpin);
        angleToSpin *= 180;
        
        //put negative back?

        
        if (negative)
            angleToSpin *= -1;

        

        //offset by 90 degrees - vsi starts at 9 0'clock on the dial
        verticalSpeed = 90f -  angleToSpin;
        
        //set to quaternion
        Quaternion target = Quaternion.Euler(0, 0, verticalSpeed);

        return target;
    }

    public static Quaternion VerticalSpeedTargetSimple(float verticalSpeed)
    {
        //vsi
        //start at 9 o'clock
        verticalSpeed = 90f - verticalSpeed * 9f;
        //clamp to "10"
        verticalSpeed = Mathf.Clamp(verticalSpeed, -90, 270);

        Quaternion target = Quaternion.Euler(0, 0, verticalSpeed);

        return target;
    }

    public static Quaternion TurnCoordinatorNeedleTarget(float v, bool flip)
    {
        if (flip)
            v = -v;

        if (float.IsNaN(v) || float.IsPositiveInfinity(v) || float.IsNegativeInfinity(v))
            return Quaternion.identity;

        v = Mathf.Clamp(v, -30f, 30f);        
        Quaternion target = Quaternion.Euler(0, 0, -v);

        return target;
    }

    public static Quaternion TurnCoordinatorBallTarget(float ball, float multiplier)
    {
        //indicates whether the aircraft is in coordinated flight, showing the slip or skid of the turn. 
        float z = ball * multiplier;
        z = Mathf.Clamp(z, -15f, 15f);
        Quaternion target = Quaternion.Euler(0, 0, z);

        return target;
    }

    public static Quaternion ArtificialHorizonRotation(float roll, float rollMultiplier)
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

    public static Quaternion ArtificialHorizonChevronRotation(float roll, float rollMultiplier)
    {
        //rotate horizon        

        Quaternion t = Quaternion.Euler(0, 0, roll * rollMultiplier);

        //for x rotation we need to rotate around global x after z rot
        //t *= Quaternion.Euler(climb * climbMultiplier, 0, 0);

        return t;
    }


    public static Quaternion RPMATarget(float rpm, float scalar, float scalar2)
    {
        float r = rpm * -0.36f;
        Quaternion target = Quaternion.Euler(0, 0, r);

        return target;
    }

    //2nd needle on two needle rpm
    public static Quaternion RPMAInnerTarget(float rpm, float scalar, float scalar2)
    {
        float r = rpm * 0.09f;
        Quaternion target = Quaternion.Euler(0, 0, r);

        return target;
    }



    public static Quaternion RPMBTarget(float rpm, float scalar, float scalar2)
    {
        // -0.12275*
        //209.135
        float start = 155f;
        float r = rpm * -0.07f + (start);

        //clamp low is actually high, rotation are negative
        r = Mathf.Clamp(r, -180, 180);

        Quaternion target = Quaternion.Euler(0, 0, r);

        return target;
    }


    public static Quaternion RPMCTarget(float rpm, float scalar, float scalar2)
    {
        // -0.12275*
        //209.135
        float start = 160f;
        float r = rpm * -0.071f + (start);

        //clamp low is actually high, rotation are negative
        r = Mathf.Clamp(r, -180, 180);

        Quaternion target = Quaternion.Euler(0, 0, r);

        return target;
    }
    public static Quaternion RPMDSmallTarget(float rpm, float scalar, float scalar2)
    {
        float r = rpm * -0.036f;
        Quaternion target = Quaternion.Euler(0, 0, r);

        return target;
    }

    
    public static Quaternion ManifoldTargetA(float manifold, float scalar)
    {
        //US *28.95902519867009 inches of Hg
        float m = 0;
        if (manifold > 10)
            m = (manifold - 10) * -8;

        m += 160;

        Quaternion target = Quaternion.Euler(0, 0, m);

        return target;
    }

    public static Quaternion ManifoldTargetC(float manifold, float scalar)
    {       
        //US *28.95902519867009 inches of Hg
        //manifold *= 2.895902519867009f;
        
        float m = 0;
        if (manifold > 10)
            m = (manifold - 10) * -5.25f;        

        m += 105;

        Quaternion target = Quaternion.Euler(0, 0, m);

        return target;
    }

}




