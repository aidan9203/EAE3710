using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DialogueController : MonoBehaviour
{
    [TextArea(3, 4)]
    public string[] sentences;
    public bool listens = false;

    private void OnTriggerEnter(Collider other) {
        // Display our message when dialogue is triggered
        if(other.CompareTag("Player")) {
            Debug.Log("Player entered");
            foreach (string sentence in sentences) {
                Debug.Log(sentence);
            }
        }
    }
}
