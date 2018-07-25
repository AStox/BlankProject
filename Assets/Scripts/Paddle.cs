using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paddle : MonoBehaviour
{

  [HideInInspector]
  public bool horizontal;
  // public float cooldownTime;
  public float widthRatio = 0.65f;
  TouchPad touchPad;
  Vector2 screenSize;
  Animator anim;
  bool followMouse;
  GameManager gameManager;

  void Awake()
  {
    gameManager = (GameManager)GameObject.FindObjectOfType(typeof(GameManager));
    Normalize();
    followMouse = false;
    StartCoroutine(ScaleWidthCoroutine(widthRatio, transform.localScale.x, 1f));
    touchPad = GameObject.Find("TouchPad").GetComponent<TouchPad>();
    anim = GetComponentInChildren<Animator>();
    screenSize = new Vector2(Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.nearClipPlane)).x * 2, Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.nearClipPlane)).y * 2);
  }

  void Start()
  {
    if (gameObject.name == "PaddleTop")
    {
      Destroy(transform.GetChild(2).gameObject);
      Destroy(transform.GetChild(1).gameObject);
    }
    else if (gameObject.name == "PaddleLeft")
    {
      Destroy(transform.GetChild(1).gameObject);
      Destroy(transform.GetChild(0).gameObject);
    }
    else if (gameObject.name == "PaddleRight")
    {
      Destroy(transform.GetChild(2).gameObject);
      Destroy(transform.GetChild(0).gameObject);
    }
    else
    {
      Destroy(transform.GetChild(2).gameObject);
      Destroy(transform.GetChild(1).gameObject);
      Destroy(transform.GetChild(0).gameObject);
    }
  }

  // Update is called once per frame
  void LateUpdate()
  {
    if (followMouse && !gameManager.inTutorial)
    {
      FollowMouse();
    }
  }

  //Follows mouse movement along one axis
  void FollowMouse()
  {
    if (!Settings.Instance.fullScreenControls)
    {
      //if (horizontal) {
      //     transform.localPosition = new Vector3(screenSize.x * touchPad.xPercentage * 1.4f, transform.position.y, transform.position.z);
      //} else {
      //     transform.localPosition = new Vector3(transform.position.x, screenSize.y * touchPad.yPercentage * 1.5f, transform.position.z);
      //}
      Bounds touchPadBounds = touchPad.GetComponent<BoxCollider2D>().bounds;
      if (horizontal)
      {
        float horizontalWeight = 2.5f;
        float xPosition = (touchPad.worldPoint.x * horizontalWeight) + (touchPad.paddlePos.x - (touchPad.originalPos.x * horizontalWeight));
        if (xPosition / horizontalWeight < touchPadBounds.extents.x && xPosition / horizontalWeight > -touchPadBounds.extents.x && touchPad.worldPoint.x != touchPad.originalPos.x)
        {
          transform.localPosition = new Vector3(xPosition, transform.position.y, transform.position.z);
        }
      }
      else
      {
        float verticalWeight = 3.3f;
        float yPosition = (touchPad.worldPoint.y * verticalWeight) + (touchPad.paddlePos.y - (touchPad.originalPos.y * verticalWeight));
        if (yPosition / verticalWeight < touchPadBounds.extents.y && yPosition / verticalWeight > -touchPadBounds.extents.y && touchPad.worldPoint.y != touchPad.originalPos.y)
        {
          transform.localPosition = new Vector3(transform.position.x, yPosition, transform.position.z);
        }
      }
    }
    else
    {
      if (horizontal)
      {
        transform.localPosition = new Vector3(transform.localPosition.x + (((Camera.main.ScreenPointToRay(Input.mousePosition).origin.x * 1.4f) - transform.localPosition.x) * 0.2f), transform.position.y, transform.position.z);
      }
      else
      {
        transform.localPosition = new Vector3(transform.position.x, transform.localPosition.y + (((Camera.main.ScreenPointToRay(Input.mousePosition).origin.y * 1.5f) - transform.localPosition.y) * 0.2f), transform.position.z);
      }
    }
  }

  //Sets width to full screen size
  void Normalize()
  {
    float screenWidthToWorld = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.nearClipPlane)).x * 2;
    float paddleWidth = gameObject.GetComponentInChildren<PolygonCollider2D>().bounds.extents.x * 2;
    gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x * screenWidthToWorld / paddleWidth, gameObject.transform.localScale.y, gameObject.transform.localScale.z);
  }

  public void ScaleWidth(float amount, float xScale, float time)
  {
    StartCoroutine(ScaleWidthCoroutine(amount, xScale, time));
  }

  IEnumerator ScaleWidthCoroutine(float amount, float xScale, float time)
  {
    int frames = (int)(time / Time.deltaTime);
    for (int i = 0; i < frames; i++)
    {
      float lerpAmount = Mathf.Lerp(1f, amount, (float)(i + 1) / frames);
      transform.localScale = new Vector3(xScale * lerpAmount, transform.localScale.y, transform.localScale.z);
      yield return new WaitForSeconds(0.01f);
      //yield return null;
    }
    followMouse = true;
  }

  void OnCollisionEnter2D(Collision2D coll)
  {
    Audio.Instance.PlaySFX("RubberThudHigh", 1f, Random.Range(2f, 2.5f));
    GetComponent<Animator>().Play("Hit");
  }

}
