using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StadtCentrum : IBuilding
{


    //ressoucres
    public int Gold, Stone, Wood, Food, Population, MaxPopulation = 25;
    int gbuff, sbuff, wbuff, fbuff, pbuff, mpbuff;
    public MainUI mui;

    //Humans
    public GameObject humanPrebas;
    public Transform Startspawn, endSpawn;

    public List<IHuman> ALL_HUMANS = new List<IHuman>();

    //UI elemente
    public TextMeshProUGUI processCount, jobless, buildhealth;
    public ProcessElement pe;

    int healthbuff;


    //new buildingsystem
    public IBuilding[] buildings;


    // Start is called before the first frame update
    void Start()
    {
       // buildingHealth = 100000;
       // pe.processLimet = 120;
    }

    // Update is called once per frame
    void Update()
    {

        //check ist sz instanziert
        if (processCount == null || mui == null || jobless == null) return;

        pe.processLimet = 120;
        processCount.text = pe.waitHumans.Count.ToString();
        updateJobless();
        updateBuildHealthText();
        UpdateResouces();

        if (pe.endProcess)
        {
            bool can = (Population + 1 < MaxPopulation ? true : false);
         
            if (can)
            {//new
               // IHuman human = pe.spawnHuman(Startspawn, true).GetComponent<IHuman>();
               // ALL_HUMANS.Add(human);
               // human.walkTo(endSpawn.position, 1);
               // Population = ALL_HUMANS.Count;
            }  //
        }
        
    }

    void UpdateResouces()
    {
        if(Gold != gbuff || Stone != sbuff || Wood != wbuff 
            || Food != fbuff || Population != pbuff|| MaxPopulation != mpbuff)
        {
            gbuff = Gold; sbuff = Stone;
            wbuff = Wood; fbuff = Food;
            pbuff = Population; mpbuff = MaxPopulation;
        
            mui.gold_value.text = Gold.ToString();
            mui.stone_value.text = Stone.ToString();
            mui.wood_value.text = Wood.ToString();
            mui.food_value.text = Food.ToString();
            mui.pop_value.text = Population.ToString();
            mui.maxpop_value.text = MaxPopulation.ToString();

        }
    }

    void updateJobless()
    {
        int j = 0;

        for (int i = 0; i < ALL_HUMANS.Count; i++)
        {
            if (ALL_HUMANS[i] is Villager)
            {
                if (!ALL_HUMANS[i].GetComponent<Villager>().isWork)
                {
                    j++;
                }
            }
        }

        jobless.text = j.ToString();
    }

    void updateBuildHealthText()
    {
        if(buildingCurrentHealth != healthbuff)
        {
            healthbuff = buildingCurrentHealth;

            buildhealth.text = buildingCurrentHealth.ToString() + " | " + buildingHealth.ToString();
        }
    }

    public void addHumanToProgess()
    {
        var info = humanPrebas.GetComponent<IHuman>().costInfo;
        if (Gold - info.Gold >= 0 && Food - info.Food >= 0)
        {
            Gold -= info.Gold; 
            Food -= info.Food;
            pe.addHumanToProcess(humanPrebas.GetComponent<IHuman>());
        }
    }

    //new wir können die butten nicht zuweisen also überprüfen wir hier nur noch 
    public bool createBuilding(int index)
    {
        IINFO i = buildings[index].GetComponent<IINFO>();
        
        if(Gold - i.costInfo.Gold > 0 &&
            Stone - i.costInfo.Stone > 0 &&
            Wood - i.costInfo.Wood > 0 &&
            Food - i.costInfo.Food > 0)
        {
            Gold -= i.costInfo.Gold;
            Stone -= i.costInfo.Stone;
            Wood -= i.costInfo.Wood;
            Food -= i.costInfo.Food;

            return true;
        }
        else
        {
            //nicht genug ressourcen sound apspielen
            return false;
        }
    }


    public override void setBuilding(StadtCentrum sc)
    {
        throw new System.NotImplementedException();
    }

    public override void onDestoryBuilding(StadtCentrum sc)
    {
        throw new System.NotImplementedException();
    }
}
