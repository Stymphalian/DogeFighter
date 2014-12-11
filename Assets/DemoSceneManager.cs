using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class DemoSceneManager : MonoBehaviour {

	public GameObject playerPrefab;
	public GameObject launchBox;
	public GameObject spaceStation;
	public GameObject[] planets;
	public bool[] usedPlanets = new bool[4] {false,false,false,false};
	public bool inGameFlag = false;
	public Dictionary<NetworkPlayer, int> playerLives = new Dictionary<NetworkPlayer, int>();



	public static DemoSceneManager instance;
	void Awake(){
		DemoSceneManager.instance = this;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {}

	public EventSubscriber ev = new EventSubscriber();
	[RPC]
	public void StartGame(Vector3 postition){
		spaceStation.SetActive(false);
		ev.Publish(null);
		inGameFlag = true;

		if(Network.isServer){
			// prevent anyone from connection after the game starts
			Network.maxConnections = 0;
			playerLives[Network.player] = 3;

			foreach (NetworkPlayer player in Network.connections) {
				if (player != Network.player) {
					Debug.Log("Not the server");
					Vector3 planetPosition = getStartingPlanetPosition();
					Debug.Log("planetp =  "+ planetPosition);
					networkView.RPC("StartGame",player, planetPosition);
					playerLives[player] = 3;
				}
			}
			
		}

		//		OnStartGame.Publish(null);
		if( playerPrefab != null){
//			GameObject p = (Network.Instantiate(playerPrefab,Vector3.zero,Quaternion.identity,0) as GameObject);
			GameObject p = SpaceShipControl.instance.gameObject;
			GameObject l = (Network.Instantiate(launchBox,Vector3.zero,Quaternion.identity,0) as GameObject);
			SpaceShipControl c = p.GetComponent<SpaceShipControl>();
			Vector3 launchPadPosition;
			if (Network.isServer)
				launchPadPosition = getStartingPlanetPosition();
			else
				launchPadPosition = postition;
			
			launchPadPosition = new Vector3(launchPadPosition.x, launchPadPosition.y + 50, launchPadPosition.z);
			
			l.transform.position = launchPadPosition;
			c.Respawn(l);
		}
	}


	private Vector3 getStartingPlanetPosition() {
		System.Random rand = new System.Random();

		int planet = 0;
		while (usedPlanets[planet]) {
			planet = rand.Next(0, 4);
		}
		Debug.Log ("planet:" + planet);
		usedPlanets[planet] = true;
		return planets[planet].transform.position;
	}

	[RPC]
	public void reduceLives(NetworkPlayer player) {
		if (Network.isServer && inGameFlag == true) {

			playerLives [player]--;
			Debug.Log("Remaining lives:" + playerLives [player]);
			if (playerLives [player] <= 0) {
				Debug.Log ("END GAME");
			}
		}
//		else {
//			networkView.RPC("reduceLives",RPCMode.Server, player);
//		}
	}

}
