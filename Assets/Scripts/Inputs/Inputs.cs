using UnityEngine;
using UnityEngine.UI;
using UnityRawInput;
using System.Collections.Generic;

public class Inputs : MonoBehaviour
{
    
    public DialsManager dM;
    public List<RawKey> allKeysDown = new List<RawKey>();
    public List<RawKey> oilWaterKeys = new List<RawKey>();
    public bool oilWaterKeyRecord;
    public GameObject KeyCodePanel;
    public Text keyCodeText;
    public GameObject recordButton;
    public GameObject stopButton;
    public GameObject keyCodePanel;
    public GameObject blur;

    private void Start()
    {
        if (PlayerPrefs.HasKey("German oil switch keys"))
        {
            string value = PlayerPrefs.GetString("German oil switch keys");
            string[] split = value.Split(" + ");

            foreach (string s in split)
            {
                RawKey k;
                System.Enum.TryParse(s, out k);
                oilWaterKeys.Add(k);
            }
        }        
    }

    private void OnEnable()
    {
        bool workInBackground = true;
        RawInput.Start(workInBackground);
        RawInput.OnKeyUp += OilButtonUp;
        RawInput.OnKeyDown += OilButtonDown;
      //  RawInput.OnKeyDown += DisableIntercept;
    }

    private void OnDisable()
    {
        RawInput.Stop();
        RawInput.OnKeyUp -= OilButtonUp;
        RawInput.OnKeyDown -= OilButtonDown;
      //  RawInput.OnKeyDown -= DisableIntercept;
    }

    private void DisableIntercept(RawKey key)
    {
       // if (RawInput.InterceptMessages && key == DisableInterceptKey)
       //     RawInput.InterceptMessages = InterceptMessages = false;
    }

    private void OilButtonDown(RawKey key)
    {
        if (key == RawKey.LeftButton || key == RawKey.RightButton)
            return;


        if (!allKeysDown.Contains(key))
        {
            allKeysDown.Add(key);            
        }
    }

    private void OilButtonUp(RawKey key)
    {
        if (key == RawKey.LeftButton || key == RawKey.RightButton)
            return;
        
        allKeysDown.Remove(key);
    }

    private void Update()
    {
        if (oilWaterKeyRecord)
        {
            foreach (RawKey key in allKeysDown)
            {
                if (!oilWaterKeys.Contains(key))
                {
                    oilWaterKeys.Add(key);

                    string s = "";
                    for (int i = 0; i < oilWaterKeys.Count; i++)
                    {
                        if (i > 0)
                            s += " + ";

                        s += oilWaterKeys[i].ToString();
                    }

                    keyCodeText.text = s;

                    PlayerPrefs.SetString("German oil switch keys", s);
                }
            }
        }
        else
        {
            if (dM.countryDialBoard!= null)
            {
                if (CheckKeyCombo(allKeysDown, oilWaterKeys))
                    dM.countryDialBoard.GetComponent<RotateNeedle>().germanWaterOilSwitch = true;

                else if (!Input.GetMouseButton(0) && !Input.GetMouseButton(1))
                {
                    dM.countryDialBoard.GetComponent<RotateNeedle>().germanWaterOilSwitch = false; //overriding button press?
                }
            }
        }        
    }

    private static bool CheckKeyCombo(List<RawKey>currentKeysDown, List<RawKey> listToMatch)
    {
        //listen for oil key combo press
        bool comboPressed = true;

        if (currentKeysDown.Count == 0 || listToMatch.Count == 0)
        {
            comboPressed = false;
            return comboPressed;
        }

        int counter = 0;
        foreach (RawKey keyDown in currentKeysDown)
        {
            if (listToMatch.Contains(keyDown))
            {
                counter++;
            }
        }
        if (counter != listToMatch.Count)
            comboPressed = false;

        return comboPressed;
    }

    public void OilKeyRecordToggle()
    {
        oilWaterKeyRecord = !oilWaterKeyRecord;

        if (oilWaterKeyRecord)
        {
            recordButton.SetActive(false);
            stopButton.SetActive(true);
            oilWaterKeys.Clear();
            keyCodeText.text = "...";
        }
        else
        {
            recordButton.SetActive(true);
            stopButton.SetActive(false);
        }
    }
}
