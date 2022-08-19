using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;

public class Bot : MatchMember
{
    [SerializeField] private Vehicle vehicle;
    public override void OnStartServer()
    {
        base.OnStartServer();

        teamId = MatchController.GetNextTeam();
        nickname = "b_" + GetRandomName();

        data = new MatchMemberData((int)netId, nickname, teamId, netIdentity);

        transform.position = NetworkSessionManager.Instance.GetSpawnPointByTeam(teamId);

        ActiveVehicle = vehicle;
        ActiveVehicle.TeamId = teamId;
        ActiveVehicle.Owner = netIdentity;
        ActiveVehicle.name = nickname;


    }

    public override void OnStopServer()
    {
        base.OnStopServer();

        MatchMemberList.Instance.SvRemovePlayer(data);


    }

    private void Start()
    {
        if(isServer == true)
        {
            MatchMemberList.Instance.SvAddPlayer(data);
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        ActiveVehicle = vehicle;
        ActiveVehicle.TeamId = teamId;
        ActiveVehicle.Owner = netIdentity;
        ActiveVehicle.name = nickname;
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
    }

    private string GetRandomName()
    {
        string[] names =
        {
            "Ахмед",
            "Абдулла",
            "Ибрагим",
            "Рамзан",
            "Иван",
            "Владимир",
            "Аскольд",
            "Афанасий",
            "Ефросий",
            "Салават",
            "Зураб",
            "Петр",
            "Александр",
            "Юлия",
            "Наталья",
            "Зарема",
            "Татьяна",
            "Иван",
            "Алексей",
            "Андрей",
            "Сергей",
            "Павел",
            "Есения",
            "Егор",
            "Евгений",
            "Алена",
            "Эльвина",
            "Эльвира",
            "Элина",
            "Георгий",
            "Камо",
            "Вадим",
            "Антон",
            "Алла",
            "Денис",
            "Николай",
            "Родион",
            "Варвара",
            "Ольга",
            "Джон"
        };
        return names[Random.Range(0, name.Length)];
    }
}
