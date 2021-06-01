using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//extending from airplanedata for Country enum
public class PlaneCountryFromName : AirplaneData
{
    public static Country AsignCountryFromName(string name)
    {
        Country country;

        switch (name)
        {

            //RSE.RSE::CCockpitInstruments::simulation+1117 - 48 8D 15 DA7A1200     - lea rdx,[RSE.RSE::CAeroplane_LaGG_3_ser_29::`vftable'+210] { ("LaGG-3 ser.29") }
            case "LaGG-3 ser.29":
                country = Country.RU;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+3E0 - 48 8D 15 29E11200     - lea rdx,[RSE.RSE::CAeroplane_Bf_109_F4::`vftable'+310] { ("Bf 109 F-4") }
            case "Bf 109 F-4":
                country = Country.GER;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+1995 - 48 8D 15 FC911700     - lea rdx,[RSE.RSE::CAeroplane_Il_2_m_41::`vftable'+1E8] { ("Il-2 mod.1941") }
            case "Il-2 mod.1941":
                country = Country.RU;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+19B0 - 48 8D 15 D9F61200     - lea rdx,[RSE.RSE::CAeroplane_Il_2_m_42::`vftable'+1E8] { ("Il-2 mod.1942") }
            case "Il-2 mod.1942":
                country = Country.RU;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+19CB - 48 8D 15 96ED0900     - lea rdx,[RSE.RSE::CAeroplane_Il_2_m_43::`vftable'+1E8] { ("Il-2 mod.1943") }
            case "Il-2 mod.1943":
                country = Country.RU;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+159 - 48 8D 15 78421300     - lea rdx,[RSE.RSE::CAeroplane_Ju_87_D_3::`vftable'+238] { ("Ju-87 D3") }
            case "Ju-87 D3":
                country = Country.GER;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+1E0 - 48 8D 15 89211400     - lea rdx,[RSE.RSE::CAeroplane_Yak_1_ser_69::`vftable'+1E8] { ("Yak-1 ser.69") }
            case "Yak-1 ser.69":
                country = Country.RU;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+3FB - 48 8D 15 D64E1400     - lea rdx,[RSE.RSE::CAeroplane_Bf_109_G2::`vftable'+310] { ("Bf 109 G-2") }
            case "Bf 109 G-2":
                country = Country.GER;
                break;

            //RSE.RSE::CCockpitInstruments::simulation + 1C5 - 48 8D 15 1C9B1300 - lea rdx,[RSE.RSE::CAeroplane_Pe_2_ser_87::`vftable'+1E8] { ("Pe-2 ser.87") }
            case "Pe-2 ser.87":
                country = Country.RU;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+216 - 48 8D 15 AB701400     - lea rdx,[RSE.RSE::CAeroplane_La_5_ser_8::`vftable'+290] { ("La-5 ser.8") }
            case "La-5 ser.8":
                country = Country.RU;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+482 - 48 8D 15 07AA1400     - lea rdx,[RSE.RSE::CAeroplane_FW_190_A3::`vftable'+2C0] { ("FW 190 A3") }
            case "FW 190 A3":
                country = Country.GER;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+174 - 48 8D 15 35CB1400     - lea rdx,[RSE.RSE::CAeroplane_He_111_H6::`vftable'+298] { ("He 111 H-6") }
            case "He 111 H-6":
                country = Country.GER;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+2199 - 48 8D 15 80B41400     - lea rdx,[RSE.RSE::CAeroplane_MC_202_s8::`vftable'+1E8] { ("MC 202 s8") }
            case "MC 202 s8":
                country = Country.ITA;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+CA2 - 48 8D 15 6FFD1400     - lea rdx,[RSE.RSE::CAeroplane_Ju523mg4e::`vftable'+1E8] { ("Ju-52/3m g4e") }
            case "Ju-52/3m g4e":
                country = Country.GER;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+2580 - 48 8D 15 416A1500     - lea rdx,[RSE.RSE::CAeroplane_I_16_t_24::`vftable'+1C0] { ("I-16 type 24") }
            case "I-16 type 24":
                country = Country.RU;
                break;
                            
            //RSE.RSE::CCockpitInstruments::simulation+467 - 48 8D 15 721C1600     - lea rdx,[RSE.RSE::CAeroplane_Bf_109_E7::`vftable'+1C0] { ("Bf 109 E-7") }
            case "Bf 109 E-7":
                country = Country.GER;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+5265 - 48 8D 15 84231600     - lea rdx,[RSE.RSE::CAeroplane_Bf_110_E2::`vftable'+400] { ("Bf-110 E2") }
            case "Bf-110 E2":
                country = Country.GER;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+BFB - 4C 8D 05 56E31500     - lea r8,[RSE.RSE::CFlywheelStarter_P_40E_1::`vftable'+68] { ("P-40E-1") }
            case "P-40E-1":
                country = Country.US;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+C51 - 48 8D 15 88A51600     - lea rdx,[RSE.RSE::CAeroplane_MiG_3_ser_24::`vftable'+1C0] { ("MiG-3 ser.24") }
            case "MiG-3 ser.24":
                country = Country.RU;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+3C5 - 48 8D 15 94131700     - lea rdx,[RSE.RSE::CAeroplane_Bf_109_F2::`vftable'+310] { ("Bf 109 F-2") }
            case "Bf 109 F-2":
                country = Country.GER;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+C87 - 48 8D 15 22131700     - lea rdx,[RSE.RSE::CAeroplane_Ju_88_A4::`vftable'+238] { ("Ju-88 A4") }
            case "Ju-88 A4":
                country = Country.GER;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+1AA - 48 8D 15 F7B01700     - lea rdx,[RSE.RSE::CAeroplane_Pe_2_ser_35::`vftable'+1E8] { ("Pe-2 ser.35") }
            case "Pe-2 ser.35":
                country = Country.RU;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+1FB - 48 8D 15 5E751000     - lea rdx,[RSE.RSE::CAeroplane_Yak_1_ser_127::`vftable'+1E8] { ("Yak-1 ser.127") }
            case "Yak-1 ser.127":
                country = Country.RU;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+416 - 48 8D 15 93BD0400     - lea rdx,[RSE.RSE::CAeroplane_Bf_109_G4::`vftable'+310] { ("Bf 109 G-4") }
            case "Bf 109 G-4":
                country = Country.GER;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+24C - 48 8D 15 3D5B0700     - lea rdx,[RSE.RSE::CAeroplane_FW_190_A5::`vftable'+2C0] { ("FW 190 A5") }
            case "FW 190 A5":
                country = Country.GER;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+5289 - 48 8D 15 08250500     - lea rdx,[RSE.RSE::CAeroplane_Bf_110_G2::`vftable'+400] { ("Bf-110 G2") }
            case "Bf-110 G2":
                country = Country.GER;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+217E - 48 8D 15 334D0800     - lea rdx,[RSE.RSE::CAeroplane_He_111_H16::`vftable'+298] { ("He 111 H-16") }
            case "He 111 H-16":
                country = Country.GER;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+CBD - 48 8D 15 7C330F00     - lea rdx,[RSE.RSE::CAeroplane_Spitfire_Mk_Vb::`vftable'+210] { ("Spitfire Mk.Vb") }
            case "Spitfire Mk.Vb":
                country = Country.UK;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+CD8 - 48 8D 15 D9EC0800     - lea rdx,[RSE.RSE::CAeroplane_Hs_129_B2::`vftable'+2E8] { ("Hs 129 B-2") }
            case "Hs 129 B-2":
                country = Country.GER;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+CFC - 4C 8D 05 9D5E0300     - lea r8,[RSE.RSE::CAeroplane_A_20_B::`vftable'+2E0] { ("A-20B") }
            case "A-20B":
                country = Country.US;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+D83 - 4C 8D 05 6EAF0C00     - lea r8,[RSE.RSE::CAeroplane_P_39L_1::`vftable'+210] { ("P-39L-1") }
            case "P-39L-1":
                country = Country.US;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+D29 - 48 8D 15 C0DE1000     - lea rdx,[RSE.RSE::CAeroplane_Yak_7B_ser_36::`vftable'+2C0] { ("Yak-7B ser.36") }
            case "Yak-7B ser.36":
                country = Country.RU;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+231 - 48 8D 15 78770A00     - lea rdx,[RSE.RSE::CAeroplane_La_5_FN_ser_2::`vftable'+2B8] { ("La-5 FN ser.2") }
            case "La-5 FN ser.2":
                country = Country.RU;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+2A8 - 48 8D 15 A1FB0400     - lea rdx,[RSE.RSE::CAeroplane_Bf_109_G6::`vftable'+310] { ("Bf 109 G-6") }
            case "Bf 109 G-6":
                country = Country.GER;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+2BF - 48 8D 15 72360400     - lea rdx,[RSE.RSE::CAeroplane_Bf_109_G14::`vftable'+310] { ("Bf 109 G-14") }
            case "Bf 109 G-14":
                country = Country.GER;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+DA9 - 48 8D 15 58B90E00     - lea rdx,[RSE.RSE::CAeroplane_Spitfire_Mk_IXe::`vftable'+2E8] { ("Spitfire Mk.IXe") }
            case "Spitfire Mk.IXe":
                country = Country.UK;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+27A - 48 8D 15 FFF10700     - lea rdx,[RSE.RSE::CAeroplane_FW_190_A8::`vftable'+2C0] { ("FW 190 A8") }
            case "FW 190 A8":
                country = Country.GER;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+E2F - 48 8D 15 62B70D00     - lea rdx,[RSE.RSE::CGearPost_P_47D_28::`vftable'+68] { ("P-47D-28") }
            case "P-47D-28":
                country = Country.US;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+E65 - 48 8D 15 044A0500     - lea rdx,[RSE.RSE::CAeroplane_Bf_109_K4::`vftable'+310] { ("Bf 109 K-4") }
            case "Bf 109 K-4":
                country = Country.GER;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+E4A - 48 8D 15 6F600E00     - lea rdx,[RSE.RSE::CAeroplane_P_51D_15::`vftable'+220] { ("P-51D-15") }
            case "P-51D-15":
                country = Country.US;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+5778 - 48 8D 15 09890A00     - lea rdx,[RSE.RSE::CAeroplane_Me_262_A::`vftable'+260] { ("Me 262 A") }
            case "Me 262 A":
                country = Country.GER;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+E80 - 48 8D 15 C1F80700     - lea rdx,[RSE.RSE::CAeroplane_FW_190_D9::`vftable'+430] { ("FW 190 D9") }
            case "FW 190 D9":
                country = Country.GER;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+DF9 - 48 8D 15 981B0C00     - lea rdx,[RSE.RSE::CAeroplane_P_38J_25::`vftable'+288] { ("P-38J-25") }
            case "P-38J-25":
                country = Country.US;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+E9B - 48 8D 15 56E10F00     - lea rdx,[RSE.RSE::CAeroplane_Tempest_Mk_V_ser_2::`vftable'+2E8] { ("Tempest Mk.V ser.2") }
            case "Tempest Mk.V ser.2":
                country = Country.US;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+EBF - 4C 8D 05 8A0C0600     - lea r8,[RSE.RSE::CAeroplane_B_25_D::`vftable'+2B8] { ("B-25D") }
            case "B-25D":
                country = Country.US;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+D44 - 48 8D 15 F5D01100     - lea rdx,[RSE.RSE::CAeroplane_Yak_9_ser_1::`vftable'+2E8] { ("Yak-9 ser.1") }
            case "Yak-9 ser.1":
                country = Country.RU;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+D5F - 48 8D 15 AA681100     - lea rdx,[RSE.RSE::CAeroplane_Yak_9T_ser_1::`vftable'+2E8] { ("Yak-9T ser.1") }
            case "Yak-9T ser.1":
                country = Country.RU;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+E14 - 48 8D 15 ED210D00     - lea rdx,[RSE.RSE::CGearPost_P_47D_22::`vftable'+68] { ("P-47D-22") }
            case "P-47D-22":
                country = Country.US;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+EE9 - 48 8D 15 C85E0900     - lea rdx,[RSE.RSE::CAeroplane_Hurricane_Mk_II::`vftable'+2E8] { ("Hurricane Mk.II") }
            case "Hurricane Mk.II":
                country = Country.UK;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+F0D - 4C 8D 05 ECA40600     - lea r8,[RSE.RSE::CAeroplane_C_47A::`vftable'+210] { ("C-47A") }
            case "C-47A":
                country = Country.US;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+F3F - 4C 8D 05 F2D40D00     - lea r8,[RSE.RSE::CAeroplane_P_51B_5::`vftable'+210] { ("P-51B-5") }
            case "P-51B-5":
                country = Country.US;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+F66 - 48 8D 15 530F0500     - lea rdx,[RSE.RSE::CAeroplane_Bf_109_G6_Late::`vftable'+310] { ("Bf 109 G-6 Late") }
            case "Bf 109 G-6 Late":
                country = Country.GER;
                break;

            //RSE.RSE::CCockpitInstruments::simulation + F7D - 48 8D 15 747B0F00 - lea rdx,[RSE.RSE::CAeroplane_Spitfire_Mk_XIV::`vftable'+2E8] { ("Spitfire Mk.XIV") }
            case "Spitfire Mk.XIV":
                country = Country.GER;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+263 - 48 8D 15 0EC40700     - lea rdx,[RSE.RSE::CAeroplane_FW_190_A6::`vftable'+8] { ("FW 190 A6") }
            case "FW 190 A6":
                country = Country.GER;
                break;

            //RSE.RSE::CCockpitInstruments::simulation+589A - 48 8D 15 DF201000     - lea rdx,[RSE.RSE::CAeroplane_Typhoon_Mk_Ib::`vftable'+310] { ("Typhoon Mk.Ib") }
            case "Typhoon Mk.Ib":
                country = Country.UK;
                break;

            default:
                country = Country.UNDEFINED;
                break;
        }

        return country;
    }
}
