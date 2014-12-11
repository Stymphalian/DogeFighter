using UnityEngine;
using System.Collections;

public class SpaceShipControl : MonoBehaviour {
	public static float BULLET_MAX_HOT_GAUGE = 100.0f;
	public static float BULLET_TIME_TO_START_COOLING_DOWN = 0.75f;

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
	public TextMesh messageText;


	// Bullet
	public GameObject bulletPrefab;
	public GameObject bulletBurstPrefab;	// temporarily using same prefab as missile explosion. Get new explosion prefab for bullet
	public GameObject bulletHotGaugeProgressBar;
	public Transform bulletHatch;
	public float bulletFireInterval;
	public float bulletHotGaugeIncreaseDeltaPerBullet;
	public float bulletHotGaugeDecreaseDeltaPerSecond;
	private float timeElapsedSinceLastBulletFire;
	private float bulletHotGauge;
	private float initialXPos;

	public Camera defaultCamera;
	public GameObject OVRRig;
	public GameObject fireMissileExplosion;
	public Transform missileHatch;
	public Transform aimingCameraTransform;
	public GameObject crosshair;

	private float initialEmissionRate;
	private int currentMissileCount;
	private float timeElapsedSinceLastMissileRegen;

	// player model stuff
	public int health;
	public bool deadFlag = false;


	public static SpaceShipControl instance;
	void Awake(){
		if (networkView.isMine == false)
		{
			defaultCamera.enabled = false; // disable the camera of the non-owned Player;
			defaultCamera.gameObject.GetComponent<MouseLook>().enabled = false;
			defaultCamera.GetComponent<AudioListener>().enabled = false;// Disables AudioListener of non-owned Player - prevents multiple AudioListeners from being present in scene.
		}else{
			instance = this;
		}
	}
	
	// Use this for initialization
	void Start () {
		rigidbody.maxAngularVelocity = maxAngularVelocity;
		initialEmissionRate = tailTrail.emissionRate;
		currentMissileCount = initialMissileCount;
		timeElapsedSinceLastMissileRegen = 0.0f;
		timeElapsedSinceLastBulletFire = 0.0f;
		bulletHotGauge = 0.0f;
		health = 100;
		messageText.gameObject.SetActive(false);


		
		if (useOVR) {
			if (networkView.isMine) {
				OVRRig.SetActive(true);
			}
			Destroy(defaultCamera.gameObject);
		} else {
			Destroy(OVRRig);
			crosshair = defaultCamera.gameObject.transform.Find("Crosshair").gameObject;
			aimingCameraTransform = defaultCamera.transform;
		}

		bulletHotGaugeProgressBar.renderer.material.color = new Color (0.80f, 0.30f, 0.20f);
	}

	void FixedUpdate () {
		if( networkView.isMine == false || deadFlag){return;}

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
		//for testing purposes only!

		rigidbody.AddRelativeTorque(pitch*turnspeed, yaw*turnspeed, roll*turnspeed);
		rigidbody.AddForce(gas * force * this.transform.forward);

		float speed = rigidbody.velocity.magnitude;
		velocityText.text = "Speed: " + speed.ToString("0.0") + "km/s";
	}

	// Update is called once per frame
	void Update () {
		if (networkView.isMine == false || deadFlag) {return;}

		timeElapsedSinceLastBulletFire += Time.deltaTime;
		if (Input.GetKeyDown("0")) {
			if (Network.isServer) {
				DemoSceneManager.instance.StartGame(Vector3.zero);
			}
		}
		if (Input.GetKeyDown ("1") && currentMissileCount > 0) {
			fireMissle (networkView.viewID, networkView.viewID);
		} else if (Input.GetKey ("3")) {
//			health -= 5;
//			updateHealth(health);
//			Debug.Log("damaging self");
			if (timeElapsedSinceLastBulletFire >= bulletFireInterval && bulletHotGauge < BULLET_MAX_HOT_GAUGE) {
				timeElapsedSinceLastBulletFire = 0;
				fireGun ();
				bulletHotGauge += bulletHotGaugeIncreaseDeltaPerBullet;
				if (bulletHotGauge >= BULLET_MAX_HOT_GAUGE) {
					bulletHotGauge = BULLET_MAX_HOT_GAUGE;
				}
			}
		}

		if (timeElapsedSinceLastBulletFire >= BULLET_TIME_TO_START_COOLING_DOWN) 
		{
			bulletHotGauge -= (bulletHotGaugeDecreaseDeltaPerSecond * Time.deltaTime);
			if (bulletHotGauge <= 0) 
			{
				bulletHotGauge = 0;
			}
		}

		float currentScaleX = bulletHotGauge / BULLET_MAX_HOT_GAUGE;
		bulletHotGaugeProgressBar.transform.localScale = new Vector3 (currentScaleX, 1.0f, 1.0f);

		timeElapsedSinceLastMissileRegen += Time.deltaTime;
		if (timeElapsedSinceLastMissileRegen >= missileRegenInterval) {
			timeElapsedSinceLastMissileRegen = 0;
			currentMissileCount++;
		}
		float timeUntilMissileRegen = missileRegenInterval - timeElapsedSinceLastMissileRegen;
		missileCount.text = "# Missiles: " + currentMissileCount + "\nRegens in: " + timeUntilMissileRegen.ToString("0.0") + "s";

		// Engine Sound
		float speed = rigidbody.velocity.magnitude;
		audio.pitch = speed / 200;
//		Debug.Log (audio.pitch);

	}

