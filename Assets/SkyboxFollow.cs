using UnityEngine;
using System.Collections;

public class SkyboxFollow : MonoBehaviour {
	
	public GameObject following;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (following != null) {
			this.transform.position = following.transform.position;
		}
	}
}
