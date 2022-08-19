using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Player))]
public class VehicleInputControl : MonoBehaviour
{
    public const float AimDistance = 1000;
    private Player player;
    private void Awake()
    {
        player = GetComponent<Player>();
    }

    protected virtual void Update()
    {
        if (player == null) return;

        if (player.ActiveVehicle == null) return;

        if(player.hasAuthority && player.isLocalPlayer)
        {
            player.ActiveVehicle.SetTargetControl(new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Jump"), Input.GetAxis("Vertical")));

            player.ActiveVehicle.NetAimPoint = TraceAimPointWithoutPlayerVehicle(VehicleCamera.Instance.transform.position, VehicleCamera.Instance.transform.forward);

            if(Input.GetMouseButtonDown(0) == true)
            {
                player.ActiveVehicle.Fire();
            }

            if (Input.GetKeyDown(KeyCode.Alpha1) == true) player.ActiveVehicle.Turret.SetSelectProjectile(0);
            if (Input.GetKeyDown(KeyCode.Alpha2) == true) player.ActiveVehicle.Turret.SetSelectProjectile(1);
            if (Input.GetKeyDown(KeyCode.Alpha3) == true) player.ActiveVehicle.Turret.SetSelectProjectile(2);
        }
    }
    public static Vector3 TraceAimPointWithoutPlayerVehicle(Vector3 start, Vector3 direction)
    {
        Ray ray = new Ray(start, direction);

        RaycastHit[] hits = Physics.RaycastAll(ray, AimDistance);

        var t = Player.Local.ActiveVehicle;

        for (int i = hits.Length -1; i >= 0; i--)
        {
            if (hits[i].collider.isTrigger == true)
                continue;

            if (hits[i].collider.transform.root.GetComponent<Vehicle>() == t)
                continue;

            return hits[i].point;
        }       
        return ray.GetPoint(AimDistance);
    }
}
