using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class MainUI : MonoBehaviour
{
    //new wir verzichten auf die static variabeln und methoden da wir eh �berall eine instanz haben
    public bool inUI;  
    public bool inWorldUI; 
    public InUICheck[]  inUIChecks;//wir pr�fen jetzt die UIcheck elemente direkt
                                   //in der MainUI dann giebt es keine probleme mehr damit
    //

    public GameObject costeObject, buildingInfoObject;

    public PlayerCursor cursor;
    public RectTransform UI_Frame;
    public float step;
    float bbuff;

    public TextMeshProUGUI gold_value, stone_value, wood_value, food_value, pop_value, maxpop_value;

    public GameObject rathausUI;
    public IBuilding build;

    //new network tut
    public TextMeshProUGUI sz_procescount, sz_jobless, sz_buildinghealth;
    public ProcessElement sz_process;


    #region

    //Human menu
    //ui gameobject f�r einheiten
    public GameObject HumanSelect;
    //die verschiednenen einheiten icons (new Heiler)
    public GameObject villager, knight, bowman, heiler;
    //tmp f�r die anzahl der einheiten  (new Heiler)
    public TextMeshProUGUI v_value, kn_value, b_value, h_value;
    //gameobjecte f�r die einzelinfo und die gruppen info
    public GameObject SingleInfo, GroupInfo;

    //die einheiten liste the heart
    public List<IHuman> humans = new List<IHuman>();

    // Singel infos 
    //der index f�r die jeweilige einheiten info
    //[0] Villager [1] Knight [2] Bowman
    int singleIndex = -1;
    //tpm f�r den namen der einheit
    public TextMeshProUGUI singleInfo_Name;
    //array f�r die werte der jeweiligen kategorie
    public TextMeshProUGUI[] singleInfo_values;
    //eine list diee mit dem listen index der einheit
    //mit einem array das sagt welche infos activiert werden 
    List<int[]> activateInfosList = new List<int[]> {
        new int[] {0, 2, 3, 4, 6},//Villager
        new int[] {0, 1, 2, 3, 4},//Knight
        new int[] {0, 1, 2, 3, 4, 5},//Bowman
        new int[] {0, 2, 5, 7}//Healer            new
        };
    //["health"]      = [0]
    //["armor"]       = [1]
    //["speed"]       = [2]
    //["damage"]      = [3]
    //["dps"]         = [4]
    //["range"]       = [5]
    //["carryweight"] = [6]
    //["healing"]     = [7] new
   
    //eine hilfs klasse die das info update triggert 
    public class Trigger
    {
        private bool t = true;

        public bool Is()
        {
            bool i = t;
            t = false;
            return i;
        }

        public void reset()
        {
            t = true;
        }
    }

    Trigger singleTigger = new Trigger();

   
    Trigger singleOrGroup = new Trigger();


    //ein buffer um zusehen ob die anzahl der einheiten sich ver�ndert hat
    int humanCountBuffer;
    //ein buffer umzusehen ob die lisst sich ge�ndert hat
    IHuman hbuff;

    //ein hilfs buffer um zwischen den verschiedenen infos zuwelchseln von sing to group
    public bool infoToggle;
    #endregion

    #region
    ///Group

    //das gruppen parent gameobject also der container vom scrollrect
    public Transform GroupContent;
    //das prefabs von dem EinheitBar Ui Element
    public GameObject Einheit_bar;
    //die sortier buttons
    public ModeButton sortingButton, orderButton;
    //ob die liste sortiert ist oder nicht
    public bool sorted = false;
    //die reinfolge wie die eiheiten gelisted sind 
    //[0] garnicht
    //[1] mit amwenigsten leben zuerst
    //[2] mit amwenigsten leben zuletzt
    public int order = 0;


    #endregion


    // Update is called once per frame
    void Update()
    {
        if(cursor == null) return;
        //new static weg und UI check sind jetzzt hier (network tut)
        updateInUI();
        inUI = allUICheckFalse();
        
        if (inUI) inWorldUI = false;
        //

        bool onb = cursor.buildSystem.onBuild;
        if (onb) inUI = false;

        BlendUI();

        if (build != null)
        {
            if (build is StadtCentrum)
            {
                if (!rathausUI.activeInHierarchy) rathausUI.SetActive(true);
            }

        }
        else
        {
            if (rathausUI.activeInHierarchy) rathausUI.SetActive(false);
        }

        if (onb)
        {
            build = null;
            return;
        }



        // erst kommen die einheit simbole und updatet die anzahl mit humanValues
        if (humans.Count > 0)
            {
                //wir buffern ob sich die anzahl der einheiten ge�ndert hat
                bool isHchange = isHumanListChange();

                if (!HumanSelect.activeInHierarchy) HumanSelect.SetActive(true);
                humanValues(isHchange);

                //wir �berpr�fen mit checkIsSingel ob eine oder mehrere einheiten ausgew�hlt sind
                bool s = checkIsSingel();

                if (singleOrGroup.Is())
                {
                    SingleInfo.SetActive(s);
                    GroupInfo.SetActive(!s);
                }


                //wenn der s(ingel) boolean fals ist haben wir mehrere einheiten
                //und aktivieren die group methoden
                if (!s)
                {
                    sorted = (sortingButton.mode == 1 ? true : false);
                    order = orderButton.mode;

                    if (sortingButton.isChange() || orderButton.isChange())
                        sortedList();


                    isListValueChange(isHchange);
                }
                else singleInformationFields();

            }
            else if (HumanSelect.activeInHierarchy) HumanSelect.SetActive(false);
      
    }

    //eine methode damit wir nicht durchg�ng updaten sondern nur wenn wirs m�ssen
    bool isHumanListChange()
    {
        if (humans.Count != humanCountBuffer || humans[0] != hbuff)
        {
            hbuff = humans[0];
            humanCountBuffer = humans.Count;
            return true;
        }

        return false;
    }

    void humanValues(bool it)
    {
        if (it)
        {
            //ints die unsere einheiten z�hlen
            int v = 0;//villager
            int w = 0;//knght
            int b = 0;//bowman
            int h = 0;//heiler   new
            //wir z�hlen die jeweilige einheit hoch
            for (int i = 0; i < humans.Count; i++)
            {
                if (humans[i] is Villager) v++;
                else if (humans[i] is Warrior) w++;
                else if (humans[i] is Bowman) b++;
                else if (humans[i] is Heiler) h++;
            }

            //wirr aktivieren nur die einheiten die auch asugew�hlt sind
            if (v >= 1)
            {
                if (!villager.activeInHierarchy) villager.SetActive(true);
                v_value.text = v.ToString();
            }
            else if (villager.activeInHierarchy) villager.SetActive(false);

            if (w >= 1)
            {
                if (!knight.activeInHierarchy) knight.SetActive(true);
                kn_value.text = w.ToString();
            }
            else if (knight.activeInHierarchy) knight.SetActive(false);

            if (b >= 1)
            {
                if (!bowman.activeInHierarchy) bowman.SetActive(true);
                b_value.text = b.ToString();
            }
            else if (bowman.activeInHierarchy) bowman.SetActive(false);

            
            if (h >= 1)
            {
                if (!heiler.activeInHierarchy) heiler.SetActive(true);
                h_value.text = h.ToString();
            }
            else if (heiler.activeInHierarchy) heiler.SetActive(false);

        }
    }

    bool checkIsSingel()
    {
        //ist die einheiten anzahl gr��er als 1 werden die singelField attribute zur�ck gesetzt
        if (humans.Count > 1)
        {
            singleIndex = -1;
            singleTigger.reset();
            singleOrGroup.reset();
            return false;
        }
        else
        {
            //wir �berprufen hier damit wir nicht unn�tig updaten
            if (humans.Count > 0)
            {
                //�berpr�ft welcher typ einheit in unserer IHuman list ist und setzen den index
                if (humans[0] is Villager) singleIndex = 0;
                else if (humans[0] is Warrior) singleIndex = 1;
                else if (humans[0] is Bowman) singleIndex = 2;
                else if (humans[0] is Heiler) singleIndex = 3;

                singleTigger.reset();
            }

            
            singleOrGroup.reset();
            
            // giebt treu zur�ck wenn wir nur eine einheit ausgew�hlt haben
            return (humans.Count == 1);
        }
    }
   
    void singleInformationFields()
    {
        //wir nutzen den singelTrigger um einmal die information zu updaten und nicht jeden frame
        if (singleTigger.Is())
        {
            for (int j = 0; j < singleInfo_values.Length; j++)
            {
                singleInfo_values[j].text = " ";
                singleInfo_values[j].transform.parent.gameObject.SetActive(false);
            }
            //setzt den header namen
            if (singleIndex == 0) singleInfo_Name.text = "Villager";
            else if (singleIndex == 1) singleInfo_Name.text = "Knight";
            else if (singleIndex == 2) singleInfo_Name.text = "Bowman";
            else if (singleIndex == 3) singleInfo_Name.text = "Heiler";

            //aktiviert nur die spezifischen feld f�r die jeweilige einheit
            for (int k = 0; k < activateInfosList[singleIndex].Length; k++)
            {
                for (int j = 0; j < singleInfo_values.Length; j++)
                {
                    if (j == activateInfosList[singleIndex][k])
                    {
                        singleInfo_values[j].transform.parent.gameObject.SetActive(true);
                        break;
                    }
                }
            }
        }
        //wir bufferm die einheit erleichte die schreibweise
        var h = humans[0];

        //updatet die singelInfos f�r die einheiten
        if (singleIndex == 0)
        {
            
            //wir haben die Health anstatt die currentHealth
            //angezeigt nun wird die UI richtig angezeigt
            singleInfo_values[0].text = h.currentHealth.ToString();
            singleInfo_values[2].text = h.Speed.ToString();
            singleInfo_values[3].text = h.Damage.ToString();
            singleInfo_values[4].text = (h.Damage * h.attackSpeed).ToString();
            //spizele nur einheit spezifische variabelm m�ssen einzeln  gecasted werden 
            //um drauf zuzugreifen
            if (h is Villager)
            {
                Villager v = (Villager)h;
                singleInfo_values[6].text = v.carryweight.ToString();
            }

        }
        else if (singleIndex == 1)
        {
            singleInfo_values[0].text = h.currentHealth.ToString();
            singleInfo_values[1].text = h.Armor.ToString();
            singleInfo_values[2].text = h.Speed.ToString();
            singleInfo_values[3].text = h.Damage.ToString();
            singleInfo_values[4].text = (h.Damage * h.attackSpeed).ToString();
        }
        else if (singleIndex == 2)
        {
            singleInfo_values[0].text = h.currentHealth.ToString();
            singleInfo_values[1].text = h.Armor.ToString();
            singleInfo_values[2].text = h.Speed.ToString();
            singleInfo_values[3].text = h.Damage.ToString();
            singleInfo_values[4].text = (h.Damage * h.attackSpeed).ToString();

            if (h is Bowman)
            {
                Bowman b = (Bowman)h;
                singleInfo_values[5].text = b.Range.ToString();
            }
        }
        else if (singleIndex == 3)
        {
            singleInfo_values[0].text = h.currentHealth.ToString();
            singleInfo_values[2].text = h.Speed.ToString();

            if (h is Heiler)
            {
                Heiler b = (Heiler)h;
                singleInfo_values[5].text = b.range.ToString();
                singleInfo_values[7].text = b.healPower.ToString();
            }
        }
    }

  
   
    void isListValueChange(bool it)
    {
        if (it)
        {


            //new wir f�gen die child objecte (EinheitBar) nur dann hinzu wenn sich nicht schon existiert 
            //anstatt alle zul�schen und neu zu erzeugen das spaart leistung und funktioniert schneller
            for (int i = GroupContent.childCount - 1; i >= 0; i--)
            {
                Transform child = GroupContent.GetChild(i);
                IHuman ih = child.GetComponent<EinheitBar>().ih;

                if (!humans.Contains(ih))
                {
                    Destroy(child.gameObject);
                }
            }

            foreach (IHuman ih in humans)
            {
                bool found = false;

                for (int i = 0; i < GroupContent.childCount; i++)
                {
                    if (GroupContent.GetChild(i).GetComponent<EinheitBar>().ih == ih)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    var ho = Instantiate(Einheit_bar);
                    ho.GetComponent<EinheitBar>().ih = ih;
                    ho.GetComponent<EinheitBar>().mainUI = this;
                    ho.transform.SetParent(GroupContent);
                }
            }

            sortedList();
        }


    }
   
    //new Update wir verwenden einen neuen sortier Algorithmus da der alte fehler hafte ergebnisse hatte
    void sortedList()
    {
        List<Transform> children = new List<Transform>();

        
        foreach (Transform child in GroupContent)
        {
            children.Add(child);
        }

        if (!sorted)
        {
            if (order == 1)
            {
                children.Sort((a, b) =>
                {
                    return a.GetComponent<EinheitBar>().value.CompareTo(b.GetComponent<EinheitBar>().value);
                });
            }
            else if( order == 2)
            {
                children.Sort((a, b) =>
                {
                    return b.GetComponent<EinheitBar>().value.CompareTo(a.GetComponent<EinheitBar>().value);
                });
            }

            if (order == 1 || order == 2)
            {
                for (int i = 0; i < children.Count; i++)
                    children[i].SetSiblingIndex(i);
            }
            else
            {
                                     
                //wir gehen die child objecte vom letzten bis zum ersten durch
                //und setzen ihren index auf die reihenfolge wie wir sie hinzugef�gt haben
                for (int i = 0; i < humans.Count; i++)
                {
                    for (int j = children.Count -1; j >= 0; j--)
                    {
                        if (children[j].GetComponent<EinheitBar>().ih == humans[i])
                        {
                            children[j].SetSiblingIndex(i);
                            break;
                        }
                    }
                }
            }
        }
        else
        {
            //wir benutzen jetzt die Linq  OrderBy Methode diese hat ein
            //besseres sortier ergebnisse und ist leichter zu erweitern ist
            if (order == 0)
            {
                // Erstelle eine Dictionary, das die Reihenfolge der IHuman-Objekte in humans speichert
                Dictionary<IHuman, float> humanOrder = new Dictionary<IHuman, float>();
                for (int i = 0; i < humans.Count; i++)
                {
                    humanOrder[humans[i]] = i;
                }

                children = children.OrderBy(child =>
                {
                    IHuman script = child.GetComponent<EinheitBar>().ih.GetComponent<IHuman>();
                    if (script is Heiler)
                    {
                        return 1f;
                    }
                    else if (script is Villager)
                    {
                        return 2f;
                    }
                    else if (script is Warrior)
                    {
                        return 3f;
                    }
                    else if (script is Bowman)
                    {
                        return 4f;
                    }
                    return 5f;
                }).ThenBy(child =>
                {
                    return humanOrder[child.GetComponent<EinheitBar>().ih];
                }).ToList();

                for (int i = 0; i < children.Count; i++)
                    children[i].SetSiblingIndex(i);

            }
            else if(order == 1)
            {
                //bei einer sortier order verwenden wir jetzt auch Linq
                children = children.OrderBy(child =>
                {
                    IHuman script = child.GetComponent<EinheitBar>().ih.GetComponent<IHuman>();
                    if (script is Heiler)
                    {
                        return 1f;
                    }
                    else if (script is Villager)
                    {
                        return 2f;
                    }
                    else if (script is Warrior)
                    {
                        return 3f;
                    }
                    else if (script is Bowman)
                    {
                        return 4f;
                    }
                    return 5f;
                }).ThenBy(child =>
                {
                    return child.GetComponent<EinheitBar>().value;
                }, Comparer<float>.Create((x, y) => x.CompareTo(y))).ToList();

                for (int i = 0; i < children.Count; i++)
                    children[i].SetSiblingIndex(i);
            }
            else if(order == 2)
            {
                children = children.OrderBy(child =>
                {
                    IHuman script = child.GetComponent<EinheitBar>().ih.GetComponent<IHuman>();
                    if (script is Heiler)
                    {
                        return 1f;
                    }
                    else if (script is Villager)
                    {
                        return 2f;
                    }
                    else if (script is Warrior)
                    {
                        return 3f;
                    }
                    else if (script is Bowman)
                    {
                        return 4f;
                    }
                    return 5f;
                }).ThenByDescending(child =>
                {
                    return child.GetComponent<EinheitBar>().value;
                }).ToList();

                for (int i = 0; i < children.Count; i++)
                    children[i].SetSiblingIndex(i);
            }
            
        }
    }



    //methode um alle Einheiten einer klasse auszuw�hlen
    public void seletcGroupWithIcon(int index)
    {
        List<IHuman> unsel = new List<IHuman>();

        for (int i = 0;i < humans.Count;i++)
        {
            if(index == 0)
            {
                if (humans[i] is Villager) continue;
                else
                {
                    unsel.Add(humans[i]);
                }
            }
            else if (index == 1)
            {
                if (humans[i] is Warrior) continue;
                else
                {
                    unsel.Add(humans[i]);
                }
            }
            else if (index == 2)
            {
                if (humans[i] is Bowman) continue;
                else
                {
                    unsel.Add(humans[i]);
                }
            }
            else if (index == 3)
            {
                if (humans[i] is Heiler) continue;
                else
                {
                    unsel.Add(humans[i]);
                }
            }
        }

        for (int i = 0;i<unsel.Count;i++)
        {
            if (humans.Contains(unsel[i])) humans.Remove(unsel[i]);
            if (cursor.selectedObjects.Contains(unsel[i].gameObject))
                cursor.removeSelcetion(unsel[i].gameObject);
        }
    }

    //methode um eine eizelnde einheit �ber seinen lebensbalken zubekommen
    public void selectSingleHuman(IHuman h)
    {
        humans.Clear();
        humans.Add(h);
        cursor.singleSelectObject(h.gameObject);   
    }

    
    //methode die die UI ein und ausblendet
    public void BlendUI()
    {
        var frame =  UI_Frame.anchoredPosition;

        //new ui kommt nur wenn auch benutzt
        if (humans.Count > 0 || build != null)
        {
            
            if (frame.y < 210)
            {
                bbuff += Time.deltaTime;
                
                if (bbuff > .01f)
                {
                    bbuff = 0;
                    frame += new Vector2(0, step);
                    if (frame.y > 210) frame = new Vector2(frame.x, 210);
                }
            }
            else
            {
                frame = new Vector2(frame.x, 210);
                bbuff = 0;
            }
        }
        else
        {
            if (humans.Count != 0) return;

            if (frame.y > -50)
            {
                bbuff += Time.deltaTime;

                if (bbuff > .01f)
                {
                    bbuff = 0;
                    frame -= new Vector2(0, step);
                    if (frame.y < -50) frame = new Vector2(frame.x, -50);
                }
            }
            else
            {
                bbuff = 0;
                frame = new Vector2(frame.x, -50);
            }
        }

        UI_Frame.anchoredPosition = frame;
    }


    //new  (network tut)

    private void updateInUI()
    {
        for (int i = 0; i < inUIChecks.Length; i++)
        {
            if (!inUIChecks[i].gameObject.activeInHierarchy) continue;

            Vector3[] cc = new Vector3[4];
            inUIChecks[i].rect.GetWorldCorners(cc);
            var sp = Input.mousePosition;

            if (sp.x >= cc[0].x && sp.x <= cc[2].x &&
                sp.y >= cc[0].y && sp.y <= cc[1].y) inUIChecks[i].inUI = true;
            else inUIChecks[i].inUI = false;

        }
        
    }

    bool allUICheckFalse()
    {
        for (int i = 0; i < inUIChecks.Length; i++)
            if (inUIChecks[i].inUI) return true;
        return false;
    }
}
