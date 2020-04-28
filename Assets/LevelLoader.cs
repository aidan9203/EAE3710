using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelLoader : MonoBehaviour
{
    public string nextLevel;
    [Range(0, 1)]
    public float dimSpeed = 0.5f;

    private TextMeshProUGUI loadingText;
    private CanvasGroup dimmer;

    void Start() {
        var canvasRef = GameObject.FindGameObjectWithTag("Canvas");
        if(!canvasRef) {
            Debug.LogError("Cannot find an appropriate canvas. Won't be able to use loading scenes. " +
                "Please add a canvas to the scene.");
        }

        loadingText = canvasRef.transform.Find("LoadingText").GetComponent<TextMeshProUGUI>();
        dimmer = canvasRef.transform.Find("Dimmer").GetComponent<CanvasGroup>();

        loadingText.gameObject.SetActive(false);
        dimmer.alpha = 0;
    }

    void OnCollisionEnter(Collision collision) {
        if(collision.gameObject.CompareTag("Player")) {
            collision.gameObject.GetComponent<PlayerMovement>().frozen = true;

            // Fade screen, then start loading text
            LeanTween.value(gameObject,
                (val) => dimmer.alpha = val,
                0, 1, dimSpeed)
                .setOnComplete(() => {
                    AsyncOperation op = SceneManager.LoadSceneAsync(nextLevel);
                    StartCoroutine(LoadingScreen(op));
                });
        }    
    }

    public IEnumerator LoadingScreen(AsyncOperation op) {
        int dotCount = 0;
        loadingText.gameObject.SetActive(true);
        while(!op.isDone) {
            yield return new WaitForSeconds(0.5f);
            string txt = loadingText.text;
            if(dotCount == 3) {
                loadingText.text = "Loading";
            }
            else {
                loadingText.text += ".";
                dotCount++;
            }
        }
    }
}
