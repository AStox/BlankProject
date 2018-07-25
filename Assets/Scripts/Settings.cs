using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : Singleton<Settings> {

  [HideInInspector]
	public bool fullScreenControls = true;
	[HideInInspector]
	public bool music = true;
	[HideInInspector]
	public bool sfx = true;
  [HideInInspector]
  public bool screenShake = true;
  public bool fromLoader;
  public bool continued;
  public bool showAds;

  void Awake()
  {
    continued = false;
    fullScreenControls = false;
    PlayerPrefs.SetInt("ControlScheme", 1);
    if (PlayerPrefs.GetInt("ShowAds") == 0)
    {
      showAds = true;
    }
    else
    {
      showAds = false;
    }
    if (PlayerPrefs.GetInt("Music") == 0)
    {
      music = true;
      Firebase.Analytics.FirebaseAnalytics.SetUserProperty("music", "on");
    }
    else
    {
      music = false;
      Firebase.Analytics.FirebaseAnalytics.SetUserProperty("music", "off");
    }
    if (PlayerPrefs.GetInt("SFX") == 0)
    {
      sfx = true;
      Firebase.Analytics.FirebaseAnalytics.SetUserProperty("sfx", "on");
    }
    else
    {
      sfx = false;
      Firebase.Analytics.FirebaseAnalytics.SetUserProperty("sfx", "off");
    }
    if (PlayerPrefs.GetInt("ScreenShake") == 0)
    {
      screenShake = true;
      Firebase.Analytics.FirebaseAnalytics.SetUserProperty("screen_shake", "on");
    }
    else
    {
      screenShake = false;
      Firebase.Analytics.FirebaseAnalytics.SetUserProperty("screen_shake", "off");
    }
  }
}
