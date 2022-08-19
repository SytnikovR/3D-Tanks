using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MatchTimer : NetworkBehaviour, IMatchCondition
{
    [SerializeField] private float matchTime;

    [SyncVar]
    private float timeLeft;
    public float TimeLeft => timeLeft;

    private bool timerEnd = false;

    bool IMatchCondition.IsTriggered => timerEnd;

    void IMatchCondition.OnServerMatchStart(MatchController controller)
    {
        Reset();
    }

    void IMatchCondition.OnServerMatchEnd(MatchController controller)
    {
        enabled = false;
    }

    private void Start()
    {
        timeLeft = matchTime;
        if(isServer == true)
        {
            enabled = false;
        }
    }

    private void Update()
    {
        if(isServer == true)
        {
            timeLeft -= Time.deltaTime;
            if(timeLeft <= 0)
            {
                timeLeft = 0;
                timerEnd = true;
            }
        }
    }

    private void Reset()
    {
        enabled = true;
        timeLeft = matchTime;
        timerEnd = false;
    }
}
