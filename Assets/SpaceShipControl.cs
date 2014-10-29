using UnityEngine;
using System.Collections;

public class SpaceShipControl : MonoBehaviour {
	public Vector3 acceleration;
	public float force;

	// Use this for initialization
	void Start () {
		
	}

	void FixedUpdate () {
		acceleration = this.transform.forward;
		this.rigidbody.AddForce (acceleration * force);
	}

	// Update is called once per frame
	void Update () {
	
	}
}
