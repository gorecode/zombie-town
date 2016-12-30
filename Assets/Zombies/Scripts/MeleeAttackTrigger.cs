using UnityEngine;
using System.Collections;

public class MeleeAttackTrigger : MonoBehaviour
{
    public float damage = 10;
    public bool earthquake = false;
    public float delayBetweenNextDamage = 1f;
    public float lastDamageTakeTime;

    void Start()
    {
	
    }
	
    // Update is called once per frame
    void Update()
    {
	
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            if (Time.time - lastDamageTakeTime <= delayBetweenNextDamage)
                return;

            lastDamageTakeTime = Time.time;

            vp_FPPlayerDamageHandler damageHandler = other.gameObject.GetComponent<vp_FPPlayerDamageHandler>();

            Debug.Log(gameObject.name + " damages " + other.name);

            damageHandler.Damage(damage);

            if (earthquake)
            {
                var player = other.gameObject.GetComponent<vp_FPPlayerEventHandler>();

                player.CameraBombShake.Send(0.05f);
            }
        }
    }
}
