using UnityEngine;
using System.Collections;

/// <summary>
/// Manages caracter movement according to CharacterInput values
/// </summary>
public class CharacterMovement : MonoBehaviour {

    [SerializeField]
    public float speed = 6.0F;
    [SerializeField]
    public float jumpSpeed = 8.0F;
    [SerializeField]
    public float gravity = 20.0F;

    private CharacterFixedController controller;
    private CharacterInput input;
    private Vector3 moveDirection = Vector3.zero;
    private Vector3 lookDirection;

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

    void SuperUpdate()
    {
        lookDirection = input.currentInput.inputAim;
        moveDirection = Vector3.MoveTowards(moveDirection, LocalMovement() * speed, Mathf.Infinity);
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
            local += right * input.currentInput.inputHorizontal;
        }

        if (input.currentInput.inputVertical != 0)
        {
            local += lookDirection * input.currentInput.inputVertical;
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

}
