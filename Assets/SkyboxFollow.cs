using UnityEngine;
using System.Collections;

public class SkyboxFollow : MonoBehaviour {
	
	public GameObject following;
	public static SkyboxFollow instance;
	void Awake(){
		SkyboxFollow.instance = this;
	}

	// Update is called once per frame
	void LateUpdate () {
		if (following != null) {
			this.transform.position = following.transform.position;
		}
	}
}
