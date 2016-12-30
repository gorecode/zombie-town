using UnityEngine;
using System.Collections;

public class SyncRagdoll : MonoBehaviour
{
    public Transform animator;
    public Transform ragdoll;

    private Rigidbody[] rigidbodies;
    private Transform[] animatorLimbs;
    private Transform[] ragdollLimbs;

    void Start()
    {
        ragdollLimbs = ragdoll.GetComponentsInChildren<Transform>(true);
        animatorLimbs = new Transform[ragdollLimbs.Length];
        for (int i = 0; i < ragdollLimbs.Length; i++)
        {
            string name = GetGameObjectPath(ragdollLimbs[i].gameObject, ragdoll.transform);
            animatorLimbs[i] = animator.transform.parent.Find(name);
            Debug.Log(animatorLimbs[i] + " for " + name);
        }
    }

    public static string GetGameObjectPath(GameObject obj, Transform parent)
    {
        string path = "/" + obj.name;

        while (obj.transform.parent != null && obj.transform.parent != parent)
        {
            obj = obj.transform.parent.gameObject;
            path = "/" + obj.name + path;
        }

        return path.Substring(1);
    }

    void Update()
    {
        for (int i = 0; i < animatorLimbs.Length; i++)
        {
            if (animatorLimbs[i] == null)
                continue;
            
            ragdollLimbs[i].rotation = animatorLimbs[i].rotation;
            ragdollLimbs[i].localPosition = animatorLimbs[i].localPosition;
        }
    }
}
