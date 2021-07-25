//cs example
using UnityEngine;
using System.Collections;

public class grid : MonoBehaviour
{

    private Rect m_LastGraphExtents;
    private static readonly Color kGridMinorColorDark = new Color(0f, 0f, 0f, 0.18f);
    private static readonly Color kGridMajorColorDark = new Color(0f, 0f, 0f, 0.28f);

    void Start()
    {
        CreateLineMaterial();
    }

    void OnGUI()
    {
        DrawGrid();
    }

    void DrawGrid()
    {
        if (Event.current.type != EventType.Repaint)
            return;

        lineMaterial.SetPass(0);

        GL.PushMatrix();
        GL.Begin(GL.LINES);

        DrawGridLines(10.0f, kGridMinorColorDark);

        DrawGridLines(50.0f, kGridMajorColorDark);

        GL.End();
        GL.PopMatrix();

    }

    private void DrawGridLines(float gridSize, Color gridColor)
    {
        GL.Color(gridColor);
        for (float x = 0.0f; x < Screen.width; x += gridSize)
            DrawLine(new Vector2(x, 0.0f), new Vector2(x, Screen.height));
        GL.Color(gridColor);
        for (float y = 0.0f; y < Screen.height; y += gridSize)
            DrawLine(new Vector2(0.0f, y), new Vector2(Screen.width, y));
    }

    private void DrawLine(Vector2 p1, Vector2 p2)
    {
        GL.Vertex(p1);
        GL.Vertex(p2);
    }


    public Material lineMaterial;

    private void CreateLineMaterial()
    {
        if (!lineMaterial)
        {
            lineMaterial = new Material("Shader \"Lines/Colored Blended\" {" +
                "SubShader { Pass { " +
                "    Blend SrcAlpha OneMinusSrcAlpha " +
                "    ZWrite Off Cull Off Fog { Mode Off } " +
                "    BindChannels {" +
                "      Bind \"vertex\", vertex Bind \"color\", color }" +
                "} } }");
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            lineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
        }
    }

}