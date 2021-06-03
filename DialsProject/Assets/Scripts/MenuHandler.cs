using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MenuHandler : MonoBehaviour
{

    
    public bool deletePrefs = false;
    public GameObject title;
    public GameObject titleMask;
    public GameObject missionStart;
    public GameObject welcomePanel;
    public GameObject serverMessagePanel;
    public GameObject blurPanel;
    public GameObject menuPanel;
    public GameObject menuButton;
    public GameObject ledParent;
    public GameObject ipTextField;
    public GameObject portTextField;
    public GameObject scanDebug;
    public TCPClient tcpClient;
    public bool dontShowAgain;
    public Toggle dontShowAgainToggle;
    public bool ipFieldOpen;
    public bool portFieldOpen;

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
    public void Start()
    {

        if (deletePrefs)
        {
            PlayerPrefs.DeleteAll();
        }

        //load preferences
        tcpClient.userIP = PlayerPrefs.GetString("IPAddress");
        tcpClient.hostName = PlayerPrefs.GetString("IPAddress");//flaw in design, why host name and user ip
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

        if (!welcomePanel.activeInHierarchy)
        {
            if (!stopwatch.IsRunning)
                stopwatch.Start();

            TitleFade();

            MissionStartGlow();
        }
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
        Debug.Log("Menu Clicked");

        //set to opposite
        blurPanel.SetActive(!blurPanel.activeSelf);
        menuPanel.SetActive(!menuPanel.activeSelf);
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
}