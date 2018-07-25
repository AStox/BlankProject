using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
#if UNITY_ANDROID
using GooglePlayGames;
#elif UNITY_IOS
using UnityEngine.SocialPlatforms;
#endif

public class MainMenu : MonoBehaviour
{

  void Start()
  {
    //Animator anim = GetComponent<Animator>();
    Animator bordersAnim = GameObject.Find("Borders").GetComponent<Animator>();
    if (Settings.Instance.fromLoader)
    {
      bordersAnim.Play("MainMenuBordersFadeIn");
      Settings.Instance.fromLoader = false;
    }
    Audio.Instance.backgroundMusic.mute = !Settings.Instance.music;
    if (!Settings.Instance.showAds)
    {
      GameObject.Find("AdFreeButton").SetActive(false);
    }
  }

  void Update()
  {

  }

  public void PlayClick()
  {
    Animator anim = GetComponent<Animator>();
    StartCoroutine(WaitForAnimation(anim, "MainMenuPlay", Play));
    Audio.Instance.PlaySFX("BongoHit", 0.5f, 1.5f);
  }

  void Play()
  {
    Score.Instance.score = 0;
    Score.Instance.level = 0;
    Score.Instance.SpeedReset();
    Score.Instance.availableBlockers = null;
    Score.Instance.blockerIndex = -1;
    Score.Instance.colorSchemes = new ColorScheme[] { };
    Settings.Instance.continued = false;
    if (Settings.Instance.showAds)
    {
      SceneManager.LoadScene("Gameplay");
    }
    else
    {
      SceneManager.LoadScene("Gameplay_AdFree");
    }
  }

  public void SettingsClick()
  {
    Animator anim = GetComponent<Animator>();
    StartCoroutine(WaitForAnimation(anim, "MainMenuSettings", LoadSettings));
    Audio.Instance.PlaySFX("BongoHit", 0.5f, 1.5f * 1.05946f * 1.05946f);
  }

  void LoadSettings()
  {
    SceneManager.LoadScene("Settings");
  }

  private IEnumerator WaitForAnimation(Animator anim, string stateName, System.Action del)
  {
    anim.Play(stateName);
    while (!anim.GetCurrentAnimatorStateInfo(0).IsName(stateName))
    {
      yield return null;
    }
    while (anim.GetCurrentAnimatorStateInfo(0).IsName(stateName) && anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.99)
    {
      yield return null;
    }
    del();
  }

  public void ShowLeader()
  {
    Audio.Instance.PlaySFX("BongoHit", 0.5f, 1.5f * 1.05946f);
#if UNITY_ANDROID
    if (PlayGamesPlatform.Instance.IsAuthenticated())
    {
      PlayGamesPlatform.Instance.ShowLeaderboardUI("CgkIzdKUkfsMEAIQAQ");
      Firebase.Analytics.FirebaseAnalytics.LogEvent("show_leaderboard");
    }
    else
    {
      Social.localUser.Authenticate((bool success) =>
      {
        if (success)
        {
          PlayGamesPlatform.Instance.ShowLeaderboardUI("CgkIzdKUkfsMEAIQAQ");
          Firebase.Analytics.FirebaseAnalytics.LogEvent("gpgs_login_from_leaderboard_success");
          Firebase.Analytics.FirebaseAnalytics.SetUserProperty("signed_in", true.ToString());
        }
        else
        {
          Firebase.Analytics.FirebaseAnalytics.LogEvent("gpgs_login_from_leaderboard_failure");
          Firebase.Analytics.FirebaseAnalytics.SetUserProperty("signed_in", false.ToString());
        }
      });
    }
#elif UNITY_IOS
    Debug.Log("BUTTON CLICKED");
    if (Social.localUser.authenticated)
    {
      Debug.Log("ALREADY AUTHENTICATED. SHOWING LEADERBOARD");
      Social.ShowLeaderboardUI();
    }
    else
    {
      Debug.Log("NOT ALREADY AUTHENTICATED. AUTHENTICATING");
      Social.localUser.Authenticate(success =>
      {
      Debug.Log("AUTHENTICATING");
      if (success)
        {
          Debug.Log("SUCCESS");
          Firebase.Analytics.FirebaseAnalytics.SetUserProperty("signed_in", true.ToString());
          Social.ShowLeaderboardUI();
        }
        else
        {
          Debug.Log("FAILURE");
          Firebase.Analytics.FirebaseAnalytics.LogEvent("gamecenter_login_failure");
          Firebase.Analytics.FirebaseAnalytics.SetUserProperty("signed_in", false.ToString());
        }
      });
    }

#endif
  }
}
