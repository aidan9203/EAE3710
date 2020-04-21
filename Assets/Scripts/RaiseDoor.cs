using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaiseDoor : MonoBehaviour
{
    public Transform door;

    void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Player")) {
            // Raise door
            LeanTween.moveLocalY(door.gameObject, 7.25f, 0.5f);
        }    
    }
}
