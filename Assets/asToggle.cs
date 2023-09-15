using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(Button))]
public class asToggle : MonoBehaviour
{

    private int mode;

    public Sprite[] icons;
    public Color[] iconsColors;
    public UnityEvent[] _Event;

    Image i;
    Button b;

    // Start is called before the first frame update
    void Start()
    {
        b = GetComponent<Button>();    
        i = GetComponent<Image>();
        i.sprite = icons[0];
        i.color = iconsColors[0];
    }

    public void KlickEvent()
    {
        mode++;
        
        if(mode >= _Event.Length)
            mode = 0;

        i.sprite = icons[mode];
        i.color = iconsColors[mode];
        _Event[mode].Invoke();
    }
    
}
