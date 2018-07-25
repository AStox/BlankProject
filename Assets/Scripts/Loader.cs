using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_ANDROID
using GooglePlayGames;
#endif
using UnityEngine.SocialPlatforms;
#if UNITY_IOS
using UnityEngine.SocialPlatforms.GameCenter;
#endif

public class Loader : MonoBehaviour
{

  void Start()
  {
    PlayerPrefs.SetInt("SessionCount", (PlayerPrefs.GetInt("SessionCount") + 1));

    Screen.orientation = ScreenOrientation.Portrait;
    Settings.Instance.fromLoader = true;

    StartCoroutine(SocialConnect());
    SceneManager.LoadScene("MainMenu");
  }
	
  IEnumerator SocialConnect()
  {
#if UNITY_ANDROID
    PlayGamesPlatform.DebugLogEnabled = true;
    PlayGamesPlatform.Activate();
    Social.localUser.Authenticate((bool success) =>
    {
      if (success)
      {
        Debug.Log("SUCCESS");
        Firebase.Analytics.FirebaseAnalytics.SetUserProperty("signed_in", true.ToString());
      }
      else
      {
        Debug.Log("FAILURE");
        Firebase.Analytics.FirebaseAnalytics.LogEvent("gpgs_login_failure");
        Firebase.Analytics.FirebaseAnalytics.SetUserProperty("signed_in", false.ToString());
      }

    });
#elif UNITY_IOS
    Social.localUser.Authenticate(success =>
    {
      if (success)
      {
        Debug.Log("SUCCESS");
        Firebase.Analytics.FirebaseAnalytics.SetUserProperty("signed_in", true.ToString());
      }
      else
      {
        Debug.Log("FAILURE");
        Firebase.Analytics.FirebaseAnalytics.LogEvent("gamecenter_login_failure");
        Firebase.Analytics.FirebaseAnalytics.SetUserProperty("signed_in", false.ToString());
      }
    });
#endif 
    yield return null;
  }

  public void LoadGame() {
  }

  IEnumerator WaitToLoad() {
    yield return new WaitForSeconds(1.8f);
    Social.localUser.Authenticate((bool success) => {
      LoadGame();
    });
  }
}
