using UnityEngine;
using System.Collections;


public class UniverseModel : MonoBehaviour {
	
	public float visionRange;
	public GameObject planetModel;
	public GameObject shipModel;
	public GameObject host;
	public GameObject ship;
	public GameObject globe;
	public GameObject enemyShip;
	public LineRenderer line;

	private bool zoomed = true;
	public float targetScale;
	float scaleToRangeRatio;
	// Use this for initialization
	void Start () {
		targetScale = globe.transform.localScale.x;
		scaleToRangeRatio = visionRange * targetScale;

		DemoSceneManager.instance.ev.Sub (this.gameObject,handler);
	}

	void handler(GameObject self, System.Object data){
		GameObject[] ships = GameObject.FindGameObjectsWithTag ("Ship");
		foreach (GameObject shipFound in ships) {
			if (shipFound != ship) {
				enemyShip = shipFound;
			}
		}
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
				model.transform.localPosition = Quaternion.Inverse(ship.transform.rotation) * (relativePosition / visionRange);
			}
		}
		if (enemyShip) {
			Vector3 relativePosition = (enemyShip.transform.position - ship.transform.position);
			
			GameObject model = (GameObject)GameObject.Instantiate(shipModel);
			model.transform.parent = host.transform;
			model.transform.localPosition = Quaternion.Inverse(ship.transform.rotation) * (relativePosition / visionRange);
			model.transform.rotation = Quaternion.Inverse(ship.transform.rotation) * 
				Quaternion.Inverse(enemyShip.transform.rotation);
			line.SetPosition(0, model.transform.position);
		}
		line.SetPosition (1, globe.transform.position);
		globe.transform.localScale = Vector3.Lerp (globe.transform.localScale, Vector3.one * targetScale, Time.deltaTime);
		float currentScale = globe.transform.localScale.x;
		visionRange = scaleToRangeRatio / currentScale;
		Vector3 pos = transform.localPosition;
		pos.y = currentScale * 0.046f;
		transform.localPosition = pos;

		zoomed = Input.GetButton("Info");
		
		line.renderer.enabled = zoomed;
		if (zoomed) {
						targetScale = 0.1f;
				} else {
			targetScale = 0.005f;
				}
	}
}
