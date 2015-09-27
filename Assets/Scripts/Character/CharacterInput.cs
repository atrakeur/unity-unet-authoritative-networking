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

            currentInput.setInputHorizontal(Input.GetAxis("Horizontal"));
            currentInput.setInputVertical(Input.GetAxis("Vertical"));
            currentInput.setPitch(cameraAim.pitch);
            currentInput.setYaw(cameraAim.yaw);

            currentInput.inputJump = Input.GetButton("Jump");
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

        public sbyte inputHorizontal;
        public sbyte inputVertical;
        public bool inputJump;
        public short pitch;
        public short yaw;

        public void setPitch(float value)
        {
            pitch = (short)(value * 10);
        }

        public void setYaw(float value)
        {
            yaw = (short)(value * 10);
        }

        public float getPitch()
        {
            return (float)pitch / 10;
        }

        public float getYaw()
        {
            return (float)yaw / 10;
        }

        public void setInputHorizontal(float value)
        {
            inputHorizontal = (sbyte)(value * 127);
        }

        public void setInputVertical(float value)
        {
            inputVertical = (sbyte)(value * 127);
        }

        public float getInputHorizontal()
        {
            return (float)inputHorizontal / 127;
        }

        public float getInputVertical()
        {
            return (float)inputVertical / 127;
        }
    }

}
