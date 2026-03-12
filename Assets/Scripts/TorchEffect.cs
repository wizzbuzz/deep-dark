using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

public class TorchEffect : MonoBehaviour
{
    // Time range in seconds between each light intensity change
    [Range(0f, 1f)]
    [SerializeField]
    private float minTime, maxTime;
    // Intensity range for the randomised light flicker
    [Range(0f, 10f)]
    [SerializeField]
    private float minLight, maxLight;
    // The child Light component driven by this effect
    private new Light light;
    // When true, the light gradually fades in from dark on start
    [SerializeField] private bool fadeIn = false;
    // Current fade multiplier applied to the light intensity
    [SerializeField] private float fade = 1;
    // Rate at which the fade multiplier increases each flicker step
    [SerializeField] private float fadeSpeed = .01f;


    // Initialises the light, sets the starting fade value, and begins the flicker coroutine
    void Start()
    {
        light = GetComponentInChildren<Light>();
        light.intensity = 0;
        fade = fadeIn ? -.5f : 1;
        StartCoroutine(ChangeLight());
    }

    // Randomly varies light intensity each tick, optionally ramping up the fade-in multiplier
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
