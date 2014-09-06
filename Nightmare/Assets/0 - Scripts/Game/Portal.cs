using UnityEngine;
using System.Collections;

public class Portal : MonoBehaviour {
	public Transform destination;
	public string level;
	
	
	
	void OnTriggerEnter(Collider other){
		if(other.tag == "Player"){
			//If the destination variable is set, use that to teleport to a place within the level.
			if(destination != null){
				other.transform.position = destination.position;
			}else {
				Application.LoadLevel (level);
			}
		}
	}
}
