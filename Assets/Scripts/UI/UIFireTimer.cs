using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFireTimer : MonoBehaviour
{
    [SerializeField] private Text text;

    private Turret turret;

    private void Start()
    {
        NetworkSessionManager.Events.PlayerVehicleSpawned += OnPlayerVehicleSpawned;
    }

    private void OnDestroy()
    {
        NetworkSessionManager.Events.PlayerVehicleSpawned -= OnPlayerVehicleSpawned;

        if (turret != null)
            turret.TimerChanged = OnTimerChanged;
    }

    private void OnPlayerVehicleSpawned(Vehicle vehicle)
    {
        turret = vehicle.Turret;

        turret.TimerChanged += OnTimerChanged;

        text.text = turret.fireRate.ToString();
    }

    private void OnTimerChanged(double time)
    {
        if(time<= 0)
        {
            text.text = turret.fireRate.ToString();
        }
        else
        {
            text.text = time.ToString();
        }
    }
}

