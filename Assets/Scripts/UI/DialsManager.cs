using System;
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
    public List<GameObject> waterTempObjects = new List<GameObject>();
    public List<GameObject> oilTempInObjects = new List<GameObject>();
    public List<GameObject> oilTempOutObjects = new List<GameObject>();
    public List<GameObject> oilTempComboObjects = new List<GameObject>();
    public List<GameObject> oilTempPressureObjects = new List<GameObject>();
    public List<GameObject> cylinderHeadObjects = new List<GameObject>();
    public List<GameObject> carbTempObjects = new List<GameObject>();
    public List<GameObject> fuelObjects = new List<GameObject>();
    public GameObject speedometer;
    public GameObject turnIndicator;
    public GameObject vsi;
    public GameObject artificialHorizon;
    public GameObject headingIndicator;

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

            InstantiateDials();
        }
    }

    private void InstantiateDials()
    {
        SwitchDialBoardFromCountry(airplaneData.planeAttributes.country);

        //switch off any unavailable dials to this plane
        DeactivateUnavailableDials(countryDialBoard, airplaneData.planeAttributes);

        //asign correct needle to rotate scripts depending on what plane we have loaded
        AsignNeedles();

        Markings(airplaneData);

        //plane specific functions
        P51FaceSwitch();

        MosquitoFaceSwitch();

        ME410ManifoldRotate();

        LeftRightNeedleSwitch();
        //end of plane specifics

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
            menuHandler.OpenLayoutClick();
            openLayoutOnLoad = false;
        }
    }

    private void ME410ManifoldRotate()
    {
        if (airplaneData.planeType == "Me 410 A-1")
            //dial is puin 90 degrees in cockpit
            manifoldObjects[0].transform.Find("Dial").Find("Face").transform.rotation *= Quaternion.Euler(0, 0, 90);
    }

    private void MosquitoFaceSwitch()
    {
        if (airplaneData.planeType == "Mosquito F.B. Mk.VI ser.2")
        {
            if (airplaneData.engineModification == 1)
            {
                for (int i = 0; i < manifoldObjects.Count; i++)
                {
                    manifoldObjects[i].transform.Find("Dial").GetChild(0).gameObject.SetActive(false);
                    manifoldObjects[i].transform.Find("Dial").GetChild(3).gameObject.SetActive(false);

                    manifoldObjects[i].transform.Find("Dial").GetChild(1).gameObject.SetActive(true);
                    manifoldObjects[i].transform.Find("Dial").GetChild(4).gameObject.SetActive(true);
                }
            }
        }
    }

    private void LeftRightNeedleSwitch()
    {
        if (airplaneData.planeType == "Hs 129 B-2" || airplaneData.planeType == "Me 410 A-1")
        {
            //needles need re-order so R is underneath
            manifoldObjects[0].transform.Find("Dial").Find("Needle Left").transform.SetAsLastSibling();
        }
    }

    void Markings(AirplaneData airplaneData)
    {
        //rpm markings
        //p47 28 needs one red removed
        if (airplaneData.planeType == "P-47D-28")
        {
            countryDialBoard.transform.Find("RPM D 0").Find("Dial").Find("Markings").Find("Red 2.7").gameObject.SetActive(false);
        }

        //only the B has a red mark, so disable the one on the the "p51 D"
        if (airplaneData.planeType == "P-51D-15")
        {
            countryDialBoard.transform.Find("RPM C 0").Find("Dial").Find("Markings").Find("Red 3").gameObject.SetActive(false);
        }
    }

    void P51FaceSwitch()
    {
        //if 150 octane fuel switch
        if (airplaneData.planeType == "P-51D-15" || airplaneData.planeType == "P-51B-5")
        {
            if (airplaneData.engineModification == 1)
            {
                for (int i = 0; i < manifoldObjects.Count; i++)
                {
                    manifoldObjects[i].transform.Find("Dial").GetChild(0).gameObject.SetActive(false);
                    manifoldObjects[i].transform.Find("Dial").GetChild(1).gameObject.SetActive(true);
                }
            }

        }
    }

    void AsignNeedles()
    {
        AsignSpeedometer();

        AsignVSI();

        AsignRPM();

        AsignManifold();

        AsignTurnCoordinator();

        AsignHorizon();

        AsignHeadingIndicator();

        AsignWaterTemp();

        AsignOilTempIn();

        AsignOilTempOut();

        AsignOilTempPressure();

        AsignOilTempCombo();

        AsignCylinderHead();

        AsignCarbAir();
    }

    private void AsignCarbAir()
    {
        //empty lists first
        countryDialBoard.GetComponent<RotateNeedle>().carbTempNeedles.Clear();
        if (airplaneData.planeType == "C-47A" || airplaneData.planeType == "A-20B" || airplaneData.planeType == "P-38J-25")
        {
            GameObject needleLargeL = carbTempObjects[0].transform.Find("Dial").Find("Needle Large L").gameObject;
            countryDialBoard.GetComponent<RotateNeedle>().carbTempNeedles.Add(needleLargeL);
            GameObject needleLargeR = carbTempObjects[0].transform.Find("Dial").Find("Needle Large R").gameObject;
            countryDialBoard.GetComponent<RotateNeedle>().carbTempNeedles.Add(needleLargeR);

            return;
        }
        for (int i = 0; i < carbTempObjects.Count; i++)
        {
            GameObject needleLarge = carbTempObjects[i].transform.Find("Dial").Find("Needle Large").gameObject;
            countryDialBoard.GetComponent<RotateNeedle>().carbTempNeedles.Add(needleLarge);
        }
    }

    private void AsignCylinderHead()
    {
        //empty lists first
        countryDialBoard.GetComponent<RotateNeedle>().cylinderHeadNeedles.Clear();
        if (airplaneData.planeType == "C-47A" || airplaneData.planeType == "A-20B")
        {
            GameObject needleLargeL = cylinderHeadObjects[0].transform.Find("Dial").Find("Needle Large L").gameObject;
            countryDialBoard.GetComponent<RotateNeedle>().cylinderHeadNeedles.Add(needleLargeL);
            GameObject needleLargeR = cylinderHeadObjects[0].transform.Find("Dial").Find("Needle Large R").gameObject;
            countryDialBoard.GetComponent<RotateNeedle>().cylinderHeadNeedles.Add(needleLargeR);

            return;
        }

        for (int i = 0; i < cylinderHeadObjects.Count; i++)
        {
            GameObject needleLarge = cylinderHeadObjects[i].transform.Find("Dial").Find("Needle Large").gameObject;
            countryDialBoard.GetComponent<RotateNeedle>().cylinderHeadNeedles.Add(needleLarge);
        }
    }

    private void AsignOilTempCombo()
    {
        //empty lists first
        countryDialBoard.GetComponent<RotateNeedle>().oilTempComboNeedles.Clear();

        for (int i = 0; i < oilTempComboObjects.Count; i++)
        {
            GameObject needleIn = oilTempComboObjects[i].transform.Find("Dial").Find("Needle In").gameObject;
            countryDialBoard.GetComponent<RotateNeedle>().oilTempComboNeedles.Add(needleIn);
            GameObject needleOut = oilTempComboObjects[i].transform.Find("Dial").Find("Needle Out").gameObject;
            countryDialBoard.GetComponent<RotateNeedle>().oilTempComboNeedles.Add(needleOut);
        }
    }

    private void AsignOilTempPressure()
    {
        //empty lists first
        countryDialBoard.GetComponent<RotateNeedle>().oilTempPressureNeedles.Clear();

        for (int i = 0; i < oilTempPressureObjects.Count; i++)
        {
            GameObject needleLarge = oilTempPressureObjects[i].transform.Find("Dial").Find("Needle Large").gameObject;
            countryDialBoard.GetComponent<RotateNeedle>().oilTempPressureNeedles.Add(needleLarge);
        }
    }

    private void AsignOilTempOut()
    {
        //empty lists first
        countryDialBoard.GetComponent<RotateNeedle>().oilTempOutNeedles.Clear();
        if (airplaneData.planeType == "C-47A" || airplaneData.planeType == "A-20B")
        {
            GameObject needleLargeL = oilTempOutObjects[0].transform.Find("Dial").Find("Needle Large L").gameObject;
            countryDialBoard.GetComponent<RotateNeedle>().oilTempOutNeedles.Add(needleLargeL);
            GameObject needleLargeR = oilTempOutObjects[0].transform.Find("Dial").Find("Needle Large R").gameObject;
            countryDialBoard.GetComponent<RotateNeedle>().oilTempOutNeedles.Add(needleLargeR);

            return;
        }
        for (int i = 0; i < oilTempOutObjects.Count; i++)
        {
            GameObject needleLarge = oilTempOutObjects[i].transform.Find("Dial").Find("Needle Large").gameObject;
            countryDialBoard.GetComponent<RotateNeedle>().oilTempOutNeedles.Add(needleLarge);
        }
    }


    private void AsignOilTempIn()
    {
        //empty lists first
        countryDialBoard.GetComponent<RotateNeedle>().oilTempInNeedles.Clear();
        for (int i = 0; i < oilTempInObjects.Count; i++)
        {
            GameObject needleLarge = oilTempInObjects[i].transform.Find("Dial").Find("Needle Large").gameObject;
            countryDialBoard.GetComponent<RotateNeedle>().oilTempInNeedles.Add(needleLarge);
        }
    }

    private void AsignWaterTemp()
    {
        //empty lists first
        countryDialBoard.GetComponent<RotateNeedle>().waterTempNeedles.Clear();

        if (airplaneData.planeType == "P-38J-25")
        {
            GameObject needleLargeL = waterTempObjects[0].transform.Find("Dial").Find("Needle Large L").gameObject;
            countryDialBoard.GetComponent<RotateNeedle>().waterTempNeedles.Add(needleLargeL);
            GameObject needleLargeR = waterTempObjects[0].transform.Find("Dial").Find("Needle Large R").gameObject;
            countryDialBoard.GetComponent<RotateNeedle>().waterTempNeedles.Add(needleLargeR);

            return;
        }

        for (int i = 0; i < waterTempObjects.Count; i++)
        {
            GameObject needleLarge = waterTempObjects[i].transform.Find("Dial").Find("Needle Large").gameObject;
            countryDialBoard.GetComponent<RotateNeedle>().waterTempNeedles.Add(needleLarge);
        }
    }

    private void AsignHeadingIndicator()
    {
        if (airplaneData.planeAttributes.country == Country.US)
        {
            headingIndicator = GameObject.FindGameObjectWithTag("heading indicator");
            countryDialBoard.GetComponent<RotateNeedle>().headingIndicator = headingIndicator.transform.Find("Dial").Find("Mask").Find("Parent").transform.gameObject;

            if (airplaneData.planeAttributes.headingIndicatorType == DialVariant.B)
            {

                countryDialBoard.GetComponent<RotateNeedle>().headingIndicatorBall = headingIndicator.transform.Find("Dial").Find("BallParent").transform.gameObject;
            }
        }
    }

    private void AsignHorizon()
    {
        if (airplaneData.planeAttributes.country == Country.US)
        {
            artificialHorizon = GameObject.FindGameObjectWithTag("horizon");
            countryDialBoard.GetComponent<RotateNeedle>().artificialHorizon = artificialHorizon.transform.Find("Dial").Find("Mask").Find("Line").transform.gameObject;
            countryDialBoard.GetComponent<RotateNeedle>().artificialHorizonChevron = artificialHorizon.transform.Find("Dial").Find("Mask").Find("Roll Mark").transform.gameObject;
        }
    }

    private void AsignTurnCoordinator()
    {
        if (airplaneData.planeAttributes.country == Country.US)
        {
            turnIndicator = GameObject.FindGameObjectWithTag("turn coordinator");
            countryDialBoard.GetComponent<RotateNeedle>().turnCoordinatorNeedle = turnIndicator.transform.Find("Dial").Find("NeedleParent").transform.gameObject;
            countryDialBoard.GetComponent<RotateNeedle>().turnCoordinatorBall = turnIndicator.transform.Find("Dial").Find("BallParent").transform.gameObject;
        }
    }

    void AsignVSI()
    {

        if (airplaneData.planeAttributes.country == Country.US)
        {
            vsi = GameObject.FindGameObjectWithTag("vsi");
            countryDialBoard.GetComponent<RotateNeedle>().vsiNeedle = vsi.transform.Find("Dial").Find("Needle").transform.gameObject;
        }
        else
        {
            //there are more than one vsi but never more than one at the same time, so we share prefabs
            //let the rotate needle script know which needle to turn
            if (airplaneData.planeAttributes.vsiLarge)
            {
                countryDialBoard.GetComponent<RotateNeedle>().vsiNeedle = countryDialBoard.transform.Find("VSI Large").Find("Dial").Find("Needle").gameObject;
            }

            else if (airplaneData.planeAttributes.vsiSmall)
            {
                countryDialBoard.GetComponent<RotateNeedle>().vsiNeedle = countryDialBoard.transform.Find("VSI Small").Find("Dial").Find("Needle").gameObject;
            }

            else if (airplaneData.planeAttributes.vsiSmallest)
            {
                countryDialBoard.GetComponent<RotateNeedle>().vsiNeedle = countryDialBoard.transform.Find("VSI Smallest").Find("Dial").Find("Needle").gameObject;
            }
        }

    }

    void AsignRPM()
    {
        //empty lists first
        countryDialBoard.GetComponent<RotateNeedle>().rpmNeedlesLarge.Clear();
        countryDialBoard.GetComponent<RotateNeedle>().rpmNeedlesSmall.Clear();

        for (int i = 0; i < rpmObjects.Count; i++)
        {
            //Primary needle - most planes have this
            if (rpmObjects[i].transform.Find("Dial").Find("Needle Large") != null)
            {
                GameObject needleLarge = rpmObjects[i].transform.Find("Dial").Find("Needle Large").gameObject;
                countryDialBoard.GetComponent<RotateNeedle>().rpmNeedlesLarge.Add(needleLarge);
            }

            //country specific
            if (airplaneData.planeAttributes.country == Country.RU)
            {
                if (airplaneData.planeAttributes.rpmType == DialVariant.A)
                {
                    GameObject needleSmall = rpmObjects[i].transform.Find("Dial").Find("Needle Small").gameObject;
                    countryDialBoard.GetComponent<RotateNeedle>().rpmNeedlesSmall.Add(needleSmall);
                }


                //pe-2 
                if (airplaneData.planeAttributes.country == Country.RU && airplaneData.planeAttributes.rpmType == DialVariant.C)
                {
                    GameObject needleSmall = rpmObjects[i].transform.Find("Dial").Find("Needle Small").gameObject;
                    countryDialBoard.GetComponent<RotateNeedle>().rpmNeedlesSmall.Add(needleSmall);
                }

            }

            else if (airplaneData.planeAttributes.country == Country.US)
            {
                //p38 J
                if (airplaneData.planeType == "P-38J-25" || airplaneData.planeType == "C-47A")
                {
                    if (i == 0)
                    {
                        //do both needles and return, p38 has two needles on one dial
                        GameObject needleLeft = rpmObjects[i].transform.Find("Dial").Find("Needle Left").gameObject;
                        countryDialBoard.GetComponent<RotateNeedle>().rpmNeedlesLarge.Add(needleLeft);

                        GameObject needleRight = rpmObjects[i].transform.Find("Dial").Find("Needle Right").gameObject;
                        countryDialBoard.GetComponent<RotateNeedle>().rpmNeedlesLarge.Add(needleRight);

                        return;
                    }
                }

                if (airplaneData.planeAttributes.rpmType == DialVariant.A || airplaneData.planeAttributes.rpmType == DialVariant.D)
                {
                    GameObject needleSmall = rpmObjects[i].transform.Find("Dial").Find("Needle Small").gameObject;
                    countryDialBoard.GetComponent<RotateNeedle>().rpmNeedlesSmall.Add(needleSmall);
                }
            }

            else if (airplaneData.planeAttributes.country == Country.GER)
            {
                if (airplaneData.planeType == "Me 410 A-1")
                {
                    if (i == 0)
                    {
                        //do both needles and return, p38 has two needles on one dial
                        GameObject needleLeft = rpmObjects[i].transform.Find("Dial").Find("Needle Left").gameObject;
                        countryDialBoard.GetComponent<RotateNeedle>().rpmNeedlesLarge.Add(needleLeft);

                        GameObject needleRight = rpmObjects[i].transform.Find("Dial").Find("Needle Right").gameObject;
                        countryDialBoard.GetComponent<RotateNeedle>().rpmNeedlesLarge.Add(needleRight);

                        return;
                    }
                }
            }

            else if (airplaneData.planeAttributes.country == Country.UK || airplaneData.planeAttributes.country == Country.FR)
            {
                //Mosquito
                if (airplaneData.planeAttributes.rpmType == DialVariant.C)
                {
                    GameObject needleSmall = rpmObjects[i].transform.Find("Dial").Find("Needle Small").gameObject;
                    countryDialBoard.GetComponent<RotateNeedle>().rpmNeedlesSmall.Add(needleSmall);
                }
            }
        }
    }

    void AsignManifold()
    {
        //empty lists first
        countryDialBoard.GetComponent<RotateNeedle>().manifoldNeedlesLarge.Clear();
        countryDialBoard.GetComponent<RotateNeedle>().manifoldNeedlesSmall.Clear();


        for (int i = 0; i < manifoldObjects.Count; i++)
        {
            if (airplaneData.planeType == "P-38J-25" ||
                    airplaneData.planeType == "He 111 H-16" ||
                        airplaneData.planeType == "Me 410 A-1" ||
                            airplaneData.planeType == "Hs 129 B-2" ||
                              airplaneData.planeType == "C-47A")
            {
                //p38 J or he 111 h16
                GameObject needleLeft = manifoldObjects[i].transform.Find("Dial").Find("Needle Left").gameObject;
                countryDialBoard.GetComponent<RotateNeedle>().manifoldNeedlesLarge.Add(needleLeft);

                GameObject needleRight = manifoldObjects[i].transform.Find("Dial").Find("Needle Right").gameObject;
                countryDialBoard.GetComponent<RotateNeedle>().manifoldNeedlesLarge.Add(needleRight);

                //both needles asign and jump out of loop (has 2 engines)
                return;

            }
            else
            {
                GameObject needleLarge = manifoldObjects[i].transform.Find("Dial").Find("Needle Large").gameObject;
                countryDialBoard.GetComponent<RotateNeedle>().manifoldNeedlesLarge.Add(needleLarge);
            }
        }
    }

    void AsignSpeedometer()
    {
        speedometer = GameObject.FindGameObjectWithTag("speedometer");
        countryDialBoard.GetComponent<RotateNeedle>().airspeedNeedle = speedometer.transform.Find("Dial").Find("Needle Large").transform.gameObject;
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
            case Country.RU:
                //countryDials[0].SetActive(true);
                GameObject RUprefab = Resources.Load("Prefabs/RU") as GameObject;
                countryDialBoard = GameObject.Instantiate(RUprefab, canvas.transform.position, Quaternion.identity, canvas.transform.GetChild(0).transform);
                break;

            case Country.GER:
                GameObject GERprefab = Resources.Load("Prefabs/GER") as GameObject;
                countryDialBoard = GameObject.Instantiate(GERprefab, canvas.transform.position, Quaternion.identity, canvas.transform.GetChild(0).transform);
                break;

            case Country.US:
                GameObject USprefab = Resources.Load("Prefabs/US") as GameObject;
                countryDialBoard = GameObject.Instantiate(USprefab, canvas.transform.position, Quaternion.identity, canvas.transform.GetChild(0).transform);
                break;

            case Country.FR:
            case Country.UK:
                GameObject UKprefab = Resources.Load("Prefabs/UK") as GameObject;
                countryDialBoard = GameObject.Instantiate(UKprefab, canvas.transform.position, Quaternion.identity, canvas.transform.GetChild(0).transform);
                break;

            case Country.ITA:
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
                if (airplaneData.planeType == "P-38J-25" || airplaneData.planeType == "Me 410 A-1" || airplaneData.planeType == "C-47A")
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
                        rpmInstance.transform.SetSiblingIndex(rpm.transform.GetSiblingIndex() + 1);

                    }

                    rpmInstance.transform.name = "RPM " + airplaneData.planeAttributes.rpmType.ToString() + " " + i.ToString();
                    rpmObjects.Add(rpmInstance);
                }

                if (dialsToInstantiate == 2) // 3 engine ger plane, no support atm (just ui icons)
                {
                    for (int i = 0; i < rpmObjects.Count; i++)
                    {
                        //engine dials have "L" or "R"
                        GameObject parent = rpmObjects[i].transform.Find("UI Handlers").GetChild(0).Find("Left Right").gameObject;
                        parent.transform.GetChild(i).gameObject.SetActive(true);
                    }
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
                if (airplaneData.planeType == "P-38J-25" ||
                        airplaneData.planeType == "He 111 H-16" ||
                            airplaneData.planeType == "Me 410 A-1" ||
                                airplaneData.planeType == "Hs 129 B-2" ||
                                  airplaneData.planeType == "C-47A")
                    dialsToInstantiate = 1;

                for (int i = 0; i < dialsToInstantiate; i++)
                {
                    //create instance variable if we need to duplicate
                    GameObject manifoldInstance = manifold;
                    if (i > 0)
                    {
                        //duplicate if we have more than one engine
                        manifoldInstance = Instantiate(manifold, manifold.transform.position, Quaternion.identity, countryDialBoard.transform);
                        manifoldInstance.transform.SetSiblingIndex(manifoldInstance.transform.GetSiblingIndex() + 1);
                    }
                    manifoldInstance.transform.name = "Manifold " + airplaneData.planeAttributes.manifoldType.ToString() + " " + i.ToString();
                    manifoldObjects.Add(manifoldInstance);
                }

                if (dialsToInstantiate == 2)
                {
                    for (int i = 0; i < manifoldObjects.Count; i++)
                    {
                        //engine dials have "L" or "R"
                        GameObject parent = manifoldObjects[i].transform.Find("UI Handlers").GetChild(0).Find("Left Right").gameObject;
                        parent.transform.GetChild(i).gameObject.SetActive(true);
                    }
                }
            }

            //instantiate water temps
            waterTempObjects.Clear();

            string waterTempString = airplaneData.planeAttributes.waterTempType.ToString();

            if (countryDialBoard.transform.Find("Water Temp " + waterTempString) != null)
            {
                //find prefab outside of loop
                GameObject waterTemp = countryDialBoard.transform.Find("Water Temp " + waterTempString).gameObject;

                int dialsToInstantiate = airplaneData.planeAttributes.engines;
                if (airplaneData.planeType == "P-38J-25")
                    dialsToInstantiate = 1;

                for (int i = 0; i < dialsToInstantiate; i++)
                {
                    //create instance variable if we need to duplicate
                    GameObject waterTempInstance = waterTemp;
                    if (i > 0)
                    {
                        //duplicate if we have more than one engine
                        waterTempInstance = Instantiate(waterTemp, waterTemp.transform.position, Quaternion.identity, countryDialBoard.transform);

                    }
                    //set to last position - getting fiddly with this rpm/manifolds/water temps - need better solution
                    waterTempInstance.transform.SetSiblingIndex(countryDialBoard.transform.childCount - 1);
                    waterTempInstance.transform.name = "Water Temp " + airplaneData.planeAttributes.waterTempType.ToString() + " " + i.ToString();
                    waterTempObjects.Add(waterTempInstance);
                }

                if (dialsToInstantiate == 2) // 3 engine ger plane, no support atm
                {
                    for (int i = 0; i < waterTempObjects.Count; i++)
                    {
                        //engine dials have "L" or "R"
                        GameObject parent = waterTempObjects[i].transform.Find("UI Handlers").GetChild(0).Find("Left Right").gameObject;
                        parent.transform.GetChild(i).gameObject.SetActive(true);
                    }
                }
            }

            //instantiate oil In temps
            oilTempInObjects.Clear();
            string oilString = airplaneData.planeAttributes.oilTempInType.ToString();
            if (countryDialBoard.transform.Find("Oil Temp In " + oilString) != null)
            {
                //find prefab outside of loop
                GameObject oilTemp = countryDialBoard.transform.Find("Oil Temp In " + oilString).gameObject;

                int dialsToInstantiate = airplaneData.planeAttributes.engines;
                if (airplaneData.planeType == "P-38J-25")
                    dialsToInstantiate = 1;

                for (int i = 0; i < dialsToInstantiate; i++)
                {
                    //create instance variable if we need to duplicate
                    GameObject oilTempInstance = oilTemp;
                    if (i > 0)
                    {
                        //duplicate if we have more than one engine
                        oilTempInstance = Instantiate(oilTemp, oilTemp.transform.position, Quaternion.identity, countryDialBoard.transform);

                    }
                    //set to last position - getting fiddly with this rpm/manifolds/oil temps - need better solution
                    oilTempInstance.transform.SetSiblingIndex(countryDialBoard.transform.childCount - 1);
                    oilTempInstance.transform.name = "Oil Temp In " + airplaneData.planeAttributes.oilTempInType.ToString() + " " + i.ToString();
                    oilTempInObjects.Add(oilTempInstance);
                }

                if (dialsToInstantiate == 2) // 3 engine ger plane, no support atm
                {
                    for (int i = 0; i < oilTempInObjects.Count; i++)
                    {
                        //engine dials have "L" or "R"
                        GameObject parent = oilTempInObjects[i].transform.Find("UI Handlers").GetChild(0).Find("Left Right").gameObject;
                        parent.transform.GetChild(i).gameObject.SetActive(true);
                    }
                }
            }

            //instantiate oil out temps
            oilTempOutObjects.Clear();
            oilString = airplaneData.planeAttributes.oilTempOutType.ToString();
            if (countryDialBoard.transform.Find("Oil Temp Out " + oilString) != null)
            {
                //find prefab outside of loop
                GameObject oilTemp = countryDialBoard.transform.Find("Oil Temp Out " + oilString).gameObject;

                int dialsToInstantiate = airplaneData.planeAttributes.engines;
                if (airplaneData.planeType == "C-47A" || airplaneData.planeType == "A-20B")
                    dialsToInstantiate = 1;

                for (int i = 0; i < dialsToInstantiate; i++)
                {
                    //create instance variable if we need to duplicate
                    GameObject oilTempInstance = oilTemp;
                    if (i > 0)
                    {
                        //duplicate if we have more than one engine
                        oilTempInstance = Instantiate(oilTemp, oilTemp.transform.position, Quaternion.identity, countryDialBoard.transform);

                    }
                    //set to last position - getting fiddly with this rpm/manifolds/oil temps - need better solution
                    oilTempInstance.transform.SetSiblingIndex(countryDialBoard.transform.childCount - 1);
                    oilTempInstance.transform.name = "Oil Temp Out " + airplaneData.planeAttributes.oilTempOutType.ToString() + " " + i.ToString();
                    oilTempOutObjects.Add(oilTempInstance);
                }

                if (dialsToInstantiate == 2) // 3 engine ger plane, no support atm
                {
                    for (int i = 0; i < oilTempOutObjects.Count; i++)
                    {
                        //engine dials have "L" or "R"
                        GameObject parent = oilTempOutObjects[i].transform.Find("UI Handlers").GetChild(0).Find("Left Right").gameObject;
                        parent.transform.GetChild(i).gameObject.SetActive(true);
                    }
                }
            }

            //instantiate oil pressure combo temps
            oilTempPressureObjects.Clear();
            string oilTempPressureString = airplaneData.planeAttributes.oilTempPressureType.ToString();
            if (countryDialBoard.transform.Find("Oil Temp Pressure " + oilTempPressureString) != null)
            {
                //find prefab outside of loop
                GameObject oilTemp = countryDialBoard.transform.Find("Oil Temp Pressure " + oilTempPressureString).gameObject;

                int dialsToInstantiate = airplaneData.planeAttributes.engines;
                //if (airplaneData.planeType == "P-38J-25")
                //  dialsToInstantiate = 1;

                for (int i = 0; i < dialsToInstantiate; i++)
                {
                    //create instance variable if we need to duplicate
                    GameObject oilTempInstance = oilTemp;
                    if (i > 0)
                    {
                        //duplicate if we have more than one engine
                        oilTempInstance = Instantiate(oilTemp, oilTemp.transform.position, Quaternion.identity, countryDialBoard.transform);

                    }
                    //set to last position - getting fiddly with this rpm/manifolds/oil temps - need better solution
                    oilTempInstance.transform.SetSiblingIndex(countryDialBoard.transform.childCount - 1);
                    oilTempInstance.transform.name = "Oil Temp Pressure " + airplaneData.planeAttributes.oilTempPressureType.ToString() + " " + i.ToString();
                    oilTempPressureObjects.Add(oilTempInstance);
                }

                if (dialsToInstantiate == 2) // 3 engine ger plane, no support atm
                {
                    for (int i = 0; i < oilTempPressureObjects.Count; i++)
                    {
                        //engine dials have "L" or "R"
                        GameObject parent = oilTempPressureObjects[i].transform.Find("UI Handlers").GetChild(0).Find("Left Right").gameObject;
                        parent.transform.GetChild(i).gameObject.SetActive(true);
                    }
                }
            }

            //instantiate oil combo in and out temps
            oilTempComboObjects.Clear();
            string oilTempComboString = airplaneData.planeAttributes.oilTempComboType.ToString();
            if (countryDialBoard.transform.Find("Oil Temp Combo " + oilTempComboString) != null)
            {
                //find prefab outside of loop
                GameObject oilTemp = countryDialBoard.transform.Find("Oil Temp Combo " + oilTempComboString).gameObject;

                int dialsToInstantiate = airplaneData.planeAttributes.engines;
                for (int i = 0; i < dialsToInstantiate; i++)
                {
                    //create instance variable if we need to duplicate
                    GameObject oilTempInstance = oilTemp;
                    if (i > 0)
                    {
                        //duplicate if we have more than one engine
                        oilTempInstance = Instantiate(oilTemp, oilTemp.transform.position, Quaternion.identity, countryDialBoard.transform);

                    }
                    //set to last position - getting fiddly with this rpm/manifolds/oil temps - need better solution
                    oilTempInstance.transform.SetSiblingIndex(countryDialBoard.transform.childCount - 1);
                    oilTempInstance.transform.name = "Oil Temp Combo " + airplaneData.planeAttributes.oilTempComboType.ToString() + " " + i.ToString();
                    oilTempComboObjects.Add(oilTempInstance);
                }

                if (dialsToInstantiate == 2) // 3 engine ger plane, no support atm (just ui icons)
                {
                    for (int i = 0; i < oilTempComboObjects.Count; i++)
                    {
                        //engine dials have "L" or "R"
                        GameObject parent = oilTempComboObjects[i].transform.Find("UI Handlers").GetChild(0).Find("Left Right").gameObject;
                        parent.transform.GetChild(i).gameObject.SetActive(true);
                    }
                }
            }

            //instantiate cylinder head temps
            cylinderHeadObjects.Clear();
            string cylinderTempComboString = airplaneData.planeAttributes.cylinderHeadType.ToString();
            if (countryDialBoard.transform.Find("Cylinder Head Temp " + cylinderTempComboString) != null)
            {
                //find prefab outside of loop
                GameObject cylinderTemp = countryDialBoard.transform.Find("Cylinder Head Temp " + cylinderTempComboString).gameObject;

                int dialsToInstantiate = airplaneData.planeAttributes.engines;
                if (airplaneData.planeType == "C-47A" || airplaneData.planeType == "A-20B")
                    dialsToInstantiate = 1;
                for (int i = 0; i < dialsToInstantiate; i++)
                {
                    //create instance variable if we need to duplicate
                    GameObject cylinderTempInstance = cylinderTemp;
                    if (i > 0)
                    {
                        //duplicate if we have more than one engine
                        cylinderTempInstance = Instantiate(cylinderTemp, cylinderTemp.transform.position, Quaternion.identity, countryDialBoard.transform);

                    }
                    //set to last position - getting fiddly with this rpm/manifolds/oil temps - need better solution
                    cylinderTempInstance.transform.SetSiblingIndex(countryDialBoard.transform.childCount - 1);
                    cylinderTempInstance.transform.name = "Cylinder Head Temp " + airplaneData.planeAttributes.cylinderHeadType.ToString() + " " + i.ToString();
                    cylinderHeadObjects.Add(cylinderTempInstance);
                }

                if (dialsToInstantiate == 2) // 3 engine ger plane, no support atm (just ui icons)
                {
                    for (int i = 0; i < cylinderHeadObjects.Count; i++)
                    {
                        //engine dials have "L" or "R"
                        GameObject parent = cylinderHeadObjects[i].transform.Find("UI Handlers").GetChild(0).Find("Left Right").gameObject;
                        parent.transform.GetChild(i).gameObject.SetActive(true);
                    }
                }
            }

            //instantiate carb air temps
            carbTempObjects.Clear();
            string carbAirTempComboString = airplaneData.planeAttributes.carbAirTempType.ToString();
            if (countryDialBoard.transform.Find("Carb Air Temp " + carbAirTempComboString) != null)
            {
                //find prefab outside of loop
                GameObject carbAirTemp = countryDialBoard.transform.Find("Carb Air Temp " + carbAirTempComboString).gameObject;

                int dialsToInstantiate = airplaneData.planeAttributes.engines;                
                if (airplaneData.planeType == "C-47A" || airplaneData.planeType == "A-20B" || airplaneData.planeType == "P-38J-25")
                        
                    dialsToInstantiate = 1;
                for (int i = 0; i < dialsToInstantiate; i++)
                {
                    //create instance variable if we need to duplicate
                    GameObject carbAirInstance = carbAirTemp;
                    if (i > 0)
                    {
                        //duplicate if we have more than one engine
                        carbAirInstance = Instantiate(carbAirTemp, carbAirTemp.transform.position, Quaternion.identity, countryDialBoard.transform);

                    }
                    //set to last position - getting fiddly with this rpm/manifolds/oil temps - need better solution
                    carbAirInstance.transform.SetSiblingIndex(countryDialBoard.transform.childCount - 1);
                    carbAirInstance.transform.name = "Carb Air Temp " + airplaneData.planeAttributes.carbAirTempType.ToString() + " " + i.ToString();
                    carbTempObjects.Add(carbAirInstance);
                }

                if (dialsToInstantiate == 2) // 3 engine ger plane, no support atm (just ui icons)
                {
                    for (int i = 0; i < carbTempObjects.Count; i++)
                    {
                        //engine dials have "L" or "R"
                        GameObject parent = carbTempObjects[i].transform.Find("UI Handlers").GetChild(0).Find("Left Right").gameObject;
                        parent.transform.GetChild(i).gameObject.SetActive(true);
                    }
                }
            }
        }
    }

    //static to refactor to new class - to do
    void DeactivateUnavailableDials(GameObject countryDialBoard, PlaneDataFromName.PlaneAttributes planeAttributes)
    {
        //check what dials are available and switch off as needed

        GameObject[] speedos = GameObject.FindGameObjectsWithTag("speedometer");
        for (int i = 0; i < speedos.Length; i++)
        {
            if (speedos[i].name != "Speedometer " + planeAttributes.speedometerType.ToString())
                speedos[i].SetActive(false);
        }

        if (!planeAttributes.turnAndBank)
            if (countryDialBoard.transform.Find("Turn And Bank") != null)
                countryDialBoard.transform.Find("Turn And Bank").gameObject.SetActive(false);


        if (!planeAttributes.repeaterCompass)
            if (countryDialBoard.transform.Find("Repeater Compass") != null)
                countryDialBoard.transform.Find("Repeater Compass").gameObject.SetActive(false);

        if (!planeAttributes.repeaterCompassAlternate)
            if (countryDialBoard.transform.Find("Repeater Compass Alternate") != null)
                countryDialBoard.transform.Find("Repeater Compass Alternate").gameObject.SetActive(false);


        //heading indicator
        if (planeAttributes.country != Country.US)
        {
            if (!planeAttributes.headingIndicator)
                if (countryDialBoard.transform.Find("Heading Indicator") != null)
                    countryDialBoard.transform.Find("Heading Indicator").gameObject.SetActive(false);
        }
        else
        {
            GameObject[] headings = GameObject.FindGameObjectsWithTag("heading indicator");
            for (int i = 0; i < headings.Length; i++)
            {
                if (headings[i].name != "Heading Indicator " + planeAttributes.headingIndicatorType.ToString())
                    headings[i].SetActive(false);
            }
        }

        //horizon
        if (planeAttributes.country != Country.US)
        {
            if (!planeAttributes.artificialHorizon)
                if (countryDialBoard.transform.Find("Artificial Horizon") != null)
                    countryDialBoard.transform.Find("Artificial Horizon").gameObject.SetActive(false);
        }
        else
        {
            GameObject[] horizons = GameObject.FindGameObjectsWithTag("horizon");
            for (int i = 0; i < horizons.Length; i++)
            {
                if (horizons[i].name != "Artificial Horizon " + planeAttributes.horizonType.ToString())
                    horizons[i].SetActive(false);
            }
        }

        //turn co-ord
        if (planeAttributes.country != Country.US)
        {
            if (!planeAttributes.turnCoordinator)
                if (countryDialBoard.transform.Find("Turn Coordinator").gameObject != null)
                    countryDialBoard.transform.Find("Turn Coordinator").gameObject.SetActive(false);
        }
        else
        {
            GameObject[] turnCoordinators = GameObject.FindGameObjectsWithTag("turn coordinator");
            for (int i = 0; i < turnCoordinators.Length; i++)
            {
                if (turnCoordinators[i].name != "Turn Coordinator " + planeAttributes.turnCoordinatorType.ToString())
                    turnCoordinators[i].SetActive(false);
            }
        }

        //vsi
        if (planeAttributes.country != Country.US)
        {
            //old system
            if (!planeAttributes.vsiSmallest)
                if (countryDialBoard.transform.Find("VSI Smallest") != null)
                    countryDialBoard.transform.Find("VSI Smallest").gameObject.SetActive(false);

            if (!planeAttributes.vsiSmall)
                if (countryDialBoard.transform.Find("VSI Small") != null)
                    countryDialBoard.transform.Find("VSI Small").gameObject.SetActive(false);

            if (!planeAttributes.vsiLarge)
                if (countryDialBoard.transform.Find("VSI Large") != null)
                    countryDialBoard.transform.Find("VSI Large").gameObject.SetActive(false);
        }
        else
        {
            //new system
            GameObject[] vsis = GameObject.FindGameObjectsWithTag("vsi");
            for (int i = 0; i < vsis.Length; i++)
            {
                if (vsis[i].name != "VSI " + planeAttributes.vsiType.ToString())
                    vsis[i].SetActive(false);
            }
        }


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

        GameObject[] allWaterTempsArray = GameObject.FindGameObjectsWithTag("water temp");
        List<GameObject> allWaterTemps = new List<GameObject>();
        allWaterTemps.AddRange(allWaterTempsArray);

        foreach (GameObject waterTemp in waterTempObjects)
            allWaterTemps.Remove(waterTemp);

        for (int i = 0; i < allWaterTemps.Count; i++)
        {
            allWaterTemps[i].SetActive(false);
        }

        GameObject[] allOilInTempsArray = GameObject.FindGameObjectsWithTag("oil temp in");
        List<GameObject> allOilInTemps = new List<GameObject>();
        allOilInTemps.AddRange(allOilInTempsArray);

        foreach (GameObject oilTemp in oilTempInObjects)
            allOilInTemps.Remove(oilTemp);

        for (int i = 0; i < allOilInTemps.Count; i++)
        {
            allOilInTemps[i].SetActive(false);
        }

        GameObject[] allOilOutTempsArray = GameObject.FindGameObjectsWithTag("oil temp out");
        List<GameObject> allOilOutTemps = new List<GameObject>();
        allOilOutTemps.AddRange(allOilOutTempsArray);

        foreach (GameObject oilTemp in oilTempOutObjects)
            allOilOutTemps.Remove(oilTemp);

        for (int i = 0; i < allOilOutTemps.Count; i++)
        {
            allOilOutTemps[i].SetActive(false);
        }

        GameObject[] allOilPressureTempsArray = GameObject.FindGameObjectsWithTag("oil temp pressure");
        List<GameObject> allOilPressureTemps = new List<GameObject>();
        allOilPressureTemps.AddRange(allOilPressureTempsArray);

        foreach (GameObject oilTemp in oilTempPressureObjects)
            allOilPressureTemps.Remove(oilTemp);

        for (int i = 0; i < allOilPressureTemps.Count; i++)
        {
            allOilPressureTemps[i].SetActive(false);
        }

        GameObject[] allOilComboTempsArray = GameObject.FindGameObjectsWithTag("oil temp combo");
        List<GameObject> allOilComboTemps = new List<GameObject>();
        allOilComboTemps.AddRange(allOilComboTempsArray);

        foreach (GameObject oilTemp in oilTempComboObjects)
            allOilComboTemps.Remove(oilTemp);

        for (int i = 0; i < allOilComboTemps.Count; i++)
        {
            allOilComboTemps[i].SetActive(false);
        }

        GameObject[] allCylinderHeadTempsArray = GameObject.FindGameObjectsWithTag("cylinder head temp");
        List<GameObject> allCylinderHeadTemps = new List<GameObject>();
        allCylinderHeadTemps.AddRange(allCylinderHeadTempsArray);

        foreach (GameObject cylinderHeadTemp in cylinderHeadObjects)
            allCylinderHeadTemps.Remove(cylinderHeadTemp);

        for (int i = 0; i < allCylinderHeadTemps.Count; i++)
        {
            allCylinderHeadTemps[i].SetActive(false);
        }

        GameObject[] allCarbAirTempsArray = GameObject.FindGameObjectsWithTag("carb air temp");
        List<GameObject> allCarbAirTemps = new List<GameObject>();
        allCarbAirTemps.AddRange(allCarbAirTempsArray);

        foreach (GameObject carbAirTemp in carbTempObjects)
            allCarbAirTemps.Remove(carbAirTemp);

        for (int i = 0; i < allCarbAirTemps.Count; i++)
        {
            allCarbAirTemps[i].SetActive(false);
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
        if (!menuHandler.dialsInTray.Contains(speedometer))
        {
            //parent position containing dial and UI
            layout.speedoPos = speedometer.GetComponent<RectTransform>().anchoredPosition;
            // but only dial scale, we don't want to alter the UI default scale
            float s = speedometer.transform.Find("Dial").GetComponent<RectTransform>().localScale.x;

            layout.speedoScale = s;
        }
        else
            //if we don't find it, look for it in the tray
            SpeedoInTray(layout);


        if (countryDialBoard.transform.Find("Altimeter") != null)
        {
            layout.altPos = countryDialBoard.transform.Find("Altimeter").GetComponent<RectTransform>().anchoredPosition;
            layout.altScale = countryDialBoard.transform.Find("Altimeter").Find("Dial").GetComponent<RectTransform>().localScale.x;
        }
        else
            DialInTray("Altimeter", layout);

        if (airplaneData.planeAttributes.country == Country.US)
        {
            //new
            if (!menuHandler.dialsInTray.Contains(headingIndicator))
            {
                layout.headingPos = headingIndicator.GetComponent<RectTransform>().anchoredPosition;
                layout.headingScale = headingIndicator.transform.Find("Dial").GetComponent<RectTransform>().localScale.x;
            }
            else
                //if we don't find it, look for it in the tray
                HeadingIndicatorInTray(layout);
        }
        else
        {
            //old
            if (countryDialBoard.transform.Find("Heading Indicator") != null)
            {
                layout.headingPos = countryDialBoard.transform.Find("Heading Indicator").GetComponent<RectTransform>().anchoredPosition;
                layout.headingScale = countryDialBoard.transform.Find("Heading Indicator").Find("Dial").GetComponent<RectTransform>().localScale.x;
            }
            else
                DialInTray("Heading Indicator", layout);
        }


        if (countryDialBoard.transform.Find("Turn And Bank") != null)
        {
            layout.turnAndBankPos = countryDialBoard.transform.Find("Turn And Bank").GetComponent<RectTransform>().anchoredPosition;
            layout.turnAndBankScale = countryDialBoard.transform.Find("Turn And Bank").Find("Dial").GetComponent<RectTransform>().localScale.x;
        }
        else
            DialInTray("Turn And Bank", layout);

        if (airplaneData.planeAttributes.country == Country.US)
        {
            //new
            if (!menuHandler.dialsInTray.Contains(turnIndicator))
            {

                layout.turnIndicatorPos = turnIndicator.GetComponent<RectTransform>().anchoredPosition;
                layout.turnIndicatorScale = turnIndicator.transform.Find("Dial").GetComponent<RectTransform>().localScale.x;
            }
            else
                //if we don't find it, look for it in the tray
                TurnIndicatorInTray(layout);
        }
        else
        {
            if (countryDialBoard.transform.Find("Turn Coordinator") != null)
            {
                layout.turnIndicatorPos = countryDialBoard.transform.Find("Turn Coordinator").GetComponent<RectTransform>().anchoredPosition;
                layout.turnIndicatorScale = countryDialBoard.transform.Find("Turn Coordinator").Find("Dial").GetComponent<RectTransform>().localScale.x;
            }
            else
                DialInTray("Turn Coordinator", layout);
        }


        if (airplaneData.planeAttributes.country == Country.US)
        {
            if (!menuHandler.dialsInTray.Contains(vsi))
            {
                layout.vsiPos = vsi.GetComponent<RectTransform>().anchoredPosition;
                layout.vsiScale = vsi.transform.Find("Dial").GetComponent<RectTransform>().localScale.x;
            }
            else
                VSIInTray(layout);
        }
        else
        {
            if (countryDialBoard.transform.Find("VSI Smallest") != null)
            {
                layout.vsiSmallestPos = countryDialBoard.transform.Find("VSI Smallest").GetComponent<RectTransform>().anchoredPosition;
                layout.vsiSmallestScale = countryDialBoard.transform.Find("VSI Smallest").Find("Dial").GetComponent<RectTransform>().localScale.x;
            }
            else
                DialInTray("VSI Smallest", layout);


            if (countryDialBoard.transform.Find("VSI Small") != null)
            {
                layout.vsiSmallPos = countryDialBoard.transform.Find("VSI Small").GetComponent<RectTransform>().anchoredPosition;
                layout.vsiSmallScale = countryDialBoard.transform.Find("VSI Small").Find("Dial").GetComponent<RectTransform>().localScale.x;
            }
            else
                DialInTray("VSI Small", layout);


            if (countryDialBoard.transform.Find("VSI Large") != null)
            {
                layout.vsiLargePos = countryDialBoard.transform.Find("VSI Large").GetComponent<RectTransform>().anchoredPosition;
                layout.vsiLargeScale = countryDialBoard.transform.Find("VSI Large").Find("Dial").GetComponent<RectTransform>().localScale.x;
            }
            else
                DialInTray("VSI Large", layout);
        }

        if (airplaneData.planeAttributes.country == Country.US)
        {
            //new
            if (!menuHandler.dialsInTray.Contains(artificialHorizon))
            {
                layout.artificialHorizonPos = artificialHorizon.GetComponent<RectTransform>().anchoredPosition;
                layout.artificialHorizonScale = artificialHorizon.transform.Find("Dial").GetComponent<RectTransform>().localScale.x;
            }
            else
                //if we don't find it, look for it in the tray
                HorizonInTray(layout);
        }
        else
        {
            //old
            if (countryDialBoard.transform.Find("Artificial Horizon") != null)
            {
                layout.artificialHorizonPos = countryDialBoard.transform.Find("Artificial Horizon").GetComponent<RectTransform>().anchoredPosition;
                layout.artificialHorizonScale = countryDialBoard.transform.Find("Artificial Horizon").Find("Dial").GetComponent<RectTransform>().localScale.x;
            }
            else
                DialInTray("Artificial Horizon", layout);
        }


        if (countryDialBoard.transform.Find("Repeater Compass") != null)
        {
            layout.repeaterCompassPos = countryDialBoard.transform.Find("Repeater Compass").GetComponent<RectTransform>().anchoredPosition;
            layout.repeaterCompassScale = countryDialBoard.transform.Find("Repeater Compass").Find("Dial").GetComponent<RectTransform>().localScale.x;
        }
        else
            DialInTray("Repeater Compass", layout);

        if (countryDialBoard.transform.Find("Repeater Compass Alternate") != null)
        {
            layout.repeaterCompassAlternatePos = countryDialBoard.transform.Find("Repeater Compass Alternate").GetComponent<RectTransform>().anchoredPosition;
            layout.repeaterCompassAlternateScale = countryDialBoard.transform.Find("Repeater Compass Alternate").Find("Dial").GetComponent<RectTransform>().localScale.x;
        }
        else
            DialInTray("Repeater Compass Alternate", layout);

        for (int i = 0; i < rpmObjects.Count; i++)
        {
            if (rpmObjects[i].transform.parent == countryDialBoard.transform)
            {
                layout.rpmPos[i] = rpmObjects[i].GetComponent<RectTransform>().anchoredPosition;
                layout.rpmScale[i] = rpmObjects[i].transform.Find("Dial").GetComponent<RectTransform>().localScale.x;
            }
            else
                RPMInTray(layout, i, rpmObjects[i]);
        }

        for (int i = 0; i < manifoldObjects.Count; i++)
        {
            //if on dial board
            if (manifoldObjects[i].transform.parent == countryDialBoard.transform)
            {
                layout.manifoldPos[i] = manifoldObjects[i].GetComponent<RectTransform>().anchoredPosition;
                layout.manifoldScale[i] = manifoldObjects[i].transform.Find("Dial").GetComponent<RectTransform>().localScale.x;
            }
            else
                ManifoldInTray(layout, i, manifoldObjects[i]);
        }

        for (int i = 0; i < waterTempObjects.Count; i++)
        {
            //if on dial board
            if (waterTempObjects[i].transform.parent == countryDialBoard.transform)
            {
                layout.waterTempPos[i] = waterTempObjects[i].GetComponent<RectTransform>().anchoredPosition;
                layout.waterTempScale[i] = waterTempObjects[i].transform.Find("Dial").GetComponent<RectTransform>().localScale.x;
            }
            else
                WaterTempInTray(layout, i, waterTempObjects[i]);
        }

        for (int i = 0; i < oilTempInObjects.Count; i++)
        {
            //if on dial board
            if (oilTempInObjects[i].transform.parent == countryDialBoard.transform)
            {
                layout.oilTempInPos[i] = oilTempInObjects[i].GetComponent<RectTransform>().anchoredPosition;
                layout.oilTempInScale[i] = oilTempInObjects[i].transform.Find("Dial").GetComponent<RectTransform>().localScale.x;
            }
            else
                OilTempInInTray(layout, i, oilTempInObjects[i]);
        }

        for (int i = 0; i < oilTempOutObjects.Count; i++)
        {
            //if on dial board
            if (oilTempOutObjects[i].transform.parent == countryDialBoard.transform)
            {
                layout.oilTempOutPos[i] = oilTempOutObjects[i].GetComponent<RectTransform>().anchoredPosition;
                layout.oilTempOutScale[i] = oilTempOutObjects[i].transform.Find("Dial").GetComponent<RectTransform>().localScale.x;
            }
            else
                OilTempOutInTray(layout, i, oilTempOutObjects[i]);
        }

        for (int i = 0; i < oilTempPressureObjects.Count; i++)
        {
            //if on dial board
            if (oilTempPressureObjects[i].transform.parent == countryDialBoard.transform)
            {
                layout.oilTempPressurePos[i] = oilTempPressureObjects[i].GetComponent<RectTransform>().anchoredPosition;
                layout.oilTempPressureScale[i] = oilTempPressureObjects[i].transform.Find("Dial").GetComponent<RectTransform>().localScale.x;
            }
            else
                OilTempPressureInTray(layout, i, oilTempPressureObjects[i]);
        }

        for (int i = 0; i < oilTempComboObjects.Count; i++)
        {
            //if on dial board
            if (oilTempComboObjects[i].transform.parent == countryDialBoard.transform)
            {
                layout.oilTempComboPos[i] = oilTempComboObjects[i].GetComponent<RectTransform>().anchoredPosition;
                layout.oilTempComboScale[i] = oilTempComboObjects[i].transform.Find("Dial").GetComponent<RectTransform>().localScale.x;
            }
            else
                OilTempComboInTray(layout, i, oilTempComboObjects[i]);
        }

        for (int i = 0; i < cylinderHeadObjects.Count; i++)
        {
            //if on dial board
            if (cylinderHeadObjects[i].transform.parent == countryDialBoard.transform)
            {
                layout.cylinderHeadPos[i] = cylinderHeadObjects[i].GetComponent<RectTransform>().anchoredPosition;
                layout.cylinderHeadScale[i] = cylinderHeadObjects[i].transform.Find("Dial").GetComponent<RectTransform>().localScale.x;
            }
            else
                CylinderHeadInTray(layout, i, cylinderHeadObjects[i]);
        }

        for (int i = 0; i < carbTempObjects.Count; i++)
        {
            //if on dial board
            if (carbTempObjects[i].transform.parent == countryDialBoard.transform)
            {
                layout.carbAirPos[i] = carbTempObjects[i].GetComponent<RectTransform>().anchoredPosition;
                layout.carbAirScale[i] = carbTempObjects[i].transform.Find("Dial").GetComponent<RectTransform>().localScale.x;
            }
            else
                CarbAirInTray(layout, i, carbTempObjects[i]);
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

    private void VSIInTray(Layout layout)
    {
        layout.vsiPos = vsi.GetComponent<RectTransform>().anchoredPosition;
        layout.vsiScale = vsi.transform.Find("Dial").GetComponent<RectTransform>().localScale.x;
        layout.vsiInTray = true;
    }

    private void TurnIndicatorInTray(Layout layout)
    {
        layout.turnIndicatorPos = turnIndicator.GetComponent<RectTransform>().anchoredPosition;
        layout.turnIndicatorScale = turnIndicator.GetComponent<RectTransform>().localScale.x;
        layout.turnIndicatorInTray = true;
    }

    private void HorizonInTray(Layout layout)
    {
        layout.artificialHorizonPos = artificialHorizon.GetComponent<RectTransform>().anchoredPosition;
        layout.artificialHorizonScale = artificialHorizon.GetComponent<RectTransform>().localScale.x;
        layout.artificialHorizonInTray = true;
    }


    private void HeadingIndicatorInTray(Layout layout)
    {
        layout.headingPos = headingIndicator.GetComponent<RectTransform>().anchoredPosition;
        layout.headingScale = headingIndicator.GetComponent<RectTransform>().localScale.x;
        layout.headingIndicatorInTray = true;
    }

    void RPMInTray(Layout layout, int i, GameObject rpm)
    {
        layout.rpmPos[i] = rpm.GetComponent<RectTransform>().anchoredPosition;
        layout.rpmScale[i] = rpm.GetComponent<RectTransform>().localScale.x;
        layout.rpmInTray[i] = true;
    }

    void ManifoldInTray(Layout layout, int i, GameObject manifold)
    {
        layout.manifoldPos[i] = manifold.GetComponent<RectTransform>().anchoredPosition;
        layout.manifoldScale[i] = manifold.GetComponent<RectTransform>().localScale.x;
        layout.manifoldInTray[i] = true;
    }

    private void WaterTempInTray(Layout layout, int i, GameObject waterTemp)
    {
        layout.waterTempPos[i] = waterTemp.GetComponent<RectTransform>().anchoredPosition;
        layout.waterTempScale[i] = waterTemp.GetComponent<RectTransform>().localScale.x;
        layout.waterTempInTray[i] = true;
    }

    private void OilTempInInTray(Layout layout, int i, GameObject oilTemp)
    {
        layout.oilTempInPos[i] = oilTemp.GetComponent<RectTransform>().anchoredPosition;
        layout.oilTempInScale[i] = oilTemp.GetComponent<RectTransform>().localScale.x;
        layout.oilTempInInTray[i] = true;
    }

    private void OilTempOutInTray(Layout layout, int i, GameObject oilTemp)
    {
        layout.oilTempOutPos[i] = oilTemp.GetComponent<RectTransform>().anchoredPosition;
        layout.oilTempOutScale[i] = oilTemp.GetComponent<RectTransform>().localScale.x;
        layout.oilTempOutInTray[i] = true;
    }

    private void OilTempPressureInTray(Layout layout, int i, GameObject oilTemp)
    {
        layout.oilTempPressurePos[i] = oilTemp.GetComponent<RectTransform>().anchoredPosition;
        layout.oilTempPressureScale[i] = oilTemp.GetComponent<RectTransform>().localScale.x;
        layout.oilTempPressureInTray[i] = true;
    }

    private void OilTempComboInTray(Layout layout, int i, GameObject oilTemp)
    {
        layout.oilTempComboPos[i] = oilTemp.GetComponent<RectTransform>().anchoredPosition;
        layout.oilTempComboScale[i] = oilTemp.GetComponent<RectTransform>().localScale.x;
        layout.oilTempComboInTray[i] = true;
    }

    private void CylinderHeadInTray(Layout layout, int i, GameObject cylinderHead)
    {
        layout.cylinderHeadPos[i] = cylinderHead.GetComponent<RectTransform>().anchoredPosition;
        layout.cylinderHeadScale[i] = cylinderHead.GetComponent<RectTransform>().localScale.x;
        layout.cylinderHeadInTray[i] = true;
    }

    private void CarbAirInTray(Layout layout, int i, GameObject carbAir)
    {
        layout.carbAirPos[i] = carbAir.GetComponent<RectTransform>().anchoredPosition;
        layout.carbAirScale[i] = carbAir.GetComponent<RectTransform>().localScale.x;
        layout.carbAirInTray[i] = true;
    }

    void SpeedoInTray(Layout layout)
    {
        layout.speedoPos = speedometer.GetComponent<RectTransform>().anchoredPosition;
        layout.speedoScale = speedometer.GetComponent<RectTransform>().localScale.x;
        layout.speedoInTray = true;
    }

    void DialInTray(string name, Layout layout)
    {
        //moving all to individual functions called directly (WIP)

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

    public void DeleteLayout()
    {
        Debug.Log("Deleting = " + airplaneData.planeType);
#if UNITY_ANDROID
        string key = airplaneData.planeType;
#else
        string key = "layout " + slaveManager.id + " " + airplaneData.planeType;
#endif
        PlayerPrefs.DeleteKey(key);
        //put all dials back to country board
        for (int i = 0; i < menuHandler.dialsInTray.Count; i++)
        {
            menuHandler.dialsInTray[i].transform.parent = countryDialBoard.transform;
        }

        InstantiateDials();

        //now reset list
        menuHandler.dialsInTray.Clear();

        //reset tray target
        if (menuHandler.trayPulled)
            menuHandler.TrayPulldown();
        else
            menuHandler.DropDownYTarget();


    }
}
