using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torch : MonoBehaviour
{
	float intensity;
	float timer = 0;
    // Start is called before the first frame update
    void Start()
    {
        intensity = GetComponent<Light>().intensity;
    }

    // Update is called once per frame
    void Update()
    {
		timer += Time.deltaTime;
		if (timer > 0.1f) {
			timer = 0;
			GetComponent<Light>().intensity = intensity - Random.Range(0, 0.5f);
		}
    }
}
