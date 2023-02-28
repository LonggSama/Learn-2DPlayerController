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
    }
}
