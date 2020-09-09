using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public AudioSource aud;
    public AudioSource BGM;

  // Start is called before the first frame update
  void Start()
    {
      aud = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
      if (Input.GetMouseButtonDown(0) || Input.anyKeyDown)
      {
        BGM.Stop();
        StartCoroutine(playAndStartGame());
        //wait until audio clip stops to switch scenes

      }
    }

    IEnumerator playAndStartGame()
    {
      aud.Play();
      yield return new WaitForSeconds(aud.clip.length);
      SceneManager.LoadScene(1);
    }
}
