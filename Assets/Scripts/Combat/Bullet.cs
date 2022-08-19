using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class Bullet : NetworkBehaviour
{
     [SerializeField] private Rigidbody rigidbody = null;
    [SerializeField] private int damgeToAttack = 10;
    [SerializeField] private float fireForce = 10f;
    [SerializeField] private float destroyAffterTime = 5f;
    void Start()
    {
        rigidbody.AddForce(transform.forward * fireForce, ForceMode.VelocityChange);
        AudioManager.Instance.PlayOneShotAtPoint(AudioManager.Instance.clipList[1], transform.position);
    }
    #region Server
    private void OnTriggerEnter(Collider collider)
    {
        if(collider.TryGetComponent<NetworkIdentity>( out NetworkIdentity networkIdentity))
        {
            if (networkIdentity.connectionToClient == connectionToClient) return;
        }    
        if (collider.TryGetComponent<Health>(out Health health))
        {
            health.TakeDamge(damgeToAttack);
        }
        DestroySelf();
    }
    public override void OnStartServer()
    {
        Invoke(nameof(DestroySelf), destroyAffterTime);
    }

    [Server]
    private void DestroySelf()
    {
        NetworkServer.Destroy(gameObject);
    }
    #endregion

}
