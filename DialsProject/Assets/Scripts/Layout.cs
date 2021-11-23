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
    public List<Vector2> rpmAPos = new List<Vector2>() { Vector2.zero, Vector2.zero, Vector2.zero };
    public List<Vector2> rpmBPos = new List<Vector2>() { Vector2.zero, Vector2.zero, Vector2.zero };



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
    public List<float> rpmAScale = new List<float>() { 0f, 0f, 0f }; //max 3 engines in game
    public List<float> rpmBScale = new List<float>() { 0f, 0f, 0f };


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
    public List<bool> rpmAInTray = new List<bool>() {false, false, false};
    public List<bool> rpmBInTray = new List<bool>() {false, false, false};


}
