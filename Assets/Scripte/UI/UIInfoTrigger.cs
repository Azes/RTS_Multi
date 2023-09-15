using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIInfoTrigger : MonoBehaviour
{
    //das Info fenster Object und sein recttransform 
    public GameObject HoverObject;
    RectTransform hoRect;

    public GameObject buildingInfoObject, createCosteInfoObject;

    public TextMeshProUGUI[] costeInfo;
    //[Gold]  = [0]
    //[Stone] = [1]
    //[Wood]  = [2]
    //[Food]  = [3]
    //[Time] =  [4]

    RectTransform rectTransform;
    Vector3[] cc = new Vector3[4];

    public IINFO info;
    public bool show;
    public float waitForShowTime;
    float t;

    private void Start()
    {
        hoRect = HoverObject.GetComponent<RectTransform>();
        rectTransform = GetComponent<RectTransform>();
    }


    // Update is called once per frame
    void Update()
    {
        Vector3 sp = Input.mousePosition;

        rectTransform.GetWorldCorners(cc);

        if (sp.x >= cc[0].x && sp.x <= cc[2].x &&
            sp.y >= cc[0].y && sp.y <= cc[1].y)
        {
            t += Time.deltaTime;
            if(t > waitForShowTime)
            {
                show = true;
                costeInfo[0].text = info.costInfo.Name;
                costeInfo[1].text = info.costInfo.Gold.ToString();
                costeInfo[2].text = info.costInfo.Stone.ToString();
                costeInfo[3].text = info.costInfo.Wood.ToString();
                costeInfo[4].text = info.costInfo.Food.ToString();
                costeInfo[5].text = System.Math.Round(info.costInfo.ProcessTime, 2).ToString();
            }
        }
        else
        {
            show=false;
            t = 0;
        }


        if (show)
        {
            if (!HoverObject.activeInHierarchy) HoverObject.SetActive(true);
          
            if (buildingInfoObject.activeInHierarchy) buildingInfoObject.SetActive(false);
            if (!createCosteInfoObject.activeInHierarchy) createCosteInfoObject.SetActive(true);

            hoRect.anchoredPosition = new Vector2(
                Input.mousePosition.x - (Screen.width/2) + 30,
                Input.mousePosition.y - (Screen.height/2) + 30);
        }
        else if (HoverObject.activeInHierarchy) HoverObject.SetActive(false);
    }
}
