using UnityEngine;

public class RandomStalactiteMesh : MonoBehaviour
{
    [SerializeField]
    private Mesh[] meshes;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameObject.GetComponent<MeshFilter>().mesh = meshes[Random.Range(0, meshes.Length)];
    }
}
