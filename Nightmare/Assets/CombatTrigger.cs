using UnityEngine;
using System.Collections;

public class CombatTrigger : MonoBehaviour {


	void OnTriggerEnter(Collider other){
		if(other.gameObject.name == "Snow"){
			SYS_Status.instance.InCombat (true);
		}
	}

	void OnTriggerExit(Collider other){
		if(other.gameObject.name == "Snow"){
			SYS_Status.instance.InCombat (false);
		}
	}
}
