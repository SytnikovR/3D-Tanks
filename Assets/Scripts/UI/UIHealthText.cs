using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHealthText : MonoBehaviour
{
    [SerializeField] private Text text;

    private Destructible destructible;

    private void Start()
    {
        NetworkSessionManager.Events.PlayerVehicleSpawned += OnPlayerVehicleSpawned;
    }

    private void OnDestroy()
    {
        NetworkSessionManager.Events.PlayerVehicleSpawned -= OnPlayerVehicleSpawned;

        if (destructible != null)
            destructible.HitPointChanged -= OnHitPointChange;
    }

    private void OnPlayerVehicleSpawned(Vehicle vehicle)
    {
        destructible = vehicle;

        destructible.HitPointChanged += OnHitPointChange;

        text.text = destructible.HitPoint.ToString();
    }

    private void OnHitPointChange(int hitPoint)
    {
        text.text = hitPoint.ToString();
    }
}
