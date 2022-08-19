using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ConditionCaptureBase : NetworkBehaviour, IMatchCondition
{
    [SerializeField] private TeamBase redBase;
    [SerializeField] private TeamBase blueBase;

    [SyncVar]
    private float redBaseCaptureLevel;
    public float RedBaseCaptureLevel => redBaseCaptureLevel;

    [SyncVar]
    private float blueBaseCaptureLevel;
    public float BlueBaseCaptureLevel => blueBaseCaptureLevel;

    private bool triggered;

    bool IMatchCondition.IsTriggered => triggered;

    void IMatchCondition.OnServerMatchEnd(MatchController controller)
    {
        enabled = false;
    }

    void IMatchCondition.OnServerMatchStart(MatchController controller)
    {
        Reset();
    }

    private void Start()
    {
        enabled = true;
    }

    private void Update()
    {
        if(isServer == true)
        {
            redBaseCaptureLevel = redBase.CaptureLevel;
            blueBaseCaptureLevel = blueBase.CaptureLevel;

            if(redBaseCaptureLevel == 100|| blueBaseCaptureLevel == 100)
            {
                triggered = true;
            }
        }
    }

    private void Reset()
    {
        redBase.Reset();
        blueBase.Reset();

        redBaseCaptureLevel = 0;
        blueBaseCaptureLevel = 0;

        triggered = false;
        enabled = true;
    }
}
