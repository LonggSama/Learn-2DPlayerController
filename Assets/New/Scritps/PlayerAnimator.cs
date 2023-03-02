using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    #region COMPONENTS
    private Animator anim;
    private SpriteRenderer sprite;
    private PlayerMovement move;
    #endregion

    #region TRIGGERS
    public bool isLanded { private get; set; }
    public bool isDoubleJump { private get; set; }
    #endregion

    #region ANIMATION STATES
    string currentState;
    public const string PLAYER_IDLE = "idle";
    public const string PLAYER_RUN = "run";
    public const string PLAYER_JUMP = "jump";
    public const string PLAYER_FALL = "fall";
    public const string PLAYER_DOUBLEJUMP = "doubleJump";
    #endregion

    private void Awake()
    {
        move = GetComponent<PlayerMovement>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        anim = sprite.GetComponent<Animator>();
    }

    private void Update()
    {
        anim.SetFloat("yVelocity", move.PlayerRb.velocity.y);
        anim.SetFloat("Speed", Mathf.Abs(move.PlayerRb.velocity.x));
        anim.SetBool("IsLanded", isLanded);
        anim.SetBool("IsDoubleJump", isDoubleJump);
        CheckDoubleJump();
    }

    public void CheckDoubleJump()
    {
        if (isDoubleJump)
        {
            anim.Play("doubleJump", 0);
            isDoubleJump = false;
        }
    }

    public void CheckAnimationState(string newState)
    {
        if (newState == currentState)
        {
            return;
        }

        anim.Play(newState);

        currentState = newState;
    }

    public bool IsAnimationPlaying(Animator animator, string stateName)
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName(stateName) && animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            return true;
        }
        else
            return false;
    }
}
