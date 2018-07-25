using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Blocker : MonoBehaviour {

	public int hitCount;
	public GameObject ripple;
	protected Text hitText;
	public ParticleSystem ps;
	int emissionRateInterval;
	int backgroundParticleEmissionRate;
	int backgroundParticleSpeedInterval;
	int backgroundParticleStartSpeed;
  bool onCooldown;
  float timer;
  public float cooldownTime = 0.25f;
  public int difficulty;
  public static float pitch;

	void Start () {
    pitch = 1.5f;
    onCooldown = false;
		hitText = GetComponentInChildren<Text>();
		hitText.text = hitCount.ToString();
    hitText.font = Resources.Load("Quicksand-Regular") as Font;
    GameObject.Find("GameManager").GetComponent<GameManager>().blockerCount++;
		GameObject.Find("GameManager").GetComponent<GameManager>().blockerExists = true;
		int minEmRate = 50;
		int maxEmRate = 50;
		int minStartSpeed = 1;
		int maxStartSpeed = 5;
		UpdateBackgroundParticles(minEmRate, minStartSpeed);
		emissionRateInterval = (maxEmRate - minEmRate)/(hitCount);
		backgroundParticleSpeedInterval = (maxStartSpeed - minStartSpeed)/(hitCount);
		
	}

    void Update() {
      if (timer >= cooldownTime) {
        onCooldown = false;
      }
      if (onCooldown) {
        timer += Time.deltaTime;
      }
    }

	private IEnumerator WaitForAnimation ( Animator anim ) {
		while ( anim.GetCurrentAnimatorStateInfo(0).IsName("BlockerEnter") ) {
			yield return null;
		}
	}


	void UpdateBackgroundParticles (int rate, int speed) {
		backgroundParticleEmissionRate = rate;
		backgroundParticleStartSpeed = speed;
		speed = rate;
		foreach (GameObject obj in GameObject.FindGameObjectsWithTag("BackgroundParticles")) {
      if (obj.transform.IsChildOf(transform)) {
				ps = obj.GetComponent<ParticleSystem>();
        ((Tint)GameObject.FindObjectOfType(typeof(Tint))).UpdateObjectColor(obj);
				var em = ps.emission;
				var main = ps.main;
				//em.enabled = true;
				//em.type = ParticleSystemEmissionType.Time;
				em.rate = rate;
				main.startSpeed = backgroundParticleStartSpeed;
      }
		}
	}

	void OnCollisionEnter2D (Collision2D coll) {
		if (coll.gameObject.tag == "Ball") {
      if (!onCooldown) {
				pitch *= 1.05946f;
				Audio.Instance.PlaySFX("BongoHit", 0.5f, pitch);
				timer = 0;
        onCooldown = true;
				UpdateBackgroundParticles(backgroundParticleEmissionRate + emissionRateInterval, backgroundParticleStartSpeed + backgroundParticleSpeedInterval);
				//Score.Instance.IncrementScore(coll.contacts[0].point);
				Camera.main.gameObject.GetComponent<MainCamera>().Hit();
				hitCount--;
				hitText.text = hitCount.ToString();
        if (Settings.Instance.screenShake)
        {
          Camera.main.GetComponent<CameraShake>().shakeDuration = .2f;
          Camera.main.GetComponent<CameraShake>().shakeAmount = .2f;
        }
				if (hitCount <= 0) {
          Kill(transform.position);
				}            
      }
		}
	}

	public void Kill (Vector3 hitLocation) {
    GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    gameManager.blockerCount--;
    if (gameManager.blockerCount <= 0) {
			gameManager.RemoveAvailableBlocker();
      Destroy(transform.root.gameObject);
			Score.Instance.NextLevel();
      float pitch = 0.1f * Mathf.RoundToInt(Random.Range(1, 4)) * 1.05946f;
      pitch = 0.1f * 4 * 1.05946f;
      Audio.Instance.PlaySFX("StrangeZap", 1f, pitch);
	    foreach (GameObject starPatternObj in GameObject.FindGameObjectsWithTag("StarPattern")) {
				Destroy(starPatternObj);
	    }
      int levelCount = (PlayerPrefs.GetInt("LevelCount") + 1);
      PlayerPrefs.SetInt("LevelCount", levelCount);
      Firebase.Analytics.FirebaseAnalytics.SetUserProperty("level_count", levelCount.ToString());
      if (PlayerPrefs.GetInt("LevelCount") == 1)
      {
        Firebase.Analytics.FirebaseAnalytics.LogEvent("first_level_completed");
      }
      else
      {
        Firebase.Analytics.FirebaseAnalytics.LogEvent("level_completed", new Firebase.Analytics.Parameter("level_count", levelCount));
      }
		}
		GameObject rippleObj = Instantiate(ripple, new Vector3(hitLocation.x, hitLocation.y, 3), Quaternion.identity);
    ((Tint)GameObject.FindObjectOfType(typeof(Tint))).UpdateObjectColor(rippleObj);
    rippleObj.name = "BlockerDeathRipples";
		Destroy(gameObject);
    gameManager.blockerExists = false;
	}

}
