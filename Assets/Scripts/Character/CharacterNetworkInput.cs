using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This script manage the following parts of the communication
///  - Client create input state at fixedDeltaTime rate
///  - Client simulate player at fixedDeltaTime rate
///  - Client send non ACK states every 0.33 seconds
///  - Server receive states and simulate them
///  - Server ACK states and send simulation result
///  - Client receive result (true position)
///  - Client play back all unACKed states on top of the new true state
///  - Client interpolate smoothly between predicted server state and his own state
/// </summary>
/// <remarks>
/// Updates are send on channel 1 (Unreliable)
/// </remarks>
[NetworkSettings(channel = 1, sendInterval = 0.33f)]
public class CharacterNetworkInput : NetworkBehaviour {

    private const float WARNING_CLIENT_WAITING_STATES = 30;     //How many states to keep before warning
    private const float MAX_CLIENT_WAITING_STATES = 50;         //How many states to keep on client
    private const float MAX_CLIENT_DISTANCE_WARNING = 0.25f;    //Max distance between server and localy calculated position
    private const float MAX_SERVER_DISTANCE_SNAP = 0.15f;       //Max distance between client and server calculated position before SNAPPING

    [SerializeField]
    private int localInputState = 0;    //CLIENT SIDE last sended state
    [SerializeField]
    private int clientInputState = 0;   //SERVER SIDE last received state
    [SerializeField]
    private int clientAckState = 0;     //CLIENT SIDE last ack state

    //Others characters scripts (see each script to know what it does)
    private CameraMouseAim cameraMouseAim;
    private CameraAimPoint cameraAimPoint;
    private CharacterInput characterInput;
    private CharacterMovement characterMovement;
    private CharacterRotation characterRotation;

    private float nextSendTime;                             //CLIENT SIDE time when the client must send data to server
    private Queue<CharacterInput.InputState> inputStates;   //CLIENT SIDE input states not ack by server

    private Vector3 serverLastRecvPosition;         //CLIENT SIDE last received pos from server
    private Vector3 serverLastPredPosition;         //CLIENT SIDE last predicted pos from server
    private Quaternion serverLastRecvRotation;      //CLIENT SIDE last received rot from server
    private Quaternion serverLastPredRotation;      //CLIENT SIDE last predicted rot from server

    void Start()
    { 
        inputStates = new Queue<CharacterInput.InputState>();

        cameraMouseAim = Camera.main.GetComponent<CameraMouseAim>();
        cameraAimPoint = Camera.main.GetComponent<CameraAimPoint>();

        characterInput = GetComponent<CharacterInput>();
        characterMovement = GetComponent<CharacterMovement>();
        characterRotation = GetComponent<CharacterRotation>();
    }

	void FixedUpdate () {
        //Client: Please read: http://forum.unity3d.com/threads/tips-for-server-authoritative-player-movement.199538/

        //Client: Only client run simulation in realtime for the player to see
        if (isLocalPlayer)
        {
            Profile.StartProfile("CharacterNetworkInput.FixedUpdate");
            //Client: start a new state
            localInputState = localInputState + 1;
            //Client: Updates camera
            cameraMouseAim.RunUpdate(Time.fixedDeltaTime);
            cameraAimPoint.RunUpdate(Time.fixedDeltaTime);
            //Client: gathers user input state
            characterInput.Parse(localInputState);
            //Client: add new input to the list
            inputStates.Enqueue(characterInput.currentInput);
            //Client: execute simulation on local data
            characterMovement.RunUpdate(Time.fixedDeltaTime);
            characterRotation.RunUpdate(Time.fixedDeltaTime);
            //Client: Trim commands to 25 and send commands to server
            if (inputStates.Count > WARNING_CLIENT_WAITING_STATES)
            {
                Debug.LogWarning("[NetworkInput]: States starting pulling up, are network condition bad?");
            }
            if (inputStates.Count > MAX_CLIENT_WAITING_STATES) {
                Debug.LogError("Too many waiting states, starting to drop frames"); 
            }
            while (inputStates.Count > MAX_CLIENT_WAITING_STATES) { inputStates.Dequeue(); }
            //Client: Send every sendInterval
            if (isServer && isLocalPlayer || nextSendTime < Time.time)
            {
                CmdSetServerInput(inputStates.ToArray(), transform.position);
                nextSendTime = Time.time + 0.33f;
            }
            Profile.EndProfile("CharacterNetworkInput.FixedUpdate");
        }
	}

