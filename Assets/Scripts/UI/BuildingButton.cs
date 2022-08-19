using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BuildingButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Building building = null;
    [SerializeField] private Image iconImage = null;
    [SerializeField] private TMP_Text priceText = null;
    [SerializeField] private LayerMask floorMask = new LayerMask();



    private Camera mainCamera;
    private BoxCollider buildingCollider;
    private RTSPlayer player;
    private GameObject buildingPreviewInstance;
    private Renderer buildingRendererInstance;


    private void Start()
    {
        mainCamera = Camera.main;
        player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();

        iconImage.sprite = building.Icon;
        priceText.text = building.Price.ToString();

        player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();

        buildingCollider = building.GetComponent<BoxCollider>();
        buildingRendererInstance = building.GetComponentInChildren<Renderer>();
    }

    private void Update()
    {
        if (buildingPreviewInstance == null) { return; }

        UpdateBuildingPreview();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) { return; }

        //if (player.GetResources() < building.GetPrice()) { return; }

        buildingPreviewInstance = Instantiate(building.BuildingPreview);

        buildingPreviewInstance.SetActive(false);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (buildingPreviewInstance == null) { return; }

        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, floorMask))
        {
            player.CmdTryPlaceBuilding(building.Id, hit.point);
        }

        Destroy(buildingPreviewInstance);
    }

    private void UpdateBuildingPreview()
    {
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, floorMask)) { return; }

        buildingPreviewInstance.transform.position = hit.point;

        if (!buildingPreviewInstance.activeSelf)
        {
            buildingPreviewInstance.SetActive(true);
        }

        Color color = player.CanPlaceBuilding(buildingCollider, hit.point) ? Color.green : Color.red;

        //buildingRendererInstance.material.color = color;
    }
}
       
