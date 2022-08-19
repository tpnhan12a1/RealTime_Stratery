using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
public class GameOverHandler : NetworkBehaviour
{
    public static event Action ServerOnGameOver; // ch?a còn fix
    public static event Action<string> ClientOnGameOver;

    [SerializeField] private List<Core> cores = new List<Core>();

    #region Server
    public override void OnStartServer()
    {
        Core.ServerOnCoreSpawned += ServerHandleCoreSpawned;
        Core.ServerOnCoreDespawned += ServerHandleCoreDespawned;
    }
    public override void OnStopServer()
    {
        Core.ServerOnCoreSpawned -= ServerHandleCoreSpawned;
        Core.ServerOnCoreDespawned -= ServerHandleCoreDespawned;
    }
    [Server]
    private void ServerHandleCoreSpawned(Core core)
    {
        cores.Add(core);
    }
    private  void ServerHandleCoreDespawned(Core core)
    {
        cores.Remove(core);
        if (cores.Count != 1) return;

        int playerId = cores[0].connectionToClient.connectionId;

        RpcGameOver($"Player {playerId}");

        Debug.Log($"Player {playerId} is wined");

        ServerOnGameOver?.Invoke();
    }
    #endregion

    #region Client
    [ClientRpc]
    private void RpcGameOver(string winner)
    {
        ClientOnGameOver?.Invoke(winner);
    }
    #endregion
}
