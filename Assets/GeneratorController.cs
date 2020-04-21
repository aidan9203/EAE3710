using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GeneratorController : MonoBehaviour
{
    public List<GameObject> enemies;
    public Transform door;

    private TextMeshProUGUI messageText;
    private GameObject notificationText;
    private GameObject panelUI;

    private int enemyCount;
    private bool triggered;


    // Start is called before the first frame update
    void Start()
    {
        enemyCount = enemies.Count;

        if(enemyCount == 0) {
            Debug.LogWarning("The generator hasn't been assigned any enemies to monitor. This is likely because of incorrect setup.");
        }

        var canvasRef = GameObject.FindGameObjectWithTag("Canvas");
        notificationText = canvasRef.transform.Find("NotificationText").gameObject;
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

        }
    }
}
