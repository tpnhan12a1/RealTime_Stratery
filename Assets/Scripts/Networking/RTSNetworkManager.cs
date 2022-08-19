using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using System;
public class RTSNetworkManager : NetworkManager
{
    [SerializeField] GameObject corePrefabs = null;

    [SerializeField] GameOverHandler gameOverHandlerPrefab = null;
    //Join Lobby Menu register event
    public static event Action ClientOnConnected;
    public static event Action ClientOnDisconnected;

    public List<RTSPlayer> players = new List<RTSPlayer>();

    private bool isGameInProgress = false;

    #region Server
    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        if (!isGameInProgress) return;

        conn.Disconnect();
    }
    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        RTSPlayer player = conn.identity.GetComponent<RTSPlayer>();
        players.Remove(player);
        base.OnServerDisconnect(conn);
    }
    public override void OnStopServer()
    {
        players.Clear();
        isGameInProgress = false;
    }
    public void StartGame()
    {
        if(players.Count <2 ) return;
        isGameInProgress=true;
        ServerChangeScene("Scene_Map_01");
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);
        RTSPlayer player = conn.identity.GetComponent<RTSPlayer>();

        players.Add(player);
        player.DisplayName = $"Player {players.Count}"; /// ??i tên
        player.IsPartyOwner = (players.Count == 1);
    }
    public override void OnServerSceneChanged(string newSceneName)
    {
        if (SceneManager.GetActiveScene().name.StartsWith("Scene_Map_01"))
        {
            GameOverHandler gameOverHandlerInstance = Instantiate(gameOverHandlerPrefab);

            NetworkServer.Spawn(gameOverHandlerInstance.gameObject);

            foreach (RTSPlayer player in players)
            {
                GameObject baseInstance = Instantiate(
                    corePrefabs,
                    GetStartPosition().position,
                    Quaternion.identity);

                NetworkServer.Spawn(baseInstance, player.connectionToClient);
            }
        }
    }
    #endregion

    #region Client
    public override void OnClientConnect()
    {
        base.OnClientConnect();
        ClientOnConnected?.Invoke();
    }
    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();
        ClientOnDisconnected?.Invoke();
    }
    public override void OnStopClient()
    {
        players.Clear();
    }
    #endregion
}
