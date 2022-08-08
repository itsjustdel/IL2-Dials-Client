using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.EventSystems;

public class ButtonManager : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IPointerClickHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public MenuHandler menuHandler;
    public bool move;    
    public bool scale;
    public bool remove;
    public bool compass;
    public bool navArrow;
    public bool gear;
    public bool returnToBoard;
    public GameObject trayParent;
    public bool leftArrow;
    private bool navArrowDown;
    public float navArrowDownSpeed =10f;
    public float navArrowClickSpeed = 0f;
    public float scaleButtonSpeed = 10f;
    public float compassSpinSpeed = 100f;
    public Canvas canvas;
    public RectTransform parentRect;
    public RectTransform dialRect;
    private Quaternion compassTarget;
    private float moveSnap = 20;
    public float scaleSnap = 20;
    private float minScale = 0.1f;
    private float maxScale = 2f;
    public GameObject openContainer;
    public GameObject closedContainer;

    private void Awake()
    {
        canvas = GameObject.FindGameObjectWithTag("Canvas").transform.GetComponent<Canvas>();
        menuHandler = GameObject.Find("Menu").GetComponent<MenuHandler>();
        if (!compass)
        {
            parentRect = transform.parent.parent.parent.GetComponent<RectTransform>();
            dialRect = parentRect.Find("Dial").GetComponent<RectTransform>();
        }
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

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (menuHandler.layoutOpen)
        {
            if (move)
            {
                //was it in tray?
                GameObject dialParent = transform.parent.parent.parent.gameObject;
                if (menuHandler.dialsInTray.Contains(dialParent))
                {                    
                    //it is in the tray
                    //put it back to orignal parent
                    dialParent.transform.parent = menuHandler.udpClient.rN.transform;
                    //reset scale ?                   
                    //float defaultScale = Layout.dialScale;
                    //parentRect.transform.Find("Dial").localScale = new Vector3(defaultScale, defaultScale, 1f);

                    //remove from tray list
                    menuHandler.dialsInTray.Remove(parentRect.gameObject);

                    //close tray
                    menuHandler.TrayPulldown();
                }

                parentRect.SetAsLastSibling();
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
                parentRect.anchoredPosition += d2;
            }

            if (scale)
            {
                if (transform.name == "Arrow Down" || transform.name == "Arrow Up")
                {
                    //don't drag on buttons
                    return;
                }
                //remove parent so the panel doesn't scale
                GameObject container = transform.parent.gameObject;
                GameObject containerParent = container.transform.parent.gameObject;
                Vector3 prevPos = container.transform.position;
                container.transform.parent = null;

                Vector2 d2 = eventData.delta;
                float avg = (d2.x + d2.y) / 2;
                avg /= canvas.scaleFactor;                                         
                avg /= 200;

                //scale only the dial- not the parent or UI
                dialRect.localScale += new Vector3(avg, avg, 0f);

                float clampX = Mathf.Clamp(dialRect.localScale.x, minScale, maxScale);
                float clampY = Mathf.Clamp(dialRect.localScale.y, minScale, maxScale);

                dialRect.localScale = new Vector3(clampX, clampY, 1f);
               
                //re parent
                container.transform.parent = containerParent.transform;
                container.transform.position = prevPos;              
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
            Vector2 d2 = parentRect.anchoredPosition;

            d2.x = Mathf.Round(d2.x / moveSnap) * moveSnap;
            d2.y = Mathf.Round(d2.y / moveSnap) * moveSnap;
           
            parentRect.anchoredPosition = ScreenTrap(d2);

            //ensure correct icons are showing
            GameObject UIHandlerObject = transform.parent.parent.gameObject;
            UIHandlerObject.transform.Find("Container").gameObject.SetActive(true);
            UIHandlerObject.transform.Find("Return Container").gameObject.SetActive(false);

        }

        if (scale)
        {

            //remove parent so the panel doesn't scale
            GameObject container = transform.parent.gameObject;
            GameObject containerParent = container.transform.parent.gameObject;
            Vector3 prevPos = container.transform.position;
            container.transform.parent = null;


            Vector2 d2 = dialRect.localScale;
            
            d2.x = Mathf.Round(d2.y * scaleSnap) / scaleSnap;
            d2.y = Mathf.Round(d2.y* scaleSnap) / scaleSnap;

            //clamping on drag above function
            dialRect.localScale = new Vector3(d2.x, d2.y, 1f);

            //re parent
            container.transform.parent = containerParent.transform;
            container.transform.position = prevPos;
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
            //float defaultScale = 0.6f;// Layout.DefaultDialScale(LoadManager.ActiveDials(menuHandler.dialsManager.countryDialBoard) );
            //parentRect.localScale = new Vector3(defaultScale, defaultScale, 1f);
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
           // parentRect.transform.Find("Dial").localScale = new Vector3(1f, 1f, 1f);
        }

        if (navArrow)
            navArrowDown = false;

        if(gear)
        {    
            //turn on controllers
            closedContainer.SetActive(false); //to scale animation?
            openContainer.SetActive(true); //to scale animation?
        }
        if (scale)
        {
            //remove parent so the panel doesn't scale
            GameObject container = transform.parent.gameObject;            
            GameObject containerParent = container.transform.parent.gameObject;
            Vector3 prevPos = container.transform.position;
            container.transform.parent = null;
            
            if (transform.name == "Arrow Up")
            { 
                dialRect.transform.localScale += new Vector3(scaleButtonSpeed, scaleButtonSpeed, 0f);
              
            } 
            else if (transform.name == "Arrow Down")
            {
                dialRect.transform.localScale -= new Vector3(scaleButtonSpeed, scaleButtonSpeed, 0f);                
            }
            //re parent
            container.transform.parent = containerParent.transform;
            container.transform.position = prevPos;
        }

        if (remove)
        {
            //parent from button press is first parameter
            PutDialInTray(transform.parent.parent.parent.gameObject, menuHandler);
            Debug.Log("Remove Dial Click");

            menuHandler.dialsManager.SaveLayout();

        }
        if (returnToBoard)
        {
            //should have a better way to get country dial board
            transform.parent.parent.parent.parent = menuHandler.udpClient.rN.transform;
        }
    }

    public Vector2 ScreenTrap(  Vector2 d2 )
    {
        RectTransform rectTransformParent = transform.parent.parent.GetComponent<RectTransform>();
        //face width is 100 
        float minWidth = canvas.GetComponent<RectTransform>().rect.width * -.5f + rectTransformParent.localScale.x * 50;
        float maxWidth = canvas.GetComponent<RectTransform>().rect.width * .5f - rectTransformParent.localScale.x * 50;

        float minHeight = canvas.GetComponent<RectTransform>().rect.height *-.5f + rectTransformParent.localScale.x * 50;
        float maxHeight = canvas.GetComponent<RectTransform>().rect.height * .5f - rectTransformParent.localScale.x * 50; 

        d2.x = Mathf.Clamp(d2.x, minWidth, maxWidth);
        d2.y = Mathf.Clamp(d2.y, minHeight, maxHeight);

        return d2;
    }


    //put dial in tray
    public static void PutDialInTray(GameObject dialParent, MenuHandler menuHandler)
    {
        //Debug.Log("Put dial in tray");
        //put canvas width to tray
        RectTransform canvasRect = GameObject.FindGameObjectWithTag("Canvas").GetComponent<RectTransform>();
        RectTransform trayRect = menuHandler.trayParent.GetComponent<RectTransform>();               
        
        trayRect.sizeDelta = new Vector2( canvasRect.sizeDelta.x, canvasRect.sizeDelta.y*.5f);
        dialParent.transform.parent = menuHandler.trayParent.transform;// targetTray.transform;
        menuHandler.dialsInTray.Add(dialParent);

        float max = Mathf.Max(canvasRect.rect.width, canvasRect.rect.height);
        float s = max / 5000;
        //ensure on
        bool trayActive = menuHandler.layoutPanel.activeSelf;
        menuHandler.layoutPanel.SetActive(true);
        GridLayoutGroup glg = GameObject.FindGameObjectWithTag("DialsTray").GetComponent<GridLayoutGroup>();
        //return to previous state - workaround for being inactive on launch. Could set public var and assign for all dials? :S
        menuHandler.layoutPanel.SetActive(trayActive);
        float spacing = s * 800 + 20;//20 general padding
        glg.spacing = new Vector2(spacing, spacing); 
        int p = Mathf.RoundToInt( s * 400);
        glg.padding = new RectOffset(p, p, 0, p);
        dialParent.transform.Find("Dial").transform.localScale = new Vector3(s, s, 1f);

        if (menuHandler.trayPulled)
            menuHandler.TrayPulldown();
        else
            menuHandler.DropDownYTarget();


        //switch icon to reset
        //Debug.Log("Parent = " + dialParent.gameObject);
        dialParent.transform.Find("UI Handlers").Find("Container").gameObject.SetActive(false);
        dialParent.transform.Find("UI Handlers").Find("Return Container").gameObject.SetActive(true);
    }
        
    public static void EmptyTrays( MenuHandler menuHandler)
    {

        for (int i = 0; i < menuHandler.dialsInTray.Count; i++)
        {
            Destroy(menuHandler.dialsInTray[i]);
        }

        //empty list
        menuHandler.dialsInTray.Clear();
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