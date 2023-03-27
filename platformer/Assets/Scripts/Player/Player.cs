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
    
        Debug.Log("Player State: " + playerState.ToString() + ", Jumps: "+ (dbleJumps, wallJumps).ToString()
                  + ", Attacking: " + isAttacking.ToString() + ", Grounded|Still?" + (is_grounded, is_still()).ToString()
                  + ", Dash CD: " + dashCooldown.ToString() + ", Jump CD: " + jumpCooldown.ToString() + ", Wall?: " + check_for_wall().ToString());
        
        movementControl();
    }

    private int check_for_wall(){
        // TODO: finds nearest wall and returns -1 if left of user, 0 if not close enough, and 1 if right of user
        if (wallOnRight){
            return 1;
        }
        else if (wallOnLeft) {
            return -1;
        }
        else{
            return 0;
        }
    }
    private bool is_still(){  // not moving much horizontally
        return _rigidbody2D.velocity.x <= 0.5f && _rigidbody2D.velocity.x >= -0.5f && _rigidbody2D.velocity.y <= 0.3f && _rigidbody2D.velocity.y >= -0.3f;
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
        float xMove = Input.GetAxis("Horizontal");

        if (is_grounded){  // on the ground
            dbleJumps = defNumDbJumps;
            wallJumps = defNumWallJumps;

            if (Input.GetButton("Fire3") && dashCooldown == 0) {  // dashing
                dash();
            }
            else if (yMove > 0 && jumpCooldown == 0) { // jumping
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
            if(Input.GetButton("Fire3") && dashCooldown == 0){  // dashing
                dash();
            }
            else if (yMove > 0 && jumpCooldown == 0) { // extra jumping
                int wallDir = check_for_wall();
                if (wallJumps > 0 && wallDir != 0){  // wall-jumping
                    wallJump(wallDir);
                }
                else if(dbleJumps > 0){   // double-jumping
                    doubleJump();
                }
                else{
                    playerState = state.jumping;
                }
            }
            else if (xMove != 0){  // walking
                walk(xMove);
            }
            else{
                playerState = state.jumping;
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
    void walk(float xMove) {
        facing = (xMove >= 0) ? 1 : -1;
        if(is_grounded) {
            _rigidbody2D.velocity = new Vector2(xMove*walkSpeed, _rigidbody2D.velocity.y);
            playerState = state.walking;
        }
    }

    void jump() {
        if(is_grounded) {
            _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, jumpHeight);
            jumpCooldown += 0.3f;
            playerState = state.jumping;
        }
    }

    void dash() {
        //Note: we half our vertical velocity (better feel)
        _rigidbody2D.velocity = new Vector2(dashDist*facing, _rigidbody2D.velocity.y/2);
        
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
        jumpCooldown += 0.5f;
        playerState = state.dbJumping;
    }

    void wallJump(int wallDir) {
        _rigidbody2D.velocity = new Vector2(wallDir*walkSpeed*2, jumpHeight);
        wallJumps -= 1;
        jumpCooldown += 0.6f;
        // dbleJumps += 1;
        playerState = state.wallJumping;
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
