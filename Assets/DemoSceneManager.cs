using UnityEngine;
using System.Collections;

public class DemoSceneManager : MonoBehaviour {

	public GameObject playerPrefab;

	public static DemoSceneManager instance;
	void Awake(){
		DemoSceneManager.instance = this;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {}

	[RPC]
	public void StartGame(){
		if(Network.isServer){
			// prevent anyone from connection after the game starts
			Network.maxConnections = 0;
			networkView.RPC("StartGame",RPCMode.Others);
		}

//		OnStartGame.Publish(null);
		if( playerPrefab != null){
			GameObject p = (Network.Instantiate(playerPrefab,Vector3.zero,Quaternion.identity,0) as GameObject);
			SpaceShipControl c = p.GetComponent<SpaceShipControl>();
			c.Respawn();
		}
	}
	
}
