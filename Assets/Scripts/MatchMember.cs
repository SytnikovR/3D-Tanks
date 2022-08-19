using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;

[System.Serializable]
public class MatchMemberData
{
    public int Id;
    public string Nickname;
    public int TeamId;
    public NetworkIdentity Member;

    public MatchMemberData(int id, string nickname, int teamId, NetworkIdentity member)
    {
        Id = id;
        Nickname = nickname;
        TeamId = teamId;
        Member = member;
    }
}

public static class MatchMemberDataEctention
{
    public static void WriteMatchMemberData(this NetworkWriter writer, MatchMemberData value)
    {
        writer.WriteInt(value.Id);
        writer.WriteString(value.Nickname);
        writer.WriteInt(value.TeamId);
        writer.WriteNetworkIdentity(value.Member);
    }

    public static MatchMemberData ReadMatchMemberData(this NetworkReader reader)
    {
        return new MatchMemberData(reader.ReadInt(), reader.ReadString(), reader.ReadInt(), reader.ReadNetworkIdentity());
    }
}
public class MatchMember : NetworkBehaviour
{
    public static event UnityAction<MatchMember, int> ChangeFrags;

    public Vehicle ActiveVehicle { get; set; }

    #region Data

    protected MatchMemberData data;
    public MatchMemberData Data => data;

    [Command]
    protected void CmdUpdateData(MatchMemberData data)
    {
        this.data = data;
    }

    #endregion

    #region Frags

    [SyncVar(hook = nameof(OnFragsChanged))]
    protected int fragsAmount;

    [Server]

    public void SvAddFrags()
    {
        fragsAmount++;
        ChangeFrags?.Invoke(this, fragsAmount);
    }

    [Server]
    public void SvResetFrags()
    {
        fragsAmount = 0;
    }

    private void OnFragsChanged(int old, int newVal)
    {
        ChangeFrags?.Invoke(this, newVal);
    }

    #endregion

    #region Nickname
    [SyncVar(hook = nameof(OnNicknameChanged))]
    protected string nickname;

    public string Nickname => nickname;

    [Command]
    protected void CmdSetName(string name)
    {
        nickname = name;
        gameObject.name = name;
    }
    private void OnNicknameChanged(string old, string newVal)
    {
        gameObject.name = newVal;
    }
    #endregion

    #region TeamId
    [SyncVar]
    protected int teamId;
    public int TeamId => teamId;

    #endregion
}
