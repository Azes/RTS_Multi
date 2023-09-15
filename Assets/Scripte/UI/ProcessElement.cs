using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProcessElement : MonoBehaviour
{
    //waitObject prefabs
    public GameObject ipObjectprefab;
    //Sichtbare Objekt der ProcessBar
    public GameObject viewObject;
    //Process Einhet Icon und FillImage (ladebalken)
    public Image inProcessIcon, fillbar;
    //layoutgroup um die ansicht anzupassen
    public HorizontalLayoutGroup hgroup;
    //parent object der WaitObjecte
    public Transform waitGroup;
    //process anzahl text der jeweiligen UI
    public TextMeshProUGUI processCountText;
    //warte liste zu erstellender einheiten
    public List<IHuman> waitHumans = new List<IHuman>();
    //die zu erstellende einheit
    public IHuman inpHuman;
    //process abfolgen 
    public bool startProcess,isProcess,endProcess;
    //die maximale anzahl an processen die man starten kann
    public int processLimet;
    //fortschrittsblagen value
    float processValue;
    //time counter
    float t;
    
    
    void Update()
    {
        if (waitHumans.Count > 0)
        {
            if (processCountText != null)
                processCountText.text = waitHumans.Count.ToString();

            if (!viewObject.activeInHierarchy) viewObject.SetActive(true);
        }
        else if (viewObject.activeInHierarchy)
        {
            if (processCountText != null)
                processCountText.text = "0";

            viewObject.SetActive(false);
        }


        ifWaitObjectChange();
        onProcess();
        ProcesValue();
    }

    void ProcesValue()
    {
        //aktiviert sich wenn der process startet
        if (startProcess || isProcess)
        {
            isProcess = true;
            startProcess = false;
            t += Time.deltaTime;

            //berechnet die fillbar image value
            processValue = Mathf.InverseLerp(0, inpHuman.costInfo.ProcessTime, t);
            fillbar.fillAmount = processValue;

            //setzt das Process Icon an die procesValue position
            inProcessIcon.rectTransform.anchoredPosition = new Vector2(Mathf.Lerp(0, 370, processValue), 0);

            //scalliert die horizontal layout group damit die icons richtig angereiht sind
            float i = Mathf.InverseLerp(0, processLimet, waitGroup.childCount);
            hgroup.padding.right = Mathf.RoundToInt(Mathf.Lerp(0, 300f, 1f - i));

            //wenn der process beendet wird setzten wir den endProcess auf zu um spawnen zukönnen wenn möglich
            if(t >= inpHuman.costInfo.ProcessTime)
            {
                t = 0;
                processValue = 0;
                isProcess = false;
                endProcess = true;
            }
        }
    }

    //methode die den process automatisch startet wenn nicht schon aktive
    void onProcess()
    {
        if(waitHumans.Count > 0 && !endProcess)
        {
            startProcess = true;
            inpHuman = waitHumans[0];
            
        }
    }

    //methode zum spawnen der jeweiligen einheit wenn der process beendet ist und wenn noch genug platz ist
    public GameObject spawnHuman(Transform spawn, bool popSpace)
    {
        if (endProcess && popSpace)
        {
            endProcess = false;
            t = 0;
            processValue = 0;
            
            waitHumans.RemoveAt(0);
            Destroy(waitGroup.GetChild(0).gameObject);
            GameObject spoj = Instantiate(inpHuman.gameObject, spawn.position, Quaternion.identity);

            return spoj;
        }

        return null;
    }

    //methode die überprüft ob ein waitObject angeklickt wurde wodurch dieser process gelöscht wird
    void ifWaitObjectChange()
    {
        for(int i = 0; i < waitGroup.childCount; i++)
        {
            var ii = waitGroup.GetChild(i).GetComponent<waitObject>();
            
            if (ii.change)
            {
                if(waitGroup.childCount == 1)
                {
                    t = 0;
                    processValue = 0;
                    isProcess = false;
                }

                waitHumans.Remove(ii.h);
                Destroy(ii.gameObject);
            }
        }
    }

    //methode zum benden aller processe
    public void quitProcess()
    {
       
        for(int i = waitGroup.childCount -1;i >= 0; i--)
        {
            Destroy(waitGroup.GetChild(i).gameObject);
        }

        waitHumans.Clear(); 
        inpHuman = null;
        isProcess = false;
        t = 0;
        processValue = 0;
    }

    //methode zum hinzufügen neuer processe
    public void addHumanToProcess(IHuman h)
    {
        if (waitHumans.Count < processLimet)
        {
            waitHumans.Add(h);

            var ii = Instantiate(ipObjectprefab);
            ii.GetComponent<waitObject>().h = h;
            ii.GetComponent<waitObject>().icon.sprite = h.costInfo.icon;
            ii.transform.SetParent(waitGroup);
        }
    }
}
