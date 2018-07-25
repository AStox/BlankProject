using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class MainCamera : MonoBehaviour {

	public PostProcessingProfile normal;
	public PostProcessingProfile hit;
	PostProcessingBehaviour postProcessor;

	void Start () {
		postProcessor = GetComponent<PostProcessingBehaviour>();
		postProcessor.profile = normal;
	}

	public void Hit () {
		postProcessor.profile = hit;
		StartCoroutine(WaitToDo(Normal, 0.1f));
	}

	void Normal () {
		postProcessor.profile = normal;

	}

	IEnumerator WaitToDo(System.Action del, float time) {
		yield return new WaitForSeconds(time);
		del();
	}
}
