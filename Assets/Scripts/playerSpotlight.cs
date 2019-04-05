using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerSpotlight : MonoBehaviour
{
    float noise1;
    float noise2;
    public static float intensity = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        noise1 = Mathf.PerlinNoise(Random.Range(0f, 65535f), 15f * Time.time);
        noise2 = Mathf.PerlinNoise(Random.Range(0f, 65535f), 0.1f * Time.time);

        this.GetComponent<Light>().intensity = (Mathf.Lerp(0.5f, 2.8f, noise1)+Mathf.Lerp(0.5f, 1.0f, noise2))*intensity;
    }
}
