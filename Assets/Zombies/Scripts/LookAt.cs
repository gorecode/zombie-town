using UnityEngine;
using System.Collections;

public class LookAt : MonoBehaviour
{
    public Transform target;

    void Start()
    {
        if (target)
        {
            this.transform.LookAt(target);
        }
    }
}
