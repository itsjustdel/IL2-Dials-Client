using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static PlaneLists;

public class MenuHandler : MonoBehaviour
{
    public AirplaneData airplaneData;
    public DialsManager dialsManager;
    public FlagButtons flagButtons;
    public SlaveManager slaveManager;
    public PlaneDropdown planeDropdown;
    public bool deletePrefs = false;
    public GameObject title;
    public GameObject missionStart;
    public GameObject welcomePanel;
    public GameObject serverMessagePanel;
    public GameObject keyCodePanel;
    public Inputs inputs;
    public GameObject blurPanel;
    public GameObject menuPanel;
    public GameObject displayPanel;
    public GameObject menuButton;
    public GameObject fullSreenButton;
    public GameObject connectionPanel;
    public GameObject layoutPanel;
    public GameObject layoutButton;
    public GameObject flagsPanel;
    public GameObject screensPanel;
    public GameObject planeDropdownPanel;
    public GameObject connectionsButton;
    public GameObject ledParent;
    public GameObject ipTextField;
    public GameObject portTextField;
    public GameObject scanDebug;
    public GameObject eyeTray;
    public UDPClient udpClient;
    public bool dontShowAgain;
    public Toggle dontShowAgainToggle;
    public bool ipFieldOpen;
    public bool portFieldOpen;
    public bool trayOpen;
    public GameObject trayParent;
    public GameObject trayPullDown;
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
    public System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
    public float idleTimer;
    private Vector3 mousePos;
    public bool layoutOpen;
    public bool planeDropdownPanelOpen;
    public bool inFlight;
    public bool trayPulled;
    public float trayYTarget = 100f;
    public float currentTrayY = 600;
    public bool tigerMothSelected;
    public List<GameObject> androidHideObjects = new List<GameObject>();
    public List<GameObject> androidAdjustObjects = new List<GameObject>();

    public void Start()
    {
        mousePos = Input.mousePosition;

        if (deletePrefs)
        {
            PlayerPrefs.DeleteAll();
        }

        udpClient.serverAddress = PlayerPrefs.GetString("IPAddress");
        udpClient.portNumber = PlayerPrefs.GetInt("PortNumber");

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
        ipTextField.GetComponentInParent<InputField>(true).text = udpClient.serverAddress;
        if (udpClient.portNumber == 11200 || udpClient.portNumber == 0)
        {
            //set default port number text field to null so unity shows the greyed out placeholder text
            portTextField.GetComponentInParent<InputField>(true).text = null;
        }
        else
        {
            portTextField.GetComponentInParent<InputField>(true).text = udpClient.portNumber.ToString();
        }

        //force this false to start with, above code flags this as true because we changed a value
        ipFieldOpen = false;
        portFieldOpen = false;

        //set alpha in mission glow and title to 0 - this way we can see it in the editor before we press play
        missionStartColor = missionStart.GetComponent<Text>().color;
        missionStartColor.a = 0;
        missionStart.GetComponent<Text>().color = missionStartColor;

        titleColor = title.GetComponent<Image>().color;
        titleColor.a = 0;
        title.GetComponent<Image>().color = titleColor;

#if UNITY_ANDROID
        HideForAndroid();
        AdjustForAndroid();
#endif

        // Clear config status when any button inside the menu is pressed (connection, layout, screens, close-all etc.)
        if (menuPanel != null)
        {
            var menuButtons = menuPanel.GetComponentsInChildren<UnityEngine.UI.Button>(true);
            foreach (var b in menuButtons)
            {
                b.onClick.AddListener(() => ConfigUpdateButton.ClearAllStatuses());
            }
        }

    }

    public void Update()
    {
        if (Input.GetKeyDown("escape"))
        {
            if (connectionPanel.activeInHierarchy)
            {
                ConnectionPanelBack();
            }
            else if (layoutOpen)
            {
                AcceptLayoutClick();
            }
            else if (flagsPanel.activeInHierarchy)
            {
                flagButtons.BackPressed();
            }
            else if (screensPanel.activeInHierarchy)
            {
                slaveManager.ScreensPanelBack();
            }
            else if (planeDropdownPanel.activeInHierarchy)
            {
                planeDropdown.OnBack();
            }
            else if (menuPanel.activeInHierarchy)
            {
                MenuButtonClicked();
            }
        }

        if (airplaneData.planeAttributes != null && airplaneData.planeAttributes.country != Country.UNDEFINED)
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

        AnimatePulldown();

        IdleTimer();
    }

    void HideForAndroid()
    {
        foreach (var item in androidHideObjects)
        {
            item.SetActive(false);
        }
    }

