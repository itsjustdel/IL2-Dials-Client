using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using UnityEngine;

public class AirplaneData : MonoBehaviour
{

    public float scalar0;public float scalar1;
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
    //stores name avaialble dials
    
    public PlaneDataFromName.PlaneAttributes planeAttributes;

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
            //construct country and available dials in to planeAttributes class/struct
            planeAttributes = PlaneDataFromName.AttributesFromName(planeType);
            //set as its own public variable to expose in hierarchy (in unity) for testing ease
            country = planeAttributes.country;


            //empty the trays

            //enable and disable dials depending on plane/country
            SwitchDialsFromCountry();

            //select correct needle form many vsi prefacbs depeding on plane
            SetVSINeedle();

            if (countryDialBoard != null)
               menuHandler.LoadLayout();
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

        //update previous so we can detect a change on next update
        planeTypePrevious = planeType;
    }

    void SetVSINeedle()
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
        


    void SwitchDialsFromCountry()
    {
        //change dials depending on what value we received from the networking component

        //remove any existing dials board prefab in scene
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

            //switch off any unavailable dials to this plane
            DeactivateUnavailableDials(countryDialBoard, planeType, planeAttributes);
        }
        
    }   

    //static to refactor to new class - to do
    static void DeactivateUnavailableDials(GameObject countryDialBoard, string planeName, PlaneDataFromName.PlaneAttributes planeAttributes)
    {
        //check what dials are available and switch off as needed
        //altimeter and speedo are always available
        
        if(!planeAttributes.headingIndicator)
            if(countryDialBoard.transform.Find("Heading Indicator")!= null)
            countryDialBoard.transform.Find("Heading Indicator").gameObject.SetActive(false);

        if (!planeAttributes.turnAndBank)
            if(countryDialBoard.transform.Find("Turn And Bank")!= null)
                countryDialBoard.transform.Find("Turn And Bank").gameObject.SetActive(false);

        if (!planeAttributes.turnCoordinator)
            if(countryDialBoard.transform.Find("Turn Coordinator").gameObject != null)
                countryDialBoard.transform.Find("Turn Coordinator").gameObject.SetActive(false);

        if (!planeAttributes.vsiSmallest)
            if (countryDialBoard.transform.Find("VSI Smallest") != null)
                countryDialBoard.transform.Find("VSI Smallest").gameObject.SetActive(false);

        if (!planeAttributes.vsiSmall)
            if(countryDialBoard.transform.Find("VSI Small")!= null)
                countryDialBoard.transform.Find("VSI Small").gameObject.SetActive(false);

        if (!planeAttributes.vsiLarge)
            if(countryDialBoard.transform.Find("VSI Large")!= null)
                countryDialBoard.transform.Find("VSI Large").gameObject.SetActive(false);

        if (!planeAttributes.repeaterCompass)
            if(countryDialBoard.transform.Find("Repeater Compass")!= null)
                countryDialBoard.transform.Find("Repeater Compass").gameObject.SetActive(false);

        if (!planeAttributes.repeaterCompassAlternate)
            if (countryDialBoard.transform.Find("Repeater Compass Alternate") != null)
                countryDialBoard.transform.Find("Repeater Compass Alternate").gameObject.SetActive(false);

        if (!planeAttributes.artificialHorizon)
            if(countryDialBoard.transform.Find("Artificial Horizon") != null)
                countryDialBoard.transform.Find("Artificial Horizon").gameObject.SetActive(false);


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

}
