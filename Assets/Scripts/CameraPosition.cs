using UnityEngine;

public class CameraPosition : MonoBehaviour
{
    public Transform cameraPos;

    // Sync this object's position and rotation to the target every frame
    void Update()
    {
        transform.position = cameraPos.position;
        transform.rotation = cameraPos.rotation;
    }
}
