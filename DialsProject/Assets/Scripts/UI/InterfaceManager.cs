using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InterfaceManager : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{

    public bool move;
    public bool scale;

    private Vector2 positionOnBeginDrag;
    
    public Canvas canvas;
    private RectTransform rectTransform;
    private void Awake()
    {
        rectTransform = transform.parent.parent.GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("OnBeginDrag");
        positionOnBeginDrag = eventData.position;
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

            rectTransform.localScale = new Vector3(clampX, clampY, 0f);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("OnEndDrag");

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

            rectTransform.localScale = new Vector3(clampX, clampY, 0f);

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

    Vector3 KeepFullyOnScreen(GameObject panel, Vector3 newPos)
    {
        RectTransform rect = panel.GetComponent<RectTransform>();
        RectTransform CanvasRect = canvas.GetComponent<RectTransform>();

        float minX = (CanvasRect.sizeDelta.x - rect.sizeDelta.x) * -0.5f;
        float maxX = (CanvasRect.sizeDelta.x - rect.sizeDelta.x) * 0.5f;
        float minY = (CanvasRect.sizeDelta.y - rect.sizeDelta.y) * -0.5f;
        float maxY = (CanvasRect.sizeDelta.y - rect.sizeDelta.y) * 0.5f;

        newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
        newPos.y = Mathf.Clamp(newPos.y, minY, maxY);

        return newPos;
    }

}