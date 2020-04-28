﻿using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    void Start() {
        Cursor.visible = true;    
    }
    public void StartClicked() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
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
}
