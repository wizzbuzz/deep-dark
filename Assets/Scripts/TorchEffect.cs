using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

public class TorchEffect : MonoBehaviour
{
    [Range(0f, 1f)]
    [SerializeField]
    private float minTime, maxTime;
    [Range(0f, 10f)]
    [SerializeField]
    private float minLight, maxLight;
    private new Light light;
    [SerializeField] private bool fadeIn = false;
    [SerializeField] private float fade = 1;
    [SerializeField] private float fadeSpeed = .01f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        light = GetComponentInChildren<Light>();
        light.intensity = 0;
        fade = fadeIn ? -.5f : 1;
        StartCoroutine(ChangeLight());
    }

    IEnumerator ChangeLight()
    {
        while (true)
        {
            float newIntensity = Random.Range(minLight, maxLight);
            newIntensity *= Math.Clamp(fade, 0, 10000);
            if (fadeIn && fade < 1)
            {
                fade += fadeSpeed;
            }
            light.intensity = newIntensity;
            yield return new WaitForSeconds(Random.Range(minTime, maxTime));
        }
    }
}
