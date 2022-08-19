using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;

public class Ammunition : NetworkBehaviour
{
    public event UnityAction<int> AmmoCountChanged;

    [SerializeField] protected ProjectileProperties projectileProp;

    [SyncVar(hook = nameof(SyncAmmoCount))]
    [SerializeField] protected int syncAmmoCount;

    public ProjectileProperties ProjectileProp => projectileProp;
    public int AmmoCount => syncAmmoCount;

    #region Server

    [Server]
    public void SvAddAmmo(int count)
    {
        syncAmmoCount += count;
    }

    [Server]
    public bool SvDrawAmmo(int count)
    {
        if (syncAmmoCount == 0)
            return false;

        if(syncAmmoCount >= count)
        {
            syncAmmoCount -= count;
            return true;
        }
        return false;
    }
    #endregion

    #region Client
    private void SyncAmmoCount(int old, int newVal)
    {
        AmmoCountChanged?.Invoke(newVal);
    }

    #endregion
}
