using UnityEngine;

class CameraFollowTarget : MonoBehaviour
{
    private Camera mainCamera;
    public float SmoothSpeed = 0.125f;

    void Awake()
    {
        mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        Vector3 newPosition = transform.position;
        newPosition.z = mainCamera.transform.position.z; // Maintain camera's z position
        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, newPosition, SmoothSpeed);
    }
}