    void AdjustForAndroid()
    {
        foreach (var item in androidAdjustObjects)
        {
            Vector3 p = item.GetComponent<RectTransform>().transform.localPosition;
            p.x = 0;
            item.GetComponent<RectTransform>().transform.localPosition = p;
        }

    }

    void AnimatePulldown()
    {
        RectTransform r = trayPullDown.GetComponent<RectTransform>();
        r.anchoredPosition = Vector3.Lerp(r.anchoredPosition, new Vector3(0, trayYTarget, 1), Time.deltaTime * 10);
    }

    void IdleTimer()
    {
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

        if (idleTimer == 0f)
        {
            Color color = menuButton.GetComponent<Image>().color;
            color.a = 1f;
            menuButton.GetComponent<Image>().color = color;

            color = fullSreenButton.GetComponent<Image>().color;
            color.a = 1f;
            fullSreenButton.GetComponent<Image>().color = color;
        }

        if (idleTimer > 10f)
        {
            Color color = menuButton.GetComponent<Image>().color;
            color.a -= 1f * Time.deltaTime;
            menuButton.GetComponent<Image>().color = color;

            color = fullSreenButton.GetComponent<Image>().color;
            color.a -= 1f * Time.deltaTime;
            fullSreenButton.GetComponent<Image>().color = color;
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
            if (stopwatch.ElapsedMilliseconds > startLEDFade * 1000)
            {
                //led script on same object
                if (!GetComponent<LEDs>().fadeInProgress && !GetComponent<LEDs>().fadeInComplete)
                    GetComponent<LEDs>().startFadeIn = true;
            }
        }
    }

    public void MissionStartGlow()
    {
        //wait until slide is finished
        if (stopwatch.ElapsedMilliseconds < startMissionGlowFade * 1000)
            return;

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

        if (missionStartColor.a < 0f)
        {
            missionStartColor.a = 0f;
            glowDirection = !glowDirection;
        }
        else if (missionStartColor.a > 1f)
        {
            missionStartColor.a = 1f;
            glowDirection = !glowDirection;
        }
    }

    public void MenuButtonClicked()
    {
        if (menuPanel.activeInHierarchy)
        {
            //close from main main menu 
            menuPanel.SetActive(false);
            blurPanel.SetActive(false);
            layoutWarningMessage.SetActive(false);
            planeDropdownPanelOpen = false;
            ConfigUpdateButton.ClearAllStatuses();
        }
        else if (connectionPanel.activeInHierarchy)
        {
            //close from connection panel
            //Debug.Log("Closing from connection panel");
            connectionPanel.SetActive(false);
            menuPanel.SetActive(false);
            blurPanel.SetActive(false);
            layoutWarningMessage.SetActive(false);
            ConfigUpdateButton.ClearAllStatuses();
        }
        else if (screensPanel.activeInHierarchy)
        {
            screensPanel.SetActive(false);
            blurPanel.SetActive(false);
            ConfigUpdateButton.ClearAllStatuses();
        }
        else if (flagsPanel.activeInHierarchy)
        {
            flagsPanel.SetActive(false);
            blurPanel.SetActive(false);
            ConfigUpdateButton.ClearAllStatuses();
        }
        else if (planeDropdownPanel.activeInHierarchy)
        {
            planeDropdownPanel.SetActive(false);
            planeDropdownPanelOpen = false;
            blurPanel.SetActive(false);
            ConfigUpdateButton.ClearAllStatuses();
        }
        else if (layoutOpen)
        {
            //leaving layout screen
            //point to accept layout function - we can close this page from menu button or accept button
            AcceptLayoutClick();
            layoutWarningMessage.SetActive(false);
            ConfigUpdateButton.ClearAllStatuses();
        }
        else
        {
            //everything closed, open menu panel
            menuPanel.SetActive(true);
            blurPanel.SetActive(true);
        }
    }

    public void FullScreenClicked()
    {
        // Toggle fullscreen
        Screen.fullScreen = !Screen.fullScreen;
    }

