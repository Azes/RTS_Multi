using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class BuildSystem : MonoBehaviour
{
    public bool onBuild;
    PlayerCursor cursor;

    public NavMeshSurface nms;
    public GameObject[] placeHolder;
    public GameObject[] Buildings;
    public float drawSpeed;
    Rigidbody rb;
    int indexbuffer;

    Vector3 stPos;


    void Start()
    {
        cursor = GetComponent<PlayerCursor>();

        GameObject n = GameObject.FindWithTag("NavMesh");//new fine navmesh in scene
        nms = n.GetComponents<NavMeshSurface>()[0];//new
    }


    private void Update()
    {
        
    
        if (onBuild)
        {
            if (PlayerCursor.DoubleClick(1))
            {
                var i = Buildings[indexbuffer].GetComponent<BuildingSample>().build;
                cursor.sc.Gold += i.costInfo.Gold;
                cursor.sc.Stone += i.costInfo.Stone;
                cursor.sc.Wood += i.costInfo.Wood;
                cursor.sc.Food += i.costInfo.Food;
                placeHolder[indexbuffer].transform.position = stPos;
                placeHolder[indexbuffer].gameObject.SetActive(false);
                rb = null;
                onBuild = false;
            }else if (Input.GetMouseButtonDown(1))
            {
                if(Physics.Raycast(cursor._c.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
                {
                    Vector3 pos = new Vector3(hit.point.x, 0, hit.point.z);
                    BoxCollider boxCollider = placeHolder[indexbuffer].GetComponent<BoxCollider>();

                    if(boxCollider != null)
                    {
                        Vector3 boxCastCenter = pos + rb.transform.forward * (boxCollider.size.y / 2);
                        Vector3 boxCastSize = boxCollider.size / 2;

                        if(Physics.BoxCast(boxCastCenter, boxCastSize, rb.transform.forward, out RaycastHit boxCastHit,
                            placeHolder[indexbuffer].transform.rotation))
                        {
                            if(boxCollider.GetType() == typeof(TerrainCollider))
                            {
                                rb.position = pos;
                            }
                        }
                        else rb.position = pos;

                    }

                }
            }


            if (Input.GetMouseButtonDown (0))
            {
                var i = Instantiate(Buildings[indexbuffer], placeHolder[indexbuffer].transform.position, Buildings[indexbuffer].transform.rotation);//new rotation
                var b = i.GetComponent<BuildingSample>();
                b.sp = cursor.sc;
                b.nms = nms;
                b.mhb.buildingInfoObject=cursor.mui.buildingInfoObject;
                b.mhb.createCosteInfoObject = cursor.mui.costeObject;
                b.mainUI = cursor.mui;//new 
                b.mhb.abbrechenUI.GetComponent<Canvas>().worldCamera = cursor.GetComponent<Camera>();//new
                placeHolder[indexbuffer].transform.position = stPos;
                placeHolder[indexbuffer].gameObject.SetActive(false);
                rb = null;
                onBuild = false;


            }
        }

    }

    private void FixedUpdate()
    {
        if (onBuild)
        {
            rb.velocity = Vector3.zero;


            if (Physics.Raycast(cursor._c.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
            {
                Vector3 pos = new Vector3(hit.point.x, 0, hit.point.z);

                if (Vector3.Distance(pos, rb.position) > .5f)
                {
                    rb.AddForce((pos - rb.position).normalized * drawSpeed, ForceMode.VelocityChange);
                }
            }
        }

        
    }

    //new wir nutzen diese methode jetzt direkt da wir sie den buttens hinzufügen können
    public void startToBuild(int index)
    {
        if (cursor.sc.createBuilding(index))
        {

            onBuild = true;
            indexbuffer = index;
            placeHolder[index].gameObject.SetActive(true);
            stPos = placeHolder[index].transform.position;
            rb = placeHolder[index].GetComponent<Rigidbody>();
        }
    }
}
