using UnityEngine;
using System.Collections;

public class BrokenLight : MonoBehaviour
{
    public float minDelay = 0.2f;
    public float maxDelay = 0.5f;

    public float nextTimeForBlinking;

    private Light l;

    void Start()
    {
        l = GetComponent<Light>();

        nextTimeForBlinking = Time.time + Random.Range(minDelay, maxDelay);
    }

    void Update()
    {
        if (Time.time > nextTimeForBlinking)
        {
            l.enabled = !l.enabled;
            nextTimeForBlinking = Time.time + Random.Range(minDelay, maxDelay);
        }
    }
}
