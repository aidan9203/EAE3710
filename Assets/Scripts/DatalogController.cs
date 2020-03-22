using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;
using TMPro;

public class DatalogController : MonoBehaviour
{
    [TextArea(3, 4)]
    public string[] sentences;
    // Testing different styles of text management
    public bool listens = false;
    public TextMeshProUGUI messageTextUI;
    public GameObject notificationText;
    public GameObject panelUI;
    public PostProcessVolume ppVolume;
    
    private float aperture = 0.05f;
    private float focalLength = 300f;
    private float blurTime = 2f;
    private bool triggered = false;
    private int currentSentenceIndex = 0;

    PostProcessVolume blurVolume;

    // Potentially to freeze the game while this is being printed
    public UnityEvent datalogTriggered;

    private void Start() {
        var dof = ScriptableObject.CreateInstance<DepthOfField>();
        dof.aperture.Override(aperture);
        dof.focalLength.Override(focalLength);
        dof.enabled.Override(true);

        blurVolume = PostProcessManager.instance.QuickVolume(0, 0, dof);
        blurVolume.weight = 0;
    }

    private void Update() {
        if(triggered) {
            // Listen for 'e' presses while the game is paused
            if(Input.GetKeyDown(KeyCode.E)) {
                if(currentSentenceIndex == 0) {
                    DisplayFirstSentence();
                }
                else {
                    DisplayNextSentence();
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Player")) {
            notificationText.SetActive(true);
        }
    }

    private void OnTriggerStay(Collider other) {
        if(Input.GetKeyDown(KeyCode.E) && other.CompareTag("Player")) {
            if(!triggered) {
                // No need to display the sentence here, Update will call and handle it
                triggered = true;
                Time.timeScale = 0;
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
            panelUI.SetActive(true);

            messageTextUI.text = sentences[currentSentenceIndex];
            currentSentenceIndex++;
        }
        else {
            Debug.LogWarning("There are no sentences for this trigger. Consider adding one.");
        }
        
        // ppVolume = blurVolume;
        // Blurs scene
        // LeanTween.value(gameObject, TweenCallback, 0f, 1f, blurTime
        // Unblurs scene
        //LeanTween.value(gameObject, TweenCallback, 1f, 0f, blurTime);
    }

    private void DisplayNextSentence() {
        if(currentSentenceIndex < sentences.Length) {
            messageTextUI.text = sentences[currentSentenceIndex];
            currentSentenceIndex++;
        }
        else {
            // Hide UI elements and reset the state
            Time.timeScale = 1;
            messageTextUI.text = "";
            panelUI.SetActive(false);
            currentSentenceIndex = 0;
            triggered = false;
        }
    }

    void TweenCallback(float newWeight) {
        blurVolume.weight = newWeight;
    }
}
