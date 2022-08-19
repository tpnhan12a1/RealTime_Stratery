using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class IdlingState : State
{
    [Server]
    public override StateType GetStateType()
    {
        return StateType.Idling;
    }
    [Server]
    public override void OnEnterState()
    {
        Debug.Log("Enter Idling State");
    }
    [Server]
    public override void OnExitState()
    {
    }
    [Server]
    public override StateType OnUpdateState()
    {
        if (stateMachine.Agent.hasPath) return StateType.Moving;

        return StateType.Idling;
    }
}
