using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundAni : MonoBehaviour
{
    public RectTransform b1, b2;
    public float speed;
    public float sp1, sp2;
    public float endPos;
    

    // Update is called once per frame
    void Update()
    {
        var t1 = b1.anchoredPosition;
        var t2 = b2.anchoredPosition;

        float t = Time.deltaTime;
        
        t1.x -= speed * t;
        t2.x -= speed * t;

        if(t1.x <= endPos)
        {
            t1.x = sp1;
            t2.x = sp2;
        }

        b1.anchoredPosition = t1;
        b2.anchoredPosition = t2;
    }
}
