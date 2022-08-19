using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;
using UnityEngine.EventSystems;
public class MiniMap : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    [SerializeField]
    private RectTransform minimapRect = null;
    [SerializeField] private float mapScale = 20f;
    [SerializeField] private float offset = -6f;

    private Transform playerCameraTranform = null;
    private void Start()
    {
        playerCameraTranform = NetworkClient.connection.identity.GetComponent<RTSPlayer>().CameraTransform;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        MoveCamera();
    }

    public void OnDrag(PointerEventData eventData)
    {
        MoveCamera();
    }
    private void MoveCamera()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
            minimapRect,
            mousePos,
            null,
            out Vector2 localPoint)) return;
        Debug.Log("Minimaprect: "+minimapRect.rect);
        Debug.Log(localPoint);
        Vector2 lerp = new Vector2((localPoint.x - minimapRect.rect.x) / minimapRect.rect.width,
            (localPoint.y - minimapRect.rect.y) / minimapRect.rect.height);

        Vector3 newCameraPos = new Vector3(Mathf.Lerp(-mapScale, mapScale, lerp.x),
            playerCameraTranform.position.y,
            Mathf.Lerp(-mapScale, mapScale, lerp.y));

        playerCameraTranform.position = newCameraPos + new Vector3(0f, 0f, offset);
    }
}
