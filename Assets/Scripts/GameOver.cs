using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using GooglePlayGames;
using UnityEngine.SocialPlatforms;


public class GameOver : MonoBehaviour
{
  Animator anim;
  GameObject continueButton;

  void Start()
  {
    DisableButtons();
    if (Settings.Instance.showAds)
    {
      Ads.Instance.bannerView.Destroy();
    }
    continueButton = GameObject.Find("ContinueButton");
    anim = GetComponent<Animator>();
    UpdateHighscore();
    PostScore(PlayerPrefs.GetInt("Score"));
    GameObject.Find("Score").GetComponent<Text>().text = PlayerPrefs.GetInt("Score").ToString();
    GameObject.Find("ScoreShadow").GetComponent<Text>().text = PlayerPrefs.GetInt("Score").ToString();
    GameObject.Find("Highscore").GetComponent<Text>().text = PlayerPrefs.GetInt("Highscore").ToString();

    Ads.Instance.continueButton = continueButton;
    continueButton.SetActive(false);

    if (Ads.Instance.rewardBasedVideo.IsLoaded() && !Settings.Instance.continued)
    {
      continueButton.SetActive(true);
      StartCoroutine(ContinueCountdown());
    }

    if (PlayerPrefs.GetInt("RoundCount") >= 5 && PlayerPrefs.GetInt("SessionCount") >= 2 && PlayerPrefs.GetInt("RateShown") == 0)
    {
      SceneManager.LoadSceneAsync("Popup", LoadSceneMode.Additive);
      PlayerPrefs.SetInt("RateShown", 1);
    }
    else
    {
      StartCoroutine(WaitToDo(EnableButtons, 0.5f));
    }
  }

  IEnumerator ContinueCountdown()
  {
    int time = 325;
    for (int i = 1; i < time; i ++)
    {
      GameObject.Find("Ring").GetComponent<Image>().fillAmount = (float)i/(float)(time - 25);
      yield return null;
    }
    continueButton.SetActive(false);
  }

  void DisableButtons()
  {
    Button[] buttons = GameObject.FindObjectsOfType(typeof(Button)) as Button[];
    for (int i = 0; i < buttons.Length; i++)
    {
      if (buttons[i].gameObject.scene.name == "GameOver")
      {
        Debug.Log(buttons[i].gameObject.name + " Disabled!");
        buttons[i].interactable = false;
      }
    }
  }

  public void EnableButtons()
  {
    Button[] buttons = (Button[])GameObject.FindObjectsOfType(typeof(Button));
    for (int i = 0; i < buttons.Length; i++)
    {
      if (buttons[i].gameObject.scene.name == "GameOver")
      {
        buttons[i].interactable = true;
      }
    }
  }

  public void Continue()
  {
    Audio.Instance.PlaySFX("BongoHit", 0.5f, 1.5f);
    Settings.Instance.continued = true;
    Ads.Instance.rewardBasedVideo.Show();
  }

  void UpdateHighscore()
  {
    if (PlayerPrefs.GetInt("Score") > PlayerPrefs.GetInt("Highscore"))
    {
      PlayerPrefs.SetInt("Highscore", PlayerPrefs.GetInt("Score"));
      PostScore(PlayerPrefs.GetInt("Score"));
    }
  }

  void PostScore(int score)
  {
#if UNITY_ANDROID
    if (PlayGamesPlatform.Instance.IsAuthenticated())
    {
      Social.ReportScore(score, "CgkIzdKUkfsMEAIQAQ", (bool success) =>
      {
        if (success)
        {
          Debug.Log("SUCCESS!");
          Firebase.Analytics.FirebaseAnalytics.LogEvent("post_highscore_success", new Firebase.Analytics.Parameter("highscore", score));
        }
        else
        {
          Debug.Log("FAILURE!");
          Firebase.Analytics.FirebaseAnalytics.LogEvent("post_highscore_failure", new Firebase.Analytics.Parameter("highscore", score));
        }
      });
    }
#elif UNITY_IOS
    Social.ReportScore (score, "Leaderboard", success => 
    {
      if (success)
      {
        Debug.Log("SUCCESS!");
        Firebase.Analytics.FirebaseAnalytics.LogEvent("post_highscore_success", new Firebase.Analytics.Parameter("highscore", score));
      }
      else
      {
        Debug.Log("FAILURE!");
        Firebase.Analytics.FirebaseAnalytics.LogEvent("post_highscore_failure", new Firebase.Analytics.Parameter("highscore", score));
      }
    });
#endif
  }

  public void AgainClick()
  {
    Audio.Instance.PlaySFX("BongoHit", 0.5f, 1.5f * 1.05946f);
  	StartCoroutine(WaitForAnimation(anim, Again, "Again"));
  }

  void Again()
  {
  	Score.Instance.score = 0;
    Score.Instance.level = 0;
    Score.Instance.SpeedReset();
    Score.Instance.availableBlockers = null;
    Score.Instance.blockerIndex = -1;
    Score.Instance.colorSchemes = new ColorScheme[]{};
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

  public void MainMenuClick()
  {
    Audio.Instance.PlaySFX("BongoHit", 0.5f, 1.5f * 1.05946f * 1.05946f);
  	StartCoroutine(WaitForAnimation(anim, MainMenu, "MainMenu"));
  }

  void MainMenu()
  {
  	Score.Instance.score = 0;
  	Score.Instance.level = 0;
    Score.Instance.SpeedReset();
    Score.Instance.availableBlockers = null;
    Score.Instance.blockerIndex = -1;
  	SceneManager.LoadScene("MainMenu");
  }


  private IEnumerator WaitForAnimation(Animator animator, System.Action del, string animatorState)
    {
  	animator.Play(animatorState);
  	while (!animator.GetCurrentAnimatorStateInfo(0).IsName(animatorState))
  	{
  		yield return null;
  	}
  	while (animator.GetCurrentAnimatorStateInfo(0).IsName(animatorState) && animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.99)
  	{
  		yield return null;
  	}
  	del();

  }

  IEnumerator WaitToDo(System.Action del, float time)
  {
    yield return new WaitForSeconds(time);
    del();
  }
}
