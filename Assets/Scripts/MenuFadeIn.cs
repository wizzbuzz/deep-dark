using UnityEngine;
using UnityEngine.UI;

public class MenuFadeIn : MonoBehaviour
{
    private float alphaFade = 1;

    // Update is called once per frame
    void Update()
    {
        GetComponent<RawImage>().material.color = new Color(0, 0, 0, Mathf.Lerp(255, 0, .05f));
    }
}
