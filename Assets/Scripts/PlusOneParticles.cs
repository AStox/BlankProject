using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlusOneParticles : MonoBehaviour {

	// Use this for initialization
	void Start () {
        ParticleSystem ps = GetComponent<ParticleSystem>();
        Gradient particleGrad = new Gradient();
        Tint tint = (Tint)GameObject.FindObjectOfType(typeof(Tint));
        particleGrad.SetKeys(new GradientColorKey[] { new GradientColorKey(tint.levels[Score.Instance.level % tint.levels.Length].scoreColor, 0.0f), new GradientColorKey(tint.levels[Score.Instance.level % tint.levels.Length].scoreColor, 1.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(0.0f, 1.0f), new GradientAlphaKey(1.0f, 0.0f) });
		var col = GetComponent<ParticleSystem>().colorOverLifetime;
		col.color = particleGrad;
	}
}
