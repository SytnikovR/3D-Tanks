﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(Vehicle))]
public class VehicleViewer : NetworkBehaviour
{
    private const float UPDATE_INTERVAL = 0.33f; 

    private const float X_RAY_DISTANCE = 50.0f;

    private const float BASE_EXIT_TIME_FROM_DISCOVERY = 0.17f;

    private const float CAMOUFLAGE_DISTANCE = 150.0f;

    [SerializeField] private float ViewDistance;
    [SerializeField] private Transform[] viewPoints;
    [SerializeField] private Color color;

    public List<VehicleDimensions> allVehicleDimensions = new List<VehicleDimensions>();

    public SyncList<NetworkIdentity> visibleVehicles = new SyncList<NetworkIdentity>();

    public List<float> remainingTime = new List<float>();

    private Vehicle vehicle;

    private float remainingTimeLastUpdate;

    public override void OnStartServer()
    {
        base.OnStartServer();

        vehicle = GetComponent<Vehicle>();

        NetworkSessionManager.Match.SvMatchStart += OnSvMatchStart;
    }

    public override void OnStopServer()
    {
        base.OnStopServer();

        NetworkSessionManager.Match.SvMatchStart -= OnSvMatchStart;
    }

    private void OnSvMatchStart()
    {
        color = Random.ColorHSV();

        Vehicle[] allVeh = FindObjectsOfType<Vehicle>();

        for (int i = 0; i < allVeh.Length; i++)
        {
            if (vehicle == allVeh[i]) continue;

            VehicleDimensions vd = allVeh[i].GetComponent<VehicleDimensions>();

            if (vd == null) continue;

            if (vehicle.TeamId != allVeh[i].TeamId)
                allVehicleDimensions.Add(vd);
            else
            {
                visibleVehicles.Add(vd.Vehicle.netIdentity);
                remainingTime.Add(-1);
            }
        }
    }
    private void Update()
    {
        if (isServer == false) return;

        remainingTimeLastUpdate += Time.deltaTime;
        if(remainingTimeLastUpdate >= UPDATE_INTERVAL)
        {
            for (int i = 0; i < allVehicleDimensions.Count; i++)
            {
                if (allVehicleDimensions[i].Vehicle == null) continue;

                bool IsVisible = false;

                for (int j = 0; j < viewPoints.Length; j++)
                {
                    IsVisible = CheckVisibility(viewPoints[j].position, allVehicleDimensions[i]);

                    if (IsVisible == true) break;
                }

                if (IsVisible == true && visibleVehicles.Contains(allVehicleDimensions[i].Vehicle.netIdentity) == false)
                {
                    visibleVehicles.Add(allVehicleDimensions[i].Vehicle.netIdentity);
                    remainingTime.Add(-1);
                }

                if (IsVisible == true && visibleVehicles.Contains(allVehicleDimensions[i].Vehicle.netIdentity) == true)
                {
                    remainingTime[visibleVehicles.IndexOf(allVehicleDimensions[i].Vehicle.netIdentity)] = -1;
                }


                if (IsVisible == false && visibleVehicles.Contains(allVehicleDimensions[i].Vehicle.netIdentity) == true)
                {
                    if (remainingTime[visibleVehicles.IndexOf(allVehicleDimensions[i].Vehicle.netIdentity)] == -1)
                        remainingTime[visibleVehicles.IndexOf(allVehicleDimensions[i].Vehicle.netIdentity)] = BASE_EXIT_TIME_FROM_DISCOVERY;
                }
            }

            for (int i = 0; i < remainingTime.Count; i++)
            {
                if (remainingTime[i] > 0)
                {
                    remainingTime[i] -= Time.deltaTime;
                    if (remainingTime[i] <= 0)
                        remainingTime[i] = 0;
                }

                if (remainingTime[i] == 0)
                {
                    remainingTime.RemoveAt(i);
                    visibleVehicles.RemoveAt(i);
                }
            }

            remainingTimeLastUpdate = 0;
        }
    }

    public bool IsVisible(NetworkIdentity identity)
    {
        return visibleVehicles.Contains(identity);
    }

    public List<Vehicle> GetAllVehicle()
    {
        List<Vehicle> av = new List<Vehicle>(allVehicleDimensions.Count);

        for (int i = 0; i < allVehicleDimensions.Count; i++)
        {
            av.Add(allVehicleDimensions[i].Vehicle);
        }

        return av;
    }

    public List<Vehicle> GetAllVisibleVehicle()
    {
        List<Vehicle> av = new List<Vehicle>(allVehicleDimensions.Count);

        for (int i = 0; i < visibleVehicles.Count; i++)
        {
            av.Add(visibleVehicles[i].GetComponent<Vehicle>());
        }

        return av;
    }

    private bool CheckVisibility(Vector3 viewPoint, VehicleDimensions vehicleDimensions)
    {
        float distance = Vector3.Distance(transform.position, vehicleDimensions.transform.position);

        if (Vector3.Distance(viewPoint, vehicleDimensions.transform.position) <= X_RAY_DISTANCE) return true;

        if (distance > ViewDistance) return false;

        float curViewDist = ViewDistance;

        if(distance >= CAMOUFLAGE_DISTANCE)
        {
            VehicleCamouflage vehicleCamouflage = vehicleDimensions.Vehicle.GetComponent<VehicleCamouflage>();
            if(vehicleCamouflage != null)
               curViewDist = ViewDistance - vehicleCamouflage.CurrentDistance;
        }

        if (distance > curViewDist) return false;

        return vehicleDimensions.IsVisibleFromPoint(transform.root, viewPoint, color);
    }

}
