using UnityEngine;
using System.Collections;

public class SYS_Con : MonoBehaviour {

	public static CharacterController charController;
	public static SYS_Con instance;


	void Awake(){
		instance = this;
		charController = GetComponent("CharacterController") as CharacterController;
	}

	void Update(){
		if(!SYS_Status.instance.isDead){
			GetMoveInput(); //Check for player input for movement
			GetActionInput();  //Check for action inputs
		}
		SYS_Motor.instance.UpdateMotor (); //Send the new moveVector to the motor
	}

	void GetMoveInput(){
		SYS_Motor.instance.verticalVelocity = SYS_Motor.instance.moveVector.y;

		if(SYS_Status.instance.isDead)
			return;

		var deadZone = 0.1f;

		//Zero out movement so speed is constant rather than additive
		SYS_Motor.instance.moveVector = Vector3.zero;

		//W/S Buttons set moveVector to move horizontally(forward)
		if (Input.GetAxis ("Vertical") > deadZone || Input.GetAxis ("Vertical") < -deadZone){
			SYS_Motor.instance.moveVector += new Vector3(0, 0, Input.GetAxis ("Vertical"));
		}

		//A/D Buttons set moveVector to move horizontally(side)
		if(Input.GetAxis ("Horizontal") > deadZone || Input.GetAxis ("Horizontal") < -deadZone)
			SYS_Motor.instance.moveVector += new Vector3(Input.GetAxis ("Horizontal"), 0, 0);

		//Have SYS_Anim determine what direction to animate
		SYS_Anim.instance.FindMoveDir();
	}

	void GetActionInput(){
		if(Input.GetButtonDown ("Jump")){
			Jump();
		}

		if(Input.GetKeyDown("mouse 0")){
			SYS_Status.instance.Attack();
		}

		if(Input.GetKeyDown ("left ctrl")){
			Dodge();
		}

		if(Input.GetKeyDown ("tab")){
			SYS_Status.instance.TargetEnemy ();
		}
	}

	public void Jump(){
		if(charController.isGrounded || SYS_Motor.instance.isClimbing){
			Debug.Log ("SYS_Con Jump:  isGrounded = " + SYS_Con.charController.isGrounded);
			SYS_Motor.instance.Jump ();
			SYS_Anim.instance.Jump ();
		}
	}

	public void Dodge(){
		Debug.Log ("Dodge");
	}

}
