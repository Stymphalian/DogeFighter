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
	public GameObject playerSpawnPoint;

	public Camera defaultCamera;
	public Object OVRRig;
	public GameObject fireMissileExplosion;
	public Transform missileHatch;

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
		if( networkView.isMine == false){return;}
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
		rigidbody.AddForce(gas * force * this.transform.forward*10);
	}

	// Update is called once per frame
	void Update () {
		if( networkView.isMine == false){return;}
		if( Input.GetKeyDown("1") ){
			Vector3 pos = missileHatch.position;
			GameObject missle1 = (Instantiate(misslePrefab,pos,Quaternion.identity) as GameObject);
			MissleController m1 = missle1.GetComponent<MissleController>();
			m1.Init(missleTarget,0.75f,0.5f,this.rigidbody.velocity);
			GameObject.Instantiate(fireMissileExplosion, missle1.transform.position, Quaternion.identity);
		}
	}


	public void Respawn(){
		if( playerSpawnPoint != null){

		}else{
			this.transform.position = Vector3.zero;
		}

		// have the sky box follow me again...
		if( SkyboxFollow.instance != null){
			SkyboxFollow.instance.following = this.gameObject;
		}
	}
}
