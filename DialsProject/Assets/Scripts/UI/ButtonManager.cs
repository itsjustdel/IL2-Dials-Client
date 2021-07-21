using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonManager : MonoBehaviour,  IPointerDownHandler , IPointerClickHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{

    public MenuHandler menuHandler;

  //  public bool moveTray;
    public bool move;    
    public bool scale;
    public bool remove;


    public GameObject originalParent;
    
    public Canvas canvas;
    private RectTransform rectTransform;
    private void Awake()
    {
        originalParent = transform.parent.parent.parent.gameObject;
        rectTransform = transform.parent.parent.GetComponent<RectTransform>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (remove)
        {
            DialInTray();
            Debug.Log("Remove Dial Click");
            
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("OnBeginDrag");
        
        if(move)
        {
            //check the dial is attached to the original parent (not in tray)

            //was it in tray?
            if(transform.parent.parent.parent.gameObject != originalParent)
            {
                //it is in the tray
                //put it back
                transform.parent.parent.parent = originalParent.transform;
                //reset scale
                rectTransform.localScale = new Vector3(0.5f, .5f, 1f);

                //remove from tray list
                menuHandler.dialsInTray.Remove(rectTransform.gameObject);
                
            }
            else
            {

            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
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
            avg /= 100;

            rectTransform.localScale += new Vector3(avg, avg, 0f);

            float clampX = Mathf.Clamp(rectTransform.localScale.x, 0.2f, 1f);
            float clampY = Mathf.Clamp(rectTransform.localScale.y, 0.2f, 1f);

            rectTransform.localScale = new Vector3(clampX, clampY, 1f);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("OnEndDrag");


        //check if user dragged dial on to tray        
        if(IsOverTray())
        {
            
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


         //   if (moveTray)
            {
                //make sure icons are on
                IconsOffOrOn(transform.parent.parent.gameObject, true);
            }
        }

        if (scale)
        {
            int snap = 20;

            Vector2 d2 = rectTransform.localScale;
            
            d2.x = Mathf.Round(d2.y * snap) / snap;
            d2.y = Mathf.Round(d2.y* snap) / snap;

            rectTransform.localScale = new Vector3(d2.x, d2.y, 1f);

            float clampX = Mathf.Clamp(rectTransform.localScale.x, 0.2f, 1f);
            float clampY = Mathf.Clamp(rectTransform.localScale.y, 0.2f, 1f);

            rectTransform.localScale = new Vector3(clampX, clampY, 1f);

            //trap in screen
            ScreenTrap(rectTransform.anchoredPosition);


        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("OnPointerDown");

        
    }

    public void ScreenTrap(  Vector2 d2 )
    {
        //use canvas height, dial panel size and dial panel scale to find edge of screen
        float bottom = canvas.GetComponent<RectTransform>().rect.height * -.5f + rectTransform.rect.height * .5f * rectTransform.localScale.y;
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
    private void DialInTray()
    {
        //get parent from "romove" button pressed
        GameObject dialParent = transform.parent.parent.gameObject;

        //find empty tray in children of trayParent

        for (int i = 0; i < menuHandler.trayParent.transform.childCount; i++)
        {
            if (menuHandler.trayParent.transform.GetChild(i).childCount == 0)
            {
                GameObject targetTray = menuHandler.trayParent.transform.GetChild(i).gameObject;
                //we have found an empty tray, place dial in tray
                dialParent.transform.parent = targetTray.transform;

                dialParent.transform.position = targetTray.transform.position;

                dialParent.transform.localScale = new Vector3(1.5f, 1.5f, 1f);

                menuHandler.dialsInTray.Add(dialParent);

                //false = off
                IconsOffOrOn(dialParent,false);

                //set draggable button on


                return;


            }
        }
    }

    private void IconsOffOrOn(GameObject dialParent,bool toggle)
    {
        //turn off icons (UI Handlers should be first child)
        for (int j = 0; j < dialParent.transform.GetChild(0).transform.childCount; j++)
        {
            Transform child = dialParent.transform.GetChild(0).transform.GetChild(j);
            if (child.tag == "UIHandler")
            {
                child.gameObject.SetActive(toggle);
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