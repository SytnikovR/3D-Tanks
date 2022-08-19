using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

[RequireComponent(typeof(TrackTank))]
public class TrackModule : MonoBehaviour
{
    [SerializeField] private GameObject leftTrackMesh;
    [SerializeField] private GameObject leftTrackDamageMesh;
    [SerializeField] private GameObject rightTrackMesh;
    [SerializeField] private GameObject rightTrackDamageMesh;

    [SerializeField] private VehicleModule leftTrack;
    [SerializeField] private VehicleModule rightTrack;

    private TrackTank tank;

    private void Start()
    {
        tank = GetComponent<TrackTank>();

        leftTrack.Destroyed += OnLeftTrackDestroyed;
        rightTrack.Destroyed += OnRightTrackDestroyed;

        leftTrack.Recovered += OnLeftTrackRecovered;
        rightTrack.Recovered += OnRightTrackRecovered;
    }

    private void OnDestroy()
    {
        leftTrack.Destroyed -= OnLeftTrackDestroyed;
        rightTrack.Destroyed -= OnRightTrackDestroyed;

        leftTrack.Recovered -= OnLeftTrackRecovered;
        rightTrack.Recovered -= OnRightTrackRecovered;
    }

    private void OnLeftTrackDestroyed(Destructible arg0)
    {
        ChangeActiveObjects(leftTrackMesh, leftTrackDamageMesh);

        TakeAwayMobility();
    }

    private void OnLeftTrackRecovered(Destructible arg0)
    {
        ChangeActiveObjects(leftTrackMesh, leftTrackDamageMesh);

        if(rightTrack.HitPoint > 0)
        {
            RegainMobility();
        }
    }

    private void OnRightTrackDestroyed(Destructible arg0)
    {
        ChangeActiveObjects(rightTrackMesh, rightTrackDamageMesh);

        TakeAwayMobility();
    }

    private void OnRightTrackRecovered(Destructible arg0)
    {
        ChangeActiveObjects(rightTrackMesh, rightTrackDamageMesh);

        if(leftTrack.HitPoint > 0)
        {
            RegainMobility();
        }
    }

    private void ChangeActiveObjects(GameObject a, GameObject b)
    {
        a.SetActive(b.activeSelf);
        b.SetActive(!b.activeSelf);
    }

    private void TakeAwayMobility()
    {
        tank.enabled = false;
    }

    private void RegainMobility()
    {
        tank.enabled = true;
    }




}
