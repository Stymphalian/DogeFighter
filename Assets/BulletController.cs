using UnityEngine;
using System.Collections;

public class BulletController : MonoBehaviour {
	public GameObject explosion;
	public float speed = 100;
	public float autoDestroyTime = 20.0f;

	public void Init(float speed,Vector3 initialVelocity){
		this.speed = speed;
		this.rigidbody.velocity = initialVelocity + this.transform.forward * 2;
		
		StartCoroutine(AutoExplode(autoDestroyTime));
	}

	void FixedUpdate () {		
		this.rigidbody.AddForce(this.transform.forward * speed);
		//		Debug.Log(rigidbody.velocity);
	}

	void OnTriggerEnter(Collider other){
		Debug.Log("trigger happened");
		ExplodeSelf();
		
		// do something to the other object
		// take away health, etc
	}

	void ExplodeSelf(){
		if( explosion != null){
			Instantiate(explosion, transform.position, Quaternion.identity);
		}
		Destroy(gameObject);
	}
	
	IEnumerator AutoExplode(float autoDestroyTime){
		yield return new WaitForSeconds(autoDestroyTime);
		ExplodeSelf();
	}
}
