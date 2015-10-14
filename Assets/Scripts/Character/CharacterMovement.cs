using UnityEngine;
using System.Collections;

/// <summary>
/// Manages caracter movement according to CharacterInput values
/// </summary>
public class CharacterMovement : MonoBehaviour {

    [SerializeField]
    public float speed = 6.0F;
    [SerializeField]
    public float runSpeed = 8.0F;
    [SerializeField]
    public float jumpHeight = 8.0F;
    [SerializeField]
    public float gravity = 20.0F;
    [SerializeField]
    public float gravityAccel = 9f;

    private CharacterFixedController controller;
    private CharacterInput input;
    private Vector3 moveDirection = Vector3.zero;
    private Vector3 lookDirection;

    private int lastGround;     //Represent last tick the controller touched ground

	void Awake () 
    {
        input = GetComponent<CharacterInput>();
        controller = GetComponent<CharacterFixedController>();
        lookDirection = transform.forward;
	}
	
    /// <summary>
    /// Run update like classic unity's Update
    /// We use an other method here because the calling must be controlled by CharacterNetwork
    /// We can't use standard Update method because Unity update order is non-deterministic
    /// </summary>
    /// <param name="delta"></param>
	public void RunUpdate(float delta) 
    {
        controller.DoUpdate(delta);
	}

    /// <summary>
    /// Called by the controller when wanting to update custom code data
    /// </summary>
    void SuperUpdate()
    {
        //Allways look forward
        lookDirection = transform.forward;

        //Adjust somes values
        lastGround++;
        if (AcquiringGround())
        {
            lastGround = 0;
        }

        //Calculate movement from keys
        float actualSpeed = speed;
        if (input.currentInput.inputRun)
        {
            actualSpeed = runSpeed;
        }
        Vector3 movement = Vector3.MoveTowards(moveDirection, LocalMovement() * actualSpeed, Mathf.Infinity);

        //Add jump velocity if jumping
        if (input.currentInput.inputJump && AcquiringGround())
        {
            controller.DisableClamping();
            movement.y = moveDirection.y + CalculateJumpVelocity();
        }
        //Calculate gravity acceleration toward ground
        else if (lastGround > 0)
        {
            controller.DisableClamping();
            movement.y = moveDirection.y - gravityAccel * controller.deltaTime;
        }
        else
        {
            controller.EnableClamping();
        }

        moveDirection = movement;
        controller.debugMove = moveDirection;
    }

    /// <summary>
    /// Constructs a vector representing our movement local to our lookDirection, which is
    /// controlled by the camera
    /// </summary>
    private Vector3 LocalMovement()
    {
        Vector3 right = Vector3.Cross(controller.up, lookDirection);

        Vector3 local = Vector3.zero;

        if (input.currentInput.inputHorizontal != 0)
        {
            local += right * input.currentInput.getInputHorizontal();
        }

        if (input.currentInput.inputVertical != 0)
        {
            local += lookDirection * input.currentInput.getInputVertical();
        }

        return local.normalized;
    }


    private bool AcquiringGround()
    {
        return controller.currentGround.IsGrounded(false, 0.01f);
    }

    private bool MaintainingGround()
    {
        return controller.currentGround.IsGrounded(true, 0.5f);
    }

    private float CalculateJumpVelocity()
    {
        return Mathf.Sqrt(0.5f * jumpHeight * gravityAccel);
    }

}
