using System;
using System.Collections.Generic;

/// <summary>
/// Data models for plane configuration JSON
/// Matches the structure of plane-config.json
/// Note: Using SimpleJSON for parsing since Unity's JsonUtility doesn't support dictionaries
/// </summary>
[Serializable]
public class PlaneConfigRoot
{
    // Optional top-level version string for the config bundle
    public string version;
    // Parsed planes will be stored here after loading
    public Dictionary<string, PlaneConfigData> planes = new Dictionary<string, PlaneConfigData>();
}

[Serializable]
public class PlaneConfigData
{
    public string country;
    public bool altimeter;
    public bool headingIndicator;
    public bool turnAndBank;
    public bool artificialHorizon;
    public bool turnCoordinator;
    public bool vsiSmallest;
    public bool vsiSmall;
    public bool vsiLarge;
    public bool repeaterCompass;
    public bool repeaterCompassAlternate;
    public string speedometerType;
    public string rpmType;
    public string manifoldType;
    public string turnCoordinatorType;
    public string vsiType;
    public string horizonType;
    public string headingIndicatorType;
    public string waterTempType;
    public string oilTempInType;
    public string oilTempOutType;
    public string oilTempPressureType;
    public string oilTempComboType;
    public string cylinderHeadType;
    public string carbAirTempType;
    public int engines = 1;

    /// <summary>
    /// Convert config data to PlaneAttributes
    /// </summary>
    public PlaneDataFromName.PlaneAttributes ToPlaneAttributes()
    {
        var attributes = new PlaneDataFromName.PlaneAttributes();
        
        // Parse country
        attributes.country = ParseCountry(country);
        
        // Boolean properties
        attributes.altimeter = altimeter;
        attributes.headingIndicator = headingIndicator;
        attributes.turnAndBank = turnAndBank;
        attributes.artificialHorizon = artificialHorizon;
        attributes.turnCoordinator = turnCoordinator;
        attributes.vsiSmallest = vsiSmallest;
        attributes.vsiSmall = vsiSmall;
        attributes.vsiLarge = vsiLarge;
        attributes.repeaterCompass = repeaterCompass;
        attributes.repeaterCompassAlternate = repeaterCompassAlternate;
        
        // Dial variant types
        attributes.speedometerType = ParseDialVariant(speedometerType);
        attributes.rpmType = ParseDialVariant(rpmType);
        attributes.manifoldType = ParseDialVariant(manifoldType);
        attributes.turnCoordinatorType = ParseDialVariant(turnCoordinatorType);
        attributes.vsiType = ParseDialVariant(vsiType);
        attributes.horizonType = ParseDialVariant(horizonType);
        attributes.headingIndicatorType = ParseDialVariant(headingIndicatorType);
        attributes.waterTempType = ParseDialVariant(waterTempType);
        attributes.oilTempInType = ParseDialVariant(oilTempInType);
        attributes.oilTempOutType = ParseDialVariant(oilTempOutType);
        attributes.oilTempPressureType = ParseDialVariant(oilTempPressureType);
        attributes.oilTempComboType = ParseDialVariant(oilTempComboType);
        attributes.cylinderHeadType = ParseDialVariant(cylinderHeadType);
        attributes.carbAirTempType = ParseDialVariant(carbAirTempType);
        
        // Engine count
        attributes.engines = engines;
        
        return attributes;
    }

    private Country ParseCountry(string countryStr)
    {
        if (string.IsNullOrEmpty(countryStr))
            return Country.UNDEFINED;
            
        switch (countryStr.ToUpper())
        {
            case "RU": return Country.RU;
            case "GER": return Country.GER;
            case "US": return Country.US;
            case "UK": return Country.UK;
            case "ITA": return Country.ITA;
            case "FR": return Country.FR;
            default: return Country.UNDEFINED;
        }
    }

    private DialVariant ParseDialVariant(string variantStr)
    {
        if (string.IsNullOrEmpty(variantStr) || variantStr == "None")
            return DialVariant.None;
            
        switch (variantStr.ToUpper())
        {
            case "A": return DialVariant.A;
            case "B": return DialVariant.B;
            case "C": return DialVariant.C;
            case "D": return DialVariant.D;
            case "E": return DialVariant.E;
            case "F": return DialVariant.F;
            default: return DialVariant.None;
        }
    }
}
