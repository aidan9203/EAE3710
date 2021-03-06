﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LineOfSight : MonoBehaviour {
    public GameObject projectile;
    [Range(0, 1)]
    public float trackingSpeed = 0.1f;
    public float projectileSpeed = 3.0f;
    [Range(1, 5)]
    public float resetTime = 2.0f;
    [Range(0, 1)]
    public float timeBetweenShots = 0.5f;
    [Range(0, 5)]
    public float disabledLength = 2f;
    private AudioSource shotSound;

    private Transform parentTransform;
    private bool playerVisible;
    private bool disabled = false;

    public UnityEvent playerEnteredEvent;
    public UnityEvent viewResetEvent;
    

    void Start() {
        parentTransform = transform.parent;
        shotSound = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider collider) {
        if(collider.CompareTag("Player")) {
            StopAllCoroutines(); // Prevents resetting view
            playerEnteredEvent?.Invoke();
            
            playerVisible = true;
            InvokeRepeating("FireProjectiles", 0, timeBetweenShots);
        }
    }

    private void OnTriggerStay(Collider collider) {
        // Follow the player while they are in line of sight
        if (collider.CompareTag("Player")) {
            Quaternion newAngle = Quaternion.LookRotation(collider.gameObject.transform.position - parentTransform.position);
            parentTransform.rotation = Quaternion.Slerp(parentTransform.rotation, newAngle, trackingSpeed);
        }
    }

    private void OnTriggerExit(Collider other) {
        if(other.CompareTag("Player")) {
            playerVisible = false;
            CancelInvoke();
            StartCoroutine("ResetView");
        }
    }

    void FireProjectiles() {
        if (!disabled) {
            GameObject spawnedProjectile = Instantiate(projectile, parentTransform.forward + parentTransform.position, parentTransform.rotation);
            spawnedProjectile.GetComponent<Rigidbody>().velocity = spawnedProjectile.transform.forward * projectileSpeed;
            shotSound.Play();
        }  
    }

    IEnumerator ResetView() {
        yield return new WaitForSeconds(resetTime);
        if (!playerVisible) {
            viewResetEvent?.Invoke();
        }
    }

    public void Disable() {
        disabled = true;
        // Do something to indicate they are disabled


        // Reenable after the amount of time
        StartCoroutine(Enable());
    }

    IEnumerator Enable() {
        yield return new WaitForSeconds(disabledLength);

        disabled = false;
    }
}
