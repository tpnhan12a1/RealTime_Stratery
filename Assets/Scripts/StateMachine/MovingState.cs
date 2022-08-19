using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingState : State
{
    public override StateType GetStateType()
    {
        return StateType.Moving;
    }

    public override void OnEnterState()
    {
        Debug.Log("Enter Moving State");
    }

    public override void OnExitState()
    {
        
    }

    public override StateType OnUpdateState()
    {


        return StateType.Moving;
    }
}
