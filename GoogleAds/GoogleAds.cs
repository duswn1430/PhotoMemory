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

    //public bool _bLoadInterstital;
    public bool _bInterstitialLoaded;

    public void Init()
    {
        _bInterstitialLoaded = false;

        UM_AdManager.instance.Init();

        GoogleMobileAd.OnInterstitialLoaded += HandleOnInterstitialLoaded;
        GoogleMobileAd.OnInterstitialFailedLoading += HandleOnInterstitialLoadFail;
        GoogleMobileAd.OnInterstitialClosed += HandleOnInterstitialClosed;

        CreateBanner();
        //CreateInterstital();
        LoadInterstitial();
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

    public bool IsBannerOnScreen()
    {
        return UM_AdManager.instance.IsBannerOnScreen(_iBannerId);
    }

    public bool IsBannerLoaded()
    {
        return UM_AdManager.instance.IsBannerLoaded(_iBannerId);
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

    void HandleOnInterstitialClosed()
    {
        Debug.Log("Interstitial Ad was closed");

        //_bLoadInterstital = false;

        if (OnInterstitialClosed != null)
        {
            LoadInterstitial();

            OnInterstitialClosed();
            OnInterstitialClosed = null;
        }
    }

    void HandleOnInterstitialLoadFail()
    {
        Debug.Log("Interstitial is failed to load");

        //_bLoadInterstital = false;

        if (OnInterstitialLoadFail != null)
        {
            OnInterstitialLoadFail();
            OnInterstitialLoadFail = null;
        }
    }

    void HandleOnInterstitialLoaded()
    {
        Debug.Log("Interstitial ad content ready");

        _bInterstitialLoaded = true;

        if (OnInterstitialLoaded != null)
        {
            OnInterstitialLoaded();
            OnInterstitialLoaded = null;
        }
    }
}
