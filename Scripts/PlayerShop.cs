using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlayerShop : MonoBehaviour {
	
	//visible in the inspector
    public Color[] colors;
	public Player player;
	
	public GameObject leftButton;
	public GameObject rightButton;
	
	public GameObject unlockButton;
	
	public int price;
	public Text diamondsLabel;
	
	public Animator cameraAnim;
	
	int current;
	
	void Start(){
		//get the current player color
		int playerColor = PlayerPrefs.GetInt("Player Color");
		current = playerColor;
		
		//show the correct color, update the left & right buttons and show the correct diamond count
		ChangeDisplayedColor(playerColor);
		UpdateButtons();
		UpdateDiamonds();
	}
	
	//change the player color and update it using the player script
	void ChangeDisplayedColor(int index){
		player.color = colors[index];
		player.UpdateColor(true);
	}
	
	//select the color to the left or right from this color
	public void Navigate(int direction){
		current += direction;
		
		ChangeDisplayedColor(current);
		
		UpdateButtons();
		
		//show a small effect to give some feedback on player input
		cameraAnim.SetTrigger("Effect");
	}
	
	public void Select(){
		//save selected player color and hide shop panel
		PlayerPrefs.SetInt("Player Color", current);
		
		GameObject.FindObjectOfType<GameManager>().Shop();
	}
	
	//check the diamond count, and if sufficient, remove the diamonds and unlock this color
	public void Purchase(){
		int diamonds = PlayerPrefs.GetInt("Diamonds");
		
		if(diamonds < price)
			return;
		
		PlayerPrefs.SetInt("Diamonds", PlayerPrefs.GetInt("Diamonds") - price);
		UpdateDiamonds();
		
		PlayerPrefs.SetInt("Unlocked Color" + current, 1);
		
		//also update the buttons and the lock again
		UpdateButtons();
	}
	
	//update the left/right navigation buttons and the lock button
	void UpdateButtons(){
		leftButton.SetActive(current > 0);
		rightButton.SetActive(current < colors.Length - 1);
		
		bool unlocked = current == 0 || PlayerPrefs.GetInt("Unlocked Color" + current) == 1;
		unlockButton.SetActive(!unlocked);
	}
	
	//show the correct diamond count using playerprefs
	void UpdateDiamonds(){
		diamondsLabel.text = PlayerPrefs.GetInt("Diamonds") + "";
	}
}
