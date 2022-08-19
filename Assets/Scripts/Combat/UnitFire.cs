using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class UnitFire : NetworkBehaviour
{
    [SerializeField] private Targeter targeter = null;
    [SerializeField] private GameObject bulletPrefab = null;
    [SerializeField] private Transform bulletSpawnPoint = null;

    [SerializeField][Range(0f, 100f)] private float fireRange = 5f;
    [SerializeField][Range(0f, 5f)] private float fireRate = 1f;
    [SerializeField][Range(0f, 360f)] private float rotationSpeed = 180f;
    [SerializeField][Range(0f, 90f)] private float angleCanFire = 10f;

    private float lastFireTime;
    [ServerCallback]
    private void Update()
    {
        if (targeter.Target == null)
        {
            return;
        }
        

        Targetable target = targeter.Target;
        float angle = Vector3.Angle(target.gameObject.transform.position - transform.position, transform.forward) / 2;
        Debug.Log(angle);
        if (angle > angleCanFire)
        {
            transform.LookAt(target.transform.position);
            //Rotate
            return;
        }
        if (!CanFireAtTarget()) return;

        if (Time.time > 1 / fireRate + lastFireTime)
        {
            Quaternion bulletRotation = Quaternion.LookRotation(
                targeter.Target.AimAtPoint.position - bulletSpawnPoint.position);

            GameObject bulletInstance = Instantiate(bulletPrefab,
                bulletSpawnPoint.position, bulletRotation);

            NetworkServer.Spawn(bulletInstance, connectionToClient);
            lastFireTime = Time.time;
        }
    }

    private bool CanFireAtTarget()
    {
        Targetable target = targeter.Target;
        Debug.Log("Distance "+ (target.transform.position - transform.position).sqrMagnitude);
        return ((target.transform.position - transform.position).sqrMagnitude <= fireRange * fireRange);
    }
}
//Quaternion targetRotation =
//              Quaternion.LookRotation(targeter.Target.transform.position - transform.position);

//transform.rotation = Quaternion.RotateTowards(
//    transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

////check angle fire
//Targetable target = targeter.Target;
//float angle = Vector3.Angle(target.gameObject.transform.position - transform.position, transform.forward) / 2;
//if (angle > angleCanFire) return;