using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuHandler : MonoBehaviour
{
    public bool deletePrefs = false;
    public GameObject blurPanel;
    public GameObject menuPanel;
    public GameObject ipTextField;
    public GameObject portTextField;
    public GameObject scanDebug;
    public TCPClient tcpClient;

    public void Start()
    {
        if (deletePrefs)
        {
            PlayerPrefs.DeleteAll();
        }

        //load preferences
        tcpClient.userIP = PlayerPrefs.GetString("IPAddress");
        tcpClient.portNumber = PlayerPrefs.GetInt("PortNumber");
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

            Debug.Log("3");

        }
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
            tcpClient.portNumber= parsed;
            //save
            PlayerPrefs.SetInt("PortNumber", parsed);

            Debug.Log("6");

        }
    }
}
