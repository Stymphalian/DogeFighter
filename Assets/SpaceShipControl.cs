using UnityEngine;
using System.Collections;

public class SpaceShipControl : MonoBehaviour {
	public Vector3 acceleration;
	public float force;
	public float turnspeed;
	public float maxAngularVelocity;

	// Use this for initialization
	void Start () {
		rigidbody.maxAngularVelocity = maxAngularVelocity;
	}

	void FixedUpdate () {

		float yaw = Input.GetAxis("Horizontal");
		float pitch = -Input.GetAxis("Vertical");
		float roll = Input.GetAxis("Roll");

		rigidbody.AddRelativeTorque(pitch*turnspeed, yaw*turnspeed, roll*turnspeed);
	}

	// Update is called once per frame
	void Update () {
	
	}
}
