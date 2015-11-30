using UnityEngine;
using System.Collections;
using System;

public class GoogleAds : MonoBehaviour
{
    private static GoogleAds instance = null;
    public static GoogleAds _Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (new GameObject("GoogleAds")).AddComponent<GoogleAds>();
            }
            return instance;
        }
    }

    int _iBannerId;

    public event Action OnInterstitialLoaded;
    public event Action OnInterstitialLoadFail;
    public event Action OnInterstitialClosed;

    public bool _bLoadInterstital;

    public void Init()
    {
        _bLoadInterstital = false;

        UM_AdManager.instance.Init();

        GoogleMobileAd.OnInterstitialLoaded += HandleOnInterstitialLoaded;
        GoogleMobileAd.OnInterstitialFailedLoading += HandleOnInterstitialLoadFail;
        GoogleMobileAd.OnInterstitialClosed += HandleOnInterstitialClosed;

        CreateBanner();
        CreateInterstital();
    }

    void HandleOnInterstitialClosed()
    {
        Debug.Log("Interstitial Ad was closed");

        _bLoadInterstital = false;

        if (OnInterstitialClosed != null)
            OnInterstitialClosed();
    }

    void HandleOnInterstitialLoadFail()
    {
        Debug.Log("Interstitial is failed to load");

        _bLoadInterstital = false;

        if (OnInterstitialLoadFail != null)
            OnInterstitialLoadFail();
    }

    void HandleOnInterstitialLoaded()
    {
        Debug.Log("Interstitial ad content ready");

        _bLoadInterstital = true;

        if (OnInterstitialLoaded != null)
            OnInterstitialLoaded();
    }

    // Banner
    public void CreateBanner()
    {
        _iBannerId = UM_AdManager.instance.CreateAdBanner(TextAnchor.LowerCenter);
    }

    public void ShowBanner()
    {
        UM_AdManager.instance.ShowBanner(_iBannerId);
    }

    public void HideBanner()
    {
        UM_AdManager.instance.HideBanner(_iBannerId);
    }

    public bool IsBanner()
    {
        return UM_AdManager.instance.IsBannerOnScreen(_iBannerId);
    }


    // Interstitial
    public void CreateInterstital()
    {
        UM_AdManager.instance.StartInterstitialAd();
    }

    public void LoadInterstitial()
    {
        UM_AdManager.instance.LoadInterstitialAd();
    }

    public void ShowInterstital()
    {
        UM_AdManager.instance.ShowInterstitialAd();
    }
}
