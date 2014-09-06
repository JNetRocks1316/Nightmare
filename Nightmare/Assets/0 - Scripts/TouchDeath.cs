using UnityEngine;
using System.Collections;

public class TouchDeath : MonoBehaviour {
	void OnTriggerEnter(Collider other){
		if (other.gameObject.name == "Snow") {
						SYS_Status.instance.Die ();
				}
		}
}
