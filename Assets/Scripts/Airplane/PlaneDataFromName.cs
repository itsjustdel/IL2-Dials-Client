public enum DialVariant
{
    A,
    B,
    C,
    D,
    E,
    F,
    None
}

//extending from airplanedata for Country enum
public class PlaneDataFromName : AirplaneData
{
    public class PlaneAttributes
    {
        public Country country;
        //public bool speedometer;
        public bool altimeter;
        public bool headingIndicator;
        public bool turnAndBank; // with slip
        public bool artificialHorizon; // 
        public bool turnCoordinator; // with ball and wobbly needle
        public bool vsiSmallest;
        public bool vsiSmall;
        public bool vsiLarge; //larger scale than standard
        public bool repeaterCompass;
        public bool repeaterCompassAlternate;
        public DialVariant speedometerType = DialVariant.A;
        public DialVariant rpmType = DialVariant.None;
        public DialVariant manifoldType = DialVariant.A;
        public DialVariant turnCoordinatorType = DialVariant.A;
        public DialVariant vsiType = DialVariant.A;
        public DialVariant horizonType = DialVariant.A;
        public DialVariant headingIndicatorType = DialVariant.A;
        public DialVariant waterTempType = DialVariant.A;
        public DialVariant oilTempInType = DialVariant.None;
        public DialVariant oilTempOutType = DialVariant.None;
        public DialVariant oilTempPressureType = DialVariant.None;
        public DialVariant oilTempComboType = DialVariant.None;
        public DialVariant cylinderHeadType = DialVariant.None;
        public DialVariant carbAirTempType = DialVariant.None;

        public int engines = 1; //default to 1

    }

