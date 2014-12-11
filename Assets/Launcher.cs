using UnityEngine;
using System.Collections;

public class Launcher : MonoBehaviour {
	private bool launch;
	private int currentLaunchSpeed;

	// Use this for initialization
	void Start () {
		currentLaunchSpeed = 10;
		launch = false;
		StartCoroutine (delayLaunch(1f));
	
	}

	IEnumerator delayLaunch (float duration) {
		yield return new WaitForSeconds (duration);
		launch = true;
	}
	
	// Update is called once per frame
	void Update () {
	}

	void OnTriggerEnter(Collider other) {
		StartCoroutine (delayLaunch(1f));
	}

	void OnTriggerStay(Collider other) {
		Debug.Log ("INSIDE THE BOX");
		if (launch) {
			if (other.attachedRigidbody) {
				Debug.Log(other.name +  " has rigidbody");
				other.attachedRigidbody.AddForce(Vector3.right * currentLaunchSpeed);
				currentLaunchSpeed += 2;
			}
		}
	}

	void OnTriggerExit(Collider other) {
		currentLaunchSpeed = 10;
		launch = false;
	}
}
