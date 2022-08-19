using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public abstract class State : MonoBehaviour
{
    public UnitStateMachine stateMachine = null;
   
    public abstract StateType GetStateType();
   
    public abstract void OnEnterState();
  
    public abstract void OnExitState();
 
    public abstract StateType OnUpdateState();
   
    public virtual void RegisterStateMachine(UnitStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }    
}
