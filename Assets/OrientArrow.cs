using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrientArrow : MonoBehaviour
{
    public GameObject gaze_reticle;

    void LateUpdate()
    {
		Camera mainCam = Camera.main;
		Vector3 camPosition = mainCam.transform.position;
		Vector3 gazeReticlePosition = gaze_reticle.transform.position;
		Vector3 arrowPosition = transform.position;
		float arrowDistance = Vector3.Distance(camPosition, arrowPosition);
		//Vector2 gr_screen_pos = mainCam.WorldToScreenPoint(gazeReticlePosition);
		Vector2 gr_screen_pos = WorldToScreenPointProjected(mainCam, gazeReticlePosition);
		Vector3 adjusted_gr_world_pos = mainCam.ScreenToWorldPoint(new Vector3(gr_screen_pos.x, gr_screen_pos.y, arrowDistance));
		//transform.LookAt(adjusted_gr_world_pos, mainCam.transform.up);
		transform.LookAt(adjusted_gr_world_pos, mainCam.transform.forward);
	}

	public static Vector2 WorldToScreenPointProjected(Camera camera, Vector3 worldPos)
	{   //https://forum.unity.com/threads/camera-worldtoscreenpoint-bug.85311/
		Vector3 camNormal = camera.transform.forward;
		Vector3 vectorFromCam = worldPos - camera.transform.position;
		float camNormDot = Vector3.Dot(camNormal, vectorFromCam);
		if (camNormDot <= 0)
		{
			// we are behind the camera forward facing plane, project the position in front of the plane
			Vector3 proj = (camNormal * camNormDot * 1.01f);
			worldPos = camera.transform.position + (vectorFromCam - proj);
		}

		return RectTransformUtility.WorldToScreenPoint(camera, worldPos);
	}
}