    public static PlaneAttributes AttributesFromName(string name)
    {
        PlaneAttributes planeAttributes = new PlaneAttributes();
        ///RSE.RSE::CAeroplane::getPlaneType - 40 55                 - push rbp --much nicer list -- but don't use has CAeroplane in string + wrong format

        switch (name)
        {
            //RSE.RSE::CCockpitInstruments::simulation+1117 - 48 8D 15 DA7A1200     - lea rdx,[RSE.RSE::CAeroplane_LaGG_3_ser_29::`vftable'+210] { ("LaGG-3 ser.29") }
            case "LaGG-3 ser.29":
                planeAttributes.country = Country.RU;
                planeAttributes.altimeter = true;
                planeAttributes.turnCoordinator = true;
                planeAttributes.headingIndicator = true;
                planeAttributes.vsiLarge = true;
                planeAttributes.rpmType = DialVariant.A;
                planeAttributes.manifoldType = DialVariant.A;
                planeAttributes.waterTempType = DialVariant.A;
                planeAttributes.oilTempPressureType = DialVariant.A;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+3E0 - 48 8D 15 29E11200     - lea rdx,[RSE.RSE::CAeroplane_Bf_109_F4::`vftable'+310] { ("Bf 109 F-4") }
            case "Bf 109 F-4":
                planeAttributes.country = Country.GER;
                planeAttributes.altimeter = true;
                planeAttributes.turnCoordinator = true;
                planeAttributes.repeaterCompass = true;
                planeAttributes.rpmType = DialVariant.A;
                planeAttributes.manifoldType = DialVariant.B;
                planeAttributes.waterTempType = DialVariant.D;
                planeAttributes.oilTempInType = DialVariant.D;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+1995 - 48 8D 15 FC911700     - lea rdx,[RSE.RSE::CAeroplane_Il_2_m_41::`vftable'+1E8] { ("Il-2 mod.1941") }
            case "Il-2 mod.1941":
                planeAttributes.country = Country.RU;
                planeAttributes.altimeter = true;
                planeAttributes.headingIndicator = true;
                planeAttributes.turnCoordinator = true;
                planeAttributes.vsiSmall = true;
                planeAttributes.turnAndBank = true;
                planeAttributes.rpmType = DialVariant.A;
                planeAttributes.manifoldType = DialVariant.A;
                planeAttributes.waterTempType = DialVariant.A;
                planeAttributes.oilTempInType = DialVariant.A;
                planeAttributes.oilTempPressureType = DialVariant.A;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+19B0 - 48 8D 15 D9F61200     - lea rdx,[RSE.RSE::CAeroplane_Il_2_m_42::`vftable'+1E8] { ("Il-2 mod.1942") }
            case "Il-2 mod.1942":
                planeAttributes.country = Country.RU;
                planeAttributes.altimeter = true;
                planeAttributes.headingIndicator = true;
                planeAttributes.turnCoordinator = true;
                planeAttributes.vsiSmall = true;
                planeAttributes.turnAndBank = true;
                planeAttributes.rpmType = DialVariant.A;
                planeAttributes.manifoldType = DialVariant.A;
                planeAttributes.waterTempType = DialVariant.A;
                planeAttributes.oilTempInType = DialVariant.A;
                planeAttributes.oilTempPressureType = DialVariant.A;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+19CB - 48 8D 15 96ED0900     - lea rdx,[RSE.RSE::CAeroplane_Il_2_m_43::`vftable'+1E8] { ("Il-2 mod.1943") }
            case "Il-2 mod.1943":
                planeAttributes.country = Country.RU;
                planeAttributes.altimeter = true;
                planeAttributes.headingIndicator = true;
                planeAttributes.turnCoordinator = true;
                planeAttributes.vsiSmall = true;
                planeAttributes.turnAndBank = true;
                planeAttributes.rpmType = DialVariant.A;
                planeAttributes.manifoldType = DialVariant.A;
                planeAttributes.waterTempType = DialVariant.A;
                planeAttributes.oilTempInType = DialVariant.A;
                planeAttributes.oilTempPressureType = DialVariant.A;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+159 - 48 8D 15 78421300     - lea rdx,[RSE.RSE::CAeroplane_Ju_87_D_3::`vftable'+238] { ("Ju-87 D3") }
            case "Ju-87 D3":
                planeAttributes.country = Country.GER;
                planeAttributes.altimeter = true;
                planeAttributes.headingIndicator = true;
                planeAttributes.repeaterCompass = true;
                planeAttributes.turnCoordinator = true;
                planeAttributes.vsiSmall = true;
                planeAttributes.rpmType = DialVariant.A;
                planeAttributes.manifoldType = DialVariant.A;
                planeAttributes.waterTempType = DialVariant.A;
                planeAttributes.oilTempOutType = DialVariant.A;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+1E0 - 48 8D 15 89211400     - lea rdx,[RSE.RSE::CAeroplane_Yak_1_ser_69::`vftable'+1E8] { ("Yak-1 ser.69") }
            case "Yak-1 ser.69":
                planeAttributes.country = Country.RU;
                planeAttributes.altimeter = true;
                planeAttributes.headingIndicator = true;
                planeAttributes.turnCoordinator = true;
                planeAttributes.rpmType = DialVariant.B;
                planeAttributes.manifoldType = DialVariant.B;
                planeAttributes.waterTempType = DialVariant.B;
                planeAttributes.oilTempPressureType = DialVariant.A;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+3FB - 48 8D 15 D64E1400     - lea rdx,[RSE.RSE::CAeroplane_Bf_109_G2::`vftable'+310] { ("Bf 109 G-2") }
            case "Bf 109 G-2":
                planeAttributes.country = Country.GER;
                planeAttributes.repeaterCompass = true;
                planeAttributes.altimeter = true;
                planeAttributes.turnAndBank = true;
                planeAttributes.rpmType = DialVariant.A;
                planeAttributes.manifoldType = DialVariant.A;
                planeAttributes.waterTempType = DialVariant.D;
                planeAttributes.oilTempInType = DialVariant.D;
                break;

            //RSE.RSE::CCockpitInstruments::simulation + 1C5 - 48 8D 15 1C9B1300 - lea rdx,[RSE.RSE::CAeroplane_Pe_2_ser_87::`vftable'+1E8] { ("Pe-2 ser.87") }
            case "Pe-2 ser.87":
                planeAttributes.country = Country.RU;
                planeAttributes.vsiLarge = true;
                planeAttributes.headingIndicator = true;
                planeAttributes.turnCoordinator = true;
                planeAttributes.turnAndBank = true;
                planeAttributes.altimeter = true;
                planeAttributes.engines = 2;
                planeAttributes.rpmType = DialVariant.C;
                planeAttributes.manifoldType = DialVariant.C;
                planeAttributes.waterTempType = DialVariant.C;
                planeAttributes.oilTempOutType = DialVariant.B;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+216 - 48 8D 15 AB701400     - lea rdx,[RSE.RSE::CAeroplane_La_5_ser_8::`vftable'+290] { ("La-5 ser.8") }
            case "La-5 ser.8":
                planeAttributes.country = Country.RU;
                planeAttributes.altimeter = true;
                planeAttributes.headingIndicator = true;
                planeAttributes.turnCoordinator = true;
                planeAttributes.vsiLarge = true;
                planeAttributes.rpmType = DialVariant.A;
                planeAttributes.manifoldType = DialVariant.A;
                planeAttributes.waterTempType = DialVariant.None;
                planeAttributes.oilTempPressureType = DialVariant.A;
                planeAttributes.cylinderHeadType = DialVariant.A;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+482 - 48 8D 15 07AA1400     - lea rdx,[RSE.RSE::CAeroplane_FW_190_A3::`vftable'+2C0] { ("FW 190 A3") }
            case "FW 190 A3":
                planeAttributes.country = Country.GER;
                planeAttributes.altimeter = true;
                planeAttributes.turnCoordinator = true;
                planeAttributes.repeaterCompass = true;
                planeAttributes.rpmType = DialVariant.A;
                planeAttributes.manifoldType = DialVariant.A;
                planeAttributes.waterTempType = DialVariant.None;
                planeAttributes.oilTempOutType = DialVariant.E;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+174 - 48 8D 15 35CB1400     - lea rdx,[RSE.RSE::CAeroplane_He_111_H6::`vftable'+298] { ("He 111 H-6") }
            case "He 111 H-6":
                planeAttributes.country = Country.GER;
                planeAttributes.altimeter = true;
                planeAttributes.turnCoordinator = true;
                planeAttributes.repeaterCompass = true;
                planeAttributes.artificialHorizon = true;
                planeAttributes.vsiSmall = true;
                planeAttributes.headingIndicator = true;// needs new model for gyro combined?
                planeAttributes.engines = 2;
                planeAttributes.rpmType = DialVariant.B;
                planeAttributes.manifoldType = DialVariant.E;
                planeAttributes.waterTempType = DialVariant.B;
                planeAttributes.oilTempOutType = DialVariant.B;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+2199 - 48 8D 15 80B41400     - lea rdx,[RSE.RSE::CAeroplane_MC_202_s8::`vftable'+1E8] { ("MC 202 s8") }
            case "MC 202 s8":
                planeAttributes.country = Country.ITA;
                planeAttributes.altimeter = true;
                planeAttributes.turnCoordinator = true;
                planeAttributes.artificialHorizon = true;
                planeAttributes.vsiLarge = true; //large for fighters
                planeAttributes.headingIndicator = true;
                planeAttributes.rpmType = DialVariant.A;
                planeAttributes.manifoldType = DialVariant.A;
                planeAttributes.waterTempType = DialVariant.A;
                planeAttributes.oilTempOutType = DialVariant.A;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+CA2 - 48 8D 15 6FFD1400     - lea rdx,[RSE.RSE::CAeroplane_Ju523mg4e::`vftable'+1E8] { ("Ju-52/3m g4e") }
            case "Ju-52/3m g4e":
                planeAttributes.country = Country.GER;
                planeAttributes.altimeter = true;
                planeAttributes.turnCoordinator = true;
                planeAttributes.artificialHorizon = true;
                planeAttributes.vsiSmall = true;//actually smallest (needs modelled)
                planeAttributes.repeaterCompassAlternate = true;
                planeAttributes.rpmType = DialVariant.A;
                planeAttributes.manifoldType = DialVariant.None;
                planeAttributes.engines = 3;
                planeAttributes.waterTempType = DialVariant.None;
                planeAttributes.oilTempInType = DialVariant.B; //needs its own dial
                planeAttributes.oilTempOutType = DialVariant.B; //needs its own dial
                break;

            //RSE.RSE::CCockpitInstruments::simulation+2580 - 48 8D 15 416A1500     - lea rdx,[RSE.RSE::CAeroplane_I_16_t_24::`vftable'+1C0] { ("I-16 type 24") }
            case "I-16 type 24":
                planeAttributes.country = Country.RU;
                planeAttributes.altimeter = true;
                planeAttributes.headingIndicator = true;
                planeAttributes.turnCoordinator = true;
                planeAttributes.vsiLarge = true;
                planeAttributes.rpmType = DialVariant.B;
                planeAttributes.manifoldType = DialVariant.A;
                planeAttributes.waterTempType = DialVariant.None;
                planeAttributes.oilTempPressureType = DialVariant.A;
                planeAttributes.cylinderHeadType = DialVariant.A;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+467 - 48 8D 15 721C1600     - lea rdx,[RSE.RSE::CAeroplane_Bf_109_E7::`vftable'+1C0] { ("Bf 109 E-7") }
            case "Bf 109 E-7":
                planeAttributes.country = Country.GER;
                planeAttributes.altimeter = true;
                planeAttributes.headingIndicator = true;
                planeAttributes.turnCoordinator = true;
                planeAttributes.rpmType = DialVariant.A;
                planeAttributes.manifoldType = DialVariant.A;
                planeAttributes.waterTempType = DialVariant.A;
                planeAttributes.oilTempOutType = DialVariant.A;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+5265 - 48 8D 15 84231600     - lea rdx,[RSE.RSE::CAeroplane_Bf_110_E2::`vftable'+400] { ("Bf-110 E2") }
            case "Bf-110 E2":
                planeAttributes.country = Country.GER;
                planeAttributes.altimeter = true;
                planeAttributes.headingIndicator = true;
                planeAttributes.repeaterCompass = true;
                planeAttributes.turnCoordinator = true;
                planeAttributes.vsiSmall = true;
                planeAttributes.artificialHorizon = true;
                planeAttributes.engines = 2;
                planeAttributes.rpmType = DialVariant.C;
                planeAttributes.manifoldType = DialVariant.B;
                planeAttributes.waterTempType = DialVariant.B;
                planeAttributes.oilTempComboType = DialVariant.A;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+BFB - 4C 8D 05 56E31500     - lea r8,[RSE.RSE::CFlywheelStarter_P_40E_1::`vftable'+68] { ("P-40E-1") }
            case "P-40E-1":
                planeAttributes.country = Country.US;
                planeAttributes.speedometerType = DialVariant.E;
                planeAttributes.altimeter = true;
                planeAttributes.headingIndicator = true;
                planeAttributes.artificialHorizon = true;
                planeAttributes.vsiLarge = true;//fighter
                planeAttributes.turnCoordinator = true;
                planeAttributes.rpmType = DialVariant.B;
                planeAttributes.manifoldType = DialVariant.B;
                planeAttributes.turnCoordinatorType = DialVariant.B;
                planeAttributes.vsiType = DialVariant.A;
                planeAttributes.horizonType = DialVariant.A;
                planeAttributes.waterTempType = DialVariant.A;
                planeAttributes.oilTempPressureType = DialVariant.A;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+C51 - 48 8D 15 88A51600     - lea rdx,[RSE.RSE::CAeroplane_MiG_3_ser_24::`vftable'+1C0] { ("MiG-3 ser.24") }
            case "MiG-3 ser.24":
                planeAttributes.country = Country.RU;
                planeAttributes.altimeter = true;
                planeAttributes.headingIndicator = true;
                planeAttributes.turnCoordinator = true;
                planeAttributes.turnAndBank = true;
                planeAttributes.vsiLarge = true;
                planeAttributes.rpmType = DialVariant.B;
                planeAttributes.manifoldType = DialVariant.A;
                planeAttributes.waterTempType = DialVariant.A;
                planeAttributes.oilTempInType = DialVariant.A;
                planeAttributes.oilTempPressureType = DialVariant.A;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+3C5 - 48 8D 15 94131700     - lea rdx,[RSE.RSE::CAeroplane_Bf_109_F2::`vftable'+310] { ("Bf 109 F-2") }
            case "Bf 109 F-2":
                planeAttributes.country = Country.GER;
                planeAttributes.altimeter = true;
                planeAttributes.repeaterCompass = true;
                planeAttributes.turnCoordinator = true;
                planeAttributes.rpmType = DialVariant.A;
                planeAttributes.manifoldType = DialVariant.A;
                planeAttributes.waterTempType = DialVariant.D;
                planeAttributes.oilTempInType = DialVariant.D;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+C87 - 48 8D 15 22131700     - lea rdx,[RSE.RSE::CAeroplane_Ju_88_A4::`vftable'+238] { ("Ju-88 A4") }
            case "Ju-88 A4":
                planeAttributes.country = Country.GER;
                planeAttributes.altimeter = true;
                planeAttributes.vsiSmall = true;
                planeAttributes.turnCoordinator = true;
                planeAttributes.artificialHorizon = true;
                planeAttributes.repeaterCompass = true;
                planeAttributes.engines = 2;
                planeAttributes.rpmType = DialVariant.B;
                planeAttributes.manifoldType = DialVariant.B;
                planeAttributes.waterTempType = DialVariant.B;
                planeAttributes.oilTempOutType = DialVariant.F;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+1AA - 48 8D 15 F7B01700     - lea rdx,[RSE.RSE::CAeroplane_Pe_2_ser_35::`vftable'+1E8] { ("Pe-2 ser.35") }
            case "Pe-2 ser.35":
                planeAttributes.country = Country.RU;
                planeAttributes.altimeter = true;
                planeAttributes.turnAndBank = true;
                planeAttributes.vsiLarge = true;
                planeAttributes.turnCoordinator = true;
                planeAttributes.headingIndicator = true;
                planeAttributes.engines = 2;
                planeAttributes.rpmType = DialVariant.C;
                planeAttributes.manifoldType = DialVariant.C;
                planeAttributes.waterTempType = DialVariant.A;
                planeAttributes.oilTempOutType = DialVariant.A;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+1FB - 48 8D 15 5E751000     - lea rdx,[RSE.RSE::CAeroplane_Yak_1_ser_127::`vftable'+1E8] { ("Yak-1 ser.127") }
            case "Yak-1 ser.127":
                planeAttributes.country = Country.RU;
                planeAttributes.altimeter = true;
                planeAttributes.turnCoordinator = true;
                planeAttributes.headingIndicator = true;
                planeAttributes.rpmType = DialVariant.B;
                planeAttributes.manifoldType = DialVariant.B;
                planeAttributes.waterTempType = DialVariant.B;
                planeAttributes.oilTempPressureType = DialVariant.A;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+416 - 48 8D 15 93BD0400     - lea rdx,[RSE.RSE::CAeroplane_Bf_109_G4::`vftable'+310] { ("Bf 109 G-4") }
            case "Bf 109 G-4":
                planeAttributes.country = Country.GER;
                planeAttributes.altimeter = true;
                planeAttributes.turnAndBank = true;
                planeAttributes.repeaterCompass = true;
                planeAttributes.rpmType = DialVariant.A;
                planeAttributes.manifoldType = DialVariant.A;
                planeAttributes.waterTempType = DialVariant.D;
                planeAttributes.oilTempInType = DialVariant.D;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+24C - 48 8D 15 3D5B0700     - lea rdx,[RSE.RSE::CAeroplane_FW_190_A5::`vftable'+2C0] { ("FW 190 A5") }
            case "FW 190 A5":
                planeAttributes.country = Country.GER;
                planeAttributes.altimeter = true;
                planeAttributes.turnAndBank = true;
                planeAttributes.repeaterCompass = true;
                planeAttributes.rpmType = DialVariant.A;
                planeAttributes.manifoldType = DialVariant.A;
                planeAttributes.waterTempType = DialVariant.None;
                planeAttributes.oilTempOutType = DialVariant.E;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+5289 - 48 8D 15 08250500     - lea rdx,[RSE.RSE::CAeroplane_Bf_110_G2::`vftable'+400] { ("Bf-110 G2") }
            case "Bf-110 G2":
                planeAttributes.country = Country.GER;
                planeAttributes.altimeter = true;
                planeAttributes.turnAndBank = true;
                planeAttributes.vsiSmall = true;
                planeAttributes.turnCoordinator = true;
                planeAttributes.headingIndicator = true;
                planeAttributes.repeaterCompass = true;
                planeAttributes.engines = 2;
                planeAttributes.rpmType = DialVariant.C;
                planeAttributes.manifoldType = DialVariant.B;
                planeAttributes.waterTempType = DialVariant.B;
                planeAttributes.oilTempComboType = DialVariant.A;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+217E - 48 8D 15 334D0800     - lea rdx,[RSE.RSE::CAeroplane_He_111_H16::`vftable'+298] { ("He 111 H-16") }
            case "He 111 H-16":
                planeAttributes.country = Country.GER;
                planeAttributes.altimeter = true;
                planeAttributes.turnAndBank = true;
                planeAttributes.turnCoordinator = true;
                planeAttributes.vsiSmall = true;
                planeAttributes.headingIndicator = true;
                planeAttributes.repeaterCompass = true;
                planeAttributes.engines = 2;
                planeAttributes.rpmType = DialVariant.B;
                planeAttributes.manifoldType = DialVariant.F;
                planeAttributes.waterTempType = DialVariant.B;
                planeAttributes.oilTempOutType = DialVariant.B;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+CBD - 48 8D 15 7C330F00     - lea rdx,[RSE.RSE::CAeroplane_Spitfire_Mk_Vb::`vftable'+210] { ("Spitfire Mk.Vb") }
            case "Spitfire Mk.Vb":
                planeAttributes.country = Country.UK;
                planeAttributes.altimeter = true;
                planeAttributes.artificialHorizon = true;
                planeAttributes.headingIndicator = true;
                planeAttributes.vsiLarge = true;
                planeAttributes.turnCoordinator = true;
                planeAttributes.repeaterCompass = true;
                planeAttributes.rpmType = DialVariant.A;
                planeAttributes.manifoldType = DialVariant.A;
                planeAttributes.waterTempType = DialVariant.A;
                planeAttributes.oilTempOutType = DialVariant.A;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+CD8 - 48 8D 15 D9EC0800     - lea rdx,[RSE.RSE::CAeroplane_Hs_129_B2::`vftable'+2E8] { ("Hs 129 B-2") }
            case "Hs 129 B-2":
                planeAttributes.country = Country.GER;
                planeAttributes.altimeter = true;
                planeAttributes.repeaterCompass = true;
                planeAttributes.turnCoordinator = true;
                planeAttributes.vsiSmall = true;
                planeAttributes.engines = 2;
                planeAttributes.rpmType = DialVariant.A;
                planeAttributes.manifoldType = DialVariant.F;
                planeAttributes.waterTempType = DialVariant.None;
                planeAttributes.oilTempOutType = DialVariant.F;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+CFC - 4C 8D 05 9D5E0300     - lea r8,[RSE.RSE::CAeroplane_A_20_B::`vftable'+2E0] { ("A-20B") }
            case "A-20B":
                planeAttributes.country = Country.US;
                planeAttributes.altimeter = true;
                planeAttributes.speedometerType = DialVariant.D;
                planeAttributes.headingIndicator = true;
                planeAttributes.turnCoordinator = true;
                planeAttributes.artificialHorizon = true;
                planeAttributes.vsiLarge = true;
                planeAttributes.engines = 2;
                planeAttributes.rpmType = DialVariant.A;
                planeAttributes.manifoldType = DialVariant.A;
                planeAttributes.turnCoordinatorType = DialVariant.B;
                planeAttributes.vsiType = DialVariant.A;
                planeAttributes.horizonType = DialVariant.A;
                planeAttributes.headingIndicatorType = DialVariant.B;
                planeAttributes.waterTempType = DialVariant.None; // engine is air cooled
                planeAttributes.oilTempOutType = DialVariant.B;
                planeAttributes.cylinderHeadType = DialVariant.B;
                planeAttributes.carbAirTempType = DialVariant.D;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+D83 - 4C 8D 05 6EAF0C00     - lea r8,[RSE.RSE::CAeroplane_P_39L_1::`vftable'+210] { ("P-39L-1") }
            case "P-39L-1":
                planeAttributes.country = Country.US;
                planeAttributes.speedometerType = DialVariant.C;
                planeAttributes.altimeter = true;
                planeAttributes.headingIndicator = true;
                planeAttributes.turnCoordinator = true;
                planeAttributes.artificialHorizon = true;
                planeAttributes.vsiLarge = true;
                planeAttributes.repeaterCompass = true;
                planeAttributes.rpmType = DialVariant.A;
                planeAttributes.manifoldType = DialVariant.C;
                planeAttributes.turnCoordinatorType = DialVariant.A;
                planeAttributes.vsiType = DialVariant.A;
                planeAttributes.horizonType = DialVariant.B;
                planeAttributes.waterTempType = DialVariant.A;
                planeAttributes.oilTempPressureType = DialVariant.A;
                planeAttributes.carbAirTempType = DialVariant.F;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+D29 - 48 8D 15 C0DE1000     - lea rdx,[RSE.RSE::CAeroplane_Yak_7B_ser_36::`vftable'+2C0] { ("Yak-7B ser.36") }
            case "Yak-7B ser.36":
                planeAttributes.country = Country.RU;
                planeAttributes.altimeter = true;
                planeAttributes.headingIndicator = true;
                planeAttributes.turnCoordinator = true;
                planeAttributes.rpmType = DialVariant.A;
                planeAttributes.manifoldType = DialVariant.B;
                planeAttributes.waterTempType = DialVariant.B;
                planeAttributes.oilTempPressureType = DialVariant.A;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+231 - 48 8D 15 78770A00     - lea rdx,[RSE.RSE::CAeroplane_La_5_FN_ser_2::`vftable'+2B8] { ("La-5 FN ser.2") }
            case "La-5 FN ser.2":
                planeAttributes.country = Country.RU;
                planeAttributes.altimeter = true;
                planeAttributes.headingIndicator = true;
                planeAttributes.turnCoordinator = true;
                planeAttributes.vsiLarge = true;
                planeAttributes.rpmType = DialVariant.A;
                planeAttributes.manifoldType = DialVariant.A;
                planeAttributes.waterTempType = DialVariant.None;
                planeAttributes.oilTempPressureType = DialVariant.A; //inbound accroding to requiem (let's ignore that for now - I don't have the plane atm)
                planeAttributes.cylinderHeadType = DialVariant.A;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+2A8 - 48 8D 15 A1FB0400     - lea rdx,[RSE.RSE::CAeroplane_Bf_109_G6::`vftable'+310] { ("Bf 109 G-6") }
            case "Bf 109 G-6":
                planeAttributes.country = Country.GER;
                planeAttributes.altimeter = true;
                planeAttributes.turnAndBank = true;
                planeAttributes.repeaterCompass = true;
                planeAttributes.rpmType = DialVariant.A;
                planeAttributes.manifoldType = DialVariant.B;
                planeAttributes.waterTempType = DialVariant.D;
                planeAttributes.oilTempInType = DialVariant.None; //combo   
                break;

            //RSE.RSE::CCockpitInstruments::simulation+2BF - 48 8D 15 72360400     - lea rdx,[RSE.RSE::CAeroplane_Bf_109_G14::`vftable'+310] { ("Bf 109 G-14") }
            case "Bf 109 G-14":
                planeAttributes.country = Country.GER;
                planeAttributes.altimeter = true;
                planeAttributes.turnAndBank = true;
                planeAttributes.repeaterCompass = true;
                planeAttributes.rpmType = DialVariant.A;
                planeAttributes.manifoldType = DialVariant.B;
                planeAttributes.waterTempType = DialVariant.A;
                planeAttributes.oilTempInType = DialVariant.None;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+DA9 - 48 8D 15 58B90E00     - lea rdx,[RSE.RSE::CAeroplane_Spitfire_Mk_IXe::`vftable'+2E8] { ("Spitfire Mk.IXe") }
            case "Spitfire Mk.IXe":
                planeAttributes.country = Country.UK;
                planeAttributes.altimeter = true;
                planeAttributes.artificialHorizon = true;
                planeAttributes.headingIndicator = true;
                planeAttributes.vsiLarge = true;
                planeAttributes.turnCoordinator = true;
                planeAttributes.repeaterCompass = true;
                planeAttributes.rpmType = DialVariant.B;
                planeAttributes.manifoldType = DialVariant.B;
                planeAttributes.waterTempType = DialVariant.A;
                planeAttributes.oilTempOutType = DialVariant.A;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+27A - 48 8D 15 FFF10700     - lea rdx,[RSE.RSE::CAeroplane_FW_190_A8::`vftable'+2C0] { ("FW 190 A8") }
            case "FW 190 A8":
                planeAttributes.country = Country.GER;
                planeAttributes.altimeter = true;
                planeAttributes.turnAndBank = true;
                planeAttributes.repeaterCompass = true;
                planeAttributes.vsiSmall = true;
                planeAttributes.rpmType = DialVariant.A;
                planeAttributes.manifoldType = DialVariant.A;
                planeAttributes.waterTempType = DialVariant.None;
                planeAttributes.oilTempOutType = DialVariant.E;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+E2F - 48 8D 15 62B70D00     - lea rdx,[RSE.RSE::CGearPost_P_47D_28::`vftable'+68] { ("P-47D-28") }
            case "P-47D-28":
                planeAttributes.country = Country.US;
                planeAttributes.speedometerType = DialVariant.B;
                planeAttributes.headingIndicator = true;
                planeAttributes.altimeter = true;
                planeAttributes.turnCoordinator = true;
                planeAttributes.artificialHorizon = true;
                planeAttributes.vsiLarge = true;
                planeAttributes.rpmType = DialVariant.D;
                planeAttributes.manifoldType = DialVariant.D;
                planeAttributes.turnCoordinatorType = DialVariant.B;
                planeAttributes.vsiType = DialVariant.A;
                planeAttributes.horizonType = DialVariant.C;
                planeAttributes.waterTempType = DialVariant.None; // air cooled
                planeAttributes.oilTempPressureType = DialVariant.A;
                planeAttributes.cylinderHeadType = DialVariant.A;
                planeAttributes.carbAirTempType = DialVariant.B;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+E65 - 48 8D 15 044A0500     - lea rdx,[RSE.RSE::CAeroplane_Bf_109_K4::`vftable'+310] { ("Bf 109 K-4") }
            case "Bf 109 K-4":
                planeAttributes.country = Country.GER;
                planeAttributes.altimeter = true;
                planeAttributes.turnAndBank = true;
                planeAttributes.repeaterCompass = true;
                planeAttributes.vsiLarge = true;
                planeAttributes.rpmType = DialVariant.A;
                planeAttributes.manifoldType = DialVariant.C;
                planeAttributes.waterTempType = DialVariant.C;
                planeAttributes.oilTempInType = DialVariant.C; // Think the game got this wrong, dial is 0-130 range but data is found on the "inbound" struct
                break;

            //RSE.RSE::CCockpitInstruments::simulation+E4A - 48 8D 15 6F600E00     - lea rdx,[RSE.RSE::CAeroplane_P_51D_15::`vftable'+220] { ("P-51D-15") }
            case "P-51D-15":
                planeAttributes.country = Country.US;
                planeAttributes.speedometerType = DialVariant.A;
                planeAttributes.altimeter = true;
                planeAttributes.repeaterCompass = true;
                planeAttributes.headingIndicator = true;
                planeAttributes.artificialHorizon = true;
                planeAttributes.vsiLarge = true;
                planeAttributes.turnCoordinator = true;
                planeAttributes.rpmType = DialVariant.C;
                planeAttributes.manifoldType = DialVariant.E;
                planeAttributes.turnCoordinatorType = DialVariant.C;
                planeAttributes.vsiType = DialVariant.B;
                planeAttributes.horizonType = DialVariant.C;
                planeAttributes.waterTempType = DialVariant.C;
                planeAttributes.oilTempPressureType = DialVariant.A;
                planeAttributes.carbAirTempType = DialVariant.C;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+5778 - 48 8D 15 09890A00     - lea rdx,[RSE.RSE::CAeroplane_Me_262_A::`vftable'+260] { ("Me 262 A") }
            case "Me 262 A":
                planeAttributes.country = Country.GER;
                planeAttributes.altimeter = true;
                planeAttributes.turnAndBank = true;
                planeAttributes.repeaterCompass = true;
                planeAttributes.vsiLarge = true;
                planeAttributes.engines = 2;
                planeAttributes.rpmType = DialVariant.D;
                planeAttributes.manifoldType = DialVariant.None;
                planeAttributes.waterTempType = DialVariant.None;
                planeAttributes.oilTempInType = DialVariant.None;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+E80 - 48 8D 15 C1F80700     - lea rdx,[RSE.RSE::CAeroplane_FW_190_D9::`vftable'+430] { ("FW 190 D9") }
            case "FW 190 D9":
                planeAttributes.country = Country.GER;
                planeAttributes.altimeter = true;
                planeAttributes.turnAndBank = true;
                planeAttributes.repeaterCompass = true;
                planeAttributes.vsiLarge = true;
                planeAttributes.rpmType = DialVariant.A;
                planeAttributes.manifoldType = DialVariant.D;
                planeAttributes.waterTempType = DialVariant.C;
                planeAttributes.oilTempOutType = DialVariant.C;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+DF9 - 48 8D 15 981B0C00     - lea rdx,[RSE.RSE::CAeroplane_P_38J_25::`vftable'+288] { ("P-38J-25") }
            case "P-38J-25":
                planeAttributes.country = Country.US;
                planeAttributes.altimeter = true;
                planeAttributes.artificialHorizon = true;
                planeAttributes.repeaterCompass = true;
                planeAttributes.vsiLarge = true;
                planeAttributes.turnCoordinator = true;
                planeAttributes.headingIndicator = true;
                planeAttributes.engines = 2;
                planeAttributes.rpmType = DialVariant.E;
                planeAttributes.manifoldType = DialVariant.F;
                planeAttributes.turnCoordinatorType = DialVariant.C;
                planeAttributes.vsiType = DialVariant.B;
                planeAttributes.horizonType = DialVariant.C;
                planeAttributes.waterTempType = DialVariant.B;
                planeAttributes.oilTempPressureType = DialVariant.A;
                planeAttributes.carbAirTempType = DialVariant.E;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+E9B - 48 8D 15 56E10F00     - lea rdx,[RSE.RSE::CAeroplane_Tempest_Mk_V_ser_2::`vftable'+2E8] { ("Tempest Mk.V ser.2") }
            case "Tempest Mk.V ser.2":
                planeAttributes.country = Country.UK;
                planeAttributes.altimeter = true;
                planeAttributes.artificialHorizon = true;
                planeAttributes.vsiLarge = true;
                planeAttributes.turnCoordinator = true;
                planeAttributes.headingIndicator = true;
                planeAttributes.repeaterCompass = true;
                planeAttributes.rpmType = DialVariant.B;
                planeAttributes.manifoldType = DialVariant.B;
                planeAttributes.waterTempType = DialVariant.A;
                planeAttributes.oilTempInType = DialVariant.A;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+EBF - 4C 8D 05 8A0C0600     - lea r8,[RSE.RSE::CAeroplane_B_25_D::`vftable'+2B8] { ("B-25D") }
            case "B-25D"://non player?
                planeAttributes.country = Country.US;
                planeAttributes.altimeter = true;
                planeAttributes.artificialHorizon = true;
                planeAttributes.repeaterCompass = true;
                planeAttributes.vsiLarge = true;
                planeAttributes.turnCoordinator = true;
                planeAttributes.headingIndicator = true;
                planeAttributes.rpmType = DialVariant.A;
                planeAttributes.manifoldType = DialVariant.A;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+D44 - 48 8D 15 F5D01100     - lea rdx,[RSE.RSE::CAeroplane_Yak_9_ser_1::`vftable'+2E8] { ("Yak-9 ser.1") }
            case "Yak-9 ser.1":
                planeAttributes.country = Country.RU;
                planeAttributes.altimeter = true;
                planeAttributes.headingIndicator = true;
                planeAttributes.turnCoordinator = true;
                planeAttributes.vsiLarge = true;
                planeAttributes.rpmType = DialVariant.A;
                planeAttributes.manifoldType = DialVariant.B;
                planeAttributes.waterTempType = DialVariant.B;
                planeAttributes.oilTempPressureType = DialVariant.A;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+D5F - 48 8D 15 AA681100     - lea rdx,[RSE.RSE::CAeroplane_Yak_9T_ser_1::`vftable'+2E8] { ("Yak-9T ser.1") }
            case "Yak-9T ser.1":
                planeAttributes.country = Country.RU;
                planeAttributes.altimeter = true;
                planeAttributes.headingIndicator = true;
                planeAttributes.turnCoordinator = true;
                planeAttributes.vsiLarge = true;
                planeAttributes.rpmType = DialVariant.A;
                planeAttributes.manifoldType = DialVariant.B;
                planeAttributes.waterTempType = DialVariant.B;
                planeAttributes.oilTempPressureType = DialVariant.A;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+E14 - 48 8D 15 ED210D00     - lea rdx,[RSE.RSE::CGearPost_P_47D_22::`vftable'+68] { ("P-47D-22") }
            case "P-47D-22":
                planeAttributes.country = Country.US;
                planeAttributes.speedometerType = DialVariant.B;
                planeAttributes.altimeter = true;
                planeAttributes.headingIndicator = true;
                planeAttributes.turnCoordinator = true;
                planeAttributes.vsiLarge = true;
                planeAttributes.artificialHorizon = true;
                planeAttributes.rpmType = DialVariant.D;
                planeAttributes.manifoldType = DialVariant.D;
                planeAttributes.turnCoordinatorType = DialVariant.B;
                planeAttributes.vsiType = DialVariant.A;
                planeAttributes.horizonType = DialVariant.B;
                planeAttributes.waterTempType = DialVariant.None; //air cooled
                planeAttributes.oilTempPressureType = DialVariant.A;
                planeAttributes.cylinderHeadType = DialVariant.A;
                planeAttributes.carbAirTempType = DialVariant.A;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+EE9 - 48 8D 15 C85E0900     - lea rdx,[RSE.RSE::CAeroplane_Hurricane_Mk_II::`vftable'+2E8] { ("Hurricane Mk.II") }
            case "Hurricane Mk.II":
                planeAttributes.country = Country.UK;
                planeAttributes.altimeter = true;
                planeAttributes.headingIndicator = true;
                planeAttributes.turnCoordinator = true;
                planeAttributes.vsiLarge = true;
                planeAttributes.artificialHorizon = true;
                planeAttributes.repeaterCompass = true;
                planeAttributes.rpmType = DialVariant.B;
                planeAttributes.manifoldType = DialVariant.B;
                planeAttributes.waterTempType = DialVariant.A;
                planeAttributes.oilTempInType = DialVariant.A;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+F0D - 4C 8D 05 ECA40600     - lea r8,[RSE.RSE::CAeroplane_C_47A::`vftable'+210] { ("C-47A") }
            case "C-47A":
                planeAttributes.country = Country.US;
                planeAttributes.engines = 2;
                planeAttributes.altimeter = true;
                planeAttributes.speedometerType = DialVariant.E;
                planeAttributes.horizonType = DialVariant.C;
                planeAttributes.repeaterCompass = true;
                planeAttributes.vsiType = DialVariant.B;
                planeAttributes.turnCoordinatorType = DialVariant.C;
                planeAttributes.headingIndicator = true;
                planeAttributes.rpmType = DialVariant.E;
                planeAttributes.manifoldType = DialVariant.F;
                planeAttributes.waterTempType = DialVariant.None;
                planeAttributes.oilTempOutType = DialVariant.A;
                planeAttributes.cylinderHeadType = DialVariant.C;
                planeAttributes.carbAirTempType = DialVariant.E;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+F3F - 4C 8D 05 F2D40D00     - lea r8,[RSE.RSE::CAeroplane_P_51B_5::`vftable'+210] { ("P-51B-5") }
            case "P-51B-5":
                planeAttributes.country = Country.US;
                planeAttributes.speedometerType = DialVariant.A;
                planeAttributes.altimeter = true;
                planeAttributes.artificialHorizon = true;
                planeAttributes.repeaterCompass = true;
                planeAttributes.vsiLarge = true;
                planeAttributes.turnCoordinator = true;
                planeAttributes.headingIndicator = true;
                planeAttributes.rpmType = DialVariant.C;
                planeAttributes.manifoldType = DialVariant.E;
                planeAttributes.turnCoordinatorType = DialVariant.A;
                planeAttributes.vsiType = DialVariant.B;
                planeAttributes.horizonType = DialVariant.B;
                planeAttributes.waterTempType = DialVariant.C;
                planeAttributes.oilTempPressureType = DialVariant.A;
                planeAttributes.carbAirTempType = DialVariant.C;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+F66 - 48 8D 15 530F0500     - lea rdx,[RSE.RSE::CAeroplane_Bf_109_G6_Late::`vftable'+310] { ("Bf 109 G-6 Late") }
            case "Bf 109 G-6 Late":
                planeAttributes.country = Country.GER;
                planeAttributes.altimeter = true;
                planeAttributes.turnAndBank = true;
                planeAttributes.repeaterCompass = true;
                planeAttributes.rpmType = DialVariant.A;
                planeAttributes.manifoldType = DialVariant.B;
                planeAttributes.waterTempType = DialVariant.D;
                planeAttributes.oilTempInType = DialVariant.D;
                break;

            //RSE.RSE::CCockpitInstruments::simulation + F7D - 48 8D 15 747B0F00 - lea rdx,[RSE.RSE::CAeroplane_Spitfire_Mk_XIV::`vftable'+2E8] { ("Spitfire Mk.XIV") }
            case "Spitfire Mk.XIV":
                planeAttributes.country = Country.UK;
                planeAttributes.altimeter = true;
                planeAttributes.artificialHorizon = true;
                planeAttributes.vsiLarge = true;
                planeAttributes.turnCoordinator = true;
                planeAttributes.headingIndicator = true;
                planeAttributes.repeaterCompass = true;
                planeAttributes.rpmType = DialVariant.B;
                planeAttributes.manifoldType = DialVariant.B;
                planeAttributes.waterTempType = DialVariant.B;
                planeAttributes.oilTempOutType = DialVariant.B;
                break;

            //pre-release
            case "Spitfire Mk.XIVe":
                planeAttributes.country = Country.UK;
                planeAttributes.altimeter = true;
                planeAttributes.artificialHorizon = true;
                planeAttributes.vsiLarge = true;
                planeAttributes.turnCoordinator = true;
                planeAttributes.headingIndicator = true;
                planeAttributes.repeaterCompass = true;
                planeAttributes.rpmType = DialVariant.B;
                planeAttributes.manifoldType = DialVariant.B;
                planeAttributes.waterTempType = DialVariant.B;
                planeAttributes.oilTempOutType = DialVariant.B;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+263 - 48 8D 15 0EC40700     - lea rdx,[RSE.RSE::CAeroplane_FW_190_A6::`vftable'+8] { ("FW 190 A6") }
            case "FW 190 A6":
                planeAttributes.country = Country.GER;
                planeAttributes.altimeter = true;
                planeAttributes.turnAndBank = true;
                planeAttributes.repeaterCompass = true;
                planeAttributes.rpmType = DialVariant.A;
                planeAttributes.manifoldType = DialVariant.A;
                planeAttributes.waterTempType = DialVariant.None;
                planeAttributes.oilTempOutType = DialVariant.E;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+589A - 48 8D 15 DF201000     - lea rdx,[RSE.RSE::CAeroplane_Typhoon_Mk_Ib::`vftable'+310] { ("Typhoon Mk.Ib") }
            case "Typhoon Mk.Ib":
                planeAttributes.country = Country.UK;
                planeAttributes.altimeter = true;
                planeAttributes.artificialHorizon = true;
                planeAttributes.vsiLarge = true;
                planeAttributes.turnCoordinator = true;
                planeAttributes.headingIndicator = true;
                planeAttributes.repeaterCompass = true;
                planeAttributes.rpmType = DialVariant.B;
                planeAttributes.manifoldType = DialVariant.C;
                planeAttributes.waterTempType = DialVariant.A;
                planeAttributes.oilTempInType = DialVariant.A;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+DDD - 4C 8D 05 6CB00C00     - lea r8,[RSE.RSE::CAeroplane_Po_2_VS::`vftable'+1D0] { ("U-2VS") }
            case "U-2VS":
                planeAttributes.country = Country.RU;
                planeAttributes.altimeter = true;
                planeAttributes.turnAndBank = true;
                planeAttributes.vsiSmall = true;
                planeAttributes.turnCoordinator = true;
                planeAttributes.headingIndicator = true;
                planeAttributes.turnAndBank = true;
                planeAttributes.rpmType = DialVariant.A;
                planeAttributes.manifoldType = DialVariant.None;
                planeAttributes.waterTempType = DialVariant.None;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+5A79 - 48 8D 15 50250400     - lea rdx,[RSE.RSE::CAeroplane_Ar_234_B2::`vftable'+238] { ("Ar 234 B-2") }
            case "Ar 234 B-2":
                planeAttributes.country = Country.GER;
                planeAttributes.altimeter = true;
                planeAttributes.turnAndBank = true;
                planeAttributes.vsiLarge = true;
                planeAttributes.repeaterCompass = true;
                planeAttributes.artificialHorizon = true;
                planeAttributes.headingIndicator = true;
                planeAttributes.turnAndBank = true;
                planeAttributes.engines = 2;
                planeAttributes.rpmType = DialVariant.D;
                planeAttributes.manifoldType = DialVariant.None;
                planeAttributes.waterTempType = DialVariant.None;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+5FD5 - 48 8D 15 645A0C00     - lea rdx,[RSE.RSE::CAeroplane_Ju_88_C6::`vftable'+238] { ("Ju-88 C6") }
            case "Ju-88 C6":
                planeAttributes.country = Country.GER;
                planeAttributes.altimeter = true;
                planeAttributes.turnCoordinator = true;
                planeAttributes.vsiSmall = true;
                planeAttributes.headingIndicator = true;
                planeAttributes.artificialHorizon = true;
                planeAttributes.repeaterCompass = true;
                planeAttributes.engines = 2;
                planeAttributes.rpmType = DialVariant.B;
                planeAttributes.manifoldType = DialVariant.E;
                planeAttributes.waterTempType = DialVariant.B;
                planeAttributes.oilTempOutType = DialVariant.F;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+574F - 48 8D 15 72AD0E00     - lea rdx,[RSE.RSE::CAeroplane_Mosquito_FB_Mk_VI_s2::`vftable'+2E8] { ("Mosquito F.B. Mk.VI ser.2") }
            case "Mosquito F.B. Mk.VI ser.2":
                planeAttributes.country = Country.UK;
                planeAttributes.altimeter = true;
                planeAttributes.artificialHorizon = true;
                planeAttributes.vsiLarge = true;
                planeAttributes.turnCoordinator = true;
                planeAttributes.headingIndicator = true;
                planeAttributes.repeaterCompass = true;
                planeAttributes.repeaterCompassAlternate = true;
                planeAttributes.engines = 2;
                planeAttributes.rpmType = DialVariant.C;
                planeAttributes.manifoldType = DialVariant.D;
                planeAttributes.waterTempType = DialVariant.A;
                planeAttributes.oilTempInType = DialVariant.A;
                break;


            //RSE.RSE::CCockpitInstruments::simulation+102D - 48 8D 15 246D0E00     - lea rdx,[RSE.RSE::CAeroplane_Me_410_A1::`vftable'+360] { ("Me 410 A-1") }
            case "Me 410 A-1":
                planeAttributes.country = Country.GER;
                planeAttributes.altimeter = true;
                planeAttributes.turnCoordinator = true;
                planeAttributes.vsiLarge = true;
                planeAttributes.headingIndicator = true;
                planeAttributes.artificialHorizon = true;
                planeAttributes.repeaterCompass = true;
                planeAttributes.engines = 2;
                planeAttributes.rpmType = DialVariant.E;
                planeAttributes.manifoldType = DialVariant.F;
                planeAttributes.waterTempType = DialVariant.A;
                planeAttributes.oilTempOutType = DialVariant.A;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+8E99 - 48 8D 15 A88B0600     - lea rdx,[RSE.RSE::CAeroplane_Bf_109_G6AS::`vftable'+238] { ("Bf 109 G-6AS") }
            case "Bf 109 G-6AS":
                planeAttributes.country = Country.GER;
                planeAttributes.altimeter = true;
                planeAttributes.turnAndBank = true;
                planeAttributes.repeaterCompass = true;
                planeAttributes.rpmType = DialVariant.A;
                planeAttributes.manifoldType = DialVariant.B;
                planeAttributes.waterTempType = DialVariant.D;
                planeAttributes.oilTempInType = DialVariant.D;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+AD76 - 48 8D 3D 3B610E00     - lea rdi,[RSE.RSE::CAeroplane_Li_2::`vftable'+210] { ("Li-2") }
            case "Li-2":
                planeAttributes.country = Country.RU;
                planeAttributes.engines = 2;
                planeAttributes.altimeter = true;
                planeAttributes.speedometerType = DialVariant.A;
                planeAttributes.horizonType = DialVariant.A;
                planeAttributes.repeaterCompass = true;
                planeAttributes.vsiType = DialVariant.A;
                planeAttributes.turnCoordinatorType = DialVariant.A;
                planeAttributes.headingIndicator = true;
                planeAttributes.rpmType = DialVariant.C;
                planeAttributes.manifoldType = DialVariant.A;
                planeAttributes.waterTempType = DialVariant.A;
                planeAttributes.oilTempInType = DialVariant.A;
                planeAttributes.cylinderHeadType = DialVariant.A;
                planeAttributes.carbAirTempType = DialVariant.A;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+637E - 48 8D 15 B3130D00     - lea rdx,[RSE.RSE::CAeroplane_IAR_80_A::`vftable'+2C8] { ("I.A.R. 80-A") }
            case "I.A.R. 80-A":
                planeAttributes.country = Country.GER;
                planeAttributes.altimeter = true;
                planeAttributes.speedometerType = DialVariant.A;
                planeAttributes.horizonType = DialVariant.A;
                planeAttributes.repeaterCompass = true;
                planeAttributes.turnCoordinator = true;
                planeAttributes.vsiType = DialVariant.A;
                planeAttributes.rpmType = DialVariant.A;
                //planeAttributes.manifoldType = DialVariant.A;  //need limits on server
                //planeAttributes.waterTempType = DialVariant.A;
                //planeAttributes.oilTempInType = DialVariant.A;
                break;

            case "I.A.R. 80-B": // same as above? same plane name check A or B
                planeAttributes.country = Country.GER;
                planeAttributes.altimeter = true;
                planeAttributes.speedometerType = DialVariant.A;
                planeAttributes.horizonType = DialVariant.A;
                planeAttributes.repeaterCompass = true;
                planeAttributes.turnCoordinator = true;
                planeAttributes.vsiType = DialVariant.A;
                planeAttributes.rpmType = DialVariant.A;
                //planeAttributes.manifoldType = DialVariant.A;  //need limits on server
                //planeAttributes.waterTempType = DialVariant.A;
                //planeAttributes.oilTempInType = DialVariant.A;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+10BB - 48 8D 15 0E471600     - lea rdx,[RSE.RSE::CAeroplane_Ta_152_H1::`vftable'+438] { ("Ta 152 H1") }            
            case "Ta 152 H1": // copy of Dora
                planeAttributes.country = Country.GER;
                planeAttributes.altimeter = true;
                planeAttributes.turnAndBank = true;
                planeAttributes.repeaterCompass = true;
                planeAttributes.vsiLarge = true;
                planeAttributes.rpmType = DialVariant.A;
                planeAttributes.manifoldType = DialVariant.D;
                planeAttributes.waterTempType = DialVariant.C;
                planeAttributes.oilTempOutType = DialVariant.C;
                break;

            //Flying Circus Vol 1
            //RSE.RSE::CCockpitInstruments::simulation+3DB5 - 48 8D 15 C4481F00     - lea rdx,[RSE.RSE::CAeroplane_SPAD_XIII_C1::`vftable'+1C8] { ("SPAD XIII.C1") }
            case "SPAD XIII.C1":
                planeAttributes.country = Country.FR;
                planeAttributes.speedometerType = DialVariant.A;
                planeAttributes.altimeter = true;
                planeAttributes.repeaterCompass = true;
                planeAttributes.rpmType = DialVariant.A;
                planeAttributes.manifoldType = DialVariant.None;
                planeAttributes.waterTempType = DialVariant.A;
                //oil pressure
                break;

            //RSE.RSE::CCockpitInstruments::simulation+3D86 - 48 8D 15 63D71B00     - lea rdx,[RSE.RSE::CAeroplane_Albatros_D_Va::`vftable'+1C8] { ("Albatros D.Va") }
            case "Albatros D.Va":
                planeAttributes.country = Country.GER;
                planeAttributes.speedometerType = DialVariant.A;
                planeAttributes.altimeter = true;
                planeAttributes.repeaterCompass = true;
                planeAttributes.rpmType = DialVariant.A;
                planeAttributes.manifoldType = DialVariant.None;
                planeAttributes.waterTempType = DialVariant.A;
                //fuel pressure
                break;

            //RSE.RSE::CCockpitInstruments::simulation+3DD0 - 48 8D 15 399A1E00     - lea rdx,[RSE.RSE::CAeroplane_Sopwith_Camel::`vftable'+1C8] { ("Sopwith Camel") }
            case "Sopwith Camel":
                planeAttributes.country = Country.UK;
                planeAttributes.speedometerType = DialVariant.A;
                planeAttributes.altimeter = true;
                planeAttributes.repeaterCompass = true;
                planeAttributes.rpmType = DialVariant.A;
                planeAttributes.manifoldType = DialVariant.None;
                planeAttributes.waterTempType = DialVariant.A;
                break;

            // Couldn't find!
            case "Sopwith Snipe":
                planeAttributes.country = Country.UK;
                planeAttributes.speedometerType = DialVariant.A;
                planeAttributes.altimeter = true;
                planeAttributes.repeaterCompass = true;
                planeAttributes.rpmType = DialVariant.A;
                planeAttributes.manifoldType = DialVariant.None;
                planeAttributes.waterTempType = DialVariant.A;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+3DEB - 48 8D 15 4E4E1E00     - lea rdx,[RSE.RSE::CAeroplane_Pfalz_D_IIIa::`vftable'+1C8] { ("Pfalz D.IIIa") }
            case "Pfalz D.IIIa":
                planeAttributes.country = Country.GER;
                planeAttributes.speedometerType = DialVariant.A;
                planeAttributes.altimeter = true;
                planeAttributes.repeaterCompass = true;
                planeAttributes.rpmType = DialVariant.A;
                planeAttributes.manifoldType = DialVariant.None;
                //art horizon
                planeAttributes.artificialHorizon = true;
                //fuel tank air pressure
                //mag compass
                //engine temp
                break;

            //RSE.RSE::CCockpitInstruments::simulation+3E06 - 48 8D 15 433F1D00     - lea rdx,[RSE.RSE::CAeroplane_Fokker_D_VII::`vftable'+1C8] { ("Fokker D.VII") }
            case "Fokker D.VII":
                planeAttributes.country = Country.GER;
                planeAttributes.speedometerType = DialVariant.A;
                planeAttributes.altimeter = true;
                planeAttributes.repeaterCompass = true;
                planeAttributes.rpmType = DialVariant.A;
                planeAttributes.manifoldType = DialVariant.None;
                planeAttributes.artificialHorizon = true;
                //main tank fuel pressure
                //art horizon
                //mag compass
                break;

            //RSE.RSE::CCockpitInstruments::simulation+3E21 - 48 8D 15 18F81C00     - lea rdx,[RSE.RSE::CAeroplane_Fokker_D_VIIF::`vftable'+1C8] { ("Fokker D.VIIF") }
            case "Fokker D.VIIF":
                planeAttributes.country = Country.GER;
                planeAttributes.speedometerType = DialVariant.A;
                planeAttributes.altimeter = true;
                planeAttributes.repeaterCompass = true;
                planeAttributes.rpmType = DialVariant.A;
                planeAttributes.manifoldType = DialVariant.None;
                planeAttributes.artificialHorizon = true;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+3E3C - 48 8D 15 DD811C00     - lea rdx,[RSE.RSE::CAeroplane_Bristol_F_2B_Falcon_II::`vftable'+1C8] { ("Bristol F.2B Falcon II") }
            case "Bristol F.2B Falcon II":
                planeAttributes.country = Country.UK;
                planeAttributes.speedometerType = DialVariant.A;
                planeAttributes.altimeter = true;
                planeAttributes.repeaterCompass = true;
                planeAttributes.rpmType = DialVariant.A;
                planeAttributes.manifoldType = DialVariant.None;
                planeAttributes.artificialHorizon = true;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+3E57 - 48 8D 15 724A1C00     - lea rdx,[RSE.RSE::CAeroplane_Bristol_F_2B_Falcon_III::`vftable'+1C8] { ("Bristol F.2B Falcon III") }
            case "Bristol F.2B Falcon III":
                planeAttributes.country = Country.UK;
                planeAttributes.speedometerType = DialVariant.A;
                planeAttributes.altimeter = true;
                planeAttributes.repeaterCompass = true;
                planeAttributes.rpmType = DialVariant.A;
                planeAttributes.manifoldType = DialVariant.None;
                planeAttributes.artificialHorizon = true;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+3E6E - 48 8D 15 031E1D00     - lea rdx,[RSE.RSE::CAeroplane_Fokker_D_VIII::`vftable'+1A0] { ("Fokker D.VIII") }
            case "Fokker D.VIII":
                planeAttributes.country = Country.GER;
                planeAttributes.speedometerType = DialVariant.A;
                planeAttributes.altimeter = true;
                planeAttributes.repeaterCompass = true;
                planeAttributes.rpmType = DialVariant.A;
                planeAttributes.manifoldType = DialVariant.None;
                planeAttributes.artificialHorizon = true;
                break;


            //RSE.RSE::CCockpitInstruments::simulation+3E85 - 48 8D 15 44751E00     - lea rdx,[RSE.RSE::CAeroplane_Pfalz_D_XII::`vftable'+1C8] { ("Pfalz D.XII") }
            case "Pfalz D.XII":
                planeAttributes.country = Country.GER;
                planeAttributes.speedometerType = DialVariant.A;
                planeAttributes.altimeter = true;
                planeAttributes.repeaterCompass = true;
                planeAttributes.rpmType = DialVariant.A;
                planeAttributes.manifoldType = DialVariant.None;
                planeAttributes.artificialHorizon = true;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+3E9C - 48 8D 15 ED0E1F00     - lea rdx,[RSE.RSE::CAeroplane_SPAD_VII_C1_150HP::`vftable'+1C8] { ("SPAD VII.C1 150HP") }
            case "SPAD VII.C1 150HP":
                planeAttributes.country = Country.FR;
                planeAttributes.speedometerType = DialVariant.A;
                planeAttributes.altimeter = true;
                planeAttributes.repeaterCompass = true;
                planeAttributes.rpmType = DialVariant.A;
                planeAttributes.manifoldType = DialVariant.None;
                planeAttributes.artificialHorizon = true;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+3EB3 - 48 8D 15 163D1F00     - lea rdx,[RSE.RSE::CAeroplane_SPAD_VII_C1_180HP::`vftable'+1C8] { ("SPAD VII.C1 180HP") }
            case "SPAD VII.C1 180HP":
                planeAttributes.country = Country.FR;
                planeAttributes.speedometerType = DialVariant.A;
                planeAttributes.altimeter = true;
                planeAttributes.repeaterCompass = true;
                planeAttributes.rpmType = DialVariant.A;
                planeAttributes.manifoldType = DialVariant.None;
                planeAttributes.artificialHorizon = true;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+3ECA - 48 8D 15 3F471D00     - lea rdx,[RSE.RSE::CAeroplane_Gotha_G5::`vftable'+1F0] { ("Gotha G.V") }
            case "Gotha G.V":
                planeAttributes.country = Country.UK;
                planeAttributes.speedometerType = DialVariant.A;
                planeAttributes.altimeter = true;
                planeAttributes.repeaterCompass = true;
                planeAttributes.rpmType = DialVariant.A;
                planeAttributes.manifoldType = DialVariant.None;
                planeAttributes.artificialHorizon = true;
                break;

            //bregeut
            //RSE.RSE::CCockpitInstruments::simulation+8DF7 - 48 8D 15 3AB21B00     - lea rdx,[RSE.RSE::CAeroplane_Breguet_14_B2::`vftable'+1F0] { ("Breguet type 14 B.2") }
            case "Breguet type 14 B.2":
                planeAttributes.country = Country.FR;
                planeAttributes.speedometerType = DialVariant.A;
                planeAttributes.altimeter = true;
                planeAttributes.repeaterCompass = true;
                planeAttributes.rpmType = DialVariant.A;
                planeAttributes.manifoldType = DialVariant.None;
                planeAttributes.artificialHorizon = true;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+8E12 - 48 8D 15 C7811D00     - lea rdx,[RSE.RSE::CAeroplane_Handley_Page_400::`vftable'+1F0] { ("Handley Page O/400") }
            case "Handley Page O/400":
                planeAttributes.country = Country.UK;
                planeAttributes.speedometerType = DialVariant.A;
                planeAttributes.altimeter = true;
                planeAttributes.repeaterCompass = true;
                planeAttributes.rpmType = DialVariant.A;
                planeAttributes.manifoldType = DialVariant.None;
                planeAttributes.artificialHorizon = true;
                break;

            //RSE.RSE::CAeroplane::simulation+91C - 48 8D 15 8D1A1F00     - lea rdx,[RSE.RSE::CAeroplane_Sopwith_Dolphin::`vftable'+1C8] { ("Sopwith Dolphin") }
            case "Sopwith Dolphin":
                planeAttributes.country = Country.UK;
                planeAttributes.speedometerType = DialVariant.A;
                planeAttributes.altimeter = true;
                planeAttributes.repeaterCompass = true;
                planeAttributes.rpmType = DialVariant.A;
                planeAttributes.manifoldType = DialVariant.None;
                planeAttributes.waterTempType = DialVariant.A;
                planeAttributes.artificialHorizon = true;
                break;

            //se 5
            //RSE.RSE::CCockpitInstruments::simulation+109B - 4C 8D 05 AE9D1F00     - lea r8,[RSE.RSE::CAeroplane_S_E_5a::`vftable'+1C8] { ("S.E.5a") }
            case "S.E.5a":
                planeAttributes.country = Country.UK;
                planeAttributes.speedometerType = DialVariant.A;
                planeAttributes.altimeter = true;
                planeAttributes.repeaterCompass = true;
                planeAttributes.rpmType = DialVariant.A;
                planeAttributes.manifoldType = DialVariant.None;
                planeAttributes.waterTempType = DialVariant.A;
                planeAttributes.artificialHorizon = true;
                break;

            //RSE.RSE::CAeroplane::simulation+997 - 48 8D 15 32241E00     - lea rdx,[RSE.RSE::CAeroplane_Halberstadt_CL_II_D_IIIa::`vftable'+1C8] { ("Halberstadt CL.II D.IIIa") }
            case "Halberstadt CL.II D.IIIa":
                planeAttributes.country = Country.GER;
                planeAttributes.speedometerType = DialVariant.A;
                planeAttributes.altimeter = true;
                planeAttributes.repeaterCompass = true;
                planeAttributes.rpmType = DialVariant.A;
                planeAttributes.manifoldType = DialVariant.None;
                planeAttributes.waterTempType = DialVariant.A;
                planeAttributes.artificialHorizon = true;
                break;

            //RSE.RSE::CAeroplane::simulation+9AF - 48 8D 15 8AF31D00     - lea rdx,[RSE.RSE::CAeroplane_Halberstadt_CL_II_D_IIIau::`vftable'+1C8] { ("Halberstadt CL.II D.IIIau") }
            case "Halberstadt CL.II D.IIIau":
                planeAttributes.country = Country.GER;
                planeAttributes.speedometerType = DialVariant.A;
                planeAttributes.altimeter = true;
                planeAttributes.repeaterCompass = true;
                planeAttributes.rpmType = DialVariant.A;
                planeAttributes.manifoldType = DialVariant.None;
                planeAttributes.waterTempType = DialVariant.A;
                planeAttributes.artificialHorizon = true;
                break;

            //RSE.RSE::CAeroplane::simulation + C07 - 48 8D 15 627D1E00 - lea rdx,[RSE.RSE::CAeroplane_Nieuport_28::`vftable'+1C8] { ("Nieuport 28.C1") }
            case "Nieuport 28.C1":
                planeAttributes.country = Country.GER;
                planeAttributes.speedometerType = DialVariant.A;
                planeAttributes.altimeter = true;
                planeAttributes.repeaterCompass = true;
                planeAttributes.rpmType = DialVariant.A;
                planeAttributes.manifoldType = DialVariant.None;
                planeAttributes.waterTempType = DialVariant.A;
                planeAttributes.artificialHorizon = true;
                break;

            //RSE.RSE::CAeroplane::simulation+6C3 - 48 8D 15 FE261D00     - lea rdx,[RSE.RSE::CAeroplane_Fokker_Dr_I::`vftable'+1F0] { ("Fokker Dr.I") }
            case "Fokker Dr.I":
                planeAttributes.country = Country.GER;
                planeAttributes.speedometerType = DialVariant.A;
                planeAttributes.altimeter = true;
                planeAttributes.repeaterCompass = true;
                planeAttributes.rpmType = DialVariant.A;
                planeAttributes.manifoldType = DialVariant.None;
                planeAttributes.waterTempType = DialVariant.A;
                planeAttributes.artificialHorizon = true;
                break;

            //RSE.RSE::CAeroplane::simulation+DA2 - 48 8D 15 6F3D1F00     - lea rdx,[RSE.RSE::CAeroplane_Sopwith_Triplane::`vftable'+1F0] { ("Sopwith Triplane") }
            case "Sopwith Triplane":
                planeAttributes.country = Country.UK;
                planeAttributes.speedometerType = DialVariant.A;
                planeAttributes.altimeter = true;
                planeAttributes.repeaterCompass = true;
                planeAttributes.rpmType = DialVariant.A;
                planeAttributes.manifoldType = DialVariant.None;
                planeAttributes.waterTempType = DialVariant.A;
                planeAttributes.artificialHorizon = true;
                break;

            //RSE.RSE::CAeroplane::simulation+DD6 - 48 8D 15 93122000     - lea rdx,[RSE.RSE::CCockpitInstruments::s_homingDeviceRefreshPeriod+2870] { ("DFW C.V") }
            case "DFW C.V":
                planeAttributes.country = Country.GER;
                planeAttributes.speedometerType = DialVariant.A;
                planeAttributes.altimeter = true;
                planeAttributes.repeaterCompass = true;
                planeAttributes.rpmType = DialVariant.A;
                planeAttributes.manifoldType = DialVariant.None;
                planeAttributes.waterTempType = DialVariant.A;
                planeAttributes.artificialHorizon = true;
                break;

            //RSE.RSE::CAeroplane::simulation+CEB - 48 8D 15 E6EA1B00     - lea rdx,[RSE.RSE::CAeroplane_Airco_DH_4::`vftable'+1C8] { ("Airco De Haviland 4") }
            case "Airco De Haviland 4":
                planeAttributes.country = Country.UK;
                planeAttributes.speedometerType = DialVariant.A;
                planeAttributes.altimeter = true;
                planeAttributes.repeaterCompass = true;
                planeAttributes.rpmType = DialVariant.A;
                planeAttributes.manifoldType = DialVariant.None;
                planeAttributes.waterTempType = DialVariant.A;
                planeAttributes.artificialHorizon = true;
                break;

            case "Tigermoth":
                planeAttributes.country = Country.UK;
                planeAttributes.speedometerType = DialVariant.A;
                planeAttributes.altimeter = true;
                planeAttributes.repeaterCompass = true;
                planeAttributes.rpmType = DialVariant.A;
                planeAttributes.manifoldType = DialVariant.None;
                planeAttributes.waterTempType = DialVariant.A;
                planeAttributes.artificialHorizon = true;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+410C - 48 8D 15 5DEA2300     - lea rdx,[RSE.RSE::CAeroplane_Schuckert_D_IV::`vftable'+268] { ("Schuckert D.IV") }
            case "Schuckert D.IV":
                planeAttributes.country = Country.GER;
                planeAttributes.speedometerType = DialVariant.A;
                planeAttributes.altimeter = true;
                planeAttributes.repeaterCompass = true;
                planeAttributes.rpmType = DialVariant.A;
                planeAttributes.manifoldType = DialVariant.None;
                planeAttributes.artificialHorizon = true;
                break;

            default:
                planeAttributes.country = Country.UNDEFINED;
                break;

        }

        return planeAttributes;

    }
}
