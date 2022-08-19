using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ProjectileHitType
{
    Penetration,
    Nopenetration,
    Ricochet,
    ModulePenetration,
    ModuleNoPenetration,
    Environment
}

public class ProjectileHitResult
{
    public ProjectileHitType Type;
    public float Damage;
    public Vector3 Point;
}

[RequireComponent(typeof(Projectile))]
public class ProjectileHit : MonoBehaviour
{
    private const float RAY_ADVANCE = 1.1f;

    private Armor hitArmor;
    private RaycastHit raycastHit;
    private bool isHit;

    public bool IsHit => isHit;
    public RaycastHit RaycastHit => raycastHit;
    public Armor HitArmor => hitArmor;

    private Projectile projectile;

    private void Awake()
    {
        projectile = GetComponent<Projectile>();
    }

    public void Check()
    {
        if (isHit == true) return;

        if (Physics.Raycast(transform.position, transform.forward, out raycastHit, projectile.Properties.Velocity * Time.deltaTime * RAY_ADVANCE))
        {
            Armor armor = raycastHit.collider.GetComponent<Armor>();

            if(armor != null)
            {
                hitArmor = armor;
            }

            isHit = true;
        }
    }

    public ProjectileHitResult GetHitResult()
    {
        ProjectileHitResult hitResult = new ProjectileHitResult();

        if(hitArmor == null)
        {
            hitResult.Type = ProjectileHitType.Environment;
            hitResult.Point = raycastHit.point;
            return hitResult;
        }

        float normalization = projectile.Properties.NormalizationAngle;

        if(projectile.Properties.Caliber > hitArmor.Thickness * 2)
        {
            normalization = (projectile.Properties.NormalizationAngle * 1.4f * projectile.Properties.Caliber) / hitArmor.Thickness;
        }

        float angle = Mathf.Abs(Vector3.SignedAngle(-projectile.transform.forward, raycastHit.normal, projectile.transform.right)) - normalization;
        float reducedArmor = hitArmor.Thickness / Mathf.Cos(angle * Mathf.Deg2Rad);
        float projectilePenetration = projectile.Properties.GetSpreadArmorPenetration();        

        if (angle > projectile.Properties.RicochetAngle && projectile.Properties.Caliber < hitArmor.Thickness * 3 && hitArmor.Type == ArmorType.Vehicle)
            hitResult.Type = ProjectileHitType.Ricochet;

        else if (projectilePenetration >= reducedArmor)
            hitResult.Type = ProjectileHitType.Penetration;          

        else if (projectilePenetration < reducedArmor)
            hitResult.Type = ProjectileHitType.Nopenetration;


        if (hitResult.Type == ProjectileHitType.Penetration)
            hitResult.Damage = projectile.Properties.GetSpreadDamage();
        else
            hitResult.Damage = 0;

        if(hitArmor.Type == ArmorType.Module)
        {
            if (hitResult.Type == ProjectileHitType.Penetration)
                hitResult.Type = ProjectileHitType.ModulePenetration;

            if (hitResult.Type == ProjectileHitType.Nopenetration)
                hitResult.Type = ProjectileHitType.ModuleNoPenetration;
        }

        hitResult.Point = raycastHit.point;      

        return hitResult;
    }
}
