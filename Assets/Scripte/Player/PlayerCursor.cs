using Fusion;//new (network tut)
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

//new von monobehaviour zu network 
public class PlayerCursor : NetworkBehaviour
{
    public StadtCentrum sc;
    public Camera _c;
    public BuildSystem buildSystem;



    public MainUI mui;
    public Image selectionRectImage;

    private RectTransform rectTransform;
    public RectTransform canvas;

    
    public List<GameObject> selectedObjects = new List<GameObject>();


    public IBuilding build;
    
    
    bool isDrag, onSelecting;
    Vector3 lsmp = Vector3.zero;
    Vector2 initialMousePos;

    List<GameObject> objectsInCameraView = new List<GameObject>();
    Vector3[] cc = new Vector3[4];
    RaycastHit hit;

    //new (network tut) wir benutzen jetzt die spawn methode von network
    public override void Spawned()
    {
        //prüft ob der gespawnte Player der ist dessen Game instanz offen ist
        if (Object.HasInputAuthority)
        {
            //hollt uns unser MainUI und übergiebt die variabeln
            mui = GetComponentInChildren<MainUI>();
            mui.cursor = this;

            _c = GetComponent<Camera>();
            buildSystem = GetComponent<BuildSystem>();

            //prüfen ob wir der Host oder der client sind 
            //Host ist immer Player1 und Client Player2
            if (Runner.IsServer)
            {
                //wir suchen in dem Level unser Rathaus und weisen es zu
                //gleich zeitig deaktivieren wir das gegnerische Rathaus
                var g = GameObject.FindGameObjectWithTag("Rathaus1");
                var g2 = GameObject.FindGameObjectWithTag("Rathaus2");

                Destroy(g2.GetComponent<StadtCentrum>());

                sc = g.GetComponent<StadtCentrum>();
                sc.processCount = mui.sz_procescount;
                sc.jobless = mui.sz_jobless;
                sc.buildhealth = mui.sz_buildinghealth;
                sc.pe = mui.sz_process;

                //new 
                sc.mui = mui;//vergessen

                gameObject.tag = "Player1";
                
                Debug.Log("Stadtzentrum = " + (sc != null ? "Gefunden" : "nicht gefunden"));
            }
            else if (Runner.IsClient)
            {
                //wenn der Runner ein Client ist deaktivier wir natürlich das Rathaus von player1
                var g = GameObject.FindGameObjectWithTag("Rathaus2");
                var g2 = GameObject.FindGameObjectWithTag("Rathaus1");

                sc = g.GetComponent<StadtCentrum>();
                Destroy(g2.GetComponent<StadtCentrum>());

                gameObject.tag = "Player2";

                Debug.Log("Stadtzentrum = " + (sc != null ? "Gefunden" : "nicht gefunden"));
            }

            lsmp = Input.mousePosition;

            

            //wir müssen einmal das SecetionRect object aktivieren um auf da Image(selectionrect) zugreifen zukönnen
            mui.transform.GetChild(0).gameObject.SetActive(true);
            selectionRectImage = mui.transform.GetChild(0).GetComponent<Image>();
            rectTransform = selectionRectImage.GetComponent<RectTransform>();
            mui.transform.GetChild(0).gameObject.SetActive(false);
            //dann schalten wir es wieder aus


        }
        else
        {
           //wenn ein Player gespawnt wird der nicht die Input rechte hat also nicht der Player ist dessen Game instanz offen ist
           //löschen wir vom anderen Playerobject alle scripte da sie nicht gebraucht werden und sonst zu problemen führen würden

            //wenn wir der Host sind setzen wir den zweiten erstellten player tg auf Player2
            //und wenn wir der client sind ist das zweite erstelte object der Player1
            if (Runner.IsServer)
                gameObject.tag = "Player2";
            else if (Runner.IsClient)
                gameObject.tag = "Player1";

            for (int i = 0; i < transform.childCount; i++)
                Destroy(transform.GetChild(i).gameObject);

            Destroy(GetComponent<UniversalAdditionalCameraData>());
            Destroy(GetComponent<Camera>());
            Destroy(GetComponent<BuildSystem>());
            Destroy(GetComponent<SimpelCameraMovement>());
            Destroy(GetComponent<AudioListener>());
            Destroy(GetComponent<PlayerCursor>());
            
        }

        // dann schlten wir den player aus weil er erst vom handler aktiviert wird 
        transform.SetSiblingIndex(0);
        gameObject.SetActive(false);
    }
    private void Start()
    {
        
    }
    //

    //new bug fix damit die drag methode nicht ausgeführt wird wenn world ui sich ändert
    bool waseWorldUI;

