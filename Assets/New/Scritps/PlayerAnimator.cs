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
        if (Mathf.Abs(move.PlayerRb.velocity.x) > 0.1 && move.LastOnGroundTime > 0 && !IsAnimationPlaying(anim, PLAYER_DOUBLEJUMP))
            CheckAnimationState(PLAYER_RUN);

        else if (Mathf.Abs(move.PlayerRb.velocity.x) < 0.1 && move.LastOnGroundTime > 0 && !IsAnimationPlaying(anim, PLAYER_DOUBLEJUMP))
            CheckAnimationState(PLAYER_IDLE);

        else if (move.PlayerRb.velocity.y > 0 && !IsAnimationPlaying(anim, PLAYER_DOUBLEJUMP) && move.LastOnGroundTime < 0)
            CheckAnimationState(PLAYER_JUMP);

        else if (move.PlayerRb.velocity.y < 0 && !IsAnimationPlaying(anim, PLAYER_DOUBLEJUMP) && move.LastOnGroundTime < 0)
            CheckAnimationState(PLAYER_FALL);

        if (move.IsDoubleJump)
        {
            CheckAnimationState(PLAYER_DOUBLEJUMP);
            move.IsDoubleJump = false;
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
