using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour {
	
	//visible in the inspector
    public GameObject fractured;
	public GameObject layerFractured;
	public GameObject center;
	public float moveDelay;
	public float moveSpeed;
	public Transform left;
	public Transform right;
	public GameObject[] obstacles;
	
	bool move;
	
	void Update(){
		//if the center is broken, move sides outwards
		if(move){
			left.Translate(Vector3.right * moveSpeed * -Time.deltaTime);
			right.Translate(Vector3.right * moveSpeed * Time.deltaTime);
		}
	}
	
	public void BreakCenter(){
		//break the orange center piece
		GameObject fracturedNew = Instantiate(fractured, center.transform.position, center.transform.rotation);
		fracturedNew.transform.SetParent(transform.parent, true);
		
		GameObject.FindObjectOfType<GameManager>().AddPoints(10);
		
		Destroy(center);
		
		//move sides outwards
		StartCoroutine(Move());
	}
	
	public void Break(Vector3 playerPos){
		//break everything and move the spikes/obstacles away as well
		for(int i = 0; i < obstacles.Length; i++){
			if(obstacles[i] == null || !obstacles[i].activeSelf)
				continue;
			
			obstacles[i].transform.parent = null;
			
			Rigidbody rb = obstacles[i].GetComponent<Rigidbody>();
			rb.isKinematic = false;
			
			Vector3 dir = (rb.transform.position - playerPos).normalized;
			dir.y = Random.Range(-1f, 1f);
			
			rb.AddForce(dir * 500);
			rb.AddTorque(new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * 500);
			
			obstacles[i].GetComponent<Collider>().enabled = false;
			
			obstacles[i].AddComponent<Remove>();
		}
		
		//instantiate the fractured layer and destroy the actual layer
		GameObject fractured = Instantiate(layerFractured, transform.position, transform.rotation);
		fractured.AddComponent<Remove>();
		Destroy(gameObject);
	}
	
	//make sure to remove this layer after moving the platforms to the side
	IEnumerator Move(){
		yield return new WaitForSeconds(moveDelay);
		
		move = true;
		
		yield return new WaitForSeconds(1);
		
		Destroy(gameObject);
	}
}
