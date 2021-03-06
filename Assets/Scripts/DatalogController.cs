﻿using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class DatalogController : MonoBehaviour
{
    [TextArea(3, 4)]
    public string[] sentences;
    private TextMeshProUGUI messageText;
    private GameObject notificationText;
    private GameObject panelUI;
    
    private bool triggered = false;
    private int currentSentenceIndex = 0;
    private bool buttonHeldDown = false;
    private ParticleSystem particles;
    private PlayerMovement playerMovement;
    // Potentially to freeze the game while this is being printed
    public UnityEvent datalogTriggered;

    private void Start() {
        particles = GetComponentInChildren<ParticleSystem>();
        // Fetching our canvas elements at runtime
        var canvasRef = GameObject.FindGameObjectWithTag("Canvas");
        if(!canvasRef) {
            Debug.LogError("Cannot find an appropriate canvas. Datalogs will not work without one.");
        }
        panelUI = canvasRef.transform.Find("Panel").gameObject;
        notificationText = canvasRef.transform.Find("NotificationText").gameObject;
        messageText = canvasRef.transform.Find("Panel/TextBoxContainer/TextBg/MessageText").GetComponent<TextMeshProUGUI>();
    }

    private void Update() {
        if(triggered) {
            // Listen for 'e' presses while the game is paused
            if(Input.GetAxisRaw("Interact") != 0) {
                // I needed to implement my own "GetKeyDown" but for input axis.
                // See here: https://answers.unity.com/questions/376587/how-to-treat-inputgetaxis-as-inputgetbuttondown.html
                if (!buttonHeldDown) {
                    buttonHeldDown = true;
                    if (currentSentenceIndex == 0) {
                        DisplayFirstSentence();
                    } 
                    else {
                        DisplayNextSentence();
                    }
                }
            } 
            else {
                buttonHeldDown = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Player")) {
            playerMovement = other.GetComponent<PlayerMovement>();
            notificationText.SetActive(true);
        }
    }

    private void OnTriggerStay(Collider other) {
        if(Input.GetAxisRaw("Interact") != 0 && other.CompareTag("Player")) {
            if (!triggered) {
                // No need to display the sentence here, Update will call and handle it
                triggered = true;
                // Disable particle system after the player has viewed it once.
                // The player can continue to view the message after finishing
                if (particles != null) {
                    particles.Stop();
                }
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if(other.CompareTag("Player")) {
            notificationText.SetActive(false);
        }
    }

    private void DisplayFirstSentence() {
        if(sentences.Length > 0) {
            notificationText.SetActive(false);
            playerMovement.frozen = true;
            panelUI.SetActive(true);

            messageText.text = sentences[currentSentenceIndex];
            currentSentenceIndex++;
        }
        else {
            Debug.LogWarning("There are no sentences for this trigger. Consider adding one.");
        }
    }

    private void DisplayNextSentence() {
        if(currentSentenceIndex < sentences.Length) {
            messageText.text = sentences[currentSentenceIndex];
            currentSentenceIndex++;
        }
        else {
            // Hide UI elements and reset the state
            messageText.text = "";
            panelUI.SetActive(false);
            currentSentenceIndex = 0;
            notificationText.SetActive(true);
            // After changing input methods to use the controller, there was an issue with the OnTriggerStay
            // being called immediately after resetting the timescale, and it would be called immediately and freeze the game again.
            // To solve this, we wait half a second before allowing the message to be triggered again.
            StartCoroutine(Reset());
            playerMovement.frozen = false;
        }
    }

    private IEnumerator Reset() {
        yield return new WaitForSeconds(0.5f);
        triggered = false;
    }
}
