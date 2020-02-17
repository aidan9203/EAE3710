using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentryController : MonoBehaviour
{
    private GameObject skull;
    public float RotateSpeed = 50.0f;
    void Start() {
        skull = transform.Find("SkullPivot").gameObject;    
    }

    private void Update() {
        skull.transform.Rotate(0, RotateSpeed * Time.deltaTime, 0);
    }

    public void ChangeSkullVisiblity(bool isVisible) {
        skull.SetActive(isVisible);
    }
}
