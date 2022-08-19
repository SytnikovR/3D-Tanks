using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Projectile : MonoBehaviour
{
    [SerializeField] private ProjectileProperties properties;
    [SerializeField] private ProjectileMovement movement;
    [SerializeField] private ProjectileHit hit;

    [SerializeField] private GameObject visualModel;

    [SerializeField] private float delayBeforeDestory;
    [SerializeField] private float lifeTime;

    public NetworkIdentity Owner { get; set; }
    public ProjectileProperties Properties => properties;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }
       
    private void Update()
    {
        hit.Check();
        movement.Move();
        if (hit.IsHit == true)
            OnHit();
    }

    private void OnHit()
    {
        transform.position = hit.RaycastHit.point;

        if (NetworkSessionManager.Instance.IsServer == true)
        {
            ProjectileHitResult hitResult = hit.GetHitResult();

            if (hitResult.Type == ProjectileHitType.Penetration || hitResult.Type == ProjectileHitType.ModulePenetration)
            {     
                    SvTakeDamage(hitResult);

                    SvAddFrags();
            }

            if(Owner != null)
            {
                Player p = Owner.GetComponent<Player>();

                if (p != null)
                    p.SvInvokeProjectileHit(hitResult);

            }
        }

        Destroy();
    }

    private void SvTakeDamage(ProjectileHitResult hitResult)
    {
        float damage = properties.Damage;
        hit.HitArmor.Destructible.SvApplyDamage ( (int) hitResult.Damage);
    }

    private void SvAddFrags()
    {
        if (hit.HitArmor.Type == ArmorType.Module) return;
        if (hit.HitArmor.Destructible.HitPoint <= 0)
        {
            if (Owner != null)
            {
                MatchMember member = Owner.GetComponent<MatchMember>();

                if (member != null)
                {
                    member.SvAddFrags();
                }
            }
        }
    }

    private void Destroy()
    {
        visualModel.SetActive(false);
        enabled = false;

        Destroy(gameObject, delayBeforeDestory);
    }

    public void SetProperties(ProjectileProperties properties)
    {
        this.properties = properties;
    }
}
