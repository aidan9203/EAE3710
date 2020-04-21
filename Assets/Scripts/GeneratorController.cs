using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GeneratorController : MonoBehaviour
{
    public List<GameObject> enemies;
    public Transform door;
    [TextArea]
    public string textToShow;
    [Range(1, 5)]
    public float timeToFreeze = 2f;

    public Material glowMaterial;
    public MeshRenderer glow;

    private TextMeshProUGUI messageText;
    private GameObject notificationText;
    private GameObject panelUI;
    private PlayerMovement playerMovement;

    private int enemyCount;
    private bool triggered;


    // Start is called before the first frame update
    void Start()
    {
        enemyCount = enemies.Count;

        if(enemyCount == 0) {
            Debug.LogWarning("The generator hasn't been assigned any enemies to monitor. This is likely because of incorrect setup.");
        }

        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();

        var canvasRef = GameObject.FindGameObjectWithTag("Canvas");
        panelUI = canvasRef.transform.Find("Panel").gameObject;
        notificationText = canvasRef.transform.Find("NotificationText").gameObject;
        messageText = canvasRef.transform.Find("Panel/TextBoxContainer/TextBg/MessageText").GetComponent<TextMeshProUGUI>();
        
    }

    void OnCollisionEnter(Collision collision) {
        if(collision.gameObject.CompareTag("Player")) {
            if (enemyCount == 0 && !triggered) {
                notificationText.SetActive(true);
            }
        }
    }

    void OnCollisionStay(Collision collision) {
        // Prevents the generator from being used multiple times
        if(!triggered) {
            if (Input.GetAxisRaw("Interact") != 0 && collision.gameObject.CompareTag("Player") && enemyCount == 0) {
                triggered = true;
                LeanTween.moveLocalY(door.gameObject, 7.25f, 0.5f);
                notificationText.SetActive(false);
            }
        }
    }

    void OnCollisionExit(Collision collision) {
        if(collision.gameObject.CompareTag("Player")) {
            notificationText.SetActive(false);
        }
    }

    public void decrementKillCount() {
        enemyCount--;

        if(enemyCount == 0) {
            // Change material
            var oldMaterials = glow.materials;
            oldMaterials[0] = glowMaterial;
            glow.materials = oldMaterials;

            // Display message alerting player that the door is now openable
            playerMovement.frozen = true;
            panelUI.SetActive(true);
            messageText.text = textToShow;
            // Auto dismiss the text after a set time
            StartCoroutine(dismiss());
        }
    }

    private IEnumerator<WaitForSeconds> dismiss() {
        yield return new WaitForSeconds(timeToFreeze);

        messageText.text = "";
        panelUI.SetActive(false);

        playerMovement.frozen = false;
    }
}
