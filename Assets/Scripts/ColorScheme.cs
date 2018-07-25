using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;

[System.Serializable]
public struct ColorScheme {

    [SerializeField]
    public Color backgroundColor;

    [SerializeField]
    public Color borderColor;

    [SerializeField]
    public Color gridColor;

    [SerializeField]
    public Color foregroundColor;

    [SerializeField]
    public Color starColor;

    [SerializeField]
    public Color ballColor;

    [SerializeField]
    public Color ballParticleColor;

    [SerializeField]
    public Color blockerParticleColor;

    [SerializeField]
    public Color blockerSpawnParticleColor;

	[SerializeField]
    public Color scoreColor;

    [SerializeField]
    public Color blockerTextColor;

    [SerializeField]
    public GameObject backgroundParticleObj;

    [SerializeField]
    public Gradient backgroundParticleGradient;

	[SerializeField]
	public Color shieldColor;
}
