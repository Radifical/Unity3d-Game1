using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Paint : MonoBehaviour {
	
	//visible in the inspector
	public Image[] images;
	public ParticleSystemRenderer particles;
	
	//set one color for all paint images and also update the particle effect color
    public void SetColor(Color color){
		for(int i = 0; i < images.Length; i++){
			images[i].color = color;
		}
		
		particles.material.color = color;
	}
	
	//returns this paint color
	public Color GetColor(){
		return particles.material.color;
	}
}
