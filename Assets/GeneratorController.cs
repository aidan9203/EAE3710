using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeneratorController : MonoBehaviour
{
    public List<GameObject> enemies;
    private GameObject notificationText;


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
            if (enemyCount == 0) {
                notificationText.SetActive(true);
            }
        }
    }

    void OnCollisionStay(Collision collision) {
        // Prevents the generator from being used multiple times
        if(!triggered) {
            if (Input.GetAxisRaw("Interact") != 0 && collision.gameObject.CompareTag("Player") && enemyCount == 0) {
                Debug.Log("Door should now be activated");
                triggered = true;
                // TODO: Trigger door activation
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
    }
}
