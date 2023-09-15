using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MenuInfoBox : MonoBehaviour
{
    public float stPos, endPos;
    public float blendSpeed;
    public TextMeshProUGUI infoText;
    RectTransform rect;


    bool printInfo, inOut;
    float t, DrawTime;

    private void Start()
    {
        rect = GetComponent<RectTransform>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (printInfo)
        {
            var p = rect.anchoredPosition;

            if (!inOut)
            {
                p.x -= blendSpeed * Time.deltaTime;

                if (p.x <= endPos)
                {
                    p.x = endPos;
                    inOut = true;
                }
            }

            if (inOut)
            {
                if(t >= DrawTime)
                {
                    p.x += blendSpeed * Time.deltaTime;

                    if(p.x >= stPos)
                    {
                        t = 0;
                        inOut = false;
                        printInfo = false;
                    }
                }
                else t += Time.deltaTime;

            }

            rect.anchoredPosition = p;
        }        
    }

    public void setInfo(string info, float showTime)
    {
        DrawTime = showTime;
        infoText.text = info;
        printInfo = true;
    }

}
