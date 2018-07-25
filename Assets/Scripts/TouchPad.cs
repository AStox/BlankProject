using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchPad : MonoBehaviour {

	BoxCollider2D box;
	bool mouseDown;
	[HideInInspector]
	public float xPercentage;
	[HideInInspector]
	public float yPercentage;
	[HideInInspector]
	public Vector2 originalPos;
	[HideInInspector]
	public Vector2 paddlePos;
	[HideInInspector]
	public Vector2 worldPoint;

	void Start () {
		box = gameObject.GetComponent<BoxCollider2D>();
    	if (Settings.Instance.fullScreenControls) {
			Destroy(gameObject);
		}
	}

	// Update is called once per frame
	void Update () {
		if (mouseDown) {
			worldPoint = Camera.main.ScreenPointToRay(Input.mousePosition).origin;
			xPercentage = Mathf.Clamp((worldPoint.x - box.bounds.min.x)/(box.bounds.max.x - box.bounds.min.x) - .5f, -0.5f, 0.5f);
			yPercentage = Mathf.Clamp((worldPoint.y - box.bounds.min.y)/(box.bounds.max.y - box.bounds.min.y) - .5f, -0.5f, 0.5f);
		}
	}

	void OnMouseDown () {
		mouseDown = true;
		originalPos = Camera.main.ScreenPointToRay(Input.mousePosition).origin;
		paddlePos = new Vector2(GameObject.Find("PaddleTop").transform.localPosition.x, GameObject.Find("PaddleRight").transform.localPosition.y); 
	}

	void OnMouseUp () {
		mouseDown = false;
	}
}