    private void Update()
    {
        //shortcut zum stadtzentrum
        if (Input.GetKeyDown(KeyCode.E) && mui.build == null) mui.build = sc;
        else if (Input.GetKeyDown(KeyCode.E) && mui.build != sc)mui.build = sc;
        else if (Input.GetKeyDown(KeyCode.E) && mui.build == sc) mui.build = null;

        
        if (mui.inUI || mui.inWorldUI || buildSystem.onBuild )
        {
            if (mui.inWorldUI) waseWorldUI = true;
            return;
        }

        if (waseWorldUI)
        {
            if (Input.GetMouseButtonUp(0)) waseWorldUI = false;
            return;
        }
        


        bool dsel = Input.GetKey(KeyCode.LeftShift);

        
        if (Input.GetMouseButtonDown(1)) singelSelect(dsel, true);
        

        


        if (Input.GetMouseButtonDown(0))
        {
            initialMousePos = Input.mousePosition;
            initialMousePos.x = initialMousePos.x - (Screen.width / 2);
            initialMousePos.y = initialMousePos.y - (Screen.height / 2);

            rectTransform.anchoredPosition = initialMousePos;
            rectTransform.sizeDelta = Vector2.zero;
            lsmp = Input.mousePosition;
        }


        if (isMouseDrag(2))
            {
                if (!selectionRectImage.gameObject.activeInHierarchy)
                    selectionRectImage.gameObject.SetActive(true);

                Vector2 currentMousePos = Input.mousePosition;
                currentMousePos.x = currentMousePos.x - (Screen.width / 2);
                currentMousePos.y = currentMousePos.y - (Screen.height / 2);
                Vector2 size = currentMousePos - initialMousePos;

                // Ensure width and height are positive for proper scaling
                size.x = Mathf.Abs(size.x);
                size.y = Mathf.Abs(size.y);

                rectTransform.sizeDelta = size;

                Vector2 center = (initialMousePos + currentMousePos) * 0.5f;
                rectTransform.anchoredPosition = center;

                if (onSelecting) dsel = false;

                GetObjectsInSelect(dsel);
            }
            else
            {
                if (selectionRectImage.gameObject.activeInHierarchy)
                    selectionRectImage.gameObject.SetActive(false);

                if (Input.GetMouseButtonDown(0)) singelSelect(dsel);
            }
       
    }


    public bool isMouseDrag(float min = 0f)
    {
        bool isMouseDown;
        bool currentMouseDown = Input.GetMouseButton(0);
        Vector3 currentMousePosition = Input.mousePosition;

        bool positionChanged = Vector3.Distance(currentMousePosition, lsmp) >= min;


        if (currentMouseDown)
        {
            isMouseDown = true;
            lsmp = currentMousePosition;

            if (positionChanged) { isDrag = true; }

        }
        else
        {
            isDrag = false;
            isMouseDown = false;
        }

        return isMouseDown && isDrag;
    }

    //new (movement tut 2)
    bool setTarget;Vector3 hitpos;
    void singelSelect(bool msel = false, bool isleft = false)
    {

        Vector3 ip = Input.mousePosition;

        Ray r = _c.ScreenPointToRay(new Vector3(ip.x, ip.y, _c.nearClipPlane));

        if (Physics.Raycast(r, out hit, Mathf.Infinity))
        {
            setTarget = true;
            hitpos = hit.point;

            if (hit.collider.GetComponent<IHuman>())
            {
                //new bug fix

                build = null;
                mui.build = null;

                //

                onSelecting = true;
               
                GameObject ho = hit.collider.gameObject;

                if (msel)
                {
                    
                    if (!selectedObjects.Contains(ho))
                    {
                      
                        ho.GetComponent<IHuman>().isSelectet = true;
                        selectedObjects.Add(ho);

                        mui.humans.Add(ho.GetComponent<IHuman>());

                    }
                    else
                    {
                        
                        ho.GetComponent<IHuman>().isSelectet = false;
                        selectedObjects.Remove(ho);

                        mui.humans.Remove(ho.GetComponent<IHuman>());
                    }
                }
                else
                {

                    for (int i = 0; i < selectedObjects.Count; i++)
                        selectedObjects[i].GetComponent<IHuman>().isSelectet = false;
                    
                    mui.humans.Clear();
                    selectedObjects.Clear();

                    ho.GetComponent<IHuman>().isSelectet = true;
                    selectedObjects.Add(ho);

                    mui.humans.Add(ho.GetComponent<IHuman>());
                }
            }
            else
            {
                if (msel) return;

                

                

                onSelecting = false;

                if (hit.collider.GetComponent<IBuilding>())
                {
                    for (int i = 0; i < selectedObjects.Count; i++)
                        selectedObjects[i].GetComponent<IHuman>().isSelectet = false;
                    mui.humans.Clear();
                    selectedObjects.Clear();

                    build = hit.collider.GetComponent<IBuilding>();
                    mui.build = build;
                }
                else
                {
                    build = null;
                    mui.build = null;


                    //movment tut 2 new
                    if (isleft)
                    {
                        for (int i = 0; i < selectedObjects.Count; i++)
                            selectedObjects[i].GetComponent<IHuman>().isSelectet = false;

                        mui.humans.Clear();
                        selectedObjects.Clear();

                        return;
                    }
                    else
                    {
                        if (selectedObjects.Count < 2 && selectedObjects.Count > 0)
                        {
                            selectedObjects[0].GetComponent<IHuman>().walkTo(hit.point, 0.1f);
                            Debug.Log("Set target");
                        }
                        else
                        {
                            for (int i = 0; i < selectedObjects.Count; i++)
                            {
                                selectedObjects[i].GetComponent<IHuman>().setWalkTarget(hit.point, 1f, i, selectedObjects.Count, 2f);
                            }
                        }

                        return;
                    }
                }

                for (int i = 0; i < selectedObjects.Count; i++)
                    selectedObjects[i].GetComponent<IHuman>().isSelectet = false;

                mui.humans.Clear();
                selectedObjects.Clear();
//end new

            }
        }
    }


    

