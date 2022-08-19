using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHitResultPopup : MonoBehaviour
{
    [SerializeField] private Text type;
    [SerializeField] private Text damage;

    public void SetTypeResult(string textResult)
    {
        type.text = textResult;
    }

    public void SetDamageResult(float dmg)
    {
        if (dmg <= 0) return;

        damage.text = "-" + dmg.ToString("F0");
    }
}
