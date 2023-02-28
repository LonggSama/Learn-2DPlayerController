using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    public PlayerData PlayerData;

    #region COMPONENTS
    public Rigidbody2D PlayerRb { get; private set; }
    public PlayerAnimator AnimHandler { get; private set; }
    #endregion

    #region VARIABLES
    public bool IsFacingRight { get; private set; }
    public bool IsJumping { get; private set; }
    #endregion

    #region STATE PARAMETERS
    //Timer
    public float LastOnGroundTime { get; private set; }

    //Jump
    private bool _isJumpCut;
    private bool _isJumpFalling;
    private int _bonusJumpLeft;
    private int _jumpCount;
    #endregion

    #region INPUT PARAMETERS
    public Vector2 _moveInput;
    public float LastPressJumpTime;
    #endregion

    #region CHECK PARAMETERS
    [Header("Checks")]
    [SerializeField] private Transform _groundCheckPoint;
    [SerializeField] private Vector2 _groundCheckSize = new Vector2(0.49f, 0.03f);
    #endregion

    #region LAYERS & TAGS
    [Header("Layers & Tags")]
    [SerializeField] private LayerMask _gorundLayer;
    #endregion

    private void Awake()
    {
        PlayerRb = GetComponent<Rigidbody2D>();
        AnimHandler = GetComponent<PlayerAnimator>();
    }

    private void Start()
    {
        IsFacingRight = true;
        _bonusJumpLeft = PlayerData.jumpBonus;
    }

    private void Update()
    {
        #region TIMERS
        LastOnGroundTime -= Time.deltaTime;

        LastPressJumpTime -= Time.deltaTime;
        #endregion

        #region INPUT HANDLER
        _moveInput.x = Input.GetAxisRaw("Horizontal");
        _moveInput.y = Input.GetAxisRaw("Vertical");

        if (_moveInput.x != 0)
        {
            CheckDirectionToFace(_moveInput.x > 0);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnJumpInput();
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            OnJumpUpInput();
        }
        #endregion

        #region COLLISION CHECKS
        if (!IsJumping)
        {
            //Ground Check
            if (Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _gorundLayer) && !IsJumping)
            {
                LastOnGroundTime = PlayerData.coyoteTime;
                _bonusJumpLeft = PlayerData.jumpBonus;
                _jumpCount = 0;
            }
        }
        #endregion

        #region JUMP CHECKS
        if (IsJumping && PlayerRb.velocity.y < 0)
        {
            IsJumping = false;

            _isJumpFalling = true;            
        }

        if (LastOnGroundTime > 0 && !IsJumping)
        {
            _isJumpCut = false;

            _isJumpFalling = false;
        }

        //Jump
        if (CanJump() && LastPressJumpTime > 0)
        {
            IsJumping = true;
            _isJumpCut = false;
            _isJumpFalling = false;

            Jump();
        }

        //Double Jump
        else if (LastPressJumpTime > 0 && _bonusJumpLeft > 0)
        {
            IsJumping = true;
            _isJumpCut = false;
            _isJumpFalling = false;

            _bonusJumpLeft--;
            
            Jump();
        }
        #endregion

        #region GRAVITY
        //Higher gravity if we've released the jump input or are falling
        if (PlayerRb.velocity.y < 0 && _moveInput.y < 0)
        {
            //Much higher gravity if holding down
            SetGravityScale(PlayerData.gravityScale * PlayerData.fastFallGravityMult);
            //Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
            PlayerRb.velocity = new Vector2(PlayerRb.velocity.x, Mathf.Max(PlayerRb.velocity.y, -PlayerData.maxFastFallSpeed));
        }
        else if (_isJumpCut)
        {
            //Higher gravity if jump button released
            SetGravityScale(PlayerData.gravityScale * PlayerData.jumpCutGravityMult);
            PlayerRb.velocity = new Vector2(PlayerRb.velocity.x, Mathf.Max(PlayerRb.velocity.y, -PlayerData.maxFallSpeed));
        }
        else if ((IsJumping || _isJumpFalling) && Mathf.Abs(PlayerRb.velocity.y) < PlayerData.jumpHangTimeThreshold)
        {
            SetGravityScale(PlayerData.gravityScale * PlayerData.jumpHangGravityMult);
        }
        else if (PlayerRb.velocity.y < 0)
        {
            //Higher gravity if falling
            SetGravityScale(PlayerData.gravityScale * PlayerData.fallGravityMult);
            //Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
            PlayerRb.velocity = new Vector2(PlayerRb.velocity.x, Mathf.Max(PlayerRb.velocity.y, -PlayerData.maxFallSpeed));
        }
        else
        {
            //Default gravity if standing on a platform or moving upwards
            SetGravityScale(PlayerData.gravityScale);
        }
        #endregion

        #region ANIMATION CALL
        if (LastOnGroundTime == PlayerData.coyoteTime)
            AnimHandler.isLanded = true;
        else
            AnimHandler.isLanded = false;

        if (_jumpCount > 1)
            AnimHandler.isDoubleJump = true;
        else
            AnimHandler.isDoubleJump = false;
        #endregion
    }

    private void FixedUpdate()
    {
        //Handle Run
        Run(1);
    }

    #region INPUT CALLBACKS
    //Methods which handle input detected in Update()
    public void OnJumpInput()
    {
        LastPressJumpTime = PlayerData.jumpInputBufferTime;
    }

    public void OnJumpUpInput()
    {
        if (CanJumpCut())
        {
            _isJumpCut = true;
        }
    }
    #endregion

    #region GENERAL METHODS
    public void SetGravityScale(float scale)
    {
        PlayerRb.gravityScale = scale;
    }
    #endregion

    //MOVEMENT METHODS
    #region RUN METHODS
    private void Run(float lerpAmount)
    {
        float targetSpeed = _moveInput.x * PlayerData.runMaxSpeed;
        targetSpeed = Mathf.Lerp(PlayerRb.velocity.x, targetSpeed, lerpAmount);

        #region Calculate AccelRate
        float accelRate;
        if (LastOnGroundTime > 0)
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? PlayerData.runAccelAmount : PlayerData.runDecelAmount;
        else
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? PlayerData.runAccelAmount * PlayerData.accelInAir : PlayerData.runDecelAmount * PlayerData.decelInAir;
        #endregion

        #region Add Bonus Jump Apex Acceleration
        if ((IsJumping || _isJumpFalling) && Mathf.Abs(PlayerRb.velocity.y) < PlayerData.jumpHangTimeThreshold)
        {
            accelRate *= PlayerData.jumpHangAccelerationMult;
            targetSpeed *= PlayerData.jumpHangMaxSpeedMult;
        }
        #endregion

        #region Perform Run
        //Calculate difference between current velocity and desired velocity
        float speedDif = targetSpeed - PlayerRb.velocity.x;
        //Calculate force along x-axis to apply to the player
        float movement = speedDif * accelRate;
        //Convert this to a vector and apply to rigidbody
        PlayerRb.AddForce(movement * Vector2.right, ForceMode2D.Force);

        //AnimHandler.SetSpeed(Mathf.Abs(_moveInput.x));

        /*
		 * AddForce() will do
		 * RB.velocity = new Vector2(RB.velocity.x + (Time.fixedDeltaTime  * speedDif * accelRate) / RB.mass, RB.velocity.y);
		 * Time.fixedDeltaTime is by default in Unity 0.02 seconds equal to 50 FixedUpdate() calls per second
		*/

        #endregion
    }

    private void Turn()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

        IsFacingRight = !IsFacingRight;
    }
    #endregion

    #region JUMP METHODS
    private void Jump()
    {
        LastOnGroundTime = 0;
        LastPressJumpTime = 0;

        #region Perform Jump
        //We increase the force applied if we are falling
        //This means we'll always feel like we jump the same amount
        float force = PlayerData.jumpForce;

        if (PlayerRb.velocity.y < 0)
        {
            force -= PlayerRb.velocity.y;
        }

        _jumpCount++;

        PlayerRb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
        #endregion
    }
    #endregion

    #region CHECK METHODS
    public void CheckDirectionToFace(bool isMovingRight)
    {
        if (isMovingRight != IsFacingRight)
        {
            Turn();
        }
    }

    private bool CanJump()
    {
        return LastOnGroundTime > 0 && !IsJumping;
    }

    private bool CanJumpCut()
    {
        return IsJumping && PlayerRb.velocity.y > 0;
    }
    #endregion

    #region EDITOR METHODS
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(_groundCheckPoint.position, _groundCheckSize);
    }
    #endregion
}
