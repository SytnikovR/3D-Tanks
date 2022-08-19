﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Vehicle))]
public class AIShooter : MonoBehaviour
{
    [SerializeField] private VehicleViewer viewer;
    [SerializeField] private Transform firePosition;

    private Vehicle vehicle;
    private Vehicle target;
    private Transform lookTranform;

    public bool HasTarget => target != null;

    private void Awake()
    {
        vehicle = GetComponent<Vehicle>();
    }

    private void Update()
    {
        LookOnTarget();
        TryFire();
    }

    public void FindTarget()
    {
        List<Vehicle> v = viewer.GetAllVisibleVehicle();

        float minDist = float.MaxValue;
        int index = -1;

        for (int i = 0; i < v.Count; i++)
        {
            if (v[i].HitPoint == 0) continue;

            if (v[i].TeamId == vehicle.TeamId) continue;

            float dist = Vector3.Distance(transform.position, v[i].transform.position);

            if(dist < minDist)
            {
                minDist = dist;
                index = i;
            }
        }

        if(index != -1)
        {
            target = v[index];

            VehicleDimensions vehicleDimensions = target.GetComponent<VehicleDimensions>();

            if (vehicleDimensions == null) return;

            lookTranform = vehicleDimensions.GetPriorityFirePoint();
        }
        else
        {
            target = null;
            lookTranform = null;
        }
    }

    private void LookOnTarget()
    {
        if (lookTranform == null) return;

        vehicle.NetAimPoint = lookTranform.position;
    }

    private void TryFire()
    {
        if (target == null) return;

        RaycastHit hit;

        if(Physics.Raycast(firePosition.position, firePosition.forward, out hit, 1000))
        {
            if(hit.collider.transform.root == target.transform.root)
            {
                vehicle.Turret.SvFire();
            }
        }
    }
}
