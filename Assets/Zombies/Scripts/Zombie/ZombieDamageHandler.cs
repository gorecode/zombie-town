using UnityEngine;
using System.Collections;

public class ZombieDamageHandler : vp_DamageHandler
{
    public override void Damage(vp_DamageInfo damageInfo)
    {
        if (damageInfo.hit.collider.name.Equals("Head"))
        {
            damageInfo.Damage *= 3f;
        }
        base.Damage(damageInfo);
    }

    public override void Die()
    {
        GetComponentInParent<ZombieControllerPt2>().Die();
    }
}
