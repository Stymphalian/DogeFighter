using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AsteroidBelt : MonoBehaviour {
	
	public int beltDivision = 20;
	public int currentBelt = -100;
	public float minimumDistance;
	public float maximumDistance;
	public int density = 20;
	public float randomSeed;
	public GameObject reference;

	public GameObject[] asteroidModels;
	public Dictionary<int, List<GameObject>> asteroids;
	public List<int> exsitedBelt;

	void Start () {
		asteroids = new Dictionary<int, List<GameObject>>();
		exsitedBelt = new List<int>();
	}

	// Update is called once per frame
	void Update () {
		float angle = Mathf.Atan2(reference.transform.position.z - transform.position.z,
		                          reference.transform.position.x - transform.position.x);
		int newBelt = (int)(angle / (Mathf.PI * 2 / (float)beltDivision));
		if (Mathf.Abs(newBelt - currentBelt) >= 1) {
			Debug.Log("Refreshing Asteroid Belt");
			currentBelt = newBelt;
			int[] divisions = {currentBelt - 2, currentBelt - 1, currentBelt, currentBelt + 1, currentBelt + 2};
			foreach (KeyValuePair<int, List<GameObject>> entry in asteroids) {
				if (exsitedBelt.Contains(entry.Key)) continue;
				foreach (GameObject asteroid in entry.Value) {
					Destroy(asteroid);
				}
			}
			asteroids[currentBelt] = new List<GameObject>();
			foreach (int beltDiv in divisions) {
				if (exsitedBelt.Contains(beltDiv)) continue;
				randomSeed = beltDiv;
				Random.seed = (int)randomSeed;
				for (int i = 0; i < density; i++) {
					float asAngle = ((Random.Range(-0.5f, 0.5f) + beltDiv) * (Mathf.PI * 2 / (float)beltDivision));
					float dis = Random.Range(minimumDistance, maximumDistance);
					GameObject asteroid = (GameObject)GameObject.Instantiate(asteroidModels[0]);
					asteroid.transform.parent = this.transform;
					asteroid.transform.position = new Vector3(Mathf.Cos(asAngle), 0, Mathf.Sin(asAngle)) * dis + this.transform.position;
					asteroids[currentBelt].Add(asteroid);
				}
			}

			exsitedBelt = new List<int>();
			foreach(int div in divisions) {
				exsitedBelt.Add(div);
			}
		}
	}
}
