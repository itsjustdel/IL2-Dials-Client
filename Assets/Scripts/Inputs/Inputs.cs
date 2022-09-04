using UnityEngine;
using UnityRawInput;
using System.Collections.Generic;

public class Inputs : MonoBehaviour
{
    public bool WorkInBackground;
    public RotateNeedle rN;
    public RawKey lastKeyPressed;
    public List<RawKey> oilWaterKeys = new List<RawKey>();
    public bool oilWaterKeyRecord;

    private void OnEnable()
    {
        RawInput.Start(WorkInBackground);
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
        lastKeyPressed = key;
        //rN.germanWaterOilSwitch = true;
        Debug.Log("Key Down: " + key);     
    }

    private void OilButtonUp(RawKey key)
    {    

        lastKeyPressed = 0x00;
        //rN.germanWaterOilSwitch = false;
     
    }

    public void SetOilWaterKey()
    {
        //pop up to tell user to input key combo
        Debug.Log("oil water key gear pressed");
        oilWaterKeyRecord = true;
        //start and stop
    }

    private void Update()
    {
        if (oilWaterKeyRecord)
        {
            if (lastKeyPressed != 0x00
                && lastKeyPressed != RawKey.LeftButton 
                && lastKeyPressed != RawKey.RightButton 
                && !oilWaterKeys.Contains(lastKeyPressed))
            {
                Debug.Log("last pressed = " + lastKeyPressed);
                Debug.Log(oilWaterKeys);
                oilWaterKeys.Add(lastKeyPressed);
            }
        }
    }
}