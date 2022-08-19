using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITankInfoCollector : MonoBehaviour
{
    [SerializeField] private Transform tankInfoPanel;

    [SerializeField] private UITankInfo tankInfoPrefab;

    private UITankInfo[] tankInfo;
    private List<Vehicle> vehicleWithoutLocal;

    private void Start()
    {
        NetworkSessionManager.Match.MatchStart += OnMatchStart;
        NetworkSessionManager.Match.MatchEnd += OnMatchEnd;
    }

    private void OnDestroy()
    {
        NetworkSessionManager.Match.MatchStart -= OnMatchStart;
        NetworkSessionManager.Match.MatchEnd -= OnMatchEnd;
    }

    private void OnMatchStart()
    {
        Vehicle[] vehicles = FindObjectsOfType<Vehicle>();
        vehicleWithoutLocal = new List<Vehicle>(vehicles.Length - 1);

        for (int i = 0; i < vehicles.Length; i++)
        {
            if (vehicles[i] == Player.Local.ActiveVehicle) continue;

            vehicleWithoutLocal.Add(vehicles[i]);
        }

        tankInfo = new UITankInfo[vehicleWithoutLocal.Count];

        for (int i = 0; i < vehicleWithoutLocal.Count; i++)
        {
            tankInfo[i] = Instantiate(tankInfoPrefab);

            tankInfo[i].SetTank(vehicleWithoutLocal[i]);
            tankInfo[i].transform.SetParent(tankInfoPanel);
        }
    }

    private void OnMatchEnd()
    {
        for (int i = 0; i < tankInfoPanel.transform.childCount; i++)
        {
            Destroy(tankInfoPanel.transform.GetChild(i).gameObject);
        }
        tankInfo = null;
    }

    private void Update()
    {
        if (tankInfo == null) return;

        for (int i = 0; i < tankInfo.Length; i++)
        {
            if (tankInfo[i] == null) continue;

            bool isVisible = Player.Local.ActiveVehicle.Viewer.IsVisible(tankInfo[i].Tank.netIdentity);

            tankInfo[i].gameObject.SetActive(isVisible);

            if (tankInfo[i].gameObject.activeSelf == false) continue;


            Vector3 screenPos = Camera.main.WorldToScreenPoint(tankInfo[i].Tank.transform.position + tankInfo[i].WorldOffset);

            if(screenPos.z > 0)
            {
                tankInfo[i].transform.position = screenPos;
            }
        }
    }
}
