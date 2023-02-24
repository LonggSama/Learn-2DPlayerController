using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class RigiRun : MonoBehaviour
{
    public RigiRunData Data;

    #region COMPONENTS
    public Rigidbody2D RB { get; private set; }
    #endregion

    #region STAGE PARAMETERS
    public bool IsFacingRight { get; private set; }
    public float LastOnGroundTime { get; private set; }
    #endregion

    #region INPUT PARAMATERS
    private Vector2 _moveInput;
    #endregion

    #region CHECK PARAMATERS
    [Header("Checks")]
    [SerializeField] private Transform _groundCheckPoint;
    [SerializeField] private Vector2 _groundCheckSize = new Vector2(0.49f, 0.03f);
    #endregion

    #region LAYERS & TAGS
    [Header("Layers & Tags")]
    [SerializeField] private LayerMask _groundLayer;
    #endregion

    private void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        IsFacingRight = true;
    }

    private void Update()
    {
        #region TIMERS
        LastOnGroundTime = Time.deltaTime;
        #endregion

        #region INPUT HANDLER
        _moveInput.x = Input.GetAxisRaw("Horizontal");

        if (_moveInput.x != 0)
        {
            CheckDirectionToFace(_moveInput.x > 0);
        }
        #endregion

        #region COLLISION CHECKS
        //Ground check
        if (Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _groundLayer)) //checks if set box overlaps with ground
        {
            LastOnGroundTime = 0.1f;
        }
        #endregion
    }

    private void FixedUpdate()
    {
        Run();
    }

    //MOVEMENT METHODS
    #region RUN METHODS
    private void Run()
    {
        //Caculate the direction we want to move in and our desired velocity
        float targetSpeed = _moveInput.x * Data.runMaxSpeed;

        #region Calculate AccelRate
        float accelRate;

        //Gets an acceleration value based on if we are accelerating (includes turning)
        //or trying to decelerate (stop). As well as applying a multiplier if we're air borns
        if (LastOnGroundTime > 0)
        {
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount : Data.runDecelAmount;
            Debug.Log("AccelRate: " + accelRate);
        }
        else
        {
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount * Data.accelInAir : Data.runDecelAmount * Data.decelInAir;
        }
        #endregion

        #region Conserve Momentum
        if (Data.doConserveMomentum && Mathf.Abs(RB.velocity.x) > Mathf.Abs(targetSpeed) && Mathf.Sign(RB.velocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f && LastOnGroundTime < 0)
        {
            //Prevent any deceleration from happening or in other words conserve are current momentum
            accelRate = 0;
        }
        #endregion

        //Calculate difference between current velocity and desired velocity
        float speedDif = targetSpeed - RB.velocity.x;
        //Calculate force along x-axis to apply to the player
        float movement = speedDif * accelRate;
        //Convert this to a vector and apply to rigidbody
        RB.AddForce(movement * Vector2.right, ForceMode2D.Force);
    }

    private void Turn()
    {
        //stores scale and flips the player along the x-axis
        Vector2 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

        IsFacingRight = !IsFacingRight;
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
    #endregion
}