    public void IPAddressChanged()
    {
        string ipAddressText = ipTextField.GetComponent<Text>().text;
        if (ipAddressText == "clear prefs")
        {
            Debug.Log("Clearing all prefs from IP address");
            PlayerPrefs.DeleteAll();
            ipTextField.GetComponentInParent<InputField>().text = "Cleared!";
        }
        else if (string.IsNullOrEmpty(ipAddressText))
        {
            //go back to autoscan 
            udpClient.autoScan = true;
            //placeholder text should show and we set ip to empty, when empty, it autoscans
            udpClient.serverAddress = null;
            //save to player prefs for next load
            PlayerPrefs.SetString("IPAddress", ipAddressText);
            PlayerPrefs.Save();
        }
        else
        {
            //direct attempt from user
            udpClient.autoScan = false;
            //give udpClient the user Ip
            //interface variable
            udpClient.serverAddress = ipAddressText;
            //code variable
            udpClient.serverAddress = ipAddressText;
            //save
            PlayerPrefs.SetString("IPAddress", ipAddressText);
            PlayerPrefs.Save();
        }

        //reset autoscan variables
        udpClient.ip3 = 0;
        udpClient.ip4 = 4;

        //flag for autoscan pause
        ipFieldOpen = false;

        udpClient.hostFound = false;
    }

    public void PortChanged()
    {
        string portText = portTextField.GetComponent<Text>().text;
        if (string.IsNullOrEmpty(portText))
        {
            //set to default port
            udpClient.portNumber = 11200;
            //save to player prefs for next load
            PlayerPrefs.SetInt("PortNumber", 11200);
        }
        else
        {
            //set port to user input
            int parsed = int.Parse(portText);
            udpClient.portNumber = parsed;
            //save
            PlayerPrefs.SetInt("PortNumber", parsed);
        }

        udpClient.ip3 = 0;
        udpClient.ip4 = 4;

        //flag for autoscan pause
        portFieldOpen = false;
        udpClient.hostFound = false;
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
        //set pref - ternary to set integer (no bool value in prefs)
        PlayerPrefs.SetInt("dontshowagain", dontShowAgain ? 1 : 0);
        PlayerPrefs.Save();

    }

    public void OpenLayoutClick()
    {
        layoutOpen = true;

        //remove metal panel
        menuPanel.SetActive(false);

        //hide Leds and Menu button
        menuButton.SetActive(false);
        ledParent.SetActive(false);

        //turn blur off so we can see what we are doing with the dials
        blurPanel.SetActive(false);
        //turn our new menu on
        layoutPanel.SetActive(true);

        //show dial controls for each dial
        TurnHandlersOn();
        TurnEyeTrayOn();
        ShowOpenEyeButton();

        DeActivateCompassTouch();
    }

    public void AcceptLayoutClick()
    {
        //go back to main menu panel
        layoutPanel.SetActive(false);
        //turn icon handlers off 
        TurnHandlersOff();
        TurnEyeTrayOff();

        layoutOpen = false;

        //turn compasses back on 
        ActivateCompassTouch();

        dialsManager.SaveLayout();

        //turn menu button and leds back on
        menuButton.SetActive(true);
        ledParent.SetActive(true);

        if (inFlight)
        {
            //reset flag
            inFlight = false;
        }
        else
        {
            //keep blur, we go back to menuPanel
            blurPanel.SetActive(true);
            airplaneData.planeType = "";
            //title looks for planeattributes to show or not
            airplaneData.planeAttributes = null;
            flagsPanel.SetActive(true);

            //remove dial board if any 
            //remove any existing dials board prefab in scene
            if (dialsManager.countryDialBoard != null)
                Destroy(dialsManager.countryDialBoard);
        }
    }

    public void ResetLayout()
    {
        dialsManager.DeleteLayout();

        TurnHandlersOn();
    }

    public void AddLayouButtonClick()
    {
        trayOpen = !trayOpen;
    }

    public void OpenConnectionsClick()
    {
        menuPanel.SetActive(false);
        //show copnnection panel - IP address, port etc
        connectionPanel.SetActive(true);
    }

