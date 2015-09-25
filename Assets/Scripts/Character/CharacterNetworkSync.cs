using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// For local player, dispatch received position to CharacterNetworkInput
/// For distant player, do interpolation between received states
/// </summary>
[NetworkSettings(channel = 1, sendInterval = 0.33f)]
public class CharacterNetworkSync : NetworkBehaviour
{

    CharacterNetworkInterpolation networkInterpolation;     //The interpolation component

    [SyncVar]
    private CharacterState serverLastState;                 //SERVER: Store last state

	void Start () {
        networkInterpolation = GetComponent<CharacterNetworkInterpolation>();
	}
	
	/// <summary>
	/// Server: Called when a state from client was received and update finished
	/// </summary>
	/// <param name="clientInputState"></param>
    void ServerStateReceived(int clientInputState)
    {
        CharacterState state = new CharacterState();
        state.state = clientInputState;
        state.position = transform.position;
        state.rotation = transform.rotation;

        //Server: trigger the synchronisation due to SyncVar property
        serverLastState = state;

        //If server and client is local, bypass the sync and set state as ACKed
        if (isServer && isLocalPlayer)
        {
            SendMessage("ServerState", state, SendMessageOptions.DontRequireReceiver);
        }
	}

    /// <summary>
    /// Server: Serialize the state over network
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="initialState"></param>
    /// <returns></returns>
    public override bool OnSerialize(NetworkWriter writer, bool initialState)
    {
        writer.Write(serverLastState.state);
        writer.Write(serverLastState.position);
        writer.Write(serverLastState.rotation);

        return true;
    }

    /// <summary>
    /// All Clients: Deserialize the state from network
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="initialState"></param>
    public override void OnDeserialize(NetworkReader reader, bool initialState)
    {
        CharacterState state = new CharacterState();

        state.state    = reader.ReadInt32();
        state.position = reader.ReadVector3();
        state.rotation = reader.ReadQuaternion();

        //Client: Received a new state for the local player, treat it as an ACK and do reconciliation
        if (isLocalPlayer) {
            SendMessage("ServerState", state, SendMessageOptions.DontRequireReceiver);
        } else {
            //Other Clients: Received a state, treat it like a new position snapshot from authority
            if (initialState)
            {
                //Others Clients: First state, just snap to new position
                transform.position = state.position;
                transform.rotation = state.rotation;
            }
            else if (networkInterpolation != null)
            {
                //Others Clients: Interpolate between received positions
                networkInterpolation.ReceiveState(state);
            }
        }
    }

    public struct CharacterState
    {
        public int state;
        public Vector3 position;
        public Quaternion rotation;
    }
}
