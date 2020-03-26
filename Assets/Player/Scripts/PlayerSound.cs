using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] footsteps;
    public PlayerMovement playermovement;
     
    void Update()
    {
        if (playermovement.isWalking)
        {
            
            if (!audioSource.isPlaying)
            {
                System.Random rnd = new System.Random();
                audioSource.clip = footsteps[rnd.Next(0, footsteps.Length)];
                audioSource.pitch = Random.Range(0.8f, 1.2f);
                audioSource.Play();
            }
        }
        else
        {
            audioSource.Stop();
        }
    }
}
