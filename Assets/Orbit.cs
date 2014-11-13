using UnityEngine;
using System.Collections;

public class Orbit : MonoBehaviour {

	public Transform sun;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		this.transform.right = (this.transform.position - sun.transform.position).normalized;
	}
}
