using UnityEngine;
using System.Collections;

public class GameObjectEditorUtilities : MonoBehaviour
{
    [ExposeInEditorAttribute]
    public void AddKinematicRigidbodiesToAllChildrenContainingMeshRenderer()
    {
        MeshRenderer[] renderers = gameObject.GetComponentsInChildren<MeshRenderer>(true);

        foreach (MeshRenderer r in renderers)
        {
            GameObject go = r.gameObject;

            Rigidbody rigid = go.GetComponent<Rigidbody>();

            if (rigid == null)
            {
                rigid = go.AddComponent<Rigidbody>();
            }

            if (rigid != null)
            {
                rigid.isKinematic = true;
                rigid.useGravity = false;
            }

            MeshCollider mc = go.GetComponent<MeshCollider>();

            if (mc == null)
            {
                mc = go.AddComponent<MeshCollider>();
            }

            if (mc != null)
            {
                mc.sharedMesh = go.GetComponent<MeshFilter>().sharedMesh;
            }
        }
    }
}
