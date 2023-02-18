using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;


public class LoadManager : MonoBehaviour
{
    public static int step = 500;

    public static void MigrateLayoutsToNewVersion(int clientId)
    {
        //called on load
        //look for all keys matching any plane name
        List<string> allPlanes = new List<string>();
        allPlanes.AddRange(PlaneLists.UkPlanes);
        allPlanes.AddRange(PlaneLists.UsPlanes);
        allPlanes.AddRange(PlaneLists.GerPlanes);
        allPlanes.AddRange(PlaneLists.RuPlanes);
        allPlanes.AddRange(PlaneLists.ItaPlanes);
        allPlanes.AddRange(PlaneLists.FrPlanes);

        foreach (string planeType in allPlanes)
        {
            string key = PlayerPrefs.GetString(planeType);
            if (key != "")
            {
                //we have found an old key
                //rebuild and save using new method
                Layout layout = JsonUtility.FromJson<Layout>(key);


                //pack with json utility
                string jsonFoo = JsonUtility.ToJson(layout);

                //save packed string to player preferences (unity)
                //save with id to know if user addded a a second window - if no id, save only plane name ( this will be the master client)
                key = "layout " + clientId.ToString() + " " + planeType;
                
                PlayerPrefs.SetString(key, jsonFoo);
                PlayerPrefs.Save();


                //now remove old key
                PlayerPrefs.DeleteKey(key);

                //layout updated!
                UnityEngine.Debug.Log("Layout upgraded");
            }
        }
    }

    public static void LoadLayout(AirplaneData airplaneData, DialsManager dialsManager)
    {

        UnityEngine.Debug.Log("Load layout");
        MenuHandler menuHandler = GameObject.FindGameObjectWithTag("MenuObject").GetComponent<MenuHandler>();

        //first of all empty trays
        ButtonManager.EmptyTrays(menuHandler);

        //to find the correct layout look through all keys in registry matching this client's id
        int id = dialsManager.slaveManager.id;
#if UNITY_ANDROID
        Layout layout = GetLayoutAndroid(airplaneData);
#else
        Layout layout = GetLayout(id,airplaneData);
#endif

        //work out default scale for dials depening how many are active for this plane - Used to control ui size too
        RectTransform canvasRectTransform = GameObject.FindGameObjectWithTag("Canvas").GetComponent<RectTransform>();
        
        List<GameObject> activeDials = ActiveDials(dialsManager.countryDialBoard);
        //workout default sizes (saves to globals)
        Layout.RowsColumnsSize(canvasRectTransform, activeDials.Count);

        if (layout != null)
            ScaleAndPositions(dialsManager, menuHandler, layout);
        else
            Layout.DefaultLayouts(activeDials,canvasRectTransform);

    }

    private static Layout GetLayoutAndroid(AirplaneData airplaneData)
    {        
        string jsonFoo = PlayerPrefs.GetString(airplaneData.planeType);
        //and rebuild layout
        Layout layout = JsonUtility.FromJson<Layout>(jsonFoo);
        return layout;        
    }


