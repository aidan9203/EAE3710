﻿/* README:
 * Add this script to an empty object with a trigger to load a new level when the player enters
 * next_level is the name of the level to be loaded
 * Make sure the next level has been loaded in the build settings: File > Build Settings > Add Open Scenes
 */

 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTrigger : MonoBehaviour
{
    public string next_level;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            SceneManager.LoadScene(next_level);
        }
    }
}
