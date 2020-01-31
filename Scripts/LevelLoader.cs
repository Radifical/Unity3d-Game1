using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

[System.Serializable]
public class Level{
	public int length;
	public Color color;
}

public class LevelLoader : MonoBehaviour {
    
	//visible in the inspector
	public Level[] levels;
	
	public LevelGenerator generator;
	public Material backgroundMat;
	
	public Text lastLevel;
	public Text nextLevel;
	
	void Awake(){
		//load the level data
		LoadLevel();
	}
	
	void LoadLevel(){
		//get the level index
		int level = PlayerPrefs.GetInt("Level");
		
		//make sure it's not out of bounds
		if(level > levels.Length - 1)
			level = levels.Length - 1;
		
		//set the level length and adjust the background color
		generator.SetLength(levels[level].length);
		backgroundMat.color = levels[level].color;
		
		//show the correct labels on the progress indicator
		lastLevel.text = "" + (level + 1);
		nextLevel.text = "" + (level + 2);
	}
}