    private static Layout GetLayout(int id, AirplaneData airplaneData)
    {
        //check if layouts need upgraded to new version
        //MigrateLayoutsToNewVersion(id); --not working

        //get all player prefs that start with this plane type
        string[] keys = PlayerPrefsHelper.GetRegistryValues();
        
        foreach (string key in keys)
        {
            //layout keys are saved with id then plane type e.g (0 il2 mod 1942), (1 spitfire-123)
            string[] subs = key.Split(' ');

            //UnityEngine.Debug.Log("Sub 0 = " + subs[0]);
            //we are looking for a layout
            if(subs[0] == "layout")
            {
                //we are looking for matching client ids
                //UnityEngine.Debug.Log("sub = " + int.Parse(subs[1]) + " id " + id);
                    
                if (int.Parse( subs[1] ) == id)
                {
                  // UnityEngine.Debug.Log("found id");
                    
                    //_h3923205751 = 12 chars - not always!
                    string planeType = "";
                    //start after "layout" and client number "0" or "1"
                    int start = subs[0].Length + subs[1].Length + 2; //2 spaces
                    for (int i = start; i < key.Length-1; i++)
                    {
                        //look for handle "_h" - we don't need values after key[x] is a char so do conversion                        
                        if (key[i].ToString() == "_" && key[i + 1].ToString() == "h")
                            break;
                        
                        planeType += key[i];
                    }

                  //  UnityEngine.Debug.Log("plane type = " + planeType);

                    //we are looking to match the plane type with the game data
                    if (airplaneData.planeType == planeType)
                    {
                       // UnityEngine.Debug.Log("found plane type");
                        //load layout from key
                        string jsonFoo = PlayerPrefs.GetString(key);
                        //and rebuild layout
                        Layout layout = JsonUtility.FromJson<Layout>(jsonFoo);
                        return layout;

                    }

                }
            }
        }

        return null;
    }

    private static void DialScalePosition(GameObject dialParent, Vector3 position, float scale)
    {
        //UnityEngine.Debug.Log("Load layout - dial scale = " + Layout.dialScale);
        dialParent.GetComponent<RectTransform>().anchoredPosition = position;

        
        dialParent.transform.Find("Dial").GetComponent<RectTransform>().localScale = new Vector3(scale, scale, 1f);
        //set ui to previously worked out ui scale
        dialParent.transform.Find("UI Handlers").GetComponent<RectTransform>().localScale = new Vector3(Layout.dialScale, Layout.dialScale, 1f);

    }

