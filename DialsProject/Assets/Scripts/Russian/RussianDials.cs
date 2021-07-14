using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RussianDials : MonoBehaviour
{
   public static Quaternion AirspeedTarget(float airspeed)
    {
        if (airspeed == 0)
            return Quaternion.identity;

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

    public static Vector3 HeadingIndicatorPosition(float heading)
    {
        


       // heading -= Mathf.PI;

        //check for Nan
        if (float.IsNaN(heading) || heading == 0f)
            return Vector3.zero;

        //range is 0 to pi*2
        float ratio = Mathf.PI * heading;
        //adjust for arbitry render camera position
        ratio *= 1.5855f;

        Vector3 pos = Vector3.right*ratio;
        

        return pos;
    }

    public static Quaternion TurnAndBankPlaneRotation(float roll,float climb, float rollMultiplier, float climbMultiplier)
    {        
        //rotate plane
        //clamp roll , in game cockpit stop rotation at just under 90 degrees - this happens when roll rate is ~1.7
        float tempRoll = -roll;
        Mathf.Clamp(tempRoll, -1.7f, 1.7f);
        Quaternion t = Quaternion.Euler(0, 0, tempRoll * rollMultiplier);
        Quaternion r = t;

        //for x rotatin we need to rotate around global x after z rot
        r *= Quaternion.Euler(climb * climbMultiplier, 0, 0);

        return r;
    }

    public static Vector3 TurnAndBankPlanePosition(float climb, float pitchMultiplier)
    {
        //move plane up and down
        return new Vector3(0, climb * pitchMultiplier, 0);
    }

    public static Vector3 TurnAndBankNumberTrackPosition(float climb, float pitchMultiplier)
    {
        //number track
        return new Vector3(0, climb * pitchMultiplier, 1.5f);
    }

    public static Quaternion RateOfTurn (float airspeed, float roll)//not correct
    {


        /*
         calculate "rate of turn"

        rate of turn = 1091*tan(angle of bank)/ True Airspeed.(knots?)
    
        km/h to knots = divide the speed value by 1.852
 

        angle of bank is roll from il2gameclient
        */
        if (airspeed == 0)
            return Quaternion.identity;

        float knots = airspeed * 1.852f;
        float rateOfTurn = (1091f * Mathf.Tan(roll)) / knots;

        Debug.Log("rate of turn = " + rateOfTurn);

        return Quaternion.Euler(0, 0, -rateOfTurn); //add spring?
    }


    public static Quaternion TurnCoordinatorNeedleTargetzzz(float currentHeading, float targetHeading, float lastMessageReceivedTime, float previousMessageTime, float multiplier, float roll, float rollMod)
    {

        //find rate of change on local y rotation

        //we know
        //roll
        //pitch
        //heading
        //velocity


        
        Quaternion target = Quaternion.Euler(0, 0, 0);



        return target;
    }

    public static Quaternion TurnCoordinatorNeedleTarget(float v)
    {
        Quaternion target = Quaternion.Euler(0, 0, v);

        return target;
    }

    public static Quaternion TurnCoordinatorNeedleTargetSpring(out float outVelocity, float previousHeading, float currentHeading, float targetHeading, float lastMessageReceivedTime, float previousMessageTime, float currentVelocity, float stiffness, float damping, float multiplier,float roll, float rollMod)
    {
        //catch where heading goes from pi*2 to 0
        if (Mathf.Abs(currentHeading - previousHeading) > Mathf.PI)
        {
            //current heading ~6.24, previous heading is ~ 0.01
            //bring current heading down to previous heading - we only need to find out the difference
            if (currentHeading - previousHeading < 0)
                currentHeading += Mathf.PI * 2;
            else
                currentHeading -= Mathf.PI * 2;
        }

        

        //heading diff between last two frames
        float diff = currentHeading - previousHeading;

        //time diff
        float delta =  lastMessageReceivedTime - previousMessageTime; //?


        //limit how fast it moves- needed?
        float clamp = .02f;
        float diffClamped = Mathf.Clamp(diff, -clamp, clamp);



        //////////
        //float prevValue = currentHeading - previousHeading;
        float currentValue = currentHeading - previousHeading;
        //float _currentVelocity = currentVelocity;
        float targetValue = targetHeading - currentHeading;
        //float stiffness = 0.01f;// .0000011f; // value highly dependent on use case
        //float damping = 0.1f; // 0 is no damping, 1 is a lot, I think
        float valueThreshold = 0.01f;
        float velocityThreshold = 0.01f;

        float dampingFactor = Mathf.Max(0, 1 - damping * delta);
        float acceleration = (targetValue - currentValue) * stiffness * delta;
        currentVelocity = currentVelocity * dampingFactor + acceleration;
        currentValue += currentVelocity * delta;

        if (Mathf.Abs(currentValue - targetValue) < valueThreshold && Mathf.Abs(currentVelocity) < velocityThreshold)
        {
            currentValue = targetValue;
            currentVelocity = 0f;
        }

        ///

        currentValue *= -multiplier;

        //diff /= delta;
        //
        // diff *= -200;

        currentValue *= 1f + ((roll * Mathf.PI) * rollMod);

        //Debug.Log("curr heading = " + currentHeading);
        //Debug.Log("prev heading = " + previousHeading);

        //Debug.Log("dif =" + diff);
        //Debug.Log("delta =" + delta);
        Debug.Log("currentValue =" + currentValue);
        Debug.Log("currentVelocity =" + currentVelocity);

        // diff = Mathf.SmoothDamp(previousHeading, currentHeading, ref velocity, .1f);
        //limit max degrees
        clamp = 20f;
        currentValue = Mathf.Clamp(currentValue, -clamp, clamp);



        Quaternion target = Quaternion.Euler(0, 0, currentValue);



        outVelocity = currentVelocity;
        return target;
    }


    public static Quaternion TurnCoordinatorBallTarget(float ball)//?
    {
        //indicates whether the aircraft is in coordinated flight, showing the slip or skid of the turn. 


        float t = ball*3;
        float clamp = 12f;
        t = Mathf.Clamp(t, -clamp, clamp);

        Quaternion target = Quaternion.Euler(0, 0, t);

        return target;
    }

    public static Quaternion VerticalSpeedTarget(float verticalSpeed)//?
    {
        //vsi
        //start at 9 o'clock
        verticalSpeed = 90f - verticalSpeed * 18f;
        //clamp to "10"
        verticalSpeed = Mathf.Clamp(verticalSpeed, -90, 270);

        Quaternion target = Quaternion.Euler(0, 0,  verticalSpeed);

        return target;
    }
}
