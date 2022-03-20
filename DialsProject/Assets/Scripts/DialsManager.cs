using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialsManager : MonoBehaviour
{
    //This class manages the loading and saving of layouts and population of dials on plane change
    public AirplaneData airplaneData;
    public MenuHandler menuHandler;
    public SlaveManager slaveManager;
    public UDPClient udpClient;
    public GameObject countryDialBoard;

    public List<GameObject> rpmObjects = new List<GameObject>();
    public List<GameObject> manifoldObjects = new List<GameObject>();
    public GameObject speedometer;


    //flagged from open layout button press
    public bool openLayoutOnLoad;

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
        // if we detect a plane change
        if (airplaneData.planeType != airplaneData.planeTypePrevious)
        {
            //check if layout panel is open, save and close before we proceed
            //simulate accept click if there was a plane loaded
            if (menuHandler.layoutOpen && airplaneData.planeAttributes != null && airplaneData.planeAttributes.country != Country.UNDEFINED)
            {
                Debug.Log("forcing menu accept");
                menuHandler.AcceptLayoutClick();
            }

            //construct country and available dials in to planeAttributes class/struct
            airplaneData.planeAttributes = PlaneDataFromName.AttributesFromName(airplaneData.planeType);

            if (airplaneData.planeAttributes.country == Country.UNDEFINED)
            {
                //remove dial board if any 
                //remove any existing dials board prefab in scene
                if (countryDialBoard != null)
                    Destroy(countryDialBoard);

                return;
            }

            SwitchDialBoardFromCountry(airplaneData.planeAttributes.country);

            //switch off any unavailable dials to this plane
            DeactivateUnavailableDials(countryDialBoard, airplaneData.planeType, airplaneData.planeAttributes, rpmObjects);

            //asign correct needle to rotate scripts depending on what plane we have loaded
            AsignNeedles();

            Markings(airplaneData);

            if (countryDialBoard != null)
                LoadManager.LoadLayout(airplaneData, this);
            else
            //close layout
            {
                //go back to main page
                menuHandler.layoutPanel.SetActive(false);

                //turn menu button and leds back on
                menuHandler.menuButton.SetActive(true);
                menuHandler.ledParent.SetActive(true);

                menuHandler.layoutOpen = false;
            }

            //once loaded, check to see it it was loaded from the layout dropdown

            if (openLayoutOnLoad)
            {
                //flagged from open 
                Debug.Log("open layout on load");
                Debug.Log("air country = " + airplaneData.planeAttributes.country);
                menuHandler.OpenLayoutClick();
                openLayoutOnLoad = false;
            }
        }
    }

    void Markings(AirplaneData airplaneData)
    {
        //rpm markings
        //p47 28 needs one red removed
        if (airplaneData.planeType == "P-47D-28")
        {
            countryDialBoard.transform.Find("RPM D 0").Find("Markings").Find("Red 2.7").gameObject.SetActive(false);
        }

        //only the B has a red mark, so disable the one on the the "p51 D"
        if (airplaneData.planeType == "P-51D-15")
        {
            countryDialBoard.transform.Find("RPM C 0").Find("Markings").Find("Red 3").gameObject.SetActive(false);
        }

    }

    void AsignNeedles()
    {
        AsignSpeedometer(airplaneData.planeAttributes, countryDialBoard);

        AsignVSI(airplaneData.planeAttributes, countryDialBoard);

        AsignRPM(airplaneData.planeAttributes, countryDialBoard);

        AsignManifold(airplaneData.planeAttributes, countryDialBoard);
    }

    
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



        for (int i = 0; i < rpmObjects.Count; i++)
        {
            //  if (planeAttributes.country ==  Country.RU)
            {
                //Primary needle - most planes have this
                if (rpmObjects[i].transform.Find("Needle Large") != null)
                {
                    GameObject needleLarge = rpmObjects[i].transform.Find("Needle Large").gameObject;
                    countryDialBoard.GetComponent<RotateNeedle>().rpmNeedlesLarge.Add(needleLarge);
                }


                if (planeAttributes.country ==  Country.RU)
                {
                    if (planeAttributes.rpmType == DialVariant.A)
                    {
                        GameObject needleSmall = rpmObjects[i].transform.Find("Needle Small").gameObject;
                        countryDialBoard.GetComponent<RotateNeedle>().rpmNeedlesSmall.Add(needleSmall);
                    }


                    //pe-2
                    if (planeAttributes.country ==  Country.RU && planeAttributes.rpmType == DialVariant.C)
                    {
                        GameObject needleSmall = rpmObjects[i].transform.Find("Needle Small").gameObject;
                        countryDialBoard.GetComponent<RotateNeedle>().rpmNeedlesSmall.Add(needleSmall);
                    }

                }

                if (planeAttributes.country ==  Country.US)
                {
                    //p38 J
                    if (airplaneData.planeType == "P-38J-25")
                    {
                        if (i == 0)
                        {
                            //do both needles and return, p38 has two needles on one dial
                            GameObject needleLeft = rpmObjects[i].transform.Find("Needle Left").gameObject;
                            countryDialBoard.GetComponent<RotateNeedle>().rpmNeedlesLarge.Add(needleLeft);

                            GameObject needleRight = rpmObjects[i].transform.Find("Needle Right").gameObject;
                            countryDialBoard.GetComponent<RotateNeedle>().rpmNeedlesLarge.Add(needleRight);

                            return;
                        }
                    }

                    if (planeAttributes.rpmType == DialVariant.A || planeAttributes.rpmType == DialVariant.D)
                    {
                        GameObject needleSmall = rpmObjects[i].transform.Find("Needle Small").gameObject;
                        countryDialBoard.GetComponent<RotateNeedle>().rpmNeedlesSmall.Add(needleSmall);
                    }
                }
            }
        }
    }

    void AsignManifold(PlaneDataFromName.PlaneAttributes planeAttributes, GameObject countrydialBoard)
    {
        //empty lists first
        countryDialBoard.GetComponent<RotateNeedle>().manifoldNeedlesLarge.Clear();
        countryDialBoard.GetComponent<RotateNeedle>().manifoldNeedlesSmall.Clear();


        for (int i = 0; i < manifoldObjects.Count; i++)
        {
            if (airplaneData.planeType == "P-38J-25" || airplaneData.planeType == "He 111 H-16")
            {
                //p38 J or he 111 h16
                GameObject needleLeft = manifoldObjects[i].transform.Find("Needle Left").gameObject;
                countryDialBoard.GetComponent<RotateNeedle>().manifoldNeedlesLarge.Add(needleLeft);

                GameObject needleRight = manifoldObjects[i].transform.Find("Needle Right").gameObject;
                countryDialBoard.GetComponent<RotateNeedle>().manifoldNeedlesLarge.Add(needleRight);

                //both needles asign and jump out of loop (has 2 engines)
                return;
             
            }
            else
            {
                GameObject needleLarge = manifoldObjects[i].transform.Find("Needle Large").gameObject;
                countryDialBoard.GetComponent<RotateNeedle>().manifoldNeedlesLarge.Add(needleLarge);
            }
        }
    }


    void AsignSpeedometer(PlaneDataFromName.PlaneAttributes planeAttributes, GameObject countrydialBoard)
    {
        speedometer = GameObject.FindGameObjectWithTag("speedometer");
        countryDialBoard.GetComponent<RotateNeedle>().airspeedNeedle = speedometer.transform.Find("Needle Large").transform.gameObject;

    }

    public void SwitchDialBoardFromCountry(Country country)
    {
        //change dials depending on what value we received from the networking component

        //remove any existing dials board prefab in scene
        if (countryDialBoard != null)
            //destroy now so any existing links to variables are immediately re-asigned. If only using "Destroy", there are problems with missing 
            DestroyImmediate(countryDialBoard);

        GameObject canvas = GameObject.FindGameObjectWithTag("Canvas");

        switch (country)
        {
            case  Country.RU:
                //countryDials[0].SetActive(true);
                GameObject RUprefab = Resources.Load("Prefabs/RU") as GameObject;
                countryDialBoard = GameObject.Instantiate(RUprefab, canvas.transform.position, Quaternion.identity, canvas.transform.GetChild(0).transform);
                break;

            case  Country.GER:
                GameObject GERprefab = Resources.Load("Prefabs/GER") as GameObject;
                countryDialBoard = GameObject.Instantiate(GERprefab, canvas.transform.position, Quaternion.identity, canvas.transform.GetChild(0).transform);
                break;

            case  Country.US:
                GameObject USprefab = Resources.Load("Prefabs/US") as GameObject;
                countryDialBoard = GameObject.Instantiate(USprefab, canvas.transform.position, Quaternion.identity, canvas.transform.GetChild(0).transform);
                break;

            case  Country.UK:
                GameObject UKprefab = Resources.Load("Prefabs/UK") as GameObject;
                countryDialBoard = GameObject.Instantiate(UKprefab, canvas.transform.position, Quaternion.identity, canvas.transform.GetChild(0).transform);
                break;

            case  Country.ITA:
                GameObject ITAprefab = Resources.Load("Prefabs/ITA") as GameObject;
                countryDialBoard = GameObject.Instantiate(ITAprefab, canvas.transform.position, Quaternion.identity, canvas.transform.GetChild(0).transform);
                break;

            default:
                countryDialBoard = null;
                break;
        }

        if (countryDialBoard != null)
        {
            //asign variables - can't asign scene variables to prefab in editor
            //asign variables - can't asign scene variables to prefab in editor
            udpClient.rN = countryDialBoard.GetComponent<RotateNeedle>();
            udpClient.rN.buildControl = GameObject.Find("Build Chooser").GetComponent<BuildControl>();
            udpClient.rN.airplaneData = GameObject.Find("Player Plane").GetComponent<AirplaneData>();
            udpClient.rN.udpClient = GameObject.Find("Networking").GetComponent<UDPClient>();

            //Instantiate RPMs
            rpmObjects.Clear();
            
            string rpmString = airplaneData.planeAttributes.rpmType.ToString();

            if (countryDialBoard.transform.Find("RPM " + rpmString) != null)
            {
                //find prefab outside of loop
                GameObject rpm = countryDialBoard.transform.Find("RPM " + rpmString).gameObject;

                int dialsToInstantiate = airplaneData.planeAttributes.engines;
                //some plane have two needles one dial, only create one in this instance
                if (airplaneData.planeType == "P-38J-25")
                    dialsToInstantiate = 1;

                for (int i = 0; i < dialsToInstantiate; i++)
                {
                    //create instance variable if we need to duplicate
                    GameObject rpmInstance = rpm;
                    if (i > 0)
                    {
                        //duplicate if we have more than one engine
                        rpmInstance = Instantiate(rpm, rpm.transform.position, Quaternion.identity, countryDialBoard.transform);
                        //place after found dial
                        rpmInstance.transform.SetSiblingIndex(rpm.transform.GetSiblingIndex() +1);

                    }

                    rpmInstance.transform.name = "RPM " + airplaneData.planeAttributes.rpmType.ToString() + " " + i.ToString();
                    rpmObjects.Add(rpmInstance);
                }
            }

            //instantiate manifolds

            manifoldObjects.Clear();
            
            string manifoldString = airplaneData.planeAttributes.manifoldType.ToString();

            if (countryDialBoard.transform.Find("Manifold " + manifoldString) != null)
            {
                //find prefab outside of loop
                GameObject manifold = countryDialBoard.transform.Find("Manifold " + manifoldString).gameObject;

                int dialsToInstantiate = airplaneData.planeAttributes.engines;
                //some plane have two needles one dial, only create one in this instance
                if (airplaneData.planeType == "P-38J-25" || airplaneData.planeType == "He 111 H-16")
                    dialsToInstantiate = 1;

                for (int i = 0; i < dialsToInstantiate; i++)
                {
                    //create instance variable if we need to duplicate
                    GameObject manifoldInstance = manifold;
                    if (i > 0)
                    {

                        //duplicate if we have more than one engine
                        manifoldInstance = Instantiate(manifold, manifold.transform.position, Quaternion.identity, countryDialBoard.transform);
                        manifoldInstance.transform.SetSiblingIndex(manifoldInstance.transform.GetSiblingIndex() +1);

                    }
                    manifoldInstance.transform.name = "Manifold " + airplaneData.planeAttributes.manifoldType.ToString() + " " + i.ToString();

                    manifoldObjects.Add(manifoldInstance);
                }
            }
        }
    }

    //static to refactor to new class - to do
    void DeactivateUnavailableDials(GameObject countryDialBoard, string planeName, PlaneDataFromName.PlaneAttributes planeAttributes, List<GameObject> rpmObjects)
    {
        //check what dials are available and switch off as needed

        GameObject[] speedos = GameObject.FindGameObjectsWithTag("speedometer");
        for (int i = 0; i < speedos.Length; i++)
        {
            if (speedos[i].name != "Speedometer " + planeAttributes.speedometer.ToString())
                speedos[i].SetActive(false);

        }

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

        foreach (GameObject rpm in rpmObjects)
            allRpms.Remove(rpm);

        for (int i = 0; i < allRpms.Count; i++)
        {
            allRpms[i].SetActive(false);
        }


        GameObject[] allManifoldsArray = GameObject.FindGameObjectsWithTag("manifold");
        List<GameObject> allManifolds = new List<GameObject>();
        allManifolds.AddRange(allManifoldsArray);

        foreach (GameObject manifold in manifoldObjects)
            allManifolds.Remove(manifold);

        for (int i = 0; i < allManifolds.Count; i++)
        {
            allManifolds[i].SetActive(false);
        }

    }

    public void SaveLayout()
    {
        //use class to write with json // https://forum.unity.com/threads/how-would-i-do-the-following-in-playerprefs.397516/#post-2595609
        Layout layout = new Layout();
        layout.planeType =  airplaneData.planeType;

        //save version to cover for updates
        layout.version = airplaneData.clientVersion;

        //look for dial on dashboard - original parent        


        if (!menuHandler.dialsInTray.Contains(speedometer))
        {
            layout.speedoPos = speedometer.GetComponent<RectTransform>().anchoredPosition;
            layout.speedoScale = speedometer.GetComponent<RectTransform>().localScale.x;
        }
        else
            //if we don't find it, look for it in the tray
            SpeedoInTray(layout);


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
            if (rpmObjects[i].transform.parent == countryDialBoard.transform)
            {
                layout.rpmPos[i] = rpmObjects[i].GetComponent<RectTransform>().anchoredPosition;
                layout.rpmScale[i] = rpmObjects[i].GetComponent<RectTransform>().localScale.x;
            }
            //or in tray
            else
                //note - RPMInTray function
                RPMInTray(layout, i, rpmObjects[i]);
        }

        //manifolds
        for (int i = 0; i < manifoldObjects.Count; i++)
        {
            //if on dial board
            if (manifoldObjects[i].transform.parent == countryDialBoard.transform)
            {
                layout.manifoldPos[i] = manifoldObjects[i].GetComponent<RectTransform>().anchoredPosition;
                layout.manifoldScale[i] = manifoldObjects[i].GetComponent<RectTransform>().localScale.x;
            }
            //or in tray
            else
                //note - RPMInTray function
                ManifoldInTray(layout, i, manifoldObjects[i]);
        }

        //pack with json utility
        string jsonFoo = JsonUtility.ToJson(layout);

        //save master/client id as first char in string, then save plane name

        //Debug.Log("saving layout");

        //save packed string to player preferences (unity)
        //save with id to know if user addded a a second window - if no id, save only plane name ( this will be the master client)
