using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    public TextMeshProUGUI loadingText;
    public CanvasGroup dimmer;

    void Start() {
        Cursor.visible = true;
        loadingText.gameObject.SetActive(false);
        dimmer.alpha = 0;
    }
    
    public void StartClicked() {
        Debug.Log("Clicked");
        var operation = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
        float startAlpha = dimmer.alpha;
        // Fade screen
        LeanTween.value(gameObject,
            (val) => dimmer.alpha = val,
            startAlpha, 1, 0.5f)
            .setOnComplete(() => {
                StartCoroutine(LoadingScreen(operation));
            }
        );
    }

    public void ControlsClicked() {
        // Don't change the ordering of scenes or else this will break
        SceneManager.LoadScene(3);
    }
    
    public void QuitClicked() {
        // Note: This will not close the game in the editor, but will once the game is built
        Application.Quit();
        // This closes the editor if it is going
        #if UNITY_EDITOR
    
        if (UnityEditor.EditorApplication.isPlaying) {
            UnityEditor.EditorApplication.isPlaying = false;
        }
        #endif
        
    }

    public void BackClicked() {
        SceneManager.LoadScene(0);
    }

    private IEnumerator LoadingScreen(AsyncOperation op) {
        int dotCount = 0;
        loadingText.gameObject.SetActive(true);
        while(!op.isDone) {
            yield return new WaitForSeconds(0.2f);
            string txt = loadingText.text;
            if(dotCount == 3) {
                loadingText.text = txt.Substring(0, txt.Length - 3);
            }
            else {
                loadingText.text += ".";
            }
        }
    }
}
