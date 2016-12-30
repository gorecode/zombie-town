using UnityEngine;
using System.Collections;

public class GamePlayUtils
{

    // Use this for initialization
    static public void Ragdollify(Transform t, bool ragdollify)
    {
        var rigids = t.GetComponentsInChildren<Rigidbody>();
        foreach (var r in rigids)
        {
            r.isKinematic = !ragdollify;
        }
    }
}
