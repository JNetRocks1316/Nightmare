using UnityEngine;
using System.Collections;

public class Intro : MonoBehaviour {

	void Awake(){
		Invoke("NextLevel", 3);
	}

	void NextLevel(){
		Application.LoadLevel ("HUB");
	}
}
