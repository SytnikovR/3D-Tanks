using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionTeamDeathmatch : MonoBehaviour, IMatchCondition
{
    private int red;
    private int blue;

    private int winTeamId = -1;
    public int WinTeamId => winTeamId;

    private bool triggered;

    bool IMatchCondition.IsTriggered => triggered;

    void IMatchCondition.OnServerMatchEnd(MatchController controller)
    {
       
    }

    void IMatchCondition.OnServerMatchStart(MatchController controller)
    {
        Reset();

        foreach (var v in FindObjectsOfType<Player>())
        {
            if(v.ActiveVehicle != null)
            {
                v.ActiveVehicle.Destroyed += OnVehicleDestroyed;
                if (v.TeamId == TeamSide.TeamRed)
                    red++;
                else
                if (v.TeamId == TeamSide.TeamBlue)
                    blue++;
            }
        }
    }

    private void OnVehicleDestroyed(Destructible dest)
    {
        Vehicle v = (dest as Vehicle);
        if (v == null) return;
        var ownerPlayer = v.Owner.GetComponent<Player>();

        if (ownerPlayer == null) return;
        switch (ownerPlayer.TeamId)
        {
            case TeamSide.TeamRed:
                red--;
                break;
            case TeamSide.TeamBlue:
                blue--;
                break;
        }

        if (red == 0)
        {
            winTeamId = 1;
            triggered = true;
        }
        else
        {
            if(blue == 0)
            {
                winTeamId = 0;
                triggered = true;
            }
        }
    }

    private void Reset()
    {
        red = 0;
        blue = 0;
        triggered = false;
    }
}
