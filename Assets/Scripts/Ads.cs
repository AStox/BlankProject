using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using GoogleMobileAds.Api;
using System;

public class Ads : Singleton<Ads>
{
  public BannerView bannerView;
  public RewardBasedVideoAd rewardBasedVideo;
  bool rewardBasedEventHandlersSet = false;
  public GameObject continueButton;
  public GameObject bannerBackgroundBlank;

  bool rewarded = false;

  void Start()
  {
#if UNITY_ANDROID
      string appId = "ca-app-pub-1728144482852364~1717657691";
#elif UNITY_IPHONE
    string appId = "ca-app-pub-1728144482852364~1887202175";
#else
      string appId = "unexpected_platform";
#endif

    // Initialize the Google Mobile Ads SDK.
    MobileAds.Initialize(appId);

    rewardBasedVideo = RewardBasedVideoAd.Instance;

  }

  public void RequestBanner()
  {
#if UNITY_EDITOR
    string adUnitId = "unused";
#elif UNITY_ANDROID
      string adUnitId = "ca-app-pub-1728144482852364/8829860958";
#elif UNITY_IPHONE
      string adUnitId = "ca-app-pub-1728144482852364/4732857369";
#else
      string adUnitId = "unexpected_platform";
#endif



    // Create a 320x50 banner at the top of the screen.
    bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Top);
    //bannerView = new BannerView(adUnitId, AdSize.SmartBanner, AdPosition.Top);

    // Called when an ad request has successfully loaded.
    bannerView.OnAdLoaded += HandleOnAdLoaded;
    // Called when an ad request failed to load.
    bannerView.OnAdFailedToLoad += HandleOnAdFailedToLoad;
    // Called when an ad is clicked.
    bannerView.OnAdOpening += HandleOnAdOpened;
    // Called when the user returned from the app after an ad click.
    bannerView.OnAdClosed += HandleOnAdClosed;
    // Called when the ad click caused the user to leave the application.
    bannerView.OnAdLeavingApplication += HandleOnAdLeftApplication;

    // Create an empty ad request.
    AdRequest request = new AdRequest.Builder().Build();
    // Load the banner with the request.
    bannerView.LoadAd(request);
    bannerView.Show();
  }

  public void RequestRewardBasedVideo()
  {
#if UNITY_EDITOR
    string adUnitId = "unused";
#elif UNITY_ANDROID
      string adUnitId = "ca-app-pub-1728144482852364/6979746753";
#elif UNITY_IPHONE
      string adUnitId = "ca-app-pub-1728144482852364/3116044435";
#else
      string adUnitId = "unexpected_platform";
#endif


    if (!rewardBasedEventHandlersSet)
    {
      // Ad event fired when the rewarded video ad
      // has been received.
      rewardBasedVideo.OnAdLoaded += HandleRewardBasedVideoLoaded;
      // has failed to load.
      rewardBasedVideo.OnAdFailedToLoad += HandleRewardBasedVideoFailedToLoad;
      // is opened.
      rewardBasedVideo.OnAdOpening += HandleRewardBasedVideoOpened;
      // has started playing.
      //rewardBasedVideo.OnAdStarted += HandleRewardBasedVideoStarted;
      // has rewarded the user.
      rewardBasedVideo.OnAdRewarded += HandleRewardBasedVideoRewarded;
      // is closed.
      rewardBasedVideo.OnAdClosed += HandleRewardBasedVideoClosed;
      // is leaving the application.
      rewardBasedVideo.OnAdLeavingApplication += HandleRewardBasedVideoLeftApplication;

      rewardBasedEventHandlersSet = true;
    }

    AdRequest request = new AdRequest.Builder().Build();
    rewardBasedVideo.LoadAd(request, adUnitId);
    rewardBasedVideo.Show();
  }


  public void DestroyBanner()
  {
    bannerView.Destroy();
  }

  public void HandleRewardBasedVideoRewarded(object sender, Reward args)
  {
    Settings.Instance.continued = true;
    rewarded = true;
#if UNITY_ANDROID
    if (Settings.Instance.showAds)
    {
      SceneManager.LoadScene("Gameplay");
    }
    else
    {
      SceneManager.LoadScene("Gameplay_AdFree");
    }
# endif
    Firebase.Analytics.FirebaseAnalytics.LogEvent("rewarded_video_rewarded");
  }

  public void HandleRewardBasedVideoLoaded(object sender, EventArgs args)
  {
    Debug.Log("rewarded video loaded");
    //continueButtonLoaded = true;
    //if (continueButton)
    //{
    //  continueButton.SetActive(true);
    //  continueButton.GetComponent<Button>().interactable = true;
    //}
    Firebase.Analytics.FirebaseAnalytics.LogEvent("rewarded_video_loaded");
  }

  public void HandleRewardBasedVideoFailedToLoad(object sender, AdFailedToLoadEventArgs args)
  {
    Debug.Log("HandleFailedToReceiveAd event received with message: " + args.Message);
    Firebase.Analytics.FirebaseAnalytics.LogEvent("rewarded_video_failed", new Firebase.Analytics.Parameter("arguments", args.Message));
  }

  public void HandleRewardBasedVideoOpened(object sender, EventArgs args)
  {
    Audio.Instance.backgroundMusic.mute = true;
    Debug.Log("HandleAdOpened event received");
    Firebase.Analytics.FirebaseAnalytics.LogEvent("rewarded_video_opened");
  }

  public void HandleRewardBasedVideoClosed(object sender, EventArgs args)
  {
    Audio.Instance.backgroundMusic.mute = !Settings.Instance.music;
    Debug.Log("HandleAdOpened event received");
    Firebase.Analytics.FirebaseAnalytics.LogEvent("rewarded_video_closed");
#if UNITY_IOS
    if (Settings.Instance.showAds)
    {
      SceneManager.LoadScene("Gameplay");
    }
    else
    {
      SceneManager.LoadScene("Gameplay_AdFree");
    }
#endif
    rewarded = false;
    RequestRewardBasedVideo();
  }

  public void HandleRewardBasedVideoLeftApplication(object sender, EventArgs args)
  {
    Debug.Log("HandleAdOpened event received");
    Firebase.Analytics.FirebaseAnalytics.LogEvent("rewarded_video_left_app");
  }

  public void HandleOnAdLoaded(object sender, EventArgs args)
  {
    Debug.Log("HandleAdLoaded event received");
    //bannerBackgroundBlank.SetActive(false);
    Firebase.Analytics.FirebaseAnalytics.LogEvent("banner_loaded");

  }

  public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
  {
    Debug.Log("HandleFailedToReceiveAd event received with message: " + args.Message);
    Firebase.Analytics.FirebaseAnalytics.LogEvent("banner_failed", new Firebase.Analytics.Parameter("arguments", args.Message));
  }

  public void HandleOnAdOpened(object sender, EventArgs args)
  {
    Debug.Log("HandleAdOpened event received");
    Firebase.Analytics.FirebaseAnalytics.LogEvent("banner_opened");
  }

  public void HandleOnAdClosed(object sender, EventArgs args)
  {
    Debug.Log("HandleAdClosed event received");
    Firebase.Analytics.FirebaseAnalytics.LogEvent("banner_closed");

  }

  public void HandleOnAdLeftApplication(object sender, EventArgs args)
  {
    Debug.Log("HandleAdLeftApplication event received");
    Firebase.Analytics.FirebaseAnalytics.LogEvent("banner_left_app");
  }
}
