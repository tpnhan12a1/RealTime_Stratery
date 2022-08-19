using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Mirror;
using UnityEngine.UI;
public class BuildingSpawner : NetworkBehaviour
{
    [SerializeField] private Health health = null;

    [SerializeField] private GameObject unitPrefab = null;
    [SerializeField] private Transform unitSpawnTransform = null;
    [SerializeField] private GameObject unitQueueParent = null;
    [SerializeField] private float maxUnitQueue = 5;
    [SerializeField] private TMPro.TMP_Text remainUnitsText = null;
    [SerializeField] private Image unitProgressImage = null;

    //[SerializeField] private float secondsBetweenSpawn = 1f;

    [SyncVar(hook =nameof(ClientHandleQueueUnitsUpdated))]
    [SerializeField] private float elapsedTime = 0.0f;

    //GameOverHandler register event
    public static event Action<BuildingSpawner> ServerOnBuildingSpawnerSpawned;
    // Unit register event
    public static event Action<BuildingSpawner> ServerOnBuildingSpawnerDeSpawned;
    #region Server
    public override void OnStartServer()
    {
        health.ServerOnDie += ServerHandleDie;
        ServerOnBuildingSpawnerSpawned?.Invoke(this);
    }
    public override void OnStopServer()
    {
        ServerOnBuildingSpawnerSpawned?.Invoke(this);
        health.ServerOnDie -= ServerHandleDie;
    }
    [Server]
    private void ServerHandleDie()
    {
        NetworkServer.Destroy(gameObject);
    }

    [ServerCallback]
    private void Update()
    {
        elapsedTime -= Time.deltaTime;
        if (elapsedTime <= 0)
        {
            elapsedTime = maxUnitQueue;
            ProduceUnits();
        }
    }
    [Server]
    private void ProduceUnits()
    {
        GameObject unitInstance = Instantiate(unitPrefab,
            unitSpawnTransform.position,
            unitSpawnTransform.rotation);

        NetworkServer.Spawn(unitInstance, connectionToClient);

    }
    #endregion

    #region Client
    private void ClientHandleQueueUnitsUpdated(float oldUnits, float newUnits)
    {
        remainUnitsText.text = Mathf.FloorToInt(newUnits).ToString();
        unitProgressImage.fillAmount = elapsedTime / maxUnitQueue;
    }
    public override void OnStartClient()
    {
        if (!hasAuthority) return;
        elapsedTime = maxUnitQueue;
        unitQueueParent.SetActive(true);
    }
    #endregion
}
