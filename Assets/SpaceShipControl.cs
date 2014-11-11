using UnityEngine;
using System.Collections;

public class SpaceShipControl : MonoBehaviour {
	public Vector3 acceleration;
	public float force;
	public float turnspeed;
	public float maxAngularVelocity;
	public float maxSpeed;
	public ParticleSystem tailTrail;
	public bool useOVR;

	public GameObject misslePrefab;
	public GameObject missleTarget; // temporary hardcoded target

	public Camera defaultCamera;
	public Object OVRRig;

	private float initialEmissionRate;

	// Use this for initialization
	void Start () {
		rigidbody.maxAngularVelocity = maxAngularVelocity;
		initialEmissionRate = tailTrail.emissionRate;
		if (useOVR) {
			Destroy(defaultCamera.gameObject);
		} else {
			Destroy(OVRRig);
		}
	}

	void FixedUpdate () {

		float yaw = Input.GetAxis("Horizontal");
		float pitch = -Input.GetAxis("Vertical");
		float roll = Input.GetAxis("Roll");

		float gas = Input.GetAxis("Gas");
		if (gas > 0) {
			tailTrail.emissionRate = initialEmissionRate * gas;
		} else {
			tailTrail.emissionRate = 0;
		}

		rigidbody.AddRelativeTorque(pitch*turnspeed, yaw*turnspeed, roll*turnspeed);
		rigidbody.AddForce(gas * force * this.transform.forward);
	}

	// Update is called once per frame
	void Update () {
		if( Input.GetKeyDown("1") ){
			Vector3 pos = transform.forward*2;
			GameObject missle1 = (Instantiate(misslePrefab,transform.position + pos,Quaternion.identity) as GameObject);
			MissleController m1 = missle1.GetComponent<MissleController>();
			m1.Init(missleTarget,0.75f,0.5f,this.rigidbody.velocity);
		}
	
	}
}
