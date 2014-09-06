using UnityEngine;
using System.Collections;

public class SYS_Cam : MonoBehaviour {

	public static SYS_Cam instance;

	public bool camFrozen = false;

	//Camera Position
	public Transform CamLookAtTarget;

	public float distance = 2f; //Current distance of camera;
	public float defaultDistance = 1f;
	public float distanceMin = 0.5f; //Closest camera can get to target
	public float distanceMax = 2f; //Furthest camera can get from target
	public float distanceSmooth = 0.05f; //Time for camera to smooth on normal smoothing
	public float distanceResumeSmooth = 0.1f; //How smooth camera returns to pre-occluded distance
	public float x_camSpeed = 6f;
	public float y_camSpeed = 6f;
	public float zoomSpeed = 2.5f;
	public float x_smooth = 0.15f;
	public float y_smooth = 0.15f;
	public float y_minLimit = -55;  //Minimum angle of the camera (looking up)
	public float y_maxLimit = 85;  //Maximum angle of the camera (looking down)
	public float occlusionSteps = 0.1f;
	public int occlusionChecks = 10;

	private float mouseX = 0f; //Cameras current X rotation
	private float mouseY = 0f;
	private float velX = 0f;  //velocity of camera on X axis
	private float velY = 0f;  //velocity of camera on Y axis
	private float velZ = 0f;  //velocity of camera on Z axis
	private float velDistance = 0f; //Velocity of camera during distance smoothing
	private Vector3 position = Vector3.zero;  //Position of camera
	private Vector3 desiredPosition = Vector3.zero;  //desired position of the camera
	private float desiredDistance = 0f; //desired distance of camera
	private float occlusionSmooth = 0f;  //the distance of the camera during occlusion distance smoothing
	private float preOccludedDist = 0;  //The distance of the camera before occlusion occurs

	void Awake(){
		instance = this;
	}

	void Start(){
		distance = Mathf.Clamp (distance, distanceMin, distanceMax);
		CamLookAtTarget = GameObject.Find ("TargetLookAt").transform;
		Reset();
		ToggleCursor(false, true);
	}

	public void Reset(){
		Vector3 angles = transform.parent.eulerAngles;

		mouseX = angles.y; //Camera is behind the player
		mouseY = 10;  //Camera is rotated slightly downward
		distance = defaultDistance;
		desiredDistance = defaultDistance; //Cameras desired position is start position (don't move)
		preOccludedDist = defaultDistance;
	}

	void LateUpdate(){ 
		//If there's no target, don't do anything
		if(CamLookAtTarget == null)
			return;

		//If the player is alive, then allow camera control
		if(!SYS_Status.instance.isDead){
			//Listen for input and update camera
			GetInput();
		} else {
			//If the player is dead, then lock the camera into a slow rotation
			desiredDistance = 3;
			mouseX += Time.deltaTime * 10;
			mouseY = 40f;
		}

		var count = 0;
		do{
			//Calculate the position the camera should move to based on input
			FindDesiredPos();
			count++;
		} while (Occlusion(count));

		CheckCameraPoints(CamLookAtTarget.position, desiredPosition); //Draw occlusion box
		UpdatePosition(); //Move the Camera
	}


	//Listens for camera control input from the player.
	void GetInput(){
		var deadZone = 0.01f;

		/*Must hold mouse button down to rotate camera around
		if(Input.GetMouseButtonDown (1))
			ToggleCursor(false, true);
		if(Input.GetMouseButtonUp (1))
			ToggleCursor(true, false);
		if(Input.GetMouseButton (1)){
			if((Input.GetAxis("Mouse X") > deadZone) || (Input.GetAxis ("Mouse X") < -deadZone))
				mouseX += Input.GetAxis ("Mouse X") * x_camSpeed;
			if((Input.GetAxis("Mouse Y") > deadZone) || (Input.GetAxis ("Mouse Y") < -deadZone))
				mouseY -= Input.GetAxis ("Mouse Y") * y_camSpeed;
			//Clamp Y-Mouse movement so cam doesn't go over players head
			mouseY = SYS_Help.ClampAngle(mouseY, y_minLimit, y_maxLimit);
		}
		*/

		//No mouse-holding required for cam steering
		if((Input.GetAxis("Mouse X") > deadZone) || (Input.GetAxis ("Mouse X") < -deadZone))
			mouseX += Input.GetAxis ("Mouse X") * x_camSpeed;
		if((Input.GetAxis("Mouse Y") > deadZone) || (Input.GetAxis ("Mouse Y") < -deadZone))
			mouseY -= Input.GetAxis ("Mouse Y") * y_camSpeed;
		//Clamp Y-Mouse movement so cam doesn't go over players head
		mouseY = SYS_Help.ClampAngle(mouseY, y_minLimit, y_maxLimit);


		//Cam Zoom with mouse wheel
		if((Input.GetAxis ("Mouse ScrollWheel") < -deadZone) || (Input.GetAxis ("Mouse ScrollWheel") > deadZone)){
			desiredDistance = Mathf.Clamp (
				distance - Input.GetAxis ("Mouse ScrollWheel") * zoomSpeed,
				distanceMin,
				distanceMax);
			preOccludedDist = desiredDistance;
			occlusionSmooth = distanceSmooth;
		}
	}


	//Sets the cursor visibility and movement
	public void ToggleCursor(bool show, bool freeze){
		Screen.showCursor = show;
		Screen.lockCursor = freeze;
	}

