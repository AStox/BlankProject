using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour {

	public float rotationSpeed;

	void Start () {
    ((Tint)GameObject.FindObjectOfType(typeof(Tint))).UpdateObjectColor(gameObject);
	}

	// Update is called once per frame
	void Update () {
    if (gameObject.name == "RotatingShields")
    {
      transform.Rotate(Vector3.forward * Time.deltaTime * rotationSpeed);
    }
	}

	void OnCollisionEnter2D (Collision2D coll) {
    Audio.Instance.PlaySFX("Tink", 0.5f, Random.Range(1.5f, 1.7f));
	}
}
