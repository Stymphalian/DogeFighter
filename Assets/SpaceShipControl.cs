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
	public int initialMissileCount;
	public float missileRegenInterval;

	public GameObject misslePrefab;
	public GameObject missleTarget; // temporary hardcoded target
	public GameObject playerSpawnPoint;
	public TextMesh velocityText;
	public TextMesh missileCount;
	public TextMesh healthText;

	public Camera defaultCamera;
	public Object OVRRig;
	public GameObject fireMissileExplosion;
	public Transform missileHatch;

	private float initialEmissionRate;
	private int currentMissileCount;
	private float timeElapsedSinceLastMissileRegen;

	// player model stuff
	public int health;

	void Awake(){
		if (networkView.isMine == false)
		{
			defaultCamera.enabled = false; // disable the camera of the non-owned Player;
			defaultCamera.gameObject.GetComponent<MouseLook>().enabled = false;
			defaultCamera.GetComponent<AudioListener>().enabled = false;// Disables AudioListener of non-owned Player - prevents multiple AudioListeners from being present in scene.
		}
	}
	
	// Use this for initialization
	void Start () {
		rigidbody.maxAngularVelocity = maxAngularVelocity;
		initialEmissionRate = tailTrail.emissionRate;
		currentMissileCount = initialMissileCount;
		timeElapsedSinceLastMissileRegen = 0.0f;

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

		float speed = rigidbody.velocity.magnitude;
		velocityText.text = "Speed: " + speed.ToString("0.0") + "km/s";
	}

	// Update is called once per frame
	void Update () {
		if( networkView.isMine == false){return;}

		if (Input.GetKeyDown ("1") && currentMissileCount > 0) {
			Vector3 pos = missileHatch.position;
			GameObject missle1 = (Instantiate (misslePrefab, pos, Quaternion.identity) as GameObject);
			MissleController m1 = missle1.GetComponent<MissleController> ();
			m1.Init (missleTarget, 0.75f, 0.5f, this.rigidbody.velocity);
			GameObject.Instantiate (fireMissileExplosion, missle1.transform.position, Quaternion.identity);
			currentMissileCount--;
		}else if( Input.GetKeyDown("2")){
			GameObject[] gos = GameObject.FindGameObjectsWithTag("Ship");
			for ( int i = 0; i < gos.Length; ++i){
				Debug.Log("gos [" + i + "] networkView.isMine " + gos[i].networkView.isMine);
				if( gos[i].networkView.isMine == false){
					gos[i].transform.Find("Camera").GetComponent<MouseLook>().enabled = false;
				}
			}
		}

		timeElapsedSinceLastMissileRegen += Time.deltaTime;
		if (timeElapsedSinceLastMissileRegen >= missileRegenInterval) {
			timeElapsedSinceLastMissileRegen = 0;
			currentMissileCount++;
		}
		float timeUntilMissileRegen = missileRegenInterval - timeElapsedSinceLastMissileRegen;
		missileCount.text = "# Missiles: " + currentMissileCount + "\nRegens in: " + timeUntilMissileRegen.ToString("0.0") + "s";
	}


	
	[RPC]
	public void updateHealth(int newHealth){
		if( Network.isServer){
			networkView.RPC("updateHealth",RPCMode.Others, newHealth);
		}
		this.health = newHealth;
		healthText.text = this.health.ToString();
	}
	
	// send to server actions...
	[RPC]
	public void updateHealthAction(int newHealth){
		if( Network.isClient){
			networkView.RPC("updateHealthAction",RPCMode.Server);
		}else if( Network.isServer){
			updateHealth(newHealth);
		}
	}
	[RPC]
	public void fireGun(){
		if( Network.isClient){
			networkView.RPC("fireGun",RPCMode.Server);
		}else if( Network.isServer){

		}
	}

	[RPC]
	public void fireMissle(GameObject target){
		if( Network.isClient){
			networkView.RPC("fireMissle",RPCMode.Server,target);
		}else if( Network.isServer){
			fireMissle(target);
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
