using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Remove : MonoBehaviour {
	
	public bool longer;
    
	void Start(){
		//wait for a bit and remove this object
		Destroy(gameObject, longer ? 3 : 1);
	}
}
