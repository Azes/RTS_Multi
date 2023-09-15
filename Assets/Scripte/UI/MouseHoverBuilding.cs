using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MouseHoverBuilding : MonoBehaviour
{

    //new 
    public MainUI mainUI;

    public GameObject buildingInfoObject, createCosteInfoObject, abbrechenUI;
    IINFO info;

    public bool onBuild;
    public float drawDeleay;
    float t;

    private void OnMouseEnter()
    {
        mainUI.inWorldUI = true;//new
    }

    private void OnMouseOver()
    {
        if (t > drawDeleay)
        {
            if (onBuild)
            {
                if(buildingInfoObject != null) 
                    buildingInfoObject.SetActive(false);
                if(createCosteInfoObject != null) 
                    createCosteInfoObject.SetActive(false);

                abbrechenUI.SetActive(true);
            }
            else
            {
                //info
            }
        }
        else t += Time.deltaTime;
    }

    private void OnMouseExit()
    {
        t = 0;
        mainUI.inWorldUI = false;//new
        if(buildingInfoObject != null)
        buildingInfoObject.SetActive(false);
        if(createCosteInfoObject != null)
        createCosteInfoObject.SetActive(false);

        abbrechenUI.SetActive(false);
    }

}
