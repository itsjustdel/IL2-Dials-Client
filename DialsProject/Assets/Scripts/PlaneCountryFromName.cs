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
            case "CAeroplane_LaGG_3_ser_29":
                country = Country.RU;
                break;

            case "CAeroplane_Bf_109_F4":
                country = Country.GER;
                break;

            case "CAeroplane_Il_2_m_42":
                country = Country.RU;
                break;

            case "CAeroplane_Ju_87_D_3":
                country = Country.GER;
                break;

            case "CAeroplane_Yak_1_ser_69":
                country = Country.RU;
                break;

            case "CAeroplane_Bf_109_G2":
                country = Country.GER;
                break;

            case "CAeroplane_Pe_2_ser_87":
                country = Country.RU;
                break;

            case "CAeroplane_La_5_ser_8":
                country = Country.RU;
                break;

            case "CAeroplane_FW_190_A3":
                country = Country.GER;
                break;

            case "CAeroplane_He_111_H6":
                country = Country.GER;
                break;

            case "CAeroplane_MC_202_s8":
                country = Country.ITA;
                break;

            case "CAeroplane_Ju523mg4e":
                country = Country.GER;
                break;

            case "CAeroplane_I_16_t_24":
                country = Country.RU;
                break;

            case "CAeroplane_Bf_109_E7":
                country = Country.GER;
                break;

            case "CAeroplane_Bf_110_E2":
                country = Country.GER;
                break;

            case "CAeroplane_P_40E_1":
                country = Country.US;
                break;

            case "CAeroplane_MiG_3_ser_24":
                country = Country.RU;
                break;

            case "CAeroplane_Bf_109_F2":
                country = Country.GER;
                break;

            case "CAeroplane_Ju_88_A4":
                country = Country.GER;
                break;

            case "CAeroplane_Il_2_m_41":
                country = Country.RU;
                break;

            case "CAeroplane_Pe_2_ser_35":
                country = Country.RU;
                break;

            case "CAeroplane_Yak_1_ser_127":
                country = Country.RU;
                break;

            case "CAeroplane_Bf_109_G4":
                country = Country.GER;
                break;

            case "CAeroplane_FW_190_A5":
                country = Country.GER;
                break;

            case "CAeroplane_Bf_110_G2":
                country = Country.GER;
                break;

            case "CAeroplane_He_111_H16":
                country = Country.GER;
                break;

            case "CAeroplane_Il_2_m_43":
                country = Country.RU;
                break;

            case "CAeroplane_Spitfire_Mk_Vb":
                country = Country.UK;
                break;

            case "CAeroplane_Hs_129_B2":
                country = Country.GER;
                break;

            case "CAeroplane_A_20_B":
                country = Country.US;
                break;

            case "CAeroplane_P_39L_1":
                country = Country.US;
                break;

            case "CAeroplane_Yak_7B_ser_36":
                country = Country.RU;
                break;

            case "CAeroplane_La_5_FN_ser_2":
                country = Country.RU;
                break;

            case "CAeroplane_Bf_109_G6":
                country = Country.GER;
                break;

            case "CAeroplane_Bf_109_G14":
                country = Country.GER;
                break;

            case "CAeroplane_Spitfire_Mk_IXe":
                country = Country.GER;
                break;

            case "CAeroplane_FW_190_A8":
                country = Country.GER;
                break;

            case "CAeroplane_P_47D_28":
                country = Country.US;
                break;

            case "CAeroplane_Bf_109_K4":
                country = Country.GER;
                break;

            case "CAeroplane_P_51D_15":
                country = Country.US;
                break;

            case "CAeroplane_Me_262_A":
                country = Country.GER;
                break;

            case "CAeroplane_FW_190_D9":
                country = Country.GER;
                break;

            case "CAeroplane_P_38J_25":
                country = Country.US;
                break;

            case "CAeroplane_Tempest_Mk_V_ser_2":
                country = Country.US;
                break;

            case "CAeroplane_B_25_D":
                country = Country.US;
                break;

            case "CAeroplane_Yak_9_ser_1":
                country = Country.RU;
                break;

            case "CAeroplane_Yak_9T_ser_1":
                country = Country.RU;
                break;

            case "CAeroplane_P_47D_22":
                country = Country.US;
                break;

            case "CAeroplane_Hurricane_Mk_II":
                country = Country.UK;
                break;

            case "CAeroplane_C_47A":
                country = Country.US;
                break;

            case "CAeroplane_P_51B_5":
                country = Country.US;
                break;

            case "CAeroplane_Bf_109_G6_Late":
                country = Country.GER;
                break;

            case "CAeroplane_Spitfire_Mk_XIV":
                country = Country.UK;
                break;

            case "CAeroplane_FW_190_A6":
                country = Country.GER;
                break;

            default:
                country = Country.UNDEFINED;
                break;
        }

        return country;
    }
}
