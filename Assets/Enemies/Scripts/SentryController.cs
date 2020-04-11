using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentryController : MonoBehaviour
{
    private Quaternion initialRotation;
    private GameObject skull;
    public float SkullRotateSpeed = 50.0f;
    [Range(0, 1)]
    public float ResetViewSpeed = 0.1f;

    private bool Resetting = false;

    void Start() {
        initialRotation = transform.rotation;
        skull = transform.Find("SkullPivot").gameObject;    
    }

    private void Update() {
        skull.transform.Rotate(0, SkullRotateSpeed * Time.deltaTime, 0);

        if(Resetting) {
            transform.rotation = Quaternion.Lerp(transform.rotation, initialRotation, ResetViewSpeed);
            if(transform.rotation == initialRotation) {
                Resetting = false;
            }
        }
    }

    public void ChangeSkullVisiblity(bool isVisible) {
        skull.SetActive(isVisible);
    }

    public void ResetPosition() {
        Resetting = true;
    }

    public void CancelReset() {
        Resetting = false;
    }
}
