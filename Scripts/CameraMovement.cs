using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {

	//Variables visible in the inspector
	public float smoothness;
	public float yOffset;
	public Transform camTarget;	
	
	Vector3 velocity;
	
	bool shaking;
	Vector3 lastTarget;
 
    void LateUpdate(){
		//Check if the camera has a target to follow
        if(!camTarget || shaking){
			if(lastTarget != Vector3.zero && !shaking)
				transform.position = Vector3.SmoothDamp(transform.position, lastTarget, ref velocity, smoothness/2f);
			
            return;
		}
		
		//get target position
		Vector3 pos = GetTargetPosition();
		
		float distance = Vector3.Distance(transform.position, pos);
		
		if(distance < 0.1f)
			distance = 0.1f;
		
		//update position
		transform.position = Vector3.SmoothDamp(transform.position, pos, ref velocity, smoothness/distance);
    }
	
	//calculates the target position based on the cam target
	Vector3 GetTargetPosition(){
		Vector3 pos = transform.position;
		pos.y = camTarget.position.y + yOffset;
		
		lastTarget = pos;
		
		return pos;
	}
	
	//shakes the camera for a nice break effect
	public IEnumerator Shake(float duration, float amount){
		Vector3 pos = transform.position;
		shaking = true;
		
		float elapsed = 0f;
		
		while(elapsed < duration){
			float x = Random.Range(-1f, 1f) * amount;
			float y = Random.Range(-1f, 1f) * amount;
			float z = Random.Range(-1f, 1f) * amount;
			
			transform.position += new Vector3(x, y, z);
			
			elapsed += Time.deltaTime;
			
			yield return 0;
		}
		
		transform.position = pos;
		shaking = false;
	}
}