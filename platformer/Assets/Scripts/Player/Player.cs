using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Player : MonoBehaviour
{
#region variables
    public PlayerData _playerData;
    GameManager _gameManager;
    Rigidbody2D _rigidbody2D;
    Animator _animator;

    AudioSource audioPlayer;
    public AudioClip clipJump;
    public AudioClip clipWalk;
    public AudioClip clipAttack;
    public AudioClip clipDash;
    public AudioClip clipSlide;
    public AudioClip clipDeath;
    private float volumeScale = 0.9f;

    int level;
    private Vector2 startingPosition;

    private Slider dashslider;
    private Slider jumpslider;
    
    public enum state{  // TODO: attach sprites to states
        idle,
            // attacking,
        walking,
            // walkAttacking,
        jumping,  // Note: anything past this counts as jumping (index: 3)
                //jumpWalking,  // TODO: implement falling
            dashing,
            dbJumping,
        wallSliding,
            wallJumping
    }
    
    state playerState = state.idle;

    GameObject feet;  // bottom of player, used to check if we are on the ground
    public LayerMask whatIsGround;
    GameObject[] walls;

    public int defNumDbJumps = 1;
    public int defNumWallJumps = 1;
    public int defNumDashes = 1;

    float jumpCooldown = 0f;
    float dashCooldown = 0f;

    float walkSpeed = 4f;
    float dashDist = 7f;
    float jumpHeight = 5.4f;
    float dbJumpHeight = 4.5f;

    bool isAttacking = false;
    int dashes;
    int dbleJumps;
    int wallJumps;
    
    bool is_grounded = true;
    int facing;
#endregion

    void Start()
    {
        _gameManager = GameObject.FindObjectOfType<GameManager>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();

        startingPosition = transform.position;
        walls = GameObject.FindGameObjectsWithTag("wall");
        feet = GameObject.FindGameObjectWithTag("playerFeet");

        dbleJumps = defNumDbJumps;
        wallJumps = defNumWallJumps;
        dashes = defNumDashes;

        dashslider = GameObject.FindGameObjectWithTag("DashSlider").GetComponent<Slider>();
        jumpslider = GameObject.FindGameObjectWithTag("JumpSlider").GetComponent<Slider>();

        audioPlayer = GetComponent<AudioSource>();
        // audioPlayer.playOnAwake = false;
    }

    void FixedUpdate()
    {
        float xSpeed = Input.GetAxis("Horizontal") * walkSpeed;
        float xScale = transform.localScale.x;
        if((xSpeed < 0 && xScale > 0) || (xSpeed > 0 && xScale < 0)) {
            transform.localScale *= new Vector2(-1, 1);
        }
    }
    
    void Update()
    {
        is_grounded = Physics2D.OverlapCircle(feet.transform.position, .1f, whatIsGround);
    #region cooldowns
        if (dashCooldown > 0){
            dashCooldown -= Time.deltaTime;
        }
        else{
            dashCooldown = 0f;  // safety in case time subtracts more than necessary
            _animator.ResetTrigger("Dash");
        }

        if (jumpCooldown > 0){
            jumpCooldown -= Time.deltaTime;
        }
        else{
            jumpCooldown = 0;
            _animator.ResetTrigger("DbJump");
            _animator.ResetTrigger("WallJump");
        }
        dashslider.value = (dashes > 0) ? 1.6f - dashCooldown : 0f;
        jumpslider.value = (defNumDbJumps == dbleJumps || dbleJumps > 0) ? (0.6f - jumpCooldown) : 0f;
        _rigidbody2D.angularVelocity = 0f; // TODO: make sure obj doesnt rotate
    #endregion

        Debug.Log("Player State: " + playerState.ToString()
                  + ", Facing: " + facing.ToString()
                  + ", Jumps: "+ (dbleJumps, wallJumps).ToString()
                + ", Attacking: " + isAttacking.ToString()
                // + ", Grounded|Still?" + (is_grounded, is_still()).ToString()
                  + ", Dash CD: " + dashCooldown.ToString()
                  + ", Jump CD: " + jumpCooldown.ToString()
                  + ", WallDir?: " + check_for_wall().ToString());

        movementControl();
    }



    private void OnTriggerEnter2D(Collider2D other) {
        // if player collides with reward, destroy reward
        if (other.CompareTag("reward")) {
            // print("reward");
            Debug.Log("REWARD");
            _gameManager.GetComponent<GameManager>().RewardInc();
            Destroy(other.gameObject);   
        }
        else if (other.CompareTag("door")) {
            print("door");
            if (PublicVars.nextLevel) {
                _gameManager.GetComponent<GameManager>().ZeroObtainedCrystals();
                level = SceneManager.GetActiveScene().buildIndex + 1;
                SceneManager.LoadScene(level);
                if (level != 6) {
                    _gameManager.GetComponent<GameManager>().LoadTutorial();
                }
            }
        }
        else if (other.CompareTag("killzone")) {
            print("killzone");
            transform.position = startingPosition;
            audioPlayer.PlayOneShot(clipDeath, volumeScale*7);
            StartCoroutine(_gameManager.GetComponent<GameManager>().ShowTutorial());
        }
    }

    private int check_for_wall(){
        // finds nearest wall and returns -1 if left of user, 0 if not close enough, and 1 if right of user
        if (walls.Length == 0){
            return 0;
        }
        else{
            GameObject closest = walls[0];   // find closest wall
            float shortest_dist = Vector2.Distance(transform.position, closest.transform.position);
            foreach (GameObject temp_wall in walls){
                float dist = Vector2.Distance(transform.position, temp_wall.transform.position);
                if (dist < shortest_dist){
                    closest = temp_wall;
                    shortest_dist = dist;
                }
            }
            return closest.GetComponent<DetectPlayer>().wallToPlayer();  // return that wall's detection of player
        }
    }

    private bool is_still(bool alsoY = false){  // not moving much
        // Note: vertical stillness must be < descent speed on wall sliding
        return Mathf.Abs(_rigidbody2D.velocity.x) <= 0.01f && ((alsoY) ? (Mathf.Abs(_rigidbody2D.velocity.y) <= 0.02f) : true);
    }
    


    public state get_state(){
        return playerState;
    }

    public (int, int) addJumps(int doubles, int walls){
        // TODO: add power-ups that change this value?
        dbleJumps += doubles;
        wallJumps += walls;
        return (dbleJumps, wallJumps);
    }

    void movementControl() {
        bool wantsJump = Input.GetKey(KeyCode.Space) || (Input.GetButton("A Button")); // jumping = space bar | Xbox A (button0)
        float xMove = Input.GetAxis("Horizontal");

        if (is_grounded){  // on the ground
            dbleJumps = defNumDbJumps;
            wallJumps = defNumWallJumps;
            dashes = defNumDashes;
            _animator.SetBool("Jump", false);
            _animator.SetBool("WallSlide", false);

            if ((Input.GetButton("Fire3") || (Input.GetButton("X Button"))) && dashCooldown == 0) {  // dashing = Shift | Xbox B (button1)
                dash();
            }
            else if (wantsJump && jumpCooldown == 0) { // jumping
                jump();
            }
            else if (xMove != 0){  // walking
                walk(xMove, false);
            }
            else{
                if (is_still()){
                    playerState = state.idle; // player idle
                    _animator.SetBool("Idle", true);
                    // _animator.SetBool("Walk", false);
                    _animator.SetFloat("Walk", 0f);
                }
                else{
                    _animator.SetFloat("Walk", Mathf.Abs(_rigidbody2D.velocity.x));
                    // _animator.SetBool("Walk", true);
                    _animator.SetBool("Idle", false);
                    playerState = state.walking;
                }
            }
        }
        else {  // mid-air
            _animator.SetBool("Idle", false);
            int wallDir = check_for_wall();
            int moveDir = (xMove > 0) ? 1 : -1;
            if (wallDir != 0 && _rigidbody2D.velocity.y <= 0
                && playerState != state.wallJumping && (xMove == 0 || moveDir == wallDir)){
                wallSlide( (xMove == 0) ? false : true );
            }

            if (playerState == state.wallSliding){  // currently sliding on a wall
                _animator.SetBool("Jump", false);
                _animator.SetBool("WallSlide", true);
                if((Input.GetButton("Fire3") || Input.GetButton("X Button")) && dashCooldown == 0){  // dashing
                    dash(-wallDir);  // can only dash opposite to wall (-facing)?
                }
                else if (wantsJump && jumpCooldown == 0 && wallJumps > 0) { // wall-jump
                    wallJump(wallDir);
                }
                else{
                    // Note: can only move opposite to wall when wall-sliding, else slide persists
                    if (moveDir != wallDir){
                        walk(xMove, true);
                    }
                }
            }
            else{
                _animator.SetBool("Jump", true);
                _animator.SetBool("WallSlide", false);
                if((Input.GetButton("Fire3") || Input.GetButton("X Button")) && dashCooldown == 0 && dashes > 0){  // dashing = Shift | Xbox B (button1)
                    dash();
                }
                else if (wantsJump && jumpCooldown == 0 && dbleJumps > 0) {  // double-jumping
                    doubleJump();
                }
                else if (xMove != 0){  // walking
                    walk(xMove, true);
                }
                else{
                    _animator.SetFloat("Walk", 0f);
                    // _animator.SetBool("Walk", false);
                    playerState = state.jumping;
                }
            }
        }
        
        if (Input.GetKey(KeyCode.Mouse0)){ // attacking = LMB | Xbox X (button2)
            //TODO: maybe add a attacking cooldown
            attack();
            isAttacking = true;
        }
        else{
            isAttacking = false;
        }

        if (Input.GetKey(KeyCode.Tab) ){  // pause menu = Tab | Xbox start (button7)
            //TODO: add Input.GetButtonDown("joystick button 7")
            //TODO: add pause menu
        }
    }

#region movement
    void walk(float xMove, bool isAirborn = false) {
        audioPlayer.PlayOneShot(clipWalk, volumeScale*0.02f);  // sounds off so taken out
        facing = (xMove >= 0) ? 1 : -1;
        if (isAirborn == true){
            float addV = (_rigidbody2D.velocity.x*facing < walkSpeed*.5f) ? xMove*walkSpeed*0.007f : 0f;
            // Note: we use dashDist here bc thats the fastest we want the user to be able to go in 
            _rigidbody2D.velocity += new Vector2(addV, 0);
            playerState = state.walking;
        }
        else{
            _rigidbody2D.velocity = new Vector2(xMove*walkSpeed/1.75f, _rigidbody2D.velocity.y);
            // Note: we move slower horizontally in the air
            playerState = state.walking;
        }
        
        if (playerState != state.wallSliding){
            _animator.SetFloat("Walk", Mathf.Abs(_rigidbody2D.velocity.x));
        }
        _animator.SetBool("Idle", false);
    }

    void jump() {
        _animator.SetBool("Jump", true);
        _animator.SetBool("Idle", false);
        audioPlayer.PlayOneShot(clipJump, volumeScale*12f);
        if(is_grounded) {
            playerState = state.jumping;
            _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, jumpHeight);
            jumpCooldown += 0.3f;
        }
    }

    void dash(int face_override = 0) {
        _animator.SetTrigger("Dash");
        _animator.SetBool("Idle", false);
        //Note: we half our vertical velocity (better feel)
        audioPlayer.PlayOneShot(clipDash, volumeScale*10f);
        if (face_override == 0) {
            _rigidbody2D.velocity = new Vector2(dashDist*facing, _rigidbody2D.velocity.y/4);
        }
        else{
            _rigidbody2D.velocity = new Vector2(dashDist*face_override, _rigidbody2D.velocity.y/4);
            facing = face_override;
        }
        
        if(is_grounded) {
            dashCooldown += 1.2f;
        }
        else {
            dashCooldown += 1.6f;
        }
        dashes -= 1;
        playerState = state.dashing;
    }

    void doubleJump() {
        _animator.SetTrigger("DbJump");
        _animator.SetBool("Idle", false);
        audioPlayer.PlayOneShot(clipJump, volumeScale*12f);
        if (_rigidbody2D.velocity.y > dbJumpHeight){
            _rigidbody2D.velocity += new Vector2(0, dbJumpHeight);
        }
        else{  // cancel current momentum if we aren't going up
            _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, dbJumpHeight);
        }
        playerState = state.dbJumping;
        dbleJumps -= 1;
        jumpCooldown += 0.6f;
    }

    void wallJump(int wallDir) {
        _animator.SetTrigger("WallJump");
        _animator.SetBool("Idle", false);
        audioPlayer.PlayOneShot(clipJump, volumeScale*10f);
        playerState = state.wallJumping;
        _rigidbody2D.velocity = new Vector2(-wallDir*walkSpeed/1.1f, dbJumpHeight*1.5f);
        wallJumps -= 1;
        jumpCooldown += 0.6f;
        facing *= -1; // switch facing directions
        // dbleJumps += 1;
        // dashCooldown = 0;
    }

    void wallSlide(bool slowly){
       _animator.SetBool("WallSlide", true);
        _animator.SetBool("Idle", false);
        // TODO: implement user sliding down a wall
        audioPlayer.PlayOneShot(clipSlide, volumeScale*0.2f);
        playerState = state.wallSliding;
        if (slowly){
            _rigidbody2D.velocity = new Vector2(0, -0.1f); // move down wall slower when moving toward wall
        }
        else{
            _rigidbody2D.velocity = new Vector2(0, -0.5f);
        }
    }
#endregion

    //attack works both in air and on ground, and with pogoing
    void attack() {
        audioPlayer.PlayOneShot(clipAttack, volumeScale);
        //float speedDif = Data.targetSpeed - _rigidbody2D.velocity.x;
        //float movement = speedDif * accelRate;
        //_rigidbody2D.AddForce(movement * Vector2.right, ForceMode2D.Force); //knockback, will come back to fix
        //Instantiate actual atk that does damage, going sideways
        //playerState = state.attacking;
    }
}
