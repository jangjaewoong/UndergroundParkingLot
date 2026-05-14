using UnityEngine;
using System.Collections;

public class FluorescentFlicker : MonoBehaviour
{
    public Light flickerLight;
    public float normalIntensity = 4f;
    public float minInterval = 4f;
    public float maxInterval = 9f;
    public int flickerCount = 3;
    public float flickerSpeed = 0.04f;

    void Start()
    {
        if (flickerLight == null)
            flickerLight = GetComponent<Light>();

        flickerLight.intensity = normalIntensity;
        StartCoroutine(FlickerRoutine());
    }

    IEnumerator FlickerRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(
                Random.Range(minInterval, maxInterval)
            );
            yield return StartCoroutine(DoFlicker());
        }
    }

    IEnumerator DoFlicker()
    {
        int count = Random.Range(flickerCount, flickerCount + 3);

        for (int i = 0; i < count; i++)
        {
            flickerLight.intensity = 0f;
            yield return new WaitForSeconds(
                flickerSpeed * Random.Range(0.5f, 1.5f)
            );
            flickerLight.intensity = Random.Range(
                normalIntensity * 0.7f, normalIntensity
            );
            yield return new WaitForSeconds(
                flickerSpeed * Random.Range(0.5f, 2f)
            );
        }

        flickerLight.intensity = normalIntensity;
    }
}