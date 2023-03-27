using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
#region variables
    public PlayerData _playerData;
    GameManager _gameManager;
    Rigidbody2D _rigidbody2D;

    public enum state{  // TODO: attach sprites to states
        idle,
            // attacking,
        walking,
            // walkAttacking,
        jumping,  // Note: anything past this counts as jumping (index: 3)
                jumpWalking,
            dashing,
            dbJumping,
        wallSliding,
            wallJumping
    }
    
    state playerState = state.idle;

    public GameObject feet;  // bottom of player, used to check if we are on the ground
    public LayerMask whatIsGround;
    public GameObject left;
    public GameObject right;
    public LayerMask whatIsWall;
    bool wallOnLeft = false;
    bool wallOnRight = false;

    public int defNumDbJumps = 1;
    public int defNumWallJumps = 1;
    public int defNumDashes = 1;

    float jumpCooldown = 0f;
    float dashCooldown = 0f;

    float walkSpeed = 5;
    float dashDist = 7f;
    float jumpHeight = 4f;
    float dbJumpHeight = 3f;

    bool isAttacking = false;
    int dbleJumps;
    int wallJumps;
    
    bool is_grounded = true;
    int facing;
#endregion

    void Start()
    {
        _gameManager = GameObject.FindObjectOfType<GameManager>();
        _rigidbody2D = GetComponent<Rigidbody2D>();

        dbleJumps = defNumDbJumps;
        wallJumps = defNumWallJumps;
    }

    void FixedUpdate()
    {
        float xSpeed = Input.GetAxis("Horizontal") * walkSpeed;
        float xScale = transform.localScale.x;
        if((xSpeed < 0 && xScale > 0) || (xSpeed > 0 && xScale < 1))
            transform.localScale *= new Vector2(-1, 1);
    }
    
    void Update()
    {
        is_grounded = Physics2D.OverlapCircle(feet.transform.position, .1f, whatIsGround);
        wallOnLeft = Physics2D.OverlapCircle(left.transform.position, .01f, whatIsWall);
        wallOnRight = Physics2D.OverlapCircle(right.transform.position, .01f, whatIsWall);
        
    #region cooldowns
        if (dashCooldown > 0){
            dashCooldown -= Time.deltaTime;
        }
        else{
            dashCooldown = 0f;  // safety in case time subtracts more than necessary
        }

        if (jumpCooldown > 0){
            jumpCooldown -= Time.deltaTime;
        }
        else{
            jumpCooldown = 0;
        }

        _rigidbody2D.angularVelocity = 0f; // TODO: make sure obj doesnt rotate
    #endregion
        int tempDir = (Input.GetAxis("Horizontal") > 0) ? 1 : ((Input.GetAxis("Horizontal") == 0) ? 0 : -1);
        Debug.Log("Player State: " + playerState.ToString()
                  + ", Facing: " + facing.ToString()
                  + ", Jumps: "+ (dbleJumps, wallJumps).ToString()
                // + ", Attacking: " + isAttacking.ToString()
                // + ", Grounded|Still?" + (is_grounded, is_still()).ToString()
                //   + ", Dash CD: " + dashCooldown.ToString()
                //   + ", Jump CD: " + jumpCooldown.ToString()
                  + ", Wall(left, right): " + (wallOnLeft, wallOnRight).ToString()
                  + ", WallDir?: " + check_for_wall().ToString()
                  + ", MoveDir: " + tempDir.ToString());
        movementControl();
    }

    private int check_for_wall(){
        // finds nearest wall and returns -1 if left of user, 0 if not close enough, and 1 if right of user
        // Note: right and left flip on runtime so we swap logic here

        // TODO: fix (always returns right wall for some reason)
        if (wallOnRight){
            return -1;  
        }
        else if (wallOnLeft) {
            return 1;
        }
        else{
            return 0;
        }
    }
    private bool is_still(){  // not moving much
        // Note: vertical stillness must be < descent speed on wall sliding
        return _rigidbody2D.velocity.x <= 0.5f && _rigidbody2D.velocity.x >= -0.5f && _rigidbody2D.velocity.y <= 0.2f && _rigidbody2D.velocity.y >= -0.2f;
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

    void updateTimers() {

    }

    void movementControl() {
        float yMove = Input.GetAxis("Vertical");  // TODO: change this input
        bool wantsJump = false;
        if (yMove > 0){
            wantsJump = true;
        }
        float xMove = Input.GetAxis("Horizontal");

        if (is_grounded){  // on the ground
            dbleJumps = defNumDbJumps;
            wallJumps = defNumWallJumps;

            if (Input.GetButton("Fire3") && dashCooldown == 0) {  // dashing
                dash();
            }
            else if (wantsJump && jumpCooldown == 0) { // jumping
                jump();
            }
            else if (xMove != 0){  // walking
                walk(xMove);
            }
            else{
                if (is_still()){
                    playerState = state.idle; // player idle
                }
                else{
                    playerState = state.walking;
                }
            }
        }
        else {  // mid-air
            int wallDir = (check_for_wall() == 0) ? 0 : facing;
            int moveDir = (xMove >= 0) ? 1 : -1;
            if (wallDir != 0 && _rigidbody2D.velocity.y <= 0
                && playerState != state.wallJumping && xMove == 0){
                wallSlide();
            }

            if (playerState == state.wallSliding){  // currently sliding on a wall
                if(Input.GetButton("Fire3") && dashCooldown == 0){  // dashing
                    dash(-wallDir);  // can only dash opposite to wall (-facing)?
                }
                else if (wantsJump && jumpCooldown == 0 && wallJumps > 0) { // wall-jump
                    wallJump(wallDir);
                }
                else{
                    // Note: can only move opposite to wall when wall-sliding, else slide persists
                    if (xMove != 0){
                        Debug.Log("GOTHERE");
                        walk(xMove);
                    }
                }
            }
            else{
                if(Input.GetButton("Fire3") && dashCooldown == 0){  // dashing
                    dash();
                }
                else if (wantsJump && jumpCooldown == 0 && dbleJumps > 0) {  // double-jumping
                    doubleJump();
                }
                else if (xMove != 0){  // walking
                    walk(xMove, true);
                }
                else{
                    playerState = state.jumping;
                }
            }
        }
        
        if (Input.GetButton("Jump")){ // attacking
            //TODO: maybe add a attacking cooldown
            attack();
            isAttacking = true;
        }
        else{
            isAttacking = false;
        }
    }

#region movement
    void walk(float xMove, bool isAirborn = false) {
        facing = (xMove >= 0) ? 1 : -1;
        if(is_grounded) {
            _rigidbody2D.velocity = new Vector2(xMove*walkSpeed, _rigidbody2D.velocity.y);
            if (isAirborn == false){
                playerState = state.walking;
            }
            else{
                playerState = state.jumpWalking;
            }
        }
    }

    void jump() {
        if(is_grounded) {
            _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, jumpHeight);
            jumpCooldown += 0.3f;
            playerState = state.jumping;
        }
    }

    void dash(int face_override = 0) {
        //Note: we half our vertical velocity (better feel)
        if (face_override == 0) {
            _rigidbody2D.velocity = new Vector2(dashDist*facing, _rigidbody2D.velocity.y/2);
        }
        else{
            _rigidbody2D.velocity = new Vector2(dashDist*face_override, _rigidbody2D.velocity.y/2);
            facing = face_override;
        }
        
        if(is_grounded) {
            dashCooldown += 1.5f;
        }
        else {
            dashCooldown += 2f;
        }
        
        playerState = state.dashing;
    }

    void doubleJump() {
        if (_rigidbody2D.velocity.y > dbJumpHeight){
            _rigidbody2D.velocity += new Vector2(0, dbJumpHeight);
        }
        else{  // cancel current momentum if we aren't going up
            _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, dbJumpHeight);
        }
        dbleJumps -= 1;
        jumpCooldown += 0.6f;
        playerState = state.dbJumping;
    }

    void wallJump(int wallDir) {
        _rigidbody2D.velocity = new Vector2(-wallDir*walkSpeed/1.5f, dbJumpHeight*1.5f);
        wallJumps -= 1;
        jumpCooldown += 0.6f;
        facing *= -1; // switch facing directions
        // dbleJumps += 1;
        // dashCooldown = 0;
        playerState = state.wallJumping;
    }

    void wallSlide(){
        // TODO: implement user sliding down a wall
        _rigidbody2D.velocity = new Vector2(0, -0.3f);
        playerState = state.wallSliding;
    }
#endregion

    //attack works both in air and on ground, and with pogoing
    void attack() {

        //float speedDif = Data.targetSpeed - _rigidbody2D.velocity.x;
        //float movement = speedDif * accelRate;
        //_rigidbody2D.AddForce(movement * Vector2.right, ForceMode2D.Force); //knockback, will come back to fix
        //Instantiate actual atk that does damage, going sideways
        //playerState = state.attacking;
    }
}
