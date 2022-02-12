using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagButtons : MonoBehaviour
{
    public GameObject menuPanel;
    public PlaneDropdown planeDropdown;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FlagPressed(GameObject buttonObject)
    {
        Debug.Log(buttonObject.name + " pressed");
        //run start to populate and sort lists
        
        planeDropdown.PopulateDropdown(planeDropdown.dropdown, buttonObject.name);
        //show flag panel
        planeDropdown.gameObject.SetActive(true);
        
        //and hide this
        this.gameObject.SetActive(false);
        


    }

    public void backPressed()
    {
        //go back to menu panel
        menuPanel.SetActive(true);
        this.gameObject.SetActive(false);
    }
}
