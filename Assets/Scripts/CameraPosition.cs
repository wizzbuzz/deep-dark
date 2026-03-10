using UnityEngine;

public class CameraPosition : MonoBehaviour
{
    public Transform cameraPos;

    void Update()
    {
        transform.position = cameraPos.position;
        transform.rotation = cameraPos.rotation;
    }
}
