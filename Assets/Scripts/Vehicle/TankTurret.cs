using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TankTurret))]
public class TankTurret : Turret
{
    private TrackTank tank;

    [SerializeField] private Transform tower;
    [SerializeField] private Transform mask;

    [SerializeField] private float horizontalRotationSpeed;
    [SerializeField] private float verticalRotationSpeed;

    [SerializeField] private float maxTopAngle;
    [SerializeField] private float maxButtonAngle;

    [SerializeField] private AudioSource fireSound;
    [SerializeField] private ParticleSystem mazzel;
    [SerializeField] private float forceRecoil;

    float maskCurrentAngle;

    private Rigidbody tankRigidbody;

    private void Start()
    {
        tank = GetComponent<TrackTank>();
        tankRigidbody = tank.GetComponent<Rigidbody>();
        maxTopAngle = -maxTopAngle;
    }

    protected override void Update()
    {
        base.Update();
        ControlTurretAim();
    }

    protected override void OnFire()
    {
        base.OnFire();

        Projectile proj = Instantiate(SelectedProjectile.ProjectilePrefab);
        proj.Owner = tank.Owner;

        proj.SetProperties(SelectedProjectile);

        proj.transform.position = launchPoint.position;

        proj.transform.forward = launchPoint.forward;



        FireSFX();
    }

    private void FireSFX()
    {
        fireSound.Play();
        mazzel.Play();
        tankRigidbody.AddForceAtPosition(-mask.forward * forceRecoil, mask.position, ForceMode.Impulse);
    }

    private void ControlTurretAim()
    {
        Vector3 lp = tower.InverseTransformPoint(tank.NetAimPoint);
        lp.y = 0;
        Vector3 lpg = tower.TransformPoint(lp);
        tower.rotation =Quaternion.RotateTowards(tower.rotation, Quaternion.LookRotation((lpg - tower.position).normalized, tower.up), horizontalRotationSpeed * Time.deltaTime);

        mask.localRotation = Quaternion.identity;

        lp = mask.InverseTransformPoint(tank.NetAimPoint);
        lp.x = 0;
        lpg = mask.TransformPoint(lp);

        float targetAngle = -Vector3.SignedAngle((lpg - mask.position).normalized, mask.forward, mask.right);
        targetAngle = Mathf.Clamp(targetAngle, maxTopAngle, maxButtonAngle);
        maskCurrentAngle = Mathf.MoveTowards(maskCurrentAngle, targetAngle, Time.deltaTime * verticalRotationSpeed);
        mask.localRotation = Quaternion.Euler(maskCurrentAngle, 0, 0);
    }
}
