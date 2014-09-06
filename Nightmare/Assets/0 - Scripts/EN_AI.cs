using UnityEngine;
using System.Collections;

public class EN_AI : MonoBehaviour {

	public Transform target;  //What the enemy is attacking
	public float moveSpeed = 2.0f;  //How fast the enemy is moving
	public float rotationSpeed = 2.0f;  //how fast he can turn

	private Transform myTransform;

	void Awake(){
		myTransform = transform;
	}

	// Use this for initialization
	void Start () {
		target = GameObject.Find ("Snow").transform;
	}
	
	// Update is called once per frame
	void Update () {
		Debug.DrawLine (target.position, myTransform.position, Color.red);

		//Turn and look at the target
		myTransform.rotation = Quaternion.Slerp (myTransform.rotation, 
		                                         Quaternion.LookRotation (target.position - myTransform.position),
		                                         rotationSpeed * Time.deltaTime);

		//Move to target
		//myTransform.position += myTransform.forward * moveSpeed * Time.deltaTime;
	}
}
