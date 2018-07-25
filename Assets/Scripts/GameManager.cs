using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

  public GameObject paddle;
  public GameObject ballPrefab;
  public GameObject blockerSpawnParticles;
  public GameObject BallDeathParticles;
  public GameObject BlockerSpawnRipple;

  public GameObject[] powerups;
  public GameObject[] blockers0;
  public GameObject[] blockers1;
  public GameObject[] blockers2;
  public GameObject[] blockers3;

  BoxCollider2D screenObj;
  List<GameObject> availableBlockers;
  int blockerIndex;
  [HideInInspector]
  int ballCount;
  [HideInInspector]
  public int blockerCount;
  [HideInInspector]
  public bool powerUpExists;
  [HideInInspector]  
  public bool blockerExists;
  [HideInInspector]
  public float timer;

  float defaultBlockerSpawnInterval = 3.0f;
  bool gameover;
  int[] difficultyThreshold = new int[] { 0, 3, 6, 9 };
  bool wasContinued;
  public GameObject shadowBase;

  GameObject tutorial;
  public bool inTutorial;

  Tint tint;

  void Awake()
  {
    inTutorial = false;
    tutorial = GameObject.Find("Tutorial");
    tutorial.SetActive(false);
    tint = ((Tint)GameObject.FindObjectOfType(typeof(Tint)));
    if (!Settings.Instance.continued)
    {
      int roundCount = (PlayerPrefs.GetInt("RoundCount") + 1);
      PlayerPrefs.SetInt("RoundCount", roundCount);
      Firebase.Analytics.FirebaseAnalytics.SetUserProperty("round_count", roundCount.ToString());
      if (PlayerPrefs.GetInt("RoundCount") == 1)
      {
        Firebase.Analytics.FirebaseAnalytics.LogEvent("first_round_start");
      }
      else
      {
        Firebase.Analytics.FirebaseAnalytics.LogEvent("round_start", new Firebase.Analytics.Parameter("round_count", roundCount));
      }
    }
    else
    {
      Firebase.Analytics.FirebaseAnalytics.LogEvent("continued", new Firebase.Analytics.Parameter("blocker_name", Score.Instance.availableBlockers[Score.Instance.blockerIndex].name));
    }

    blockerCount = 0;
    screenObj = GameObject.Find("Screen").GetComponent<BoxCollider2D>();
    if (SceneManager.GetActiveScene().name == "Gameplay")
    {
      Ads.Instance.RequestBanner();
    }
    GameObject screen = GameObject.Find("Screen");
    float screenWidthToWorld = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.nearClipPlane)).x * 2;
    float screenHeightToWorld = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.nearClipPlane)).y * 2;
    float originalHeight = 9.91f;
    float originalWidth = 5.66f;

    if (SceneManager.GetActiveScene().name == "Gameplay")
    {
      GameObject adBackground = GameObject.Find("BannerBackgroundBlank");
#if UNITY_ANDROID
      float bannerHeight = 150f * ((float)Screen.height / 1920f);
#elif UNITY_IOS
      float bannerHeight = 150f * ((float)Screen.height / 1334f);
#endif
      float newHeight = screenHeightToWorld * ((Screen.height - bannerHeight) / Screen.height);
      screen.transform.localScale = new Vector3(screenWidthToWorld/originalWidth, newHeight/originalHeight, 1);
      screen.transform.localPosition = new Vector3(0, (newHeight - originalHeight) / 2, screen.transform.localPosition.z);
      adBackground.transform.localPosition = new Vector3(0, (newHeight/2) + (newHeight - originalHeight) / 2, adBackground.transform.localPosition.z);
    }
    else
    {
      screen.transform.localScale = new Vector3(screenWidthToWorld/originalWidth, screenHeightToWorld/originalHeight, 1);
      screen.transform.localPosition = new Vector3(0, 0, screen.transform.localPosition.z);
    }
    CreatePaddles();
  }

  void Start()
  {
    if (SceneManager.GetActiveScene().name == "Gameplay_AdFree")
    {
      Ads.Instance.bannerBackgroundBlank = GameObject.Find("BannerBackgroundBlank");
      if (Ads.Instance.bannerBackgroundBlank != null)
      {
        Ads.Instance.bannerBackgroundBlank.SetActive(false);
      }
    }

    wasContinued = false;

    if (Score.Instance.availableBlockers == null)
    {
      availableBlockers = new List<GameObject>();
      for (int i = 0; i < blockers0.Length; i++)
      {
        availableBlockers.Add(blockers0[i]);
      }
    }
    else
    {
      wasContinued = true;
      availableBlockers = Score.Instance.availableBlockers;
    }

    gameover = false;

    if (GameObject.Find("DebugBallCollider") != null) 
    {
      defaultBlockerSpawnInterval = 0f;
    }

    if (PlayerPrefs.GetInt("ShowTutorial") == 0)
    {
      StartCoroutine(Tutorial());
    } 
    else 
    {
      ballCount = 1;
      if (GameObject.Find("DebugBallCollider") != null)
      {
        GameObject ballObj = Instantiate(ballPrefab, screenObj.bounds.center + new Vector3(0, 3, 0), Quaternion.identity);
        tint.UpdateObjectColor(ballObj);
      }
      else
      {
        GameObject ballObj = Instantiate(ballPrefab, screenObj.bounds.center, Quaternion.identity);
        tint.UpdateObjectColor(ballObj);
      }
      StartCoroutine(SpawnBlockerCoroutine(5f));
    }

    if (!Settings.Instance.continued)
    {
      StartCoroutine(WaitToDo(Ads.Instance.RequestRewardBasedVideo, 1));
    }
  }

  void Update ()
  {
    if (ballCount <= 0 && !gameover && !inTutorial) {
      Firebase.Analytics.FirebaseAnalytics.LogEvent("game_over", new Firebase.Analytics.Parameter("blocker_name", availableBlockers[blockerIndex].name));
      StartCoroutine(GameOver());
      gameover = true;
    }
  }

  void CreatePaddles()
  {
    GameObject paddleTop = Instantiate(paddle, new Vector3(screenObj.bounds.center.x, screenObj.bounds.center.y + screenObj.bounds.extents.y, 0), Quaternion.Euler(0, 0, 180)); //top
    paddleTop.GetComponentInChildren<Paddle>().horizontal = true;
    paddleTop.name = "PaddleTop";
    GameObject paddleBottom = Instantiate(paddle, new Vector3(screenObj.bounds.center.x, screenObj.bounds.center.y - screenObj.bounds.extents.y, 0), Quaternion.identity); //bottom
    paddleBottom.GetComponentInChildren<Paddle>().horizontal = true;
    paddleBottom.name = "PaddleBottom";
    GameObject paddleRight = Instantiate(paddle, new Vector3(screenObj.bounds.center.x + screenObj.bounds.extents.x, screenObj.bounds.center.y, 0), Quaternion.Euler(0, 0, 90)); //right
    paddleRight.GetComponentInChildren<Paddle>().horizontal = false;
    paddleRight.name = "PaddleRight";
    GameObject paddleLeft = Instantiate(paddle, new Vector3(screenObj.bounds.center.x - screenObj.bounds.extents.x, screenObj.bounds.center.y, 0), Quaternion.Euler(0, 0, -90)); //left
    paddleLeft.GetComponentInChildren<Paddle>().horizontal = false;
    paddleLeft.name = "PaddleLeft";
  }

  IEnumerator GameOver()
  {
    PlayerPrefs.SetInt("Score", Score.Instance.score);
    Score.Instance.colorSchemes = GameObject.Find("Tint").GetComponent<Tint>().levels;
    yield return new WaitForSeconds(1f);
    SceneManager.LoadSceneAsync("GameOver", LoadSceneMode.Single);
  }

  public GameObject CreateBall (Vector3 location, Quaternion rotation, Vector2 velocity)
  {
    GameObject ball = Instantiate(ballPrefab, location, rotation);
    tint.UpdateObjectColor(ball);
    ball.GetComponent<Rigidbody2D>().velocity = velocity;
    ballCount++;
    return ball;
  }

  public void DestroyBall (GameObject ball)
  {
    GameObject psObj = Instantiate(BallDeathParticles, new Vector3(ball.transform.position.x, ball.transform.position.y, 3), Quaternion.identity);
    tint.UpdateObjectColor(psObj);
    ballCount--;
  }

  public void SpawnBlocker () {
    StartCoroutine(SpawnBlockerCoroutine(defaultBlockerSpawnInterval));
  }

  IEnumerator SpawnBlockerCoroutine(float blockerSpawnInterval) {
    UpdateAvailableBlockers();
    ChooseAvailableBlocker();
    yield return new WaitForSeconds(blockerSpawnInterval);
    if (!blockerExists) {
      GameObject shadow = CreateShadow();
      GameObject psObj = Instantiate(blockerSpawnParticles, new Vector3(screenObj.bounds.center.x, screenObj.bounds.center.y, 10), Quaternion.identity);
      Audio.Instance.PlaySFX("ReverseKick", 0.5f, 0.21f);
      ParticleSystem ps = psObj.GetComponent<ParticleSystem>();
      psObj.name = "BlockerSpawnParticles";
      tint.UpdateObjectColor(psObj);
      while (ps.IsAlive()) {
        yield return null;
      }
      Destroy(psObj);

      InstantiateBlocker();
      yield return new WaitForSeconds(0.5f);
      Destroy(shadow);
    }
  }

  GameObject CreateShadow()
  {
    GameObject shadow = Instantiate(shadowBase, new Vector3(screenObj.bounds.center.x, screenObj.bounds.center.y, 0), Quaternion.identity);

    SpriteRenderer[] sprites = availableBlockers[blockerIndex].GetComponentsInChildren<SpriteRenderer>();
    for (int i = 0; i < sprites.Length; i++)
    {
      GameObject subShadow = new GameObject();
      subShadow.transform.parent = shadow.transform;
      subShadow.transform.localRotation = sprites[i].transform.localRotation;
      subShadow.transform.tag = "Shadow";
      if (sprites[i].transform.parent != null)
      {
        if (sprites[i].transform.parent.parent != null)
        {
          Vector3 newPosition = new Vector3(sprites[i].transform.parent.parent.localPosition.x + sprites[i].transform.parent.localPosition.x + sprites[i].transform.localPosition.x, sprites[i].transform.parent.parent.localPosition.y + sprites[i].transform.parent.localPosition.y + sprites[i].transform.localPosition.y, 0);
          Vector3 newScale = new Vector3(sprites[i].transform.parent.parent.localScale.x * sprites[i].transform.parent.localScale.x * sprites[i].transform.localScale.x, sprites[i].transform.parent.parent.localScale.y * sprites[i].transform.parent.localScale.y * sprites[i].transform.localScale.y, 1);
          subShadow.transform.localPosition = newPosition;
          subShadow.transform.localScale = newScale;
        }
        else
        {
          Vector3 newPosition = new Vector3(sprites[i].transform.parent.localPosition.x + sprites[i].transform.localPosition.x, sprites[i].transform.parent.localPosition.y + sprites[i].transform.localPosition.y, 0);
          Vector3 newScale = new Vector3(sprites[i].transform.parent.localScale.x * sprites[i].transform.localScale.x, sprites[i].transform.parent.localScale.y * sprites[i].transform.localScale.y, 1);
          subShadow.transform.localPosition = newPosition;
          subShadow.transform.localScale = newScale;
        }
      }
      else
      {
        subShadow.transform.localScale = sprites[i].transform.localScale;
        subShadow.transform.localPosition = sprites[i].transform.localPosition;
      }
      subShadow.name = "ShadowHolder";

      GameObject background = new GameObject();
      background.transform.parent = subShadow.transform;
      background.transform.localPosition = new Vector3(0, 0, 2);
      background.transform.localRotation = Quaternion.identity;
      background.transform.localScale = new Vector3(1,1,1);
      background.name = "background";
      SpriteRenderer bgSpriteRenderer = background.AddComponent<SpriteRenderer>();
      bgSpriteRenderer.sprite = sprites[i].sprite;

      GameObject foreground = new GameObject();
      foreground.transform.parent = subShadow.transform;
      foreground.transform.localPosition = Vector3.zero;
      foreground.transform.localRotation = Quaternion.identity;
      foreground.transform.localScale = new Vector3(1, 1, 1);;
      foreground.name = "foreground";
      SpriteRenderer fgSpriteRenderer = foreground.AddComponent<SpriteRenderer>();
      fgSpriteRenderer.sprite = sprites[i].sprite;
      tint.UpdateObjectColor(subShadow);
    }
    shadow.GetComponent<Animator>().Play("Enter");
    return shadow;
  }

  void ChooseAvailableBlocker()
  {
    if (wasContinued)
    {
      blockerIndex = Score.Instance.blockerIndex;
      wasContinued = false;
    }
    else
    {
      if (availableBlockers.Count == 0)
      {
        ResetAvailableBlockers();
      }
      blockerIndex = Mathf.RoundToInt(Random.Range(0, availableBlockers.Count - 1));
    }
    Score.Instance.availableBlockers = availableBlockers;
    Score.Instance.blockerIndex = blockerIndex;
  }

  void InstantiateBlocker()
  {
    Firebase.Analytics.FirebaseAnalytics.LogEvent("new_level", new Firebase.Analytics.Parameter("blocker_name", availableBlockers[blockerIndex].name));
    GameObject blockerObj = Instantiate(availableBlockers[blockerIndex], new Vector3(screenObj.bounds.center.x, screenObj.bounds.center.y, -1), Quaternion.identity);
    GameObject spawnRippleObj = Instantiate(BlockerSpawnRipple, new Vector3(screenObj.bounds.center.x, screenObj.bounds.center.y, 3), Quaternion.identity);
    tint.UpdateObjectColor(blockerObj);
    tint.UpdateObjectColor(spawnRippleObj);
  }

  void UpdateAvailableBlockers ()
  {
    if (Score.Instance.level == difficultyThreshold[1])
    {
      for (int i = 0; i < blockers1.Length; i++)
      {
        availableBlockers.Add(blockers1[i]);
      }
    }
    else if (Score.Instance.level == difficultyThreshold[2])
    {
      for (int i = 0; i < blockers2.Length; i++)
      {
        availableBlockers.Add(blockers2[i]);
      }
    }
    else if (Score.Instance.level == difficultyThreshold[3])
    {
      for (int i = 0; i < blockers3.Length; i++)
      {
        availableBlockers.Add(blockers3[i]);
      }

    }
  }

  public void RemoveAvailableBlocker()
  {
    availableBlockers.Remove(availableBlockers[blockerIndex]);
  }

  void ResetAvailableBlockers()
  {
    for (int i = 0; i < blockers0.Length; i++)
    {
      availableBlockers.Add(blockers0[i]);
    }
    for (int i = 0; i < blockers1.Length; i++)
    {
      availableBlockers.Add(blockers1[i]);
    }
    for (int i = 0; i < blockers2.Length; i++)
    {
      availableBlockers.Add(blockers2[i]);
    }
    for (int i = 0; i < blockers3.Length; i++)
    {
      availableBlockers.Add(blockers3[i]);
    }
  }

  
  IEnumerator Tutorial () {
    inTutorial = true;
    float waitTime = 0.1f;
    tutorial.SetActive(true);
    GameObject tut1 = GameObject.Find("Tut1");
    GameObject tut2 = GameObject.Find("Tut2");
    GameObject tut3 = GameObject.Find("Tut3");
    GameObject tutText1 = GameObject.Find("TutText1");
    GameObject tutText2 = GameObject.Find("TutText2");
    GameObject tutText3 = GameObject.Find("TutText3");
    GameObject paddle = GameObject.Find("PaddleBottom");
    tut2.SetActive(false);
    tutText2.SetActive(false);
    tut3.SetActive(false);
    tutText3.SetActive(false);
    while (!Input.GetMouseButtonUp(0))
    {
      yield return null;
    }
    yield return new WaitForSeconds(waitTime);
    tut1.SetActive(false);
    tutText1.SetActive(false);
    tut2.SetActive(true);
    tutText2.SetActive(true);
    while (!Input.GetMouseButtonUp(0))
    {
      yield return null;
    }
    yield return new WaitForSeconds(waitTime);
    tut2.SetActive(false);
    tutText2.SetActive(false);
    tut3.SetActive(true);
    tutText3.SetActive(true);
    while (!Input.GetMouseButtonUp(0))
    {
      yield return null;
    }
    yield return new WaitForSeconds(waitTime);
    tut3.SetActive(false);
    tutText3.SetActive(false);

    ballCount = 1;
    if (GameObject.Find("DebugBallCollider") != null)
    {
      GameObject ballObj = Instantiate(ballPrefab, screenObj.bounds.center + new Vector3(0, 3, 0), Quaternion.identity);
      tint.UpdateObjectColor(ballObj);
    }
    else
    {
      GameObject ballObj = Instantiate(ballPrefab, screenObj.bounds.center, Quaternion.identity);
      tint.UpdateObjectColor(ballObj);
    }
    inTutorial = false;
    StartCoroutine(SpawnBlockerCoroutine(5f));
    yield return new WaitForSeconds(5);
    PlayerPrefs.SetInt("ShowTutorial", 1);
  }

  IEnumerator WaitToDo(System.Action del, float time)
  {
    yield return new WaitForSeconds(time);
    del();
  }
}

