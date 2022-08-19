
using UnityEngine;

public class FacingCamera : MonoBehaviour
{
    private Transform mainCameraTranform;
    private void Start()
    {
        mainCameraTranform = Camera.main.transform;
    }
    private void LateUpdate()
    {
        transform.LookAt(
            transform.position + mainCameraTranform.rotation * Vector3.forward,
            mainCameraTranform.rotation * Vector3.up
            );
    }
}
