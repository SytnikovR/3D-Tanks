using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMatchTimer : MonoBehaviour
{
    [SerializeField] private MatchTimer timer;
    [SerializeField] private Text text;

    private void Update()
    {
        text.text = timer.TimeLeft.ToString("F0");
    }
}
