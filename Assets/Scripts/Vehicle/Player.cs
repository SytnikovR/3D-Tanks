﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;

public class Player : MatchMember
{
    public event UnityAction<Vehicle> VehicleSpawned;

    public event UnityAction<ProjectileHitResult> ProjectileHit;

    public static Player Local
    { 
        get
        {
            var x = NetworkClient.localPlayer;

            if (x != null)
                return x.GetComponent<Player>();

            return null;            
        }
    }

    [SerializeField] private Vehicle vehiclePrefab;
    [SerializeField] private VehicleInputControl vehicleInputControl;


    [Server]
    public void SvInvokeProjectileHit(ProjectileHitResult hitResult)
    {
        ProjectileHit?.Invoke(hitResult);
        RpcInvokeProjectileHit(hitResult.Type, hitResult.Damage, hitResult.Point);
    }

    [ClientRpc]

    public void RpcInvokeProjectileHit(ProjectileHitType type, float damage, Vector3 hitPoint)
    {
        ProjectileHitResult hitResult = new ProjectileHitResult();
        hitResult.Damage = damage;
        hitResult.Type = type;
        hitResult.Point = hitPoint;

        ProjectileHit?.Invoke(hitResult);
    }

    private void Start()
    {
        vehicleInputControl.enabled = false;
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        teamId = MatchController.GetNextTeam();
    }

    public override void OnStopServer()
    {
        base.OnStopServer();

        MatchMemberList.Instance.SvRemovePlayer(data);
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if(hasAuthority == true)
        {
            CmdSetName(NetworkSessionManager.Instance.GetComponent<NetworkManagerHUD>().PlayerNickname);

            NetworkSessionManager.Match.MatchEnd += OnMatchEnd;
            NetworkSessionManager.Match.MatchStart += OnMatchStart;

            data = new MatchMemberData((int)netId, NetworkSessionManager.Instance.GetComponent<NetworkManagerHUD>().PlayerNickname, teamId, netIdentity);

            CmdAddPlayer(Data);

            CmdUpdateData(Data);
        }
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        if (hasAuthority)
        {
            NetworkSessionManager.Match.MatchEnd -= OnMatchEnd;
        }
    }
    private void OnMatchStart()
    {
        vehicleInputControl.enabled = true;
    }

    private void OnMatchEnd()
    {
        if (ActiveVehicle != null)
        {
            ActiveVehicle.SetTargetControl(Vector3.zero);
            vehicleInputControl.enabled = false;
        }
    }

    [Command]
    private void CmdAddPlayer(MatchMemberData data)
    {
        MatchMemberList.Instance.SvAddPlayer(data);
    }
    
    private void Update()
    {
        if(isLocalPlayer == true)
        {
            if(ActiveVehicle != null)
            {
                ActiveVehicle.SetVisible(!VehicleCamera.Instance.IsZoom);
            }
        }

       if(isServer == true)
        {
            if (Input.GetKeyDown(KeyCode.F9))
            {
                NetworkSessionManager.Match.SvRestartMatch();
            }
        }

       if(hasAuthority == true)
        {
            if (Input.GetKeyDown(KeyCode.V))
            {
                if (Cursor.lockState != CursorLockMode.Locked)
                    Cursor.lockState = CursorLockMode.Locked;
                else
                    Cursor.lockState = CursorLockMode.None;
            }
        }
    }

    [Server] 
    public void SvSpawnClientVehicle()
    {
        if (ActiveVehicle != null) return;

        GameObject playerVehicle = Instantiate(vehiclePrefab.gameObject, transform.position, Quaternion.identity);

        playerVehicle.transform.position = teamId % 2 == 0 ?
            NetworkSessionManager.Instance.RandomSpawnPointRed :
            NetworkSessionManager.Instance.RandomSpawnPointBlue;


        NetworkServer.Spawn(playerVehicle, netIdentity.connectionToClient);
        ActiveVehicle = playerVehicle.GetComponent<Vehicle>();
        ActiveVehicle.Owner = netIdentity;
        ActiveVehicle.TeamId = teamId;
        
      
        RpcSetVehicle(ActiveVehicle.netIdentity);
    }

    [ClientRpc]
    private void RpcSetVehicle(NetworkIdentity vehicle)
    {
        if (vehicle == null) return;

        ActiveVehicle = vehicle.GetComponent<Vehicle>();
        ActiveVehicle.Owner = netIdentity;
        ActiveVehicle.TeamId = teamId;

        if (ActiveVehicle != null && ActiveVehicle.hasAuthority && VehicleCamera.Instance != null)
        {
            VehicleCamera.Instance.SetTarget(ActiveVehicle);
        }
        vehicleInputControl.enabled = false;

        VehicleSpawned?.Invoke(ActiveVehicle);
    }
}
