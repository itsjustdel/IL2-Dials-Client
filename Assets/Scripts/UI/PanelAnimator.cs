using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelAnimator : MonoBehaviour
{
    public float speed;
    public float animationTarget;    
    public bool animateUp;
    public bool animateDown;
    public List<RectTransform> rects;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(animateUp)
        {
            for (int i = 0; i < rects.Count; i++)
            {                
                rects[i].sizeDelta += new Vector2(speed * Time.deltaTime, 0);
            }

            if (rects[0].sizeDelta.x > animationTarget)
                animateUp = false;
                
        }
    }
}
