using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class LevelGenerator : MonoBehaviour {
    
	//visible in the inspector
	public float dist;
	public float range;
	public GameObject[] platforms;
	public GameObject finalPlatform;
	public float obstacleRemoveDistance;
	public int randomObstacles;
	
	public Transform player;
	public CameraMovement camMovement;
	public GameManager manager;
	public Animator gameOver;
	public Text gameOverDiamonds;
	public Animator diamondsAnim;
	
	int sameSideCount;
	bool lastLeft;
	
	float lastPosX;
	
	float lowestPoint;
	
	bool done;
	
	Transform parent;
	int levelLength;
	
	void Awake(){
		//create a parent transform to parent the platforms to
		parent = new GameObject().transform;
		parent.name = "Level";
	}
	
	void Start(){		
		//spawn all platforms for this level
		for(int i = 0; i < levelLength; i++){
			SpawnNew(i == levelLength - 1);
		}
	}
	
	public void SpawnNew(bool lastOne){
		//create new platform
		GameObject platform = lastOne ? finalPlatform : platforms[Random.Range(0, platforms.Length)];
		GameObject newPlatform = Instantiate(platform, transform.position, transform.rotation);
		
		//set finish line on the manager script
		if(lastOne)
			manager.SetFinishLine(newPlatform.transform);
		
		Vector3 pos = newPlatform.transform.position;
		
		float randomX = Random.Range(-range, range);
		bool left = randomX < 0;
		
		if(left == lastLeft)
			sameSideCount++;
		
		if(sameSideCount > 2){
			randomX *= -1f;
			sameSideCount = 0;
		}
		
		lastLeft = randomX < 0;
		
		pos.x = lastPosX + randomX;
		
		//set the platform x position and parent it to the level parent transform
		newPlatform.transform.position = pos;
		newPlatform.transform.SetParent(parent, true);
		
		//move the level generator down for the next layer
		transform.Translate(Vector3.up * -dist);
		
		//get the platform script 
		Platform platformScript = newPlatform.GetComponent<Platform>();
		
		if(platformScript != null){			
			//randomly enable/disable obstacles to randomize the level
			foreach(GameObject obstacle in platformScript.obstacles){
				obstacle.SetActive(Random.Range(0, randomObstacles) == 0);
				
				if(Mathf.Abs(obstacle.transform.position.x - lastPosX) < obstacleRemoveDistance){
					obstacle.SetActive(false);
				}
				else{
					//also randomize spike scale
					float random = Random.Range(0.8f, 1.1f);
					obstacle.transform.localScale *= random;
					
					obstacle.transform.Translate(Vector3.forward * -(1.1f - random) * .3f);
				}
			}
		}
		
		lastPosX = pos.x;
		
		//set the lowest level point
		if(lastOne)
			lowestPoint = pos.y + 1f;
	}
	
	void Update(){
		//if we've reached the level bottom by breaking through the gates
		if(player != null && player.position.y < lowestPoint && !done){
			done = true;
			
			camMovement.yOffset = 2;
			
			gameOver.SetTrigger("Show");
			
			//show diamond count and save the new diamonds
			int score = manager.GetScore();
			gameOverDiamonds.text = "+" + score;
			PlayerPrefs.SetInt("Diamonds", PlayerPrefs.GetInt("Diamonds") + score);
			
			diamondsAnim.SetTrigger("Show");
			
			//unlock next level
			PlayerPrefs.SetInt("Level", PlayerPrefs.GetInt("Level") + 1);
			
			//show confetti
			FinishLine finish = GameObject.FindObjectOfType<FinishLine>();
			StartCoroutine(finish.Confetti());
			
			//disable the game panel and play some sound
			manager.gameOver = true;
			manager.gamePanel.SetBool("Show", false);
			manager.win.Play();
			
			//if the powerup was active, deactivate it
			Player player = GameObject.FindObjectOfType<Player>();
			Powerup powerup = GameObject.FindObjectOfType<Powerup>();
			
			if(player.bullet)
				powerup.DisableBullet();
		}
	}
	
	//returns the level transform
	public Transform GetLevel(){
		return parent;
	}
	
	//set levellength (before generating)
	public void SetLength(int levelLength){
		this.levelLength = levelLength;
	}
	
	//get level length
	public int GetLength(){
		return this.levelLength;
	}
}
