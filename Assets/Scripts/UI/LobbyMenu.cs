using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
public class LobbyMenu : MonoBehaviour
{
    [SerializeField] private GameObject lobbyUI = null;
    [SerializeField] private Button startGameButton = null;
    [SerializeField] private TMP_Text[] displayNameText;
    private void Start()
    {
        RTSNetworkManager.ClientOnConnected += HandleClientConnected;
        RTSPlayer.AuthorityOnPartyOwnerStateUpdated += AuthorityHandlePartyOwnerStateUpdated;
        RTSPlayer.ClientOnInfoUpdated += ClientHandleInfoUpdated;
    }
    public void StartGame()
    {
        NetworkClient.connection.identity.GetComponent<RTSPlayer>().CmdStartGame();
    }    
    private void AuthorityHandlePartyOwnerStateUpdated(bool state)
    {
        startGameButton.gameObject.SetActive(state);
    }

    private void OnDestroy()
    {
        RTSNetworkManager.ClientOnConnected -= HandleClientConnected;
        RTSPlayer.AuthorityOnPartyOwnerStateUpdated -= AuthorityHandlePartyOwnerStateUpdated;
        RTSPlayer.ClientOnInfoUpdated -= ClientHandleInfoUpdated;
    }

    private void ClientHandleInfoUpdated()
    {
        List<RTSPlayer> players = ((RTSNetworkManager)NetworkManager.singleton).players;
        
        for(int i = 0; i < players.Count; i++)
        {
            displayNameText[i].text = players[i].DisplayName;
            Debug.Log(players[i].DisplayName);
        }    
        for(int i = players.Count; i < displayNameText.Length; i++)
        {
            displayNameText[i].text = "Waiting for player...";
        }
        
        startGameButton.interactable = players.Count > 1;
    }    
    private void HandleClientConnected()
    {
        lobbyUI.SetActive(true);
    }
    public void LeaveLobby()
    {
       if(NetworkServer.active &&  NetworkClient.isConnected)
       {
            NetworkManager.singleton.StopHost();
       }
        else
        {
            NetworkManager.singleton.StopClient();
            SceneManager.LoadScene(0);
        }
     }
}
