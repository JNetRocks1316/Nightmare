using UnityEngine;
using System.Collections;

public class SYS_Anim : MonoBehaviour {

	public static SYS_Anim instance;
	public Animator animator;
	public Direction moveDirection {get; set;}
	public string curState;
	public int animState;
	public States State { get; set; }

	//Available states for moveDirection
	public enum Direction{
		Stationary, Forward, Backward, Left, Right,
		LeftForward, RightForward, LeftBackward, RightBackward
	}

	public enum States{
		Move, Jump, Fall, Land, Dead
	}
	
	void Awake()
	{
		instance = this;
		animator = transform.Find ("SK_Snow").GetComponent<Animator>();
		animator.applyRootMotion = false;
		animator.SetLayerWeight (1, 0);
		animator.SetLayerWeight (2, 1.0f);
		State = States.Move;
	}
	
	// Update is called once per frame
	void Update () 
	{
		animator.SetFloat ("XSpeed", Input.GetAxis ("Horizontal"));
		animator.SetFloat ("ZSpeed", Input.GetAxis ("Vertical"));
		//DetermineCurrentState();
		DoCurState();
		curState = State.ToString ();
		//Debug.Log ("State: " + State + "     Animation State: " + animator.GetInteger("State"));
	}

	public void FindMoveDir()
	{
		//Look at the move vector from SYS_Motor and determine what direction we're moving
		var forward = false;
		var backward = false;
		var left = false;
		var right = false;

		if(SYS_Motor.instance.moveVector.z > 0)
			forward = true;
		if(SYS_Motor.instance.moveVector.z < 0)
			backward = true;
		if(SYS_Motor.instance.moveVector.x > 0)
			right = true;
		if(SYS_Motor.instance.moveVector.x < 0)
			left = true;

		//Check all possible combinations of directionals to get move direction
		if(forward)	{
			if(left)
				moveDirection = Direction.LeftForward;
			else if (right)
				moveDirection = Direction.RightForward;
			else
				moveDirection = Direction.Forward;
		} else if (backward){
			if(left)
				moveDirection = Direction.LeftBackward;
			else if (right)
				moveDirection = Direction.RightBackward;
			else
				moveDirection = Direction.Backward;
		} else if (left){
			moveDirection = Direction.Left;
		} else if (right){
			moveDirection = Direction.Right;
		} else {
			moveDirection = Direction.Stationary;
		}
	}

	//Determins whether we should be in the move state OR initiates the fall state.
	void FindCurState()
	{
		if(State != States.Fall && State != States.Jump && State != States.Land){
			if(SYS_Con.charController.isGrounded)
				State = States.Move;
			else{
				State = States.Fall;
			}
		}
	}

	//Triggers the specific functions for each state.
	void DoCurState(){
		switch(State){
		case States.Jump:
			animState = 1;
			Jumping();
			break;
		case States.Fall:
			animState = 2;
			Falling();
			break;
		case States.Land:
			animState = 3;
			Landing();
			break;
		case States.Move:
			animState = 0;
			Moving();
			break;
		case States.Dead:
			animState = 4;
			Dead();
			break;
		}

		animator.SetInteger ("State", animState);
	}

	#region State Methods
	void Moving(){

		//UPDATED
		/* Used Tristan's suggestion of using a raycast down to see how far off the ground we are.
		 * If we are walking and go down a slight bump it will no longer trigger a chance to fall state.
		 * The distance beneath Snow to fall must be greater than 0.5f (can tweak the number).
		 */
		if(SYS_Con.charController.isGrounded){
			State = States.Move;
		} else if(!SYS_Con.charController.isGrounded){
			RaycastHit hit;
			if(Physics.Raycast (transform.position, transform.TransformDirection (Vector3.down), out hit)){
				float distance = hit.distance;
				if(distance > 0.5f){
					//We're falling a small amount, transition to falling
					Fall();
					Debug.Log ("Moved over a ledge and now falling");
				}
			}
		}
	}

	void Jumping(){
		//While in the jump state, if we become grounded, switch to landing.
		if (SYS_Con.charController.isGrounded)
		{
			//We've landed
			State = States.Land;
		}

		//If the jump state has finished and we're not grounded, switch to falling state
		else if (animator.GetCurrentAnimatorStateInfo(0).IsName ("Fall")){
			//We're falling
			State = States.Fall;
			SYS_Motor.instance.IsFalling = true;
		}
	}

	void Falling(){
		//While in the falling state, if we become grounded, switch to landing.
		if(SYS_Con.charController.isGrounded){
			animator.SetBool ("Fall", false);
			SYS_Motor.instance.IsFalling = false;
			State = States.Land;
		}
	}
	
	void Landing(){
		//If the landing animation has finished, switch to move state.
		if(animator.GetCurrentAnimatorStateInfo(0).IsName ("Move")){
			State = States.Move;
		}
	}



	void Dead(){
		State = States.Dead;
	}

	#endregion

	#region Action Methods
	public void Fall(){
		animator.SetBool ("Fall", true);
		State = States.Fall;
		SYS_Motor.instance.IsFalling = true;
	}

	public void Jump(){
		//Reset the animation for Jump every time you jump
		animator.Play ("Jump", 0, 0f);
		State = States.Jump;
	}

	public void Die(){
		//Initialize everything we need to die
		State = States.Dead;
	}

	public void Respawn(){
		//Reset character so we can play again
		State = States.Move;
	}

	//Changes the weight of the Combat Layer based on whether the player is in combat or not
	public void Combat(bool inCombat){
		//Get the current weight of the Combat layer and set the new target to either
		//1.0 (in combat) or 0.0 (not in combat).  Begin a coroutine to blend between the weights
		if(inCombat){
			StartCoroutine(BlendWeight(1, animator.GetLayerWeight (1), 1.0f));
		}
		else{
			StartCoroutine(BlendWeight(1, animator.GetLayerWeight (1), 0.0f));
		}
	}

	IEnumerator BlendWeight(int layer, float from, float to){
		//Takes in the layer that we want to blend, the weight it is currently at
		//And the weight we want to make it and smoothly transitions the weight over time
		if(to < from){
			while (to < from){
				from -= Time.deltaTime;
				animator.SetLayerWeight (layer, from);
				yield return null;
			}
		} else if (to > from){
			while (to > from){
				from += Time.deltaTime;
				animator.SetLayerWeight (layer, from);
				yield return null;
			}
		}
	}
	#endregion
	
}
