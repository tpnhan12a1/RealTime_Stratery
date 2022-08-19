using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.AI;
public enum StateType
{
    Idling, Moving, Attack, Chasing
}
public class UnitStateMachine : NetworkBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Targeter targeter = null;
    public State currentState = null;
    public StateType curentStateType = StateType.Idling;

    public Dictionary<StateType, State> states = new Dictionary<StateType, State>();

    public NavMeshAgent Agent { get { return agent; } }
    public override void OnStartServer()
    {
        State[] states = GetComponents<State>();
        foreach(State state in states)
        {
            if (this.states.ContainsKey(state.GetStateType())) return;
            this.states.Add(state.GetStateType(), state);
            state.RegisterStateMachine(this);
        }    

        if (currentState == null)
        {
            curentStateType = StateType.Idling;
            currentState = this.states[StateType.Idling];
        }    

        currentState.OnEnterState();
    }
    [ServerCallback]
    public void Update()
    {
        if (currentState == null) return;

        StateType newStateType = currentState.OnUpdateState();
        if (newStateType == curentStateType) return;
        currentState.OnExitState();
        curentStateType = newStateType;
        currentState = states[newStateType];
        currentState.OnEnterState();
    }
}
