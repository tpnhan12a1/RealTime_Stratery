using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class UnitComandGiver : MonoBehaviour
{
    [SerializeField] private UnitSelectionHandler unitSelectionHandler = null;

    [SerializeField] private LayerMask layerMask = new LayerMask();

    private Camera mainCamera;
    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (!Mouse.current.rightButton.wasPressedThisFrame) return;

        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (!Physics.Raycast(ray, out RaycastHit hit,Mathf.Infinity,layerMask)) return;


        if (hit.collider.TryGetComponent<Targetable>(out Targetable target))
        {
            //If mine, move
            if (target.hasAuthority)
            {
                TryMove(hit.point);
                return;
            }
            //If collider is owned by someone else, set target
            

            TryTarget(target);
            return;
        }
        TryMove(hit.point);
    }

    private void TryTarget(Targetable target)
    {
        foreach (Unit seletedUnit in unitSelectionHandler.SelectedUnits)
        {
            seletedUnit.Targeter.CmdSetTarget(target.gameObject);
        }
    }

    private void TryMove(Vector3 point)
    {
        foreach (Unit seletedUnit in unitSelectionHandler.SelectedUnits)
        {
            seletedUnit.UnitMovement.CmdMove(point);
        }
    }
}
