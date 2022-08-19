using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using System;

public class UnitMovement : NetworkBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Targeter targeter = null;
    [SerializeField] private float chaseRange = 10f;

    private Camera mainCamera;
    #region Server
    public override void OnStartServer()
    {
        GameOverHandler.ServerOnGameOver += ServerHandleGameOver;
    }
    [Server]
    private void ServerHandleGameOver()
    {
        agent.ResetPath();
    }
    public override void OnStopServer()
    {
        GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
    }
    [ServerCallback]
    private void Update()
    {
        Targetable target = targeter.Target;
        if (target != null)
        {
            //if(Vector3.Distance(gameObject.transform.position,targeter.transform.position) > chaseRange)
            if ((target.transform.position - transform.position).sqrMagnitude > chaseRange * chaseRange)
            {
                agent.SetDestination(target.transform.position);
            }
            else if (agent.hasPath)
            {
                agent.ResetPath();
            }
            return;
        }
        if (!agent.hasPath) return;

        if (agent.remainingDistance > agent.stoppingDistance) return;

        agent.ResetPath();

        //agent.velocity = Vector3.zero;

    }
    [Command]
    public void CmdMove(Vector3 position)
    {
        targeter.ClearTarget();

        if (!NavMesh.SamplePosition(position, out NavMeshHit hit, 1f, NavMesh.AllAreas)) { return; }

        agent.SetDestination(hit.position);
    }
    #endregion
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
    }
}
