using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EinheitBar : MonoBehaviour
{
    //new
    public MainUI mainUI;

    public IHuman ih;
    public Image Health;
    public float value;

    private void Start()
    {
        Health.color = ih.typeColor;
    }

    private void Update()
    {
        if (ih == null) return;

        value = Mathf.InverseLerp(0, ih.Health, ih.currentHealth);

        Health.fillAmount = value;

        if(value <= 0)Destroy(gameObject);
    }

    public void selectIt()
    {
        mainUI.selectSingleHuman(ih);
    }

}
