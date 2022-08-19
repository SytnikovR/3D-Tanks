using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;

public class Destructible : NetworkBehaviour
{
    public event UnityAction<int> HitPointChanged;
    public event UnityAction<Destructible> Destroyed;
    public event UnityAction<Destructible> Recovered;

    [SerializeField] private int maxHitPoint;
    [SerializeField] private UnityEvent EventDestroyed;
    [SerializeField] private UnityEvent EventRecovered;

    private int currentHitPoint;
    public int MaxHitPoint => maxHitPoint;
    public int HitPoint => currentHitPoint;


    [SyncVar(hook = nameof(SyncHitPoint))]
    private int syncCurrentHitPoint;

    #region Server
    public override void OnStartServer()
    {
        base.OnStartServer();

        syncCurrentHitPoint = maxHitPoint;
        currentHitPoint = maxHitPoint;
    }

    [Server]
    public void SvApplyDamage(int damage)
    {
        syncCurrentHitPoint -= damage;

        if (syncCurrentHitPoint <= 0)
        {
            syncCurrentHitPoint = 0;

            RpcDestroy();
        }
    }

    [Server]
    protected void SvRecovery()
    {
        syncCurrentHitPoint = maxHitPoint;
        currentHitPoint = maxHitPoint;

        RpcRecovery();
    }


    #endregion

    #region Client
    private void SyncHitPoint(int oldValue, int newValue)
    {
        currentHitPoint = newValue;

        HitPointChanged?.Invoke(newValue);
    }

    [ClientRpc]
    private void RpcDestroy()
    {
        Destroyed?.Invoke(this);
        EventDestroyed?.Invoke();
    }

    [ClientRpc]
    private void RpcRecovery()
    {
        Recovered?.Invoke(this);
        EventRecovered?.Invoke();

    }
    #endregion
}
