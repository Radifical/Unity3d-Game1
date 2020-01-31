using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Powerup : MonoBehaviour {
	
	//visible in the inspector
    public Image barFill;
	public float effectSpeed;
	public float amount;
	public float removePower;
	public float powerupLength;
	
	public GameObject bulletLight;
	
	float targetFill;
	
	Player player;
	
	void Start(){
		//get the player and the bullet light effect
		player = GetComponent<Player>();
		bulletLight.SetActive(false);
	}
	
	void Update(){
		//smoothly transition the powerup fill indicator
		if(targetFill > 0)
			targetFill -= Time.deltaTime * removePower;
		
		if(barFill.fillAmount < targetFill){
			barFill.fillAmount += Time.deltaTime * effectSpeed;
			
			if(barFill.fillAmount > targetFill)
				barFill.fillAmount = targetFill;
		}
		else if(barFill.fillAmount > targetFill){
			barFill.fillAmount -= Time.deltaTime * effectSpeed;
			
			if(barFill.fillAmount < targetFill)
				barFill.fillAmount = targetFill;
		}
		
		//when the bar is full, start the powerup
		if(barFill.fillAmount >= 1f)
			FullPower();
	}
	
	//reset powerup fill, and start powerup
	void FullPower(){
		targetFill = 0;
		
		StartCoroutine(Go());
	}
	
	IEnumerator Go(){
		//bar.SetBool("Show", false);
		player.TriggerBullet(true);
		bulletLight.SetActive(true);
		
		//wait for the duration of the powerup before stopping the bullet again
		yield return new WaitForSeconds(powerupLength);
		
		if(player.bullet)
			DisableBullet();
		
		//bar.SetBool("Show", true);
	}
	
	//add some power by increasing the target fill amount
	public void AddPower(){
		targetFill += amount;
		
		//if(targetFill > 1)
		//	targetFill = 1;
	}
	
	//disable the bullet and hide bullet light effect
	public void DisableBullet(){
		player.TriggerBullet(false);
		bulletLight.SetActive(false);
	}
}
