using System;
using System.Collections;
using UnityEngine;

public class PlayerSteps : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] stepsClips;
    private AudioSource audioSource;
    [SerializeField]
    private Rigidbody rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        rb = gameObject.GetComponent<Rigidbody>();
        
        StartCoroutine(Step());
    }


    IEnumerator Step()
    {
        while (true)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(.9f, 1.2f));
            audioSource.PlayOneShot(stepsClips[(int)UnityEngine.Random.Range(0, stepsClips.Length)], rb.linearVelocity.magnitude == 0 ? 0 : 1);
        }
    }
}
