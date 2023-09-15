using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

//new class für die erstellung unseres Server raums wir erstellen erstmal
//nur einen test Raum in zukunft werden wir das räume erstellen mit dem spiel Hauptmenü
public class NetworkRunnerHandler : MonoBehaviour
{
    //new
    public bool TestMode;

    //unser networkrunner dieser scene
    public NetworkRunner networkRunner;
    //vorübergehendes TMP zum anzeig wer host und wer client ist
    public TextMeshProUGUI tmp;
    //WaitSceen und die Scenen Camera für die warte zeit
    public GameObject WaitScreen, startCam;
    //die Spawn punkte dieses Levels / dieser Map die spawn Punkte sind die jeweiligen der StadtZentren 
    //0 = player1 = Host 
    //1 = player2 = Client
    //Die spawnpunkte sind in den jeweiligen Stadtzentren 
    public Transform[] spawnPlayer;

    private void Awake()
    {
        
        networkRunner.ProvideInput = true;

        //wir starten den netrunner
        networkRunner.StartGame(new StartGameArgs()
        {
            SessionName = (TestMode ? LobbyInfo.LobbyName : "RTS"),//new neuer test mode für die entwicklung
            GameMode = GameMode.AutoHostOrClient,
            Scene = SceneManager.GetActiveScene().buildIndex,

        });

        //new deaktivieren im test mode
        if(!TestMode)
            StartCoroutine(wait());

    }

    IEnumerator wait()
    {
        yield return new WaitForSecondsRealtime(20);

        networkRunner.Shutdown(true);

        LobbyInfo.TurnBackMessage = "Lobby over Time";

        SceneManager.LoadScene(0);    
    }

    //

    private void Update()
    {
        //testet ob der runner angeschalet ist also der server lauft
        if (networkRunner.IsRunning)
        {
            //new neuer test mode
            if (TestMode)
            {
                GameObject g11 = null;
                GameObject[] g12 = Resources.FindObjectsOfTypeAll<GameObject>();

                foreach (GameObject obj in g12)
                {
                    if (obj.CompareTag("Player1") && !obj.activeSelf)
                    {
                        g11 = obj;
                    }
                }

                if (g11 != null)
                {
                    g11.SetActive(true);
                    g11.transform.position = spawnPlayer[0].transform.position;
                    g11.transform.rotation = spawnPlayer[0].rotation;

                    tmp.text = "TEST MODE";

                    //warte screen und scenen camera werden deaktiviert
                    WaitScreen.gameObject.SetActive(false);
                    startCam.SetActive(false);

                    Destroy(gameObject);
                }
            }
            //
            
            //überprüft ob die Lobby schon voll ist
            if (networkRunner.SessionInfo.PlayerCount > 2)
            {
                LobbyInfo.TurnBackMessage="Lobby is full";
                
                Destroy(networkRunner.transform.gameObject);
                SceneManager.LoadScene(0);
                Debug.Log("Lobby full");
                return;
            }

            //sucht nach nach den ersten und zweiten Player

            GameObject g1 = null;
            GameObject g2 = null;
            GameObject[] g = Resources.FindObjectsOfTypeAll<GameObject>();

            foreach (GameObject obj in g)
            {
                if (obj.CompareTag("Player1") && !obj.activeSelf)
                {
                    g1 = obj;
                }
                else if (obj.CompareTag("Player2") && !obj.activeSelf)
                {
                    g2 = obj;
                }
            }
            
            //wenn player1 und player2 in der lobby sind werden sie gesetzt
            if (g1 != null && g2 != null)
            {
                g1.SetActive(true);
                g1.transform.position = spawnPlayer[0].transform.position;
                g1.transform.rotation = spawnPlayer[0].rotation;

                g2.SetActive(true);
                g2.transform.position = spawnPlayer[1].transform.position;
                g2.transform.rotation = spawnPlayer[1].rotation;

                //zeigt und im TMP an wer host und client ist
                if (networkRunner.Mode == SimulationModes.Host) tmp.text = "Host in lobby : " + networkRunner.SessionInfo.Name;
                else if (networkRunner.Mode == SimulationModes.Client) tmp.text = "Client player in lobby : " + networkRunner.SessionInfo.Name;

                //warte screen und scenen camera werden deaktiviert
                WaitScreen.gameObject.SetActive(false);
                startCam.SetActive(false);

                Debug.Log("Players Have Join the Game");

                //dann wird der Handler nicht mehr benötigt und zerstört
                Destroy(gameObject);

            }
            else Debug.Log("Player1 = " + (g1 != null) + "  Player2 = " + (g2 != null));
        }
    }

    
   
}
