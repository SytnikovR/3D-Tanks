using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Vehicle : Destructible
{
    [SerializeField] protected float maxLinearVelocity;

    [SerializeField] private AudioSource engineSound;

    [SerializeField] private float enginePitchModifier;

    [SerializeField] protected Transform zoomOpticsPosition;
    public Transform ZoomOpticsPosition => zoomOpticsPosition;

    public virtual float LinearVelocity => 0;


    protected float syncLinearVelocity;

    public float NormalizedLinearVelocity
    {
        get
        {
            if (Mathf.Approximately(0, syncLinearVelocity) == true) return 0;

            return Mathf.Clamp01(syncLinearVelocity / maxLinearVelocity);
        }
    }

    public Turret Turret;

    public VehicleViewer Viewer;

    public int TeamId;

    [SyncVar]
    private Vector3 netAimPoint;

    public Vector3 NetAimPoint
    {
        get => netAimPoint;

        set
        {
            netAimPoint = value;

            if(hasAuthority == true)
                CmdSetNetAimPoint(value);

        }
    }

    [Command]
    private void CmdSetNetAimPoint(Vector3 v)
    {
        netAimPoint = v;
    }

    protected Vector3 targetInputControl;

    public void SetTargetControl (Vector3 control)
    {
        targetInputControl = control.normalized;
    }

    protected virtual void Update()
    {
        UpdateEngineSFX();
    }


    private void UpdateEngineSFX()
    {
        if(engineSound != null)
        {
            engineSound.pitch = 1.0f + enginePitchModifier * NormalizedLinearVelocity;

            engineSound.volume = 0.5f + NormalizedLinearVelocity;
        }
    }

    public void Fire()
    {
        Turret.Fire();
    }

    public void SetVisible(bool visible)
    {
        if (visible == true)
        {
            if (gameObject.layer == LayerMask.NameToLayer("Default")) return;
            SetLayerToAll("Default");
        }

        else
        {
            if (gameObject.layer == LayerMask.NameToLayer("Ignore Main Camera")) return;

            SetLayerToAll("Ignore Main Camera");
        }            
    }

    private void SetLayerToAll(string layerName)
    {
        gameObject.layer = LayerMask.NameToLayer(layerName);

        foreach (Transform t in transform.GetComponentsInChildren<Transform>())
        {
            t.gameObject.layer = LayerMask.NameToLayer(layerName);
        }
    }

    [SyncVar(hook = "T")]
    public NetworkIdentity Owner;
    private void T(NetworkIdentity oldValue, NetworkIdentity newValue)
    {

    }
}
