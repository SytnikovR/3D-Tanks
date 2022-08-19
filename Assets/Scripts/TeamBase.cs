using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TeamBase : MonoBehaviour
{
    [SerializeField] private float captureLevel;
    [SerializeField] private float captureAmountPerVehicle;
    [SerializeField] private int teamId;

    public float CaptureLevel => captureLevel;

    [SerializeField] private List<Vehicle> allVehicles = new List<Vehicle>();

    private void OnTriggerEnter(Collider other)
    {
        Vehicle v = other.transform.root.GetComponent<Vehicle>();

        if (v == null) return;
        if (v.HitPoint == 0) return;
        if (allVehicles.Contains(v) == true) return;
        if (v.Owner.GetComponent<MatchMember>().TeamId == teamId) return;
        v.HitPointChanged += OnHitPointChange;
        allVehicles.Add(v);
    }

    private void OnTriggerExit(Collider other)
    {
        Vehicle v = other.transform.root.GetComponent<Vehicle>();
        if (v == null) return;
        v.HitPointChanged -= OnHitPointChange;
        allVehicles.Remove(v);
    }

    private void OnHitPointChange(int hitpoint)
    {
        captureLevel = 0;
    }

    private void Update()
    {
        if(NetworkSessionManager.Instance.IsServer == true)
        {
            bool isAllDead = true;
            for (int i = 0; i < allVehicles.Count; i++)
            {
                if(allVehicles[i].HitPoint != 0)
                {
                    isAllDead = false;

                    captureLevel += captureAmountPerVehicle * Time.deltaTime;
                    captureLevel = Mathf.Clamp(captureLevel, 0, 100);
                }
            }

            if(allVehicles.Count == 0 || isAllDead == true)
            {
                captureLevel = 0;
            }
        }
    }

    public void Reset()
    {
        captureLevel = 0;

        for (int i = 0; i < allVehicles.Count; i++)
        {
            allVehicles[i].HitPointChanged -= OnHitPointChange;
        }

        allVehicles.Clear();
    }
}
