﻿using UnityEngine;
using System.Collections;

public class Notification : MonoBehaviour {

	public static Notification instance;
	public UnityEngine.UI.Text text;

	void Awake(){
		Notification.instance = this;
		text = this.transform.Find("Text").GetComponent<UnityEngine.UI.Text>();
		text.text = "";
		this.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(closeHandler);
		this.gameObject.SetActive(false);
	}
	
	public void Message(string msg, float time){
		this.gameObject.SetActive(true);
		StopAllCoroutines();
		text.text = msg;
		if( time != -1){
			StartCoroutine(timeTick(time));
		}
	}

	private void closeHandler(){
		this.gameObject.SetActive(false);
	}
	private IEnumerator timeTick(float time){
		yield return new WaitForSeconds(time);
		this.gameObject.SetActive(false);
	}
}
