using UnityEngine;
using System.Collections;


public class UniverseModel : MonoBehaviour {
	
	public float visionRange;
	public GameObject planetModel;
	public GameObject host;
	public GameObject ship;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		for (int i = 0; i < host.transform.childCount; i++) {
			GameObject obj = host.transform.GetChild(i).gameObject;
			Destroy(obj);
		}
		GameObject[] planets = GameObject.FindGameObjectsWithTag ("Planet");
		foreach (GameObject planet in planets) {
			Vector3 relativePosition = (planet.transform.position - ship.transform.position);
			if (relativePosition.magnitude < visionRange) {
				GameObject model = (GameObject)GameObject.Instantiate(planetModel);
				model.transform.parent = host.transform;
				model.transform.localPosition = ship.transform.rotation * (relativePosition / visionRange);
			}
		}
	}
}
