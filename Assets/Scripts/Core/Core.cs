using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System;
public class Core : NetworkBehaviour
{
    [SerializeField] private Health health = null;

    [SerializeField] private GameObject unitPrefab = null;
    //[SerializeField] private Transform unitSpawnTransform = null;
    //[SerializeField] private float secondsBetweenSpawn = 1f;

     //private float elapsedTime = 0.0f;
    
    //GameOverHandler register event
    public static event Action<Core> ServerOnCoreSpawned;
    // Unit register event
    public static event Action<Core> ServerOnCoreDespawned;
    #region Server
    public override void OnStartServer()
    {
        health.ServerOnDie += ServerHandleDie;
        ServerOnCoreSpawned?.Invoke(this);
    }
    public override void OnStopServer()
    {
        ServerOnCoreDespawned?.Invoke(this);
        health.ServerOnDie -= ServerHandleDie;
    }
    [Server]
    private void ServerHandleDie()
    {
        NetworkServer.Destroy(gameObject);
    }

    //[ServerCallback]
    //private void Update()
    //{
    //    elapsedTime += Time.deltaTime;
    //    if (elapsedTime > secondsBetweenSpawn)
    //    {
    //        elapsedTime = 0f;
    //        ProduceUnits();
    //    }
    //}
    //[Server]
    //private void ProduceUnits()
    //{
    //    GameObject unitInstance = Instantiate(unitPrefab,
    //        unitSpawnTransform.position,
    //        unitSpawnTransform.rotation);

    //    NetworkServer.Spawn(unitInstance,connectionToClient);
        
    //}
    #endregion

    #region Client

    #endregion
}
