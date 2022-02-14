using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialTargets : MonoBehaviour
{



    public static List<List<Quaternion>> ManifoldTarget(AirplaneData airplaneData, Country country)
    {
        List<Quaternion> manifoldSmallTargets = new List<Quaternion>(new Quaternion[airplaneData.planeAttributes.engines]);
        List<Quaternion> manifoldLargeTargets = new List<Quaternion>(new Quaternion[airplaneData.planeAttributes.engines]);

        for (int i = 0; i < airplaneData.planeAttributes.engines; i++)
        {
            switch (country)
            {
                case (Country.GER):
                    {
                        manifoldLargeTargets[i] = GermanDials.ManifoldTarget(airplaneData.manifolds[i], airplaneData.scalar0, airplaneData.scalar1);
                        break;
                    }
            }
        }

        return new List<List<Quaternion>>() { manifoldSmallTargets, manifoldLargeTargets };
    }

    public static List<List<Quaternion>> RPMTarget(AirplaneData airplaneData, Country country, RotateNeedle rotateNeedle )
    {
        List<List<Quaternion>> rpmTargets = new List<List<Quaternion>>();
        List<Quaternion> rpmLargeTargets = new List<Quaternion>( new Quaternion[airplaneData.planeAttributes.engines]);
        List<Quaternion> rpmSmallTargets = new List<Quaternion>(new Quaternion[airplaneData.planeAttributes.engines]);

        for (int i = 0; i < airplaneData.planeAttributes.engines; i++)
        {
            switch (country)
            {
                //RU
                case (Country.RU):
                    if (airplaneData.planeAttributes.rpmType == DialVariant.A)
                    {
                        rpmLargeTargets[i] = RussianDials.RPMALargeTarget(airplaneData.rpms[i]);
                        rpmSmallTargets[i] = RussianDials.RPMASmallTarget(airplaneData.rpms[i]);
                    }
                    else if (airplaneData.planeAttributes.rpmType == DialVariant.B)
                    {
                        rpmLargeTargets[i] = RussianDials.RPMBTarget(airplaneData.rpms[i], airplaneData.scalar0, airplaneData.scalar1);
                    }
                    else if (airplaneData.planeAttributes.rpmType == DialVariant.C)
                    {
                        rpmLargeTargets[i] = RussianDials.RPMCLargeTarget(airplaneData.rpms[i]);
                        rpmSmallTargets[i] = RussianDials.RPMCSmallTarget(airplaneData.rpms[i]);
                    }

                    break;

                //GER
                case (Country.GER):
                    if (airplaneData.planeAttributes.rpmType == DialVariant.A)
                    {
                        rpmLargeTargets[i] = GermanDials.RPMATarget(airplaneData.rpms[i], airplaneData.scalar0, airplaneData.scalar1);
                    }
                    else if (airplaneData.planeAttributes.rpmType == DialVariant.B)
                    {
                        rpmLargeTargets[i] = GermanDials.RPMBTarget(airplaneData.rpms[i], airplaneData.scalar0, airplaneData.scalar1,rotateNeedle.animationCurveRPMA);
                    }
                    else if (airplaneData.planeAttributes.rpmType == DialVariant.C)
                    {
                        rpmLargeTargets[i] = GermanDials.RPMCTarget(airplaneData.rpms[i], airplaneData.scalar0, airplaneData.scalar1, rotateNeedle.animationCurveRPMC);
                    }
                    else
                        rpmLargeTargets[i] = GermanDials.RPMDTarget(airplaneData.rpms[i], airplaneData.scalar0, airplaneData.scalar1, rotateNeedle.animationCurveRPMD);

                    break;

                //US
                case (Country.US):
                    if (airplaneData.planeAttributes.rpmType == DialVariant.A)
                    {
                        rpmLargeTargets[i] = USDials.RPMATarget(airplaneData.rpms[i], airplaneData.scalar0, airplaneData.scalar1);
                        rpmSmallTargets[i] = USDials.RPMAInnerTarget(airplaneData.rpms[i], airplaneData.scalar0, airplaneData.scalar1);
                    }
                    else if (airplaneData.planeAttributes.rpmType == DialVariant.B)
                    {
                        rpmLargeTargets[i] = USDials.RPMBTarget(airplaneData.rpms[i], airplaneData.scalar0, airplaneData.scalar1);
                    }
                    else if (airplaneData.planeAttributes.rpmType == DialVariant.C)
                    {
                        rpmLargeTargets[i] = USDials.RPMCTarget(airplaneData.rpms[i], airplaneData.scalar0, airplaneData.scalar1);
                    }
                    else if (airplaneData.planeAttributes.rpmType == DialVariant.D)
                    {
                        //we can use A for big needle
                        rpmLargeTargets[i] = USDials.RPMATarget(airplaneData.rpms[i], airplaneData.scalar0, airplaneData.scalar1);
                        rpmSmallTargets[i] = USDials.RPMDSmallTarget(airplaneData.rpms[i], airplaneData.scalar0, airplaneData.scalar1);
                    }
                    else if (airplaneData.planeAttributes.rpmType == DialVariant.E)
                    {
                        //note we use hard indexes for rpms and engine lists. P38J is asigend "1 engine" because it only has 1 engine dial
                        rpmLargeTargets[0] = USDials.RPMCTarget(airplaneData.rpms[0], airplaneData.scalar0, airplaneData.scalar1);
                        rpmSmallTargets[0] = USDials.RPMCTarget(airplaneData.rpms[1], airplaneData.scalar0, airplaneData.scalar1);
                    }

                    break;


                case (Country.UK):
                    if (airplaneData.planeAttributes.rpmType == DialVariant.A)
                    {
                        //A Taret is first needle
                        rpmLargeTargets[i] = UKDials.RPMATarget(airplaneData.rpms[i], airplaneData.scalar0, airplaneData.scalar1, rotateNeedle.animationCurveRPMA);

                    }
                    else if (airplaneData.planeAttributes.rpmType == DialVariant.B)
                    {
                        //"A" Target is first Needle - not the best naming
                        rpmLargeTargets[i] = UKDials.RPMBTarget(airplaneData.rpms[i], airplaneData.scalar0, airplaneData.scalar1);
                    }
                    break;

                case (Country.ITA):

                    if (airplaneData.planeAttributes.rpmType == DialVariant.A)
                    {
                        rpmLargeTargets[i] = ITADials.RPMATarget(airplaneData.rpms[i], airplaneData.scalar0, airplaneData.scalar1);
                    }
                    break;

            }
        }


        return new List<List<Quaternion>>() { rpmSmallTargets, rpmLargeTargets };


    }

    public static Quaternion ArtificialHorizonTargets(ref Quaternion artificialHorizonNeedleTarget, ref Vector3 artificialHorizonPositionTarget, ref Quaternion artificialHorizonChevronTarget, ref Quaternion artificialHorizonRotationPlaneTarget,
                                                    AirplaneData airplaneData, GameObject artificialHorizonNeedle, 
                                                    float artificialHorizonRollMod, float artificialHorizonMultiplier, Country country)
    {
        Quaternion artificialHorizonRotationTarget = Quaternion.identity;

        switch (country)
        {
            //no RU

            //GER
            case (Country.GER):
                //rotation // roll
                artificialHorizonRotationTarget = GermanDials.ArtificialHorizon(airplaneData.roll, artificialHorizonRollMod);

                //position  
                artificialHorizonPositionTarget = GermanDials.ArtificialHorizonPosition(airplaneData.pitch, artificialHorizonMultiplier);

                //use the same function that turn co-ordinator uses
                if (artificialHorizonNeedle != null)
                    artificialHorizonNeedleTarget = GermanDials.TurnCoordinatorNeedleTarget(airplaneData.turnCoordinatorNeedle, airplaneData.planeType);

                break;


            //US
            case (Country.US):
                //rotation // roll

                artificialHorizonRotationTarget = USDials.ArtificialHorizonRotation(airplaneData.roll, artificialHorizonRollMod);

                //position  /pitch                
                artificialHorizonPositionTarget = USDials.ArtificialHorizonPosition(airplaneData.pitch, artificialHorizonMultiplier);


                //chevron
                artificialHorizonChevronTarget = USDials.ArtificialHorizonChevronRotation(airplaneData.roll, artificialHorizonRollMod);


                break;

            case (Country.UK):

                artificialHorizonRotationTarget = UKDials.ArtificialHorizonRotation(airplaneData.roll, artificialHorizonRollMod);

                //position  
                artificialHorizonPositionTarget = UKDials.ArtificialHorizonPosition(airplaneData.pitch, artificialHorizonMultiplier);
                //chevron
                artificialHorizonChevronTarget = UKDials.ArtificialHorizonChevronRotation(airplaneData.roll, artificialHorizonRollMod);
                break;

            case (Country.ITA):

                //rotation of plane
                artificialHorizonRotationPlaneTarget = ITADials.ArtificialHorizonRotation(airplaneData.roll, artificialHorizonRollMod);

                //position of moving track/ball, only moves on Y axis
                artificialHorizonPositionTarget = ITADials.ArtificialHorizonPosition(airplaneData.pitch, artificialHorizonMultiplier);
                break;
        }

        return artificialHorizonRotationTarget;
    }

    public static Quaternion RepeaterCompassTarget(ref Quaternion repeaterCompassAlternateTarget, AirplaneData airplaneData, GameObject compassRim, Country country)
    {
        Quaternion repeaterCompassTarget = Quaternion.identity;

        if (country == Country.GER)
        {
            if (airplaneData.planeAttributes.repeaterCompass)
            {
                //if user spins rim
                float offset = compassRim.transform.eulerAngles.z;
                repeaterCompassTarget = GermanDials.RepeaterCompassTarget(airplaneData.heading, offset);
            }

            if (airplaneData.planeAttributes.repeaterCompassAlternate)
            {
                //Junkers unique dial
                repeaterCompassAlternateTarget = GermanDials.RepeaterCompassAlternateTarget(airplaneData.heading);
            }
        }

        else if (country == Country.US)
            repeaterCompassTarget = USDials.RepeaterCompassTarget(airplaneData.heading);

        else if (country == Country.UK)
            repeaterCompassTarget = UKDials.RepeaterCompassTarget(airplaneData.heading);

        return repeaterCompassTarget;
    }

    public static Quaternion VSITarget(AirplaneData airplaneData,AnimationCurve animationCurveVSI, Country country)
    {
        Quaternion vsiNeedleTarget = Quaternion.identity;

        switch (country)
        {
            case (Country.RU):
                if (airplaneData.planeAttributes.vsiLarge)
                    vsiNeedleTarget = RussianDials.VerticalSpeedTargetLarge(airplaneData.verticalSpeed);

                else if (airplaneData.planeAttributes.vsiSmall)
                    vsiNeedleTarget = RussianDials.VerticalSpeedTargetSmall(airplaneData.verticalSpeed);

                break;

            case (Country.GER):

                if (airplaneData.planeAttributes.vsiLarge)
                {
                    //need to clamp vertical speed - helps with coming back from over the limit
                    airplaneData.verticalSpeed = Mathf.Clamp(airplaneData.verticalSpeed, -30f, 30f);
                    vsiNeedleTarget = GermanDials.VerticalSpeedTargetLarge(airplaneData.verticalSpeed);
                }

                else if (airplaneData.planeAttributes.vsiSmall)
                {
                    airplaneData.verticalSpeed = Mathf.Clamp(airplaneData.verticalSpeed, -15f, 15f);
                    vsiNeedleTarget = GermanDials.VerticalSpeedTargetSmall(airplaneData.verticalSpeed);
                }

                else if (airplaneData.planeAttributes.vsiSmallest)
                {
                    airplaneData.verticalSpeed = Mathf.Clamp(airplaneData.verticalSpeed, -5f, 5f);
                    vsiNeedleTarget = GermanDials.VerticalSpeedTargetSmallest(airplaneData.verticalSpeed);
                }

                break;

            //these countries only have one vsi (so far)
            case (Country.US):
                vsiNeedleTarget = USDials.VerticalSpeedTarget(airplaneData.verticalSpeed, animationCurveVSI);
                break;

            case (Country.UK):
                vsiNeedleTarget = UKDials.VerticalSpeedTarget(airplaneData.verticalSpeed);
                break;

            case (Country.ITA):
                vsiNeedleTarget = ITADials.VerticalSpeedTarget(airplaneData.verticalSpeed);
                break;

        }

        return vsiNeedleTarget;
    }

    public static Quaternion TurnCoordinatorTarget(ref Quaternion turnCoordinatorBallTarget, AirplaneData airplaneData, float turnCoordinatorNeedleMod, float turnCoordinatorBallMod, Country country)
    {
        Quaternion turnCoordinatorNeedleTarget = Quaternion.identity;


        switch (country)
        {
            case (Country.RU):
                //RU
                //pendulum needle
                turnCoordinatorNeedleTarget = RussianDials.TurnCoordinatorNeedleTarget(airplaneData.turnCoordinatorNeedle, turnCoordinatorNeedleMod);

                //ball indicator                
                turnCoordinatorBallTarget = RussianDials.TurnCoordinatorBallTarget(airplaneData.turnCoordinatorBall);
                break;

            case (Country.GER):
                //RU
                //pendulum needle
                turnCoordinatorNeedleTarget = GermanDials.TurnCoordinatorNeedleTarget(airplaneData.turnCoordinatorNeedle, airplaneData.planeType);

                //ball indicator                
                turnCoordinatorBallTarget = GermanDials.TurnCoordinatorBallTarget(airplaneData.turnCoordinatorBall, turnCoordinatorBallMod);
                break;

            case (Country.US):
                if (airplaneData.planeType == "A-20B")
                    turnCoordinatorNeedleTarget = USDials.TurnCoordinatorNeedleTarget(airplaneData.turnCoordinatorNeedle, true);
                else
                    turnCoordinatorNeedleTarget = USDials.TurnCoordinatorNeedleTarget(airplaneData.turnCoordinatorNeedle, false);

                //ball indicator                
                turnCoordinatorBallTarget = USDials.TurnCoordinatorBallTarget(airplaneData.turnCoordinatorBall, turnCoordinatorBallMod);
                break;


            case (Country.UK):
                turnCoordinatorNeedleTarget = UKDials.TurnCoordinatorNeedleTarget(airplaneData.turnCoordinatorNeedle, turnCoordinatorNeedleMod);

                //second needle        
                turnCoordinatorBallTarget = UKDials.TurnCoordinatorBallTarget(airplaneData.turnCoordinatorBall, turnCoordinatorBallMod);
                break;


            case (Country.ITA):
                turnCoordinatorNeedleTarget = ITADials.TurnCoordinatorNeedleTarget(airplaneData.turnCoordinatorNeedle);

                //second needle        
                turnCoordinatorBallTarget = ITADials.TurnCoordinatorBallTarget(airplaneData.turnCoordinatorBall, turnCoordinatorBallMod);
                break;

        }

        return turnCoordinatorNeedleTarget;

    }

    //turn and bank is dial with artifical horizon and slip together
    public static Quaternion TurnAndBankTargets(ref Vector3 turnAndBankPlanePositionTarget, ref Vector3 turnAndBankNumberTrackTarget, ref Quaternion turnAndBankBallTarget, AirplaneData airplaneData,
                                                float turnAndBankPitchMultiplier, float turnAndBankRollMultiplier, float turnAndBankPlaneXMultiplier, float turnAndBankBallMultiplier,
                                                Country country)
    {
        Quaternion turnAndBankPlaneRotationTarget = Quaternion.identity;

        //plane or background pos

        switch (country)
        {
            case (Country.RU):
                //note russian is quite different - more like an artifical horizon with plane moving instead of horizon
                turnAndBankPlanePositionTarget = RussianDials.TurnAndBankPlanePosition(airplaneData.pitch, turnAndBankPitchMultiplier);

                //plane or background rotation
                turnAndBankPlaneRotationTarget = RussianDials.TurnAndBankPlaneRotation(airplaneData.roll, airplaneData.pitch, turnAndBankRollMultiplier, turnAndBankPlaneXMultiplier);

                turnAndBankNumberTrackTarget = RussianDials.TurnAndBankNumberTrackPosition(airplaneData.pitch, turnAndBankPitchMultiplier);

                break;

            // with slip?
            case (Country.GER):
                turnAndBankPlanePositionTarget = GermanDials.TurnAndBankPlanePosition(airplaneData.pitch, turnAndBankPitchMultiplier);

                turnAndBankPlaneRotationTarget = GermanDials.TurnAndBankPlaneRotation(airplaneData.roll, airplaneData.pitch, turnAndBankRollMultiplier, turnAndBankRollMultiplier);

                turnAndBankBallTarget = GermanDials.TurnAndBankBallTarget(airplaneData.turnCoordinatorBall, turnAndBankBallMultiplier);

                break;

        }

        return turnAndBankPlaneRotationTarget;


    }

    public static Vector3 HeadingTarget(Country country, AirplaneData airplaneData, float trackLength)
    {

        Vector3 headingIndicatorTarget = new Vector3();
        switch (country)
        {
            case (Country.RU):
                headingIndicatorTarget = RussianDials.HeadingIndicatorPosition(airplaneData.heading, trackLength);
                break;

            case (Country.GER):
                headingIndicatorTarget = GermanDials.HeadingIndicatorPosition(airplaneData.heading, trackLength);
                break;

            case (Country.US):
                headingIndicatorTarget = USDials.HeadingIndicatorPosition(airplaneData.heading, trackLength);
                break;

            case (Country.UK):
                headingIndicatorTarget = UKDials.HeadingIndicatorPosition(airplaneData.heading, trackLength);
                break;

            case (Country.ITA):
                headingIndicatorTarget = ITADials.HeadingIndicatorPosition(airplaneData.heading, trackLength);
                break;
        }


        return headingIndicatorTarget;
    }

    public static Quaternion AltimeterTargets(ref Quaternion altitudeSmallTarget, ref Quaternion altitudeSmallestTarget, 
                                            GameObject altitudeNeedleSmallest, AirplaneData airplaneData)
    {
        Quaternion altitudeLargeTarget;

        //if mini needle
        if (altitudeNeedleSmallest != null)
            altitudeSmallestTarget = AltitudeTargetSmallest(airplaneData.planeAttributes.country, airplaneData.altitude);

        altitudeSmallTarget = AltitudeTargetSmall(airplaneData.planeAttributes.country, airplaneData.altitude);
        altitudeLargeTarget = AltitudeTargetLarge(airplaneData.planeAttributes.country, airplaneData.altitude);

        return altitudeLargeTarget;
        
    }

    public static Quaternion PressureReferenceTargets(AirplaneData airplaneData)
    {   //Pressure //mmhg
        Quaternion mmhgTarget = Quaternion.identity;
        //set where we rotate from
        //MmhgStart();

        //set where we are rotating to
        mmhgTarget = AtmosphericPressure(airplaneData.planeAttributes.country, airplaneData.mmhg);

        return mmhgTarget;

    }

    public static Quaternion AtmosphericPressure(Country country, float unit)
    {
        Quaternion target = Quaternion.identity;

        //each country has slightly different dials, we need to work out rotations individually for each
        switch (country)
        {
            case Country.RU:
                target = RussianDials.MmhgTarget(unit);
                break;

            case Country.GER:
                target = GermanDials.MmhgTarget(unit);
                break;

            case Country.US:
                target = USDials.MmhgTarget(unit);
                break;

            case Country.UK:
                target = UKDials.MmhgTarget(unit);
                break;

            case Country.ITA:
                target = ITADials.MmhgTarget(unit);
                break;
        }

        return target;

    }

   public static Quaternion AirspeedTarget(Country country, float airspeed, Speedometer speedoType)
    {

        Quaternion airspeedTarget = Quaternion.identity;
        //each country has slightly different dials, we need to work out rotations individually for each
        switch (country)
        {
            case Country.RU:
                airspeedTarget = RussianDials.AirspeedTarget(airspeed);
                break;

            case Country.GER:
                airspeedTarget = GermanDials.AirspeedTarget(airspeed);
                break;

            case Country.US:
                if (speedoType == Speedometer.A)
                    airspeedTarget = USDials.AirspeedTargetA(airspeed);
                else
                    airspeedTarget = USDials.AirspeedTargetB(airspeed);
                break;

            case Country.UK:
                airspeedTarget = UKDials.AirspeedTarget(airspeed);
                break;

            case Country.ITA:
                airspeedTarget = ITADials.AirspeedTarget(airspeed);
                break;
        }

        return airspeedTarget;

    }

    public static Quaternion AltitudeTargetLarge(Country country, float altitude)
    {
        Quaternion target = Quaternion.identity;

        //each country has slightly different dials, we need to work out rotations individually for each
        switch (country)
        {
            case Country.RU:

                target = RussianDials.AltitudeTargetLarge(altitude);
                break;

            case Country.GER:
                target = GermanDials.AltitudeTargetLarge(altitude);
                break;

            case Country.US:
                target = USDials.AltitudeTargetLarge(altitude);
                break;

            case Country.UK:
                target = UKDials.AltitudeTargetLarge(altitude);
                break;

            case Country.ITA:
                target = ITADials.AltitudeTargetLarge(altitude);
                break;
        }


        return target;
    }

    public static Quaternion AltitudeTargetSmall(Country country, float altitude)
    {
        Quaternion target = Quaternion.identity;

        //each country has slightly different dials, we need to work out rotations individually for each
        switch (country)
        {
            case Country.RU:
                target = RussianDials.AltitudeTargetSmall(altitude);
                break;

            case Country.GER:
                target = GermanDials.AltitudeTargetSmall(altitude);
                break;

            case Country.US:
                target = USDials.AltitudeTargetSmall(altitude);
                break;

            case Country.UK:
                target = UKDials.AltitudeTargetSmall(altitude);
                break;

            case Country.ITA:
                target = ITADials.AltitudeTargetSmall(altitude);
                break;
        }


        return target;
    }

    public static Quaternion AltitudeTargetSmallest(Country country, float altitude)
    {
        Quaternion target = Quaternion.identity;

        //each country has slightly different dials, we need to work out rotations individually for each
        switch (country)
        {
            //only UK has smallest dial

            case Country.UK:
                target = UKDials.AltitudeTargetSmallest(altitude);
                break;

            case Country.US:
                target = USDials.AltitudeTargetSmallest(altitude);
                break;
        }


        return target;
    }
}
