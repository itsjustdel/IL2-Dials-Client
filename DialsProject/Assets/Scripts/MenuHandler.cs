using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuHandler : MonoBehaviour
{
    public GameObject blurPanel;
    public GameObject menuPanel;

    public void MenuButtonClicked()
    {
        Debug.Log("Menu Clicked");

        //set to opposite
        blurPanel.SetActive(!blurPanel.activeSelf);
        menuPanel.SetActive(!menuPanel.activeSelf);
    }
}
