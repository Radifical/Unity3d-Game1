using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class Controls : MonoBehaviour {
	
	//visible in the inspector
	public float sensitivity;
	public Player player;
	public float bulletFollowSpeed;
	
	float start;
	Vector3 levelStart;
	
	Transform level;
	GameManager manager;
	
	bool canGo;
	
	void Start(){
		//get the manager and the level transform
		manager = GameObject.FindObjectOfType<GameManager>();
		level = GameObject.FindObjectOfType<LevelGenerator>().GetLevel();
	}
	
	void Update(){
		//if we're game over, don't do anything
		if(manager.gameOver)
			return;
		
		//move to the platform if the player is currently using the bullet powerup
		if(player.bullet){
			MoveToPlatform();
			
			return;
		}
		
		float current = Input.mousePosition.x;
		
		//get the mouse position to drag the level around
		if(Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject() && !manager.shopPanel.activeSelf){
			if(!(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))){
				start = current;
			
				levelStart = level.position;
				canGo = true;
			}
		}
		else if(Input.GetMouseButton(0) && canGo){
			float difference = current - start;
			
			Vector3 levelPos = level.position;
			levelPos.x = levelStart.x + (difference * sensitivity);
			level.position = levelPos;
		}
		else if(Input.GetMouseButtonUp(0) && player != null && canGo){
			//when mouse button/finger gets released, break through the gate
			player.Go();
			
			canGo = false;
		}
	}
    
	//track the nearest platforms and move towards those
	void MoveToPlatform(){
		Platform[] platforms = GameObject.FindObjectsOfType<Platform>();
		
		float smallestDistance = Mathf.Infinity;
		Platform closest = null;
		
		for(int i = 0; i < platforms.Length; i++){
			float dist = Mathf.Abs(transform.position.y - platforms[i].transform.position.y);
			
			if(dist < smallestDistance){
				smallestDistance = dist;
				closest = platforms[i];
			}
		}
		
		if(closest == null)
			return;
		
		float direction = closest.transform.position.x < player.transform.position.x ? 1f : -1f;
		
		level.Translate(Vector3.right * direction * bulletFollowSpeed * Time.deltaTime, Space.World);
	}
}
