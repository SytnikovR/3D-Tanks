using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Events;


public interface IMatchCondition
{
    bool IsTriggered { get; }

    void OnServerMatchStart(MatchController controller);
    void OnServerMatchEnd(MatchController controller);
}
public class MatchController : NetworkBehaviour
{
    private static int TeamIdCounter;

    public static int GetNextTeam()
    {
        return TeamIdCounter++ % 2;
    }

    public static void ResetTeamCounter()
    {
        TeamIdCounter = 1;
    }

    public event UnityAction MatchStart;
    public event UnityAction MatchEnd;

    public event UnityAction SvMatchStart;
    public event UnityAction SvMatchEnd;

    [SerializeField] private MatchMemberSpawner spawner;
    [SerializeField] private float delayAfterSpawnBeforeStartMatch = 0.5f;

    [SyncVar]
    private bool matchActive;
    public bool IsMatchActive => matchActive;

    public int WinTeamId = -1;

    private IMatchCondition[] matchConditions;

    private void Awake()
    {
        matchConditions = GetComponentsInChildren<IMatchCondition>();
    }

    private void Update()
    {
        if(isServer == true)
        {
            if(matchActive == true)
            {
                foreach (var c in matchConditions)
                {
                    if(c.IsTriggered == true)
                    {
                        SvEndMatch();
                        break;
                    }
                }
            }
        }
    }

    [Server]
    public void SvRestartMatch()
    {
        if (matchActive == true) return;

        matchActive = true;

        spawner.SvRespawnVehiclesAllMembers();

        StartCoroutine(StartEventMatchWithDelay(delayAfterSpawnBeforeStartMatch));
    }

    private IEnumerator StartEventMatchWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        foreach (var c in matchConditions)
        {
            c.OnServerMatchStart(this);
        }

        SvMatchStart?.Invoke();

        RpcMatchStart();
    }

    [Server]
    public void SvEndMatch()
    {
        foreach (var c in matchConditions)
        {
            c.OnServerMatchEnd(this);

            if(c is ConditionTeamDeathmatch)
            {
                WinTeamId = (c as ConditionTeamDeathmatch).WinTeamId;
            }

            if(c is ConditionCaptureBase)
            {
                if((c as ConditionCaptureBase).RedBaseCaptureLevel == 100)
                {
                    WinTeamId = TeamSide.TeamBlue;
                }

                if ((c as ConditionCaptureBase).BlueBaseCaptureLevel == 100)
                {
                    WinTeamId = TeamSide.TeamRed;
                }
            }
        }
        matchActive = false;
        SvMatchEnd?.Invoke();

        RpcMatchEnd(WinTeamId);
    }


    [ClientRpc]
    private void RpcMatchStart()
    {
        MatchStart?.Invoke();
    }

    [ClientRpc]
    private void RpcMatchEnd(int winTeamId)
    {
        WinTeamId = winTeamId;
        MatchEnd?.Invoke();
    }



}
