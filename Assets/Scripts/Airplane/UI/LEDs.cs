using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LEDs : MonoBehaviour
{
    public MenuHandler menuHandler;
    public UDPClient udpClient;
    public GameObject greenOn;
    public GameObject greenOff;
    public GameObject redOn;
    public GameObject redOff;
    public List<GameObject> fadeIns;
    public float fadeInSpeed = 1f;

    public bool startFadeIn = false;
    public bool fadeInProgress = false;
    public bool fadeInComplete = false;
    // Start is called before the first frame update
    void Start()
    {
        fadeIns = new List<GameObject>() { greenOn, greenOff, redOn, redOff, menuHandler.menuButton };

        //fade off
        for (int i = 0; i < fadeIns.Count; i++)
        {
            Color color = fadeIns[i].GetComponent<Image>().color;
            color.a = 0f;
            fadeIns[i].GetComponent<Image>().color = color;
        }
    }
        
    void Update()
    {
        //intro screen, turn off all LEDS



        /*
        if (!menuHandler.fadeLeds)
        {
            greenOn.SetActive(false);
            greenOff.SetActive(false);
            redOn.SetActive(false);
            redOff.SetActive(false);
            return;
        }
        */

        //else
        //  fadeInProgress = true;


        if (startFadeIn)
        {
            fadeInProgress = true;
            startFadeIn = false;
        }

        if(fadeInProgress)       
        {
            

            //fade button and leds in            
            for (int i = 0; i < fadeIns.Count; i++)
            {
                Color color = fadeIns[i].GetComponent<Image>().color;
                color.a += fadeInSpeed * Time.deltaTime;
                fadeIns[i].GetComponent<Image>().color = color;

                
                //turn flag off
                if (color.a > 1)
                {
                    fadeInProgress = false;
                    fadeInComplete = true;
                }
            }
        }

        //show lights
        if (udpClient.connected)
        {
            greenOn.SetActive(true);
            greenOff.SetActive(false);
            redOn.SetActive(false);
            redOff.SetActive(true);
        }
        else
        {
            greenOn.SetActive(false);
            greenOff.SetActive(true);
            redOn.SetActive(true);
            redOff.SetActive(false);
        }
    }
}
