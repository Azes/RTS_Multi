using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyParticel : MonoBehaviour
{
    ParticleSystem p;


    public void startPart(float x, float y)
    {
        p = GetComponent<ParticleSystem>();
        ParticleSystem.ShapeModule pp = p.shape;
        var sp = pp.scale;
        sp.x = x; sp.y = y;
        pp.scale = sp;
        p.Play();

        Destroy(gameObject, 3);
    }
}
