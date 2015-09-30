using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Manage player list
/// </summary>
[NetworkSettings(channel=0, sendInterval=2f)]
public class PlayerManager : NetworkBehaviour {

    [Serializable]
    public struct PlayerInfo
    {
        public string name;
        public short rtt;

        public int hostID;
        public int connID;
    };

    public class SyncListPlayer : SyncListStruct<PlayerInfo> { }
    public SyncListPlayer players = new SyncListPlayer();

    public void AddPlayer(int connID, int hostID)
    {
        PlayerInfo player = new PlayerInfo();

        player.name = "?";
        player.rtt = 999;
        player.connID = connID;
        player.hostID = hostID;

        players.Add(player);
    }

    public int GetPlayerIndex(int connID, int hostID)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].connID == connID && players[i].hostID == hostID)
            {
                return i;
            }
        }

        return -1;
    }

    public PlayerInfo GetPlayer(int connID, int hostID) 
    {
        int index = GetPlayerIndex(connID, hostID);
        if (index == -1)
        {
            throw new Exception("Can't find player with connID " + connID);
        }
        return players[index];
    }

    public void RemovePlayer(int connID, int hostID)
    {
        players.Remove(GetPlayer(connID, hostID));
    }

    void Update()
    {
        //TODO update ping
    }
	
}
