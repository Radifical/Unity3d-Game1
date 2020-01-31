using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;

public class GameManager : MonoBehaviour {
	
	//visible in the inspector
	public Text[] scoreTexts;
	public Animator scoreAnim;
	public Animator fade;
	public Animator startPanel;
	public Animator gamePanel;
	public Animator shopLabel;
	public GameObject shopPanel;
	public GameObject tutorialPanel;
	public Animator camZoom;
	public CameraMovement cam;
	
	public Image progressBar;
	
	public AudioSource lose;
	public AudioSource win;
	
	[HideInInspector]
	public bool gameOver;
	
	int score;
	bool loading;
	
	bool started;
	
	Transform finish;
	Transform player;
	
	float startDistance;
	
	void Start(){
		//deactivate the shop and camera script
		shopPanel.SetActive(false);
		cam.enabled = false;
		
		//get the player
		player = GameObject.FindObjectOfType<Player>().transform;
	}
	
	void Update(){
		if(Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()){
			if(!(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))){
				//start the game when we first drag finger on the screen
				if(!started && !shopPanel.activeSelf)
					StartGame();
			}
			
			//if it's not yet loading, but we want to continue, load the next level
			if(gameOver && !loading)
				NextLevel();
		}
		
		//show progress by getting the distance between the player and the finish line
		if(finish != null && player != null){
			float distance = Vector3.Distance(finish.position, player.position);
			
			float percentage = (startDistance - distance)/startDistance;
			progressBar.fillAmount = percentage;
		}
	}
	
	//start the game by showing game UI and enabling the camera script
	void StartGame(){
		startPanel.SetTrigger("Fade out");
		gamePanel.SetBool("Show", true);
		
		cam.enabled = true;
		camZoom.enabled = false;
		
		started = true;
	}
	
	//reload scene for the next level
	void NextLevel(){
		StartCoroutine(Load(0));
		
		loading = true;
	}
    
	//play lose sound and reload scene to restart this level
	public void ReloadScene(float delay){
		TryAd();
		lose.Play();
		
		StartCoroutine(Load(delay));
	}
	
	//wait for a while, show the black fade effect and load the current scene
	IEnumerator Load(float delay){
		yield return new WaitForSeconds(delay);
		
		fade.SetTrigger("Fade");
		
		yield return new WaitForSeconds(0.4f);
		
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
	
	//add some points to the score
	public void AddPoints(int points){
		score += points;
		
		StartCoroutine(UpdateScore());
	}
	
	//return score
	public int GetScore(){
		return score;
	}
	
	//update the score text and show a small effect
	IEnumerator UpdateScore(){
		scoreAnim.SetTrigger("Effect");
		
		yield return new WaitForSeconds(1f/6f);
		
		for(int i = 0; i < scoreTexts.Length; i++){
			scoreTexts[i].text = score == 0 ? "" : "" + score;
		}
	}
	
	//open or close the shop depending on its current state
	public void Shop(){
		shopPanel.SetActive(!shopPanel.activeSelf);
		tutorialPanel.SetActive(!tutorialPanel.activeSelf);
		
		camZoom.SetBool("Zoom", !camZoom.GetBool("Zoom"));
		
		shopLabel.SetBool("Show", !shopLabel.GetBool("Show"));
	}
	
	//set the finish line transform and get the starting distance between the player and the finish line
	public void SetFinishLine(Transform finish){
		this.finish = finish;
		
		if(finish != null && player != null)
			startDistance = Vector3.Distance(finish.position, player.position);
	}
	
	void TryAd(){
       #if UNITY_ADS
        int ad = PlayerPrefs.GetInt("AdCounter");

        if (ad < 3)
        {
            PlayerPrefs.SetInt("AdCounter", ad + 1);
            return;
        }
        else
        {
            PlayerPrefs.SetInt("AdCounter", 0);
        }

        AdManager adManager = GameObject.FindObjectOfType<AdManager>();
		
		if(adManager == null)
			return;
		
		adManager.Interstitial();
		#endif
	}
}
