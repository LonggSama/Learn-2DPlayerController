using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PlayerStat")]
public class PlayerData : ScriptableObject
{
    #region GRAVITY
    [HideInInspector] public float gravityStrenght;
    [HideInInspector] public float gravityScale;
    [Space(5)]
    public float fallGravityMult;
    public float maxFallSpeed;
    [Space(5)]
    public float fastFallGravityMult;
    public float maxFastFallSpeed;

    [Space(20)]

    #endregion

    #region RUN
    [Header("Run")]
    public float runMaxSpeed;
    public float runAcceleration;
    [HideInInspector] public float runAccelAmount;
    public float runDecceleration;
    [HideInInspector] public float runDecelAmount;
    [Space(5)]
    [Range(0f, 1f)] public float accelInAir;
    [Range(0f, 1f)] public float decelInAir;
    //public bool doConserveMomentum = true;

    [Space(20)]
    #endregion

    #region JUMP
    [Header("Jump")]
    public float jumpHeight;
    public float jumpTimeToApex;
    [HideInInspector] public float jumpForce;

    [Header("Both Jump")]
    public float jumpCutGravityMult;
    [Range(0f, 1f)] public float jumpHangGravityMult;
    public float jumpHangTimeThreshold;
    [Space(1)]
    public float jumpHangAccelerationMult;
    public float jumpHangMaxSpeedMult;

    [Space(20)]

    #endregion

    #region ASSISTS
    [Header("Assists")]
    [Range(0.01f, 0.5f)] public float coyoteTime;
    [Range(0.01f, 0.5f)] public float jumpInputBufferTime;
    #endregion

    private void OnValidate()
    {
        #region Gravity
        gravityStrenght = -(2 * jumpHeight) / (jumpTimeToApex * jumpTimeToApex);
        gravityScale = gravityStrenght / Physics2D.gravity.y;
        jumpForce = Mathf.Abs(gravityStrenght) * jumpTimeToApex;
        #endregion

        #region Acceleration
        runAccelAmount = ((1 / Time.fixedDeltaTime) * runAcceleration) / runMaxSpeed;
        runDecelAmount= ((1 / Time.fixedDeltaTime) * runDecceleration) / runMaxSpeed;
        #endregion

        #region Variable Ranges
        runAcceleration = Mathf.Clamp(runAcceleration, 0.01f, runMaxSpeed);
        runDecceleration = Mathf.Clamp(runDecceleration, 0.01f, runMaxSpeed);
        #endregion
    }
}
