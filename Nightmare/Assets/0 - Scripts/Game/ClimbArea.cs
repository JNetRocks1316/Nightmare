using UnityEngine;
using System.Collections;

public class ClimbArea : MonoBehaviour {

	void OnTriggerEnter(Collider other){
		if(other.gameObject.name == "Snow"){
			other.transform.rotation = this.transform.rotation;
			SYS_Motor.instance.isClimbing = true;
			Debug.Log ("Climbing Area entered");
		}
	}

	void OnTriggerExit(Collider other){
		if(other.gameObject.name == "Snow"){
			SYS_Motor.instance.isClimbing = false;
			Debug.Log ("Climbing Area left");
		}
	}
}
