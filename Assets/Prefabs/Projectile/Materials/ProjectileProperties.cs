using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ProjectileType
{
    ArmorPiercing,
    HighExplosive,
    Subcaliber
}

[CreateAssetMenu]
public class ProjectileProperties : ScriptableObject
{
    [SerializeField] private ProjectileType type;

    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private Sprite icon;

    [SerializeField] private float velocity;
    [SerializeField] private float mass;
    [SerializeField] private float impactForce;

    [SerializeField] private float damage;
    [Range(0.0f, 1.0f)]
    [SerializeField] private float damageSpred;

    [SerializeField] float caliber;

    [SerializeField] private float armorPenetration;
    [Range(0.0f, 1.0f)]
    [SerializeField] private float armorPenetrationSpread;
    [Range(0.0f, 90.0f)]
    [SerializeField] private float normalizationAngle;
    [Range(0.0f, 90.0f)]
    [SerializeField] private float ricochetAngle;

    public ProjectileType Type => type;
    public Projectile ProjectilePrefab => projectilePrefab;
    public Sprite Icon => icon;
    public float Velocity => velocity;
    public float Mass => mass;

    public float ImpactForce => impactForce;

    public float Damage => damage;
    public float DamageSpred => damageSpred;

    public float Caliber => caliber;

    public float ArmorPenetration => armorPenetration;
    public float ArmorPenetrationSpread => armorPenetrationSpread;
    public float NormalizationAngle => normalizationAngle;
    public float RicochetAngle => ricochetAngle;

    public float GetSpreadDamage()
    {
        return damage * Random.Range(1 - damageSpred, 1 + damageSpred);
    }

    public float GetSpreadArmorPenetration()
    {
        return ArmorPenetration * Random.Range(1 - ArmorPenetrationSpread, 1 + ArmorPenetrationSpread);
    }
}
