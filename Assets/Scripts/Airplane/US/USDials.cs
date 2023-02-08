using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class USDials : MonoBehaviour
{

    internal static Quaternion AirspeedTarget700Scale(float airspeed, AnimationCurve curve, float scalar)
    {

        if (float.IsNaN(airspeed) || float.IsNegativeInfinity(airspeed) || float.IsPositiveInfinity(airspeed))
            return Quaternion.identity;

        //convert mph
        airspeed /= 1.609f;

        if (airspeed < 50)
        {
            //set to quaternion
            return Quaternion.Euler(0, 0, airspeed*-0.3f);
        }
        else
        {
            //-15 .. 23.5
            //full rotation is 321.5 degrees ( 50 to 700)

            //and work out percentage to use 0-1 scale for curve
            float percentage = (((airspeed - 50) / 650f));
            //multiply by half a dial of spin (180 degrees)
            float angleToSpin = curve.Evaluate(percentage);
            angleToSpin *= 321.5f;
            angleToSpin += 15;

            //set to quaternion
            return Quaternion.Euler(0, 0, -angleToSpin);
        }
    }

    internal static Quaternion AirspeedTargetA20(float airspeed)
    {
        if (float.IsNaN(airspeed) || float.IsNegativeInfinity(airspeed) || float.IsPositiveInfinity(airspeed))
            return Quaternion.identity;

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

    internal static Quaternion AirspeedTargetP40(float airspeed, float scalar, float scalar1)
    {
        if (float.IsNaN(airspeed) || float.IsNegativeInfinity(airspeed) || float.IsPositiveInfinity(airspeed))
            return Quaternion.identity;

        if (airspeed == 0)
            return Quaternion.identity;

        //airspeed dial has two gears
        Quaternion target = Quaternion.identity;

        //convert mph
        airspeed /= 1.609f;

        if (airspeed <= 50)
        {
            target = Quaternion.Euler(0, 0, -(airspeed * .375f));
        }
        else if (airspeed <= 150)
        {
            target = Quaternion.Euler(0, 0, -((airspeed - 50) * Easing.Exponential.In(1.085f))); //Not quite accurate < 100
        }
        else
        {
            float r = airspeed - 150f;
            r *= .46f;
            r += 180;//where gear starts degrees
            target = Quaternion.Euler(0, 0, -r);
        }

        return target;
    }

    internal static Quaternion AltitudeTargetSmall2(float altitude)
    {
        if (float.IsNaN(altitude))
            return Quaternion.identity;

        //convert to feet
        altitude *= 3.2808f;
       // Debug.Log("feet = " + altitude);
        Quaternion altitudeSmallTarget = Quaternion.Euler(0, 0, -(altitude *.018f));

        return altitudeSmallTarget;

    }

    internal static Quaternion AltitudeTargetSmall(float altitude)
    {
        //convert to feet
        altitude *= 3.2808f;
        Quaternion altitudeSmallTarget = Quaternion.Euler(0, 0, -(altitude * .036f));

        return altitudeSmallTarget;

    }

    internal static Quaternion AltitudeTargetLarge(float altitude)
    {
        if (float.IsNaN(altitude))
            return Quaternion.identity;

        //convert to feet
        altitude *= 3.2808f;

        Quaternion altitudeLargeTarget = Quaternion.Euler(0, 0, -(altitude / 1000f) * 360);
        return altitudeLargeTarget;

    }

    internal static Quaternion AltitudeTargetSmallest(float altitude)
    {
        //convert to feet
        altitude *= 3.2808f;
        Quaternion altitudeSmallTarget = Quaternion.Euler(0, 0, -(altitude * .0036f));

        return altitudeSmallTarget;

    }

    internal static Quaternion MmhgTarget(float mmhg)
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

    internal static Vector3 HeadingIndicatorPosition(float heading,float trackLength)
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

    internal static Quaternion HeadingIndicatorBallTarget(float ball, float scalar)
    {
        //indicates whether the aircraft is in coordinated flight, showing the slip or skid of the turn. 
        float z = ball * 150f;
        z = Mathf.Clamp(z, -3.6f, 3.6f);
        Quaternion target = Quaternion.Euler(0, 0, z);

        return target;
    }


    //turn and bank
    internal static Quaternion TurnAndBankPlaneRotation(float roll, float climb, float rollMultiplier, float climbMultiplier)
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

    internal static Vector3 TurnAndBankPlanePosition(float climb, float pitchMultiplier)
    {
        //move plane up and down
        return new Vector3(0, climb * pitchMultiplier, 0);
    }

    internal static Quaternion TurnAndBankRollMark(float roll)
    {
        //chevron that spins with roll
        Quaternion t = Quaternion.Euler(0, 0, -(roll));

        return t;

    }

    //repeater compass

    internal static Quaternion RepeaterCompassTarget(float heading)
    {
        //number passed is rotation in rads, pi = 180 degrees
        Quaternion target = Quaternion.Euler(0, 0, -heading * Mathf.Rad2Deg);

        return target;
    }

    internal static Quaternion VerticalSpeedTarget(float verticalSpeed,AnimationCurve curve)
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

    internal static Quaternion TurnCoordinatorNeedleTarget(float v, bool flip)
    {
        if (flip)
            v = -v;

        if (float.IsNaN(v) || float.IsPositiveInfinity(v) || float.IsNegativeInfinity(v))
            return Quaternion.identity;

        v = Mathf.Clamp(v, -30f, 30f);        
        Quaternion target = Quaternion.Euler(0, 0, -v);

        return target;
    }

    internal static Quaternion TurnCoordinatorBallTarget(float ball, float multiplier)
    {
        //indicates whether the aircraft is in coordinated flight, showing the slip or skid of the turn. 
        float z = ball * multiplier;
        z = Mathf.Clamp(z, -15f, 15f);
        Quaternion target = Quaternion.Euler(0, 0, z);

        return target;
    }

    internal static Quaternion ArtificialHorizonRotation(float roll, float rollMultiplier)
    {
        //rotate horizon        

        Quaternion t = Quaternion.Euler(0, 0, roll * rollMultiplier);

        //for x rotation we need to rotate around global x after z rot
        //t *= Quaternion.Euler(climb * climbMultiplier, 0, 0);

        return t;
    }


    internal static Vector3 ArtificialHorizonPosition(float climb, float pitchMultiplier)
    {
        //move plane up and down
        return new Vector3(0, climb * pitchMultiplier, 0);
    }

    internal static Quaternion ArtificialHorizonChevronRotation(float roll, float rollMultiplier)
    {
        //rotate horizon        

        Quaternion t = Quaternion.Euler(0, 0, roll * rollMultiplier);

        //for x rotation we need to rotate around global x after z rot
        //t *= Quaternion.Euler(climb * climbMultiplier, 0, 0);

        return t;
    }


    internal static Quaternion RPMATarget(float rpm, float scalar, float scalar2)
    {
        float r = rpm * -0.36f;
        Quaternion target = Quaternion.Euler(0, 0, r);

        return target;
    }

    //2nd needle on two needle rpm
    internal static Quaternion RPMAInnerTarget(float rpm, float scalar, float scalar2)
    {
        float r = rpm * 0.09f;
        Quaternion target = Quaternion.Euler(0, 0, r);

        return target;
    }



    internal static Quaternion RPMBTarget(float rpm, float scalar, float scalar2)
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


    internal static Quaternion RPMCTarget(float rpm, float scalar, float scalar2)
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
    internal static Quaternion RPMDSmallTarget(float rpm, float scalar, float scalar2)
    {
        float r = rpm * -0.036f;
        Quaternion target = Quaternion.Euler(0, 0, r);

        return target;
    }

    
    internal static Quaternion ManifoldTargetA(float manifold, float scalar)
    {
        //US *28.95902519867009 inches of Hg
        float m = 0;
        if (manifold > 10)
            m = (manifold - 10) * -8;

        m += 160;

        Quaternion target = Quaternion.Euler(0, 0, m);

        return target;
    }

    internal static Quaternion ManifoldTargetC(float manifold, float scalar)
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



    internal static Quaternion ManifoldTargetE(float manifold, int engineMod, float scalar)
    {
        //P 51 
        float m = 0;
        if (engineMod == 0)
        {         
            if (manifold > 10)
                m = (manifold - 10) * -5.333333333f;

            m += 100;
        }
        else
        {
            //150 octane 100 scale
            if (manifold > 15)
                m = (manifold - 15) * -4.083333333f;

            m += 56.7129f;
        }

        Quaternion target = Quaternion.Euler(0, 0, m);

        return target;
    }

    internal static Quaternion WaterTempTargetA(float v, float scalar0, float scalar1)
    {        
        v *= -.91f;
        v -= -91.2f;
        v = Mathf.Clamp(v, -50f, 50f);

        return Quaternion.Euler(0, 0, v);        
    }

    internal static Quaternion WaterTempTargetB(float v, float scalar0, float scalar1, bool v2)
    {
        v = Mathf.Clamp(v, -80f, 160f);
        if (!v2)
        {            
            v *= -0.56f;
            v -= 247.5f;
        }
        else
        {
            v *= 0.56f;
            v += 247.5f;
        }
        return Quaternion.Euler(0, 0, v);
    }

    internal static Quaternion WaterTempTargetC(float v, float scalar0, float scalar1)
    {
        v = Mathf.Clamp(v, -80f, 160f);
        v *= -0.5f;
        v -= -19.5f;
        
        return Quaternion.Euler(0, 0, v);
    }

    internal static Quaternion OilTempCombo(float v, float scalar0, float scalar1)
    {
        v = Mathf.Clamp(v, 0, 100);
        float r = v * -1.8f;
        Quaternion target = Quaternion.Euler(0, 0, r + 90);

        return target;
    }

    internal static Quaternion OilTempTargetB(float v, float scalar0, float scalar1, bool v2)
    {
        v = Mathf.Clamp(v, 20f, 120f);        
        if (!v2)
        {
            v *= .69f;
            v -= 102.5f;
        }
        else
        {
            v *= -.69f;
            v += 102.5f;
        }
        
        return Quaternion.Euler(0, 0, v);
    }

    internal static Quaternion CylinderHeadTempTargetA(float v, float scalar0, float scalar1)
    {
        v = Mathf.Clamp(v, 0, 350);
        float r = v * -0.2828f;
        Quaternion target = Quaternion.Euler(0, 0, r + 49.5f);

        return target;
    }

    internal static Quaternion CylinderHeadTempTargetB(float v, float scalar0, float scalar1, bool v2)
    {
        v = Mathf.Clamp(v, 0f, 350f);
        if (!v2)
        {
            v *= 0.197f;
            v -= 89;
        }
        else
        {
            v *= -0.197f;
            v += 89;
        }

        return Quaternion.Euler(0, 0, v);
    }

    internal static Quaternion CylinderHeadTempTargetC(float v, float scalar0, float scalar1, bool v2)
    {
        v = Mathf.Clamp(v, 0f, 350f);
        if (!v2)
        {
            v *= -0.306f;
            v -= -143.4f;
        }
        else
        {
            v *= 0.306f;
            v += -143.4f;
        }
        return Quaternion.Euler(0, 0, v);
    }
}
