using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Add PlayerManagement on top of NetworkManager
/// </summary>
public class GameNetworkManager : NetworkManager {

    PlayerManager playerManager;

    void Awake()
    {
        playerManager = GetComponentInChildren<PlayerManager>();
        
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        base.OnServerAddPlayer(conn, playerControllerId);

        playerManager.AddPlayer(conn.connectionId, conn.hostId);
    }

    public override void OnServerRemovePlayer(NetworkConnection conn, PlayerController player)
    {
        playerManager.RemovePlayer(conn.connectionId, conn.hostId);

        base.OnServerRemovePlayer(conn, player);
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
    }

}
