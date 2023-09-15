using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class RandomLoadScreen : MonoBehaviour
{
   
    public Sprite[] waitScreens;
    public Image waitScreen;


    private void Awake()
    {
        waitScreen.sprite = waitScreens[Random.Range(0, waitScreens.Length - 1)];
    }


}
