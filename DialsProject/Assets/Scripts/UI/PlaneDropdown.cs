using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
public class PlaneDropdown : MonoBehaviour
{
    public MenuHandler menuHandler;
    public GameObject blurPanel;
    public AirplaneData airplaneData;
    public GameObject flagSelectorObject;
    public List<string> planeNameList = new List<string>() { "test0 (to)", "test1 (t1)" };

    private List<string> RuPlanes = new List<string>()
    {
        "I-16 type 24",
        "LaGG-3 ser.29",
        "Il-2 mod.1941",
        "Il-2 mod.1941",
        "Il-2 mod.1942",
        "Il-2 mod.1943",
        "Yak-1 ser.69",
        "Pe-2 ser.87",
        "La-5 ser.8",
        "MiG-3 ser.24",
        "Pe-2 ser.35",
        "Yak-1 ser.127",
        "Yak-7B ser.36",
        "La-5 FN ser.2",
        "Yak-9 ser.1",
        "Yak-9T ser.1",
        "U-2VS"
    };

    private List<string> UkPlanes = new List<string>()
    {
        "Hurricane Mk.II",
        "Spitfire Mk.Vb",
        "Spitfire Mk.IXe",
        "Tempest Mk.V ser.2",
        "Spitfire Mk.XIV",
        "Typhoon Mk.Ib"
    };

    private List<string> UsPlanes = new List<string>()
    {
        "A-20B",
        "P-40E-1",
        "P-39L-1",
        "P-47D-28",
        "P-51D-15",
        "P-38J-25",
        "P-47D-22",
        "P-51B-5"
    };

    private List<string> GerPlanes = new List<string>()
    {
        "Bf 109 E-7",
        "Bf 109 F-4",
        "Ju-87 D3",
        "Bf 109 G-2",
        "FW 190 A3",
        "He 111 H-6",
        "Ju-52/3m g4e",
        "Bf-110 E2",
        "Bf 109 F-2",
        "Ju-88 A4",
        "Bf 109 G-4",
        "FW 190 A5",
        "Bf-110 G2",
        "He 111 H-16",
        "Hs 129 B-2",
        "Bf 109 G-6",
        "Bf 109 G-14",
        "Bf 109 K-4",
        "Me 262 A",
        "FW 190 D9",
        "Bf 109 G-6 Late",
        "FW 190 A6"
    };

    private List<string> ItaPlanes = new List<string>()
    {
        "MC 202 s8"
    };

    public Dropdown dropdown;
    // Start is called before the first frame update



    public void Awake()
    {

        Debug.Log("Sorting");
        SortLists();
    }
    private void Start()
    {

        //set label to upper
        dropdown.transform.GetChild(0).GetComponent<Text>().text = dropdown.transform.GetChild(0).GetComponent<Text>().text.ToUpper();
    }

    public void SortLists()
    {
        RuPlanes.Sort();
        UkPlanes.Sort();
        UsPlanes.Sort();
        GerPlanes.Sort();

 
    }

    public void PopulateDropdown(Dropdown dropdown, string country)
    {
        RuPlanes.Sort();
        UkPlanes.Sort();
        UsPlanes.Sort();
        GerPlanes.Sort();


        dropdown.ClearOptions();

        switch(country)
        {
            case "RU":                
                dropdown.AddOptions(RuPlanes);
                break;

            case "UK":
                dropdown.AddOptions(UkPlanes);
                break;

            case "US":
                dropdown.AddOptions(UsPlanes);
                break;

            case "GER":
                dropdown.AddOptions(GerPlanes);
                break;

            case "ITA":
                dropdown.AddOptions(ItaPlanes);
                break;

        }
    }

    public void onValueChanged()
    {
        dropdown.transform.GetChild(0).GetComponent<Text>().text = dropdown.transform.GetChild(0).GetComponent<Text>().text.ToUpper();
    }

    public void OnPlaneAccept()
    {
        //save so we know where to go back to after layout close
        menuHandler.planeTypeBeforeLayoutPanel = airplaneData.planeType;

        Debug.Log("Plane accepted = " + dropdown.options[dropdown.value].text);
        if (airplaneData.planeType == dropdown.options[dropdown.value].text)
        {         
            //edit already loaded plane - just "click" layout button
            menuHandler.OpenLayoutClick();

        }
        else
        {
            //force plane change
            airplaneData.planeType = dropdown.options[dropdown.value].text;

            //let dials manager know when it notices a plane change to open layout panel
            airplaneData.menuHandler.dialsManager.openLayoutOnLoad = true;
        }

        //close the panel
        this.gameObject.SetActive(false);
    }

    public void OnBack()
    {
        Debug.Log("On back");
        flagSelectorObject.SetActive(true);
        this.gameObject.SetActive(false);
    }
}

