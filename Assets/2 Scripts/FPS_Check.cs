using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPS_Check : MonoBehaviour
{
    public int fFont_Size;
    public float R, G, B;

    float deltaTime = 0.0f;

    private void Start() {
        fFont_Size = fFont_Size == 0 ? 50 : fFont_Size;
    }
    private void Update() {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }
    private void OnGUI() {
        int w = Screen.width, h = Screen.height;

        GUIStyle style = new GUIStyle();

        Rect rect = new Rect(0, 0, w, h * 0.02f);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / fFont_Size;
        style.normal.textColor = new Color(R, G, B, 1.0f);
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
        GUI.Label(rect, text, style);
    }
}
