using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkSessionManager : NetworkManager
{
    [SerializeField] private SphereArea[] spawnZonesRed;
    [SerializeField] private SphereArea[] spawnZonesBlue;

    public Vector3 RandomSpawnPointRed => spawnZonesRed[UnityEngine.Random.Range(0, spawnZonesRed.Length)].RandomInside;
    public Vector3 RandomSpawnPointBlue => spawnZonesBlue[UnityEngine.Random.Range(0, spawnZonesBlue.Length)].RandomInside;

    public static NetworkSessionManager Instance => singleton as NetworkSessionManager;
    public static GameEventCollector Events => Instance.gameEventCollector;
    public static MatchController Match => Instance.matchController;
    public bool IsServer => (mode == NetworkManagerMode.Host || mode == NetworkManagerMode.ServerOnly);
    public bool IsClient => (mode == NetworkManagerMode.Host || mode == NetworkManagerMode.ClientOnly);

    [SerializeField] private GameEventCollector gameEventCollector;

    [SerializeField] private MatchController matchController;

    public Vector3 GetSpawnPointByTeam(int teamId)
    {
        return teamId % 2 == 0 ? RandomSpawnPointRed : RandomSpawnPointBlue;
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        gameEventCollector.SvOnAddPlayer();
    }
}
