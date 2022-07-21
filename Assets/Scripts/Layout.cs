using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Layout
{
    //class used for saving layout positions to player prefs with JSON
    public float version;

    public string planeType;
    public Vector2 speedoPos;
    public Vector2 altPos;
    public Vector2 headingPos;
    public Vector2 turnAndBankPos;
    public Vector2 turnIndicatorPos;
    public Vector2 vsiPos;
    public Vector2 vsiSmallestPos;//old
    public Vector2 vsiSmallPos;//old
    public Vector2 vsiLargePos;//old -- to remove when all countries use new system
    public Vector2 artificialHorizonPos;
    public Vector2 repeaterCompassPos;
    public Vector2 repeaterCompassAlternatePos;
    public List<Vector2> rpmPos = new List<Vector2>() { Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero };
    public List<Vector2> manifoldPos = new List<Vector2>() { Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero };
    public List<Vector2> waterTempPos = new List<Vector2>() { Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero };

    public float speedoScale;
    public float altScale;
    public float headingScale;
    public float turnAndBankScale;
    public float turnIndicatorScale;
    public float vsiScale;
    public float vsiSmallestScale;//
    public float vsiSmallScale;//
    public float vsiLargeScale;//to remove
    public float artificialHorizonScale;
    public float repeaterCompassScale;
    public float repeaterCompassAlternateScale;
    public List<float> rpmScale = new List<float>() { -1f, -1f, -1f, -1f }; //max 4 engines in game?
    public List<float> manifoldScale = new List<float>() { -1f, -1f, -1f, -1f };
    public List<float> waterTempScale= new List<float>() { -1f, -1f, -1f, -1f };

    public bool speedoInTray;
    public bool altimeterInTray;
    public bool headingIndicatorInTray;
    public bool turnAndBankInTray;
    public bool turnIndicatorInTray;
    public bool vsiInTray;
    public bool vsiSmallestInTray;//
    public bool vsiSmallInTray;//
    public bool vsiLargeInTray;//to remove
    public bool artificialHorizonInTray;
    public bool repeaterCompassInTray;
    public bool repeaterCompassAlternateInTray;
    public List<bool> rpmInTray = new List<bool>() { false, false, false, false };
    public List<bool> manifoldInTray = new List<bool>() { false, false, false, false};
    public List<bool> waterTempInTray = new List<bool>() { false, false, false, false };


    public static float DefaultDialScale(List<GameObject> activeDials)
    {
        //find out if we ned to scale dials to fit them all in the screen (happens if 7 or more dials)
        //length of top will be the longest
        float f = activeDials.Count;
        //round half of count upwards and convert to int. Mathf.Ceil rounds up. If on a whole number, it doesn't round up //https://docs.unity3d.com/ScriptReference/Mathf.Ceil.html
        //half of count because there are two rows
        int longestRow = (int)Mathf.Ceil(f / 2);
        longestRow *= LoadManager.step;//step default step between dials

        GameObject canvasObject = GameObject.FindGameObjectWithTag("Canvas");
        //if longer than the canvas width
        //UnityEngine.Debug.Log("longest row = " + longestRow);
        //UnityEngine.Debug.Log("canvas X = " + canvasObject.GetComponent<RectTransform>().rect.width);

        float scale = 1f;
        if (longestRow > canvasObject.GetComponent<RectTransform>().rect.width)
        {
            //UnityEngine.Debug.Log("row longer than canvas");

            //use this ratio for all positional calculations
            scale = canvasObject.GetComponent<RectTransform>().rect.width / longestRow;

        }

        return scale;
    }

    public static List<GameObject> ActiveDials(GameObject dialsPrefab)
    {
        List<GameObject> activeDials = new List<GameObject>();
        for (int i = 0; i < dialsPrefab.transform.childCount; i++)
            if (dialsPrefab.transform.GetChild(i).gameObject.activeSelf)
                activeDials.Add(dialsPrefab.transform.GetChild(i).gameObject);

        return activeDials;
    }

    public static float scaleOverall = .6f;

    public static void DefaultLayouts(GameObject dialsPrefab)
    {
        //group any 2 engine dials, they need to be beside each other
        List<GameObject> activeDials = ActiveDials(dialsPrefab);

        //find max scale to use for each dial
        RectTransform rectTransform = GameObject.FindGameObjectWithTag("Canvas").GetComponent<RectTransform>();

        int columns = 8;
        int rows = 2;
        
        if (activeDials.Count > 6)
        {
            columns = 5;
            rows = 3;        
        }

        float parentWidth = rectTransform.rect.width;
        float parentHeight = rectTransform.rect.height;
        
        Canvas canvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<Canvas>();
        
        //step is what's smaller from the height and width ratios
        float step = Mathf.Min( parentWidth/ columns, parentHeight / rows);
        //face - width 100 scale 8 so width = 800
        float scale = step/800;        
        float start = -parentWidth * .5f + step * .5f;
        float x = start;
        float y = parentHeight*.5f - step*.5f;

        int thisRowCount = 0;

        
        float lastDiff = 0f;
        for (int i = 0; i < activeDials.Count; i++)
        {
            bool newRow = false;
            if (i + 1 < activeDials.Count
                    && activeDials[i].tag != "Untagged" 
                        && activeDials[i].tag == activeDials[i + 1].tag
                            //next dial's position over screen edge?
                            && x + step> parentWidth*.5f)
                newRow = true;

            if (newRow)
            {
                Debug.Log("new row at " + activeDials[i].name);
                x = start;
                y -= step;

                float diffNewRow = (parentWidth * .5f) / thisRowCount;
                //move all dials in this row along to center
                for (int j = 0; j < thisRowCount; j++)
                {
                    activeDials[i - 1 - j].GetComponent<RectTransform>().anchoredPosition += new Vector2(diffNewRow, 0);
                }

                thisRowCount = 0;                
            }

            activeDials[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);

            x += step;
            thisRowCount++;
            
            //will next dial be over the edge of the screen?
            if(x > parentWidth*.5f)
            {
                x = start;
                y -= step;
                float diff = parentWidth * .5f - x;
                //move all dials in this row along to center
                for (int j = 0; j < thisRowCount; j++)
                {
                    activeDials[i-j].GetComponent<RectTransform>().anchoredPosition -= new Vector2(diff, 0);
                }

                thisRowCount = 0;
                lastDiff = diff;
            }

            activeDials[i].transform.localScale = new Vector3(scale, scale, scale);
        }

        //move all dials in this row along to center - note diff polarity swap
        for (int j = 0; j < thisRowCount; j++)
        {
            activeDials[activeDials.Count-1 - j].GetComponent<RectTransform>().anchoredPosition -= new Vector2(lastDiff-step*.5f, 0);
        }
    }

    /*
     * Dictionary<string, List<GameObject>> dials = new Dictionary<string, List<GameObject>>();
        for (int i = 0; i < activeDials.Count; i++)
        {
            //create a new key with a new list if key doesn't exist
            if (!dials.ContainsKey(activeDials[i].tag))
            {
                dials.Add(activeDials[i].tag, new List<GameObject>() { activeDials[i] });
            }
            //add to list if key already exists
            else
                dials[activeDials[i].tag].Add(activeDials[i]);

        }

     */

    /// Calculates the optimal side of squares to be fit into a rectangle
    /// Inputs: x, y: width and height of the available area.
    ///         n: number of squares to fit
    /// Returns: the optimal side of the fitted squares
    private static float FitSquares(float x, float y, int n)
    {
        float sx, sy;

        var px = Mathf.Ceil(Mathf.Sqrt(n * x / y));
        if (Mathf.Floor(px * y / x) * px < n)
        {
            sx = y / Mathf.Ceil(px * y / x);
        }
        else
        {
            sx = x / px;
        }

        var py = Mathf.Ceil(Mathf.Sqrt(n * y / x));
        if (Mathf.Floor(py * x / y) * py < n)
        {
            sy = x / Mathf.Ceil(x * py / y);
        }
        else
        {
            sy = y / py;
        }

        return Mathf.Max(sx, sy);
    }

    public static void DefaultLayoutsOld(GameObject dialsPrefab)
    {
        //Programtically sort default layouts, so if there is an update, i don't need to create a prefab layout

        //organise dials depending on how many are available
        //we need to know the total amount of active dials before we continue
        List<GameObject> activeDials = ActiveDials(dialsPrefab);

        float scale = DefaultDialScale(activeDials);

        //split in to two rows, if odd number, put more on the top
        for (int i = 0; i < activeDials.Count; i++)
        {
            //ternary statement            
            int odd = activeDials.Count % 2 != 0 ? 1 : 0;

            //if odd, we will add one extra to the top row
            if (i < activeDials.Count / 2 + odd)
            {
                //0 0
                //150 1
                //step 2

                int x = ((int)((activeDials.Count - 1) / 2)) * -LoadManager.step / 2;
                //then add step
                int step = LoadManager.step * (i);
                x += step;

                int y = LoadManager.step / 2;

                //scale and round and convert to int for position
                float xFloat = x * scale;
                x = (int)(Mathf.Round(xFloat));
                float yFloat = y * scale;
                y = (int)(Mathf.Round(yFloat));

                activeDials[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);


            }
            else
            {
                //starting point //from whats left 
                //use "odd" to nudge in to position
                int diff = activeDials.Count - 1 + odd - (activeDials.Count / 2);
                int x = ((int)(diff));
                x *= -LoadManager.step / 2;
                //then add step
                int step = LoadManager.step * (i - (activeDials.Count / 2));
                x += step;

                int y = -LoadManager.step / 2;

                //scale and round and convert to int 
                float xFloat = x * scale;
                x = (int)(Mathf.Round(xFloat));
                float yFloat = y * scale;
                y = (int)(Mathf.Round(yFloat));

                activeDials[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
            }

            //scale dial            
            activeDials[i].transform.localScale = new Vector3(scale * scaleOverall, scale * scaleOverall, scale * scaleOverall);
        }
    }

}
