using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
using System.Collections;

public class SpawnPlayer : SimulationBehaviour, INetworkRunnerCallbacks { 
    public NetworkObject Player;
    PlayerRef p;
    public NetworkRunner nr;
    public GameObject leftTheGame, worldCam;

    public void OnConnectedToServer(NetworkRunner runner)
    {
       
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
       
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
      
    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
        
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
       
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
       
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
       
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.SessionInfo.PlayerCount < 3)
        {
            var s = runner.Spawn(Player, null, null, player);
            runner.SetPlayerObject(player, s);
        }

    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
       
        GameObject g = GameObject.FindGameObjectWithTag("Player1");
        GameObject g2 = GameObject.FindGameObjectWithTag("Player2");

        if(g != null) Destroy(g);
        if(g2 != null) Destroy(g2);

        worldCam.SetActive(true);
        leftTheGame.SetActive(true);
        leftTheGame.GetComponent<TurnBackToMenu>().TurnBack(3, 0, runner);

    }



    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {
       
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
       
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
      
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        if (runner.IsClient)
        {
            
            GameObject g = GameObject.FindGameObjectWithTag("Player1");
            GameObject g2 = GameObject.FindGameObjectWithTag("Player2");

            if (g != null) Destroy(g);
            if (g2 != null) Destroy(g2);

            worldCam.SetActive(true);
            leftTheGame.SetActive(true);

            if (runner.SessionInfo.PlayerCount > 2)
                leftTheGame.GetComponent<TurnBackToMenu>().TurnBack(3, 0, null, 1);
            else leftTheGame.GetComponent<TurnBackToMenu>().TurnBack(3, 0);
        }
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        
    }

}
