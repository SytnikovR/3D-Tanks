using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;

public class GameEventCollector : NetworkBehaviour
{
    public UnityAction<Vehicle> PlayerVehicleSpawned;

    [Server]
    public void SvOnAddPlayer()
    {
        RpcOnAddPlayer();
    }

    [ClientRpc]

    public void RpcOnAddPlayer()
    {
        Player.Local.VehicleSpawned += OnPlayerVehicleSpawned;
    }

    private void OnPlayerVehicleSpawned(Vehicle vehicle)
    {
        PlayerVehicleSpawned?.Invoke(vehicle);
    }

}
