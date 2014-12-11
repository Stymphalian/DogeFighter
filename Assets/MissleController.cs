using UnityEngine;
using System.Collections;

public class MissleController : MonoBehaviour {
	public GameObject explosion;
	public float homingSensitivity = 0.1f;
	public float speed = 100;
	public GameObject target = null;
	public GameObject owner = null;
	public float autoDestroyTime = 20.0f;
	public int missileDamage = 100;
	
	public ParticleSystem tailTrail;

	Vector2 CartesianToPolar(Vector3 point)
	{
		Vector2 polar;
		
		//calc longitude
		polar.y = Mathf.Atan2(point.x,point.z);
		
		//this is easier to write and read than sqrt(pow(x,2), pow(y,2))!
		float xzLen = new Vector2(point.x,point.z).magnitude;
		//atan2 does the magic
		polar.x = Mathf.Atan2(-point.y,xzLen);
		
		//convert to deg
		polar *= Mathf.Rad2Deg;
		
		return polar;
	}
	
	public void Init(GameObject owner,GameObject target,float speed,float sensitivity,Vector3 initialVelocity){
		this.owner = owner;
		this.target = target;
		this.speed = speed;
		this.homingSensitivity = sensitivity;
		this.rigidbody.velocity = initialVelocity + this.transform.forward*2;
		//		      tailTrail.emissionRate = 100;

		Physics.IgnoreCollision(this.collider,owner.collider);
		
		StartCoroutine(AutoExplode(autoDestroyTime));
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if( target != null){
			Vector3 relPos = target.transform.position - transform.position;
			relPos = relPos.normalized*homingSensitivity;
			relPos = Quaternion.Inverse(this.transform.rotation)*relPos;
			Vector2 polar = CartesianToPolar(relPos.normalized);
			//          Quaternion rot = Quaternion.LookRotation(relPos);
			//          this.transform.rotation = Quaternion.Slerp(transform.rotation,rot, homingSensitiviy);
			polar *= homingSensitivity;
			rigidbody.AddRelativeTorque(polar.x,polar.y, 0);
		}


		this.rigidbody.AddForce(this.transform.forward*speed);
//		Debug.Log(rigidbody.velocity);
	}

	void OnTriggerEnter(Collider collider){
		Debug.Log("MissleController OnTriggerEnter");
		
		var player = collider.gameObject.GetComponent<SpaceShipControl>();
		if (player) {
			Debug.Log("Missile hit a player");
			player.onHit(this.missileDamage);
		}
		
		ExplodeSelf();
	}

	void ExplodeSelf(){
		if( explosion != null){
			GameObject go = Instantiate(explosion, transform.position, Quaternion.identity) as GameObject;
			Destroy(go, 2.0f);
		}
		Destroy(gameObject);
	}
	
	IEnumerator AutoExplode(float autoDestroyTime){
		yield return new WaitForSeconds(autoDestroyTime);
		ExplodeSelf();
	}
}