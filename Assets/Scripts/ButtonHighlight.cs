using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHighlight : MonoBehaviour
{
    [SerializeField]
    private EventSystem eventSystem;

    public void MouseHover(BaseEventData data) {
        var newData = data as PointerEventData;
        var button = newData.pointerEnter.transform.parent.gameObject;

        eventSystem.SetSelectedGameObject(button);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
