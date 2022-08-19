﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleVisibilityInCamera : MonoBehaviour
{
    private List<Vehicle> vehicles = new List<Vehicle>();

    private void Start()
    {
        NetworkSessionManager.Match.MatchStart += OnMatchStart;
    }

    private void OnDestroy()
    {
        NetworkSessionManager.Match.MatchStart -= OnMatchStart;
    }

    private void OnMatchStart()
    {
        vehicles.Clear();

        Vehicle[] allVeh = FindObjectsOfType<Vehicle>();

        for (int i = 0; i < allVeh.Length; i++)
        {
            if (allVeh[i] == Player.Local.ActiveVehicle) continue;

            vehicles.Add(allVeh[i]);
        }

    }

    private void Update()
    {
        for (int i = 0; i < vehicles.Count; i++)
        {
            bool isVisible = Player.Local.ActiveVehicle.Viewer.IsVisible(vehicles[i].netIdentity);

            vehicles[i].SetVisible(isVisible);
        }
    }
}
