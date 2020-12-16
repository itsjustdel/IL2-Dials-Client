using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using UnityEngine;

public class RotateNeedle : MonoBehaviour
{
    public ReadGameData iL2GameDataClient;
    
    public GameObject altitudeNeedleSmall;
    public GameObject altitudeNeedleLarge;
    public GameObject mmhgDial;
    public GameObject airspeedNeedle;

   
    // Start is called before the first frame update
    void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    {

        RotateNeedles();

        RotateMMHG();
    }

    void RotateNeedles()
    {
        AltitudeNeedles();

        AirspeedNeedle();
    }

    void RotateMMHG()
    {
        //barometer/pressure dial inside main dial

        //sea level is 760
        //zero rotation on 760

        mmhgDial.transform.rotation = Quaternion.Euler(0, 0, ((760f - iL2GameDataClient.mmhg) / 100f) * 300) ;

    }

    void AltitudeNeedles()
    {
        //small hand gose to thousands
        altitudeNeedleSmall.transform.rotation = Quaternion.Euler(0, 0, -(iL2GameDataClient.altitude / 10000f) * 360);
        //large hand to hundreds
        altitudeNeedleLarge.transform.rotation = Quaternion.Euler(0, 0, -(iL2GameDataClient.altitude / 1000f) * 360);
    }

    void AirspeedNeedle()
    {
        //airspeed dial has three gears
        //below 100
        if (iL2GameDataClient.airspeed < 100)
            //if 0 -> 0
            //if 50 -> 15
            //if 100 -> 30
            airspeedNeedle.transform.rotation = Quaternion.Euler(0, 0, -((iL2GameDataClient.airspeed / 50f) * 15f));
        //below 300
        else if (iL2GameDataClient.airspeed < 300)
            //for every 5 kmh, move 30 degrees            
            //if 100 -> 30
            //if 150 -> 60
            //if 200 -> 90
            airspeedNeedle.transform.rotation = Quaternion.Euler(0, 0, -((iL2GameDataClient.airspeed / 50f) * 30f) + 30f);
        else
            //over 300
            //for evry 10, move 40, but starts at 300 (150degrees)
            //if 600 -> 270
            //if 700 -> 310            
            airspeedNeedle.transform.rotation = Quaternion.Euler(0, 0, -((iL2GameDataClient.airspeed / 10f) * 4) - 30);

    }
}
