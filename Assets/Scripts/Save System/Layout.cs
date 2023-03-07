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
    public List<Vector2> oilTempInPos = new List<Vector2>() { Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero };
    public List<Vector2> oilTempOutPos = new List<Vector2>() { Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero };
    public List<Vector2> oilTempPressurePos = new List<Vector2>() { Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero };
    public List<Vector2> oilTempComboPos = new List<Vector2>() { Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero };
    public List<Vector2> cylinderHeadPos = new List<Vector2>() { Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero };
    public List<Vector2> carbAirPos = new List<Vector2>() { Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero };

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
    public List<float> oilTempInScale = new List<float>() { -1f, -1f, -1f, -1f };
    public List<float> oilTempOutScale = new List<float>() { -1f, -1f, -1f, -1f };
    public List<float> oilTempPressureScale = new List<float>() { -1f, -1f, -1f, -1f };
    public List<float> oilTempComboScale = new List<float>() { -1f, -1f, -1f, -1f };
    public List<float> cylinderHeadScale = new List<float>() { -1f, -1f, -1f, -1f };
    public List<float> carbAirScale = new List<float>() { -1f, -1f, -1f, -1f };

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
    public List<bool> oilTempInInTray = new List<bool>() { false, false, false, false };
    public List<bool> oilTempOutInTray = new List<bool>() { false, false, false, false };
    public List<bool> oilTempPressureInTray = new List<bool>() { false, false, false, false };
    public List<bool> oilTempComboInTray = new List<bool>() { false, false, false, false };
    public List<bool> cylinderHeadInTray = new List<bool>() { false, false, false, false };
    public List<bool> carbAirInTray = new List<bool>() { false, false, false, false };


    public static int rows;
    public static int columns;
    public static float dialScale = 1f;
    private static float cellSize;
    public static List<GameObject> ActiveDials(GameObject dialsPrefab)
    {
        List<GameObject> activeDials = new List<GameObject>();
        for (int i = 0; i < dialsPrefab.transform.childCount; i++)
            if (dialsPrefab.transform.GetChild(i).gameObject.activeSelf)
                activeDials.Add(dialsPrefab.transform.GetChild(i).gameObject);

        return activeDials;
    }

    public static void RowsColumnsSize(RectTransform rectTransform, int activeDialsCount)
    {
        var t = EdgeColumnsRows(rectTransform.rect.width, rectTransform.rect.height, activeDialsCount);
        rows = Mathf.RoundToInt(t.Item1); //global
        columns = Mathf.RoundToInt(t.Item2); //global
        cellSize = t.Item3;
        dialScale = cellSize / 800; //global
       // Debug.Log("dial scale =" + dialScale);
    }

    public static void DefaultLayouts(List<GameObject> activeDials, RectTransform canvasRectTransform)
    {
        
        int total = 0;

        //what's left over after we place each dial on a row - used to center a row
        float diff = (canvasRectTransform.rect.width - (cellSize * columns)) * .5f;

        for (int i = 0; i < rows; i++)
        {
            int thisRow = 0;
            for (int j = 0; j < columns; j++)
            {                
                if (total > activeDials.Count - 1)
                {
                    //last row doesn't have enough dials to make it to the end
                    //center all in row
                    for (int p = total - 1; p >= total - thisRow; p--)
                    {
                        activeDials[p].GetComponent<RectTransform>().anchoredPosition += new Vector2(cellSize * .5f, 0);
                    }
                    continue;
                }

                float x = cellSize * j;
                float y = -cellSize * i;
                x -= canvasRectTransform.rect.width*.5f - cellSize*.5f;
                
                x += diff;
                
                y += canvasRectTransform.rect.height*.5f - cellSize*.5f;
                activeDials[total].GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
                activeDials[total].transform.Find("Dial").transform.localScale = new Vector3(dialScale, dialScale, 1);
                
                //set ui scale this time only
                activeDials[total].transform.Find("UI Handlers").transform.localScale = new Vector3(dialScale, dialScale, 1);

                total++;
                thisRow++;
                
            }
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

