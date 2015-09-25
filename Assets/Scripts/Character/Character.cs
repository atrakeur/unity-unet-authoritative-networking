using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

/// <summary>
/// Central Character script for simple behavior common to all characters
/// </summary>
[RequireComponent(typeof(NetworkIdentity))]
public class Character : NetworkBehaviour {

    [SerializeField]
    public GameObject cameraPointer;

    void Start()
    {
        GetComponent<CharacterMovement>().enabled = false;
        GetComponent<CharacterRotation>().enabled = false;
        if (IsLocalPlayer)
        {
            //Make the camera start following this character
            Camera.main.GetComponent<CameraDispatcher>().SetCurrentCharacterTarget(cameraPointer);
            //GetComponent<CharacterMovement>().enabled = true;
            //GetComponent<CharacterRotation>().enabled = true;
        }

        if (isServer)
        {
            GetComponent<CharacterMovement>().enabled = true;
            GetComponent<CharacterRotation>().enabled = true;
        }
    }

    public bool IsLocalPlayer
    {
        get {
            return GetComponent<NetworkIdentity>().isLocalPlayer;
        }
    }

}
