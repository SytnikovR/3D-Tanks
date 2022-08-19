using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Events;

public class Turret : NetworkBehaviour
{
    public event UnityAction<int> UpdateSelectedAmmunation;

    public event UnityAction Shot;

    [SerializeField] protected Transform launchPoint;
    public Transform LaunchPoint => launchPoint;

    [SerializeField] public float fireRate;

    [SerializeField] protected Ammunition[] ammunition;
    public Ammunition[] Ammunition => ammunition;

    private float fireTimer;

    public ProjectileProperties SelectedProjectile => Ammunition[syncSelectedAmmunitionIndex].ProjectileProp;

    public float FireTimerNormalize => fireTimer / fireRate;

    [SyncVar]
    private int syncSelectedAmmunitionIndex;

    public int SelectedAmmunitionIndex => syncSelectedAmmunitionIndex;

    public UnityAction<double> TimerChanged;

    protected virtual void OnFire()
    {

    }

    public void SetSelectProjectile(int index)
    {
        if (hasAuthority == false) return;

        if (index < 0 || index > ammunition.Length) return;

        syncSelectedAmmunitionIndex = index;

        if(isClient == true)
        {
            CmdReloadAmmunition();
        }

        UpdateSelectedAmmunation?.Invoke(index);
    }

    public void Fire()
    {
        if (hasAuthority == false) return;

        if(isClient == true)
        {
            CmdFire();
        }
    }

    [Server]
    public void SvFire()
    {
        if (fireTimer > 0) return;

        if (ammunition[syncSelectedAmmunitionIndex].SvDrawAmmo(1) == false) return;

        OnFire();

        fireTimer = fireRate;
        RpcFire();

        Shot?.Invoke();
    }


    [Command]

    private void CmdReloadAmmunition()
    {
        fireTimer = fireRate;
    }

    [Command]

    private void CmdFire()
    {
        SvFire();
    }

    [ClientRpc]
    private void RpcFire()
    {
        if (isServer == true) return;

        fireTimer = fireRate;

        OnFire();

        Shot?.Invoke();
    }

    protected virtual void Update()
    {
        if(fireTimer > 0)
        {
            fireTimer -= Time.deltaTime;
            TimerChanged?.Invoke(fireTimer);
        }
    }



}
