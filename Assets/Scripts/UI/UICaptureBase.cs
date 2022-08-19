using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICaptureBase : MonoBehaviour
{
    [SerializeField] private ConditionCaptureBase conditionCaptureBase;

    [SerializeField] private Slider localTeamSlider;
    [SerializeField] private Slider otherTeamSlider;

    private void Update()
    {
        if (Player.Local == null) return;

        if(Player.Local.TeamId == TeamSide.TeamRed)
        {
            UpdateSlider(localTeamSlider, conditionCaptureBase.RedBaseCaptureLevel);
            UpdateSlider(otherTeamSlider, conditionCaptureBase.BlueBaseCaptureLevel);
        }

        if (Player.Local.TeamId == TeamSide.TeamBlue)
        {
            UpdateSlider(localTeamSlider, conditionCaptureBase.BlueBaseCaptureLevel);
            UpdateSlider(otherTeamSlider, conditionCaptureBase.RedBaseCaptureLevel);
        }
    }
    private void UpdateSlider(Slider slider, float value)
    {
        if (value == 0)
        {
            slider.gameObject.SetActive(false);
        }
        else
        {
            if (slider.gameObject.activeSelf == false)
                slider.gameObject.SetActive(true);

            slider.value = value;
        }
    }
}
