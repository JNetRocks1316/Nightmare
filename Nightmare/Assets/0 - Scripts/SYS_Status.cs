using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SYS_Status : MonoBehaviour {

	public static SYS_Status instance;
	private Transform myTransform;
	public float maxHealth = 100.0f;
	public float curHealth = 100.0f;
	public bool isDead = false;

	public bool inCombat = false;  //Holds a state for whether Snow is in combat

	//Attack Variables
	public bool atkRDY = true;  //Can atk be used?
	public bool chainRDY = false; //If attacking, will it add a chain?
	public int atkChainStep = 1;  //Current step in attack chain
	public float atkCD = 0.5f; //Cooldown between atks
	public float chainTimer = 0.0f; //Time player has left to chain an atk

	//Sword Variables
	public GameObject sword;
	public GameObject swordSummonFX;

	//Enemy Variables
	public Transform target;
	public List<Transform> targets;

	//Tracks the current place that Snow should respawn
	//Currently set to start position in scene - but can update this to take a waypoint/checkpoint easily
	public Vector3 respawnPosition = Vector3.zero;
	public Quaternion respawnRotation = Quaternion.identity;

	//Used for GUI Respawn Button
	public float screenH;
	public float screenW;

	void Awake(){
		instance = this;
	}

	void Start(){
		//Get default position so we respawn at the start of the level
		respawnPosition = transform.position;
		respawnRotation = transform.rotation;
		screenH = Screen.height;
		screenW = Screen.width;
		sword = GameObject.Find ("Sword");
		InCombat(false);
		SheatheSword();
		target = null;
		targets = new List<Transform>();
		AddTargets();
		myTransform = transform;
	}

	public void AddTargets(){
		GameObject[] go = GameObject.FindGameObjectsWithTag ("Enemy");

		foreach(GameObject enemy in go)
			AddTarget(enemy.transform);
	}

	void AddTarget(Transform target){
		targets.Add (target);
	}

	private void SortTargets(){
		targets.Sort (delegate(Transform t1, Transform t2){
			return Vector3.Distance (t1.position, myTransform.position).CompareTo (
				Vector3.Distance (t2.position, myTransform.position));
		});
	}

	public void TargetEnemy(){
		if(target == null){
			SortTargets();
			target = targets[0];
		} else {
			int index = targets.IndexOf (target);

			if(index < targets.Count - 1){
				index++;
			} else {
				index = 0;
			}
			target = targets[index];
		}
	}


	void Update(){
		if(chainRDY == true){
			chainTimer -= Time.deltaTime;
			if(chainTimer <= 0.1f){
				chainRDY = false;
				chainTimer = 0.0f;
			}
		}
	}

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
		isDead = true;
		SYS_Anim.instance.Die ();
		//Make sure the cursor is available for clicking the respawn button.
		SYS_Cam.instance.ToggleCursor (true, false);
		//Make the scene darker to emphasize death
		RenderSettings.fogColor = new Color(0.034f, 0.042f, 0.051f, 1.000f);
		RenderSettings.fogDensity = 0.15f;
		RenderSettings.ambientLight = new Color(0.301f, 0.301f, 0.301f, 1.00f);
	}

	public void Respawn(){
		isDead = false;
		SYS_Anim.instance.Respawn ();
		//Return us to our respawn position
		transform.position = respawnPosition;
		transform.rotation = respawnRotation;
		//Make sure the cursor is not visible.
		SYS_Cam.instance.ToggleCursor(false, true);
		//Make the scene brighter to emphasize alive state
		RenderSettings.fogColor = new Color(0.060f, 0.074f, 0.096f, 1.000f);
		RenderSettings.fogDensity= 0.02f;
		RenderSettings.ambientLight = new Color (0.699f, 0.6991f, 0.699f, 1.00f);
	}



	//Combat State Flag
	public void InCombat(bool combat){
		if(combat == true){
			inCombat = true;
			SYS_Anim.instance.Combat (true);
		} else if(combat == false) {
			inCombat = false;
			SYS_Anim.instance.Combat (false);
		}
	}

	void SummonSword(){
		if(sword.activeSelf == false){
			//Turn on the sword graphics.
			GameObject FX = GameObject.Instantiate(swordSummonFX, sword.transform.position, sword.transform.rotation) as GameObject;
			FX.transform.parent = sword.transform;
			sword.SetActive(true);
		}
	}

	void SheatheSword(){
		//If we're not in combat, turn off the sword graphics.
		GameObject FX = GameObject.Instantiate(swordSummonFX, sword.transform.position, sword.transform.rotation) as GameObject;
		FX.transform.parent = this.transform;
		sword.SetActive (false);
	}

	#region Attacks
	//Base Attack Chain
	public void Attack(){
		if(atkRDY == false){
			atkChainStep = 1;
			return;
		}
		//Set atkRDY to false so player can't attack again right away
		atkRDY = false;
		
		//Check if the attack was performed in time to do a chain attack
		if(chainRDY){
			//If the player attacked in time, increment the chain to the next step
			if(atkChainStep < 3){
				atkChainStep++;
			} else
				//Or if the step is at 3, reset it to 1
				atkChainStep = 1;
		} else{
			//The player attacked too slow to chain and only performs the basic attack
			atkChainStep = 1;
		}
		
		//Perform whichever attack we are currently on in chain count
		if(atkChainStep == 1)
			SYS_Status.instance.Attack1();
		else if(atkChainStep == 2)
			SYS_Status.instance.Attack2();
		else if (atkChainStep == 3)
			SYS_Status.instance.Attack3();

		SummonSword();
		CancelInvoke("SheatheSword");
		Invoke("SheatheSword", 3.0f);

		//Set the basic attack cooldown
		Invoke("SetAtkRdy", atkCD);
	}

	//Attack Cooldown
	void SetAtkRdy(){
		atkRDY = true;
		chainRDY = true;
		chainTimer = 0.50f;
	}
	
	public void Attack1(){
		float range = 1.5f;
		float damage = -5.0f;
		RaycastHit hit;
		if(Physics.Raycast (transform.position, transform.TransformDirection (Vector3.forward), out hit)){
			float distance = hit.distance;
			
			if(distance <= range){
				Debug.Log ("Hitting: " + hit.collider.name);
				hit.transform.SendMessage ("AdjCurHealth", damage, SendMessageOptions.DontRequireReceiver);
			}
		}

		SYS_Sound.instance.ATK_Cry();
		SYS_Anim.instance.animator.CrossFade ("ATK_Chain1", 0.2f, 2);
	}


	public void Attack2(){
		float range = 1.0f;
		float damage = -10.0f;
		RaycastHit hit;
		if(Physics.Raycast (transform.position, transform.TransformDirection (Vector3.forward), out hit)){
			float distance = hit.distance;
			
			if(distance <= range){
				Debug.Log ("Hitting: " + hit.collider.name);
				hit.transform.SendMessage ("AdjCurHealth", damage, SendMessageOptions.DontRequireReceiver);
			}
		}

		SYS_Sound.instance.ATK_Cry();
		SYS_Anim.instance.animator.CrossFade ("ATK_Chain2", 0.2f, 2);
	}
	
	public void Attack3(){
		float range = 1.0f;
		float damage = -25.0f;
		RaycastHit hit;
		if(Physics.Raycast (transform.position, transform.TransformDirection (Vector3.forward), out hit)){
			float distance = hit.distance;
			
			if(distance <= range){
				Debug.Log ("Hitting: " + hit.collider.name);
				hit.transform.SendMessage ("AdjCurHealth", damage, SendMessageOptions.DontRequireReceiver);
			}
		}

		SYS_Sound.instance.ATK_Cry();
		SYS_Anim.instance.animator.CrossFade ("ATK_Chain3", 0.2f, 2);
	}

	#endregion

	void OnGUI(){
		if(isDead){
			//Show a button to allow the player to respawn only if Snow is currently dead
			if(GUI.Button (new Rect(screenW/2 - 40, screenH/2 - 10, 80, 30), "Respawn")){
				//If the button is pressed, repsawn the character
				Respawn();
			}
		} else {
			//Non-Dead Status UI stuff goes here - mostly temporary stuff
			GUI.Label(new Rect(10, 10, 300, 20), "Snow HP:  " + curHealth + "/" + maxHealth);
			GUI.Label(new Rect(350, 10, 200, 20), "ChainATK Step: " + atkChainStep);
			GUI.Label (new Rect(350, 40, 200, 20), "ChainATK Time:  " + chainTimer.ToString ("F2"));
		}
	}
}
