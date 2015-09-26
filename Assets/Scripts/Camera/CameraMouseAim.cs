using UnityEngine;
using System.Collections;

/// <summary>
/// Simple script for mouse aim around a specific target
/// </summary>
/// <remarks>
/// In our case, the target is a little above characters's shoulder
/// </remarks>



public class CameraMouseAim : MonoBehaviour {

    public const float detectionRadius = 0.15f;

    public GameObject target; 

    public float rotateSpeedH = 5;
    public float rotateSpeedV = 10;
    public Vector3 offset = new Vector3(0.50f, 0.30f, 3.4f);
    public float clampV = 40;

    private RaycastHit hit;
    Quaternion desiredRotation;
    Vector3 desiredPosition;
    Vector3 direction;

	[SerializeField] private MouseLook m_MouseLook;
	private Camera m_Camera;

	public void Start ()
	{
		m_Camera = Camera.main;
	}

    public void RunUpdate(float delta)
    {
		m_MouseLook.LookRotation (transform, m_Camera.transform);

//        if (target == null)
//        {
//            this.enabled = false;
//            return;
//        }
//
//	    //Get mouse position
//        float horizontal = Input.GetAxis("Mouse X") * rotateSpeedH;
//        float vertical = - Input.GetAxis("Mouse Y") * rotateSpeedV;
//
//        //Calculate new desired rotation
//        Vector3 desiredRotationEuler = desiredRotation.eulerAngles;
//        desiredRotationEuler.x = desiredRotationEuler.x + vertical;
//        desiredRotationEuler.y = desiredRotationEuler.y + horizontal;
//        //Wrap value to be in -180/+180 range to enable clamping
//        if (desiredRotationEuler.x > 180) { desiredRotationEuler.x = -(360 - desiredRotationEuler.x); }
//        desiredRotationEuler.x = Mathf.Clamp(desiredRotationEuler.x, -clampV, clampV);
//        desiredRotation = Quaternion.Euler(desiredRotationEuler);
//
//        // Calculate a new desired position from rotation     
//        desiredPosition = target.transform.position - (desiredRotation * offset);
//
//        // Raycast from target to desired cam position
//        direction = (desiredPosition - target.transform.position).normalized;
//        float distance = Vector3.Distance(desiredPosition, target.transform.position);
//        if (Physics.SphereCast(target.transform.position, detectionRadius, direction, out hit, distance))
//        {
//            //Place the camera just before the obstacle
//            //0.5 here ensure we are just before the obstacle
//            transform.position = hit.point + 0.4f * -direction;
//        }
//        else
//        {
//            //place the camera at the desired spot
//            transform.position = desiredPosition;
//        }
//
//        //Always look at player
//        transform.LookAt(target.transform);
	}
}
