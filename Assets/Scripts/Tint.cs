using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using SubjectNerd.Utilities;

public class Tint : MonoBehaviour {

  [SerializeField]
  [Reorderable]
  public ColorScheme[] levels;
  [HideInInspector]
  public GameObject backgroundParticles;

  bool gameplay;
  public int index;

  Ball[] balls;
  Paddle[] paddles;
  GameObject[] paddleParticles;
  Blocker[] blockers;
  Star[] stars;
  Switch[] switches;
  GameObject blockerDeathRipples;
  GameObject[] blockerBackgroundParticles;
  GameObject[] shadows;
  GameObject[] blockerSpawnRipples;
  GameObject[] paddleSecondary;
  GameObject[] shields;
  GameObject[] blockerSpawnParticles;
  GameObject[] borders;

	void Start () {
    if (!Settings.Instance.continued)
    {
      Shuffle(levels);
    }
    else
    {
      levels = Score.Instance.colorSchemes;
    }
    gameplay = false;
    gameplay = (SceneManager.GetActiveScene().name == "Gameplay");


    if (levels[Score.Instance.level % levels.Length].backgroundParticleObj != null)
		{
      backgroundParticles = Instantiate(levels[Score.Instance.level % levels.Length].backgroundParticleObj, Vector3.zero, Quaternion.identity);
      UpdateObjectColor(backgroundParticles);
    }
    UpdateColors();
	}

  void Shuffle(ColorScheme[] colorSchemes)
  {
    for (int t = 1; t < colorSchemes.Length; t++)
    {
      ColorScheme tmp = colorSchemes[t];
      int r = Random.Range(t, colorSchemes.Length);
      colorSchemes[t] = colorSchemes[r];
      colorSchemes[r] = tmp;
    }
  }

