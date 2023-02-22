using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimController : MonoBehaviour
{
    [SerializeField] PlayerController _playerMovement;

    //[SerializeField] Test _test;

    private Animator _playerAC;

    // Start is called before the first frame update
    void Awake()
    {
        _playerAC = GetComponent<Animator>();
        _playerMovement = GetComponent<PlayerController>();
        //_test = GetComponent<Test>();
    }

    // Update is called once per frame
    void Update()
    {
        _playerAC.SetFloat("Speed", Mathf.Abs(_playerMovement._velocity.x));
    }
}
