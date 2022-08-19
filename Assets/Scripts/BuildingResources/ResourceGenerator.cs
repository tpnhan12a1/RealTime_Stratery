using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;
public class ResourceGenerator : NetworkBehaviour
{
    [SerializeField] private Health health = null;
    [SerializeField] private int resourcesPerInterval = 10;
    [SerializeField] private float interval = 2f;

    [SerializeField] private GameObject resourceQueueParent = null;
    [SerializeField] private TMPro.TMP_Text remainUnitsText = null;
    [SerializeField] private Image unitProgressImage = null;

    //[SerializeField] private float secondsBetweenSpawn = 1f;

    [SyncVar(hook = nameof(ClientHandleResourcesGeneratorUpdated))]
    [SerializeField] private float timer = 0.0f;

    private RTSPlayer player;

    public override void OnStartServer()
    {
        timer = interval;
        

        health.ServerOnDie += ServerHandleDie;
        GameOverHandler.ServerOnGameOver += ServerHandleGameOver;
    }

    public override void OnStopServer()
    {
        health.ServerOnDie -= ServerHandleDie;
        GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
    }

    [ServerCallback]
    private void Update()
    {
        if(player == null)
        {
            player = connectionToClient.identity.GetComponent<RTSPlayer>();
        }   
        
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            player.Resources = player.Resources + resourcesPerInterval;

            timer += interval;
        }
    }

    private void ServerHandleDie()
    {
        NetworkServer.Destroy(gameObject);
    }

    private void ServerHandleGameOver()
    {
        enabled = false;
    }
    #region Client
    private void ClientHandleResourcesGeneratorUpdated(float oldR, float newR)
    {

        remainUnitsText.text = Mathf.FloorToInt(newR).ToString();
        unitProgressImage.fillAmount = timer / interval;
    }
    public override void OnStartClient()
    {
        if (!hasAuthority) return;
        resourceQueueParent.SetActive(true);
    }
    #endregion
}
