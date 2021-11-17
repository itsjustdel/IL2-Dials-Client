using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Layout
{
    //class used for saving layout positions to player prefs with JSON
    public float version;

    public string planeType;
    public Vector2 speedoPos;
    public Vector2 altPos;
    public Vector2 headingPos;
    public Vector2 turnAndBankPos;
    public Vector2 turnIndicatorPos;
    public Vector2 vsiSmallestPos;
    public Vector2 vsiSmallPos;
    public Vector2 vsiLargePos;
    public Vector2 artificialHorizonPos;
    public Vector2 repeaterCompassPos;
    public Vector2 repeaterCompassAlternatePos;
    public Vector2 rpmAPos;
    public Vector2 rpmBPos;



    public float speedoScale;
    public float altScale;
    public float headingScale;
    public float turnAndBankScale;
    public float turnIndicatorScale;
    public float vsiSmallestScale;
    public float vsiSmallScale;
    public float vsiLargeScale;
    public float artificialHorizonScale;
    public float repeaterCompassScale;
    public float repeaterCompassAlternateScale;
    public float rpmAScale;
    public float rpmBScale;


    public bool speedoInTray;
    public bool altimeterInTray;
    public bool headingIndicatorInTray;
    public bool turnAndBankInTray;
    public bool turnIndicatorInTray;
    public bool vsiSmallestInTray;
    public bool vsiSmallInTray;
    public bool vsiLargeInTray;
    public bool artificialHorizonInTray;
    public bool repeaterCompassInTray;
    public bool repeaterCompassAlternateInTray;
    public bool rpmAInTray;
    public bool rpmBInTray;


}