#if UNITY_ANDROID
        string key = airplaneData.planeType;
#else
        string key = "layout " + slaveManager.id + " " + airplaneData.planeType;
#endif

        //Debug.Log("saving key = " + key);


        PlayerPrefs.SetString(key, jsonFoo);
        PlayerPrefs.Save();

    }

    public void DeleteLayout()
    {
        Debug.Log("Deleting = " + airplaneData.planeType);
        PlayerPrefs.DeleteKey(airplaneData.planeType);
        //PlayerPrefs.Save();


        //put all dials back to country board
        for (int i = 0; i < menuHandler.dialsInTray.Count; i++)
        {
            menuHandler.dialsInTray[i].transform.parent = countryDialBoard.transform;
        }

        //and call default
        LoadManager.DefaultLayouts(countryDialBoard);

        //make sure all ui is on

        for (int i = 0; i < menuHandler.dialsInTray.Count; i++)
        {
            ButtonManager.IconsOn(menuHandler.dialsInTray[i]);

        }

        //now reset list
        menuHandler.dialsInTray.Clear();

        //turn trays off, they will be empty now
        for (int i = 0; i < menuHandler.trayObjects.Count; i++)
        {
            menuHandler.trayObjects[i].SetActive(false);

        }

    }

    void RPMInTray(Layout layout, int i, GameObject rpm)
    {
        //slightly different for multiple dials

        layout.rpmPos[i] = rpm.GetComponent<RectTransform>().anchoredPosition;
        layout.rpmScale[i] = rpm.GetComponent<RectTransform>().localScale.x;
        layout.rpmInTray[i] = true;

    }

    void ManifoldInTray(Layout layout, int i, GameObject manifold)
    {
        //slightly different for multiple dials

        layout.manifoldPos[i] = manifold.GetComponent<RectTransform>().anchoredPosition;
        layout.manifoldScale[i] = manifold.GetComponent<RectTransform>().localScale.x;
        layout.manifoldInTray[i] = true;

    }

    void SpeedoInTray(Layout layout)
    {
        layout.speedoPos = speedometer.GetComponent<RectTransform>().anchoredPosition;
        layout.speedoScale = speedometer.GetComponent<RectTransform>().localScale.x;
        layout.speedoInTray = true;
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
            }
        }
    }


    //helpers

    public static List<GameObject> ActiveDials(GameObject dialsPrefab)
    {
        List<GameObject> activeDials = new List<GameObject>();
        for (int i = 0; i < dialsPrefab.transform.childCount; i++)
            if (dialsPrefab.transform.GetChild(i).gameObject.activeSelf)
                activeDials.Add(dialsPrefab.transform.GetChild(i).gameObject);

        return activeDials;
    }

    public static float DefaultDialScale(List<GameObject> activeDials)
    { //find out if we ned to scale dials to fit them all in the screen (happens if 7 or more dials)
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

        return scale;
    }



}
