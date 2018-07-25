using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillBox : MonoBehaviour {

	BoxCollider2D boxColl;

	void Start () {
		boxColl = GetComponent<BoxCollider2D>();
		float screenWidthToWorld = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.nearClipPlane)).x * 2;
		float screenHeightToWorld = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.nearClipPlane)).y * 2;
		boxColl.size = new Vector2(screenWidthToWorld, screenHeightToWorld);
	}

	//void OnTriggerExit2D (Collider2D coll) {
	//	if (coll.gameObject.tag == "Ball") {
	//		GameObject.Find("GameManager").GetComponent<GameManager>().DestroyBall(coll.gameObject);
	//	}
	//}

}
