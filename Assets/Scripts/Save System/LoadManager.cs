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

        foreach(string planeType in allPlanes)
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


        if (layout != null)
            ScaleAndPositions(dialsManager, menuHandler, layout);
        else
            Layout.DefaultLayouts(dialsManager.countryDialBoard);

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



    private static void ScaleAndPositions(DialsManager dialsManager, MenuHandler menuHandler, Layout layout)
    {

        //apply to dials/positions        
        dialsManager.speedometer.GetComponent<RectTransform>().anchoredPosition = layout.speedoPos;
        dialsManager.speedometer.GetComponent<RectTransform>().localScale = new Vector3(layout.speedoScale, layout.speedoScale, 1f);

        if (layout.speedoInTray)
            AddToTrayOnLoad(dialsManager.speedometer, menuHandler);

        GameObject altimeter = dialsManager.countryDialBoard.transform.Find("Altimeter").gameObject;
        altimeter.GetComponent<RectTransform>().anchoredPosition = layout.altPos;
        altimeter.GetComponent<RectTransform>().localScale = new Vector3(layout.altScale, layout.altScale, 1f);

        if (layout.altimeterInTray)
            AddToTrayOnLoad(altimeter, menuHandler);

        if (dialsManager.airplaneData.planeAttributes.country == Country.US)
        {
            //new
            dialsManager.headingIndicator.GetComponent<RectTransform>().anchoredPosition = layout.headingPos;
            dialsManager.headingIndicator.GetComponent<RectTransform>().localScale = new Vector3(layout.headingScale, layout.headingScale, 1f);

            if (layout.headingIndicatorInTray)
                AddToTrayOnLoad(dialsManager.headingIndicator, menuHandler);
        }
        else
        {
            //old
            if (dialsManager.countryDialBoard.transform.Find("Heading Indicator") != null)
            {
                GameObject headingIndicator = dialsManager.countryDialBoard.transform.Find("Heading Indicator").gameObject;
                headingIndicator.GetComponent<RectTransform>().anchoredPosition = layout.headingPos;
                headingIndicator.GetComponent<RectTransform>().localScale = new Vector3(layout.headingScale, layout.headingScale, 1f);

                if (layout.headingIndicatorInTray)
                    AddToTrayOnLoad(headingIndicator, menuHandler);
            }
        }

        if (dialsManager.countryDialBoard.transform.Find("Turn And Bank") != null)
        {
            GameObject turnAndBank = dialsManager.countryDialBoard.transform.Find("Turn And Bank").gameObject;
            turnAndBank.GetComponent<RectTransform>().anchoredPosition = layout.turnAndBankPos;
            turnAndBank.GetComponent<RectTransform>().localScale = new Vector3(layout.turnAndBankScale, layout.turnAndBankScale, 1f);

            if (layout.turnAndBankInTray)
                AddToTrayOnLoad(turnAndBank, menuHandler);

        }

        if (dialsManager.airplaneData.planeAttributes.country == Country.US)
        {
            //new
            dialsManager.turnIndicator.GetComponent<RectTransform>().anchoredPosition = layout.turnIndicatorPos;
            dialsManager.turnIndicator.GetComponent<RectTransform>().localScale = new Vector3(layout.turnIndicatorScale, layout.turnIndicatorScale, 1f);

            if (layout.turnIndicatorInTray)
                AddToTrayOnLoad(dialsManager.turnIndicator, menuHandler);
        }
        else
        {
            if (dialsManager.countryDialBoard.transform.Find("Turn Coordinator") != null)
            {

                GameObject turnIndicator = dialsManager.countryDialBoard.transform.Find("Turn Coordinator").gameObject;
                turnIndicator.GetComponent<RectTransform>().anchoredPosition = layout.turnIndicatorPos;
                turnIndicator.GetComponent<RectTransform>().localScale = new Vector3(layout.turnIndicatorScale, layout.turnIndicatorScale, 1f);

                if (layout.turnIndicatorInTray)
                    AddToTrayOnLoad(turnIndicator, menuHandler);
            }
        }

        if (dialsManager.airplaneData.planeAttributes.country == Country.US)
        {
            //new
            dialsManager.vsi.GetComponent<RectTransform>().anchoredPosition = layout.vsiPos;
            dialsManager.vsi.GetComponent<RectTransform>().localScale = new Vector3(layout.vsiScale, layout.vsiScale, 1f);

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
                vsi.GetComponent<RectTransform>().anchoredPosition = layout.vsiSmallestPos;
                vsi.GetComponent<RectTransform>().localScale = new Vector3(layout.vsiSmallestScale, layout.vsiSmallestScale, 1f);

                if (layout.vsiSmallestInTray)
                    AddToTrayOnLoad(vsi, menuHandler);
            }

            if (dialsManager.countryDialBoard.transform.Find("VSI Small") != null)
            {

                GameObject vsi = dialsManager.countryDialBoard.transform.Find("VSI Small").gameObject;
                vsi.GetComponent<RectTransform>().anchoredPosition = layout.vsiSmallPos;
                vsi.GetComponent<RectTransform>().localScale = new Vector3(layout.vsiSmallScale, layout.vsiSmallScale, 1f);

                if (layout.vsiSmallInTray)
                    AddToTrayOnLoad(vsi, menuHandler);
            }

            //both vsi share the same variable - only one vsi per plane
            if (dialsManager.countryDialBoard.transform.Find("VSI Large") != null)
            {

                GameObject vsi = dialsManager.countryDialBoard.transform.Find("VSI Large").gameObject;
                vsi.GetComponent<RectTransform>().anchoredPosition = layout.vsiLargePos;
                vsi.GetComponent<RectTransform>().localScale = new Vector3(layout.vsiLargeScale, layout.vsiLargeScale, 1f);

                if (layout.vsiLargeInTray)
                    AddToTrayOnLoad(vsi, menuHandler);
            }
        }

        if (dialsManager.airplaneData.planeAttributes.country == Country.US)
        {
            //new
            dialsManager.artificialHorizon.GetComponent<RectTransform>().anchoredPosition = layout.artificialHorizonPos;
            dialsManager.artificialHorizon.GetComponent<RectTransform>().localScale = new Vector3(layout.artificialHorizonScale, layout.artificialHorizonScale, 1f);

            if (layout.artificialHorizonInTray)
                AddToTrayOnLoad(dialsManager.artificialHorizon, menuHandler);
        }
        else
        {
            //old
            if (dialsManager.countryDialBoard.transform.Find("Artificial Horizon") != null)
            {

                GameObject artificialHorizon = dialsManager.countryDialBoard.transform.Find("Artificial Horizon").gameObject;
                artificialHorizon.GetComponent<RectTransform>().anchoredPosition = layout.artificialHorizonPos;
                artificialHorizon.GetComponent<RectTransform>().localScale = new Vector3(layout.artificialHorizonScale, layout.artificialHorizonScale, 1f);

                if (layout.artificialHorizonInTray)
                    AddToTrayOnLoad(artificialHorizon, menuHandler);
            }
        }

        if (dialsManager.countryDialBoard.transform.Find("Repeater Compass") != null)
        {

            GameObject repeaterCompass = dialsManager.countryDialBoard.transform.Find("Repeater Compass").gameObject;
            repeaterCompass.GetComponent<RectTransform>().anchoredPosition = layout.repeaterCompassPos;
            repeaterCompass.GetComponent<RectTransform>().localScale = new Vector3(layout.repeaterCompassScale, layout.repeaterCompassScale, 1f);

            if (layout.repeaterCompassInTray)
                AddToTrayOnLoad(repeaterCompass, menuHandler);
        }

        if (dialsManager.countryDialBoard.transform.Find("Repeater Compass Alternate") != null)
        {
            GameObject repeaterCompassAlternate = dialsManager.countryDialBoard.transform.Find("Repeater Compass Alternate").gameObject;
            //using non alternate variables because we won't have two compasses 
            repeaterCompassAlternate.GetComponent<RectTransform>().anchoredPosition = layout.repeaterCompassAlternatePos;
            repeaterCompassAlternate.GetComponent<RectTransform>().localScale = new Vector3(layout.repeaterCompassAlternateScale, layout.repeaterCompassAlternateScale, 1f);

            if (layout.repeaterCompassAlternateInTray)
                AddToTrayOnLoad(repeaterCompassAlternate, menuHandler);
        }


        for (int i = 0; i < dialsManager.rpmObjects.Count; i++)
        {
            dialsManager.rpmObjects[i].GetComponent<RectTransform>().anchoredPosition = layout.rpmPos[i];
            dialsManager.rpmObjects[i].GetComponent<RectTransform>().localScale = new Vector3(layout.rpmScale[i], layout.rpmScale[i], 1f);

            if (layout.rpmInTray[i])
                AddToTrayOnLoad(dialsManager.rpmObjects[i], menuHandler);
        }

        for (int i = 0; i < dialsManager.manifoldObjects.Count; i++)
        {
            dialsManager.manifoldObjects[i].GetComponent<RectTransform>().anchoredPosition = layout.manifoldPos[i];
            dialsManager.manifoldObjects[i].GetComponent<RectTransform>().localScale = new Vector3(layout.manifoldScale[i], layout.manifoldScale[i], 1f);

            if (layout.manifoldInTray[i])
                AddToTrayOnLoad(dialsManager.manifoldObjects[i], menuHandler);
        }


        for (int i = 0; i < dialsManager.waterTempObjects.Count; i++)
        {
            dialsManager.waterTempObjects[i].GetComponent<RectTransform>().anchoredPosition = layout.waterTempPos[i];
            dialsManager.waterTempObjects[i].GetComponent<RectTransform>().localScale = new Vector3(layout.waterTempScale[i], layout.waterTempScale[i], 1f);

            if (layout.waterTempInTray[i])
                AddToTrayOnLoad(dialsManager.waterTempObjects[i], menuHandler);
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
