using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using UnityEngine.Events;

public class Unit : NetworkBehaviour
{
    [SerializeField] private int resourceCost = 5;
    [SerializeField] private Health health = null;
    [SerializeField] private UnitMovement unitMovement = null;
    [SerializeField] private Targeter targeter = null;

    [SerializeField] private UnityEvent onSelected = null;
    [SerializeField] private UnityEvent onDeselected = null;

    //RTSPlayer register event
    
    public static event Action<Unit> ServerOnUnitSpawned;
    // ch?a xóa nh?ng th?c th? c?a nó
    public static event Action<Unit> ServerOnUnitDespawned;

    //Health register event
    //RTSPlayer Register event
    public static event Action<Unit> AuthorityOnUnitSpawned;
    //UnitSelectedHandler Reigister event
    public static event Action<Unit> AuthorityOnUnitDespawned;
   
    public UnitMovement UnitMovement { get { return unitMovement; } }
    public Targeter Targeter { get { return targeter; } }
    public int ResourceCost { get { return resourceCost; } }
    #region Server
    public override void OnStartServer()
    {
        ServerOnUnitSpawned?.Invoke(this);
        health.ServerOnDie += ServerHandleDie;
        Core.ServerOnCoreDespawned += ServerHandleCoreDespawned;
        
    }
    private void ServerHandleDie()
    {
        NetworkServer.Destroy(gameObject);
    }

    public override void OnStopServer()
    {
        ServerOnUnitDespawned?.Invoke(this);
        health.ServerOnDie -= ServerHandleDie;
        Core.ServerOnCoreDespawned -= ServerHandleCoreDespawned;
    }
    private void ServerHandleCoreDespawned(Core core)
    {
        if (core.connectionToClient.connectionId != connectionToClient.connectionId) return;

        NetworkServer.Destroy(gameObject);
    }

    #endregion

    #region Client
    public override void OnStartAuthority()
    {
        AuthorityOnUnitSpawned?.Invoke(this);
    }
    public override void OnStopClient()
    {
        if (!hasAuthority) return;
        AuthorityOnUnitDespawned?.Invoke(this);
    }

    [Client]
    public void Select()
    {
        if (!hasAuthority) return;
        onSelected?.Invoke();
    }

    [Client]
    public void Deselect()
    {
        if (!hasAuthority) return;
        onDeselected?.Invoke();
    }
    #endregion
}
