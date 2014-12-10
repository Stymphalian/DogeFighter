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
	public GameObject bulletPrefab;
	public GameObject playerSpawnPoint;
	public TextMesh velocityText;
	public TextMesh missileCount;
	public TextMesh healthText;

	public GameObject bulletBurstPrefab;
	public Transform bulletSpawnPoint;

	public Camera defaultCamera;
	public Object OVRRig;
	public GameObject fireMissileExplosion;
	public Transform missileHatch;
	public Transform bulletHatch;

	private float initialEmissionRate;
	private int currentMissileCount;
	private float timeElapsedSinceLastMissileRegen;

	// player model stuff
	public int health;
	public int missleAmmo;
	public int missleCooldown;
	public int gunAmmo;
	public int gunCooldown;
	public bool deadFlag = false;

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

		if( Input.GetKeyDown("1") ){
			fireMissle(networkView.viewID,networkView.viewID);
			currentMissileCount--;
		} else if (Input.GetKeyDown("3")) {
			Vector3 pos = bulletHatch.position;
			GameObject bullet = (Instantiate (bulletPrefab, pos, Quaternion.identity) as GameObject);
			BulletController bulletController = bullet.GetComponent<BulletController> ();
			bulletController.Init(0.75f, this.rigidbody.velocity);
			GameObject.Instantiate (fireMissileExplosion, bullet.transform.position, Quaternion.identity);	// temporarily using fire explosion
		}

		timeElapsedSinceLastMissileRegen += Time.deltaTime;
		if (timeElapsedSinceLastMissileRegen >= missileRegenInterval) {
			timeElapsedSinceLastMissileRegen = 0;
			currentMissileCount++;
		}
		float timeUntilMissileRegen = missileRegenInterval - timeElapsedSinceLastMissileRegen;
		missileCount.text = "# Missiles: " + currentMissileCount + "\nRegens in: " + timeUntilMissileRegen.ToString("0.0") + "s";

		float speed = rigidbody.velocity.magnitude;
		audio.pitch = speed / 200;
	}

	public void Respawn() {
		Respawn (null);
	}

	
	[RPC]
	public void updateHealth(int newHealth){
		if( networkView.isMine){
			networkView.RPC("updateHealth",RPCMode.Others, newHealth);
		}
		this.health = newHealth;
		if( this.health < 0){
			this.health = 0;
			deadFlag = true;
		}
		healthText.text = this.health.ToString();
	}

	[RPC]
	public void fireGun(int newHealth){
		Debug.Log("hey the gun fired");
		if( networkView.isMine){
			networkView.RPC("fireGun",RPCMode.Others);
		}

	}
	[RPC]
	public void fireMissle(NetworkViewID ownerId,NetworkViewID targetViewId){
		if (networkView.isMine){
			networkView.RPC("fireMissle",RPCMode.Others,ownerId,targetViewId);
		}

		GameObject targetGameObject = NetworkManager.Find(targetViewId);
		targetGameObject = null;
		GameObject ownerObject = NetworkManager.Find(ownerId);
		Vector3 pos = missileHatch.position;
		GameObject missle1 = (Network.Instantiate(misslePrefab,pos,Quaternion.identity,0) as GameObject);
		Network.Instantiate(fireMissileExplosion, missle1.transform.position, Quaternion.identity,0);


		currentMissileCount--;

		// set the target and stuff
		MissleController m1 = missle1.GetComponent<MissleController>();
		m1.Init(ownerObject,targetGameObject,0.75f,0.5f,this.rigidbody.velocity);
	}

	[RPC]
	public void respawn(){
		if( networkView.isMine) {
			networkView.RPC("respawn", RPCMode.Others);
		}
		Respawn();
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
