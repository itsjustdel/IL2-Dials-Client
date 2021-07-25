using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using UnityEngine;

public class AirplaneData : MonoBehaviour
{
    public bool useTestPlane;
    public GameObject testPlane;

    public float clientVersion = 0.3f; //manually update this
    public float serverVersion;
    public enum Country
    {
        RU,
        GER,
        US,
        UK,
        ITA,
        UNDEFINED

    }

    public string planeType;
    public string planeTypePrevious;
    public Country country = Country.RU;
    public float altitude;
    public float mmhg;
    public float airspeed;
    public float pitch;
    public float roll;
    public float rollPrev;
    public float verticalSpeed;
    public float turnCoordinatorBall;
    public float turnCoordinatorNeedle;
    public float heading;
    public float headingPrevious;
    public float headingPreviousPrevious;


    //public List<GameObject> countryDials = new List<GameObject>();
    public GameObject countryDialBoard;

    
    private Country previousCountry;

    public BuildControl buildControl;
    public MenuHandler menuHandler;
    public TCPClient tcpClient;
    //fixed update is enough for checking status
    void FixedUpdate()
    {
        //check client version against incoming server message
        CheckVersion();

        //check for a plane switch
        CheckForPlaneChange();
    }


    void CheckVersion()
    {
        
        //checks version and shows message if mismatch (if connected)
        if (tcpClient.connected)
        {
            if (serverVersion != clientVersion)
            {
                //show server message
                menuHandler.ServerMessageOpen();
            }
            else
            {
                //all good
                menuHandler.ServerMessageClosed();
            }
        }
        else
        {
            //we are not connected to anything we don't know anything about version numbersd
            menuHandler.ServerMessageClosed();
        }
    }

    void CheckForPlaneChange()
    {
        if (planeType != planeTypePrevious)
        {
            country = PlaneCountryFromName.AsignCountryFromName(planeType);

            //empty the trays


            //enable and disable dials depending on plane/country
            SwitchDialsFromCountry();

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

        planeTypePrevious = planeType;
    }

    void SwitchDialsFromCountry()
    {
        //change dials depending on what value we received from the networking component


        //switch all off
        /*
        for (int i = 0; i < countryDials.Count; i++)
        {
            countryDials[i].SetActive(false);
        }
        */

        //remove dials board prefab
        if(countryDialBoard != null)
            Destroy(countryDialBoard);


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
            //asign variables - can't asign scen variables to prefab in editor
            tcpClient.rN = countryDialBoard.GetComponent<RotateNeedle>();
            tcpClient.rN.buildControl = GameObject.Find("Build Chooser").GetComponent<BuildControl>();
            tcpClient.rN.iL2GameDataClient = GameObject.Find("Player Plane").GetComponent<AirplaneData>();
            tcpClient.rN.tcpClient = GameObject.Find("Networking").GetComponent<TCPClient>();
        }
        
    }   

    public static int CountryIndexFromEnum(AirplaneData.Country country)
    {
        //child position in hierarchy
        int countryIndex = 0;
        switch (country)
        {
            case AirplaneData.Country.RU:
                countryIndex = 0;
                break;
            case AirplaneData.Country.GER:
                countryIndex = 1;
                break;
            case AirplaneData.Country.US:
                countryIndex = 2;
                break;
            case AirplaneData.Country.UK:
                countryIndex = 3;
                break;
            case AirplaneData.Country.ITA:
                countryIndex = 4;
                break;
        }

        return countryIndex;
    }

    void LoadLayout()
    {

        //first of all empty trays
        ButtonManager.EmptyTrays(menuHandler);

        //grab layout data if available from player prefs
        string jsonFoo = PlayerPrefs.GetString(planeType);
        if (String.IsNullOrEmpty(jsonFoo))
        {

            //set dials to default

            return;
        }

        //rebuild json
        Layout layout = JsonUtility.FromJson<Layout>(jsonFoo);

        //apply to dials/positions
        
        GameObject speedometer = countryDialBoard.transform.Find("Speedometer").gameObject;
        speedometer.GetComponent<RectTransform>().anchoredPosition = layout.speedoPos;
        speedometer.GetComponent<RectTransform>().localScale = new Vector3(layout.speedoScale, layout.speedoScale, 1f);

        if (layout.speedoInTray)
            AddToTrayOnLoad(speedometer, layout);
     
        GameObject altimeter = countryDialBoard.transform.Find("Altimeter").gameObject;
        altimeter.GetComponent<RectTransform>().anchoredPosition = layout.altPos;
        altimeter.GetComponent<RectTransform>().localScale = new Vector3(layout.altScale, layout.altScale, 1f);


        if (layout.altimeterInTray)
            AddToTrayOnLoad(altimeter, layout);
     

        GameObject headingIndicator = countryDialBoard.transform.Find("Heading Indicator").gameObject;
        headingIndicator.GetComponent<RectTransform>().anchoredPosition = layout.headingPos;
        headingIndicator.GetComponent<RectTransform>().localScale = new Vector3(layout.headingScale, layout.headingScale, 1f);

        if (layout.headingIndicatorInTray)
            AddToTrayOnLoad(headingIndicator, layout);
     
       
        GameObject turnAndBank = countryDialBoard.transform.Find("Turn And Bank").gameObject;
        turnAndBank.GetComponent<RectTransform>().anchoredPosition = layout.turnAndBankPos;
        turnAndBank.GetComponent<RectTransform>().localScale = new Vector3(layout.turnAndBankScale, layout.turnAndBankScale, 1f);

        if (layout.turnAndBankInTray)
            AddToTrayOnLoad(turnAndBank, layout);
     
       
        GameObject turnIndicator = countryDialBoard.transform.Find("Turn Coordinator").gameObject;
        turnIndicator.GetComponent<RectTransform>().anchoredPosition = layout.turnIndicatorPos;
        turnIndicator.GetComponent<RectTransform>().localScale = new Vector3(layout.turnIndicatorScale, layout.turnIndicatorScale, 1f);

        if (layout.turnIndicatorInTray)
            AddToTrayOnLoad(turnIndicator, layout);
      
       
        GameObject vsi = countryDialBoard.transform.Find("VSI").gameObject;
        vsi.GetComponent<RectTransform>().anchoredPosition = layout.vsiPos;
        vsi.GetComponent<RectTransform>().localScale = new Vector3(layout.vsiScale, layout.vsiScale, 1f);

        if (layout.vsiInTray)
            AddToTrayOnLoad(vsi, layout);

        
       


    }

    void AddToTrayOnLoad(GameObject dial, Layout layout)
    {

        //list
        //menuHandler.dialsInTray.Add(dial);
        //in hierarchy
        ButtonManager.PutDialInTray(dial, menuHandler);

        //apply transforms
//        dial.GetComponent<RectTransform>().anchoredPosition = layout.speedoPos;
        //dial.GetComponent<RectTransform>().localScale = new Vector3(layout.speedoScale, layout.speedoScale, 1f);
    }
}
