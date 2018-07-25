using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : Singleton<Score> {

	[HideInInspector]
	public int score;
	[HideInInspector]
	public int level;
	public GameObject plusOneParticles;
	Blocker[] blockers;
  public float speedLimitMax = 6f;
	public int speedGainRate = 3;
  float speedFloorDiff = 0.1f;
  public float speedLimit;
  public float speedFloor;
  [HideInInspector]
  public List<GameObject> availableBlockers;
  [HideInInspector]
  public int blockerIndex;
  [HideInInspector]
  public ColorScheme[] colorSchemes;

	void Start () {
    SpeedReset();
    Ball[] balls = (Ball[])GameObject.FindObjectsOfType(typeof(Ball));
    for (int i = 0; i < balls.Length; i++)
    {
      balls[i].speedLimit = speedLimit;
      balls[i].speedFloor = speedFloor;
    }
		level = 0;
		score = 0;
		plusOneParticles = Resources.Load("PlusOneParticles") as GameObject;
	 	blockers = (Blocker[])GameObject.FindObjectsOfType(typeof(Blocker));
	}

	void Update () {
		if (Input.GetKeyDown("up")) {
      DestroyBlockers();
    }
	}

  public void SpeedReset () {
    speedLimit = 5f;
    speedFloor = speedLimit - speedFloorDiff;
  }

	//for Debugging
	void DestroyBlockers () {
    blockers = (Blocker[])GameObject.FindObjectsOfType(typeof(Blocker));
		for (int i = 0; i < blockers.Length; i++) {
			blockers[i].Kill(Vector3.zero);
		}
	}

	public void IncrementScore(Vector3 location) {
		score++;
		GameObject plusOneObj = Instantiate(plusOneParticles, location, Quaternion.identity);
    ((Tint)GameObject.FindObjectOfType(typeof(Tint))).UpdateObjectColor(plusOneObj);
		GameObject.Find("ScoreCanvas").GetComponentsInChildren<Text>()[0].text = score.ToString();
    GameObject.Find("ScoreCanvas").GetComponentsInChildren<Text>()[1].text = score.ToString();
		Camera.main.GetComponent<CameraShake>().shakeDuration = .1f;
		Camera.main.GetComponent<CameraShake>().shakeAmount = .05f;
	}

	public void NextLevel () {
    Tint tint = ((Tint)GameObject.FindObjectOfType(typeof(Tint)));
    Score.Instance.IncrementScore(new Vector3(-1.9f, 3.7f, 0));
		Ball[] balls = (Ball[])GameObject.FindObjectsOfType(typeof(Ball));
		if (speedLimit < speedLimitMax) {
      speedLimit += (speedLimitMax - speedLimit)/speedGainRate;
      speedFloor = speedLimit - speedFloorDiff;
		}
    for (int i = 0; i < balls.Length; i++) {
      balls[i].speedLimit = speedLimit;
      balls[i].speedFloor = speedFloor;
      balls[i].GetComponentInChildren<TrailRenderer>().Clear();
    }
    if (tint.levels[Score.Instance.level % tint.levels.Length].backgroundParticleObj != null) {
      Destroy(tint.backgroundParticles);
		}
    level++;
    if (tint.levels[Score.Instance.level % tint.levels.Length].backgroundParticleObj != null) {
      tint.backgroundParticles = Instantiate(tint.levels[Score.Instance.level % tint.levels.Length].backgroundParticleObj, Vector3.zero, Quaternion.identity);
      tint.UpdateObjectColor(tint.backgroundParticles);
    }
		GameObject.Find("GameManager").GetComponent<GameManager>().SpawnBlocker();
		tint.UpdateColors();
	}

  IEnumerator SlowBall (Ball ball) {
    Rigidbody2D rb = ball.GetComponent<Rigidbody2D>();
  	ball.slow = true;
    while (rb.velocity.magnitude > ball.slowSpeed) {
      yield return null;   
    }
    yield return new WaitForSeconds(0.75f);
    ball.slow = false;
  }
}
