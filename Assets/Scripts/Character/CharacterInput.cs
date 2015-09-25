using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

/// <summary>
/// Fetches local input and store it
/// </summary>
/// <remarks>
/// Used as a proxy for Unity's Input class 
/// Sometimes we need to fake some inputs (when replaying old states for example)
/// </remarks>
public class CharacterInput : NetworkBehaviour {

    private CameraAimPoint cameraAim;

    public InputState currentInput;

    void Awake () 
    {
        cameraAim = Camera.main.GetComponent<CameraAimPoint>();
    }

    /// <summary>
    /// Ask to update input from Unity's Input
    /// </summary>
    public void Parse(int inputState)
    {
        if (isLocalPlayer)
        {
            currentInput.inputState = inputState;

            currentInput.inputHorizontal = Input.GetAxis("Horizontal");
            currentInput.inputVertical   = Input.GetAxis("Vertical");
            currentInput.inputJump       = Input.GetButton("Jump");
            currentInput.inputAim        = cameraAim.aimPoint;           //Get the aim from the camera
        }
    }

    /// <summary>
    /// Internal an clean struct to store and transmit inout states
    /// </summary>
    /// <remarks>
    /// Maybe we don't need float precision for inputVertical or Horizontal
    /// We can store it in short or ints by multipling and rounding values.
    /// Just make sure to do it here so client and server simulate using the same rounded values
    /// </remarks>
    [System.Serializable]
    public struct InputState
    {
        public int inputState;

        public float inputHorizontal;
        public float inputVertical;
        public bool inputJump;
        public Vector3 inputAim;
    }

}
