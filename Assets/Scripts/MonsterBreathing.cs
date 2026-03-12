using System;
using System.Collections;
using UnityEngine;

public class MonsterBreathing : MonoBehaviour
{
    // Collection of breathing sound clips to play randomly
    [SerializeField]
    private AudioClip[] breathClips;
    // AudioSource used to play the breathing clips
    private AudioSource audioSource;
    // Cache the AudioSource and begin the breathing loop
    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        
        StartCoroutine(Breath());
    }

    // Plays a random breath clip at random intervals between 4 and 6 seconds, indefinitely
    IEnumerator Breath()
    {
        while (true)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(4, 6));
            audioSource.PlayOneShot(breathClips[(int)UnityEngine.Random.Range(0, breathClips.Length)]);
        }
    }
}
