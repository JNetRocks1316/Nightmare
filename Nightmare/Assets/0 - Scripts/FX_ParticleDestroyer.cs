using UnityEngine;
using System.Collections;

public class FX_ParticleDestroyer : MonoBehaviour {

	void Start(){
		Invoke("Destroy", 5.0f);
	}

	void Death(){
		Destroy(this.gameObject);
	}
}
