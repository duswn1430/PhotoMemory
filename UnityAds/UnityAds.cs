using UnityEngine;
using System.Collections;
using UnityEngine.Advertisements;
using System;

public class UnityAds : MonoBehaviour
{
    private static UnityAds instance = null;
    public static UnityAds _Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (new GameObject("UnityAds")).AddComponent<UnityAds>();
            }
            return instance;
        }
    }

    public event Action OnAdFinished;
    public event Action OnAdFailed;

    public void ShowRewardedAd()
    {
        if (Advertisement.IsReady("rewardedVideo"))
        {
            var options = new ShowOptions { resultCallback = HandleShowResult };
            Advertisement.Show("rewardedVideo", options);
        }
    }

    private void HandleShowResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                Debug.Log("The ad was successfully shown.");
                if (OnAdFinished != null)
                {
                    OnAdFinished();
                    OnAdFinished = null;
                }
                break;
            case ShowResult.Skipped:
                Debug.Log("The ad was skipped before reaching the end.");
                break;
            case ShowResult.Failed:
                Debug.LogError("The ad failed to be shown.");
                if (OnAdFailed != null)
                {
                    OnAdFailed();
                    OnAdFailed = null;
                }
                break;
        }
    }

    public void IsLoaded()
    {
        Advertisement.IsReady();
    }
}
