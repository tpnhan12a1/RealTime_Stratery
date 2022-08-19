using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class Health : NetworkBehaviour
{
    [SerializeField][Range(0, 1000)] private int maxHealth = 100;

    [SyncVar(hook = nameof(HandleHealthUpdated))]
    private int currentHealth;

    //Health display register event
    public event Action<int, int> ClientOnHealthUpdated;

    public event Action ClientOnUnitSpawned;
    //Core register event
    //Unit register event
    public event Action ServerOnDie;
    #region Server

    public override void OnStartServer()
    {
        currentHealth = maxHealth;
    }

    [Server]
    public void TakeDamge(int damgeAmount)
    {
        if (currentHealth == 0) return;

        currentHealth = Mathf.Clamp(currentHealth - damgeAmount, 0, maxHealth);

        if (currentHealth == 0)
        {
            Debug.Log("We died");
            AudioManager.Instance.PlayOneShotAtPoint(AudioManager.Instance.clipList[0], transform.position);
            ServerOnDie?.Invoke();
        }
    }
    #endregion
    #region Client
    private void HandleHealthUpdated(int oldHealth, int newHealth)
    {
        ClientOnHealthUpdated?.Invoke(newHealth, maxHealth);
    }
    public override void OnStartClient()
    {
        if (!hasAuthority) return;

        ClientOnUnitSpawned?.Invoke();

    }
    #endregion
}
