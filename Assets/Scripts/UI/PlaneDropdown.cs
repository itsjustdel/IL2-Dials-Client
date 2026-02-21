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

    private List<string> RuPlanes;
    private List<string> UkPlanes;
    private List<string> UsPlanes;
    private List<string> GerPlanes;
    private List<string> ItaPlanes;
    private List<string> FrPlanes;

    public Dropdown dropdown;
    // Start is called before the first frame update

    public void Awake()
    {
      
    }
    public void Start()
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

        UsPlanes = new List<string>(PlaneLists.UsPlanes);
        RuPlanes = new List<string>(PlaneLists.RuPlanes);
        GerPlanes = new List<string>(PlaneLists.GerPlanes);
        UkPlanes = new List<string>(PlaneLists.UkPlanes);
        ItaPlanes = new List<string>(PlaneLists.ItaPlanes);
        FrPlanes = new List<string>(PlaneLists.FrPlanes);

        //SortLists(); //Not using so Flying circus planes stay at the bottom

        dropdown.ClearOptions();

        switch(country)
        {
            case "RU":                
                RuPlanes.Sort(System.StringComparer.OrdinalIgnoreCase);
                dropdown.AddOptions(RuPlanes);
                //if current plane, set as label for quick option
                //if (RuPlanes.Contains(airplaneData.planeType))
               //     dropdown.value = RuPlanes.IndexOf(airplaneData.planeType);

                break;

            case "UK":
                UkPlanes.Sort(System.StringComparer.OrdinalIgnoreCase);
                dropdown.AddOptions(UkPlanes);
               // if (UkPlanes.Contains(airplaneData.planeType))
               //     dropdown.value = UkPlanes.IndexOf(airplaneData.planeType);
                break;

            case "US":
                UsPlanes.Sort(System.StringComparer.OrdinalIgnoreCase);
                dropdown.AddOptions(UsPlanes);
               // if (UsPlanes.Contains(airplaneData.planeType))
               //     dropdown.value = UsPlanes.IndexOf(airplaneData.planeType);
                break;

            case "GER":
                GerPlanes.Sort(System.StringComparer.OrdinalIgnoreCase);
                dropdown.AddOptions(GerPlanes);
               // if (GerPlanes.Contains(airplaneData.planeType))                
               //     dropdown.value = GerPlanes.IndexOf(airplaneData.planeType);
                
                break;

            case "ITA":
                ItaPlanes.Sort(System.StringComparer.OrdinalIgnoreCase);
                dropdown.AddOptions(ItaPlanes);
              //  if (ItaPlanes.Contains(airplaneData.planeType))
              //      dropdown.value = ItaPlanes.IndexOf(airplaneData.planeType);
                break;

            case "FR":
                FrPlanes.Sort(System.StringComparer.OrdinalIgnoreCase);
                dropdown.AddOptions(FrPlanes);
                break;

        }

        
    }

    public void onValueChanged()
    {
        dropdown.transform.GetChild(0).GetComponent<Text>().text = dropdown.transform.GetChild(0).GetComponent<Text>().text.ToUpper();
    }

    public void OnPlaneAccept()
    {
       //set layout open now to stopa race condition where the server asigns an empty plane. if layout is open, plant type is not set by udpclient
        menuHandler.planeDropdownPanelOpen = false;
        menuHandler.layoutOpen = true;

        //save so we know where to go back to after layout close
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
        menuHandler.planeDropdownPanelOpen = false;
    }
}

