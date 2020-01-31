using System.Collections;
using UnityEngine;

#if UNITY_ADS
using UnityEngine.Advertisements;

public class AdManagerRewarded : MonoBehaviour, IUnityAdsListener {
    
	[Header("Only necessary if no AdManager is present")]
	public string gameId = "1234567";

    void Awake(){   
		AdManager adManager = GameObject.FindObjectOfType<AdManager>();
		
		if(adManager != null)
			gameId = adManager.gameId;
		
        Advertisement.AddListener(this);
        Advertisement.Initialize(gameId);
    }

    public void ShowRewardedVideo(){
		if(Advertisement.IsReady("rewardedVideo")){
			Advertisement.Show("rewardedVideo");
		}
		else{
			Debug.LogWarning("Rewarded ad not ready");
		}
    }

    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult){
        if(showResult == ShowResult.Finished){
            Debug.Log("Successfully finished ad; player will be rewarded");
			
			RewardPlayer();
        } 
		else if(showResult == ShowResult.Skipped){
            Debug.Log("Skipped ad; no reward");
        } 
		else if(showResult == ShowResult.Failed){
            Debug.LogWarning("Error; ad did not finish");
        }
    }
	
	void RewardPlayer(){
		//REWARD PLAYER HERE
	}
	
	public void OnUnityAdsReady(string placementId){
        // If the ready Placement is rewarded, activate the button: 
        if(placementId == "rewardedVideo")   
            Debug.Log("Rewarded video ready");
    }

    public void OnUnityAdsDidError(string message){
        // Log the error.
    }

    public void OnUnityAdsDidStart(string placementId){
        // Optional actions to take when the end-users triggers an ad.
    } 
}

#else
public class AdManagerRewarded : MonoBehaviour {
}

#endif