    /// <summary>
    /// Server: Receive a list of inputs from the client
    /// </summary>
    /// <param name="newInputs"></param>
    [Command(channel = 1)]
    void CmdSetServerInput(CharacterInput.InputState[] newInputs, Vector3 newClientPos)
    {
        Profile.StartProfile("CharacterNetworkInput.CmdSetServerInput");
        int index = 0;

        //Server: Input received but state not consecutive with the last one ACKed
        if (newInputs.Length > 0 && newInputs[index].inputState > clientInputState + 1)
        {
            Debug.LogWarning("Missing inputs from " + clientInputState + " to " + newInputs[index].inputState);
        }

        //Server: Discard all old states (state already ACK from the server)
        while (index < newInputs.Length && newInputs[index].inputState <= clientInputState)
        {
            index++;
        }

        //Server: Run through all received states to execute them
        while (index < newInputs.Length)
        {
            //Server: Set the character input
            characterInput.currentInput = newInputs[index];
            //Server: Set the client state number
            clientInputState = newInputs[index].inputState;
            //Server: Run update for this step according to received input
            if (!isLocalPlayer)
            {
                characterMovement.RunUpdate(Time.fixedDeltaTime);
                characterRotation.RunUpdate(Time.fixedDeltaTime);
            }

            index++;
        }

        //Check on server that position received from client isn't too far from the position calculated locally
        //TODO: maybe add a cheat check here
        if (Vector3.Distance(newClientPos, transform.position) > MAX_CLIENT_DISTANCE_WARNING) {
            Debug.LogWarning("Client distance too far from player (maybe net condition are very bad or move code isn't deterministic)");
        }
        
        //Server: Send to other script that state update finished
        SendMessage("ServerStateReceived", clientInputState, SendMessageOptions.DontRequireReceiver);
        Profile.EndProfile("CharacterNetworkInput.CmdSetServerInput");
    }

    /// <summary>
    /// Receive a good state from the server
    /// Discard input older than this good state
    /// Replay missing inputs on top of it
    /// </summary>
    /// <param name="serverRecvState"></param>
    /// <param name="serverRecvPosition"></param>
    /// <param name="serverRecvRotation"></param>
    void ServerState(CharacterNetworkSync.CharacterState characterState)
    {
        Profile.StartProfile("CharacterNetworkInput.ServerState");

        int serverRecvState = characterState.state;
        Vector3 serverRecvPosition = characterState.position;
        Quaternion serverRecvRotation = characterState.rotation;

        //Client: Check that we received a new state from server (not some delayed packet)
        if (clientAckState < serverRecvState)
        {
            //Client: Set the last server ack state
            clientAckState = serverRecvState;

            //Client: Discard all input states where state are before the ack state
            bool loop = true;
            while (loop && inputStates.Count > 0)
            {
                CharacterInput.InputState state = inputStates.Peek();
                if (state.inputState <= clientAckState)
                {
                    inputStates.Dequeue();
                }
                else
                {
                    loop = false;
                }
            }

            //Client: store actual Character position, rotation and velocity along with current input
            CharacterInput.InputState oldState = characterInput.currentInput;
            Vector3 oldPos = transform.position;
            Quaternion oldRot = transform.rotation;

            //Client: move back the player to the received server position
            serverLastRecvPosition = serverRecvPosition;
            serverLastRecvRotation = serverRecvRotation;
            transform.position = serverLastRecvPosition;
            transform.rotation = serverLastRecvRotation;

            //Client: replay all input based on new correct position
            foreach(CharacterInput.InputState state in inputStates) 
            {
                //Set the input
                characterInput.currentInput = state;
                //Run the simulation
                characterMovement.RunUpdate(Time.fixedDeltaTime);
                characterRotation.RunUpdate(Time.fixedDeltaTime);
            }
            //Client: save the new predicted character position
            serverLastPredPosition = transform.position;
            serverLastPredRotation = transform.rotation;

            //Client: restore initial position, rotation and velocity
            characterInput.currentInput = oldState;
            transform.position = oldPos;
            transform.rotation = oldRot;

            //Client: Check if a prediction error occured in the past
            //Debug.Log("States in queue: " + inputStates.Count + " Predicted distance: " + Vector3.Distance(transform.position, serverLastPredPosition));
            if (Vector3.Distance(transform.position, serverLastPredPosition) > MAX_SERVER_DISTANCE_SNAP)
            {
                //Client: Snap to correct position
                Debug.LogWarning("Prediction error!");
                transform.position = Vector3.Lerp(transform.position, serverLastPredPosition, Time.fixedDeltaTime * 10);
                transform.rotation = Quaternion.Lerp(transform.rotation, serverLastPredRotation, Time.fixedDeltaTime * 10);
            }
        }

        Profile.EndProfile("CharacterNetworkInput.ServerState");
    }

    void OnDrawGizmos()
    {
        if (isServer)
        {

        }
        else if (isLocalPlayer)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(serverLastRecvPosition + Vector3.up, Vector3.one + Vector3.up);
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(serverLastPredPosition + Vector3.up, Vector3.one + Vector3.up);
        }
    }
}
