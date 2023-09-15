using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
public class LobbyInfo : MonoBehaviour
{
    private static GameObject _instance;
    public static string LobbyName = "";
    public static string TurnBackMessage = "";
    

    private void Awake()
    {
        if (_instance == null) _instance = gameObject;
        else if (_instance != gameObject) Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public static string isBack()
    {
        string s = TurnBackMessage;
        TurnBackMessage = "";
        return s;
    }
}
