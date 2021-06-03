using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using UnityEngine;

public class AirplaneData : MonoBehaviour
{
    public float clientVersion = 0.2f; //manually update this
    public float serverVersion;

    public enum Country
    {
        RU,
        GER,
        US,
        UK,
        ITA,
        UNDEFINED

    }
    public string planeType;
    public string planeTypePrevious;
    public Country country = Country.RU;
    public float altitude;
    public float mmhg;
    public float airspeed;
    public float climbRate;
    public float rollRate;

    public List<GameObject> countryDials = new List<GameObject>();
    private Country previousCountry;

    public BuildControl buildControl;
    public MenuHandler menuHandler;
    public TCPClient tcpClient;
    //fixed update is enough for checking status
    void FixedUpdate()
    {
        //check client version against incoming server message
        CheckVersion();

        RemoveTitle();

        CheckForPlaneChange();

        if (country != Country.UNDEFINED )
            RemoveTitle();
        else 
            EnableTitle();
      
    }

    void CheckVersion()
    {
        //checks version and shows message if mismatch (if connected)
        if (tcpClient.connected)
        {
            if (serverVersion != clientVersion)
            {
                //show server message
                menuHandler.ServerMessageOpen();
            }
            else
            {
                //all good
                menuHandler.ServerMessageClosed();
            }
        }
        else
        {
            //we are not connected to anything we don't know anything about version numbersd
            menuHandler.ServerMessageClosed();
        }
    }

    void RemoveTitle()
    {
        menuHandler.title.SetActive(false);
        menuHandler.missionStart.SetActive(false);
    }
    void EnableTitle()
    {
        menuHandler.title.SetActive(true);
        menuHandler.missionStart.SetActive(true);
    }


    void CheckForPlaneChange()
    {
        if(planeType != planeTypePrevious)
        {
            country = PlaneCountryFromName.AsignCountryFromName(planeType);

            //enable and disable dials depending on plane/country
            SwitchDialsFromCountry();
        }

        planeTypePrevious = planeType;
    }

    void SwitchDialsFromCountry()
    {
        //change dials depending on what value we received from the networking component
        

        //switch all off
        for (int i = 0; i < countryDials.Count; i++)
        {
            countryDials[i].SetActive(false);
        }

        switch (country)
        {
            case Country.RU:
                countryDials[0].SetActive(true);
                break;
            case Country.GER:
                countryDials[1].SetActive(true);
                break;
            case Country.US:
                countryDials[2].SetActive(true);
                break;
            case Country.UK:
                countryDials[3].SetActive(true);
                break;
            case Country.ITA:
                countryDials[4].SetActive(true);
                break;
        }

    }

   
}
