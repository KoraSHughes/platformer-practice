using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Buttons : MonoBehaviour
{
    public Animator animation;
    public GameManager _gameManager;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayGame() {
        print("PLAYING");
        animation.SetTrigger("fadeOut");
        SceneManager.LoadScene(1);
        _gameManager = GameObject.FindObjectOfType<GameManager>();
        _gameManager.score = 0;
       
    }
    public void QuitGame() {
        Application.Quit();
    }
}
