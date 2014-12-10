using UnityEngine;
using System.Collections;

public class MainScreenUI : MonoBehaviour {

	public string levelName = "Demo";
	public void PlayButtonHandler(){
		NetworkManager.instance.Play(playCallback);
	}

	public void QuitButtonHandler(){
		Application.Quit();
	}

	private void playCallback(){
		Application.LoadLevel(levelName);
	}
}
