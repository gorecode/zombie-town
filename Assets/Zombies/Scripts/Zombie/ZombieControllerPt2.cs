using UnityEngine;
using System.Collections;
using RootMotion.FinalIK;
using RootMotion.Demos;

[System.Serializable]
public class RandomSoundDescriptor
{
    public AudioClip[] audio;
    public float minDelay;
    public float maxDelay;
}

[System.Serializable]
public class LootDescriptor
{
    public GameObject prefab;
    public float prohability;
}

public class ZombieControllerPt2 : MonoBehaviour
{
    public static int killed;

    private AudioSource audioSource;
    Animator avatar;
    LookAtIK lookAtIK;
    private FullBodyBipedIK fullbodyIk;
    private HitReaction hitReaction;
    private Transform ground;
    private ZombieControllerPt1 follow;
    public bool stickToGround = true;
    public RandomSoundDescriptor idlingSounds;
    public RandomSoundDescriptor attackSounds;
    public GameObject bulletHitPrefab;
    private bool dead;
    public Transform leftHandFallback;
    public Transform rightHandFallback;

    public LootDescriptor[] loot;

    public void Start()
    {
        GamePlayUtils.Ragdollify(transform, false);

        ground = transform.Find("Ground");
        audioSource = GetComponent<AudioSource>();
        avatar = GetComponent<Animator>();
        lookAtIK = GetComponent<LookAtIK>();
        fullbodyIk = GetComponent<FullBodyBipedIK>();
        hitReaction = GetComponent<HitReaction>();
        follow = GetComponent<ZombieControllerPt1>();

        StartCoroutine(IdlingSoundsCoroutine());
    }

    public void AllowJumping(int allow)
    {
        AllowJumping(allow == 1);
    }

    public void AllowJumping(bool allow)
    {
        stickToGround = !allow;
    }

    public void Die()
    {
        if (dead)
            return;
        killed++;
        dead = true;
        avatar.enabled = false;
        GetComponent<ZombieControllerPt1>().enabled = false;
        Destroy(GetComponent<CharacterController>());
        Destroy(GetComponent<Rigidbody>());
        Destroy(GetComponent<GrounderBipedIK>());
        Destroy(GetComponent<ZombieDamageHandler>());
        lookAtIK.enabled = false;
        fullbodyIk.enabled = false;
        enabled = false;
        StopAllCoroutines();
        GamePlayUtils.Ragdollify(transform, true);
        SpawnLoot();
        Invoke("DisableRagdoll", 3f);
        Invoke("DestroySelf", 30f);
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    public void DisableRagdoll()
    {
        Debug.Log("Disabling Ragdoll");

        Destroy(GetComponent<FliesSystem>());

        Rigidbody[] rs = GetComponentsInChildren<Rigidbody>();
        Collider[] col = GetComponentsInChildren<Collider>();
        Joint[] js = GetComponentsInChildren<Joint>();

        for (int i = 0; i < js.Length; i++)
        {
            GameObject.Destroy(js[i]);
        }

        for (int i = 0; i < rs.Length; i++)
        {
            GameObject.Destroy(rs[i]);
        }
        for (int i = 0; i < col.Length; i++)
        {
            GameObject.Destroy(col[i]);
        }

    }

    public void SpawnLoot()
    {
        for (int i = 0; i < loot.Length; i++)
        {
            if (loot[i].prohability >= UnityEngine.Random.Range(0, 1f))
            {
                Vector3 position = transform.position;
                position.y += 1f;
                GameObject go = GameObject.Instantiate(loot[i].prefab, position, Quaternion.identity) as GameObject;
                go.SetActive(true);
            }
        }
    }

    public void BulletHit(BulletHitInfo info)
    {
        if (dead)
            return;

        if (follow.GetState() == ZombieControllerPt1.State.IDLE)
        {
            follow.ForceHunt();
        }

        if (bulletHitPrefab != null)
        {
            RaycastHit hit = info.hit;
            GameObject.Instantiate(bulletHitPrefab, hit.point - info.ray.direction * 0.02f, Quaternion.LookRotation(-info.ray.direction));

        }
        if (hitReaction != null)
        {
            RaycastHit hit = info.hit;
            Ray ray = info.ray;
            hitReaction.Hit(hit.collider, ray.direction * 2f, info.hit.point);
        }
    }

    public IEnumerator IdlingSoundsCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(idlingSounds.minDelay, idlingSounds.maxDelay));

            if (follow.GetState() != ZombieControllerPt1.State.IDLE)
            {
                continue;
            }

            AudioClip[] clips = idlingSounds.audio;

            if (clips != null && clips.Length > 0)
            {
                AudioClip clip = clips[Random.Range(0, clips.Length)];

                PlaySound(clip);
            }
        }
    }

    public void Scream(int enabled)
    {
        Transform head = avatar.GetBoneTransform(HumanBodyBones.Head);
        Transform scream = head.Find("Scream");
        scream.gameObject.SetActive(enabled == 1);
        if (enabled == 1)
        {
            ParticleSystem ps = scream.GetComponentInChildren<ParticleSystem>(true);
            ps.Clear();
            ps.Play();
        }
    }

    public void EnableMeleeAttack(string name)
    {
        EnableMeleeAttack(name, true);
    }

    public void DisableMeleeAttack(string name)
    {
        EnableMeleeAttack(name, false);
    }

    public void EnableMeleeAttack(string name, bool enabled)
    {
        Transform limb = null;
        if (name.Equals("RightHand"))
        {
            limb = avatar.GetBoneTransform(HumanBodyBones.RightHand);
            if (limb == null)
            {
                limb = rightHandFallback;
            }
        }
        else if (name.Equals("LeftHand"))
        {
            limb = avatar.GetBoneTransform(HumanBodyBones.LeftHand);
            if (limb == null)
            {
                limb = leftHandFallback;
            }
        }

        if (limb == null)
        {
            Debug.LogError("Unable to find limb for " + limb + " game object");
            return;
        }

        Transform damage = limb.Find("Damage");
        damage.gameObject.SetActive(enabled);
    }

    public void EnableMeleeAttack1()
    {
        EnableMeleeAttack("RightHand", true);
    }

    public void DisableMeleeAttack1()
    {
        EnableMeleeAttack("RightHand", false);
    }

    public void PlaySound(AudioClip clip)
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
        audioSource.clip = clip;
        audioSource.Play();
    }

    void LateUpdate()
    {
        if (stickToGround)
        {
            Vector3 down = -ground.up;

            RaycastHit hit;

            int layerMask = 1 << vp_Layer.Default;

            Ray ray = new Ray(ground.position, down);

            if (Physics.Raycast(ray, out hit, 100f, layerMask))
            {
                Vector3 position = transform.position;
                position.y -= hit.distance;
                transform.position = position;
            }
            else
            {
            }
        }
    }
}
