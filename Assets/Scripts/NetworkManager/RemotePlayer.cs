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
    private GameObject characterPrefab;

    [SyncVar]
    public string displayName = "unnamed";

    [SyncVar]
    public short ping = 999;

    [SyncVar]
    public NetworkInstanceId spawnedCharacterID;

    private int hostID;
    private int connID;

    void Start()
    {
        if (isLocalPlayer)
        {
            CmdSetDisplayName("Atrakeur");
        }
    }

    void Update()
    {
        //Init some values (first frame only)
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

        //Update player ping
        if (isServer && !isLocalPlayer && Time.time > nextUpdate)
        {
            nextUpdate = Time.time + GetNetworkSendInterval();

            byte error;
            this.ping = (short)NetworkTransport.GetCurrentRtt(hostID, connID, out error);
        }

        //TODO remove spawn code
        if (Input.GetKeyUp(KeyCode.K))
        {
            CmdSpawnPlayer();
        }
    }

    /// <summary>
    /// Set player name
    /// </summary>
    /// <param name="name"></param>
    [Command]
    void CmdSetDisplayName(string name)
    {
        this.displayName = name;
    }

    /// <summary>
    /// Spawn a new character for this player
    /// </summary>
    [Command]
    void CmdSpawnPlayer()
    {
        if (ClientScene.FindLocalObject(spawnedCharacterID) == null)
        {
            var go = (GameObject)Instantiate(characterPrefab, Vector3.up, Quaternion.identity);
            NetworkServer.AddPlayerForConnection(GetComponent<NetworkIdentity>().connectionToClient, go, 1);
            spawnedCharacterID = go.GetComponent<NetworkIdentity>().netId;
        }
        else
        {
            Debug.LogWarning("Server: Can't spawn two character for the same player");
        }
    }

}
