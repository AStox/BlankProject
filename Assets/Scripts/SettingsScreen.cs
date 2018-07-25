using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsScreen : MonoBehaviour
{
  bool controlSchemeClickable = true;
  bool musicClickable = true;
  bool sfxClickable = true;
  bool vibrationClickable = true;
  Animator anim;

  void Start ()
  {
    anim = GetComponent<Animator>();
    anim.SetLayerWeight(1, 1f);
    anim.SetLayerWeight(2, 1f);
    anim.SetLayerWeight(3, 1f);
    anim.SetLayerWeight(4, 1f);

    if (!Settings.Instance.music)
    {
      anim.Play("MusicOff", 1);
    }
    if (!Settings.Instance.sfx)
    {
      anim.Play("SFXOff", 2);
    }
    if (!Settings.Instance.screenShake)
    {
      anim.Play("VibrationOff", 3);
    }
  }

  void Update()
  {
    if (Input.GetKey("escape"))
    {
      BackClick();
    }    
  }

  public void ToggleMusic()
  {
    if (musicClickable)
    {
      Audio.Instance.PlaySFX("BongoHit", 0.5f, 1.5f * 1.05946f);
      if (!Settings.Instance.music)
      {
        musicClickable = false;
        Settings.Instance.music = true;
        anim.Play("MusicOn", 1);
        StartCoroutine(WaitToDo(0.5f, ToggleMusicClickable));
        PlayerPrefs.SetInt("Music", 0);
        Firebase.Analytics.FirebaseAnalytics.SetUserProperty("music", "on");

      }
      else
      {
        musicClickable = false;
        Settings.Instance.music = false;
        anim.Play("MusicOff", 1);
        StartCoroutine(WaitToDo(0.5f, ToggleMusicClickable));
        PlayerPrefs.SetInt("Music", 1);
        Firebase.Analytics.FirebaseAnalytics.SetUserProperty("music", "off");
      }
    }
    Audio.Instance.backgroundMusic.mute = !Settings.Instance.music;
  }

  public void ToggleSFX()
  {
    if (sfxClickable)
    {
      if (!Settings.Instance.sfx)
      {
        sfxClickable = false;
        Settings.Instance.sfx = true;
        anim.Play("SFXOn", 2);
        StartCoroutine(WaitToDo(0.5f, ToggleSFXClickable));
        PlayerPrefs.SetInt("SFX", 0);
        Firebase.Analytics.FirebaseAnalytics.SetUserProperty("sfx", "on");
      }
      else
      {
        sfxClickable = false;
        Settings.Instance.sfx = false;
        anim.Play("SFXOff", 2);
        StartCoroutine(WaitToDo(0.5f, ToggleSFXClickable));
        PlayerPrefs.SetInt("SFX", 1);
        Firebase.Analytics.FirebaseAnalytics.SetUserProperty("sfx", "off");
      }
      Audio.Instance.PlaySFX("BongoHit", 0.5f, 1.5f * 1.05946f * 1.05946f);
    }
  }

  public void ToggleVibration()
  {
    if (vibrationClickable)
    {
      Audio.Instance.PlaySFX("BongoHit", 0.5f, 1.5f * 1.05946f * 1.05946f * 1.05946f);
      if (!Settings.Instance.screenShake)
      {
        vibrationClickable = false;
        Settings.Instance.screenShake = true;
        anim.Play("VibrationOn", 3);
        StartCoroutine(WaitToDo(0.5f, ToggleVibrationClickable));
        PlayerPrefs.SetInt("ScreenShake", 0);
      }
      else
      {
        vibrationClickable = false;
        Settings.Instance.screenShake = false;
        anim.Play("VibrationOff", 3);
        StartCoroutine(WaitToDo(0.5f, ToggleVibrationClickable));
        PlayerPrefs.SetInt("ScreenShake", 1);
      }
    }
  }

  void ToggleControlSchemeClickable()
  {
    controlSchemeClickable = !controlSchemeClickable;
  }

  void ToggleSFXClickable()
  {
    sfxClickable = !sfxClickable;
  }

  void ToggleMusicClickable()
  {
    musicClickable = !musicClickable;
  }

  void ToggleVibrationClickable()
  {
    vibrationClickable = !vibrationClickable;
  }

  public void BackClick()
  {
    Audio.Instance.PlaySFX("BongoHit", 0.5f, 1.5f);
    Animator anim = GetComponent<Animator>();
    anim.SetLayerWeight(1, 0f);
    anim.SetLayerWeight(2, 0f);
    anim.SetLayerWeight(3, 0f);
    anim.SetLayerWeight(4, 0f);
    StartCoroutine(WaitForAnimation(anim, "SettingsBack", 0, Back));
  }

  void Back()
  {
    SceneManager.LoadScene("MainMenu");
  }

  private IEnumerator WaitToDo(float time, System.Action del)
  {
    yield return new WaitForSeconds(time);
    del();
  }

  private IEnumerator WaitForAnimation(Animator anim, string stateName, int layer, System.Action del)
  {
    anim.Play(stateName, layer);
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
}
