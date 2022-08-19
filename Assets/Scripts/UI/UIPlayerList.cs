using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerList : MonoBehaviour
{
    [SerializeField] private Transform localTeamPanel;
    [SerializeField] private Transform otherTeamPanel;
    [SerializeField] private UIPlayerLable playerLablePrefab;

    private List<UIPlayerLable> allPlayerLable = new List<UIPlayerLable>();

    private void Start()
    {
       MatchMemberList.UpdateList += OnUpdatePlayerList;
        Player.ChangeFrags += OnChangeFrags;
    }

    private void OnDestroy()
    {
       MatchMemberList.UpdateList += OnUpdatePlayerList;
        Player.ChangeFrags -= OnChangeFrags;
    }

    private void OnChangeFrags(MatchMember member, int frags)
    {
        for (int i = 0; i < allPlayerLable.Count; i++)
        {
            if(allPlayerLable[i].NetId == member.netId)
            {
                allPlayerLable[i].UpdateFrag(frags);
            }
        }
    }

    private void OnUpdatePlayerList(List<MatchMemberData> playerData)
    {
        for (int i = 0; i < localTeamPanel.childCount; i++)
        {
            Destroy(localTeamPanel.GetChild(i).gameObject);
        }

        for (int i = 0; i < otherTeamPanel.childCount; i++)
        {
            Destroy(otherTeamPanel.GetChild(i).gameObject);
        }

        allPlayerLable.Clear();

        for (int i = 0; i < playerData.Count; i++)
        {
            if(playerData[i].TeamId == Player.Local.TeamId)
            {
                AddPlayerLable(playerData[i], playerLablePrefab, localTeamPanel);
            }

            if (playerData[i].TeamId != Player.Local.TeamId)
            {
                AddPlayerLable(playerData[i], playerLablePrefab, otherTeamPanel);
            }
        }
    }

    private void AddPlayerLable(MatchMemberData data, UIPlayerLable playerLable, Transform parent)
    {
        UIPlayerLable uiPlayerLable = Instantiate(playerLable);
        uiPlayerLable.transform.SetParent(parent);
        uiPlayerLable.Init(data.Id, data.Nickname);
        allPlayerLable.Add(uiPlayerLable);
    }

}
