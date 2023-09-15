using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.AI.Navigation;

public class BuildingSample : MonoBehaviour
{
    public MainUI mainUI;
    public StadtCentrum sp;
    public NavMeshSurface nms;
    
//new    //public WorldCanvasToCamera wtc; useless mit neuer cam
    public MouseHoverBuilding mhb;

    public List<Villager> villagerList = new List<Villager>();

    public IBuilding build;
    public GameObject destroyparticel;
    public float ingessProcessTimer;
    public float processTime;

    public GameObject[] buildSteps;
    public Slider abbrechen;

    float processValue;
    bool trigger = true;

    private void Start()
    {
        processTime = build.costInfo.ProcessTime;
        //new
        mhb.mainUI = mainUI;
    }

    private void Update()
    {
        if(villagerList.Count > 0)
        {
            if(trigger)
            {
                StartCoroutine(Process());
                trigger = false;
            }

            for (int i = 0; i < villagerList.Count; i++)
            {
                if (!villagerList[i].isWork) villagerList.RemoveAt(i);
            }

            setStep();
        }
    }


    void setStep()
    {
        if (processValue >= (processTime / 2) - (processTime / 10)
           && processValue < processTime - ((processTime / 10) * 2))
        {
            if (!buildSteps[1].activeInHierarchy)
            {
                buildSteps[0].SetActive(false);
                buildSteps[1].SetActive(true);
            }
        }
        else if (processValue >= processTime - ((processTime / 10) * 2))
        {
            if (!buildSteps[2].activeInHierarchy)
            {
                buildSteps[1].SetActive(false);
                buildSteps[2].SetActive(true);
            }
        }
    }

    IEnumerator Process()
    {
        while(villagerList.Count > 0)
        {
            yield return new WaitForSeconds(ingessProcessTimer);

            processValue += .1f * villagerList.Count;

            if (processValue > processTime)
            {
                var i = Instantiate(build.gameObject, transform.position, transform.rotation);
                nms.BuildNavMesh();
                i.GetComponent<IBuilding>().setBuilding(sp);
                Destroy(gameObject);
                break;
            }
            
        }

        trigger = true;
    }

    public void destoryBuilding()
    {
        if(abbrechen.value == 1)
        {
            mainUI.inWorldUI = false;//new  (network tut)
            if (GetComponent<BoxCollider>())
            {
                var b = GetComponent<BoxCollider>();
                var p = Instantiate(destroyparticel, transform.position, Quaternion.Euler(-90, transform.rotation.y, 0));
                p.GetComponent<DestroyParticel>().startPart(b.size.x, b.size.z);

            }

            sp.Gold += build.costInfo.Gold;
            sp.Stone += build.costInfo.Stone;
            sp.Wood += build.costInfo.Wood;
            sp.Food += build.costInfo.Food;

            Destroy(gameObject);
        }
    }
}
