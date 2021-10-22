using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MenuHandler : MonoBehaviour
{
    public AirplaneData airplaneData;
    
    public bool deletePrefs = false;
    public GameObject title;    
    public GameObject missionStart;
    public GameObject welcomePanel;
    public GameObject serverMessagePanel;
    public GameObject blurPanel;
    public GameObject menuPanel;
    public GameObject menuButton;
    public GameObject connectionPanel;
    public GameObject layoutPanel;
    public GameObject layoutButton;

    public GameObject trayParent;
    public bool trayOpen;
    public List<GameObject> trayObjects;
    public List<GameObject> dialsInTray;
    public List<GameObject> dialsOnBoard;
    public GameObject connectionsButton;
    public GameObject ledParent;
    public GameObject ipTextField;
    public GameObject portTextField;
    public GameObject scanDebug;
    public TCPClient tcpClient;
    public bool dontShowAgain;
    public Toggle dontShowAgainToggle;
    public bool ipFieldOpen;
    public bool portFieldOpen;

    //messages
    public GameObject layoutWarningMessage;

    //override UI toggle fire on first frame
    private bool dontFire;


    //opening animations
  
    public float slideSpeed = 1f;    
    private Color missionStartColor;
    private Color titleColor;
    private bool glowDirection;
    public float missionGlowSpeed = 1f;
    public float titleFadeSpeed = 1f;
    public float startLEDFade = 1f;
    public float startMissionGlowFade = 1f;
    //public bool fadeLeds = false;
    public System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
    //idle timer
    public float idleTimer;
    private Vector3 mousePos;
    public  bool layoutOpen;


    public void Start()
    {
        mousePos = Input.mousePosition;

        if (deletePrefs)
        {
            PlayerPrefs.DeleteAll();
        }

        //load preferences
        tcpClient.userIP = PlayerPrefs.GetString("IPAddress");
        tcpClient.hostName = PlayerPrefs.GetString("IPAddress");//flaw in design, why host name and user ip?
        tcpClient.portNumber = PlayerPrefs.GetInt("PortNumber");



        //toggle         
        //first frame, don't fire event, just set bool value
        dontFire = true;
        dontShowAgain = PlayerPrefs.GetInt("dontshowagain") == 1 ? true : false;
        dontShowAgainToggle.isOn = dontShowAgain;//this would fire the toggle function
        dontFire = false;


        if (dontShowAgain)
        {
            welcomePanel.SetActive(false);
            //disable blur
            blurPanel.SetActive(false);
            //buttons and leds
            ledParent.SetActive(true);
            menuButton.SetActive(true);
        }
        else
        {
            //hello!
            welcomePanel.SetActive(true);
            
            blurPanel.SetActive(true);
            //buttons and leds
            ledParent.SetActive(false);
            menuButton.SetActive(false);
        }


        if (tcpClient.portNumber == 0)
            tcpClient.portNumber = 11200;

        //apply to UI
        //inlcude inactive with bool flag
        ipTextField.GetComponentInParent<InputField>(true).text = tcpClient.userIP;

        Debug.Log(tcpClient.portNumber);
        if (tcpClient.portNumber == 11200 || tcpClient.portNumber == 0)
        {
            //set default port number text field to null so unity shows the greyed out placeholder text
            Debug.Log("0");
            portTextField.GetComponentInParent<InputField>(true).text = null;
        }
        else
        {
            Debug.Log("1");
            portTextField.GetComponentInParent<InputField>(true).text = tcpClient.portNumber.ToString();
        }


        //force this false to start with, above code flags this as true because we changed a value
        ipFieldOpen = false;
        portFieldOpen = false;


        //set alhpa in mission glow and title to 0 - this way we can see it in the editor before we press play
        missionStartColor = missionStart.GetComponent<Text>().color;
        missionStartColor.a = 0;
        missionStart.GetComponent<Text>().color = missionStartColor;


        titleColor = title.GetComponent<Image>().color;
        titleColor.a = 0;
        title.GetComponent<Image>().color = titleColor;
        


    }

    public void Update()
    {
        //SlideMask();

        RemoveTitle();
                

        if (airplaneData.country != AirplaneData.Country.UNDEFINED)
            RemoveTitle();
        else
            EnableTitle();


        if (!welcomePanel.activeInHierarchy)
        {
            if (!stopwatch.IsRunning)
                stopwatch.Start();

            TitleFade();

            MissionStartGlow();
        }

        IdleTimer();
    }

    void IdleTimer()
    {
        //TODO  if android add click?

        //return if intro animation not finished
        if (!GetComponent<LEDs>().fadeInComplete)
            return;

        //if mouse is not moving, update timer
        if (Input.mousePosition == mousePos)
            idleTimer += Time.deltaTime;
        //else, reset timer
        else
            idleTimer = 0f;


        //click can reset timer too - works on android?
        if (Input.GetButtonDown("Fire1"))
        {
            idleTimer = 0f;
        }    

        //update for next frame
        mousePos = Input.mousePosition;

        if(idleTimer == 0f)
        {
            Color color = menuButton.GetComponent<Image>().color;
            color.a = 1f;// * Time.deltaTime;
            menuButton.GetComponent<Image>().color = color;
        }

        if(idleTimer > 10f)
        {
            int fadeInSpeed = 10;
            Color color = menuButton.GetComponent<Image>().color;
            color.a -= 1f * Time.deltaTime;
            menuButton.GetComponent<Image>().color = color;
        }

    }

    void RemoveTitle()
    {
        title.SetActive(false);
        missionStart.SetActive(false);
    }
    void EnableTitle()
    {
        title.SetActive(true);
        missionStart.SetActive(true);
    }


   
    void TitleFade()
    {
        

        if (titleColor.a < 1f)
        {
          
            //alpha starts at 0 in hierarchy so it fades in to view

            titleColor.a += Time.deltaTime * titleFadeSpeed;
            title.GetComponent<Image>().color = titleColor;
        }
        else
        {

            if(stopwatch.ElapsedMilliseconds > startLEDFade*1000)
            {
                //led script on same object
                if(!GetComponent<LEDs>().fadeInProgress && !GetComponent<LEDs>().fadeInComplete)                   
                    GetComponent<LEDs>().startFadeIn = true;
            }
        }

        
        
    }

    public void MissionStartGlow()
    {

        //wait until slide is finished
        if (stopwatch.ElapsedMilliseconds < startMissionGlowFade*1000)
            return;

        //missionStartColor = missionStart.GetComponent<Image>().color;

        //alpha starts at 0 in hierarchy so it fades in to view
        if (!glowDirection)
        {
            missionStartColor.a += Time.deltaTime * missionGlowSpeed;
        }
        else
        {
            missionStartColor.a -= Time.deltaTime * missionGlowSpeed;
        }

        missionStart.GetComponent<Text>().color = missionStartColor; 

        if(missionStartColor.a < 0f)
        {
            missionStartColor.a = 0f;
            glowDirection = !glowDirection;
        }
        else if(missionStartColor.a > 1f)
        {
            missionStartColor.a = 1f;
            glowDirection = !glowDirection;
        }
    }

    

    public void InputFieldOpen11()
    {
        // Debug.Log("ip open");
        //pause the game if autosearching, makes input smoother because scan is cpu heavy for mobile device
        ipFieldOpen = true;

    }

    public void MenuButtonClicked()
    {

        if(!connectionPanel.activeInHierarchy && !menuPanel.activeInHierarchy)
        {
            //everything closed, open menu panel
            Debug.Log("Opening menu from closed");
            menuPanel.SetActive(true);
            blurPanel.SetActive(true);
        }


        else if (menuPanel.activeInHierarchy)
        {
            //close from main main menu 
            Debug.Log("Closing from main menu");
            menuPanel.SetActive(false);
            //
            blurPanel.SetActive(false);
            layoutWarningMessage.SetActive(false);

        }
        
        else if(connectionPanel.activeInHierarchy)
        {
            //close from connection panel
            Debug.Log("Closing from connection panel");
            connectionPanel.SetActive(false);
            menuPanel.SetActive(false);
            blurPanel.SetActive(false);
            layoutWarningMessage.SetActive(false);
        }        

        else if (layoutOpen)
        {
            //leaving layout screen
            Debug.Log("Closing menu from layout");                

            //point to accept layout function - we can close this page from menu button or accept button
            AcceptLayoutClick();

            layoutWarningMessage.SetActive(false);

        }
       
            
        //blurPanel.SetActive(!blurPanel.activeSelf);
        //menuPanel.SetActive(!menuPanel.activeSelf);
        //connectionPanel.SetActive(false);
    }

    public void IPAddressChanged()
    {
        Debug.Log("IP changed");
        string ipAddressText = ipTextField.GetComponent<Text>().text;
        if (string.IsNullOrEmpty(ipAddressText))
        {
            //placeholder text should show and we set ip to empty, when empty, it autoscans
            tcpClient.userIP = null;
            //save to player prefs for next load
            PlayerPrefs.SetString("IPAddress", ipAddressText);
            PlayerPrefs.Save();



            Debug.Log("2");
        }
        else
        {
            //give tcpClient the user Ip
            //interface variable
            tcpClient.userIP = ipAddressText;
            //code variable
            tcpClient.hostName = ipAddressText;
            //save
            PlayerPrefs.SetString("IPAddress", ipAddressText);
            PlayerPrefs.Save();


            //let user know what's happening
            //            scanDebug.GetComponent<Text>().text = "Attempting Connection: " + tcpClient.hostName.ToString(); ;

            Debug.Log("3");

        }

        //reset autoscan variables
        tcpClient.ip3 = 0;
        tcpClient.ip4 = 4;

        //flag for autoscan pause
        ipFieldOpen = false;

        tcpClient.hostFound = false;


        tcpClient.timer = tcpClient.socketTimeoutTime;


    }

    public void PortChanged()
    {
        Debug.Log("Port changed");

        string portText = portTextField.GetComponent<Text>().text;
        if (string.IsNullOrEmpty(portText))
        {
            //set to default port
            tcpClient.portNumber = 11200;

            //save to player prefs for next load
            PlayerPrefs.SetInt("PortNumber", 11200);

            Debug.Log("4");
        }
        else
        {
            //set port to user input
            int parsed = int.Parse(portText);
            tcpClient.portNumber = parsed;
            //save
            PlayerPrefs.SetInt("PortNumber", parsed);

            Debug.Log("6");

        }

        tcpClient.ip3 = 0;
        tcpClient.ip4 = 4;

        //flag for autoscan pause
        portFieldOpen = false;

        tcpClient.hostFound = false;

        tcpClient.timer = tcpClient.socketTimeoutTime;
    }

    public void WelcomeClosed()
    {
        welcomePanel.SetActive(false);
        blurPanel.SetActive(false);

        //turn the leds and menu button on
        ledParent.SetActive(true);
        menuButton.SetActive(true);
    }

    public void ServerMessageOpen()
    {
        serverMessagePanel.SetActive(true);
        blurPanel.SetActive(true);
        menuButton.SetActive(false);
    }

    public void ServerMessageClosed()
    {
        serverMessagePanel.SetActive(false);
    }

    public void DontShowAgainToggle()
    {
        //first frame
        if (dontFire)
            return;

        dontShowAgain = !dontShowAgain;        
        //not saving to player prefs

        //set pref - ternary to set integer (no bool value in prefs)
        PlayerPrefs.SetInt("dontshowagain", dontShowAgain ? 1 : 0);
        PlayerPrefs.Save();

    }

    public void OpenLayoutClick()
    {
        Debug.Log("Layout Click");
        layoutOpen = true;
        
        //check for plane - can only organise if plane loaded
        if(airplaneData.country == AirplaneData.Country.UNDEFINED)
        {
            layoutWarningMessage.SetActive(true);
            Debug.Log("Layout Warning message");
            return;
        }
        else
            layoutWarningMessage.SetActive(false);

        //remove metal panel
        menuPanel.SetActive(false);

        //hide Leds and Menu button
        menuButton.SetActive(false);
        ledParent.SetActive(false);

        

        //turn blur off so we can see what we are doing with the dials
        blurPanel.SetActive(false);
        //turn our new menu on
        layoutPanel.SetActive(true);
        //work out how it should look
        UpdateLayoutPanel();


        //show dial controls for each dial
        TurnHandlersOn();


        DeActivateCompassTouch();

    }

    public void AcceptLayoutClick()
    {
        //go back to main page
        layoutPanel.SetActive(false);
        //turn icon handlers off 
        TurnHandlersOff();

        //tunr menu button and leds back on
        menuButton.SetActive(true);
        ledParent.SetActive(true);

        layoutOpen = false;

        //turn compasses back on 
        ActivateCompassTouch();

        SaveLayout();

    }

    public void AddLayouButtonClick()
    {
        Debug.Log("Add layout click");
        trayOpen = !trayOpen;
        UpdateLayoutPanel();
    }

    public void OpenConnectionsClick()
    {
        Debug.Log("Connections Click");
        menuPanel.SetActive(false);
    
        //show copnnection panel - IP address, port etc
        connectionPanel.SetActive(true);
    }



    public void UpdateLayoutPanel()
    {
        //only show how many blank tray space we need to
        for (int i = 0; i < trayObjects.Count; i++)
        {
          //  Debug.Log(trayObjects[i].transform.childCount);
            if(trayObjects[i].transform.childCount != 0)            
                trayObjects[i].SetActive(true);
            
            else
                trayObjects[i].SetActive(false);
        }
    }
   
    private void DeActivateCompassTouch()
    {
        //remove compass interactiveness
        GameObject[] compassSpinners = GameObject.FindGameObjectsWithTag("CompassSpin");

        Debug.Log("compasses = " + compassSpinners.Length);
        for (int i = 0; i < compassSpinners.Length; i++)
        {
            compassSpinners[i].GetComponent<Image>().raycastTarget = false;
        }


    }

    private void ActivateCompassTouch()
    {
        //compass interactiveness go!
        GameObject[] compassSpinners = GameObject.FindGameObjectsWithTag("CompassSpin");

        for (int i = 0; i < compassSpinners.Length; i++)
        {
            compassSpinners[i].GetComponent<Image>().raycastTarget = true;
        }

    }

    private void TurnHandlersOn()
    {
        //check if in tray?

        
        GameObject[] UIhandlers = GameObject.FindGameObjectsWithTag("UIHandler");

        


        for (int i = 0; i < UIhandlers.Length; i++)
        {
            //turn image off, not gameobject, find with tag can't find find inactive objects

            //if in tray, don't do this
            if(!dialsInTray.Contains( UIhandlers[i].transform.parent.parent.gameObject))
                UIhandlers[i].GetComponent<Image>().enabled = true;
        }
    }

    private void TurnHandlersOff()
    {
        GameObject[] UIhandlers = GameObject.FindGameObjectsWithTag("UIHandler");
        for (int i = 0; i < UIhandlers.Length; i++)
        {
            UIhandlers[i].GetComponent<Image>().enabled = false;
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
            DefaultLayouts(airplaneData.countryDialBoard);
            return;
        }

        //rebuild json
        Layout layout = JsonUtility.FromJson<Layout>(jsonFoo);

        //check for version change
        if(layout.version != airplaneData.clientVersion)
        {
            //reset all dials :(

            //set dials to default
            Debug.Log("Version change detected");
            DefaultLayouts(airplaneData.countryDialBoard);
            return;
        }

        //apply to dials/positions

        GameObject speedometer = airplaneData.countryDialBoard.transform.Find("Speedometer").gameObject;
        speedometer.GetComponent<RectTransform>().anchoredPosition = layout.speedoPos;
        speedometer.GetComponent<RectTransform>().localScale = new Vector3(layout.speedoScale, layout.speedoScale, 1f);

        if (layout.speedoInTray)
            AddToTrayOnLoad(speedometer, layout, menuHandler);

        GameObject altimeter = airplaneData.countryDialBoard.transform.Find("Altimeter").gameObject;
        altimeter.GetComponent<RectTransform>().anchoredPosition = layout.altPos;
        altimeter.GetComponent<RectTransform>().localScale = new Vector3(layout.altScale, layout.altScale, 1f);


        if (layout.altimeterInTray)
            AddToTrayOnLoad(altimeter, layout, menuHandler);

        if (airplaneData.countryDialBoard.transform.Find("Heading Indicator") != null)
        {
            GameObject headingIndicator = airplaneData.countryDialBoard.transform.Find("Heading Indicator").gameObject;
            headingIndicator.GetComponent<RectTransform>().anchoredPosition = layout.headingPos;
            headingIndicator.GetComponent<RectTransform>().localScale = new Vector3(layout.headingScale, layout.headingScale, 1f);

            if (layout.headingIndicatorInTray)
                AddToTrayOnLoad(headingIndicator, layout, menuHandler);
        }

        if (airplaneData.countryDialBoard.transform.Find("Turn And Bank") != null)
        {
            GameObject turnAndBank = airplaneData.countryDialBoard.transform.Find("Turn And Bank").gameObject;
            turnAndBank.GetComponent<RectTransform>().anchoredPosition = layout.turnAndBankPos;
            turnAndBank.GetComponent<RectTransform>().localScale = new Vector3(layout.turnAndBankScale, layout.turnAndBankScale, 1f);

            if (layout.turnAndBankInTray)
                AddToTrayOnLoad(turnAndBank, layout, menuHandler);

        }

        if (airplaneData.countryDialBoard.transform.Find("Turn Coordinator") != null)
        {

            GameObject turnIndicator = airplaneData.countryDialBoard.transform.Find("Turn Coordinator").gameObject;
            turnIndicator.GetComponent<RectTransform>().anchoredPosition = layout.turnIndicatorPos;
            turnIndicator.GetComponent<RectTransform>().localScale = new Vector3(layout.turnIndicatorScale, layout.turnIndicatorScale, 1f);

            if (layout.turnIndicatorInTray)
                AddToTrayOnLoad(turnIndicator, layout, menuHandler);
        }

        //both vsi share the same variable - only one vsi per plane
        if (airplaneData.countryDialBoard.transform.Find("VSI Small") != null)
        {

            GameObject vsi = airplaneData.countryDialBoard.transform.Find("VSI Small").gameObject;
            vsi.GetComponent<RectTransform>().anchoredPosition = layout.vsiSmallPos;
            vsi.GetComponent<RectTransform>().localScale = new Vector3(layout.vsiSmallScale, layout.vsiSmallScale, 1f);

            if (layout.vsiSmallInTray)
                AddToTrayOnLoad(vsi, layout, menuHandler);
        }

        //both vsi share the same variable - only one vsi per plane
        if (airplaneData.countryDialBoard.transform.Find("VSI Large") != null)
        {

            GameObject vsi = airplaneData.countryDialBoard.transform.Find("VSI Large").gameObject;
            vsi.GetComponent<RectTransform>().anchoredPosition = layout.vsiLargePos;
            vsi.GetComponent<RectTransform>().localScale = new Vector3(layout.vsiLargeScale, layout.vsiLargeScale, 1f);

            if (layout.vsiLargeInTray)
                AddToTrayOnLoad(vsi, layout, menuHandler);
        }

        if (airplaneData.countryDialBoard.transform.Find("Artificial Horizon") != null)
        {

            GameObject artificialHorizon = airplaneData.countryDialBoard.transform.Find("Artificial Horizon").gameObject;
            artificialHorizon.GetComponent<RectTransform>().anchoredPosition = layout.artificialHorizonPos;
            artificialHorizon.GetComponent<RectTransform>().localScale = new Vector3(layout.artificialHorizonScale, layout.artificialHorizonScale, 1f);

            if (layout.repeaterCompassInTray)
                AddToTrayOnLoad(artificialHorizon, layout, menuHandler);
        }

        if (airplaneData.countryDialBoard.transform.Find("Repeater Compass") != null)
        {

            GameObject repeaterCompass = airplaneData.countryDialBoard.transform.Find("Repeater Compass").gameObject;
            repeaterCompass.GetComponent<RectTransform>().anchoredPosition = layout.repeaterCompassPos;
            repeaterCompass.GetComponent<RectTransform>().localScale = new Vector3(layout.repeaterCompassScale, layout.repeaterCompassScale, 1f);

            if (layout.repeaterCompassInTray)
                AddToTrayOnLoad(repeaterCompass, layout, menuHandler);
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

        if (airplaneData.countryDialBoard.transform.Find("Speedometer") != null)
        {
            layout.speedoPos = airplaneData.countryDialBoard.transform.Find("Speedometer").GetComponent<RectTransform>().anchoredPosition;
            layout.speedoScale = airplaneData.countryDialBoard.transform.Find("Speedometer").GetComponent<RectTransform>().localScale.x;
        }
        else
            //if we don't find it, look for it in the tray
            DialInTray("Speedometer", layout);


        if (airplaneData.countryDialBoard.transform.Find("Altimeter") != null)
        {
            layout.altPos = airplaneData.countryDialBoard.transform.Find("Altimeter").GetComponent<RectTransform>().anchoredPosition;
            layout.altScale = airplaneData.countryDialBoard.transform.Find("Altimeter").GetComponent<RectTransform>().localScale.x;
        }
        else
            DialInTray("Altimeter", layout);


        if (airplaneData.countryDialBoard.transform.Find("Heading Indicator") != null)
        {
            layout.headingPos = airplaneData.countryDialBoard.transform.Find("Heading Indicator").GetComponent<RectTransform>().anchoredPosition;
            layout.headingScale = airplaneData.countryDialBoard.transform.Find("Heading Indicator").GetComponent<RectTransform>().localScale.x;
        }
        else
            DialInTray("Heading Indicator", layout);


        if (airplaneData.countryDialBoard.transform.Find("Turn And Bank") != null)
        { 
            layout.turnAndBankPos = airplaneData.countryDialBoard.transform.Find("Turn And Bank").GetComponent<RectTransform>().anchoredPosition;
        layout.turnAndBankScale = airplaneData.countryDialBoard.transform.Find("Turn And Bank").GetComponent<RectTransform>().localScale.x;
        }
        else        
            DialInTray("Turn And Bank", layout);


        if (airplaneData.countryDialBoard.transform.Find("Turn Coordinator") != null)
        {
            layout.turnIndicatorPos = airplaneData.countryDialBoard.transform.Find("Turn Coordinator").GetComponent<RectTransform>().anchoredPosition;
            layout.turnIndicatorScale = airplaneData.countryDialBoard.transform.Find("Turn Coordinator").GetComponent<RectTransform>().localScale.x;
        }
        else
            DialInTray("Turn Coordinator", layout);


        if (airplaneData.countryDialBoard.transform.Find("VSI Small") != null)
        {
            layout.vsiSmallPos = airplaneData.countryDialBoard.transform.Find("VSI Small").GetComponent<RectTransform>().anchoredPosition;
            layout.vsiSmallScale = airplaneData.countryDialBoard.transform.Find("VSI Small").GetComponent<RectTransform>().localScale.x;
        }

        else
            DialInTray("VSI Small", layout);


        if (airplaneData.countryDialBoard.transform.Find("VSI Large") != null)
        {
            layout.vsiLargePos = airplaneData.countryDialBoard.transform.Find("VSI Large").GetComponent<RectTransform>().anchoredPosition;
            layout.vsiLargeScale = airplaneData.countryDialBoard.transform.Find("VSI Large").GetComponent<RectTransform>().localScale.x;
        }
        else
            DialInTray("VSI Large", layout);


        if (airplaneData.countryDialBoard.transform.Find("Artificial Horizon") != null)
        {
            layout.artificialHorizonPos = airplaneData.countryDialBoard.transform.Find("Artificial Horizon").GetComponent<RectTransform>().anchoredPosition;
            layout.artificialHorizonScale = airplaneData.countryDialBoard.transform.Find("Artificial Horizon").GetComponent<RectTransform>().localScale.x;
        }
        else
            DialInTray("Artificial Horizon", layout);


        if (airplaneData.countryDialBoard.transform.Find("Repeater Compass") != null)
        {
            layout.repeaterCompassPos = airplaneData.countryDialBoard.transform.Find("Repeater Compass").GetComponent<RectTransform>().anchoredPosition;
            layout.repeaterCompassScale = airplaneData.countryDialBoard.transform.Find("Repeater Compass").GetComponent<RectTransform>().localScale.x;
        }
        else
            DialInTray("Repeater Compass", layout);



        //pack with json utility
        string jsonFoo = JsonUtility.ToJson(layout);

        //save packed string to player preferences (unity)
        PlayerPrefs.SetString(airplaneData.planeType, jsonFoo);
        PlayerPrefs.Save();

    }

    void DialInTray(string name, Layout layout)
    {
        
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
        UnityEngine.Debug.Log("longest row = " + longestRow);
        UnityEngine.Debug.Log("canvas X = " + canvasObject.GetComponent<RectTransform>().rect.width);

        float scale = 1f;
        if (longestRow > canvasObject.GetComponent<RectTransform>().rect.width)
        {
            UnityEngine.Debug.Log("row longer than canvas");

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

    void AddToTrayOnLoad(GameObject dial, Layout layout, MenuHandler menuHandler)
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