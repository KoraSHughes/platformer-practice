using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class testPlayer : MonoBehaviour
{
/* #region variables
    public PlayerData _playerData;
    GameManager _gameManager;
    Rigidbody2D _rigidbody2D;

    private Vector2 playerPosition;
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
    
//     state playerState = state.idle;

//     public GameObject feet;  // bottom of player, used to check if we are on the ground
//     public LayerMask whatIsGround;
//     GameObject[] walls;

//     public int defNumDbJumps = 1;
//     public int defNumWallJumps = 1;
//     public int defNumDashes = 1;

//     float jumpCooldown = 0f;
//     float dashCooldown = 0f;

//     float walkSpeed = 4f;
//     float dashDist = 7f;
//     float jumpHeight = 4f;
//     float dbJumpHeight = 3f;

//     bool isAttacking = false;
//     int dbleJumps;
//     int wallJumps;
    
//     bool is_grounded = true;
//     int facing;
// #endregion

//     void Start()
//     {
//         _gameManager = GameObject.FindObjectOfType<GameManager>();
//         _rigidbody2D = GetComponent<Rigidbody2D>();
//         dbleJumps = defNumDbJumps;
//         wallJumps = defNumWallJumps;
//         playerPosition = GameObject.FindWithTag("Player").transform.position;
//         walls = GameObject.FindGameObjectsWithTag("wall");
//     }

//     void FixedUpdate()
//     {
//         float xSpeed = Input.GetAxis("Horizontal") * walkSpeed;
//         float xScale = transform.localScale.x;
//         if((xSpeed < 0 && xScale > 0) || (xSpeed > 0 && xScale < 1))
//             transform.localScale *= new Vector2(-1, 1);
//     }
    
//     void Update()
//     {
//         is_grounded = Physics2D.OverlapCircle(feet.transform.position, .1f, whatIsGround);
        
//     #region cooldowns
//         if (dashCooldown > 0){
//             dashCooldown -= Time.deltaTime;
//         }
//         else{
//             dashCooldown = 0f;  // safety in case time subtracts more than necessary
//         }

//         if (jumpCooldown > 0){
//             jumpCooldown -= Time.deltaTime;
//         }
//         else{
//             jumpCooldown = 0;
//         }

//         _rigidbody2D.angularVelocity = 0f; // TODO: make sure obj doesnt rotate
//     #endregion

//         // Debug.Log("Player State: " + playerState.ToString()
//         //           + ", Facing: " + facing.ToString()
//         //           + ", Jumps: "+ (dbleJumps, wallJumps).ToString()
//         //         + ", Attacking: " + isAttacking.ToString()
//         //         // + ", Grounded|Still?" + (is_grounded, is_still()).ToString()
//         //           + ", Dash CD: " + dashCooldown.ToString()
//         //           + ", Jump CD: " + jumpCooldown.ToString()
//         //           + ", WallDir?: " + check_for_wall().ToString());

//         movementControl();

//         if (transform.position.y < -10) {
//             transform.position = playerPosition;
//         }

//         Debug.Log("Player State: " + playerState.ToString() + "\n");
//     }

//     private void OnTriggerEnter2D(Collider2D other) {
//         // if player collides with reward, destroy reward
//         if (other.CompareTag("reward")) {
//             // print("reward");
//             Debug.Log("REWARD");
//             _gameManager.GetComponent<GameManager>().RewardInc();
//             Destroy(other.gameObject);   
//         }
//         else if (other.CompareTag("door")) {
//             if (PublicVars.nextLevel) {
//                 SceneManager.LoadScene("Level2 Dash");
//             }
//         }
//         else if (other.CompareTag("killzone")) {
//             print("killzone");
//             transform.position = playerPosition;
//         }
//     }

//     private int check_for_wall(){
//         // finds nearest wall and returns -1 if left of user, 0 if not close enough, and 1 if right of user
//         if (walls.Length == 0){
//             return 0;
//         }
//         else{
//             GameObject closest = walls[0];   // find closest wall
//             float shortest_dist = Vector2.Distance(transform.position, closest.transform.position);
//             foreach (GameObject temp_wall in walls){
//                 float dist = Vector2.Distance(transform.position, temp_wall.transform.position);
//                 if (dist < shortest_dist){
//                     closest = temp_wall;
//                     shortest_dist = dist;
//                 }
//             }
//             return closest.GetComponent<DetectPlayer>().wallToPlayer();  // return that wall's detection of player
//         }
//     }

//     private bool is_still(){  // not moving much
//         // Note: vertical stillness must be < descent speed on wall sliding
//         return _rigidbody2D.velocity.x <= 0.5f && _rigidbody2D.velocity.x >= -0.5f && _rigidbody2D.velocity.y <= 0.2f && _rigidbody2D.velocity.y >= -0.2f;
//     }
    


//     public state get_state(){
//         return playerState;
//     }

//     public (int, int) addJumps(int doubles, int walls){
//         // TODO: add power-ups that change this value?
//         dbleJumps += doubles;
//         wallJumps += walls;
//         return (dbleJumps, wallJumps);
//     }

//     void movementControl() {
//         bool wantsJump = Input.GetKey(KeyCode.Space) || Input.GetButton("XJump"); // jumping = space bar | Xbox A (button0)
//         float xMove = Input.GetAxis("Horizontal");

//         if (is_grounded){  // on the ground
//             dbleJumps = defNumDbJumps;
//             wallJumps = defNumWallJumps;

//             if (Input.GetButton("Fire3") && dashCooldown == 0) {  // dashing = Shift | Xbox B (button1)
//                 dash();
//             }
//             else if (wantsJump && jumpCooldown == 0) { // jumping
//                 jump();
//             }
//             else if (xMove != 0){  // walking
//                 walk(xMove, false);
//             }
//             else{
//                 if (is_still()){
//                     playerState = state.idle; // player idle
//                 }
//                 else{
//                     playerState = state.walking;
//                 }
//             }
//         }
//         else {  // mid-air
//             int wallDir = check_for_wall();
//             int moveDir = (xMove > 0) ? 1 : -1;
//             if (wallDir != 0 && _rigidbody2D.velocity.y <= 0
//                 && playerState != state.wallJumping && (xMove == 0 || moveDir == wallDir)){
//                 wallSlide( (xMove == 0) ? false : true );
//             }

//             if (playerState == state.wallSliding){  // currently sliding on a wall
//                 if(Input.GetButton("Fire3") && dashCooldown == 0){  // dashing
//                     dash(-wallDir);  // can only dash opposite to wall (-facing)?
//                 }
//                 else if (wantsJump && jumpCooldown == 0 && wallJumps > 0) { // wall-jump
//                     wallJump(wallDir);
//                 }
//                 else{
//                     // Note: can only move opposite to wall when wall-sliding, else slide persists
//                     if (moveDir != wallDir){
//                         walk(xMove, true);
//                     }
//                 }
//             }
//             else{
//                 if(Input.GetButton("Dash") && dashCooldown == 0){  // dashing = Shift | Xbox B (button1)
//                     dash();
//                 }
//                 else if (wantsJump && jumpCooldown == 0 && dbleJumps > 0) {  // double-jumping
//                     doubleJump();
//                 }
//                 else if (xMove != 0){  // walking
//                     walk(xMove, true);
//                 }
//                 else{
//                     playerState = state.jumping;
//                 }
//             }
//         }
        
//         if (Input.GetKey(KeyCode.Mouse0) || Input.GetButton("XAtk")){ // attacking = LMB | Xbox X (button2)
//             //TODO: maybe add a attacking cooldown
//             attack();
//             isAttacking = true;
//         }
//         else{
//             isAttacking = false;
//         }

//         if (Input.GetKey(KeyCode.Tab) ){  // pause menu = Tab | Xbox start (button7)
//             //TODO: add Input.GetButtonDown("joystick button 7")
//             //TODO: add pause menu
//         }
//     }

// #region movement
//     void walk(float lerpAmount) {
//         playerState = state.walking;
//         facing = (xMove >= 0) ? 1 : -1;
//         float targetSpeed = _moveInput.x * Data.walkMaxSpeed;

//         targetSpeed = Mathf.Lerp(RB.velocity.x, targetSpeed, lerpAmount);

        
// 		float accelRate;

// 		if (LastOnGroundTime > 0)
// 			accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ?
//                 Data.runAccelAmount : Data.runDeccelAmount;
// 		else
// 			accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ?
//                 Data.runAccelAmount * Data.accelInAir
//                 : Data.runDeccelAmount * Data.deccelInAir;

//         if (isAirborn == true){
//             float addV = (_rigidbody2D.velocity.x*facing < walkSpeed) ? xMove*walkSpeed*0.007f : 0f;
//             // Note: we use dashDist here bc thats the fastest we want the user to be able to go in 
//             _rigidbody2D.velocity += new Vector2(addV, 0);
//         }
//         else{
//             _rigidbody2D.velocity = new Vector2(xMove*walkSpeed/2, _rigidbody2D.velocity.y);
//             // Note: we move slower horizontally in the air
            
//         }
//     }

//     void jump() {
//         if(is_grounded) {
//             playerState = state.jumping;
//             _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, jumpHeight);
//             jumpCooldown += 0.3f;
//         }
//     }

//     void dash(int face_override = 0) {
//         //Note: we half our vertical velocity (better feel)
//         if (face_override == 0) {
//             _rigidbody2D.velocity = new Vector2(dashDist*facing, _rigidbody2D.velocity.y/4);
//         }
//         else{
//             _rigidbody2D.velocity = new Vector2(dashDist*face_override, _rigidbody2D.velocity.y/4);
//             facing = face_override;
//         }
        
//         if(is_grounded) {
//             dashCooldown += 1.5f;
//         }
//         else {
//             dashCooldown += 2f;
//         }
        
        playerState = state.dashing;
    }

    void doubleJump() {
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
        playerState = state.wallJumping;
        _rigidbody2D.velocity = new Vector2(-wallDir*walkSpeed/1.5f, dbJumpHeight*1.5f);
        wallJumps -= 1;
        jumpCooldown += 0.6f;
        facing *= -1; // switch facing directions
        // dbleJumps += 1;
        // dashCooldown = 0;
    }

    void wallSlide(bool slowly){
        // TODO: implement user sliding down a wall
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

        //float speedDif = Data.targetSpeed - _rigidbody2D.velocity.x;
        //float movement = speedDif * accelRate;
        //_rigidbody2D.AddForce(movement * Vector2.right, ForceMode2D.Force); //knockback, will come back to fix
        //Instantiate actual atk that does damage, going sideways
        //playerState = state.attacking;
    } */
}

