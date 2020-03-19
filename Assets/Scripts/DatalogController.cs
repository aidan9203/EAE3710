using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class DatalogController : MonoBehaviour
{
    [TextArea(3, 4)]
    public string[] sentences;
    // Testing different styles of text management
    public bool listens = false;
    public Text messageTextUI;
    
    private bool triggered = false;

    // Potentially to freeze the game while this is being printed
    public UnityEvent datalogTriggered;

    private void OnTriggerEnter(Collider other) {
        if(!listens) {
            // Display our message when dialogue is triggered
            if (other.CompareTag("Player") && !triggered) {
                triggered = true;
                datalogTriggered?.Invoke(); // Potentially could be using time.timescale(0). Discuss with group about best strategy to pause game

                DisplayMessage();

                
            }
        }
    }

    private void OnTriggerStay(Collider other) {
        if(Input.GetKeyDown(KeyCode.E) && other.CompareTag("Player")) {
            // Preventing multiple activations
            if(!triggered) {
                triggered = true;
                // Display the text
                DisplayMessage();
            }
        }
    }

    private void DisplayMessage() {
        foreach (string sentence in sentences) {
            // StopAllCoroutines();
            // StartCoroutine(TypeSentence(sentence));
            messageTextUI.text = sentence;
            messageTextUI.gameObject.SetActive(true);
            Debug.Log(sentence);
        }
    }

    // Pulled from Brackey's tutorial: https://www.youtube.com/watch?v=_nRzoTzeyxU
    private IEnumerator TypeSentence(string sentence) {
        messageTextUI.text = "";
        foreach(char letter in sentence.ToCharArray()) {
            messageTextUI.text += letter;
            yield return null;
        }
    }
}
