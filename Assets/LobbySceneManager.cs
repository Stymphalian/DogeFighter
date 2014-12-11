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
		if(Network.isServer){
			Debug.LogError("I am the server");
			Network.Instantiate(playerShipPrefab,new Vector3(spawnPoint.transform.position.x - 30, spawnPoint.transform.position.y + 10, spawnPoint.transform.position.z) ,Quaternion.identity,0);
		}else if(Network.isClient){
			Debug.LogError("I am the client");
			Network.Instantiate(playerShipPrefab,new Vector3(spawnPoint.transform.position.x + 30, spawnPoint.transform.position.y + 10, spawnPoint.transform.position.z),Quaternion.identity,0);

		}
	}
		
	// called on the server when a player is connected...
	void OnPlayerConnected(NetworkPlayer player){
		instructions ();
		if (Network.isServer) {
			// no more players can connect...
			Network.maxConnections = 0;
			networkView.RPC("instructions",RPCMode.Others);
		}
		if( playZoneTriggerActive == true){return;}
		if( Network.connections.Length == Config.MAX_NUM_PLAYERS){
			playZoneTriggerActive = true;
			if( Network.isServer){
				// active the playZoneTriggers
				PlayZoneTrigger.instance.gameObject.SetActive(true);
			}
		}
	}

	[RPC]
	public void instructions() {
		Debug.Log("displaying instructions");
		StartCoroutine(displayInstructions());
	}

	IEnumerator displayInstructions(){
		Notification.instance.Message ("Welcome to DOGEFIGHTERS");
		yield return new WaitForSeconds (3.0f);
		Notification.instance.Message ("Both players have connected");
		yield return new WaitForSeconds (3.0f);
		Notification.instance.Message ("Fly to the sun start!");
		yield return new WaitForSeconds (3.0f);
		Notification.instance.Message ("Have fun! :)");
		yield return new WaitForSeconds (3.0f);
		Notification.instance.hideMessage ();
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


		// turn off all the stuff...
		this.enabled = false;
		PlayZoneTrigger.instance.enabled = false;

		if( Network.isServer){
			DemoSceneManager.instance.StartGame(Vector3.zero);
		}


//		Application.LoadLevel(gameSceneName);
	}
}
