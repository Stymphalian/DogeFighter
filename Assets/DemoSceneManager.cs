using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class DemoSceneManager : MonoBehaviour {

	public GameObject playerPrefab;
	public GameObject launchBox;
	public GameObject[] planets;
	public static DemoSceneManager instance;
	public Dictionary<NetworkPlayer, int> playerLives = new Dictionary<NetworkPlayer, int>();
	public bool[] usedPlanets = new bool[4] {false,false,false,false};
	void Awake(){
		DemoSceneManager.instance = this;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {}

	[RPC]
	public void StartGame(Vector3 postition){
		if(Network.isServer){
			// prevent anyone from connection after the game starts
			Network.maxConnections = 0;
			Debug.Log("NUMBER OF CONNECTIONS: " + Network.connections.Length);

			playerLives[Network.player] = 3;
			foreach (NetworkPlayer player in Network.connections) {
				Vector3 planetPosition = getStartingPlanetPosition();
				networkView.RPC("StartGame",player, planetPosition);
				playerLives[player] = 3;
			}

		}

//		OnStartGame.Publish(null);
		if( playerPrefab != null){
			GameObject p = (Network.Instantiate(playerPrefab,Vector3.zero,Quaternion.identity,0) as GameObject);
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

		int planet = rand.Next(0, 4);
		while (usedPlanets[planet]) {
			planet = rand.Next(0, 4);
		}
		Debug.Log ("planet:" + planet);
		usedPlanets[planet] = true;
		return planets[planet].transform.position;
	}

	[RPC]
	public void incrementScore(NetworkPlayer player) {
		if (Network.isServer) {
			playerLives[player]--;
			if (playerLives[player] == 0) {
				Debug.Log("END GAME");
			}
		}
		else if (player == Network.player) {
			networkView.RPC("incrementScore",RPCMode.Others, player);
		}

	}

}
