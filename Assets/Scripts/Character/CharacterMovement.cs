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

    private CharacterInput input;
    private Vector3 moveDirection = Vector3.zero;

	void Awake () 
    {
        input = GetComponent<CharacterInput>();
	}
	
    /// <summary>
    /// Run update like classic unity's Update
    /// We use an other method here because the calling must be controlled by CharacterNetwork
    /// We can't use standard Update method because Unity update order is non-deterministic
    /// </summary>
    /// <param name="delta"></param>
	public void RunUpdate(float delta) 
    {
        if (IsGrounded())
        {
            moveDirection = new Vector3(input.currentInput.inputHorizontal, 0, input.currentInput.inputVertical);
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection *= speed;

            //TODO add jump and gravity
            /*if (input.currentInput.inputJump)
            {
                moveDirection.y = jumpSpeed;
            }*/
        }
        Move(moveDirection);
	}

    public void Move(Vector3 direction)
    {
        //Move to given direction and check if it doesn"t intersect anything
        //TODO check that position doesn't intersect anything, and move accordigly (block before a wall, step up stairs)
        transform.Translate(direction, Space.World);
    }

    public bool IsGrounded()
    {
        //TODO check if character is actually touching ground
        return true;
    }

}
