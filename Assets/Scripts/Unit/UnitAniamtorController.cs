using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;
public class UnitAniamtorController : MonoBehaviour
{
    [SerializeField] private Animator animator = null;
    [SerializeField] private NavMeshAgent agent = null;
    [SerializeField] private NetworkAnimator networkAnimator = null;

    [SerializeField] private string speedString = "Speed";
    [SerializeField] private string attackString = "Attack";
    [SerializeField] private string isDieString = "IsDie";

    private int speedHash;
    private int attackHash;
    private int isDieHash;

    private void Start()
    {
        speedHash = Animator.StringToHash(speedString);
        attackHash = Animator.StringToHash(attackString);
        isDieHash = Animator.StringToHash(isDieString);
    }
    [ServerCallback]
    public void Update()
    {
        networkAnimator.animator.SetFloat(speedHash, agent.velocity.magnitude);
    }  
}
