using UnityEngine;
using System.Collections;

public class SpawnRandomPrefab : MonoBehaviour
{
    public GameObject[] prefabs;
    public Vector3 offset;

    void Start()
    {
        int index = Random.Range(0, prefabs.Length);
        GameObject go = GameObject.Instantiate(prefabs[index], transform.position + offset, transform.rotation) as GameObject;
        go.transform.parent = transform;
    }
}
