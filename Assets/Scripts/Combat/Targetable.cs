using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class Targetable : NetworkBehaviour
{
    [SerializeField] private Transform aimToPoint = null;
    public Transform AimAtPoint { get { return aimToPoint; } }
}
