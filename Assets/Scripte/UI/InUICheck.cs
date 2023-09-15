using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//new (network tut)
public class InUICheck : MonoBehaviour
{
    
    public bool inUI;
    public RectTransform rect;
    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }
    //static List<bool> its = new List<bool>();
    //static int index;
    //private int klass_index;
    //
    //Vector3[] cc = new Vector3[4];
    //
    //RectTransform rectTransform;
    //
    //private void Start()
    //{
    //    rectTransform = GetComponent<RectTransform>();
    //    its.Add(false);
    //    index++;
    //
    //    klass_index = index;
    //}
    //
    //private void Update()
    //{
    //    rectTransform.GetWorldCorners(cc);
    //    var sp = Input.mousePosition;
    //
    //    if(sp.x >= cc[0].x && sp.x <= cc[2].x &&
    //        sp.y >= cc[0].y && sp.y <= cc[1].y)
    //    {
    //        MainUI.inUI = true;
    //        its[klass_index-1] = true;
    //    }
    //    else
    //    {
    //        its[klass_index-1] = false;
    //
    //        if (allFalse()) MainUI.inUI = false;
    //    }
    //
    //}
    //
    //bool allFalse()
    //{
    //    for (int i = 0; i < its.Count; i++)
    //        if (its[i]) return false;
    //    return true;
    //}
}