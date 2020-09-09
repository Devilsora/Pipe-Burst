using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioLoseCheck : MonoBehaviour
{
    // Start is called before the first frame update

    public AudioSource aud;
    public AudioLowPassFilter lowPass;
    public AudioClip loseMusic;

    public GameObject loseScreen;
    public bool checkedLoseScreen;
    void Start()
    {
      aud = GetComponent<AudioSource>();
      lowPass = GetComponent<AudioLowPassFilter>();
    }

    // Update is called once per frame
    void Update()
    {
      if (loseScreen.activeInHierarchy && !checkedLoseScreen)
      {
        lowPass.enabled = false;
        aud.Stop();
        aud.clip = loseMusic;
        aud.Play();
        checkedLoseScreen = true;
      }
    }
}
