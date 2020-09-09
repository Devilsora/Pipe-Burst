using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIButtons : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RestartGame()
    {
      SceneManager.LoadScene(1);
    }

    public void BackToTitle()
    {
      SceneManager.LoadScene(0);
    }

    public void OpenPanel(GameObject obj)
    {
      obj.SetActive(true);
    }

    public void HidePanel(GameObject obj)
    {
      obj.SetActive(false);
    }
}
