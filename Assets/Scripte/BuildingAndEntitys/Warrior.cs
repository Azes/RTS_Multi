using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Warrior : IHuman
{
    public Material om;
    Material _m;
    bool toggel;
    Color nc;

    // Start is called before the first frame update
    void Start()
    {
        //new 
        agent = GetComponent<NavMeshAgent>();
        agent.speed = Speed;
        //

        currentHealth = Health;
        nc = om.GetColor("_BaseColor");
        _m = Instantiate(om);
        GetComponent<Renderer>().material = _m;

    }
    //new
    protected override void nextUpdate()
    {
        if (isSelectet != toggel)
        {
            toggel = isSelectet;
            if (isSelectet) _m.SetColor("_BaseColor", typeColor);
            else _m.SetColor("_BaseColor", nc);
        }

        
        if (isDead()) Destroy(gameObject);
    }
}
