﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPath : MonoBehaviour
{
    public static AIPath Instance;

    [SerializeField] private Transform baseRedPoint;
    [SerializeField] private Transform baseBluePoint;

    [SerializeField] private Transform[] fireRedPoint;
    [SerializeField] private Transform[] fireBluePoint;

    [SerializeField] private Transform[] patrolRedPoint;
    [SerializeField] private Transform[] patrolBluePoint;



    private void Awake()
    {
        Instance = this;
    }

    public Vector3 GetBasePoint(int teamId)
    {
        if(teamId == TeamSide.TeamRed)
        {
            return baseBluePoint.position;
        }
        if(teamId == TeamSide.TeamBlue)
        {
            return baseRedPoint.position;
        }

        return Vector3.zero;
    }

    public Vector3 GetRandomFirePoint(int teamId)
    {
        if(teamId == TeamSide.TeamBlue)
        {
            return fireBluePoint[Random.Range(0, fireBluePoint.Length)].position;
        }

        if(teamId == TeamSide.TeamRed)
        {
            return fireRedPoint[Random.Range(0, fireRedPoint.Length)].position;
        }

        return Vector3.zero;
    }
    public Vector3 GetRandomPatrolPoint(int teamId)
    {
        if(teamId == TeamSide.TeamBlue)
        {
            return patrolBluePoint[Random.Range(0, patrolBluePoint.Length)].position;
        }

        if (teamId == TeamSide.TeamRed)
        {
            return patrolRedPoint[Random.Range(0, patrolRedPoint.Length)].position;
        }

        return Vector3.zero;
    }
}
