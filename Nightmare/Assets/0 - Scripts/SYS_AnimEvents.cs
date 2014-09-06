using UnityEngine;
using System.Collections;

public class SYS_AnimEvents : MonoBehaviour {

	//This class holds methods that will be called by the animations
	//Such as the sliding effect in attack chain 1
	//Or any other time-specific functions of an attack
	

	public IEnumerator ATK_Chain1Slide(){
		//Apply forward dash effect to Attack Chain 1
		Debug.Log ("ATK_Chain1Slide");
		SYS_Motor.instance.dashing = true;
		yield return new WaitForSeconds(0.2f);
		SYS_Motor.instance.dashing = false;
	}
}
