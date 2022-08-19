using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIResultPanel : MonoBehaviour
{
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private Text resultText;

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
        resultPanel.SetActive(false);
    }

    private void OnMatchEnd()
    {
        resultPanel.SetActive(true);
        int winTeamId = NetworkSessionManager.Match.WinTeamId;

        if(winTeamId == -1)
        {
            resultText.text = "Ничья!";
            return;
        }

        if(winTeamId == Player.Local.TeamId)
        {
            resultText.text = "Победа";
        }
        else
        {
            resultText.text = "Поражение";
        }
    }
}