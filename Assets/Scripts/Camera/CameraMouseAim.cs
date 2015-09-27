using UnityEngine;
using System.Collections;

/// <summary>
/// Simple script for mouse aim around a specific target
/// </summary>
/// <remarks>
/// In our case, the target is a little above characters's shoulder
/// </remarks>

public class CameraMouseAim : MonoBehaviour {

    public GameObject target;

	[SerializeField] private MouseLook m_MouseLook;
	private Camera m_Camera;

    public void RunUpdate(float delta)
    {
        m_MouseLook.LookRotation(target.transform, m_Camera.transform);
	}

    public void SetTarget(GameObject target)
    {
        this.target = target;

        if (target == null)
        {
            this.enabled = false;
        }
        else
        {
            this.transform.parent = target.transform;
            this.transform.localPosition = Vector3.zero;
            this.transform.rotation = target.transform.rotation;
            this.enabled = true;

            m_Camera = Camera.main;
            m_MouseLook.Init(target.transform, m_Camera.transform);
        }
    }
}
