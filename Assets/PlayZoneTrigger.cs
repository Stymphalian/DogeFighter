using UnityEngine;
using System.Collections;

public class PlayZoneTrigger : MonoBehaviour {

	// Use this for initialization
	public int targetNumPlayers = 1;
	public int count = 0;
	public bool keepLookingFlag = true;
	public static PlayZoneTrigger instance;
	public int triggerRadius = 200;

	void Awake(){
		PlayZoneTrigger.instance = this;
	}

	void FixedUpdate(){
		if( keepLookingFlag == false){return;}
		GameObject[] players = GameObject.FindGameObjectsWithTag("Ship");

		int count = 0;
		foreach (GameObject p in players){
			if((p.transform.position - this.transform.position).magnitude  < triggerRadius){
				count++;
			}
		}


		if( count >= targetNumPlayers){
			LobbySceneManager.instance.startGame();
			keepLookingFlag = false;
		}

	}

	void OnTriggerEnter(Collider other){
		Debug.Log("ontrigger enter");
		if( keepLookingFlag == false){return;}
		SpaceShipControl c = other.gameObject.GetComponent<SpaceShipControl>();
		if( c != null){
			count++;
		}

		if( count == targetNumPlayers ){
			LobbySceneManager.instance.startGame();
			keepLookingFlag = false;
		}
	}
	void OnTriggerExit(Collider other){
		Debug.Log("ontrigger exit");
		if( keepLookingFlag == false){return;}
		SpaceShipControl c = other.gameObject.GetComponent<SpaceShipControl>();
		if( c != null){
			count--;
		}
	}

}
