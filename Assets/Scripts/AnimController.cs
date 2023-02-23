using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimController : MonoBehaviour
{
    //[SerializeField] PlayerController _playerMovement;

    public RigiRun RigiRun;

    private Animator _playerAC;

    // Start is called before the first frame update
    void Awake()
    {
        _playerAC = GetComponent<Animator>();
        //_playerMovement = GetComponent<PlayerController>();
        RigiRun = GetComponent<RigiRun>();
    }

    // Update is called once per frame
    void Update()
    {
        _playerAC.SetFloat("Speed", Mathf.Abs(RigiRun.RB.velocity.x));
    }
}
