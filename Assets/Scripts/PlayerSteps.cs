using System;
using System.Collections;
using UnityEngine;

public class PlayerSteps : MonoBehaviour
{
    // Collection of footstep sound clips to play randomly
    [SerializeField]
    private AudioClip[] stepsClips;
    // AudioSource used to play footstep clips
    private AudioSource audioSource;
    // Rigidbody used to check if the player is moving before playing a step
    [SerializeField]
    private Rigidbody rb;
    // Caches components and begins the footstep loop
    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        rb = gameObject.GetComponent<Rigidbody>();
        
        StartCoroutine(Step());
    }


    // Plays a random footstep clip at random intervals, silenced when the player is stationary
    IEnumerator Step()
    {
        while (true)
        {
            Debug.Log("Step");
            yield return new WaitForSeconds(UnityEngine.Random.Range(.9f, 1.2f));
            audioSource.PlayOneShot(stepsClips[(int)UnityEngine.Random.Range(0, stepsClips.Length)], rb.linearVelocity.magnitude == 0 ? 0 : 1);
        }
    }
}
