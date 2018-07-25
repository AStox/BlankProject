using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Popup : MonoBehaviour
{

  public void ExitClick()
  {
    StartCoroutine(ExitCoroutine());
  }

  IEnumerator ExitCoroutine()
  {
    Animator animator = GetComponent<Animator>();
    animator.Play("PopupExit");
    while (!animator.GetCurrentAnimatorStateInfo(0).IsName("PopupExit"))
    {
      yield return null;
    }
    while (animator.GetCurrentAnimatorStateInfo(0).IsName("PopupExit") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.99)
    {
      yield return null;
    }
    ((GameOver)GameObject.FindObjectOfType(typeof(GameOver))).EnableButtons();
    SceneManager.UnloadSceneAsync("Popup");
  }

  public void Rate()
  {
    PlayerPrefs.SetInt("Rated", 1);
    Firebase.Analytics.FirebaseAnalytics.LogEvent("rated");
    Firebase.Analytics.FirebaseAnalytics.SetUserProperty("rated", "true");
#if UNITY_ANDROID
    Application.OpenURL("market://details?id=com.HappySquare.PongPing");
#elif UNITY_IOS
    Application.OpenURL("itms-apps://itunes.apple.com/app/id1322583373");
#endif
    ExitClick();
  }
}
