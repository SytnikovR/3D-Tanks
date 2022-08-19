using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAmmunitionElement : MonoBehaviour
{
    [SerializeField] private Text ammoCountText;
    [SerializeField] private Image projectileIcon;
    [SerializeField] private GameObject selectionBorder;

    public void SetAmmunition(Ammunition ammunition)
    {
        projectileIcon.sprite = ammunition.ProjectileProp.Icon;
        UpdateAmmoCount(ammunition.AmmoCount);
    }

    public void UpdateAmmoCount(int count)
    {
        ammoCountText.text = count.ToString();
    }

    public void Select()
    {
        selectionBorder.SetActive(true);
    }

    public void UnSelect()
    {
        selectionBorder.SetActive(false);
    }
}
