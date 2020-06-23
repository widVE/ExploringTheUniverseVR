using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OculusTouchEvent : MonoBehaviour {
	public OVRInput.RawButton button;
	public UnityEvent activateMe;

	void Update()
	{
		if (OVRInput.GetDown(button))
		{
			activate();
		}
	}

	public void activate()
	{
		if (activateMe != null)
		{
			activateMe.Invoke();
		}
	}
}
