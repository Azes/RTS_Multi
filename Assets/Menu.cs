using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Menu : MonoBehaviour
{

    public TMP_Dropdown map;
    public TMP_InputField lobbyname;
    public MenuInfoBox info;
    
    

    private void Start()
    {
        if (LobbyInfo.TurnBackMessage.Length > 0)
        {
            info.setInfo(LobbyInfo.isBack(), 3);
        }
    }

    public void StartSingel()
    {
        Debug.Log("Singel Player Game");
    }

    public bool correctInput(string input)
    {
        bool r = false;

        if(input.Length > 0)
        {

        }

        foreach (char c in input)
        {
            if ((c >= 'a' && c <= 'z') ||  
                (c >= 'A' && c <= 'Z') || 
                (c >= '0' && c <= '9'))
                r = true;
            else return false;
        }

        return r;
    }



    public void create()
    {
        if (lobbyname.text == "")
        {
            info.setInfo("Lobby Name cannot be null", 2);
            return;
        }
        else if (lobbyname.text.Length < 4)
        {
            info.setInfo("Lobby Name is to short", 2);
            return;
        }
        else if (!correctInput(lobbyname.text))
        {
            info.setInfo("Lobby name has illegal character", 2);
            return;
        }

        LobbyInfo.LobbyName = lobbyname.text;
        SceneManager.LoadScene(map.value + 1);

    }
    
}
