using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class Targeter : NetworkBehaviour
{
   
    [SerializeField] private Targetable target;
    public Targetable Target { get { return target; } }

    #region Server
    public override void OnStartServer()
    {
        GameOverHandler.ServerOnGameOver += ServerHandleGameOver;
    }
    public override void OnStopServer()
    {
        GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
    }
    [Server]
    public void ServerHandleGameOver()
    {
        ClearTarget();
    }    
    [Command]
    public void CmdSetTarget(GameObject targetObject)
    {
        if (targetObject.TryGetComponent<Targetable>(out Targetable target))
        {
            Debug.Log(target);
            this.target = target;
        }
    }
    [Server]
    public void ClearTarget()
    {
        this.target = null;
    }

    #endregion

    #region Client
   
    #endregion

}
