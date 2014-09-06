using UnityEngine;

public static class SYS_Help{
	
	
	//Structure to save the points for the camera clip plane
	public struct ClipPlanePoints{
		public Vector3 UpperLeft;
		public Vector3 UpperRight;
		public Vector3 LowerLeft;
		public Vector3 LowerRight;
	}
	
	//Clamp an angle between a min and max, used by camera to calculate Y-Axis angle
	public static float ClampAngle(float angle, float min, float max){
		do{
			//Remove revolutions to clamp angle between -360 and 360
			if(angle < -360)
				angle += 360;
			if(angle > 360)
				angle -= 360;
		} while (angle < -360 || angle > 360);
		
		return Mathf.Clamp (angle, min, max);
	}
	
	
	public static ClipPlanePoints ClipPlaneAtNear(Vector3 pos){
		var clipPlanePoints = new ClipPlanePoints();  //Holds the calculated clipplane point
		
		//Make sure there's a camera
		if(Camera.main == null)
			return clipPlanePoints;
		
		var transform = Camera.main.transform;  //Holds the camera transform
		var halfFOV = (Camera.main.fieldOfView / 2) * Mathf.Deg2Rad;  //Get the field of view as radians
		var aspect = Camera.main.aspect; //Camera aspect ratio
		var distance = Camera.main.nearClipPlane;  //Distance from camera to near clip plane
		var height = distance * Mathf.Tan (halfFOV); //Height of clip plane
		var width = height * aspect;  //Width of clip plane
		
		//Set the clip plane point positions
		clipPlanePoints.LowerRight = pos + transform.right * width;
		clipPlanePoints.LowerRight -= transform.up * height;
		clipPlanePoints.LowerRight += transform.forward * distance; //Move the clip plane forward to distance
		
		clipPlanePoints.LowerLeft = pos - transform.right * width;
		clipPlanePoints.LowerLeft -= transform.up * height;
		clipPlanePoints.LowerLeft += transform.forward * distance; //Move the clip plane forward to distance
		
		clipPlanePoints.UpperRight = pos + transform.right * width;
		clipPlanePoints.UpperRight += transform.up * height;
		clipPlanePoints.UpperRight += transform.forward * distance; //Move the clip plane forward to distance
		
		clipPlanePoints.UpperLeft = pos - transform.right * width;
		clipPlanePoints.UpperLeft += transform.up * height;
		clipPlanePoints.UpperLeft += transform.forward * distance; //Move the clip plane forward to distance
		
		return clipPlanePoints;
	}
}
