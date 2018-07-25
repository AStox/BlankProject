using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour {

	Animator anim;
	public GameObject destroyParticles;

	void Start () {
    ((Tint)GameObject.FindObjectOfType(typeof(Tint))).UpdateObjectColor(gameObject);
    ((Tint)GameObject.FindObjectOfType(typeof(Tint))).UpdateObjectColor(gameObject.transform.GetChild(0).gameObject);
		anim = GetComponent<Animator>();
		anim.SetFloat("Offset", Random.value);
	}

	// Update is called once per frame
	void Update () {

	}

	void OnTriggerEnter2D (Collider2D other) {
		if (other.gameObject.tag == "Ball") {
      Audio.Instance.PlaySFX("BellTinkle", 0.25f, Random.Range(1f, 1.5f));
			Score.Instance.IncrementScore(transform.position);
			GameObject starDestroyObj = Instantiate(destroyParticles, transform.position, Quaternion.identity);
      starDestroyObj.name = "StarSpawnParticles";
      ((Tint)GameObject.FindObjectOfType(typeof(Tint))).UpdateObjectColor(starDestroyObj);
			Destroy(gameObject);
		}
	}
}
