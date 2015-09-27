using UnityEngine;
using System.Collections;

/// <summary>
/// Manages character rotation according to CharacterInput
/// </summary>
public class CharacterRotation : MonoBehaviour {

    private CharacterInput input;
    private Vector3 aimPoint;

	void Awake () {
        input = GetComponent<CharacterInput>();

        aimPoint = input.currentInput.inputAim;
	}
	
    /// <summary>
    /// Run update like classic unity's Update
    /// We use an other method here because the calling must be controlled by CharacterNetwork
    /// We can't use standard Update method because Unity update order is non-deterministic
    /// </summary>
    /// <param name="delta"></param>
    public void RunUpdate(float delta)
    {
        aimPoint = input.currentInput.inputAim;
        aimPoint.y = transform.position.y;      //Make the model face always the same height
        transform.LookAt(aimPoint);
	}
}
