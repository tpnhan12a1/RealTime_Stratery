using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class RTSPlayer : NetworkBehaviour
{
    [SerializeField] private Transform cameraTransform = null;
    public Transform CameraTransform { get { return cameraTransform; } }
    [SerializeField] private List<Unit> myUnits = new List<Unit>();
    public List<Unit> MyUnits { get { return myUnits; } }
    [SerializeField] private List<Building> myBuildings = new List<Building>();
    public List<Building> Buildings { get { return myBuildings; } }

    [SerializeField] private LayerMask buildingBlockLayer = new LayerMask();
    [SerializeField] private Building[] buildings = new Building[0];
    [SerializeField] private float buildingRangeLimit = 5f;

    [SyncVar(hook = nameof(ClientHandleResourcesUpdated))]
    [SerializeField] private int resources = 100;

    [SyncVar(hook =nameof(AuthorityHandlePartyOwnerStateUpdated))]
    [SerializeField] private bool isPartyOwner = false;
    public int Resources { get { return resources; }
        set { resources = value; }
    }
    [SyncVar(hook =nameof(ClientHandleDisplayNameUpdated))]
    private string displayName;

    public static event Action ClientOnInfoUpdated;
    public string DisplayName
    { 
        get { return displayName; } 
        [Server]
        set { displayName = value; }
    }
    

    public event Action<int> ClientOnResourcesUpdated;

    public static event Action<bool> AuthorityOnPartyOwnerStateUpdated;
    public bool IsPartyOwner { get { return isPartyOwner; }
        [Server]
        set { isPartyOwner = value; }
    }
   
    public bool CanPlaceBuilding(BoxCollider buildingCollider, Vector3 point)
    {
        if (Physics.CheckBox(
                    point + buildingCollider.center,
                    buildingCollider.size / 2,
                    Quaternion.identity,
                    buildingBlockLayer))
        {
            return false;
        }

        foreach (Building building in myBuildings)
        {
            if ((point - building.transform.position).sqrMagnitude
                <= buildingRangeLimit * buildingRangeLimit)
            {
                return true;
            }
        }
        return false;
    }

    #region Server
    public override void OnStartServer()
    {
        Unit.ServerOnUnitSpawned += ServerHandleUnitSpawned;
        Unit.ServerOnUnitDespawned += ServerHandleUnitDespawned;
        Building.ServerOnBuildingSpawned += ServerHandleBuildingSpawned;
        Building.ServerOnBuildingDespawned += ServerHandleBuildingDespawned;
        DontDestroyOnLoad(gameObject);
    }

    public override void OnStopServer()
    {
        Unit.ServerOnUnitSpawned -= ServerHandleUnitSpawned;
        Unit.ServerOnUnitDespawned -= ServerHandleUnitDespawned;
        Building.ServerOnBuildingSpawned -= ServerHandleBuildingSpawned;
        Building.ServerOnBuildingDespawned -= ServerHandleBuildingDespawned;
    }

    private void ServerHandleUnitSpawned(Unit unit)
    {
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId) return;
        myUnits.Add(unit);
    }
    private void ServerHandleUnitDespawned(Unit unit)
    {
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId) return;

        myUnits.Remove(unit);
    }

    private void ServerHandleBuildingDespawned(Building building)
    {
        if (building.connectionToClient.connectionId != connectionToClient.connectionId) return;
        myBuildings.Remove(building);
    }

    private void ServerHandleBuildingSpawned(Building building)
    {
        if (building.connectionToClient.connectionId != connectionToClient.connectionId) return;
        myBuildings.Add(building);
    }
    [Command]
    public void CmdStartGame()
    {
        if (!isPartyOwner) return;

        ((RTSNetworkManager)NetworkManager.singleton).StartGame();
    }    

    [Command]
    public void CmdTryPlaceBuilding(int buildingId, Vector3 point)
    {
        Building buildingToPlace = null;

        foreach (Building building in buildings)
        {
            if (building.Id == buildingId)
            {
                buildingToPlace = building;
                break;
            }
        }

        if (buildingToPlace == null) { return; }

        if (resources < buildingToPlace.Price) { return; }

        BoxCollider buildingCollider = buildingToPlace.GetComponent<BoxCollider>();

        if (!CanPlaceBuilding(buildingCollider, point)) { return; }
        Quaternion rotation = buildingToPlace.transform.rotation;
        rotation = Quaternion.Euler(rotation.x, UnityEngine.Random.Range(0f, 360f), rotation.z);
        GameObject buildingInstance =
            Instantiate(buildingToPlace.gameObject, point, rotation);

        NetworkServer.Spawn(buildingInstance, connectionToClient);

        Resources = resources - buildingToPlace.Price;
    }

    #endregion

    #region Client
    private void ClientHandleDisplayNameUpdated(string oldName, string newName)
    {
        ClientOnInfoUpdated?.Invoke();
    }
    private void AuthorityHandlePartyOwnerStateUpdated(bool oldState, bool newState)
    {
        if (!hasAuthority) return;
        AuthorityOnPartyOwnerStateUpdated?.Invoke(newState);
    }    
    public override void OnStartAuthority()
    {
        if(NetworkServer.active) { return; }

        Unit.AuthorityOnUnitSpawned += AuthorityHandleUnitSpawned;
        Unit.AuthorityOnUnitDespawned += AuthorityHandleUnitDespawned;
    }
    public override void OnStartClient()
    {
        //ClientOnInfoUpdated?.Invoke();
        if (NetworkServer.active) { return; }
        DontDestroyOnLoad(gameObject);
        ((RTSNetworkManager)NetworkManager.singleton).players.Add(this);
    }
    public override void OnStopClient()
    {
        ClientOnInfoUpdated?.Invoke();
        if (!isClientOnly) { return; }
        ((RTSNetworkManager)NetworkManager.singleton).players.Remove(this);

        if(!hasAuthority) { return; }
        Unit.AuthorityOnUnitSpawned -= AuthorityHandleUnitSpawned;
        Unit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;
    }

    private void ClientHandleResourcesUpdated(int oldResources, int newResources)
    {
        ClientOnResourcesUpdated?.Invoke(newResources);
    }

    private void AuthorityHandleUnitSpawned(Unit unit)
    {
        //Debug.Log("AuthorityHandleUnitSpawned");
        myUnits.Add(unit);
    }

    private void AuthorityHandleUnitDespawned(Unit unit)
    {
        myUnits.Remove(unit);
    }

    #endregion

}
