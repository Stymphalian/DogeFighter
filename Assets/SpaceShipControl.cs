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

	void Awake(){
		if (networkView.isMine == false)
		{
			defaultCamera.enabled = false; // disable the camera of the non-owned Player;
			defaultCamera.GetComponent<AudioListener>().enabled = false;// Disables AudioListener of non-owned Player - prevents multiple AudioListeners from being present in scene.
		}
	}
	
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
//		Debug.Log (roll);

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

	public void Respawn() {
		Respawn (null);
	}

	public void Respawn(GameObject box){
		if (box != null || playerSpawnPoint != null) {
			Debug.Log ("Position within launchbox");
			if (playerSpawnPoint == null) {
				playerSpawnPoint = box;
				playerSpawnPoint.transform.position = Vector3.zero;
			}
			this.transform.position = new Vector3 (playerSpawnPoint.transform.position.x - 39,playerSpawnPoint.transform.position.y + 15, 0);
			this.transform.rotation = Quaternion.AngleAxis (90, Vector3.up);
		} else {
			Debug.Log("Position without launchbox");
			this.transform.position = Vector3.zero;
		}

		// have the sky box follow me again...
		if( SkyboxFollow.instance != null){
			SkyboxFollow.instance.following = this.gameObject;
		}
	}
}
