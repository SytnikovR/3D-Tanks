using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHealthSlider : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Image sliderImage;

    [SerializeField] private Color localTeamColor;
    [SerializeField] private Color otherTeamColor;

    private Destructible destructible;

    public void Init(Destructible destructible, int destructibleTeamId, int localPlayerTeamId)
    {
        this.destructible = destructible;

        destructible.HitPointChanged += OnHitPointChange;
        slider.maxValue = destructible.MaxHitPoint;
        slider.value = slider.maxValue;  
        
        if(localPlayerTeamId == destructibleTeamId)
        {
            SetLocalColor();
        }
        else
        {
            SetOtherColor();
        }

    }

    private void OnDestroy()
    {
        if (destructible == null) return;

        destructible.HitPointChanged -= OnHitPointChange;
    }

    private void OnHitPointChange(int hitPoint)
    {
        slider.value = hitPoint;
    }

    private void SetLocalColor()
    {
        sliderImage.color = localTeamColor;
    }

    private void SetOtherColor()
    {
        sliderImage.color = otherTeamColor;
    }
}