    public void GetObjectsInSelect(bool delete = false)
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(_c);

        objectsInCameraView.Clear();


        foreach (GameObject go in FindObjectsByType<GameObject>(FindObjectsSortMode.None))
        {
            Collider collider = go.GetComponent<Collider>();

            if (collider != null && GeometryUtility.TestPlanesAABB(planes, collider.bounds))
            {
                if (!objectsInCameraView.Contains(go)) objectsInCameraView.Add(go);
            }
        }


        for (int i = 0; i < objectsInCameraView.Count; i++)
        {
           
            GameObject co = objectsInCameraView[i];

            Vector3 sp = _c.WorldToScreenPoint(co.transform.position);

            rectTransform.GetWorldCorners(cc);

            if (sp.x >= cc[0].x && sp.x <= cc[2].x &&
                sp.y >= cc[0].y && sp.y <= cc[1].y)
            {
                if (delete)
                {
                    if (selectedObjects.Contains(co))
                    {
                        co.GetComponent<IHuman>().isSelectet = false;
                        selectedObjects.Remove(co);

                        mui.humans.Remove(co.GetComponent<IHuman>());
                    }
                }
                else
                {
                    if (!selectedObjects.Contains(co) && co.GetComponent<IHuman>())
                    {
                        co.GetComponent<IHuman>().isSelectet = true;
                        selectedObjects.Add(co);

                        mui.humans.Add(co.GetComponent<IHuman>());
                    }
                }
            }
            else
            {
                if (delete || onSelecting) continue;

                if (selectedObjects.Contains(co) && co.GetComponent<IHuman>())
                {
                    co.GetComponent<IHuman>().isSelectet = false;
                    selectedObjects.Remove(co);

                    mui.humans.Remove(co.GetComponent<IHuman>());
                }
            }
        }
    }


    //hilfs methode um selections zu removen und deSelecten
    public void removeSelcetion(GameObject g)
    {
        g.GetComponent<IHuman>().isSelectet = false;
        selectedObjects.Remove(g);
    }

    //hilfs methode um alle selection zu löschen
    public void ClearSelection()
    {
        for (int i = 0; i < selectedObjects.Count; i++)
        {
            selectedObjects[i].GetComponent<IHuman>().isSelectet = false;
        }
        selectedObjects.Clear();
    }

    //methode um ein einzelnde einheit überandere scripte auszuwählen
    public void singleSelectObject(GameObject g)
    {
        for (int i = 0; i < selectedObjects.Count; i++)
        {
            selectedObjects[i].GetComponent<IHuman>().isSelectet = false;
        }

        selectedObjects.Clear();
        selectedObjects.Add(g);
        selectedObjects[0].GetComponent<IHuman>().isSelectet = true;
        
    }


    //new
    private static float doubleClickTime = 0.3f; // Zeitintervall für Doppelklick in Sekunden
    private static float lastClickTime = 0f;

    public static bool DoubleClick(int index)
    {
        if (Input.GetMouseButtonDown(index))
        {
            float currentTime = Time.time;
            if (currentTime - lastClickTime < doubleClickTime)
            {
                lastClickTime = 0f; // Setze den Zeitstempel zurück
                return true; // Doppelklick wurde erkannt
            }
            lastClickTime = currentTime;
        }
        return false; // Kein Doppelklick
    }

    
}
