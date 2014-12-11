using UnityEngine;
using System.Collections;

public class LobbySceneManager : MonoBehaviour {

	public static LobbySceneManager instance;
	public GameObject playerShipPrefab;
	public GameObject spawnPoint;
	public string gameSceneName = "Demo";
	public int timeDown = 3;
	public GameObject beginGameZone;
	private bool playZoneTriggerActive = false;

	void Awake(){
		LobbySceneManager.instance = this;
	}

	// Use this for initialization
	void Start () {
//		Network.Instantiate(playerShipPrefab,spawnPoint.transform.position,Quaternion.identity,0);
		NetworkManager.instance.Play(OnConnectedCallback);
	}
	
	void OnConnectedCallback(){
		Network.Instantiate(playerShipPrefab,spawnPoint.transform.position,Quaternion.identity,0);

		if(Network.isServer){
			Debug.LogError("I am the server");
			// do nothign, we are waiting for other players.
			// wiating for players...
		}else if(Network.isClient){
			Debug.LogError("I am the client");
			// create a ship for the player to fly in for now...
		}
	}
		
	// called on the server when a player is connected...
	void OnPlayerConnected(NetworkPlayer player){
		if( playZoneTriggerActive == true){return;}
		if( Network.connections.Length == Config.MAX_NUM_PLAYERS){
			playZoneTriggerActive = true;
			if( Network.isServer){
				// no more players can connect...
				Network.maxConnections = 0;

				// active the playZoneTriggers
				PlayZoneTrigger.instance.gameObject.SetActive(true);
			}
		}
	}

	[RPC]
	public void startGame(){
		if( Network.isServer){
			networkView.RPC("startGame",RPCMode.Others);
		}
		StartCoroutine(tickDownTimer(timeDown));
	}
	private IEnumerator tickDownTimer(int seconds){
		// somehow get a reference to the ui of the player...
//		waitingText.text = "Starting the game";
		Notification.instance.Message("Starting the game...",1.5f);
		yield return new WaitForSeconds(1.5f);

		Notification.instance.Message(seconds.ToString(),1.0f);
		while( seconds > 0){
			yield return new WaitForSeconds(1);
			seconds -= 1;
			Notification.instance.Message(seconds.ToString(),1.0f);
		}


//		if( Network.isServer){
//			DemoSceneManager.instance.StartGame(Vector3.zero);
//		}
//		// turn off all the stuff...
//		this.gameObject.SetActive(false);
//		PlayZoneTrigger.instance.gameObject.SetActive(false);

		Application.LoadLevel(gameSceneName);
	}
}
