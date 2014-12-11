using UnityEngine;
using System.Collections;

public class BulletController : MonoBehaviour {
	public GameObject explosion;
	public float speed = 250;
	public float autoDestroyTime = 5.0f;
	public int bulletDamage = 20;

	public void Init(float speed,Vector3 initialVelocity, Collider collider){
		this.speed = speed;
		this.rigidbody.velocity = initialVelocity + this.transform.forward * 2;

		Physics.IgnoreCollision(this.collider, collider);

		StartCoroutine(AutoExplode(autoDestroyTime));
	}

	void FixedUpdate () {		
		this.rigidbody.AddForce(this.transform.forward * speed);
		//		Debug.Log(rigidbody.velocity);
	}

//	void OnTriggerEnter(Collider collision){
//		Debug.Log("bullet contorller trigger happened");
//		
//		var player = collision.gameObject.GetComponent<SpaceShipControl>();
//		if (player) {
//			player.onHit(this.bulletDamage);
//		}
//		
//		ExplodeSelf();	// test if this is needed
//	}
	
	void OnCollisionEnter(Collision collision){
		Debug.Log("Bullet controller on collision enter");

		var player = collision.gameObject.GetComponent<SpaceShipControl>();
		if (player) {
			player.onHit(this.bulletDamage);
		}

		ExplodeSelf();	// test if this is needed
	}

	void ExplodeSelf(){
		if( explosion != null){
			Instantiate(explosion, transform.position, Quaternion.identity);
		}
		NetworkManager.Destroy(gameObject);
	}
	
	IEnumerator AutoExplode(float autoDestroyTime){
		yield return new WaitForSeconds(autoDestroyTime);
		ExplodeSelf();
	}
}