    private static void ScaleAndPositions(DialsManager dialsManager, MenuHandler menuHandler, Layout layout)
    {
        UnityEngine.Debug.Log("ScaleAndPositions from saved layout");
        DialScalePosition(dialsManager.speedometer, layout.speedoPos, layout.speedoScale);
        if (layout.speedoInTray)
            AddToTrayOnLoad(dialsManager.speedometer, menuHandler);

        GameObject altimeter = dialsManager.countryDialBoard.transform.Find("Altimeter").gameObject;        
        DialScalePosition(altimeter, layout.altPos, layout.altScale);
        if (layout.altimeterInTray)
            AddToTrayOnLoad(altimeter, menuHandler);

        if (dialsManager.airplaneData.planeAttributes.country == Country.US)
        {
            //new            
            DialScalePosition(dialsManager.headingIndicator, layout.headingPos, layout.headingScale);

            if (layout.headingIndicatorInTray)
                AddToTrayOnLoad(dialsManager.headingIndicator, menuHandler);
        }
        else
        {
            //old
            if (dialsManager.countryDialBoard.transform.Find("Heading Indicator") != null)
            {
                GameObject headingIndicator = dialsManager.countryDialBoard.transform.Find("Heading Indicator").gameObject;
                DialScalePosition(headingIndicator, layout.headingPos, layout.headingScale);
                if (layout.headingIndicatorInTray)
                    AddToTrayOnLoad(headingIndicator, menuHandler);
            }
        }

        if (dialsManager.countryDialBoard.transform.Find("Turn And Bank") != null)
        {
            GameObject turnAndBank = dialsManager.countryDialBoard.transform.Find("Turn And Bank").gameObject;
            DialScalePosition(turnAndBank, layout.turnAndBankPos, layout.turnAndBankScale);
            if (layout.turnAndBankInTray)
                AddToTrayOnLoad(turnAndBank, menuHandler);

        }

        if (dialsManager.airplaneData.planeAttributes.country == Country.US)
        {
            //new
            DialScalePosition(dialsManager.turnIndicator, layout.turnIndicatorPos, layout.turnIndicatorScale);
            if (layout.turnIndicatorInTray)
                AddToTrayOnLoad(dialsManager.turnIndicator, menuHandler);
        }
        else
        {
            if (dialsManager.countryDialBoard.transform.Find("Turn Coordinator") != null)
            {

                GameObject turnIndicator = dialsManager.countryDialBoard.transform.Find("Turn Coordinator").gameObject;
                DialScalePosition(turnIndicator, layout.turnIndicatorPos, layout.turnIndicatorScale);
                if (layout.turnIndicatorInTray)
                    AddToTrayOnLoad(turnIndicator, menuHandler);
            }
        }

        if (dialsManager.airplaneData.planeAttributes.country == Country.US)
        {
            //new
            DialScalePosition(dialsManager.vsi, layout.vsiPos, layout.vsiScale);
            if (layout.vsiInTray)
                AddToTrayOnLoad(dialsManager.vsi, menuHandler);
        }
        else
        {
            //old

            //both vsi share the same variable - only one vsi per plane
            if (dialsManager.countryDialBoard.transform.Find("VSI Smallest") != null)
            {
                GameObject vsi = dialsManager.countryDialBoard.transform.Find("VSI Smallest").gameObject;
                DialScalePosition(vsi, layout.vsiSmallestPos, layout.vsiSmallestScale);
                if (layout.vsiSmallestInTray)
                    AddToTrayOnLoad(vsi, menuHandler);
            }

            if (dialsManager.countryDialBoard.transform.Find("VSI Small") != null)
            {

                GameObject vsi = dialsManager.countryDialBoard.transform.Find("VSI Small").gameObject;
                DialScalePosition(vsi, layout.vsiSmallPos, layout.vsiSmallScale);
                if (layout.vsiSmallInTray)
                    AddToTrayOnLoad(vsi, menuHandler);
            }

            //both vsi share the same variable - only one vsi per plane
            if (dialsManager.countryDialBoard.transform.Find("VSI Large") != null)
            {

                GameObject vsi = dialsManager.countryDialBoard.transform.Find("VSI Large").gameObject;
                DialScalePosition(vsi, layout.vsiLargePos, layout.vsiLargeScale);
                if (layout.vsiLargeInTray)
                    AddToTrayOnLoad(vsi, menuHandler);
            }
        }

        if (dialsManager.airplaneData.planeAttributes.country == Country.US)
        {
            //new
            DialScalePosition(dialsManager.artificialHorizon, layout.artificialHorizonPos, layout.artificialHorizonScale);
            if (layout.artificialHorizonInTray)
                AddToTrayOnLoad(dialsManager.artificialHorizon, menuHandler);
        }
        else
        {
            //old
            if (dialsManager.countryDialBoard.transform.Find("Artificial Horizon") != null)
            {
                GameObject artificialHorizon = dialsManager.countryDialBoard.transform.Find("Artificial Horizon").gameObject;
                DialScalePosition(artificialHorizon, layout.artificialHorizonPos, layout.artificialHorizonScale);
                if (layout.artificialHorizonInTray)
                    AddToTrayOnLoad(artificialHorizon, menuHandler);
            }
        }

        if (dialsManager.countryDialBoard.transform.Find("Repeater Compass") != null)
        {
            GameObject repeaterCompass = dialsManager.countryDialBoard.transform.Find("Repeater Compass").gameObject;
            DialScalePosition(repeaterCompass, layout.repeaterCompassPos, layout.repeaterCompassScale);
            if (layout.repeaterCompassInTray)
                AddToTrayOnLoad(repeaterCompass, menuHandler);
        }

        if (dialsManager.countryDialBoard.transform.Find("Repeater Compass Alternate") != null)
        {
            GameObject repeaterCompassAlternate = dialsManager.countryDialBoard.transform.Find("Repeater Compass Alternate").gameObject;
            //using non alternate variables because we won't have two compasses (yet?)
            DialScalePosition(repeaterCompassAlternate, layout.repeaterCompassAlternatePos, layout.repeaterCompassAlternateScale);
            if (layout.repeaterCompassAlternateInTray)
                AddToTrayOnLoad(repeaterCompassAlternate, menuHandler);
        }


        for (int i = 0; i < dialsManager.rpmObjects.Count; i++)
        {
            DialScalePosition(dialsManager.rpmObjects[i], layout.rpmPos[i], layout.rpmScale[i]);
            if (layout.rpmInTray[i])
                AddToTrayOnLoad(dialsManager.rpmObjects[i], menuHandler);
        }

        for (int i = 0; i < dialsManager.manifoldObjects.Count; i++)
        {
            DialScalePosition(dialsManager.manifoldObjects[i], layout.manifoldPos[i], layout.manifoldScale[i]);
            if (layout.manifoldInTray[i])
                AddToTrayOnLoad(dialsManager.manifoldObjects[i], menuHandler);
        }


        for (int i = 0; i < dialsManager.waterTempObjects.Count; i++)
        {
            DialScalePosition(dialsManager.waterTempObjects[i], layout.waterTempPos[i], layout.waterTempScale[i]);
            if (layout.waterTempInTray[i])
                AddToTrayOnLoad(dialsManager.waterTempObjects[i], menuHandler);
        }
      
        for (int i = 0; i < dialsManager.oilTempInObjects.Count; i++)
        {
            DialScalePosition(dialsManager.oilTempInObjects[i], layout.oilTempInPos[i], layout.oilTempInScale[i]);
            if (layout.oilTempInInTray[i])
                AddToTrayOnLoad(dialsManager.oilTempInObjects[i], menuHandler);
        }

        for (int i = 0; i < dialsManager.oilTempOutObjects.Count; i++)
        {
            DialScalePosition(dialsManager.oilTempOutObjects[i], layout.oilTempOutPos[i], layout.oilTempOutScale[i]);
            if (layout.oilTempOutInTray[i])
                AddToTrayOnLoad(dialsManager.oilTempOutObjects[i], menuHandler);
        }

        for (int i = 0; i < dialsManager.oilTempPressureObjects.Count; i++)
        {
            DialScalePosition(dialsManager.oilTempPressureObjects[i], layout.oilTempPressurePos[i], layout.oilTempPressureScale[i]);
            if (layout.oilTempPressureInTray[i])
                AddToTrayOnLoad(dialsManager.oilTempPressureObjects[i], menuHandler);
        }

        for (int i = 0; i < dialsManager.oilTempComboObjects.Count; i++)
        {
            DialScalePosition(dialsManager.oilTempComboObjects[i], layout.oilTempComboPos[i], layout.oilTempComboScale[i]);
            if (layout.oilTempComboInTray[i])
                AddToTrayOnLoad(dialsManager.oilTempComboObjects[i], menuHandler);
        }

        for (int i = 0; i < dialsManager.cylinderHeadObjects.Count; i++)
        {
            DialScalePosition(dialsManager.cylinderHeadObjects[i], layout.cylinderHeadPos[i], layout.cylinderHeadScale[i]);
            if (layout.cylinderHeadInTray[i])
                AddToTrayOnLoad(dialsManager.cylinderHeadObjects[i], menuHandler);
        }

        for (int i = 0; i < dialsManager.carbTempObjects.Count; i++)
        {
            DialScalePosition(dialsManager.carbTempObjects[i], layout.carbAirPos[i], layout.carbAirScale[i]);
            if (layout.carbAirInTray[i])
                AddToTrayOnLoad(dialsManager.carbTempObjects[i], menuHandler);
        }
    }

    static void AddToTrayOnLoad(GameObject dial, MenuHandler menuHandler)
    {
        //USe button manager class to store dial in tray
        ButtonManager.PutDialInTray(dial, menuHandler);
    }

    public static List<GameObject> ActiveDials(GameObject dialsPrefab)
    {
        List<GameObject> activeDials = new List<GameObject>();
        for (int i = 0; i < dialsPrefab.transform.childCount; i++)
            if (dialsPrefab.transform.GetChild(i).gameObject.activeSelf)
                activeDials.Add(dialsPrefab.transform.GetChild(i).gameObject);

        return activeDials;
    }



}
