using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarSpawner : MonoBehaviour
{

	public GameObject starPatternPrefab;
	public bool starsAsChildren;
    GameObject starPattern;


	void Start()
	{
    starPattern = Instantiate(starPatternPrefab, GameObject.Find("Screen").GetComponent<BoxCollider2D>().bounds.center, Quaternion.identity);
    ((Tint)GameObject.FindObjectOfType(typeof(Tint))).UpdateObjectColor(starPattern);
    if (starsAsChildren)
		{
			StartCoroutine(WaitForAnimation(GetComponent<Animator>()));
		}
	}

  private IEnumerator WaitForAnimation(Animator anim)
  {
    //while (anim.GetCurrentAnimatorStateInfo(0).IsName("BlockerEnter"))
    //{
    //    yield return null;
    //}
    yield return new WaitForSeconds(0.4f);
    starPattern.transform.parent = transform;
  }

}
