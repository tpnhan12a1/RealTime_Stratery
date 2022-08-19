using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;

public class ResourcesDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text resourcesText = null;
    private RTSPlayer player;

    private void Start()
    {
        player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
        ClientHandleResourcesUpdated(player.Resources);
        player.ClientOnResourcesUpdated += ClientHandleResourcesUpdated;
        ClientHandleResourcesUpdated(player.Resources);
    }
    private void OnDestroy()
    {
        player.ClientOnResourcesUpdated -= ClientHandleResourcesUpdated;
    }

    private void ClientHandleResourcesUpdated(int resources)
    {
        resourcesText.text = $"Resources: {resources}$";
    }
}
