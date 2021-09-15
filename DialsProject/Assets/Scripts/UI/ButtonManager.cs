using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.EventSystems;

public class ButtonManager : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IPointerClickHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{

    public MenuHandler menuHandler;

  //  public bool moveTray;
    public bool move;    
    public bool scale;
    public bool remove;


  //  public GameObject originalParent;
    
    public Canvas canvas;
    private RectTransform rectTransform;
    private void Awake()
    {
        canvas = GameObject.FindGameObjectWithTag("Canvas").transform.GetComponent<Canvas>();
        menuHandler = GameObject.Find("Menu").GetComponent<MenuHandler>();
      //  originalParent = transform.parent.parent.parent.gameObject;
        rectTransform = transform.parent.parent.GetComponent<RectTransform>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {        
        if (!menuHandler.layoutOpen)
            return;

        if (remove)
        {
            //parent from button press is first parameter
            PutDialInTray(transform.parent.parent.gameObject, menuHandler);
            Debug.Log("Remove Dial Click");

            menuHandler.SaveLayout();



        }

      
    }



    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!menuHandler.layoutOpen)
            return;

        if (move)
        {
            //check the dial is attached to the original parent (not in tray)

            //was it in tray?
            if(menuHandler.dialsInTray.Contains(transform.parent.parent.gameObject))
            //if (transform.parent.parent.parent.gameObject != originalParent)
            {
                //it is in the tray
                //put it back to orignal parent
                transform.parent.parent.gameObject.transform.parent = menuHandler.tcpClient.rN.transform;
                //reset scale
                rectTransform.localScale = new Vector3(0.35f, .35f, 1f);

                //remove from tray list
                menuHandler.dialsInTray.Remove(rectTransform.gameObject);

                //turn on/off empty trays
                menuHandler.UpdateLayoutPanel();

            }
            else
            {

            }
        }
        Debug.Log("OnBeginDrag");
        
       
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!menuHandler.layoutOpen)
            return;

        Debug.Log("OnDrag");

        if (move)
        {
            //int snap = 10;


            Vector2 d2 =  eventData.delta / canvas.scaleFactor ;
            
         //   d2.x = Mathf.Round(d2.x / snap) * snap;
           // d2.y = Mathf.Round(d2.y / snap) * snap;
            rectTransform.anchoredPosition += d2 ;
        }

        if(scale)
        {
           
            Vector2 d2 = eventData.delta ;
            float avg = (d2.x + d2.y) / 2;
            avg /= canvas.scaleFactor;// * Time.deltaTime;
            //scale speed var
            avg /= 200;

            rectTransform.localScale += new Vector3(avg, avg, 0f);

            float clampX = Mathf.Clamp(rectTransform.localScale.x, 0.2f, .75f);
            float clampY = Mathf.Clamp(rectTransform.localScale.y, 0.2f, .75f);

            rectTransform.localScale = new Vector3(clampX, clampY, 1f);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!menuHandler.layoutOpen)
            return;

        Debug.Log("OnEndDrag");


        //check if user dragged dial on to tray        
       // if(IsOverTray())
        {
            //Debug.Log("over tray");
        }

        //snap 

        if (move)
        {
            Vector2 d2 = rectTransform.anchoredPosition;

            //snap
            int snap = 10;

            d2.x = Mathf.Round(d2.x / snap) * snap;
            d2.y = Mathf.Round(d2.y / snap) * snap;

            d2.x = Mathf.Clamp(d2.x, Screen.width * -.5f, Screen.width * .5f);
            float ySize = Screen.height * .5f + rectTransform.rect.height;
            d2.y = Mathf.Clamp(d2.y, Screen.height * -.5f - rectTransform.rect.height, rectTransform.rect.height * canvas.scaleFactor);//bottom, top
            rectTransform.anchoredPosition = d2;

            //trap in screen
            ScreenTrap(d2);

            //make sure icons are 
            IconsOn(transform.parent.parent.gameObject);

        }

        if (scale)
        {
            int snap = 20;

            Vector2 d2 = rectTransform.localScale;
            
            d2.x = Mathf.Round(d2.y * snap) / snap;
            d2.y = Mathf.Round(d2.y* snap) / snap;

            rectTransform.localScale = new Vector3(d2.x, d2.y, 1f);

            //  float clampX = Mathf.Clamp(rectTransform.localScale.x, 0.2f, 1f);
            //  float clampY = Mathf.Clamp(rectTransform.localScale.y, 0.2f, 1f);

            //  rectTransform.localScale = new Vector3(clampX, clampY, 1f);

            //clamping on drag above function
            rectTransform.localScale = new Vector3(d2.x, d2.y, 1f);

            //trap in screen
            ScreenTrap(rectTransform.anchoredPosition);


        }


        menuHandler.SaveLayout();
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!menuHandler.layoutOpen)
            return;

        Debug.Log("OnPointerDown");
        //is it in tray? - make scale larger to  preview dial
        //if (transform.parent.parent.parent.gameObject != originalParent)
        if (menuHandler.dialsInTray.Contains(transform.parent.parent.gameObject))
        {
            rectTransform.localScale = new Vector3(3.5f, 3.5f, 1f);
        }

    }

    public void OnPointerUp(PointerEventData eventData)
    {

        if (!menuHandler.layoutOpen)
            return;

        Debug.Log("OnPointerUp");
        //is it in tray? - put it to small scale (happens after a prewiew click when in tray)
        //if (transform.parent.parent.parent.gameObject != originalParent)
        if (menuHandler.dialsInTray.Contains(transform.parent.parent.gameObject))
        {
            rectTransform.localScale = new Vector3(1f, 1f, 1f);
        }

    }

    public void ScreenTrap(  Vector2 d2 )
    {
        //use canvas height, dial panel size and dial panel scale to find edge of screen
        float bottom = canvas.GetComponent<RectTransform>().rect.height * -.5f + rectTransform.rect.height * .5f * rectTransform.localScale.y;
        //addspace for menu button/leds?
        //bottom += 50;
        float top = canvas.GetComponent<RectTransform>().rect.height * .5f - rectTransform.rect.height * .5f * rectTransform.localScale.x;
        d2.y = Mathf.Clamp(d2.y, bottom, top);

        //width
        float left = canvas.GetComponent<RectTransform>().rect.width * -.5f + rectTransform.rect.width * .5f * rectTransform.localScale.x;
        float right = canvas.GetComponent<RectTransform>().rect.width * .5f - rectTransform.rect.width * .5f * rectTransform.localScale.x;
        d2.x = Mathf.Clamp(d2.x, left, right);

        rectTransform.anchoredPosition = d2;
    }



    //private helpers


    //put dial in tray
    public static void PutDialInTray(GameObject dialParent, MenuHandler menuHandler)
    {

        //find empty tray in children of trayParent

        for (int i = 0; i < menuHandler.trayParent.transform.childCount; i++)
        {
            if (menuHandler.trayParent.transform.GetChild(i).childCount == 0)
            {
                GameObject targetTray = menuHandler.trayParent.transform.GetChild(i).gameObject;
                //we have found an empty tray, place dial in tray
                dialParent.transform.parent = targetTray.transform;

                dialParent.transform.position = targetTray.transform.position;

                dialParent.transform.localScale = new Vector3(1f, 1f, 1f);

                menuHandler.dialsInTray.Add(dialParent);

                //turn image off for icons
                IconsOff(dialParent);

                //turn on/off empty trays
                menuHandler.UpdateLayoutPanel();

                return;

            }
        }
    }

    //put dial in tray
    public static void EmptyTrays( MenuHandler menuHandler)
    {

        //empty list
        menuHandler.dialsInTray.Clear();

        //remove dials in tray hierarchy

        for (int i = 0; i < menuHandler.trayParent.transform.childCount; i++)
        {
            if (menuHandler.trayParent.transform.GetChild(i).childCount != 0)
            {
                //should only be one child/dial ever
                //destroy happens on next frame but there is aparent check before this so let's just null the parent until destroyed
                Destroy(menuHandler.trayParent.gameObject.transform.GetChild(i).GetChild(0).gameObject);
                menuHandler.trayParent.gameObject.transform.GetChild(i).GetChild(0).transform.parent = null;
                
            }
        }
    }

    private static void IconsOn(GameObject dialParent)
    {
        //turn off icons (UI Handlers should be first child)
        for (int j = 0; j < dialParent.transform.GetChild(0).transform.childCount; j++)
        {
            Transform child = dialParent.transform.GetChild(0).transform.GetChild(j);
            if (child.tag == "UIHandler")
            {
                //only turn on dial icons if not in tray

                //child.gameObject.SetActive(toggle);
                child.gameObject.GetComponent<UnityEngine.UI.Image>().enabled = true;
            }
        }
    }

    private static void IconsOff(GameObject dialParent)
    {
        //turn off icons (UI Handlers should be first child)
        for (int j = 0; j < dialParent.transform.GetChild(0).transform.childCount; j++)
        {
            Transform child = dialParent.transform.GetChild(0).transform.GetChild(j);
            if (child.tag == "UIHandler")
            {
                //child.gameObject.SetActive(toggle);
                child.gameObject.GetComponent<UnityEngine.UI.Image>().enabled = false;
            }
        }
    }

    private bool IsOverTray()
    {
        //https://answers.unity.com/questions/1429689/specific-ui-element.html

        bool isOverTaggedElement = false;
        if (EventSystem.current.IsPointerOverGameObject())
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                pointerId = -1,
            };

            pointerData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            Debug.Log(results.Count);
            if (results.Count > 0)
            {
                for (int i = 0; i < results.Count; ++i)
                {
                    Debug.Log(results[i].gameObject.name);
                    if (results[i].gameObject.CompareTag("DialsTray"))
                    {
                        Debug.Log("found");
                        isOverTaggedElement = true;
                    }
                }
            }
        }

        return isOverTaggedElement;
    }

  
}