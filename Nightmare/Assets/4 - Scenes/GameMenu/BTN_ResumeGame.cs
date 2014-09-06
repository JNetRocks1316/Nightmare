using UnityEngine;
using System.Collections;

public class BTN_ResumeGame: MonoBehaviour {

	private GUITexture iconResumeGame;
	private int saveGame = 0;  //0 for no game saved, 1 for a saved game 

	private Color fadeColor = new Color(0.5f, 0.5f, 0.5f, 0.1f);
	private Color regColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
	private Color highlightColor = new Color(0.5f, 0.5f, 0.5f, 0.7f);
	

	void Start(){
		iconResumeGame = transform.GetComponent ("GUITexture") as GUITexture;
		//Check if there is a save game available

		//If no save is found, set new game Icon to enabled
		if(saveGame == 0)
			iconResumeGame.color = fadeColor;
		else if(saveGame == 1)
			iconResumeGame.color = regColor;
	}

	void OnMouseEnter(){
		if(saveGame == 1){
			iconResumeGame.color = highlightColor; //Mouse hover effect
		}
	}

	void OnMouseExit(){
		if(saveGame == 1){
			iconResumeGame.color = regColor; //Mouse hover off
		}
	}

	void OnMouseDOwn(){
		if(saveGame == 1){
			//Load the game at the resume point
		}
	}
}
