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

    public Camera doorCam;
    private Camera playerCam;

    [SerializeField] 
    private PlayerMovement player = null;

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
        playerCam = player.camera_prefab.GetComponent<Camera>();

        if(enemyCount == 0) {
            Debug.LogWarning("The generator hasn't been assigned any enemies to monitor. This is likely because of incorrect setup.");
        }

        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();

        var canvasRef = GameObject.FindGameObjectWithTag("Canvas");
        panelUI = canvasRef.transform.Find("Panel").gameObject;
        notificationText = canvasRef.transform.Find("NotificationText").gameObject;
        messageText = canvasRef.transform.Find("Panel/TextBoxContainer/TextBg/MessageText").GetComponent<TextMeshProUGUI>();

        doorCam.enabled = false;
    }

    void OnCollisionEnter(Collision collision) {
        if(collision.gameObject.CompareTag("Player")) {
            if (enemyCount < 1 && !triggered) {
                notificationText.SetActive(true);
            }
        }
    }

    void OnCollisionStay(Collision collision) {
        // Prevents the generator from being used multiple times
        if(!triggered) {
            if (Input.GetAxisRaw("Interact") != 0 && collision.gameObject.CompareTag("Player") && enemyCount < 1) {
                triggered = true;

                player.frozen = true;
                playerCam.enabled = false;
                doorCam.enabled = true;
                LeanTween.moveLocalY(door.gameObject, 7.25f, 1.0f)
                    .setOnComplete(() => {
                        playerCam.enabled = true;
                        doorCam.enabled = false;
                        player.frozen = false;
                    });
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
