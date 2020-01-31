using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pitch : MonoBehaviour {
    
	void Start(){
		//randomizes pitch and volume for break and explosion sounds
		AudioSource source = GetComponent<AudioSource>();
		source.pitch = Random.Range(0.6f, 0.75f);
		source.volume = Random.Range(0.5f, 0.7f);
	}
}
