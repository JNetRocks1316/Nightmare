﻿using UnityEngine;
using System.Collections;

public class BTN_NewGame: MonoBehaviour {

	private GUITexture iconNewGame;
	private int saveGame = 0;  //0 for no game saved, 1 for a saved game 

	private Color fadeColor = new Color(0.5f, 0.5f, 0.5f, 0.05f);
	private Color regColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
	private Color highlightColor = new Color(0.5f, 0.5f, 0.5f, 0.8f);
	

	void Start(){
		iconNewGame = transform.GetComponent ("GUITexture") as GUITexture;
		//Check if there is a save game available

		//If no save is found, set new game Icon to enabled
		if(saveGame == 0)
			iconNewGame.color = regColor;
		else if (saveGame == 1)
			iconNewGame.color = fadeColor;
	}

	void OnMouseEnter(){
		if(saveGame == 0){
			iconNewGame.color = highlightColor; //Mouse hover effect
		}
	}

	void OnMouseExit(){
		if(saveGame == 0){
			iconNewGame.color = regColor; //Mouse hover off
		}
	}

	void OnMouseDown(){
		if(saveGame == 0){
			Debug.Log ("MouseClick");
			Application.LoadLevel ("Intro");
		}
	}
}
