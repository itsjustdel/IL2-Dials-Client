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
    public List<float> waterTempScale = new List<float>() { -1f, -1f, -1f, -1f };

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
    public List<bool> manifoldInTray = new List<bool>() { false, false, false, false };
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

        RectTransform rectTransform = GameObject.FindGameObjectWithTag("Canvas").GetComponent<RectTransform>();
        
        var t = EdgeColumnsRows(rectTransform.rect.width, rectTransform.rect.height, activeDials.Count);
        
        int rows = Mathf.RoundToInt(t.Item1);
        int columns = Mathf.RoundToInt(t.Item2);
        float cellSize = t.Item3;
        float scale = cellSize / 800;
        int d = 0;

        //what's left over after we place each dial on a row - used to center a row
        float diff = (rectTransform.rect.width - (cellSize * columns)) * .5f;

        for (int i = 0; i < rows; i++)
        {            
            for (int j = 0; j < columns; j++)
            {                
                if (d > activeDials.Count - 1)
                {
                    //last row doesn't have enough dials to make it to the end
                    //center all in row
                    for (int p = d-1; p > d-columns; p--)
                    {
                        activeDials[p].GetComponent<RectTransform>().anchoredPosition += new Vector2(cellSize * .5f, 0);
                    }
                    continue;
                }

                float x = cellSize * j;
                float y = -cellSize * i;
                x -= rectTransform.rect.width*.5f - cellSize*.5f;
                
                x += diff;
                
                y += rectTransform.rect.height*.5f - cellSize*.5f;
                
                activeDials[d].GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
                activeDials[d].transform.localScale = new Vector3(scale, scale, 1);
                
                d++;
                
            }
        }
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

    //https://math.stackexchange.com/questions/466198/algorithm-to-get-the-maximum-size-of-n-squares-that-fit-into-a-rectangle-with-a
    //translated form javascript example
    private static (float,float,float) EdgeColumnsRows(float x, float y, float n){        

        // Compute number of rows and columns, and cell size
        float ratio = x / y;
        float ncols_float = Mathf.Sqrt(n * ratio);
        float nrows_float = n / ncols_float;

        // Find best option filling the whole height
        float nrows1 = Mathf.Ceil(nrows_float);
        float ncols1 = Mathf.Ceil(n / nrows1);
        while (nrows1 * ratio < ncols1)
        {
            nrows1++;
            ncols1 = Mathf.Ceil(n / nrows1);
        }
        float cell_size1 = y / nrows1;

        // Find best option filling the whole width
        float ncols2 = Mathf.Ceil(ncols_float);
        float nrows2 = Mathf.Ceil(n / ncols2);
        while (ncols2 < nrows2 * ratio)
        {
            ncols2++;
            nrows2 = Mathf.Ceil(n / ncols2);
        }
        float cell_size2 = x / ncols2;

        // Find the best values
        float nrows, ncols, cell_size;
        if (cell_size1 < cell_size2)
        {
            nrows = nrows2;
            ncols = ncols2;
            cell_size = cell_size2;
        }
        else
        {
            nrows = nrows1;
            ncols = ncols1;
            cell_size = cell_size1;
        }
        return (nrows,ncols,cell_size);
    }

}

