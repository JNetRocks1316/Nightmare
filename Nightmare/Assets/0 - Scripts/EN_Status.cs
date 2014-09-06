using UnityEngine;
using System.Collections;

public class EN_Status : MonoBehaviour {

	
	public float maxHealth = 100.0f;
	public float curHealth = 100.0f;

	
	public void AdjCurHealth (float adj){
		curHealth += adj;
		
		if(curHealth < 0){
			curHealth = 0;
			Die();
		}
		
		if(curHealth > maxHealth)
			curHealth = maxHealth;
		
	}
	
	
	public void Die(){
		//Temporarily refill health to max
		curHealth = maxHealth;
	}
	

	
	void OnGUI(){
		GUI.Label(new Rect(10, 40, 300, 20), "Enemy HP:  " + curHealth + "/" + maxHealth);
	}
}


