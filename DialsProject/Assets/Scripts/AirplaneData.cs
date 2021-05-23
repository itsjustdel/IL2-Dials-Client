using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using UnityEngine;

public class AirplaneData : MonoBehaviour
{
   
    public enum Country
    {
        RU,
        GER,
        US,
        UK,
        ITA
    }

    public Country country = Country.RU;
    public float altitude;
    public float mmhg;
    public float airspeed;
    public float climbRate;
    public float rollRate;

    public List<GameObject> countryDials = new List<GameObject>();
    private Country previousCountry;

    public BuildControl buildControl;

    // Update is called once per frame
    void Update()
    {
        CheckForCountryChange();
      
    }

    void CheckForCountryChange()
    {
        
        if(country != previousCountry)
            //a change has been detected
            SetCountry();


        previousCountry = country;
    }

    void SetCountry()
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
