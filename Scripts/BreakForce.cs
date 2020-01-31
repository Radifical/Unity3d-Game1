using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakForce : MonoBehaviour {
    
	//visible in the inspector
	public Rigidbody[] rbs;
	public float force;
	
	void Start(){
		//find the player and get its position
		Player player = GameObject.FindObjectOfType<Player>();
		Vector3 playerPos = player == null ? transform.position : player.transform.position;
		
		//use the direction between the player and the pieces to add force and torque
		foreach(Rigidbody rb in rbs){
			Vector3 dir = (rb.transform.position - playerPos).normalized;
			dir.y = Random.Range(-1f, 1f);
			
			rb.AddForce(dir * force);
			rb.AddTorque(new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * force);
		}
	}
}
