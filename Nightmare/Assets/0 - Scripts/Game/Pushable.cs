using UnityEngine;
using System.Collections;

public class Pushable : MonoBehaviour {

	public float pushPower = 2.0f;

	void OnControllerColliderHit(ControllerColliderHit hit) {
		Debug.Log ("Hit");
		Rigidbody body = hit.collider.attachedRigidbody;
		if (body == null || body.isKinematic)
			return;
		
		if (hit.moveDirection.y < -0.3F)
			return;
		
		Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
		body.velocity = pushDir * pushPower;
	}

}
