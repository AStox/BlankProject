using UnityEngine;
using System.Collections;

public class Audio : Singleton<Audio>
{
    [HideInInspector]
    public AudioSource backgroundMusic;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
      backgroundMusic = gameObject.AddComponent<AudioSource>();
      backgroundMusic.clip = Resources.Load("Roulette") as AudioClip;
      backgroundMusic.pitch = 1.1f;
      backgroundMusic.volume = 0.3f;
      backgroundMusic.loop = true;
      if (Settings.Instance.music)
      {
        backgroundMusic.Play();
      }
    }

    public void PlaySFX(string path)
    {
      if (Settings.Instance.sfx)
      {
  			AudioSource sfx = gameObject.AddComponent<AudioSource>();
  			sfx.clip = Resources.Load(path) as AudioClip;
  			sfx.Play();
  			StartCoroutine(WaitToRemove(sfx));
      }
    }

    public void PlaySFX(string path, float volume)
    {
        if (Settings.Instance.sfx)
        {
            AudioSource sfx = gameObject.AddComponent<AudioSource>();
            sfx.clip = Resources.Load(path) as AudioClip;
            sfx.volume = volume;
            sfx.Play();
            StartCoroutine(WaitToRemove(sfx));
        }
    }

    public void PlaySFX(string path, float volume, float pitch)
    {
        if (Settings.Instance.sfx)
        {
			AudioSource sfx = gameObject.AddComponent<AudioSource>();
			sfx.clip = Resources.Load(path) as AudioClip;
            sfx.pitch = pitch;
			sfx.volume = volume;
			sfx.Play();
			StartCoroutine(WaitToRemove(sfx));
        }
    }

    IEnumerator WaitToRemove(AudioSource audio)
    {
        while (audio.isPlaying)
        {
            yield return null;
        }
        Destroy(audio);
    }
}
