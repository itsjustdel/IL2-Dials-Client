using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class DialsManager : MonoBehaviour
{
    //This class manages the loading and saving of layouts and population of dials on plane change
    public AirplaneData airplaneData;
    public MenuHandler menuHandler;
    public TCPClient tcpClient;
    public GameObject countryDialBoard;

    public List<GameObject> rpmObjects = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckForPlaneChange();


        //update previous so we can detect a change on next update
        airplaneData.planeTypePrevious = airplaneData.planeType;
    }


    void CheckForPlaneChange()
    {
        // if we detect a plance change
        if (airplaneData.planeType != airplaneData.planeTypePrevious)
        {
            //check if layout panel is open, save and close before we proceed
            //simulate accept click if there was a plane loaded
            if (menuHandler.layoutOpen && airplaneData.planeAttributes.country != PlaneDataFromName.Country.UNDEFINED)
                menuHandler.AcceptLayoutClick();

            //construct country and available dials in to planeAttributes class/struct
            airplaneData.planeAttributes = PlaneDataFromName.AttributesFromName(airplaneData.planeType);

            if (airplaneData.planeAttributes.country == PlaneDataFromName.Country.UNDEFINED)
                return;

           

            //set as its own public variable to expose in hierarchy (in unity) for testing ease
            airplaneData.country = airplaneData.planeAttributes.country;

            SwitchDialBoardFromCountry(airplaneData.country);

            //switch off any unavailable dials to this plane
            DeactivateUnavailableDials(countryDialBoard, airplaneData.planeType, airplaneData.planeAttributes, rpmObjects);


            //vis needle variables need updated depending on what dials were loaded
            AsignVSI(airplaneData.planeAttributes, countryDialBoard);

            AsignRPM(airplaneData.planeAttributes, countryDialBoard);


            if (countryDialBoard != null)
                LoadLayout();
            else
            //close layout
            {
                //go back to main page
                menuHandler.layoutPanel.SetActive(false);

                //tunr menu button and leds back on
                menuHandler.menuButton.SetActive(true);
                menuHandler.ledParent.SetActive(true);

                menuHandler.layoutOpen = false;
            }

        }

    }


    //POOSIBLE NEW CLASS FROM HERE?
    void AsignVSI(PlaneDataFromName.PlaneAttributes planeAttributes, GameObject countryDialBoard)
    {
        //there are more than one vsi but never more than one at the same time, so we share prefabs
        //let the rotate needle script know which needle to turn
        if (planeAttributes.vsiLarge)
        {
            countryDialBoard.GetComponent<RotateNeedle>().vsiNeedle = countryDialBoard.transform.Find("VSI Large").Find("Needle").gameObject;
        }

        else if (planeAttributes.vsiSmall)
        {
            countryDialBoard.GetComponent<RotateNeedle>().vsiNeedle = countryDialBoard.transform.Find("VSI Small").Find("Needle").gameObject;
        }

        else if (planeAttributes.vsiSmallest)
        {
            countryDialBoard.GetComponent<RotateNeedle>().vsiNeedle = countryDialBoard.transform.Find("VSI Smallest").Find("Needle").gameObject;
        }
    }

    void AsignRPM(PlaneDataFromName.PlaneAttributes planeAttributes, GameObject countrydialBoard)
    {
        //empty lists first
        countryDialBoard.GetComponent<RotateNeedle>().rpmNeedlesLarge.Clear();
        countryDialBoard.GetComponent<RotateNeedle>().rpmNeedlesSmall.Clear();


        for (int i = 0; i < planeAttributes.engines; i++)
        {
            /*
            if (planeAttributes.country == AirplaneData.Country.RU)
            {
                if (planeAttributes.rpmA)
                {
                    //large and small needles for each rpm
                    countryDialBoard.GetComponent<RotateNeedle>().rpmNeedlesLarge.Add(countryDialBoard.transform.Find("RPM A " + i.ToString()).Find("Needle Large").gameObject);
                    countryDialBoard.GetComponent<RotateNeedle>().rpmNeedlesSmall.Add(countryDialBoard.transform.Find("RPM A " + i.ToString()).Find("Needle Small").gameObject);
                }

                if (planeAttributes.rpmB)
                {
                    //russian rpm b only has one needle
                    countryDialBoard.GetComponent<RotateNeedle>().rpmNeedlesLarge.Add(countryDialBoard.transform.Find("RPM B " + i.ToString()).Find("Needle Large").gameObject);
                    
                }
            }

            if (planeAttributes.country == AirplaneData.Country.GER)
            {
                if (planeAttributes.rpmA)
                {
                    countryDialBoard.GetComponent<RotateNeedle>().rpmNeedlesLarge.Add(countryDialBoard.transform.Find("RPM A " + i.ToString()).Find("Needle Large").gameObject);
                }
            }

            if (planeAttributes.country == AirplaneData.Country.UK)
            {
                if (planeAttributes.rpmA)
                {
                    countryDialBoard.GetComponent<RotateNeedle>().rpmNeedlesLarge.Add(countryDialBoard.transform.Find("RPM A " + i.ToString()).Find("Needle Large").gameObject);

                }

                if (planeAttributes.rpmB)
                {

                    countryDialBoard.GetComponent<RotateNeedle>().rpmNeedlesLarge.Add(countryDialBoard.transform.Find("RPM B " + i.ToString()).Find("Needle Large").gameObject);
                }
            }

            if (planeAttributes.country == AirplaneData.Country.US)
            {
                if (planeAttributes.rpmA)
                {
                    countryDialBoard.GetComponent<RotateNeedle>().rpmNeedlesLarge.Add(countryDialBoard.transform.Find("RPM A " + i.ToString()).Find("Needle Large").gameObject);
                    countryDialBoard.GetComponent<RotateNeedle>().rpmNeedlesSmall.Add(countryDialBoard.transform.Find("RPM A " + i.ToString()).Find("Needle Small").gameObject);

                }

                if (planeAttributes.rpmB)
                {

                    countryDialBoard.GetComponent<RotateNeedle>().rpmNeedlesLarge.Add(countryDialBoard.transform.Find("RPM B " + i.ToString()).Find("Needle Large").gameObject);
                }
            }

            if (planeAttributes.country == AirplaneData.Country.ITA)
            {
                if (planeAttributes.rpmA)
                {
                    countryDialBoard.GetComponent<RotateNeedle>().rpmNeedlesLarge.Add(countryDialBoard.transform.Find("RPM A " + i.ToString()).Find("Needle Large").gameObject);
                }
            }
            */
        }

    }

   

    public void SwitchDialBoardFromCountry(AirplaneData.Country country)
    {
        //change dials depending on what value we received from the networking component

        //remove any existing dials board prefab in scene
        if (countryDialBoard != null)
            Destroy(countryDialBoard);

        GameObject canvas = GameObject.FindGameObjectWithTag("Canvas");

        switch (country)
        {
            case AirplaneData.Country.RU:
                //countryDials[0].SetActive(true);
                GameObject RUprefab = Resources.Load("Prefabs/RU") as GameObject;
                countryDialBoard = GameObject.Instantiate(RUprefab, canvas.transform.position, Quaternion.identity, canvas.transform.GetChild(0).transform);
                break;

            case AirplaneData.Country.GER:
                GameObject GERprefab = Resources.Load("Prefabs/GER") as GameObject;
                countryDialBoard = GameObject.Instantiate(GERprefab, canvas.transform.position, Quaternion.identity, canvas.transform.GetChild(0).transform);
                break;

            case AirplaneData.Country.US:
                GameObject USprefab = Resources.Load("Prefabs/US") as GameObject;
                countryDialBoard = GameObject.Instantiate(USprefab, canvas.transform.position, Quaternion.identity, canvas.transform.GetChild(0).transform);
                break;

            case AirplaneData.Country.UK:
                GameObject UKprefab = Resources.Load("Prefabs/UK") as GameObject;
                countryDialBoard = GameObject.Instantiate(UKprefab, canvas.transform.position, Quaternion.identity, canvas.transform.GetChild(0).transform);
                break;

            case AirplaneData.Country.ITA:
                GameObject ITAprefab = Resources.Load("Prefabs/ITA") as GameObject;
                countryDialBoard = GameObject.Instantiate(ITAprefab, canvas.transform.position, Quaternion.identity, canvas.transform.GetChild(0).transform);
                break;

            default:
                countryDialBoard = null;
                break;
        }

        if (countryDialBoard != null)
        {
            //asign variables - can't asign scen variables to prefab in editor
            tcpClient.rN = countryDialBoard.GetComponent<RotateNeedle>();
            tcpClient.rN.buildControl = GameObject.Find("Build Chooser").GetComponent<BuildControl>();
            tcpClient.rN.airplaneData = GameObject.Find("Player Plane").GetComponent<AirplaneData>();
            tcpClient.rN.tcpClient = GameObject.Find("Networking").GetComponent<TCPClient>();

            //Instantiate RPMs
            rpmObjects.Clear();
            //is this condition true ? yes : no
            string rpmString = airplaneData.planeAttributes.rpmType == RpmType.A ? "A" : "B";
            if (countryDialBoard.transform.Find("RPM " + rpmString) != null)
            {
                //find prefab outside of loop
                GameObject rpm = countryDialBoard.transform.Find("RPM " + rpmString).gameObject;
                for (int i = 0; i < airplaneData.planeAttributes.engines; i++)
                {
                    //create instance variable if we need to duplicate
                    GameObject rpmInstance = rpm;
                    if (i > 0)
                    {
                        

                        //duplicate if we have more than one engine
                        rpmInstance = Instantiate(rpm, rpm.transform.position, Quaternion.identity, countryDialBoard.transform);
                        
                    }
                    rpmInstance.transform.name = "RPM " + airplaneData.planeAttributes.rpmType.ToString() + " " + i.ToString();
                                        
                    rpmObjects.Add(rpmInstance);
                }
            }
        }
    }

    //static to refactor to new class - to do
    static void DeactivateUnavailableDials(GameObject countryDialBoard, string planeName, PlaneDataFromName.PlaneAttributes planeAttributes, List<GameObject> rpmObjects)
    {
        //check what dials are available and switch off as needed
        //altimeter and speedo are always available

        if (!planeAttributes.headingIndicator)
            if (countryDialBoard.transform.Find("Heading Indicator") != null)
                countryDialBoard.transform.Find("Heading Indicator").gameObject.SetActive(false);

        if (!planeAttributes.turnAndBank)
            if (countryDialBoard.transform.Find("Turn And Bank") != null)
                countryDialBoard.transform.Find("Turn And Bank").gameObject.SetActive(false);

        if (!planeAttributes.turnCoordinator)
            if (countryDialBoard.transform.Find("Turn Coordinator").gameObject != null)
                countryDialBoard.transform.Find("Turn Coordinator").gameObject.SetActive(false);

        if (!planeAttributes.vsiSmallest)
            if (countryDialBoard.transform.Find("VSI Smallest") != null)
                countryDialBoard.transform.Find("VSI Smallest").gameObject.SetActive(false);

        if (!planeAttributes.vsiSmall)
            if (countryDialBoard.transform.Find("VSI Small") != null)
                countryDialBoard.transform.Find("VSI Small").gameObject.SetActive(false);

        if (!planeAttributes.vsiLarge)
            if (countryDialBoard.transform.Find("VSI Large") != null)
                countryDialBoard.transform.Find("VSI Large").gameObject.SetActive(false);

        if (!planeAttributes.repeaterCompass)
            if (countryDialBoard.transform.Find("Repeater Compass") != null)
                countryDialBoard.transform.Find("Repeater Compass").gameObject.SetActive(false);

        if (!planeAttributes.repeaterCompassAlternate)
            if (countryDialBoard.transform.Find("Repeater Compass Alternate") != null)
                countryDialBoard.transform.Find("Repeater Compass Alternate").gameObject.SetActive(false);

        if (!planeAttributes.artificialHorizon)
            if (countryDialBoard.transform.Find("Artificial Horizon") != null)
                countryDialBoard.transform.Find("Artificial Horizon").gameObject.SetActive(false);



        GameObject[] allRpmsArray = GameObject.FindGameObjectsWithTag("rpm");
        List<GameObject> allRpms = new List<GameObject>();
        allRpms.AddRange(allRpmsArray);

        Debug.Log("allrpm count = " + allRpms.Count);
        foreach (GameObject rpm in rpmObjects)
            allRpms.Remove(rpm);

        for (int i = 0; i < allRpms.Count; i++)
        {
            allRpms[i].SetActive(false);
        }

    }

    public void LoadLayout()
    {
        MenuHandler menuHandler = GameObject.FindGameObjectWithTag("MenuObject").GetComponent<MenuHandler>();

        //Save layout is in MenuHandler

        //first of all empty trays
        ButtonManager.EmptyTrays(menuHandler);

        //grab layout data if available from player prefs
        string jsonFoo = PlayerPrefs.GetString(airplaneData.planeType);
        if (System.String.IsNullOrEmpty(jsonFoo))
        {

            //set dials to default
            DefaultLayouts(countryDialBoard);
            return;
        }

        //continue if there is a pref file

        //rebuild json
        Layout layout = JsonUtility.FromJson<Layout>(jsonFoo);

        //check for version change
        if (layout.version != airplaneData.clientVersion)
        {
            //reset all dials :(

            //set dials to default
            Debug.Log("Version change detected");
            DefaultLayouts(countryDialBoard);

            return;
        }

        //apply to dials/positions

        GameObject speedometer = countryDialBoard.transform.Find("Speedometer").gameObject;
        speedometer.GetComponent<RectTransform>().anchoredPosition = layout.speedoPos;
        speedometer.GetComponent<RectTransform>().localScale = new Vector3(layout.speedoScale, layout.speedoScale, 1f);

        if (layout.speedoInTray)
            AddToTrayOnLoad(speedometer, menuHandler);

        GameObject altimeter = countryDialBoard.transform.Find("Altimeter").gameObject;
        altimeter.GetComponent<RectTransform>().anchoredPosition = layout.altPos;
        altimeter.GetComponent<RectTransform>().localScale = new Vector3(layout.altScale, layout.altScale, 1f);


        if (layout.altimeterInTray)
            AddToTrayOnLoad(altimeter, menuHandler);

        if (countryDialBoard.transform.Find("Heading Indicator") != null)
        {
            GameObject headingIndicator = countryDialBoard.transform.Find("Heading Indicator").gameObject;
            headingIndicator.GetComponent<RectTransform>().anchoredPosition = layout.headingPos;
            headingIndicator.GetComponent<RectTransform>().localScale = new Vector3(layout.headingScale, layout.headingScale, 1f);

            if (layout.headingIndicatorInTray)
                AddToTrayOnLoad(headingIndicator, menuHandler);
        }

        if (countryDialBoard.transform.Find("Turn And Bank") != null)
        {
            GameObject turnAndBank = countryDialBoard.transform.Find("Turn And Bank").gameObject;
            turnAndBank.GetComponent<RectTransform>().anchoredPosition = layout.turnAndBankPos;
            turnAndBank.GetComponent<RectTransform>().localScale = new Vector3(layout.turnAndBankScale, layout.turnAndBankScale, 1f);

            if (layout.turnAndBankInTray)
                AddToTrayOnLoad(turnAndBank, menuHandler);

        }

        if (countryDialBoard.transform.Find("Turn Coordinator") != null)
        {

            GameObject turnIndicator = countryDialBoard.transform.Find("Turn Coordinator").gameObject;
            turnIndicator.GetComponent<RectTransform>().anchoredPosition = layout.turnIndicatorPos;
            turnIndicator.GetComponent<RectTransform>().localScale = new Vector3(layout.turnIndicatorScale, layout.turnIndicatorScale, 1f);

            if (layout.turnIndicatorInTray)
                AddToTrayOnLoad(turnIndicator, menuHandler);
        }

        //both vsi share the same variable - only one vsi per plane

        if (countryDialBoard.transform.Find("VSI Smallest") != null)
        {

            GameObject vsi = countryDialBoard.transform.Find("VSI Smallest").gameObject;
            vsi.GetComponent<RectTransform>().anchoredPosition = layout.vsiSmallestPos;
            vsi.GetComponent<RectTransform>().localScale = new Vector3(layout.vsiSmallestScale, layout.vsiSmallestScale, 1f);

            if (layout.vsiSmallestInTray)
                AddToTrayOnLoad(vsi, menuHandler);
        }

        if (countryDialBoard.transform.Find("VSI Small") != null)
        {

            GameObject vsi = countryDialBoard.transform.Find("VSI Small").gameObject;
            vsi.GetComponent<RectTransform>().anchoredPosition = layout.vsiSmallPos;
            vsi.GetComponent<RectTransform>().localScale = new Vector3(layout.vsiSmallScale, layout.vsiSmallScale, 1f);

            if (layout.vsiSmallInTray)
                AddToTrayOnLoad(vsi, menuHandler);
        }

        //both vsi share the same variable - only one vsi per plane
        if (countryDialBoard.transform.Find("VSI Large") != null)
        {

            GameObject vsi = countryDialBoard.transform.Find("VSI Large").gameObject;
            vsi.GetComponent<RectTransform>().anchoredPosition = layout.vsiLargePos;
            vsi.GetComponent<RectTransform>().localScale = new Vector3(layout.vsiLargeScale, layout.vsiLargeScale, 1f);

            if (layout.vsiLargeInTray)
                AddToTrayOnLoad(vsi, menuHandler);
        }

        if (countryDialBoard.transform.Find("Artificial Horizon") != null)
        {

            GameObject artificialHorizon = countryDialBoard.transform.Find("Artificial Horizon").gameObject;
            artificialHorizon.GetComponent<RectTransform>().anchoredPosition = layout.artificialHorizonPos;
            artificialHorizon.GetComponent<RectTransform>().localScale = new Vector3(layout.artificialHorizonScale, layout.artificialHorizonScale, 1f);

            if (layout.artificialHorizonInTray)
                AddToTrayOnLoad(artificialHorizon, menuHandler);
        }

        if (countryDialBoard.transform.Find("Repeater Compass") != null)
        {

            GameObject repeaterCompass = countryDialBoard.transform.Find("Repeater Compass").gameObject;
            repeaterCompass.GetComponent<RectTransform>().anchoredPosition = layout.repeaterCompassPos;
            repeaterCompass.GetComponent<RectTransform>().localScale = new Vector3(layout.repeaterCompassScale, layout.repeaterCompassScale, 1f);

            if (layout.repeaterCompassInTray)
                AddToTrayOnLoad(repeaterCompass, menuHandler);
        }

        if (countryDialBoard.transform.Find("Repeater Compass Alternate") != null)
        {
            GameObject repeaterCompassAlternate = countryDialBoard.transform.Find("Repeater Compass Alternate").gameObject;
            //using non alternate variables because we won't have two compasses 
            repeaterCompassAlternate.GetComponent<RectTransform>().anchoredPosition = layout.repeaterCompassAlternatePos;
            repeaterCompassAlternate.GetComponent<RectTransform>().localScale = new Vector3(layout.repeaterCompassAlternateScale, layout.repeaterCompassAlternateScale, 1f);

            if (layout.repeaterCompassAlternateInTray)
                AddToTrayOnLoad(repeaterCompassAlternate, menuHandler);
        }


        for (int i = 0; i < rpmObjects.Count; i++)
        {
            rpmObjects[i].GetComponent<RectTransform>().anchoredPosition = layout.rpmPos[i];
            rpmObjects[i].GetComponent<RectTransform>().localScale = new Vector3(layout.rpmScale[i], layout.rpmScale[i], 1f);

            if (layout.rpmInTray[i])
                AddToTrayOnLoad(rpmObjects[i], menuHandler);
        }

    }

    public void SaveLayout()
    {
        //use class to write with json // https://forum.unity.com/threads/how-would-i-do-the-following-in-playerprefs.397516/#post-2595609
        Layout layout = new Layout();
        layout.planeType = airplaneData.planeType;

        //save version to cover for updates
        layout.version = airplaneData.clientVersion;

        //look for dial on dashboard - original parent        

        if (countryDialBoard.transform.Find("Speedometer") != null)
        {
            layout.speedoPos = countryDialBoard.transform.Find("Speedometer").GetComponent<RectTransform>().anchoredPosition;
            layout.speedoScale = countryDialBoard.transform.Find("Speedometer").GetComponent<RectTransform>().localScale.x;
        }
        else
            //if we don't find it, look for it in the tray
            DialInTray("Speedometer", layout);


        if (countryDialBoard.transform.Find("Altimeter") != null)
        {
            layout.altPos = countryDialBoard.transform.Find("Altimeter").GetComponent<RectTransform>().anchoredPosition;
            layout.altScale = countryDialBoard.transform.Find("Altimeter").GetComponent<RectTransform>().localScale.x;
        }
        else
            DialInTray("Altimeter", layout);


        if (countryDialBoard.transform.Find("Heading Indicator") != null)
        {
            layout.headingPos = countryDialBoard.transform.Find("Heading Indicator").GetComponent<RectTransform>().anchoredPosition;
            layout.headingScale = countryDialBoard.transform.Find("Heading Indicator").GetComponent<RectTransform>().localScale.x;
        }
        else
            DialInTray("Heading Indicator", layout);


        if (countryDialBoard.transform.Find("Turn And Bank") != null)
        {
            layout.turnAndBankPos = countryDialBoard.transform.Find("Turn And Bank").GetComponent<RectTransform>().anchoredPosition;
            layout.turnAndBankScale = countryDialBoard.transform.Find("Turn And Bank").GetComponent<RectTransform>().localScale.x;
        }
        else
            DialInTray("Turn And Bank", layout);


        if (countryDialBoard.transform.Find("Turn Coordinator") != null)
        {
            layout.turnIndicatorPos = countryDialBoard.transform.Find("Turn Coordinator").GetComponent<RectTransform>().anchoredPosition;
            layout.turnIndicatorScale = countryDialBoard.transform.Find("Turn Coordinator").GetComponent<RectTransform>().localScale.x;
        }
        else
            DialInTray("Turn Coordinator", layout);

        if (countryDialBoard.transform.Find("VSI Smallest") != null)
        {
            layout.vsiSmallestPos = countryDialBoard.transform.Find("VSI Smallest").GetComponent<RectTransform>().anchoredPosition;
            layout.vsiSmallestScale = countryDialBoard.transform.Find("VSI Smallest").GetComponent<RectTransform>().localScale.x;
        }

        else
            DialInTray("VSI Smallest", layout);


        if (countryDialBoard.transform.Find("VSI Small") != null)
        {
            layout.vsiSmallPos = countryDialBoard.transform.Find("VSI Small").GetComponent<RectTransform>().anchoredPosition;
            layout.vsiSmallScale = countryDialBoard.transform.Find("VSI Small").GetComponent<RectTransform>().localScale.x;
        }

        else
            DialInTray("VSI Small", layout);


        if (countryDialBoard.transform.Find("VSI Large") != null)
        {
            layout.vsiLargePos = countryDialBoard.transform.Find("VSI Large").GetComponent<RectTransform>().anchoredPosition;
            layout.vsiLargeScale = countryDialBoard.transform.Find("VSI Large").GetComponent<RectTransform>().localScale.x;
        }
        else
            DialInTray("VSI Large", layout);


        if (countryDialBoard.transform.Find("Artificial Horizon") != null)
        {
            layout.artificialHorizonPos = countryDialBoard.transform.Find("Artificial Horizon").GetComponent<RectTransform>().anchoredPosition;
            layout.artificialHorizonScale = countryDialBoard.transform.Find("Artificial Horizon").GetComponent<RectTransform>().localScale.x;
        }
        else
            DialInTray("Artificial Horizon", layout);


        if (countryDialBoard.transform.Find("Repeater Compass") != null)
        {
            layout.repeaterCompassPos = countryDialBoard.transform.Find("Repeater Compass").GetComponent<RectTransform>().anchoredPosition;
            layout.repeaterCompassScale = countryDialBoard.transform.Find("Repeater Compass").GetComponent<RectTransform>().localScale.x;
        }
        else
            DialInTray("Repeater Compass", layout);

        if (countryDialBoard.transform.Find("Repeater Compass Alternate") != null)
        {
            layout.repeaterCompassAlternatePos = countryDialBoard.transform.Find("Repeater Compass Alternate").GetComponent<RectTransform>().anchoredPosition;
            layout.repeaterCompassAlternateScale = countryDialBoard.transform.Find("Repeater Compass Alternate").GetComponent<RectTransform>().localScale.x;
        }
        else
            DialInTray("Repeater Compass Alternate", layout);

        //rpms

        for (int i = 0; i < rpmObjects.Count; i++)
        {
            //if on dial board
            if(rpmObjects[i].transform.parent = countryDialBoard.transform)
            {
                layout.rpmPos[i] = rpmObjects[i].GetComponent<RectTransform>().anchoredPosition;
                layout.rpmScale[i] = rpmObjects[i].GetComponent<RectTransform>().localScale.x;
            }
            //or in tray
            else
                DialInTray(rpmObjects[i].name, layout);
        }

            /*
            if (airplaneData.planeAttributes.rpmType == RpmType.A)
            {
                if (countryDialBoard.transform.Find("RPM A " + i.ToString()) != null)
                {
                    layout.rpmPos[i] = countryDialBoard.transform.Find("RPM A " + i.ToString()).GetComponent<RectTransform>().anchoredPosition;
                    layout.rpmScale[i] = countryDialBoard.transform.Find("RPM A " + i.ToString()).GetComponent<RectTransform>().localScale.x;
                }
                else
                    DialInTray("RPM A " + i.ToString(), layout);
            }

            else if (airplaneData.planeAttributes.rpmType == RpmType.B)
            {
                if (countryDialBoard.transform.Find("RPM B " + i.ToString()) != null)
                {
                    layout.rpmPos[i] = countryDialBoard.transform.Find("RPM B " + i.ToString()).GetComponent<RectTransform>().anchoredPosition;
                    layout.rpmScale[i] = countryDialBoard.transform.Find("RPM B " + i.ToString()).GetComponent<RectTransform>().localScale.x;
                }
                else
                    DialInTray("RPM B " + i.ToString(), layout);
            }
            */
        
        
        //pack with json utility
        string jsonFoo = JsonUtility.ToJson(layout);

        //save packed string to player preferences (unity)
        PlayerPrefs.SetString(airplaneData.planeType, jsonFoo);
        PlayerPrefs.Save();

    }

    void DialInTray(string name, Layout layout)
    {
        //grab from menu handler
        List<GameObject> dialsInTray = menuHandler.dialsInTray;

        //find passed string's game object in tray, and add it to the layout class so we can save it
        for (int i = 0; i < dialsInTray.Count; i++)
        {
            switch (name)
            {
                case "Speedometer":
                    layout.speedoPos = dialsInTray[i].GetComponent<RectTransform>().anchoredPosition;
                    layout.speedoScale = dialsInTray[i].GetComponent<RectTransform>().localScale.x;
                    layout.speedoInTray = true;
                    break;

                case "Altimeter":
                    layout.altPos = dialsInTray[i].GetComponent<RectTransform>().anchoredPosition;
                    layout.altScale = dialsInTray[i].GetComponent<RectTransform>().localScale.x;
                    layout.altimeterInTray = true;
                    break;

                case "Heading Indicator":
                    layout.headingPos = dialsInTray[i].GetComponent<RectTransform>().anchoredPosition;
                    layout.headingScale = dialsInTray[i].GetComponent<RectTransform>().localScale.x;
                    layout.headingIndicatorInTray = true;
                    break;

                case "Turn And Bank":
                    layout.turnAndBankPos = dialsInTray[i].GetComponent<RectTransform>().anchoredPosition;
                    layout.turnAndBankScale = dialsInTray[i].GetComponent<RectTransform>().localScale.x;
                    layout.turnAndBankInTray = true;
                    break;

                case "Turn Coordinator":
                    layout.turnIndicatorPos = dialsInTray[i].GetComponent<RectTransform>().anchoredPosition;
                    layout.turnIndicatorScale = dialsInTray[i].GetComponent<RectTransform>().localScale.x;
                    layout.turnIndicatorInTray = true;
                    break;

                case "VSI Smallest":
                    layout.vsiSmallestPos = dialsInTray[i].GetComponent<RectTransform>().anchoredPosition;
                    layout.vsiSmallestScale = dialsInTray[i].GetComponent<RectTransform>().localScale.x;
                    layout.vsiSmallestInTray = true;
                    break;

                case "VSI Small":
                    layout.vsiSmallPos = dialsInTray[i].GetComponent<RectTransform>().anchoredPosition;
                    layout.vsiSmallScale = dialsInTray[i].GetComponent<RectTransform>().localScale.x;
                    layout.vsiSmallInTray = true;
                    break;

                case "VSI Large":
                    layout.vsiLargePos = dialsInTray[i].GetComponent<RectTransform>().anchoredPosition;
                    layout.vsiLargeScale = dialsInTray[i].GetComponent<RectTransform>().localScale.x;
                    layout.vsiLargeInTray = true;
                    break;

                case "Artificial Horizon":
                    layout.artificialHorizonPos = dialsInTray[i].GetComponent<RectTransform>().anchoredPosition;
                    layout.artificialHorizonScale = dialsInTray[i].GetComponent<RectTransform>().localScale.x;
                    layout.artificialHorizonInTray = true;
                    break;

                case "Repeater Compass":
                    layout.repeaterCompassPos = dialsInTray[i].GetComponent<RectTransform>().anchoredPosition;
                    layout.repeaterCompassScale = dialsInTray[i].GetComponent<RectTransform>().localScale.x;
                    layout.repeaterCompassInTray = true;
                    break;

                case "Repeater Compass Alternate":
                    layout.repeaterCompassAlternatePos = dialsInTray[i].GetComponent<RectTransform>().anchoredPosition;
                    layout.repeaterCompassAlternateScale = dialsInTray[i].GetComponent<RectTransform>().localScale.x;
                    layout.repeaterCompassAlternateInTray = true;
                    break;

                    /*
                //Needs redesign!
                case "RPM A 0":
                    layout.rpmAPos[0] = dialsInTray[i].GetComponent<RectTransform>().anchoredPosition;
                    layout.rpmAScale[0] = dialsInTray[i].GetComponent<RectTransform>().localScale.x;
                    layout.rpmAInTray[0] = true;
                    break;

                case "RPM A 1":
                    layout.rpmAPos[1] = dialsInTray[i].GetComponent<RectTransform>().anchoredPosition;
                    layout.rpmAScale[1] = dialsInTray[i].GetComponent<RectTransform>().localScale.x;
                    layout.rpmAInTray[1] = true;
                    break;

                case "RPM A 2":
                    layout.rpmAPos[2] = dialsInTray[i].GetComponent<RectTransform>().anchoredPosition;
                    layout.rpmAScale[2] = dialsInTray[i].GetComponent<RectTransform>().localScale.x;
                    layout.rpmAInTray[2] = true;
                    break;

                case "RPM B 0":
                    layout.rpmBPos[0] = dialsInTray[i].GetComponent<RectTransform>().anchoredPosition;
                    layout.rpmBScale[0] = dialsInTray[i].GetComponent<RectTransform>().localScale.x;
                    layout.rpmBInTray[0] = true;
                    break;

                case "RPM B 1":
                    layout.rpmBPos[1] = dialsInTray[i].GetComponent<RectTransform>().anchoredPosition;
                    layout.rpmBScale[1] = dialsInTray[i].GetComponent<RectTransform>().localScale.x;
                    layout.rpmBInTray[1] = true;
                    break;

                case "RPM B 2":
                    layout.rpmBPos[2] = dialsInTray[i].GetComponent<RectTransform>().anchoredPosition;
                    layout.rpmBScale[2] = dialsInTray[i].GetComponent<RectTransform>().localScale.x;
                    layout.rpmBInTray[2] = true;
                    break;
                    */
            }
        }
    }

    static void DefaultLayouts(GameObject dialsPrefab)
    {
        //Programtically sort default layouts, so if there is an update, i don't need to create a prefab layout

        //organise dials depending on how many are available
        //we need to know the total amount of active dials before we continue
        List<GameObject> activeDials = new List<GameObject>();
        for (int i = 0; i < dialsPrefab.transform.childCount; i++)
            if (dialsPrefab.transform.GetChild(i).gameObject.activeSelf)
                activeDials.Add(dialsPrefab.transform.GetChild(i).gameObject);


        //find out if we ned to scale dials to fit them all in the screen (happens if 7 or more dials)
        //length of top will be the longest
        float f = activeDials.Count;
        //round half of count upwards and convert to int. Mathf.Ceil rounds up. If on a whole number, it doesn't round up //https://docs.unity3d.com/ScriptReference/Mathf.Ceil.html
        //half of count because there are two rows
        int longestRow = (int)Mathf.Ceil(f / 2);
        longestRow *= 300;//300 default step between dials

        GameObject canvasObject = GameObject.FindGameObjectWithTag("Canvas");
        //if longer than the canvas width
        //UnityEngine.Debug.Log("longest row = " + longestRow);
        //UnityEngine.Debug.Log("canvas X = " + canvasObject.GetComponent<RectTransform>().rect.width);

        float scale = 1f;
        if (longestRow > canvasObject.GetComponent<RectTransform>().rect.width)
        {
            //UnityEngine.Debug.Log("row longer than canvas");

            //use this ratio for all positional calculations
            scale = canvasObject.GetComponent<RectTransform>().rect.width / longestRow;

        }


        //split in to two rows, if odd number, put more on the top
        for (int i = 0; i < activeDials.Count; i++)
        {
            //ternary statement            
            int odd = activeDials.Count % 2 != 0 ? 1 : 0;

            //if odd, we will add one extra to the top row
            if (i < activeDials.Count / 2 + odd)
            {
                //0 0
                //150 1
                //300 2

                int x = ((int)((activeDials.Count - 1) / 2)) * -150;
                //then add step
                int step = 300 * (i);
                x += step;

                int y = 150;

                //scale and round and convert to int for position
                float xFloat = x * scale;
                x = (int)(Mathf.Round(xFloat));
                float yFloat = y * scale;
                y = (int)(Mathf.Round(yFloat));

                activeDials[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);


            }
            else
            {
                //starting point //from whats left 
                //use "odd" to nudge in to position
                int diff = activeDials.Count - 1 + odd - (activeDials.Count / 2);
                int x = ((int)(diff));
                x *= -150;
                //then add step
                int step = 300 * (i - (activeDials.Count / 2));
                x += step;

                int y = -150;

                //scale and round and convert to int 
                float xFloat = x * scale;
                x = (int)(Mathf.Round(xFloat));
                float yFloat = y * scale;
                y = (int)(Mathf.Round(yFloat));

                activeDials[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
            }

            //scale dial
            activeDials[i].transform.localScale *= scale;
        }
    }

    void AddToTrayOnLoad(GameObject dial, MenuHandler menuHandler)
    {
        //USe button manager class to store dial in tray
        ButtonManager.PutDialInTray(dial, menuHandler);
    }


}
