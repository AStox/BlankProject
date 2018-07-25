using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour {

  public GameObject onSprite;
  public GameObject offSprite;
  bool on;
  public GameObject[] switchObjects;
  public Switch[] connectedSwitches;
  Tint tint;

  private void Awake()
  {
    tint = ((Tint)GameObject.FindObjectOfType(typeof(Tint)));
    tint.UpdateObjectColor(gameObject);
  }

  void Start () {
    on = switchObjects[0].activeSelf;
    onSprite.SetActive(on);
    offSprite.SetActive(!on);
	}
	
	public void SwitchHit (bool onBool) {
    for (int i = 0; i < switchObjects.Length; i++) {
      switchObjects[i].SetActive(onBool);
      tint.UpdateObjectColor(switchObjects[i]);
    }
		onSprite.SetActive(onBool);
    offSprite.SetActive(!onBool);
    on = onBool;
	}

	void OnTriggerEnter2D(Collider2D other) {
    foreach (Switch switchObj in connectedSwitches) {
      switchObj.SwitchHit(!on);
    }
	}
}
