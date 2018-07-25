using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour {

	Rigidbody2D rb;
  [HideInInspector]
	public float speedLimit;
  [HideInInspector]
	public float speedFloor;
	public bool randomStartForce;
  int pitch;
  bool pitchForwards;
  [HideInInspector]
  public bool slow;
  [HideInInspector]
  public float slowSpeed = 2f;
  public GameObject ripple;



	void Start () {
    slow = false;
    pitch = 0;
    pitchForwards = true;
		rb = GetComponent<Rigidbody2D>();
		if (randomStartForce) {
			StartCoroutine(WaitToDo(AddStartForce, 1f));
		}
    speedLimit = Score.Instance.speedLimit;
    speedFloor = Score.Instance.speedFloor;
	}

	IEnumerator WaitToDo(System.Action del, float time) {
		yield return new WaitForSeconds(time);
		del();
	}

	void AddStartForce() {
    Vector3 startForce = new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), 0).normalized * 0.01f;
		rb.AddForce(startForce);
    randomStartForce = false;
	}

	void Update () {
    if (!slow)
    {
      RegularMotion();
    }
    else
    {
      SlowBall();
    }
	}

  void SlowBall () {
    if (rb.velocity.magnitude > slowSpeed)
    {
      Vector2 force = rb.velocity * 0.1f;
      rb.AddForce(-force);
    }
  }

  void RegularMotion() {
    if (rb.velocity.magnitude == 0 && !randomStartForce)
    {
      AddStartForce();
    }
    else if (rb.velocity.magnitude > speedLimit)
    {
      //TOO FAST
      Vector3 oppositeForce = -rb.velocity.normalized * 0.05f;
      rb.AddForce(oppositeForce);
    }
    else if (rb.velocity.magnitude < speedFloor)
    {
      //TOO SLOW
      Vector3 moreForce = rb.velocity.normalized * 0.05f;
      rb.AddForce(moreForce);
    }
  }

  void OnTriggerExit2D(Collider2D coll)
  {
    if (coll.gameObject.tag == "KillBox")
    {
      GameObject.Find("GameManager").GetComponent<GameManager>().DestroyBall(gameObject);
    }
  }
	void OnCollisionEnter2D (Collision2D coll) {
    GameObject rippleObj = Instantiate(ripple, new Vector3(transform.localPosition.x, transform.localPosition.y, 0f), Quaternion.identity);
    ((Tint)GameObject.FindObjectOfType(typeof(Tint))).UpdateObjectColor(rippleObj);
	}
}
