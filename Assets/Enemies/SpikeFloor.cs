using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeFloor : MonoBehaviour
{
    public bool pressure;
    public float time_interval;

    Transform spikes;
    int engaged = 0;
    float timer = 0;
    float offset = 0;

    // Start is called before the first frame update
    void Start()
    {
        spikes = transform.GetChild(0);
        timer = offset;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (time_interval > 0)
        {
            if (timer < time_interval / 2.0f) { engaged = 0; }
            else if (timer < time_interval) { engaged = 1; }
            else { timer = 0; }
        }

        if (engaged > 0) { spikes.localPosition = Vector3.Lerp(spikes.localPosition, new Vector3(1.5f, 0.08f, -1.5f), 0.2f); }
        else { spikes.localPosition = Vector3.Lerp(spikes.localPosition, new Vector3(1.5f, -0.25f, -1.5f), 0.2f); }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (pressure)
        {
            engaged++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (pressure)
        {
            engaged--;
        }
    }
}
