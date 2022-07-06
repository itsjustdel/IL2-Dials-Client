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
    public bool compass;
    public bool navArrow;
    public GameObject trayParent;
    public bool leftArrow;
    private bool navArrowDown;
    public float navArrowDownSpeed =10f;
    public float navArrowClickSpeed = 0f;

    public float compassSpinSpeed = 100f;

    
    public Canvas canvas;
    private RectTransform rectTransform;

    private Quaternion compassTarget;

    private float minScale = 0.1f;
    private float maxScale = 2f;

    

    private void Awake()
    {
        canvas = GameObject.FindGameObjectWithTag("Canvas").transform.GetComponent<Canvas>();
        menuHandler = GameObject.Find("Menu").GetComponent<MenuHandler>();
        rectTransform = transform.parent.parent.GetComponent<RectTransform>();
    }

    private void Update()
    {
        if(compass)
        {
             transform.rotation = Quaternion.RotateTowards(transform.rotation, compassTarget, Time.deltaTime * compassSpinSpeed);
        }

        if(navArrow && navArrowDown)
        {
            
            if (leftArrow)
                trayParent.transform.position -= Vector3.right * navArrowDownSpeed * Time.fixedDeltaTime;
            else
                trayParent.transform.position += Vector3.right * navArrowDownSpeed * Time.fixedDeltaTime;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {

        if (!menuHandler.layoutOpen)
            return;

        if (remove)
        {
            //parent from button press is first parameter
            PutDialInTray(transform.parent.parent.gameObject, menuHandler);
            //Debug.Log("Remove Dial Click");

            menuHandler.dialsManager.SaveLayout();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (menuHandler.layoutOpen)
        {
            if (move)
            {
                //check the dial is attached to the original parent (not in tray)

                //was it in tray?
                if (menuHandler.dialsInTray.Contains(transform.parent.parent.gameObject))                
                {
                    //it is in the tray
                    //put it back to orignal parent
                    transform.parent.parent.gameObject.transform.parent = menuHandler.udpClient.rN.transform;
                    //reset scale
                    List<GameObject> dials = Layout.ActiveDials(menuHandler.dialsManager.countryDialBoard);
                    float defaultScale = Layout.DefaultDialScale(dials);
                    //defauly scale in prefab is 0.6f, factor this in
                    defaultScale *= Layout.scaleOverall;
                    rectTransform.localScale = new Vector3(defaultScale, defaultScale, 1f);

                    //remove from tray list
                    menuHandler.dialsInTray.Remove(rectTransform.gameObject);

                    //turn on/off empty trays
                    menuHandler.UpdateLayoutPanel();
                }
            }
        }
       
    }

    public void OnDrag(PointerEventData eventData)
    {

        if (menuHandler.layoutOpen)
        {
            if (move)
            {
                Vector2 d2 = eventData.delta / canvas.scaleFactor;
                rectTransform.anchoredPosition += d2;
            }

            if (scale)
            {

                Vector2 d2 = eventData.delta;
                float avg = (d2.x + d2.y) / 2;
                avg /= canvas.scaleFactor;// * Time.deltaTime;
                                          //scale speed var
                avg /= 200;

                rectTransform.localScale += new Vector3(avg, avg, 0f);

                float clampX = Mathf.Clamp(rectTransform.localScale.x, minScale, maxScale);
                float clampY = Mathf.Clamp(rectTransform.localScale.y, minScale, maxScale);

                rectTransform.localScale = new Vector3(clampX, clampY, 1f);
            }
        }
        else
        {
            //dragging to move arrows
            compassTarget = GetCompassTarget(eventData);

        }

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!menuHandler.layoutOpen)
            return;

        //Debug.Log("OnEndDrag");

        //snap 
        if (move)
        {
            Vector2 d2 = rectTransform.anchoredPosition;


            //snap
            int snap = 10;

            d2.x = Mathf.Round(d2.x / snap) * snap;
            d2.y = Mathf.Round(d2.y / snap) * snap;

            //  d2.x = Mathf.Clamp(d2.x, Screen.width * -.5f, Screen.width * .5f);
            //  float ySize = Screen.height * .5f + rectTransform.rect.height;
            //  d2.y = Mathf.Clamp(d2.y, Screen.height * -.5f - rectTransform.rect.height, rectTransform.rect.height * canvas.scaleFactor);//bottom, top
             rectTransform.anchoredPosition = ScreenTrap(d2);


            //make sure icons are 
            //Debug.Log(transform.parent.parent.gameObject.name);
            IconsOn(transform.parent.parent.gameObject);

        }

        if (scale)
        {
            int snap = 20;

            Vector2 d2 = rectTransform.localScale;
            
            d2.x = Mathf.Round(d2.y * snap) / snap;
            d2.y = Mathf.Round(d2.y* snap) / snap;

            rectTransform.localScale = new Vector3(d2.x, d2.y, 1f);

            //clamping on drag above function
            rectTransform.localScale = new Vector3(d2.x, d2.y, 1f);

            //trap in screen
            ScreenTrap(rectTransform.anchoredPosition);

        }


        menuHandler.dialsManager.SaveLayout();
        
    }

    Quaternion GetCompassTarget(PointerEventData eventData)
    {
        // Get Angle in Radians
        float AngleRad = Mathf.Atan2(eventData.position.y - transform.position.y, eventData.position.x - transform.position.x);
        // Get Angle in Degrees
        float AngleDeg = (180 / Mathf.PI) * AngleRad;
        // Rotate Object

        Quaternion target = Quaternion.Euler(0, 0, AngleDeg - 90);

        return target;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (compass)
        {
            //one time to click to move arrows on compass
            //Debug.Log("Compass Pointer Click");           

            compassTarget = GetCompassTarget(eventData);
        }

        //Debug.Log("OnPointerDown");

        if (!menuHandler.layoutOpen)
            return;
      
        //is it in tray? - make scale larger to preview dial
        if (menuHandler.dialsInTray.Contains(transform.parent.parent.gameObject))
        {
            //using same functions as when we load the dials to find out what size they are by default
            float defaultScale = Layout.DefaultDialScale(LoadManager.ActiveDials(menuHandler.dialsManager.countryDialBoard) );
            rectTransform.localScale = new Vector3(defaultScale, defaultScale, 1f);
        }

        if (navArrow)
        {
            navArrowDown = true;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!menuHandler.layoutOpen)
            return;

        //Debug.Log("OnPointerUp");
        //is it in tray? - put it to small scale (happens after a preview click when in tray)
        //if (transform.parent.parent.parent.gameObject != originalParent)
        if (menuHandler.dialsInTray.Contains(transform.parent.parent.gameObject))
        {
            rectTransform.localScale = new Vector3(1f, 1f, 1f);
        }

        if (navArrow)
            navArrowDown = false;
    }

    public Vector2 ScreenTrap(  Vector2 d2 )
    {
        RectTransform rectTransformParent = transform.parent.parent.GetComponent<RectTransform>();
        //400 because face width is 100 and face scale is 8. Move half of that out
        float minWidth = canvas.GetComponent<RectTransform>().rect.width * -.5f + rectTransformParent.localScale.x * 400;
        float maxWidth = canvas.GetComponent<RectTransform>().rect.width * .5f - rectTransformParent.localScale.x * 400;

        
        float minHeight = canvas.GetComponent<RectTransform>().rect.height * -.5f + rectTransformParent.localScale.x * 400;
        float maxHeight = canvas.GetComponent<RectTransform>().rect.height * .5f - rectTransformParent.localScale.x * 400; 

        d2.x = Mathf.Clamp(d2.x, minWidth, maxWidth);
        d2.y = Mathf.Clamp(d2.y, minHeight, maxHeight);

        return d2;
    }


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
                //Debug.Log("B4 =" + dialParent.transform.parent.name);
                dialParent.transform.parent = targetTray.transform;
                //Debug.Log("After =" + dialParent.transform.parent.name);
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

    public static void IconsOn(GameObject dialParent)
    {
        //turn  icons (UI Handlers should be first child)
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

    public void TrayArrowLeft(GameObject parent)
    {
        Debug.Log("Tray Arrow Left");
        parent.transform.position -= navArrowClickSpeed * Vector3.right;
    }

    public void TrayArrowRight(GameObject parent)
    {
        Debug.Log("Tray Arrow Right");
        parent.transform.position += navArrowClickSpeed * Vector3.right;
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

            //Debug.Log(results.Count);
            if (results.Count > 0)
            {
                for (int i = 0; i < results.Count; ++i)
                {
                    //Debug.Log(results[i].gameObject.name);
                    if (results[i].gameObject.CompareTag("DialsTray"))
                    {
                        //Debug.Log("found");
                        isOverTaggedElement = true;
                    }
                }
            }
        }

        return isOverTaggedElement;
    }

  
}