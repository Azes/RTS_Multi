using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ModeButton : MonoBehaviour
{
    public Sprite[] modeIcon;
    public int modeSize;
    public int mode;
    bool change;
    Button b;

    private void Start()
    {
        b = GetComponent<Button>();  
    }

    public void changeMode()
    {
        if(mode < modeSize)
        {
            change = true;
            mode++;

            if(mode == modeSize)mode = 0;

            b.image.sprite = modeIcon[mode];
        }
    }

    public bool isChange()
    {
        bool i = change;
        change = false;
        return i;
    }
}