    private void DeActivateCompassTouch()
    {
        //remove compass interactiveness
        GameObject[] compassSpinners = GameObject.FindGameObjectsWithTag("CompassSpin");
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

    public void TurnHandlersOn()
    {
        GameObject[] UIhandlers = GameObject.FindGameObjectsWithTag("UIHandler");
        for (int i = 0; i < UIhandlers.Length; i++)
        {
            //german water oil switch
            if (UIhandlers[i].name == "Gear")
            {
                UIhandlers[i].GetComponent<Image>().enabled = true;
            }
            else if (dialsInTray.Contains(UIhandlers[i].transform.parent.gameObject))
            {
                GameObject container = UIhandlers[i].transform.Find("Return Container").gameObject;
                container.SetActive(true);
            }
            else
            {
                GameObject container = UIhandlers[i].transform.Find("Container").gameObject;
                container.SetActive(true);
            }
        }
    }

    public void TurnHandlersOff()
    {
        GameObject[] UIhandlers = GameObject.FindGameObjectsWithTag("UIHandler");
        for (int i = 0; i < UIhandlers.Length; i++)
        {
            //german water oil switch
            if (UIhandlers[i].name == "Gear")
            {
                UIhandlers[i].GetComponent<Image>().enabled = false;
            }
            else
            {
                for (int j = 0; j < UIhandlers[i].transform.childCount; j++)
                {

                    UIhandlers[i].transform.Find("Container").gameObject.SetActive(false);

                }
            }
        }
    }
    public void OpenDisplayPanel()
    {
        menuPanel.SetActive(false);
        displayPanel.SetActive(true);
    }

    public bool InFlight()
    {
        if (airplaneData.planeAttributes == null)
            return false;

        bool inFlight = false;
        switch (airplaneData.planeAttributes.country)
        {
            case Country.RU:
                if (RuPlanes.Contains(airplaneData.planeType))
                    inFlight = true;
                break;

            case Country.UK:
                if (UkPlanes.Contains(airplaneData.planeType))
                    inFlight = true;
                break;

            case Country.US:
                if (UsPlanes.Contains(airplaneData.planeType))
                    inFlight = true;
                break;

            case Country.GER:
                if (GerPlanes.Contains(airplaneData.planeType))
                    inFlight = true;

                break;
            case Country.ITA:
                if (ItaPlanes.Contains(airplaneData.planeType))
                    inFlight = true;
                break;

            case Country.FR:
                if (FrPlanes.Contains(airplaneData.planeType))
                    inFlight = true;
                break;

        }

        return inFlight;
    }

    public void ShowFlagsPanel()
    {
        inFlight = InFlight();

        if (inFlight)
        {
            //open layout directly with current plane
            OpenLayoutClick();
        }
        else
        {
            menuPanel.SetActive(false);
            flagsPanel.SetActive(true);
        }
    }

    public void ConnectionPanelBack()
    {
        connectionPanel.SetActive(false);
        menuPanel.SetActive(true);
    }

    public void TrayPulldown()
    {
        trayPulled = !trayPulled;
        DropDownYTarget();
    }

    public void DropDownYTarget()
    {
        if (trayPulled)
        {
            DropdownYTargetWithChildren();
        }
        else if (trayParent.transform.childCount == 0)
        {
            //keep high if none
            trayYTarget = 2100;
        }
        else
        {
            //hang it down a bit
            //keep high if none
            trayYTarget = 1950;
        }
    }
    public void DropdownYTargetWithChildren()
    {
        HashSet<float> rows = new HashSet<float>();
        for (int i = 0; i < trayParent.transform.childCount; i++)
        {
            float childY = trayParent.transform.GetChild(i).GetComponent<RectTransform>().anchoredPosition.y;
            rows.Add(childY);
        }
        float spacing = GameObject.FindGameObjectWithTag("DialsTray").GetComponent<GridLayoutGroup>().spacing.x;

        trayYTarget = 1800 - (rows.Count * spacing) + 150;
    }

    public void TurnEyeTrayOn()
    {
        eyeTray.SetActive(true);
    }
    public void TurnEyeTrayOff()
    {
        eyeTray.SetActive(false);
    }
    public void ShowClosedEyeButton()
    {
        eyeTray.transform.Find("Eye On").gameObject.SetActive(false);
        eyeTray.transform.Find("Eye Off").gameObject.SetActive(true);
    }
    public void ShowOpenEyeButton()
    {
        eyeTray.transform.Find("Eye On").gameObject.SetActive(true);
        eyeTray.transform.Find("Eye Off").gameObject.SetActive(false);
    }

    public void EnableKeyCodePanel()
    {
        keyCodePanel.SetActive(true);
        blurPanel.SetActive(true);
        layoutPanel.SetActive(false);

        string s = "";
        if (inputs.oilWaterKeys.Count == 0)
            s = "...";
        else
        {
            for (int i = 0; i < inputs.oilWaterKeys.Count; i++)
            {
                if (i != 0)
                    s += " + ";

                s += inputs.oilWaterKeys[i].ToString();
            }
        }
        inputs.keyCodeText.text = s;
    }

    public void AcceptOilKeyCode()
    {
        keyCodePanel.SetActive(false);
        blurPanel.SetActive(false);
        layoutPanel.SetActive(true);

        if (inputs.oilWaterKeyRecord)
        {
            //if user just pressed click without stoppping recording, assume they are happy with what they entered
            inputs.OilKeyRecordToggle();
        }
    }
}