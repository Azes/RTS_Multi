using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class waitObject : MonoBehaviour
{
    public Image icon;
    public IHuman h;
    public bool change;

    // Start is called before the first frame update
    void Start()
    {
        icon = GetComponent<Image>();
        icon.sprite = h.costInfo.icon;
    }

    public void onSelect()
    {
        change = true;
    }
}
