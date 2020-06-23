using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtObject : MonoBehaviour
{
	[SerializeField]
	Transform LookAt;

	[ContextMenu("Look At Transform")]
	void LookAtTransform()
	{
		transform.LookAt(LookAt);
	}
}
