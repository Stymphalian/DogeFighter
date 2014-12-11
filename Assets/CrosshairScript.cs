using UnityEngine;
using System.Collections;

public class CrosshairScript : MonoBehaviour {
	public Camera CameraFacing;
	private Vector3 originalScale;
	
	// Use this for initialization
	void Start () {
		originalScale = transform.localScale;
	}
	
	// Update is called once per frame
	void Update () {
		RaycastHit hit = new RaycastHit();
		Ray ray = new Ray (CameraFacing.transform.position, CameraFacing.transform.rotation * Vector3.forward);
		float distance;
		if (Physics.Raycast (ray, out hit)) {
			distance = hit.distance;
		} else {
			distance = CameraFacing.farClipPlane * 0.95f;
		}
		//transform.position = CameraFacing.transform.position + CameraFacing.transform.rotation * Vector3.forward * distance;
		transform.position = CameraFacing.transform.position + CameraFacing.transform.rotation * Vector3.forward * 0.5f;
		transform.LookAt (CameraFacing.transform.position);
		transform.Rotate (0.0f, 180.0f, 0.0f);
		//transform.localScale = originalScale * distance;
	}
}