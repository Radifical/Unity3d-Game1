using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    
	//visible 
	public Rigidbody rb;
	public float bulletSpeed;
	public float jumpForce;
	public float stretchAmount;
	public Transform mesh;
	public Animator meshAnimator;
	public float force;
	public float glowTime;
	public GameObject explosion;
	public TrailRenderer line;
	public Color color;
	
	public GameObject[] paintSplats;
	public float paintDistance;
	public float muchPaintTreshold;
	
	public Material glow;
	public Material normal;
	public Renderer rend;
	
	public CameraMovement cam;
	
	public Animator bulletAnim;
	public AudioSource jumpAudio;
	
	[HideInInspector]
	public bool bullet;
	
	Vector3 startScale;
	
	bool fullForce;
	bool wait;
	
	bool wasBullet;
	
	Powerup powerup;
	
	[HideInInspector]
	public bool animPlaying;
	
	float yMin;
	bool prepareBreak;
	
	void Start(){
		//save the start mesh scale
		startScale = mesh.localScale;
		line.enabled = false;
		
		//get powerup script
		powerup = GetComponent<Powerup>();
		
		//update player color
		UpdateColor(false);
	}
	
	void Update(){
		//update the line and bullet state
		if(Input.GetMouseButtonDown(0)){
			if(line != null)
				line.enabled = false;
		
			if(!bullet && wasBullet)
				wasBullet = false;
		}
		
		//if the animation is not playing, use velocity to get the player scale
		if(!animPlaying)
			mesh.localScale = startScale + Vector3.up * Mathf.Abs(rb.velocity.y) * stretchAmount + Vector3.right * -Mathf.Abs(rb.velocity.y) * stretchAmount/3f;
	
		if(prepareBreak){
			if(transform.position.y > yMin){
				rb.isKinematic = false;
				
				StartCoroutine(Glow());
		
				fullForce = true;
				prepareBreak = false;
			}
			else{
				transform.Translate(Vector3.up * Time.deltaTime * 5f);
			}
		}
	}
	
	//set the color, and possibly reuse previous alpha value so the paint will be transparent
	public void UpdateColor(bool reuseAlpha){
		Color newColor = color;
		
		if(reuseAlpha){
			Color previous = rend.material.color;
			float alpha = previous.a;
			
			newColor.a = alpha;
		}
		
		//apply color
		rend.material.color = newColor;
	}
	
	//change renderer state for the player mesh
	IEnumerator SetRenderer(float delay, bool state){
		yield return new WaitForSeconds(delay);
		rend.enabled = state;
	}
	
	void OnTriggerEnter(Collider other){	
		//check if we hit an obstacle, and if so die
		if(other.gameObject.CompareTag("Obstacle")){
			if(bullet){
				return;
			}
			else if(wasBullet){
				wasBullet = false;
				other.gameObject.SetActive(false);
				
				return;
			}
			
			Die();
			
			return;
		}
		
		//if we're a bullet, break everything
		if(bullet && (other.gameObject.CompareTag("Breakable") || other.gameObject.CompareTag("Platform"))){
			Platform platform = other.transform.parent.GetComponent<Platform>();
			platform.Break(transform.position);
			
			return;
		}
		
		//stop here if the animation is playing
		//if(animPlaying)
		//	return;
		
		//if we're just jumping, play the jump animation
		if(!fullForce){
			StartCoroutine(Anim());
		}
		else if(!prepareBreak){
			//otherwise, check if we can break through a gate and die otherwise
			if(other.gameObject.transform.parent.CompareTag("Fractured"))
				return;
		
			fullForce = false;
			
			if(other.gameObject.CompareTag("Breakable")){
				Platform platform = other.transform.parent.GetComponent<Platform>();
				platform.BreakCenter();
				
				//shake the camera for a nice effect
				StartCoroutine(cam.Shake(0.1f, 0.15f));
				
				powerup.AddPower();
				
				//rb.velocity = Vector3.up * -force;
			}
			else{
				Die();
			}
		}
	}
	
	//burst down
	public void Go(){
		if(wait)
			return;
		
		if(line != null)
			line.enabled = true;
		
		wait = true;
		
		Platform[] platforms = GameObject.FindObjectsOfType<Platform>();
		float above = Mathf.Infinity;
		float below = -Mathf.Infinity;
		
		for(int i = 0; i < platforms.Length; i++){
			float yPos = platforms[i].transform.position.y;
			
			if(yPos > transform.position.y){
				if(yPos < above)
					above = yPos;
			}
			else{
				if(yPos > below)
					below = yPos;
			}
		}
		
		rb.isKinematic = true;
		yMin = (below + above)/2f;
		
		if(Mathf.Abs(yMin - transform.position.y) > 10f)
			yMin = below + 1f;
		
		meshAnimator.enabled = false;
		animPlaying = false;
		
		prepareBreak = true;
	}
	
	//either disable, or enable the bullet powerup
	public void TriggerBullet(bool state){
		bullet = state;
		bulletAnim.SetBool("Show", state);
			
		cam.smoothness /= state ? 10f : 0.1f;
			
		StartCoroutine(SetRenderer(0.2f, !state));
		
		if(state){
			Vector3 vel = rb.velocity;
			vel.y = -bulletSpeed;
			
			rb.velocity = vel;
			
			wasBullet = true;
		}
			
		rb.useGravity = !state;
		
		//remove obstacles after bullet stops
		if(!state)
			RemoveSurroundingPlatforms();
	}
	
	//remove platforms after the bullet is done so we don't immediately die
	void RemoveSurroundingPlatforms(){
		Platform[] platforms = GameObject.FindObjectsOfType<Platform>();
		
		for(int i = 0; i < platforms.Length; i++){
			if(Mathf.Abs(transform.position.y - platforms[i].transform.position.y) < 3f)
				platforms[i].Break(transform.position);
		}
	}
	
	//show the effect while breaking downwards
	IEnumerator Glow(){		
		rb.velocity = Vector3.up * -force;
		
		rend.material = glow;
		
		yield return new WaitForSeconds(glowTime);
		
		rend.material = normal;
		rend.material.color = color;
		
		wait = false;
		
		//stop displaying the line after breaking is done
		if(line != null)
			line.enabled = false;
	}
	
	//remove player and reload the scene
	void Die(){
		GameObject.FindObjectOfType<GameManager>().ReloadScene(1f);
		
		StartCoroutine(cam.Shake(0.1f, 0.2f));
				
		Instantiate(explosion, transform.position, transform.rotation);
		Destroy(gameObject);
	}
	
	//instantiates the paint effect when player lands
	void PaintEffect(){
		if(PaintClose())
			return;
		
		bool muchPaint = Mathf.Abs(rb.velocity.y) > muchPaintTreshold;
		GameObject paintSplat = muchPaint ? paintSplats[2] : paintSplats[Random.Range(0, paintSplats.Length - 1)];
		
		RaycastHit hit;
		
		if(!Physics.Raycast(transform.position + Vector3.up, -Vector3.up, out hit))
			return;
		
		Vector3 hitPos = hit.point;
		
		hitPos.z = paintSplat.transform.position.z;
		hitPos.y += 0.02f;
		
		Quaternion rot = paintSplat.transform.rotation;
		
		GameObject paint = Instantiate(paintSplat, hitPos, rot);
		
		Transform other = hit.collider.gameObject.transform;
		
		paint.transform.SetParent(other, true);
		
		//update the paint splat color to the current player color
		paint.GetComponent<Paint>().SetColor(color);
	}
	
	//check if there's already paint with the same color before adding more paint
	bool PaintClose(){
		GameObject[] paint = GameObject.FindGameObjectsWithTag("Paint");
		
		int count = 0;
		
		for(int i = 0; i < paint.Length; i++){
			if(Vector3.Distance(transform.position, paint[i].transform.position) < paintDistance && paint[i].GetComponent<Paint>().GetColor() == color){
				count++;
				
				paint[i].GetComponentInChildren<ParticleSystem>().Play();
			}
		}
		
		//return true if there's at least 3 other paint splats of the same color
		return count > 3;
	}
	
	//show the jump animation at the start of a jump
	IEnumerator Anim(){
		animPlaying = true;
		
		meshAnimator.enabled = true;
		meshAnimator.SetTrigger("Play");
		
		PaintEffect();
		rb.velocity = Vector3.up * jumpForce;
		
		jumpAudio.Play();
		
		yield return new WaitForSeconds(1f/2f);
		
		meshAnimator.enabled = false;
		
		animPlaying = false;
	}
}
