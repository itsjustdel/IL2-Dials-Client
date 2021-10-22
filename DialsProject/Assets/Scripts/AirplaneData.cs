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

        if (!planeAttributes.vsiSmall)
            if(countryDialBoard.transform.Find("VSI Small")!= null)
                countryDialBoard.transform.Find("VSI Small").gameObject.SetActive(false);

        if (!planeAttributes.vsiLarge)
            if(countryDialBoard.transform.Find("VSI Large")!= null)
                countryDialBoard.transform.Find("VSI Large").gameObject.SetActive(false);

        if (!planeAttributes.repeaterCompass)
            if(countryDialBoard.transform.Find("Repeater Compass")!= null)
                countryDialBoard.transform.Find("Repeater Compass").gameObject.SetActive(false);

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

    /*

    void LoadLayout()
    {

        //MenuHandler menuHandler = GameObject.FindGameObjectWithTag("MenuHandler").GetComponent<MenuHandler>();

        //Save layout is in MenuHandler

        //first of all empty trays
        ButtonManager.EmptyTrays(menuHandler);

        //grab layout data if available from player prefs
        string jsonFoo = PlayerPrefs.GetString(planeType);
        if (String.IsNullOrEmpty(jsonFoo))
        {

            //set dials to default
            DefaultLayouts(countryDialBoard);
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

        if (countryDialBoard.transform.Find("Heading Indicator") != null)
        {
            GameObject headingIndicator = countryDialBoard.transform.Find("Heading Indicator").gameObject;
            headingIndicator.GetComponent<RectTransform>().anchoredPosition = layout.headingPos;
            headingIndicator.GetComponent<RectTransform>().localScale = new Vector3(layout.headingScale, layout.headingScale, 1f);

            if (layout.headingIndicatorInTray)
                AddToTrayOnLoad(headingIndicator, layout);
        }

        if (countryDialBoard.transform.Find("Turn And Bank") != null)
        {
            GameObject turnAndBank = countryDialBoard.transform.Find("Turn And Bank").gameObject;
            turnAndBank.GetComponent<RectTransform>().anchoredPosition = layout.turnAndBankPos;
            turnAndBank.GetComponent<RectTransform>().localScale = new Vector3(layout.turnAndBankScale, layout.turnAndBankScale, 1f);

            if (layout.turnAndBankInTray)
                AddToTrayOnLoad(turnAndBank, layout);

        }

        if (countryDialBoard.transform.Find("Turn Coordinator") != null)
        {

            GameObject turnIndicator = countryDialBoard.transform.Find("Turn Coordinator").gameObject;
            turnIndicator.GetComponent<RectTransform>().anchoredPosition = layout.turnIndicatorPos;
            turnIndicator.GetComponent<RectTransform>().localScale = new Vector3(layout.turnIndicatorScale, layout.turnIndicatorScale, 1f);

            if (layout.turnIndicatorInTray)
                AddToTrayOnLoad(turnIndicator, layout);
        }

        //both vsi share the same variable - only one vsi per plane
        if (countryDialBoard.transform.Find("VSI Small") != null)
        {

            GameObject vsi = countryDialBoard.transform.Find("VSI Small").gameObject;
            vsi.GetComponent<RectTransform>().anchoredPosition = layout.vsiSmallPos;
            vsi.GetComponent<RectTransform>().localScale = new Vector3(layout.vsiSmallScale, layout.vsiSmallScale, 1f);

            if (layout.vsiSmallInTray)
                AddToTrayOnLoad(vsi, layout);
        }

        //both vsi share the same variable - only one vsi per plane
        if (countryDialBoard.transform.Find("VSI Large") != null)
        {

            GameObject vsi = countryDialBoard.transform.Find("VSI Large").gameObject;
            vsi.GetComponent<RectTransform>().anchoredPosition = layout.vsiLargePos;
            vsi.GetComponent<RectTransform>().localScale = new Vector3(layout.vsiLargeScale, layout.vsiLargeScale, 1f);

            if (layout.vsiLargeInTray)
                AddToTrayOnLoad(vsi, layout);
        }

        if (countryDialBoard.transform.Find("Artificial Horizon") != null)
        {

            GameObject artificialHorizon= countryDialBoard.transform.Find("Artificial Horizon").gameObject;
            artificialHorizon.GetComponent<RectTransform>().anchoredPosition = layout.artificialHorizonPos;
            artificialHorizon.GetComponent<RectTransform>().localScale = new Vector3(layout.artificialHorizonScale, layout.artificialHorizonScale, 1f);

            if (layout.repeaterCompassInTray)
                AddToTrayOnLoad(artificialHorizon, layout);
        }

        if (countryDialBoard.transform.Find("Repeater Compass") != null)
        {

            GameObject repeaterCompass = countryDialBoard.transform.Find("Repeater Compass").gameObject;
            repeaterCompass.GetComponent<RectTransform>().anchoredPosition = layout.repeaterCompassPos;
            repeaterCompass.GetComponent<RectTransform>().localScale = new Vector3(layout.repeaterCompassScale, layout.repeaterCompassScale, 1f);

            if (layout.repeaterCompassInTray)
                AddToTrayOnLoad(repeaterCompass, layout);
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
        int longestRow = (int)Mathf.Ceil(f/ 2);
        longestRow *= 300;//300 default step between dials

        GameObject canvasObject = GameObject.FindGameObjectWithTag("Canvas");
        //if longer than the canvas width
        UnityEngine.Debug.Log("longest row = " + longestRow);
        UnityEngine.Debug.Log("canvas X = " + canvasObject.GetComponent<RectTransform>().rect.width);

        float scale = 1f;
        if (longestRow > canvasObject.GetComponent<RectTransform>().rect.width)
        {
            UnityEngine.Debug.Log("row longer than canvas");

            //use this ratio for all positional calculations
            scale =  canvasObject.GetComponent<RectTransform>().rect.width / longestRow;

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
                
                int x = ((int)((activeDials.Count-1) / 2)) * -150;
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
                int diff = activeDials.Count-1 + odd - (activeDials.Count/2 );
                int x = ((int)(diff) );
                x*= -150;
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

    */
}
