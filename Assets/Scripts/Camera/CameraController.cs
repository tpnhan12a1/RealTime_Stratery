using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;
using System;

public class CameraController : NetworkBehaviour
{
    [SerializeField] private Transform cameraTransform = null;
    [SerializeField] private float speed = 20f;
    [SerializeField] private float screenBorderThickness = 10f;
    [SerializeField] private Vector2 screenXLimits = Vector2.zero;
    [SerializeField] private Vector2 screenZLimits = Vector2.zero;

    private Vector2 previousInput;
    private Controls controls;

    public override void OnStartAuthority()
    {
        cameraTransform.gameObject.SetActive(true);
        controls = new Controls();
        controls.Player.CameraMovement.performed += SetPreviousInput;
        controls.Player.CameraMovement.canceled += SetPreviousInput;
        controls.Enable();
    }

    [ClientCallback]
    private void Update()
    {
        if (!hasAuthority || !Application.isFocused) return;
        UpdateCameraPosition();
    }

    private void UpdateCameraPosition()
    {
       Vector3 pos = cameraTransform.position;

        if(previousInput == Vector2.zero)
        {
            Vector3 cursorMovement = Vector3.zero;

            Vector3 cursorPosition = Mouse.current.position.ReadValue();

            if(cursorPosition.y > Screen.height - screenBorderThickness)
            {
                cursorMovement.z += 1;
            }    
            else if(cursorPosition.y <= screenBorderThickness)
            {
                cursorMovement.z -= 1;
            }

            if (cursorPosition.x > Screen.width - screenBorderThickness)
            {
                cursorMovement.x += 1;
            }
            else if (cursorPosition.x <= screenBorderThickness)
            {
                cursorMovement.x -= 1;
            }
            pos += cursorMovement.normalized*speed*Time.deltaTime;
        }
        else
        {
            pos += new Vector3(previousInput.x, 0f, previousInput.y) * speed*Time.deltaTime;
        }
        pos.x = Mathf.Clamp(pos.x,screenXLimits.x,screenXLimits.y);
        pos.z = Mathf.Clamp(pos.z, screenZLimits.x, screenZLimits.y);

        cameraTransform.position = pos; 
    }

    //public override void OnStopAuthority()
    //{
    //    controls.Player.CameraMovement.performed -= SetPreviousInput;
    //    controls.Player.CameraMovement.canceled -= SetPreviousInput;
    //}
    private void SetPreviousInput(InputAction.CallbackContext context)
    {
        previousInput = context.ReadValue<Vector2>();
    }    
}
