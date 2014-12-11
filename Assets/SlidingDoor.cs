using UnityEngine;
using System.Collections;

public class SlidingDoor : MonoBehaviour {

	private Vector3 openPosition;
	private Vector3 closedPosition;
	private bool opened;
	// Use this for initialization
	void Start () {
		openPosition = new Vector3 (transform.position.x, transform.position.y + 50, transform.position.z);
		closedPosition = new Vector3 (transform.position.x, transform.position.y, transform.position.z);
		opened = false;
	}
	
	// Update is called once per frame
	void Update () {

	}

	void OnTriggerEnter(Collider other){
		Debug.Log("Trigger open door");
//		transform.position = Vector3.Lerp(transform.position, openPosition, 0.1f);
		if (!opened)
			StartCoroutine (moveDoor());
	}

	void OnTriggerExit(Collider other) {
		opened = false;
	}

	IEnumerator moveDoor(){
		opened = true;
		float time = 0;
		while (time < 1) {
			transform.position = Vector3.Lerp(transform.position, openPosition, (float) 4*Time.deltaTime);
			yield return new WaitForSeconds(Time.deltaTime);
			time += Time.deltaTime;
		}
		yield return new WaitForSeconds(1.0f);

		time = 0;
		while (time < 1) {
			transform.position = Vector3.Lerp(transform.position, closedPosition, (float) 4*Time.deltaTime);
			yield return new WaitForSeconds(Time.deltaTime);
			time += Time.deltaTime;
		}
		transform.position = closedPosition;
	} 
}
