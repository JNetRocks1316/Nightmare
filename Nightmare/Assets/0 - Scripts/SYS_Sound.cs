using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SYS_Sound : MonoBehaviour {

	public static SYS_Sound instance;

	public AudioSource AS;
	public AudioClip atkCry1;
	public AudioClip atkCry2;
	public AudioClip atkCry3;
	public AudioClip atkCry4;
	public AudioClip atkWep1;
	public AudioClip atkWep2;
	public AudioClip atkWep3;

	void Awake(){
		instance = this;
	}

	void Start(){
		AS = GetComponent("AudioSource") as AudioSource;
	}


	public void ATK_Cry(){

		int RNG = Random.Range (0, 100);

		if (RNG > 20 && RNG <= 40)
			AS.PlayOneShot (atkCry1);
		else if (RNG > 40 && RNG <= 60)
			AS.PlayOneShot (atkCry2);
		else if (RNG > 60 && RNG <= 80)
			AS.PlayOneShot (atkCry3);
		else if (RNG > 80 && RNG <= 100)
			AS.PlayOneShot (atkCry4);

		ATK_Wep();

	}

	public void ATK_Wep(){
		int RNG = Random.Range (0, 100);
		if(RNG <= 33)
			AS.PlayOneShot (atkWep1);
		else if (RNG > 33 && RNG <= 66)
			AS.PlayOneShot(atkWep2);
		else if (RNG > 66 && RNG <= 100)
			AS.PlayOneShot(atkWep3);
	}
}
