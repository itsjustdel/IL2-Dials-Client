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
                case (Country.RU):
                    if (airplaneData.planeAttributes.manifoldType == DialVariant.A)
                    {
                        manifoldLargeTargets[i] = RussianDials.ManifoldTargetAB(airplaneData.manifolds[i], airplaneData.scalar0, airplaneData.scalar1);
                    }
                    else if (airplaneData.planeAttributes.manifoldType == DialVariant.B)
                    {
                        manifoldLargeTargets[i] = RussianDials.ManifoldTargetAB(airplaneData.manifolds[i], airplaneData.scalar0, airplaneData.scalar1);
                    }
                    else if (airplaneData.planeAttributes.manifoldType == DialVariant.C)
                    {
                        manifoldLargeTargets[i] = RussianDials.ManifoldTargetC(airplaneData.manifolds[i], airplaneData.scalar0);
                    }
                    break;

                case (Country.GER):
                    if (airplaneData.planeAttributes.manifoldType == DialVariant.A
                        || airplaneData.planeAttributes.manifoldType == DialVariant.B
                            || airplaneData.planeAttributes.manifoldType == DialVariant.C
                                || airplaneData.planeAttributes.manifoldType == DialVariant.F)
                    {

                        manifoldLargeTargets[i] = GermanDials.ManifoldTargetA(airplaneData.manifolds[i], airplaneData.planeType, airplaneData.engineModification, airplaneData.scalar0);


                        //me 410 has it's dial spun by 90 degrees, so add 90 to the target
                        if (airplaneData.planeType == "Me 410 A-1")
                            manifoldLargeTargets[i] *= Quaternion.Euler(0, 0, 90);
                    }

                    else if (airplaneData.planeAttributes.manifoldType == DialVariant.D)
                    {
                        manifoldLargeTargets[i] = GermanDials.ManifoldTargetD(airplaneData.manifolds[i], airplaneData.planeType, airplaneData.scalar0);
                    }
                    else if (airplaneData.planeAttributes.manifoldType == DialVariant.E)
                    {
                        manifoldLargeTargets[i] = GermanDials.ManifoldTargetE(airplaneData.manifolds[i], airplaneData.planeType, airplaneData.scalar0);
                    }

                    break;

                case (Country.US):
                    if (airplaneData.planeAttributes.manifoldType == DialVariant.A
                            || airplaneData.planeAttributes.manifoldType == DialVariant.B)
                    {
                        manifoldLargeTargets[i] = USDials.ManifoldTargetA(airplaneData.manifolds[i], airplaneData.scalar0);
                    }
                    else if (airplaneData.planeAttributes.manifoldType == DialVariant.C
                            || airplaneData.planeAttributes.manifoldType == DialVariant.D
                                    || airplaneData.planeAttributes.manifoldType == DialVariant.F)
                    {

                        manifoldLargeTargets[i] = USDials.ManifoldTargetC(airplaneData.manifolds[i], airplaneData.scalar0);
                    }
                    else if (airplaneData.planeAttributes.manifoldType == DialVariant.E)
                    {

                        manifoldLargeTargets[i] = USDials.ManifoldTargetE(airplaneData.manifolds[i], airplaneData.engineModification, airplaneData.scalar0);
                    }
                    break;

                case (Country.FR): //switch stacking - OR functionality // FR is a placeholder, uses UK dials
                case (Country.UK):
                    if (airplaneData.planeAttributes.manifoldType == DialVariant.A
                            || airplaneData.planeAttributes.manifoldType == DialVariant.B
                                || airplaneData.planeAttributes.manifoldType == DialVariant.D)
                    {
                        manifoldLargeTargets[i] = UKDials.ManifoldTargetA(airplaneData.manifolds[i], airplaneData.scalar0);
                    }
                    else if (airplaneData.planeAttributes.manifoldType == DialVariant.C)
                    {
                        manifoldLargeTargets[i] = UKDials.ManifoldTargetC(airplaneData.manifolds[i], airplaneData.scalar0, airplaneData.scalar1);
                    }
                    break;

                case (Country.ITA):
                    if (airplaneData.planeAttributes.manifoldType == DialVariant.A)
                    {
                        manifoldLargeTargets[i] = ITADials.ManifoldTargetA(airplaneData.manifolds[i], airplaneData.scalar0, airplaneData.scalar1);
                    }
                    break;
            }
        }

        return new List<List<Quaternion>>() { manifoldSmallTargets, manifoldLargeTargets };
    }

    public static List<List<Quaternion>> RPMTarget(AirplaneData airplaneData, Country country, RotateNeedle rotateNeedle)
    {
        List<Quaternion> rpmLargeTargets = new List<Quaternion>(new Quaternion[airplaneData.planeAttributes.engines]);
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
                        rpmLargeTargets[i] = GermanDials.RPMBTarget(airplaneData.rpms[i], airplaneData.scalar0, airplaneData.scalar1, rotateNeedle.animationCurveRPMA);
                    }
                    else if (airplaneData.planeAttributes.rpmType == DialVariant.C)
                    {
                        rpmLargeTargets[i] = GermanDials.RPMCTarget(airplaneData.rpms[i], airplaneData.scalar0, airplaneData.scalar1, rotateNeedle.animationCurveRPMC);
                    }
                    else if (airplaneData.planeAttributes.rpmType == DialVariant.D)
                    {
                        rpmLargeTargets[i] = GermanDials.RPMDTarget(airplaneData.rpms[i], airplaneData.scalar0, airplaneData.scalar1, rotateNeedle.animationCurveRPMD);
                    }
                    else
                        rpmLargeTargets[i] = GermanDials.RPMCTarget(airplaneData.rpms[i], airplaneData.scalar0, airplaneData.scalar1, rotateNeedle.animationCurveRPMC);

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
                        //p38 - two needles, one dial
                        //note we use hard indexes for rpms 
                        rpmLargeTargets[0] = USDials.RPMCTarget(airplaneData.rpms[0], airplaneData.scalar0, airplaneData.scalar1);
                        rpmLargeTargets[1] = USDials.RPMCTarget(airplaneData.rpms[1], airplaneData.scalar0, airplaneData.scalar1);

                        //force loop finish
                        i = airplaneData.planeAttributes.engines + 1;
                    }

                    break;

                case (Country.FR):
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
                    else if (airplaneData.planeAttributes.rpmType == DialVariant.C)
                    {
                        rpmLargeTargets[i] = UKDials.RPMCLargeTarget(airplaneData.rpms[i]);
                        rpmSmallTargets[i] = UKDials.RPMCSmallTarget(airplaneData.rpms[i]);
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

    internal static List<Quaternion> WaterTempTargets(AirplaneData airplaneData, Country country, RotateNeedle rotateNeedle)
    {
        List<Quaternion> waterTempTargets = new List<Quaternion>(new Quaternion[airplaneData.planeAttributes.engines]);

        for (int i = 0; i < airplaneData.planeAttributes.engines; i++)
        {
            switch (country)
            {
                //RU
                case (Country.RU):
                    if (airplaneData.planeAttributes.waterTempType == DialVariant.A)
                    {
                        waterTempTargets[i] = RussianDials.WaterTempTargetA(airplaneData.waterTemps[i], airplaneData.scalar0, airplaneData.scalar1);
                    }
                    else if (airplaneData.planeAttributes.waterTempType == DialVariant.B)
                    {
                        waterTempTargets[i] = RussianDials.WaterTempTargetB(airplaneData.waterTemps[i], airplaneData.scalar0, airplaneData.scalar1, airplaneData.planeType);
                    }
                    if (airplaneData.planeAttributes.waterTempType == DialVariant.C)
                    {
                        waterTempTargets[i] = RussianDials.WaterTempTargetC(airplaneData.waterTemps[i], airplaneData.scalar0, airplaneData.scalar1);
                    }

                    break;

                //GER
                case (Country.GER):

                    if (airplaneData.planeAttributes.waterTempType == DialVariant.A)
                    {
                        waterTempTargets[i] = GermanDials.WaterTempTargetA(airplaneData.waterTemps[i], airplaneData.scalar0, airplaneData.scalar1);
                    }
                    else if (airplaneData.planeAttributes.waterTempType == DialVariant.B)
                    {
                        waterTempTargets[i] = GermanDials.WaterTempTargetB(airplaneData.waterTemps[i], airplaneData.scalar0, airplaneData.scalar1);
                    }
                    else if (airplaneData.planeAttributes.waterTempType == DialVariant.C)
                    {
                        waterTempTargets[i] = GermanDials.WaterTempTargetC(airplaneData.waterTemps[i], airplaneData.scalar0, airplaneData.scalar1);
                    }
                    else if (airplaneData.planeAttributes.waterTempType == DialVariant.D)
                    {
                        waterTempTargets[i] = GermanDials.WaterTempTargetD(airplaneData.waterTemps[i], airplaneData.scalar0, airplaneData.scalar1);
                    }

                    break;

                //US
                case (Country.US):
                    if (airplaneData.planeAttributes.waterTempType == DialVariant.A)
                    {
                        waterTempTargets[i] = USDials.WaterTempTargetA(airplaneData.waterTemps[i], airplaneData.scalar0, airplaneData.scalar1);
                    }
                    else if (airplaneData.planeAttributes.waterTempType == DialVariant.B)
                    {
                        //p38 - two needles, one dial
                        waterTempTargets[0] = USDials.WaterTempTargetB(airplaneData.waterTemps[0], airplaneData.scalar0, airplaneData.scalar1, true);
                        waterTempTargets[1] = USDials.WaterTempTargetB(airplaneData.waterTemps[1], airplaneData.scalar0, airplaneData.scalar1, false);
                    }
                    else if (airplaneData.planeAttributes.waterTempType == DialVariant.C)
                    {
                        waterTempTargets[i] = USDials.WaterTempTargetC(airplaneData.waterTemps[i], airplaneData.scalar0, airplaneData.scalar1);
                    }

                    break;

                case (Country.FR):
                case (Country.UK):
                    if (airplaneData.planeAttributes.waterTempType == DialVariant.A)
                    {
                        waterTempTargets[i] = UKDials.WaterTempTargetA(airplaneData.waterTemps[i], airplaneData.scalar0, airplaneData.scalar1, rotateNeedle.animationCurveWaterTempA, airplaneData.planeType);
                    }
                    else if (airplaneData.planeAttributes.waterTempType == DialVariant.B)
                    {
                        waterTempTargets[i] = UKDials.WaterTempTargetB(airplaneData.waterTemps[i], airplaneData.scalar0, airplaneData.scalar1, airplaneData.planeType);
                    }

                    break;

                case (Country.ITA):

                    if (airplaneData.planeAttributes.waterTempType == DialVariant.A)
                    {
                        waterTempTargets[i] = ITADials.WaterTempTargetA(airplaneData.waterTemps[i], airplaneData.scalar0, airplaneData.scalar1, rotateNeedle.animationCurveWaterTempA);
                    }
                    break;

            }
        }

        return waterTempTargets;
    }

    internal static List<Quaternion> OilTempPressureTargets(AirplaneData airplaneData, Country country, RotateNeedle rotateNeedle)
    {
        List<Quaternion> oilTempTargets = new List<Quaternion>(new Quaternion[airplaneData.planeAttributes.engines]);
        for (int i = 0; i < airplaneData.planeAttributes.engines; i++)
        {
            switch (country)
            {
                //RU
                case (Country.RU):
                    if (airplaneData.planeAttributes.oilTempPressureType == DialVariant.A)
                    {
                        //inbound oil fro top dial of combo
                        oilTempTargets[i] = RussianDials.OilTempCombo(airplaneData.oilTempsOut[i], airplaneData.scalar0, airplaneData.scalar1);
                    }
                    break;

                //GER
                case (Country.GER):
                    break;

                //US
                case (Country.US):
                    if (airplaneData.planeAttributes.oilTempPressureType == DialVariant.A)
                    {
                        //inbound oil fro top dial of combo
                        oilTempTargets[i] = USDials.OilTempCombo(airplaneData.oilTempsOut[i], airplaneData.scalar0, airplaneData.scalar1);
                    }
                    break;

                case (Country.FR):
                case (Country.UK):
                    break;

                case (Country.ITA):
                    break;
            }
        }

        return oilTempTargets;
    }

    internal static List<Quaternion> OilTempOutTargets(AirplaneData airplaneData, Country country, RotateNeedle rotateNeedle)
    {
        List<Quaternion> oilTempTargets = new List<Quaternion>(new Quaternion[airplaneData.planeAttributes.engines]);

        for (int i = 0; i < airplaneData.planeAttributes.engines; i++)
        {
            switch (country)
            {
                //RU
                case (Country.RU):
                    if (airplaneData.planeAttributes.oilTempOutType == DialVariant.A)
                    {
                        oilTempTargets[i] = RussianDials.WaterTempTargetA(airplaneData.oilTempsOut[i], airplaneData.scalar0, airplaneData.scalar1);
                    }
                    else if (airplaneData.planeAttributes.oilTempOutType == DialVariant.B)
                    {
                        //note targetC is correct - there's one more water dial so variant type enums don't match up
                        oilTempTargets[i] = RussianDials.WaterTempTargetC(airplaneData.oilTempsOut[i], airplaneData.scalar0, airplaneData.scalar1);
                    }

                    break;

                //GER
                case (Country.GER):

                    if (airplaneData.planeAttributes.oilTempOutType == DialVariant.A)
                    {
                        oilTempTargets[i] = GermanDials.WaterTempTargetA(airplaneData.oilTempsOut[i], airplaneData.scalar0, airplaneData.scalar1);
                    }
                    else if (airplaneData.planeAttributes.oilTempOutType == DialVariant.B || airplaneData.planeAttributes.oilTempOutType == DialVariant.E)
                    {
                        oilTempTargets[i] = GermanDials.WaterTempTargetB(airplaneData.oilTempsOut[i], airplaneData.scalar0, airplaneData.scalar1);
                    }
                    else if (airplaneData.planeAttributes.oilTempOutType == DialVariant.C)
                    {
                        oilTempTargets[i] = GermanDials.OilTempTargetC(airplaneData.oilTempsOut[i], airplaneData.scalar0, airplaneData.scalar1);
                    }
                    else if (airplaneData.planeAttributes.oilTempOutType == DialVariant.F)
                    {
                        oilTempTargets[i] = GermanDials.OilTempTargetF(airplaneData.oilTempsOut[i], airplaneData.scalar0, airplaneData.scalar1, rotateNeedle.animationCurveOilTempF);
                    }

                    break;

                //US
                case (Country.US):
                    if (airplaneData.planeAttributes.oilTempOutType == DialVariant.A) //c47 double needle dial
                    {
                        //re use water target, same dial with a different label
                        oilTempTargets[0] = USDials.WaterTempTargetB(airplaneData.oilTempsOut[0], airplaneData.scalar0, airplaneData.scalar1, true);
                        oilTempTargets[1] = USDials.WaterTempTargetB(airplaneData.oilTempsOut[1], airplaneData.scalar0, airplaneData.scalar1, false);
                    }
                    else if (airplaneData.planeAttributes.oilTempOutType == DialVariant.B) // A20 double needle dial
                    {
                        oilTempTargets[0] = USDials.OilTempTargetB(airplaneData.oilTempsOut[0], airplaneData.scalar0, airplaneData.scalar1, true);
                        oilTempTargets[1] = USDials.OilTempTargetB(airplaneData.oilTempsOut[1], airplaneData.scalar0, airplaneData.scalar1, false);
                    }
                    else if (airplaneData.planeAttributes.oilTempOutType == DialVariant.C)
                    {
                        oilTempTargets[i] = USDials.WaterTempTargetC(airplaneData.oilTempsOut[i], airplaneData.scalar0, airplaneData.scalar1);
                    }

                    break;

                case (Country.FR):
                case (Country.UK):
                    if (airplaneData.planeAttributes.oilTempOutType == DialVariant.A)
                    {
                        oilTempTargets[i] = UKDials.OilTempTargetA(airplaneData.oilTempsOut[i], airplaneData.scalar0, airplaneData.scalar1);
                    }
                    else if (airplaneData.planeAttributes.oilTempOutType == DialVariant.B)
                    {
                        oilTempTargets[i] = UKDials.OilTempTargetB(airplaneData.oilTempsOut[i], airplaneData.scalar0, airplaneData.scalar1, airplaneData.planeType);
                    }

                    break;

                case (Country.ITA):
                    if (airplaneData.planeAttributes.oilTempOutType == DialVariant.A)
                    {
                        oilTempTargets[i] = ITADials.WaterTempTargetA(airplaneData.oilTempsOut[i], airplaneData.scalar0, airplaneData.scalar1, rotateNeedle.animationCurveWaterTempA);
                    }

                    break;
            }
        }

        return oilTempTargets;
    }

    internal static List<Quaternion> OilTempInTargets(AirplaneData airplaneData, Country country, RotateNeedle rotateNeedle)
    {
        List<Quaternion> oilTempTargets = new List<Quaternion>(new Quaternion[airplaneData.planeAttributes.engines]);

        for (int i = 0; i < airplaneData.planeAttributes.engines; i++)
        {
            switch (country)
            {
                //RU
                case (Country.RU):
                    if (airplaneData.planeAttributes.oilTempInType == DialVariant.A)
                    {
                        oilTempTargets[i] = RussianDials.OilTempInA(airplaneData.oilTempsIn[i], airplaneData.scalar0, airplaneData.scalar1);
                    }
                    else if (airplaneData.planeAttributes.oilTempInType == DialVariant.B)
                    {
                        oilTempTargets[i] = RussianDials.WaterTempTargetC(airplaneData.oilTempsIn[i], airplaneData.scalar0, airplaneData.scalar1);
                    }

                    break;

                //GER
                case (Country.GER):

                    if (airplaneData.planeAttributes.oilTempInType == DialVariant.A)
                    {
                        oilTempTargets[i] = GermanDials.WaterTempTargetA(airplaneData.oilTempsIn[i], airplaneData.scalar0, airplaneData.scalar1);
                    }
                    else if (airplaneData.planeAttributes.oilTempInType == DialVariant.B)
                    {
                        oilTempTargets[i] = GermanDials.WaterTempTargetB(airplaneData.oilTempsIn[i], airplaneData.scalar0, airplaneData.scalar1);
                    }
                    else if (airplaneData.planeAttributes.oilTempInType == DialVariant.C)
                    {
                        oilTempTargets[i] = GermanDials.WaterTempTargetC(airplaneData.oilTempsIn[i], airplaneData.scalar0, airplaneData.scalar1);
                    }
                    else if (airplaneData.planeAttributes.oilTempInType == DialVariant.D)
                    {
                        oilTempTargets[i] = GermanDials.WaterTempTargetD(airplaneData.oilTempsIn[i], airplaneData.scalar0, airplaneData.scalar1);
                    }
                    else if (airplaneData.planeAttributes.oilTempInType == DialVariant.E)
                    {
                        oilTempTargets[i] = GermanDials.OilTempFW190(airplaneData.oilTempsIn[i], airplaneData.oilTempsOut[i], airplaneData.scalar0, airplaneData.scalar1);
                    }
                    else if (airplaneData.planeAttributes.oilTempInType == DialVariant.F)
                    {
                        oilTempTargets[i] = GermanDials.OilTempTargetF(airplaneData.oilTempsIn[i], airplaneData.scalar0, airplaneData.scalar1, rotateNeedle.animationCurveOilTempF);
                    }
                    break;

                //US
                case (Country.US):
                    if (airplaneData.planeAttributes.oilTempInType == DialVariant.A)
                    {
                        oilTempTargets[i] = USDials.WaterTempTargetA(airplaneData.oilTempsIn[i], airplaneData.scalar0, airplaneData.scalar1);
                    }
                    else if (airplaneData.planeAttributes.oilTempInType == DialVariant.B)
                    {
                        //p38 - two needles, one dial
                        oilTempTargets[0] = USDials.WaterTempTargetB(airplaneData.oilTempsIn[0], airplaneData.scalar0, airplaneData.scalar1, true);
                        oilTempTargets[1] = USDials.WaterTempTargetB(airplaneData.oilTempsIn[1], airplaneData.scalar0, airplaneData.scalar1, false);
                    }
                    else if (airplaneData.planeAttributes.oilTempInType == DialVariant.C)
                    {
                        oilTempTargets[i] = USDials.WaterTempTargetC(airplaneData.oilTempsIn[i], airplaneData.scalar0, airplaneData.scalar1);
                    }

                    break;

                case (Country.FR):
                case (Country.UK):
                    if (airplaneData.planeAttributes.oilTempInType == DialVariant.A)
                    {
                        oilTempTargets[i] = UKDials.OilTempTargetA(airplaneData.oilTempsIn[i], airplaneData.scalar0, airplaneData.scalar1);
                    }
                    else if (airplaneData.planeAttributes.oilTempInType == DialVariant.B)
                    {
                        oilTempTargets[i] = UKDials.OilTempTargetB(airplaneData.oilTempsIn[i], airplaneData.scalar0, airplaneData.scalar1, airplaneData.planeType);
                    }

                    break;

                case (Country.ITA):

                    if (airplaneData.planeAttributes.oilTempInType == DialVariant.A)
                    {
                        oilTempTargets[i] = ITADials.WaterTempTargetA(airplaneData.oilTempsIn[i], airplaneData.scalar0, airplaneData.scalar1, rotateNeedle.animationCurveWaterTempA);
                    }
                    break;
            }
        }

        return oilTempTargets;
    }

    internal static List<Quaternion> OilTempComboTargets(AirplaneData airplaneData, Country country, RotateNeedle rotateNeedle)
    {
        List<Quaternion> oilTempTargets = new List<Quaternion>(new Quaternion[airplaneData.planeAttributes.engines * 2]); //two for each engine

        for (int i = 0; i < airplaneData.planeAttributes.engines; i++)
        {
            switch (country)
            {
                //RU
                case (Country.RU):
                    break;

                //GER
                case (Country.GER):

                    if (airplaneData.planeAttributes.oilTempComboType == DialVariant.A)
                    {
                        // we will save engine in in out as index 0, 1 and engine 2 as index 2, 3
                        oilTempTargets[0] = GermanDials.OilTempTargetF(airplaneData.oilTempsIn[0], airplaneData.scalar0, airplaneData.scalar1, rotateNeedle.animationCurveOilTempF);
                        oilTempTargets[1] = GermanDials.OilTempTargetF(airplaneData.oilTempsOut[0], airplaneData.scalar0, airplaneData.scalar1, rotateNeedle.animationCurveOilTempF);
                        oilTempTargets[2] = GermanDials.OilTempTargetF(airplaneData.oilTempsIn[1], airplaneData.scalar0, airplaneData.scalar1, rotateNeedle.animationCurveOilTempF);
                        oilTempTargets[3] = GermanDials.OilTempTargetF(airplaneData.oilTempsOut[1], airplaneData.scalar0, airplaneData.scalar1, rotateNeedle.animationCurveOilTempF);
                    }

                    //return out of for loop - Consider if we need the for loop when implementing other countries/planes
                    return oilTempTargets;

                //US
                case (Country.US):
                    break;

                case (Country.FR):
                case (Country.UK):
                    break;

                case (Country.ITA):
                    break;
            }
        }

        return oilTempTargets;
    }

    internal static List<Quaternion> CylinderHeadTargets(AirplaneData airplaneData, Country country)
    {
        List<Quaternion> targets = new List<Quaternion>(new Quaternion[airplaneData.planeAttributes.engines]);

        for (int i = 0; i < airplaneData.planeAttributes.engines; i++)
        {
            switch (country)
            {
                //RU
                case (Country.RU):
                    if (airplaneData.planeAttributes.cylinderHeadType == DialVariant.A)
                    {
                        targets[i] = RussianDials.CylinderHeadTempTargetA(airplaneData.cylinderHeadTemps[i], airplaneData.scalar0, airplaneData.scalar1);
                    }

                    break;

                //GER
                case (Country.GER):
                    break;
                //US
                case (Country.US):
                    if (airplaneData.planeAttributes.cylinderHeadType == DialVariant.A)
                    {
                        targets[i] = USDials.CylinderHeadTempTargetA(airplaneData.cylinderHeadTemps[i], airplaneData.scalar0, airplaneData.scalar1);
                    }
                    else if (airplaneData.planeAttributes.cylinderHeadType == DialVariant.B) // A20 double needle dial
                    {
                        targets[0] = USDials.CylinderHeadTempTargetB(airplaneData.cylinderHeadTemps[0], airplaneData.scalar0, airplaneData.scalar1, true);
                        targets[1] = USDials.CylinderHeadTempTargetB(airplaneData.cylinderHeadTemps[1], airplaneData.scalar0, airplaneData.scalar1, false);
                    }
                    else if (airplaneData.planeAttributes.cylinderHeadType == DialVariant.C)
                    {
                        //C47
                        targets[0] = USDials.CylinderHeadTempTargetC(airplaneData.cylinderHeadTemps[0], airplaneData.scalar0, airplaneData.scalar1, true);
                        targets[1] = USDials.CylinderHeadTempTargetC(airplaneData.cylinderHeadTemps[1], airplaneData.scalar0, airplaneData.scalar1, false);
                    }
                    break;

                case (Country.FR):
                case (Country.UK):
                    break;

                case (Country.ITA):
                    break;
            }
        }

        return targets;
    }

    internal static List<Quaternion> CarbAirTargets(AirplaneData airplaneData, Country country)
    {
        List<Quaternion> targets = new List<Quaternion>(new Quaternion[airplaneData.planeAttributes.engines]);

        for (int i = 0; i < airplaneData.planeAttributes.engines; i++)
        {
            switch (country)
            {
                //RU
                case (Country.RU):
                    break;

                //GER
                case (Country.GER):
                    break;
                //US
                case (Country.US):
                    if (airplaneData.planeAttributes.carbAirTempType == DialVariant.A
                        || airplaneData.planeAttributes.carbAirTempType == DialVariant.B
                            || airplaneData.planeAttributes.carbAirTempType == DialVariant.C)
                    {
                        targets[i] = USDials.CarbAirTargetA(airplaneData.carbAirTemps[i], airplaneData.scalar0, airplaneData.scalar1);
                    }
                    else if (airplaneData.planeAttributes.carbAirTempType == DialVariant.D) // A20 double needle dial
                    {
                        targets[0] = USDials.CarbAirTargetD(airplaneData.carbAirTemps[0], airplaneData.scalar0, airplaneData.scalar1, true);
                        targets[1] = USDials.CarbAirTargetD(airplaneData.carbAirTemps[1], airplaneData.scalar0, airplaneData.scalar1, false);
                    }
                    else if (airplaneData.planeAttributes.carbAirTempType == DialVariant.E) // p38/c47 double needle dial
                    {
                        targets[0] = USDials.CarbAirTargetE(airplaneData.carbAirTemps[0], airplaneData.scalar0, airplaneData.scalar1, true);
                        targets[1] = USDials.CarbAirTargetE(airplaneData.carbAirTemps[1], airplaneData.scalar0, airplaneData.scalar1, false);
                    }
                    else if (airplaneData.planeAttributes.carbAirTempType == DialVariant.F)
                    {
                        targets[i] = USDials.CarbAirTargetF(airplaneData.carbAirTemps[i], airplaneData.scalar0, airplaneData.scalar1);
                    }
                    break;

                case (Country.FR):
                case (Country.UK):
                    break;

                case (Country.ITA):
                    break;
            }
        }

        return targets;
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

            case (Country.FR):
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

        else if (country == Country.UK || country == Country.FR)
        {
            repeaterCompassTarget = UKDials.RepeaterCompassTarget(airplaneData.heading);

            //Mosquito unique dial
            repeaterCompassAlternateTarget = UKDials.RepeaterCompassAlternateTarget(airplaneData.heading);
        }

        return repeaterCompassTarget;
    }

    public static Quaternion VSITarget(AirplaneData airplaneData, Country country, RotateNeedle rN)
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


            case (Country.US):
                if (airplaneData.planeAttributes.vsiType == DialVariant.A)
                    vsiNeedleTarget = USDials.VerticalSpeedTarget(airplaneData.verticalSpeed, rN.animationCurveVSI);
                else
                    vsiNeedleTarget = USDials.VerticalSpeedTarget(airplaneData.verticalSpeed, rN.animationCurveVSIB);
                break;

            case (Country.FR):
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

            case (Country.FR):
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

    internal static Quaternion HeadingIndicatorBallTarget(AirplaneData airplaneData)
    {
        Quaternion target = Quaternion.identity;
        switch (airplaneData.planeAttributes.country)
        {
            case (Country.US):
                target = USDials.HeadingIndicatorBallTarget(airplaneData.turnCoordinatorBall, airplaneData.scalar0);
                break;

        }

        return target;
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

            case (Country.FR):
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

            case (Country.FR):
            case Country.UK:
                target = UKDials.MmhgTarget(unit);
                break;

            case Country.ITA:
                target = ITADials.MmhgTarget(unit);
                break;
        }

        return target;

    }

    public static Quaternion AirspeedTarget(Country country, AirplaneData airplaneData, RotateNeedle rN)
    {

        Quaternion airspeedTarget = Quaternion.identity;
        //each country has slightly different dials, we need to work out rotations individually for each
        switch (country)
        {
            case Country.RU:
                airspeedTarget = RussianDials.AirspeedTarget(airplaneData.airspeed);
                break;

            case Country.GER:
                airspeedTarget = GermanDials.AirspeedTarget(airplaneData.airspeed);
                break;

            case Country.US:
                if (airplaneData.planeAttributes.speedometerType == DialVariant.A || airplaneData.planeAttributes.speedometerType == DialVariant.B || airplaneData.planeAttributes.speedometerType == DialVariant.C)
                    airspeedTarget = USDials.AirspeedTarget700Scale(airplaneData.airspeed, rN.animationCurveSpeedometerA, airplaneData.scalar0);
                else if (airplaneData.planeAttributes.speedometerType == DialVariant.D)
                    airspeedTarget = USDials.AirspeedTargetA20(airplaneData.airspeed);
                else if (airplaneData.planeAttributes.speedometerType == DialVariant.E)
                    airspeedTarget = USDials.AirspeedTargetP40(airplaneData.airspeed, airplaneData.scalar0, airplaneData.scalar1);

                break;

            case (Country.FR):
            case Country.UK:
                airspeedTarget = UKDials.AirspeedTarget(airplaneData.airspeed);
                break;

            case Country.ITA:
                airspeedTarget = ITADials.AirspeedTarget(airplaneData.airspeed);
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

            case (Country.FR):
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

            case (Country.FR):
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

            case (Country.FR):
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
