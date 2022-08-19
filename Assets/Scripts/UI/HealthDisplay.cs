using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
public class HealthDisplay : MonoBehaviour
{
    [SerializeField] private Health health = null;
   // [SerializeField] private GameObject healthBar = null;
    [SerializeField] private Image healthImge = null;
    private void Awake()
    {
        health.ClientOnHealthUpdated += HandleHealthUpdated;
        health.ClientOnUnitSpawned += ClientHandleOnUnitSpawned;
        //healthImge.color = Color.white;
    }
    private void OnDestroy()
    {
        health.ClientOnHealthUpdated -= HandleHealthUpdated;
        health.ClientOnUnitSpawned -= ClientHandleOnUnitSpawned;
    }

    private void ClientHandleOnUnitSpawned()
    {
        healthImge.color = Color.white;
    }

    private void HandleHealthUpdated(int currentHealth, int maxHealth)
    {
        healthImge.fillAmount = (float)currentHealth / maxHealth;
    }
}
