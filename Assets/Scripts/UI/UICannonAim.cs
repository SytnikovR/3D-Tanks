using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICannonAim : MonoBehaviour
{
    [SerializeField] private Image aim;

    [SerializeField] private Image reloadSlider;

    private Vector3 aimPosition;

    private float FireRate;

    private void Update()
    {
        if (Player.Local == null) return;

        if (Player.Local.ActiveVehicle == null) return;

        Vehicle v = Player.Local.ActiveVehicle;

        reloadSlider.fillAmount = v.Turret.FireTimerNormalize;

        aimPosition = VehicleInputControl.TraceAimPointWithoutPlayerVehicle(v.Turret.LaunchPoint.position, v.Turret.LaunchPoint.forward);

        Vector3 result = Camera.main.WorldToScreenPoint(aimPosition);

        if(result.z > 0)
        {
            result.z = 0;
            aim.transform.position = result;
        }
    }
}