	//Figures out where the camera is going and evaluates distance with smoothing
	void FindDesiredPos(){
		ResetDesiredDistance();
		distance = Mathf.SmoothDamp (distance, desiredDistance, ref velDistance, occlusionSmooth);

		//Set desired position to new calculated position
		desiredPosition = CalcPos(mouseY, mouseX, distance);
	}
	
	Vector3 CalcPos(float rotationX, float rotationY, float distance){
		//Create a direction vector that points behind character
		Vector3 direction = new Vector3(0, 0, -distance);

		Quaternion rotation = Quaternion.Euler (rotationX, rotationY, 0);
		//Add TargetLookAt.position to rotation and direction to offset the camera
		return CamLookAtTarget.position + rotation * direction;
	}

	//Determines if the camera is currently occluded
	bool Occlusion(int count){
		var isOccluded = false;
		var nearestDistance = CheckCameraPoints(CamLookAtTarget.position, desiredPosition);

		//Evaluate the nearestDistance
		if(nearestDistance != -1){
			if(count < occlusionChecks){
				isOccluded = true;
				distance -= occlusionSteps;
			} else {
				distance = nearestDistance - Camera.main.nearClipPlane;
			}

			if(distance < distanceMin){
				//Switch to FPCamera**************************************************************************************** PLACEHOLDER  *** FIRST PERSON CAMERA SWITCH
				//For now, turn off occlusion so camera doesn't jerk around
				distance = Mathf.Clamp (distance, distanceMin, distanceMax);
			}
			
			desiredDistance = distance;
			distanceSmooth = distanceResumeSmooth;
		}
		return isOccluded;
	}


	//Checks for the nearest point from Camera to something occluding it
	float CheckCameraPoints(Vector3 from, Vector3 to){
		var nearestDistance = -1f;  //Hasn't collided with anything
		
		//Compare if any camera rays are hitting something
		
		RaycastHit hitInfo;
		SYS_Help.ClipPlanePoints clipPlanePoints = SYS_Help.ClipPlaneAtNear(to);
		
		//Draw lines in the editor to visualize camera occlusion area
		Debug.DrawLine (from, to + transform.forward * -camera.nearClipPlane, Color.red);
		
		Debug.DrawLine (from, clipPlanePoints.UpperLeft);
		Debug.DrawLine (from, clipPlanePoints.UpperRight);
		Debug.DrawLine (from, clipPlanePoints.LowerLeft);
		Debug.DrawLine (from, clipPlanePoints.LowerRight);
		
		Debug.DrawLine (clipPlanePoints.UpperLeft, clipPlanePoints.UpperRight);
		Debug.DrawLine (clipPlanePoints.UpperRight, clipPlanePoints.LowerRight);
		Debug.DrawLine (clipPlanePoints.LowerRight, clipPlanePoints.LowerLeft);
		Debug.DrawLine (clipPlanePoints.LowerLeft, clipPlanePoints.UpperLeft);
		
		//Do line casts to check if the camera area is colliding with anything (ignores Player and System tags)
		if(Physics.Linecast (from, clipPlanePoints.UpperLeft, out hitInfo) && (hitInfo.collider.tag != "Player" && hitInfo.collider.tag != "System"))
			nearestDistance = hitInfo.distance;
		
		if(Physics.Linecast (from, clipPlanePoints.LowerLeft, out hitInfo) && (hitInfo.collider.tag != "Player" && hitInfo.collider.tag != "System"))
			if(hitInfo.distance < nearestDistance || nearestDistance == -1)
				nearestDistance = hitInfo.distance;
		
		if(Physics.Linecast (from, clipPlanePoints.UpperRight, out hitInfo) && (hitInfo.collider.tag != "Player" && hitInfo.collider.tag != "System"))
			if(hitInfo.distance < nearestDistance || nearestDistance == -1)
				nearestDistance = hitInfo.distance;
		
		if(Physics.Linecast (from, clipPlanePoints.LowerRight, out hitInfo) && (hitInfo.collider.tag != "Player" && hitInfo.collider.tag != "System"))
			if(hitInfo.distance < nearestDistance || nearestDistance == -1)
				nearestDistance = hitInfo.distance;
		
		if(Physics.Linecast (from, to + transform.forward * -camera.nearClipPlane, out hitInfo) && (hitInfo.collider.tag != "Player" && hitInfo.collider.tag != "System"))
			if(hitInfo.distance < nearestDistance || nearestDistance == -1)
				nearestDistance = hitInfo.distance;
		
		
		return nearestDistance;
	}

	void ResetDesiredDistance(){
		//The camera has been fixed by occlusion
		if(desiredDistance < preOccludedDist){
			//Get the previous location
			var pos = CalcPos(mouseY, mouseX, preOccludedDist);
			//Check the camera points to see if they are occluded
			var nearestDistance = CheckCameraPoints(CamLookAtTarget.position, pos);
			//If not, then move the camera back to preOccludedDistance
			if (nearestDistance == -1 || nearestDistance > preOccludedDist){
				desiredDistance = preOccludedDist;
			}
		}
	}

	void UpdatePosition(){
		float posX = 0f;
		float posY = 0f;
		float posZ = 0f;
		
		//Smooth X, Y, and Z seperately
		posX = Mathf.SmoothDamp(position.x, desiredPosition.x, ref velX, x_smooth);
		posY = Mathf.SmoothDamp(position.y, desiredPosition.y, ref velY, y_smooth);
		posZ = Mathf.SmoothDamp(position.z, desiredPosition.z, ref velZ, x_smooth);

		//Assemble result into a vector
		position = new Vector3(posX, posY, posZ);
		
		//Move the camera to the new calculated position and rotate to face target
		transform.position = position;
		transform.LookAt (CamLookAtTarget);
	}

}
