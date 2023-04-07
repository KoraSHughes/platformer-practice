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
    public TextMeshProUGUI levelText, scoreUI, crystalsUI;

    // messages for tutorials
    string walkJump;
    string dash;
    string doubleJump;
    string wallJump;
    string attack;

    int buildIndex;

    int crystalsObtained;
    public static bool pause = false;

    private int score = 0;
    public GameObject pauseUI;
    private int crystalsNeeded;
    private AudioSource reward;

    private void Awake() {
    if (GameObject.FindObjectsOfType<GameManager>().Length > 1) {
        Destroy(gameObject);
    }
    else {
        DontDestroyOnLoad(gameObject);
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        walkJump = "Press A/LEFT to go left\nPress D/RIGHT to go right\nPress SPACE to jump";
        dash = "Hold SHIFT + LEFT/RIGHT while jumping to dash";
        doubleJump = "Press SPACE two times to double jump";
        wallJump = "Press SPACE on the sides of walls\nto jump off of them";
        attack = "Put all your abilities to the test!";

        pauseUI.SetActive(false);

        // get build index to use to determine which message to show
        buildIndex = SceneManager.GetActiveScene().buildIndex;
        // call coroutine
        StartCoroutine(ShowTutorial());

        SetNeededCrystals();

        scoreUI.text = "SCORE: " + score;
        crystalsUI.text = "CRYSTALS: " + crystalsObtained + "/" + crystalsNeeded;

        reward = GetComponent<AudioSource>();
        if (reward == null){
            Debug.Log("Add Reward Audio to Game Manager!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        SetNeededCrystals();

        if (scoreUI == null) {
            scoreUI = GameObject.FindGameObjectWithTag("ScoreUI").GetComponent<TextMeshProUGUI>();
            scoreUI.text = "SCORE: " + score;
        }
        
        if (crystalsUI == null) {
            crystalsUI = GameObject.FindGameObjectWithTag("CrystalUI").GetComponent<TextMeshProUGUI>();
            crystalsUI.text = "CRYSTALS: " + crystalsObtained + "/" + crystalsNeeded;
        }

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
        crystalsObtained += 1;
        reward.Play();
        score += 10;
        scoreUI.text = "SCORE: " + score;
        crystalsUI.text = "CRYSTALS: " + crystalsObtained + "/" + crystalsNeeded;
        if (crystalsObtained == crystalsNeeded) {
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

    public void ZeroObtainedCrystals() {
        crystalsObtained = 0;
    }

    public void SetNeededCrystals() {
        string scene = SceneManager.GetActiveScene().name;
        if (scene == "Level1 Jump") {
            crystalsNeeded = 2;
        }
        else if (scene == "Level2 Dash") {
            crystalsNeeded = 3;
        }
        else if (scene == "Level3 Double") {
            crystalsNeeded = 5;
        }
        else if (scene == "Level4 WallJump") {
            crystalsNeeded = 3;
        }
        else if (scene == "Level5 Attack") {
            crystalsNeeded = 4;
        }
    }

    public void LoadTutorial() {
        // get build index to use to determine which message to show
        buildIndex = SceneManager.GetActiveScene().buildIndex + 1;
        // call coroutine
        StartCoroutine(ShowTutorial());
    }
    
}
