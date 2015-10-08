using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

/// <summary>
/// Designate a Player
/// </summary>
[NetworkSettings(sendInterval=1, channel=0)]
public class RemotePlayer : NetworkBehaviour {

    private bool isInit = false;
    private float nextUpdate = 0;

    [SerializeField]
    private float updateInterval = 1;
    [SerializeField]
    private GameObject playerPrefab;

    [SyncVar]
    public string name = "unnamed";

    [SyncVar]
    public short ping = 999;

    public int hostID;
    public int connID;

    void Start()
    {
        if (isLocalPlayer)
        {
            CmdSetPlayerName("Atrakeur");
        }
    }

    void Update()
    {
        if (isServer && !isInit)
        {
            NetworkIdentity identity = GetComponent<NetworkIdentity>();
            if (identity.connectionToClient != null)
            {
                hostID = identity.connectionToClient.hostId;
                connID = identity.connectionToClient.connectionId;
                isInit = true;
            }
        }
        else 
        {
            isInit = true;
        }

        if (isServer && !isLocalPlayer && Time.time > nextUpdate)
        {
            nextUpdate = Time.time + updateInterval;

            byte error;
            this.ping = (short)NetworkTransport.GetCurrentRtt(hostID, connID, out error);
        }
    }

    [Command]
    void CmdSetPlayerName(string name)
    {
        this.name = name;
    }

    [Command]
    void CmdSpawnPlayer()
    {
        var go = (GameObject)Instantiate(playerPrefab, Vector3.up, Quaternion.identity);

        //TODO
    }

}