	public void Respawn() {
		Respawn (null);
	}

	
	[RPC]
	public void updateHealth(int newHealth){
		Debug.Log ("Updating health: " + newHealth);
		if( networkView.isMine){
			Debug.Log("My health");
			this.health = newHealth;
			if( this.health <= 0 && !deadFlag){
				this.health = 0;
				healthText.text = this.health.ToString();
				deadFlag = true;
				Debug.Log("You died!");
				setCockpitMessage("YOU DIED.");
				DemoSceneManager.instance.reduceLives(Network.player);
				StartCoroutine(delayRespawn());
			} else if (this.health > 0) {
				healthText.text = this.health.ToString();
			}

			networkView.RPC("updateHealth",RPCMode.Others, newHealth);
		}
	}

	public IEnumerator delayRespawn() {
		messageText.gameObject.SetActive(true);
		yield return new WaitForSeconds(3.0f);
		messageText.gameObject.SetActive(false);
		this.health = 100;
		updateHealth (this.health);
		deadFlag = false;
		Respawn();
	}

	[RPC]
	private void _fireGun(NetworkViewID bulletId,Vector3 target){
		GameObject bullet = NetworkManager.Find(bulletId);
		if( bullet == null){return;}
		BulletController bulletController = bullet.GetComponent<BulletController> ();
		bulletController.Init(0.75f, target, this.collider);
	}
	[RPC]
	public void fireGun(){
		Debug.Log("hey the gun fired");
		Debug.Log(Input.mousePosition);

		Vector3 target = crosshair.transform.position - aimingCameraTransform.position;
		target.Normalize ();
		Vector3 pos = crosshair.transform.position + target * 10;
		target = target * 300;
		target = target + rigidbody.velocity;

		GameObject bullet = (Instantiate (bulletPrefab, pos, Quaternion.identity) as GameObject);
		BulletController bulletController = bullet.GetComponent<BulletController> ();
		bulletController.Init(0.75f, target, this.collider);

		if( networkView.isMine){
			networkView.RPC("_fireGun",RPCMode.Others,bullet.networkView.viewID,target);
		}
	}

	[RPC]
	private void _fireMissle(NetworkViewID ownerId,NetworkViewID targetViewId,NetworkViewID missleID){
		GameObject targetGameObject = NetworkManager.Find(targetViewId);
		targetGameObject = null; // UNHACK THIS shit...
		GameObject ownerObject = NetworkManager.Find(ownerId);
		Vector3 pos = missileHatch.position;
		GameObject missle1 = NetworkManager.Find(missleID);

		currentMissileCount--;
		
		// set the target and stuff
		MissleController m1 = missle1.GetComponent<MissleController>();
		m1.Init(ownerObject,targetGameObject,0.75f,0.5f,this.rigidbody.velocity);
	}


	[RPC]
	public void fireMissle(NetworkViewID ownerId,NetworkViewID targetViewId){
		GameObject targetGameObject = NetworkManager.Find(targetViewId);
		targetGameObject = null; // UNHACK this shit...
		GameObject ownerObject = NetworkManager.Find(ownerId);
		Vector3 pos = missileHatch.position;
		GameObject missle1 = (Network.Instantiate(misslePrefab,pos,Quaternion.identity,0) as GameObject);
		Network.Instantiate(fireMissileExplosion, missle1.transform.position, Quaternion.identity,0);

		currentMissileCount--;

		// set the target and stuff
		MissleController m1 = missle1.GetComponent<MissleController>();
		m1.Init(ownerObject,targetGameObject,0.75f,0.5f,this.rigidbody.velocity);

		if (networkView.isMine){
			networkView.RPC("_fireMissle",RPCMode.Others,ownerId,targetViewId,missle1.networkView.viewID);
		}
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
				Debug.Log("HERE");
				Debug.Log(box.transform.position);
				playerSpawnPoint = box;
			}
			this.transform.position = new Vector3 (playerSpawnPoint.transform.position.x - 39,playerSpawnPoint.transform.position.y + 15, playerSpawnPoint.transform.position.z);
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

	public void setCockpitMessage(string text) {
		messageText.text = text;
	}

	public void onHit(int damage)
	{
		this.updateHealth (health - damage);
	}
}
