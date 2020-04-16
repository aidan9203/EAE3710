using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Battery : MonoBehaviour
{
    public float recharge;
    public float discharge;

    public GameObject flashlight;

    public Sprite battery0, battery1, battery2, battery3, battery4;

    Image battery;
    public static float charge = 1;
    public static Dictionary<string, float> drains = new Dictionary<string, float>();

    // Start is called before the first frame update
    void Start()
    {
        battery = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        //Update icon with charge
        if (charge == 1) { battery.sprite = battery4; }
        else if (charge > 0.75f) { battery.sprite = battery3; }
        else if (charge > 0.5f) { battery.sprite = battery2; }
        else if (charge > 0.25f) { battery.sprite = battery1; }
        else { battery.sprite = battery0; }

        if (Input.GetAxisRaw("Flashlight") > 0)
        {
            if (!drains.ContainsKey("flashlight")) { drains.Add("flashlight", 1); }
            if (charge > 0) { flashlight.SetActive(true);}
            else { flashlight.SetActive(false); }
        }
        else
        {
            if (drains.ContainsKey("flashlight")) { drains.Remove("flashlight"); }
            flashlight.SetActive(false);
        }

        if (drains.Keys.Count > 0)
        {
            charge = Mathf.Max(0, charge - GetDrainRate() * discharge * Time.deltaTime);
        }
        else
        {
            charge = Mathf.Min(1, charge + recharge * Time.deltaTime);
        }
    }

    private float GetDrainRate()
    {
        float rate = 0;
        foreach(float r in drains.Values)
        {
            rate += r;
        }
        return rate;
    }
}
