using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Villager : IHuman
{
    //new
    public Animator ani;
    public SkinnedMeshRenderer smr;
    public bool switchani;
    //
    public Material stM;
    Material _m;
    bool toggel;
    //Color or;

    public bool isWork;
    public int carryweight = 200;

    
    public Transform walkTarget;

    // Start is called before the first frame update
    void Start()
    {
       
        agent = GetComponent<NavMeshAgent>();
         agent.speed = Speed;
       
        //new
        _m = Instantiate(stM);
        smr.materials[1] = _m;
    }

    //new wir benutzen jetzt die eigene Update methode (movement tut)
    protected override void nextUpdate()
    {
        //new
        if (isSelectet != toggel)
        {
            toggel = isSelectet;
            if (isSelectet) smr.materials[1].SetFloat("_OutlineTick", 0.5f);
            else smr.materials[1].SetFloat("_OutlineTick", 0.0f);
        }

        removeOnId(agent);
        //removeOnId(GetComponent<Villager>());

        if (moveTo && !switchani)
        {
            switchani = true;
            ani.SetTrigger("walk");
        }
        else if(!moveTo && switchani)
        {
            switchani = false;
            ani.SetTrigger("normal");
        }
        if (isDead()) Destroy(gameObject);
    }

    
}
