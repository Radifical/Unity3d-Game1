using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLine : MonoBehaviour {
	
	//confetti
	public GameObject[] confetti;
	
	//play the confetti effect by enabling the objects one by one
	public IEnumerator Confetti(){
		for(int i = 0; i < 3; i++){
			confetti[i].SetActive(true);
			
			yield return new WaitForSeconds(0.2f);
		}
	}
}
