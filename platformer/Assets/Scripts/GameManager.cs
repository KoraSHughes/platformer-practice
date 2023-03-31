using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // check scene build index... 
    // display corresponding info (tutorial) for the abilities player has

    public  UnityEngine.UI.RawImage bgImage;
    public TextMeshProUGUI levelText;

    // messages for tutorials
    string walkJump;
    string dash;
    string doubleJump;
    string wallJump;
    string attack;

    int buildIndex;

    public int totalRewards;
    int rewardsObtained;
    public static bool pause = false;
    public GameObject pauseUI;
    private AudioSource reward;
    
    // Start is called before the first frame update
    void Start()
    {
        walkJump = "Press A/LEFT to go left\nPress D/RIGHT to go right\nPress SPACE to jump";
        dash = "Hold SHIFT to dash";
        doubleJump = "Press SPACE while in the air to double jump";
        wallJump = "When jumping on a wall,\npress SPACE to jump over it";
        attack = "Press LMB to attack the enemy";

        pauseUI.SetActive(false);

        // get build index to use to determine which message to show
        buildIndex = SceneManager.GetActiveScene().buildIndex;
        // call coroutine
        StartCoroutine(ShowTutorial());

        reward = GetComponent<AudioSource>();
        if (reward == null){
            Debug.Log("Add Reward Audio to Game Manager!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (pause) {
                Resume();
            }
            else {
                Pause();
            }
        }
    }

    public void PlayGame() {
        SceneManager.LoadScene(1);
    }

    public void QuitGame() {
        Application.Quit();
    }

    public IEnumerator ShowTutorial() {
        // enable background
        bgImage.enabled = true;
        
        // change text of tutorial depending on scene
        if (buildIndex == 1) {
            levelText.text = walkJump;
        }
        else if (buildIndex == 2) {
            levelText.text = dash;
        }
        else if (buildIndex == 3) {
            levelText.text = doubleJump;
        }
        else if (buildIndex == 4) {
            levelText.text = wallJump;
        }
        else if (buildIndex == 5) {
            levelText.text = attack;
        }
        // bg.enabled = true;
        // levelText.text = walkJump; //walkJump;
        yield return new WaitForSeconds(5f);
        levelText.text = "";
        bgImage.enabled = false;
    }

    public void RewardInc() {
        rewardsObtained += 1;
        reward.Play();
        if (rewardsObtained == totalRewards) {
            PublicVars.nextLevel = true;
        }
    }

    public void Resume() {
        pauseUI.SetActive(false);
        Time.timeScale = 1f;
        pause = false;
    }
    
    public void Pause() {
        pauseUI.SetActive(true);
        Time.timeScale = 0f;
        pause = true;
    }
}
