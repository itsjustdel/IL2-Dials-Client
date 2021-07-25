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
    public bool fadeLeds = false;
    public System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();


    public  bool layoutOpen;
    public void Start()
    {

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
            //enable blur
            blurPanel.SetActive(false);
            //buttons and leds
            ledParent.SetActive(true);
            menuButton.SetActive(true);
        }
        else
        {
            welcomePanel.SetActive(true);
            //straight to unblurred dials
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
                fadeLeds = true;
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
        

        

        //
        if(connectionPanel.activeInHierarchy)
        {
            Debug.Log("Menu Clicked 0");
            connectionPanel.SetActive(false);
            menuPanel.SetActive(false);
        }

        else if (!menuPanel.activeInHierarchy)
        {
            if(layoutOpen)
            {
                //leaving layout screen
                Debug.Log("Closing menu from layout");
                

                //point to accept layout function - we can close this page from menu button or accept button
                AcceptLayoutClick();

                
                
            }
            else
            {
                Debug.Log("Opening menu from closed");
                menuPanel.SetActive(true);
            }


        }
        else if (menuPanel.activeInHierarchy)
        {
            Debug.Log("Menu Clicked 2");
            menuPanel.SetActive(false);
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
       // menuButton.SetActive(false);
        ledParent.SetActive(false);

        

        //turn blur off so we can see what we are doing with the dials
        blurPanel.SetActive(false);
        //turn our new menu on
        layoutPanel.SetActive(true);
        //work out how it should look
        UpdateLayoutPanel();


        //show dial controls for each dial
        TurnHandlersOn(); 



        //show add dial button

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
        

    public void SaveLayout()
    {
        //use class to write with json // https://forum.unity.com/threads/how-would-i-do-the-following-in-playerprefs.397516/#post-2595609
        Layout layout = new Layout();
        layout.planeType = airplaneData.planeType;

        //get position in hierarchy
        //int countryIndex = AirplaneData.CountryIndexFromEnum(airplaneData.country);      

        //use location of current Rotate Needle script to get dials positions

        //look for dial on dashboard - original parent
        try
        {
            GameObject speedometer = airplaneData.countryDialBoard.transform.Find("Speedometer").gameObject;

            layout.speedoPos = airplaneData.countryDialBoard.transform.Find("Speedometer").GetComponent<RectTransform>().anchoredPosition;
            layout.speedoScale = airplaneData.countryDialBoard.transform.Find("Speedometer").GetComponent<RectTransform>().localScale.x;
        }
        catch
        {
            //if we don't find it, look for it in the tray
            DialInTray("Speedometer", layout);
        }
        
        try
        {
            GameObject altimeter = airplaneData.countryDialBoard.transform.Find("Altimeter").gameObject;
            layout.altPos = airplaneData.countryDialBoard.transform.Find("Altimeter").GetComponent<RectTransform>().anchoredPosition;
            layout.altScale = airplaneData.countryDialBoard.transform.Find("Altimeter").GetComponent<RectTransform>().localScale.x;
        }
        catch
        {
            DialInTray("Altimeter", layout);
        }

        try
        {
            GameObject headingIndicator = airplaneData.countryDialBoard.transform.Find("Heading Indicator").gameObject;
            layout.headingPos = airplaneData.countryDialBoard.transform.Find("Heading Indicator").GetComponent<RectTransform>().anchoredPosition;
            layout.headingScale = airplaneData.countryDialBoard.transform.Find("Heading Indicator").GetComponent<RectTransform>().localScale.x;
        }
        catch
        {
            DialInTray("Heading Indicator", layout);
        }

        try
        {
            GameObject turnAndBank = airplaneData.countryDialBoard.transform.Find("Turn And Bank").gameObject;
            layout.turnAndBankPos = airplaneData.countryDialBoard.transform.Find("Turn And Bank").GetComponent<RectTransform>().anchoredPosition;
            layout.turnAndBankScale = airplaneData.countryDialBoard.transform.Find("Turn And Bank").GetComponent<RectTransform>().localScale.x;
        }
        catch
        {
            DialInTray("Turn And Bank", layout);
        }

        try
        {
            GameObject turnIndicator = airplaneData.countryDialBoard.transform.Find("Turn Coordinator").gameObject;
            layout.turnIndicatorPos = airplaneData.countryDialBoard.transform.Find("Turn Coordinator").GetComponent<RectTransform>().anchoredPosition;
            layout.turnIndicatorScale = airplaneData.countryDialBoard.transform.Find("Turn Coordinator").GetComponent<RectTransform>().localScale.x;
        }
        catch 
        {
            DialInTray("Turn Coordinator", layout);
        }

        try
        {
            GameObject vsi = airplaneData.countryDialBoard.transform.Find("VSI").gameObject;
            layout.vsiPos = airplaneData.countryDialBoard.transform.Find("VSI").GetComponent<RectTransform>().anchoredPosition;
            layout.vsiScale = airplaneData.countryDialBoard.transform.Find("VSI").GetComponent<RectTransform>().localScale.x;

        }
        catch
        {
            DialInTray("VSI", layout);            
        }
               

        string jsonFoo = JsonUtility.ToJson(layout);

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

                case "VSI":
                    layout.vsiPos = dialsInTray[i].GetComponent<RectTransform>().anchoredPosition;
                    layout.vsiScale = dialsInTray[i].GetComponent<RectTransform>().localScale.x;
                    layout.vsiInTray = true;
                    break;
            }
        }
    }

}