  public void UpdateColors() {
    index = Score.Instance.level % levels.Length;
		GetComponent<SpriteRenderer>().color = levels[index].backgroundColor;

    Gradient ballParticleGrad;
    Gradient blockerSpawnParticleGrad;
    Gradient blockerSpawnRippleGrad;
    Gradient starParticleGrad;
    Gradient blockerBackgroundParticlesGrad;
    Gradient paddleParticlesGrad;
    ballParticleGrad = new Gradient();
    blockerSpawnParticleGrad = new Gradient();
    blockerSpawnRippleGrad = new Gradient();
    starParticleGrad = new Gradient();
    blockerBackgroundParticlesGrad = new Gradient();
    paddleParticlesGrad = new Gradient();

		balls = (Ball[])GameObject.FindObjectsOfType(typeof(Ball));
    paddles = (Paddle[])GameObject.FindObjectsOfType(typeof(Paddle));
    paddleParticles = GameObject.FindGameObjectsWithTag("PaddleParticles");
		blockers = (Blocker[])GameObject.FindObjectsOfType(typeof(Blocker));
		stars = (Star[])GameObject.FindObjectsOfType(typeof(Star));
		switches = (Switch[])GameObject.FindObjectsOfType(typeof(Switch));
    blockerBackgroundParticles = GameObject.FindGameObjectsWithTag("BackgroundParticles");
		shadows = GameObject.FindGameObjectsWithTag("Shadow");
    blockerSpawnRipples = GameObject.FindGameObjectsWithTag("BlockerSpawnRipple");
		paddleSecondary = GameObject.FindGameObjectsWithTag("PaddleSecondaryColor");
		shields = GameObject.FindGameObjectsWithTag("Shield");
    blockerSpawnParticles = GameObject.FindGameObjectsWithTag("BlockerSpawnParticles");
    blockerDeathRipples = GameObject.Find("BlockerDeathRipples");
		borders = GameObject.FindGameObjectsWithTag("Border");

		for (int i = 0; i < balls.Length; i++)
		{
      balls[i].GetComponentInChildren<SpriteRenderer>().color = levels[index].ballColor;
		}

		for (int i = 0; i < borders.Length; i++)
		{
      borders[i].GetComponent<SpriteRenderer>().color = levels[index].borderColor;
		}

		for (int i = 0; i < paddles.Length; i++)
		{
			paddles[i].GetComponent<SpriteRenderer>().color = levels[index].foregroundColor;
		}

		for (int i = 0; i < shields.Length; i++)
		{
			shields[i].GetComponent<SpriteRenderer>().color = levels[index].shieldColor;
		}

    for (int i = 0; i < paddleSecondary.Length; i++)
    {
      paddleSecondary[i].GetComponent<SpriteRenderer>().color = levels[index].ballColor;
    }

		for (int i = 0; i < switches.Length; i++)
		{
      foreach (SpriteRenderer sprite in switches[i].GetComponentsInChildren<SpriteRenderer>()) {
        sprite.color = levels[index].starColor;
      }
		}

		for (int i = 0; i < blockers.Length; i++)
		{
			blockers[i].GetComponentInChildren<SpriteRenderer>().color = levels[index].foregroundColor;
      blockers[i].GetComponentInChildren<Text>().color = levels[index].blockerTextColor;
		}

    for (int i = 0; i < shadows.Length; i++)
    {
      shadows[i].GetComponentsInChildren<SpriteRenderer>()[0].color = new Color(levels[index].blockerSpawnParticleColor.r, levels[index].blockerSpawnParticleColor.g, levels[index].blockerSpawnParticleColor.b, 0.25f);
      shadows[i].GetComponentsInChildren<SpriteRenderer>()[1].color = new Color(levels[index].blockerSpawnParticleColor.r, levels[index].blockerSpawnParticleColor.g, levels[index].blockerSpawnParticleColor.b, 0.15f);
    }

		starParticleGrad.SetKeys(new GradientColorKey[] { new GradientColorKey(levels[index].blockerParticleColor, 0.0f), new GradientColorKey(levels[index].blockerParticleColor, 1.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(levels[index].blockerParticleColor.a, 0.0f), new GradientAlphaKey(levels[index].blockerParticleColor.a, 1.0f) });
		for (int i = 0; i < stars.Length; i++)
		{
			stars[i].GetComponentInChildren<SpriteRenderer>().color = levels[index].starColor;
			ParticleSystem ps = stars[i].GetComponentInChildren<ParticleSystem>();
      if (ps != null) {
				var col = ps.colorOverLifetime;
				col.enabled = true;
				col.color = starParticleGrad;
      }
		}

		GameObject grid = GameObject.Find("Grid");
		grid.GetComponent<SpriteRenderer>().color = levels[index].gridColor;

    GameObject score = GameObject.Find("ScoreText");
		score.GetComponent<Text>().color = levels[index].scoreColor;

    blockerSpawnParticleGrad.SetKeys(new GradientColorKey[] { new GradientColorKey(levels[index].blockerSpawnParticleColor, 0.0f), new GradientColorKey(levels[index].blockerSpawnParticleColor, 1.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(levels[index].blockerSpawnParticleColor.a, 0.0f), new GradientAlphaKey(levels[index].blockerSpawnParticleColor.a, 1.0f) });
    paddleParticlesGrad.SetKeys(new GradientColorKey[] { new GradientColorKey(levels[index].foregroundColor, 0.0f), new GradientColorKey(levels[index].foregroundColor, 1.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(0.8f, 0.0f), new GradientAlphaKey(0.8f, 1.0f) });
    blockerBackgroundParticlesGrad.SetKeys(new GradientColorKey[] { new GradientColorKey(levels[index].blockerParticleColor, 0.0f), new GradientColorKey(levels[index].blockerParticleColor, 1.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(levels[index].blockerParticleColor.a, 0.0f), new GradientAlphaKey(levels[index].blockerParticleColor.a, 1.0f) });
    blockerSpawnRippleGrad.SetKeys(new GradientColorKey[] { new GradientColorKey(levels[index].blockerSpawnParticleColor, 0.0f), new GradientColorKey(levels[index].blockerSpawnParticleColor, 1.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(levels[index].blockerSpawnParticleColor.a, 0.0f), new GradientAlphaKey(0.0f, 1.0f) });
    ballParticleGrad.SetKeys(new GradientColorKey[] { new GradientColorKey(levels[index].ballParticleColor, 0.0f), new GradientColorKey(levels[index].ballParticleColor, 1.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(levels[index].ballParticleColor.a, 0.0f), new GradientAlphaKey(levels[index].ballParticleColor.a, 1.0f) });

    for (int i = 0; i < balls.Length; i++)
    {
      balls[i].GetComponentInChildren<TrailRenderer>().colorGradient = ballParticleGrad;
    }

    for (int i = 0; i < paddleParticles.Length; i++)
    {
      if (paddleParticles[i].GetComponent<ParticleSystem>() != null)
      {
        var col = paddleParticles[i].GetComponent<ParticleSystem>().colorOverLifetime;
        col.enabled = true;
        col.color = paddleParticlesGrad;
      }
    }

    for (int i = 0; i < blockerBackgroundParticles.Length; i++)
    {
      if (blockerBackgroundParticles[i].GetComponent<ParticleSystem>() != null)
      {
        var col = blockerBackgroundParticles[i].GetComponent<ParticleSystem>().colorOverLifetime;
        col.enabled = true;
        col.color = blockerBackgroundParticlesGrad;
      }
    }

    for (int i = 0; i < blockerSpawnRipples.Length; i++)
    {
      var col = blockerSpawnRipples[i].GetComponent<ParticleSystem>().colorOverLifetime;
      col.enabled = true;
      col.color = blockerSpawnRippleGrad;
    }
    if (blockerDeathRipples != null)
    {
        var col = blockerDeathRipples.GetComponent<ParticleSystem>().colorOverLifetime;
        col.enabled = true;
        col.color = blockerSpawnParticleGrad;
    }

    for (int i = 0; i < blockerSpawnParticles.Length; i++)
    {
      var col = blockerSpawnParticles[i].GetComponent<ParticleSystem>().colorOverLifetime;
      col.enabled = true;
      col.color = blockerSpawnParticleGrad;
    }

    if (levels[index].backgroundParticleObj != null) {
      ParticleSystem[] ps = backgroundParticles.GetComponentsInChildren<ParticleSystem>();
      for (int i = 0; i < ps.Length; i++)
      {
        var col = ps[i].colorOverLifetime;
        col.enabled = true;
        col.color = levels[index].backgroundParticleGradient;
      }
    }
    
  }

  public void UpdateObjectColor(GameObject obj)
  {
    index = Score.Instance.level % levels.Length;

    if (obj.GetComponent<Ball>() != null)
    {
      Gradient ballParticleGrad;
      ballParticleGrad = new Gradient();
      ballParticleGrad.SetKeys(new GradientColorKey[] { new GradientColorKey(levels[index].ballParticleColor, 0.0f), new GradientColorKey(levels[index].ballParticleColor, 1.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(levels[index].ballParticleColor.a, 0.0f), new GradientAlphaKey(levels[index].ballParticleColor.a, 1.0f) });
      obj.GetComponentInChildren<SpriteRenderer>().color = levels[index].ballColor;
      obj.GetComponentInChildren<TrailRenderer>().colorGradient = ballParticleGrad;
    }

    if (obj.CompareTag("PaddleSecondaryColor"))
    {
      obj.GetComponent<SpriteRenderer>().color = levels[index].ballColor;
    }

    //multiple blockers
    if (obj.GetComponentsInChildren<Blocker>() != null)
    {
      Blocker[] blockers = obj.GetComponentsInChildren<Blocker>();
      for (int i = 0; i < blockers.Length; i++)
      {
        SpriteRenderer sprite = blockers[i].GetComponentInChildren<SpriteRenderer>();
        sprite.color = levels[index].foregroundColor;
        blockers[i].GetComponentInChildren<Text>().color = levels[index].blockerTextColor;
        if (blockers[i].transform.root.name != "BlockerPinwheel(animHolder)(Clone)")
        {
          GameObject shadow = new GameObject();
          shadow.name = "Shadow";
          SpriteRenderer shadowSprite = shadow.AddComponent<SpriteRenderer>();
          shadowSprite.sprite = sprite.sprite;
          shadowSprite.color = new Color(0, 0, 0, 0.196f);
          shadow.transform.localPosition = new Vector3(blockers[i].transform.position.x, blockers[i].transform.position.y - 0.17f, blockers[i].transform.position.y + 0.01f);
          shadow.transform.localScale = blockers[i].transform.lossyScale;
          shadow.transform.rotation = blockers[i].transform.rotation;
          shadow.transform.parent = blockers[i].transform;
          shadow.transform.localPosition = new Vector3(shadow.transform.localPosition.x, shadow.transform.localPosition.y, 0.1f);
        }
      }
    }

    if (obj.GetComponent<Star>() != null)
    {
      obj.GetComponent<SpriteRenderer>().color = levels[index].starColor;
    }

    if (obj.name == "StarSpawnParticles")
    {
      if (obj.GetComponent<ParticleSystem>() != null)
      {
        Gradient blockerBackgroundParticlesGrad;
        blockerBackgroundParticlesGrad = new Gradient();
        blockerBackgroundParticlesGrad.SetKeys(new GradientColorKey[] { new GradientColorKey(levels[index].starColor, 0.0f), new GradientColorKey(levels[index].starColor, 1.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(levels[index].starColor.a, 0.0f), new GradientAlphaKey(levels[index].starColor.a, 1.0f) });
        var col = obj.GetComponent<ParticleSystem>().colorOverLifetime;
        col.enabled = true;
        col.color = blockerBackgroundParticlesGrad;
      }
    }

    if (obj.GetComponent<Switch>() != null)
    {
      obj.GetComponentsInChildren<SpriteRenderer>()[0].color = levels[index].starColor;
      obj.GetComponentsInChildren<SpriteRenderer>()[1].color = levels[index].starColor;
    }

    if (obj.CompareTag("BackgroundParticles"))
    {
      if (obj.GetComponent<ParticleSystem>() != null)
      {
        Gradient blockerBackgroundParticlesGrad;
        blockerBackgroundParticlesGrad = new Gradient();
        blockerBackgroundParticlesGrad.SetKeys(new GradientColorKey[] { new GradientColorKey(levels[index].blockerParticleColor, 0.0f), new GradientColorKey(levels[index].blockerParticleColor, 1.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(levels[index].blockerParticleColor.a, 0.0f), new GradientAlphaKey(levels[index].blockerParticleColor.a, 1.0f) });
        var col = obj.GetComponent<ParticleSystem>().colorOverLifetime;
        col.enabled = true;
        col.color = blockerBackgroundParticlesGrad;
      }
    }

    if (obj.CompareTag("Shadow"))
    {
      obj.GetComponentsInChildren<SpriteRenderer>()[0].color = new Color(levels[index].blockerSpawnParticleColor.r, levels[index].blockerSpawnParticleColor.g, levels[index].blockerSpawnParticleColor.b, 0.25f);
      obj.GetComponentsInChildren<SpriteRenderer>()[1].color = new Color(levels[index].blockerSpawnParticleColor.r, levels[index].blockerSpawnParticleColor.g, levels[index].blockerSpawnParticleColor.b, 0.25f);
    }

    if (obj.CompareTag("BlockerSpawnRipple"))
    {
      Gradient blockerSpawnRippleGrad;
      blockerSpawnRippleGrad = new Gradient();
      blockerSpawnRippleGrad.SetKeys(new GradientColorKey[] { new GradientColorKey(levels[index].blockerSpawnParticleColor, 0.0f), new GradientColorKey(levels[index].blockerSpawnParticleColor, 1.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(levels[index].blockerSpawnParticleColor.a, 0.0f), new GradientAlphaKey(0.0f, 1.0f) });
      var col = obj.GetComponent<ParticleSystem>().colorOverLifetime;
      col.enabled = true;
      col.color = blockerSpawnRippleGrad;
    }

    if (obj.CompareTag("Shield"))
    {
      obj.GetComponent<SpriteRenderer>().color = levels[index].shieldColor;
    }

    if (obj.CompareTag("BlockerSpawnParticles"))
    {
      Gradient blockerSpawnParticleGrad;
      blockerSpawnParticleGrad = new Gradient();
      blockerSpawnParticleGrad.SetKeys(new GradientColorKey[] { new GradientColorKey(levels[index].blockerSpawnParticleColor, 0.0f), new GradientColorKey(levels[index].blockerSpawnParticleColor, 1.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(levels[index].blockerSpawnParticleColor.a, 0.0f), new GradientAlphaKey(levels[index].blockerSpawnParticleColor.a, 1.0f) });
      var col = obj.GetComponent<ParticleSystem>().colorOverLifetime;
      col.enabled = true;
      col.color = blockerSpawnParticleGrad;
    }

    if (obj.name == "BlockerDeathRipples")
    {
      Gradient blockerSpawnParticleGrad;
      blockerSpawnParticleGrad = new Gradient();
      blockerSpawnParticleGrad.SetKeys(new GradientColorKey[] { new GradientColorKey(levels[index].blockerSpawnParticleColor, 0.0f), new GradientColorKey(levels[index].blockerSpawnParticleColor, 1.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(levels[index].blockerSpawnParticleColor.a, 0.0f), new GradientAlphaKey(levels[index].blockerSpawnParticleColor.a, 1.0f) });
      var col = obj.GetComponent<ParticleSystem>().colorOverLifetime;
      col.enabled = true;
      col.color = blockerSpawnParticleGrad;
    }
  }

}
