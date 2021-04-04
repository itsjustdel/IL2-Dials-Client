using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LEDs : MonoBehaviour
{
    public TCPClient tcpClient;
    public GameObject greenOn;
    public GameObject greenOff;
    public GameObject redOn;
    public GameObject redOff;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    
    void FixedUpdate()
    {
        if(tcpClient.connected)
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
