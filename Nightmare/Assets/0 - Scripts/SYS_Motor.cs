using UnityEngine;
using System.Collections;

public class SYS_Motor : MonoBehaviour {

	//Variables
	public static SYS_Motor instance;
	public bool alignWithCam = true;
	public bool dashing;

	public float fSpeed = 2.5f;
	public float bSpeed = 1.5f;
	public float strafeSpeed = 2.0f;
	public float slideSpeed = 2.5f;

	public float jumpSpeed = 3f;
	public float gravity = 5f;
	public float fallSpeed = 15f; //Maximum downward velocity
	public float slideThresh = 0.7f; //Angle character begins sliding on slopes
	public float maxSlopeAngle = 0.3f;  //Angle player loses control on slopes
	public float fatalFallHeight = 7f;  //How far we can fall before dying
	private bool isFalling = false;
	public bool isClimbing = false;

	/* New Bool - Called by SYS_MecAnim during state changes to let the motor now it is now in the fall state.
	 * Tracks starting position when called as true.
	 * Calculates fall distance when called as false.
	 * If the fall distance is terminal, calls Death
	 */
	public bool IsFalling{
		get{ return isFalling; }
		set
		{ 
			isFalling = value;

			if(isFalling){
				startFallHeight = transform.position.y;
			} else {
				if(startFallHeight - transform.position.y > fatalFallHeight){
					SYS_Status.instance.Die();
					Debug.Log ("SYS_Motor IsFalling:  Fall Height too high, calling SYS_Status DIE()");
				} else {
					Debug.Log ("SYS_Motor IsFalling:  Fall Height isn't high - just fall damage.");
					//Fall isn't fatal
					//Implement fall-damage here
				}
			}
		}
	}

	private Vector3 slideDirection; //Direction player is sliding in
	public Vector3 moveVector {get; set;} //Players move direction horizontally
	public float verticalVelocity {get; set;} //Direction player is moving vertically
	private float startFallHeight;  //Height at which we started falling
	

	void Awake(){
		instance = this;
		alignWithCam = true;
	}

	public void UpdateMotor(){
		if(!isClimbing){
			if(alignWithCam)
				SnapAlignCharacterWithCamera(); //Rotates the player for camera steering.
			ProcessMotion(); //Move the character.
		}
		else
			ProcessClimbMotion();
	}

	void ProcessMotion(){
		if(!SYS_Status.instance.isDead){
			//If the player is alive, get their moveVector
			moveVector = transform.TransformDirection(moveVector);

			//if dashing is true, override with forward dash speed
			if(dashing)
				SYS_Con.charController.Move (transform.forward * 2.5f * Time.deltaTime);
		} else{
			//If the player is dead only allow them to fall
			moveVector = new Vector3(0, moveVector.y, 0);
		}

		//Normalize moveVector
		if(moveVector.magnitude > 1)
			moveVector = Vector3.Normalize (moveVector);

		ApplySlide();
		//Set a new magnitude
		moveVector *= MoveSpeed();
		//Reapply vertical velocity
		moveVector = new Vector3(moveVector.x, verticalVelocity, moveVector.z);
		ApplyGravity();

		//Move character in World Space in time per second
		SYS_Con.charController.Move(moveVector * Time.deltaTime);

	}

	void ProcessClimbMotion(){
		//Turn horizontal forward/backward movement into veritcal upward/downward movement
		//Make it relative to the current direction of Snow then move.
		var newMoveVector = new Vector3(moveVector.x, moveVector.z, 0f);
		newMoveVector = transform.TransformDirection (newMoveVector);
		moveVector = newMoveVector;
		SYS_Con.charController.Move (moveVector * Time.deltaTime);


		//Check if we're grounded and turn off climbing
		if(SYS_Con.charController.isGrounded)
			isClimbing = false;
	}

	void ApplyGravity(){
		//If the player is not falling at max fall speed than increase fall speed by gravity
		if(moveVector.y > -fallSpeed)
			moveVector = new Vector3(moveVector.x, moveVector.y - gravity * Time.deltaTime, moveVector.z);
		//If the player is grounded, keep gravity steady
		if(SYS_Con.charController.isGrounded && moveVector.y < -1)
			moveVector = new Vector3(moveVector.x, -1, moveVector.z);
	}

	void ApplySlide(){
		//If the player isn't grounded, don't do anything
		if(!SYS_Con.charController.isGrounded)
			return;
		//If the player is grounded, zero out the slide direction
		slideDirection = Vector3.zero;
		//Cast a ray to see the slope beneath the character
		RaycastHit hitInfo;
		if(Physics.Raycast (transform.position + Vector3.up, Vector3.down, out hitInfo)){
			//If the normal returned by the ray is less than the threshold for sliding, slide to the normal of the slope
			if(hitInfo.normal.y < slideThresh)
				slideDirection = new Vector3(hitInfo.normal.x, -hitInfo.normal.y, hitInfo.normal.z);
		}

		//If the slide direction angle is within controllable range, apply slide to movement
		if(slideDirection.magnitude < maxSlopeAngle)
			moveVector += slideDirection;
		else
			//If the slope is too sleep, just slide
			moveVector = slideDirection;
	}

	public void Jump(){
		verticalVelocity = jumpSpeed;
		//Backwards Velocity away from wall (Snow's Current rotation)
		Debug.Log ("Jump");
		isClimbing = false;
	}

	public void SnapAlignCharacterWithCamera(){
		//If the player is moving forward/backward or side/side
		if(moveVector.x != 0 || moveVector.z != 0)
			transform.rotation = Quaternion.Euler (
				transform.eulerAngles.x,
				Camera.main.transform.eulerAngles.y, //Rotate char to camera's rotation
				transform.eulerAngles.z);
	}

	float MoveSpeed(){
		//Default moveSpeed to 0
		var moveSpeed = 0f;
		
		//Check moveDirection
		switch(SYS_Anim.instance.moveDirection){
		case SYS_Anim.Direction.Stationary:
			moveSpeed = 0;
			break;
		case SYS_Anim.Direction.Forward:
			moveSpeed = fSpeed;
			break;
		case SYS_Anim.Direction.Backward:
			moveSpeed = bSpeed;
			break;
		case SYS_Anim.Direction.Left:
			moveSpeed = strafeSpeed;
			break;
		case SYS_Anim.Direction.Right:
			moveSpeed = strafeSpeed;
			break;
		case SYS_Anim.Direction.LeftForward:
			moveSpeed = fSpeed;
			break;
		case SYS_Anim.Direction.RightForward:
			moveSpeed = fSpeed;
			break;
		case SYS_Anim.Direction.LeftBackward:
			moveSpeed = bSpeed;
			break;
		case SYS_Anim.Direction.RightBackward:
			moveSpeed = bSpeed;
			break;
		}
		
		if(slideDirection.magnitude > 0)
			moveSpeed = slideSpeed;
		
		return moveSpeed;
	}
}
