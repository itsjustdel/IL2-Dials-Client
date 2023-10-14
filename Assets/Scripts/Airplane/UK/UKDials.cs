using UnityEngine;

public class UKDials : MonoBehaviour
{
    internal static Quaternion AirspeedTarget(float airspeed)
    {
        if (airspeed == 0)
            return Quaternion.Euler(0, 0, -180);//60 at 220;

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

    internal static Quaternion AltitudeTargetSmall(float altitude)
    {
        //convert to feet
        altitude *= 3.2808f;
        Quaternion altitudeSmallTarget = Quaternion.Euler(0, 0, -(altitude * .036f));

        return altitudeSmallTarget;

    }

    internal static Quaternion AltitudeTargetSmallest(float altitude)
    {
        //convert to feet
        altitude *= 3.2808f;
        Quaternion altitudeSmallTarget = Quaternion.Euler(0, 0, -(altitude * .0036f));

        return altitudeSmallTarget;

    }

    internal static Quaternion AltitudeTargetLarge(float altitude)
    {
        //convert to feet
        altitude *= 3.2808f;

        Quaternion altitudeLargeTarget = Quaternion.Euler(0, 0, -(altitude / 1000f) * 360);
        return altitudeLargeTarget;

    }

    internal static Quaternion MmhgTarget(float mmhg)
    {
        float input = mmhg * 1.333f;

        float z = ((1013.25f - input) * 1f) - 13.25f;


        Quaternion mmhgTarget = Quaternion.identity;

        //catch bad value
        if (!float.IsNaN(z))
            mmhgTarget = Quaternion.Euler(0, 0, z);


        return mmhgTarget;
    }

    internal static Vector3 HeadingIndicatorPosition(float heading, float trackLength)
    {
        //check for Nan
        if (float.IsNaN(heading))
            return Vector3.zero;

        //range is 0 to pi*2
        float ratio = Mathf.PI * heading;
        //adjust for arbitry render camera position
        ratio *= 20.32f;

        Vector3 pos = Vector3.right * ratio;
        //add track length
        pos += Vector3.right * trackLength;


        return pos;
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

    internal static Quaternion TurnCoordinatorNeedleTarget(float v, float mod)//bottom needle
    {

        v *= -.62f; //clamped at 31 in game sp double that?
        Quaternion target = Quaternion.identity;
        if (Mathf.Abs(v) < 10f)
            target = Quaternion.Euler(0, 0, v * 2f + 180);
        //geared
        else if (Mathf.Abs(v) >= 10f)
            //take in to account if v is positive or negative
            if (v > 0)
                target = Quaternion.Euler(0, 0, 10 + ((v)) + 180);
            else
                target = Quaternion.Euler(0, 0, -10 + ((v)) + 180);


        float z = Mathf.Clamp(target.eulerAngles.z, 140, 220); //could need more work, flicks to each side at some point over 200
        target = Quaternion.Euler(0, 0, z);

        return target;
    }

    internal static Quaternion TurnCoordinatorBallTarget(float v, float multiplier)//top needle
    {
        v *= -1000;

        float gearChange = 10f;
        Quaternion target = Quaternion.identity;
        if (Mathf.Abs(v) < gearChange)
            target = Quaternion.Euler(0, 0, v);
        //geared
        else if (Mathf.Abs(v) >= gearChange)
            //take in to account if v is positive or negative
            if (v > 0)
                target = Quaternion.Euler(0, 0, ((gearChange + v)));
            else
                target = Quaternion.Euler(0, 0, -(gearChange - v));



        return target;

    }


    internal static Quaternion VerticalSpeedTarget(float verticalSpeed)
    {
        //vsi
        //start at 9 o'clock
        verticalSpeed = 90f - verticalSpeed * 7.2f;


        //clamp to 90 degrees /TODO dont clamp hee, clamp after thre rotations applied to its hitsd the pin opn the dial rather coming to a smooth stop
        verticalSpeed = Mathf.Clamp(verticalSpeed, -90, 270);

        Quaternion target = Quaternion.Euler(0, 0, verticalSpeed);

        return target;
    }

    //repeater compass

    internal static Quaternion RepeaterCompassTarget(float heading)
    {
        //number passed is rotation in rads, pi = 180 degrees
        Quaternion target = Quaternion.Euler(0, 0, -heading * Mathf.Rad2Deg);

        return target;
    }


    internal static Quaternion RPMATarget(float rpm, float scalar, float scalar1, AnimationCurve curve)
    {
        //-315 full needle spin to 3000
        //and work out percentage to use 0-1 scale for curve
        float highest = 4000;
        float percentage = (Mathf.Abs(rpm / highest));

        //multiply by half a dial of spin (180 degrees)
        float angleToSpin = curve.Evaluate(percentage);

        angleToSpin *= -335;
        angleToSpin -= 180;
        Quaternion target = Quaternion.Euler(0, 0, angleToSpin);



        return target;
    }

    internal static Quaternion RPMBTarget(float rpm, float scalar, float scalar2)
    {
        float r;

        //geared
        if (rpm < 1000)
        {
            r = rpm * -0.02f;
            r -= 180;
        }
        else if (rpm >= 1000 && rpm < 2000)
        {
            //make start point for rpm multiplication 
            rpm -= 1000;
            r = rpm * -0.07f;
            //add degrees back on where we started ( at 10 on dial)
            r -= 200;
        }
        else if (rpm >= 2000 && rpm < 4000)
        {
            //make start point for rpm multiplication 
            rpm -= 2000;
            r = rpm * -0.09f;
            //add degrees back on where we started ( at 20 on dial)
            r -= 270;
        }
        else
        //over 4000
        {
            //make start point for rpm multiplication 
            rpm -= 4000;
            r = rpm * -0.07f;
            //add degrees back on where we started ( at 40 on dial)
            r -= 90;
        }



        Quaternion target = Quaternion.Euler(0, 0, r);

        return target;
    }

    internal static Quaternion RPMCLargeTarget(float rpm)
    {
        float r = rpm * -0.036f;
        Quaternion target = Quaternion.Euler(0, 0, r);

        return target;
    }

    //2nd needle on two needle rpm
    internal static Quaternion RPMCSmallTarget(float rpm)
    {
        float r = rpm * -0.36f;
        Quaternion target = Quaternion.Euler(0, 0, r);

        return target;
    }

    internal static Quaternion ManifoldTargetA(float manifold, float scalar)
    {
        //UK =  read manifold, minus 101325.00, divide by 6894.76
        //manifold -= 101325f; //air pressure
        //manifold /= 6894.76f; // pascal

        //some strange behaviour on low values
        //if (manifold <= -7)
        //    manifold = -8.2f;

        float m = 0;


        if (manifold <= 8)
        {
            m = manifold * -11;
        }
        else
        {
            //gear 3
            m = (manifold - 8) * -9;
            m -= 90;
        }



        Quaternion target = Quaternion.Euler(0, 0, m);

        return target;
    }

    internal static Quaternion ManifoldTargetC(float manifold, float scalar, float s2)
    {
        //UK =  read manifold, minus 101325.00, divide by 6894.76
        // manifold -= 101325f; //air pressure
        // manifold /= 6894.76f; // pascal

        //some strange behaviour on low values
        //if (manifold <= -7)
        //    manifold = -8.2f;

        float m = 0;

        if (manifold <= 4)
        {
            m = manifold * -15;
        }
        else
        {
            //gear 3
            m = (manifold - 4) * -12.66666667f;
            m -= 60;
        }

        Quaternion target = Quaternion.Euler(0, 0, m);

        return target;
    }

    internal static Quaternion RepeaterCompassAlternateTarget(float heading)
    {
        //number passed is rotation in rads, pi = 180 degrees
        Quaternion target = Quaternion.Euler(0, 0, -heading * Mathf.Rad2Deg);

        return target;
    }

    internal static Quaternion WaterTempTargetA(float v, float scalar, float scalar1, AnimationCurve curve, string name)
    {
        if (name == "Spitfire Mk.Vb" || name == "Spitfire Mk.XIV") //proba;y some other calcs going on in the game
        {
            v *= -1.118f;
        }
        else if (name == "Spitfire Mk.IXe")
        {
            v *= -1.1667f;
        }
        //and work out percentage to use 0-1 scale for curve
        float highest = 140;
        float percentage = (Mathf.Abs(v / highest));

        //Debug.Log(percentage);

        //multiply by half a dial of spin (180 degrees)
        float angleToSpin = curve.Evaluate(percentage);

        angleToSpin *= -330;
        angleToSpin -= -180;

        Quaternion target = Quaternion.Euler(0, 0, angleToSpin);

        return target;
    }

    internal static Quaternion WaterTempTargetB(float v, float scalar0, float scalar1, string name)
    {
        if (name == "Spitfire Mk.XIV") //only one with this dial?
        {
            v *= -0.86f;
        }
        else
        {
            v *= -0.789f;
        }
        v -= -71;
        v = Mathf.Clamp(v, -39.33f, 39.33f);


        return Quaternion.Euler(0, 0, v);
    }

    internal static Quaternion OilTempTargetA(float v, float scalar0, float scalar1)
    {
        v *= -3f;
        v += 150;
        Quaternion target = Quaternion.Euler(0, 0, v);

        return target;
    }

    internal static Quaternion OilTempTargetB(float v, float scalar0, float scalar1, string name)
    {
        if (name == "Spitfire Mk.XIV") //only one with this dial?
        {
            v *= -1f;// -1.05f;
        }
        else
        {
            v *= -0.789f;
        }
        v -= -71;
        v = Mathf.Clamp(v, -39.33f, 39.33f);


        return Quaternion.Euler(0, 0, v);
    }
}
