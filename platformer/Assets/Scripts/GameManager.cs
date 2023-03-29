using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // check scene build index... 
    // display corresponding info (tutorial) for the abilities player has

    public TextMeshProUGUI levelText;
    string walkJump;
    // string dash = "Press A and SHIFT or left and SHIFT to double jump";
    // string doubleJump = "Press SPACE while in the air to double jump";
    // string wallJump = "When jumping on a wall, press SPACE to jump over it";
    // string attack = "Press the Left Mouse Button to attack the enemy";

    int buildIndex;


    // Start is called before the first frame update
    void Start()
    {
        walkJump = "Press A to go left, Press D to go right, Press W to jump";
        buildIndex = SceneManager.GetActiveScene().buildIndex;
        // call coroutine
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayGame() {
        SceneManager.LoadScene(1);
    }

    public void QuitGame() {
        Application.Quit();
    }

    public IEnumerator ShowTutorial() {
        // if (buildIndex == 1) {
        //     levelText.text = walkJump;
        // }
        // if (buildIndex == 2) {
        //     levelText.text = dash;
        // }
        // if (buildIndex == 3) {
        //     levelText.text = doubleJump;
        // }
        // if (buildIndex == 4) {
        //     levelText.text = wallJump;
        // }
        // if (buildIndex == 5) {
        //     levelText.text = attack;
        // }
        levelText.text = walkJump;
        yield return new WaitForSeconds(3f);
        levelText.text = "";
    }
}
