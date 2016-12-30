using UnityEngine;
using System.Collections;

public class DestroyAfter : MonoBehaviour
{
    public float delay = 1f;

    void Start()
    {
        Invoke("Destroy", delay);
    }

    void Destroy()
    {
        GameObject.Destroy(gameObject);
    }
}
