using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    #region COMPONENTS
    private Animator anim;
    private SpriteRenderer sprite;
    #endregion

    private void Awake()
    {
        sprite = GetComponentInChildren<SpriteRenderer>();
        anim = sprite.GetComponent<Animator>();
    }

    public void SetSpeed(float speed)
    {
        anim.SetFloat("Speed", speed);
    }

    public void SetIsJumping(bool isJumping)
    {
        anim.SetBool("IsJumping", isJumping);
    }

    public void SetIsGrounded(bool isGrounded)
    {
        anim.SetBool("IsGrounded", isGrounded);
    }

    public void SetVelocity(float velocity)
    {
        anim.SetFloat("Velocity", velocity);
    }

    public void SetAnotherJump(int jumpCount)
    {
        anim.SetInteger("JumpCount", jumpCount);
    }
}
