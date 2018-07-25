using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpExtraBalls : MonoBehaviour {

	public int ballCount;
	bool hit;

	void Start () {
		hit = false;
		GameObject.Find("GameManager").GetComponent<GameManager>().powerUpExists = true;
	}

	void OnTriggerEnter2D (Collider2D coll) {
		if (!hit) {
			if (coll.gameObject.tag == "Ball") {
				hit = true;
				for (int i = 0; i < ballCount - 1; i++) {
					GameObject.Find("GameManager").GetComponent<GameManager>().CreateBall(coll.transform.localPosition, coll.transform.localRotation, coll.attachedRigidbody.velocity);
				}
				Destroy(gameObject);
			}
		}
	}

}
