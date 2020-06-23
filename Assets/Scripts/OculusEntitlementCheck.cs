using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Platform;

//simple script to perform entitlement check on launch
public class OculusEntitlementCheck : MonoBehaviour
{   //https://developer.oculus.com/blog/unity-engine-entitlement-checks-for-gear-vr-and-rift/
	void Awake()
	{
		try
		{	//app ID should be set in Unity's OVRPlatformSettings menu, not here
			Core.AsyncInitialize();
		}
		catch (UnityException ue)
		{
			Debug.LogError("Oculus Entitlement Check failed.");
			Debug.LogException(ue);
			//UnityEngine.Application.Quit();
		}
	}
}
