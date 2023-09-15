using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowsSettings : MonoBehaviour
{
    private Vector2 minWindowSize = new Vector2(640, 480); // Minimale Fenstergröße
    private Vector2 maxWindowSize = new Vector2(1920, 1080); // Maximale Fenstergröße
    private float targetAspectRatio = 16f / 9f; // Gewünschtes Seitenverhältnis (16:9)
    private RectTransform rectTransform;



   

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    private void OnRectTransformDimensionsChange()
    {
        UpdateWindowSize();
    }


    private void UpdateWindowSize()
    {
        if (rectTransform == null)
            return;

        float currentAspectRatio = (float)Screen.width / Screen.height;
        float scaleMultiplier = targetAspectRatio / currentAspectRatio;

        float scaledWidth = Mathf.Clamp(Screen.width * scaleMultiplier, minWindowSize.x, maxWindowSize.x);
        float scaledHeight = Mathf.Clamp(Screen.height, minWindowSize.y, maxWindowSize.y);

        Screen.MoveMainWindowTo(Screen.mainWindowDisplayInfo, new Vector2Int(Screen.mainWindowPosition.x, Screen.mainWindowPosition.y));
        Screen.SetResolution((int)scaledWidth, (int)scaledHeight, Screen.fullScreenMode);
        Screen.MoveMainWindowTo(Screen.mainWindowDisplayInfo, new Vector2Int(Screen.mainWindowPosition.x, Screen.mainWindowPosition.y));
    }
}
