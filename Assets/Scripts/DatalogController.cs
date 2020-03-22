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
    public PostProcessVolume ppVolume;
    
    private float aperture = 0.05f;
    private float focalLength = 300f;
    private float blurTime = 2f;
    private bool triggered = false; // Prevent duplicate text boxes

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

    private void OnTriggerStay(Collider other) {
        if(Input.GetKeyDown(KeyCode.E) && other.CompareTag("Player")) {
            // Preventing multiple activations
            if(!triggered) {
                triggered = true;
                DisplayMessage();
            }
        }
    }

    private void DisplayMessage() {
        ppVolume = blurVolume;
        
        // Blurs scene
        LeanTween.value(gameObject, TweenCallback, 0f, 1f, blurTime);

        foreach (string sentence in sentences) {
            // StopAllCoroutines();
            // StartCoroutine(TypeSentence(sentence));
            messageTextUI.text = sentence;
            messageTextUI.gameObject.SetActive(true);
            Debug.Log(sentence);
        }

        // Unblurs scene
        //LeanTween.value(gameObject, TweenCallback, 1f, 0f, blurTime);
    }

    // Pulled from Brackey's tutorial: https://www.youtube.com/watch?v=_nRzoTzeyxU
    private IEnumerator TypeSentence(string sentence) {
        messageTextUI.text = "";
        foreach(char letter in sentence.ToCharArray()) {
            messageTextUI.text += letter;
            yield return null;
        }
    }

    void TweenCallback(float newWeight) {
        blurVolume.weight = newWeight;
    }
}
