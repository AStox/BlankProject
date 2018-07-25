using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.IO;
using System;

/*
 * https://github.com/ChrisMaire/unity-native-sharing
 */

public class NativeShare : MonoBehaviour
{
  
  public Texture2D icon;
#if UNITY_ANDROID
  string url = "https://play.google.com/store/apps/details?id=com.HappySquare.PongPing";
#elif UNITY_IOS
  string url = "https://itunes.apple.com/us/app/palmpong/id1322583373";
#endif

  IEnumerator delayedShare()
  {
    while (!File.Exists(Application.persistentDataPath + "/icon.png"))
    {
      yield return new WaitForSeconds(.05f);
    }

    Share("PalmPong", Application.persistentDataPath + "/icon.png", url);
  }

  void SaveTextureToFile()
  {
    Byte[] bytes = icon.EncodeToPNG();
    File.WriteAllBytes(Application.persistentDataPath + "/icon.png", bytes);
  }

  public void ShareClick()
  {
    PlayerPrefs.SetInt("Shared", 1);
    Firebase.Analytics.FirebaseAnalytics.LogEvent("shared");
    Firebase.Analytics.FirebaseAnalytics.SetUserProperty("shared", "true");
    SaveTextureToFile();
    StartCoroutine(delayedShare());
  }

  public void Share(string shareText, string imagePath, string url, string subject = "Tell your friends about PalmPong!")
  {
#if UNITY_ANDROID
    AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
    AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");

    intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
    AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
    // AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse", "file://" + imagePath);
    intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), "Brick breaking, ball bouncing mayhem; It's PalmPong! \n" + url);
    intentObject.Call<AndroidJavaObject>("setType", "text/plain");

    AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
    AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");

    AndroidJavaObject jChooser = intentClass.CallStatic<AndroidJavaObject>("createChooser", intentObject, subject);
    currentActivity.Call("startActivity", jChooser);
#elif UNITY_IOS
    CallSocialShareAdvanced("Brick breaking, ball bouncing mayhem; It's PalmPong! \n" + url, null, null, null);
#else
    Debug.Log("No sharing set up for this platform.");
#endif
  }

#if UNITY_IOS
  public struct ConfigStruct
  {
    public string title;
    public string message;
  }

  [DllImport ("__Internal")] private static extern void showAlertMessage(ref ConfigStruct conf);

  public struct SocialSharingStruct
  {
    public string text;
    public string url;
    public string image;
    public string subject;
  }

  [DllImport ("__Internal")] private static extern void showSocialSharing(ref SocialSharingStruct conf);

  public static void CallSocialShare(string title, string message)
  {
    ConfigStruct conf = new ConfigStruct();
    conf.title  = title;
    conf.message = message;
    showAlertMessage(ref conf);
    // showSocialSharing(ref conf);

  }


  public static void CallSocialShareAdvanced(string defaultTxt, string subject, string url, string img)
  {
    SocialSharingStruct conf = new SocialSharingStruct();
    conf.text = defaultTxt;
    conf.url = url;
    conf.image = img;
    conf.subject = subject;

    showSocialSharing(ref conf);
  }
#endif
}
