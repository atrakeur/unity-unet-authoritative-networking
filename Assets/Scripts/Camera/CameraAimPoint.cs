using UnityEngine;
using System.Collections;

/// <summary>
/// Calculates aimPoint based on camera current looking direction
/// </summary>
public class CameraAimPoint : MonoBehaviour {

    [SerializeField]
    public float distance = 100;

    public Vector3 aimPoint;

    public void RunUpdate(float delta)
    {
        aimPoint = transform.position + transform.forward * distance;
	}
}
