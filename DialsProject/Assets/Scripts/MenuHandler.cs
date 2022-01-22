using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MenuHandler : MonoBehaviour
{
    public AirplaneData airplaneData;
    public DialsManager dialsManager;
    
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


    
    public GameObject connectionsButton;
    public GameObject ledParent;
    public GameObject ipTextField;
    public GameObject portTextField;
    public GameObject scanDebug;
    public UDPClient udpClient;
    public bool dontShowAgain;
    public Toggle dontShowAgainToggle;
    public bool ipFieldOpen;
    public bool portFieldOpen;


    public bool trayOpen;
    public GameObject trayParent;
    public List<GameObject> trayObjects;
    public List<GameObject> dialsInTray;
    public List<GameObject> dialsOnBoard;

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
        /*
        udpClient.userIP = PlayerPrefs.GetString("IPAddress");
        udpClient.hostName = PlayerPrefs.GetString("IPAddress");//flaw in design, why host name and user ip?
        udpClient.portNumber = PlayerPrefs.GetInt("PortNumber");
        */


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


        if (udpClient.portNumber == 0)
            udpClient.portNumber = 11200;

        //apply to UI
        //inlcude inactive with bool flag
        ipTextField.GetComponentInParent<InputField>(true).text = udpClient.userIP;

        //Debug.Log(udpClient.portNumber);
        if (udpClient.portNumber == 11200 || udpClient.portNumber == 0)
        {
            //set default port number text field to null so unity shows the greyed out placeholder text
            //Debug.Log("0");
            portTextField.GetComponentInParent<InputField>(true).text = null;
        }
        else
        {
            //Debug.Log("1");
            portTextField.GetComponentInParent<InputField>(true).text = udpClient.portNumber.ToString();
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
            //Debug.Log("Opening menu from closed");
            menuPanel.SetActive(true);
            blurPanel.SetActive(true);
        }


        else if (menuPanel.activeInHierarchy)
        {
            //close from main main menu 
            //Debug.Log("Closing from main menu");
            menuPanel.SetActive(false);
            //
            blurPanel.SetActive(false);
            layoutWarningMessage.SetActive(false);

        }
        
        else if(connectionPanel.activeInHierarchy)
        {
            //close from connection panel
            //Debug.Log("Closing from connection panel");
            connectionPanel.SetActive(false);
            menuPanel.SetActive(false);
            blurPanel.SetActive(false);
            layoutWarningMessage.SetActive(false);
        }        

        else if (layoutOpen)
        {
            //leaving layout screen
            //Debug.Log("Closing menu from layout");                

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
            udpClient.userIP = null;
            //save to player prefs for next load
            PlayerPrefs.SetString("IPAddress", ipAddressText);
            PlayerPrefs.Save();



            Debug.Log("2");
        }
        else
        {
            //give udpClient the user Ip
            //interface variable
            udpClient.userIP = ipAddressText;
            //code variable
            udpClient.hostName = ipAddressText;
            //save
            PlayerPrefs.SetString("IPAddress", ipAddressText);
            PlayerPrefs.Save();


            //let user know what's happening
            //            scanDebug.GetComponent<Text>().text = "Attempting Connection: " + udpClient.hostName.ToString(); ;

            Debug.Log("3");

        }

        //reset autoscan variables
        udpClient.ip3 = 0;
        udpClient.ip4 = 4;

        //flag for autoscan pause
        ipFieldOpen = false;

        udpClient.hostFound = false;


        udpClient.timer = udpClient.socketTimeoutTime;


    }

    public void PortChanged()
    {
        Debug.Log("Port changed");

        string portText = portTextField.GetComponent<Text>().text;
        if (string.IsNullOrEmpty(portText))
        {
            //set to default port
            udpClient.portNumber = 11200;

            //save to player prefs for next load
            PlayerPrefs.SetInt("PortNumber", 11200);

            Debug.Log("4");
        }
        else
        {
            //set port to user input
            int parsed = int.Parse(portText);
            udpClient.portNumber = parsed;
            //save
            PlayerPrefs.SetInt("PortNumber", parsed);

            Debug.Log("6");

        }

        udpClient.ip3 = 0;
        udpClient.ip4 = 4;

        //flag for autoscan pause
        portFieldOpen = false;

        udpClient.hostFound = false;

        udpClient.timer = udpClient.socketTimeoutTime;
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
        //Debug.Log("Layout Click");
        layoutOpen = true;
        
        //check for plane - can only organise if plane loaded
        if(airplaneData.country == AirplaneData.Country.UNDEFINED)
        {
            layoutWarningMessage.SetActive(true);
            //Debug.Log("Layout Warning message");
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

        dialsManager.SaveLayout();

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

        //Debug.Log("compasses = " + compassSpinners.Length);
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

}