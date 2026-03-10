using System;
using System.Collections;
using UnityEngine;

public class MonsterBreathing : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] breathClips;
    private AudioSource audioSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        
        StartCoroutine(Breath());
    }

    IEnumerator Breath()
    {
        while (true)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(4, 6));
            audioSource.PlayOneShot(breathClips[(int)UnityEngine.Random.Range(0, breathClips.Length)]);
        }
    }
}
