using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Animation : MonoBehaviour
{
    public Animator _rollAnim, _squishAnim;
    Player_Controller _playerScript;
    Rigidbody2D _myBody;

    // Start is called before the first frame update
    void Start()
    {
        _myBody = GetComponent<Rigidbody2D>();
        _playerScript = GetComponent<Player_Controller>();
    }

    // Update is called once per frame
    void Update()
    {
        //Set grounded based on player state.
        _rollAnim.SetBool("_grounded", _playerScript._grounded);
        _squishAnim.SetBool("_grounded", _playerScript._grounded);

        //Track horizontal speed.
        float _moveSpeed = _myBody.velocity.x;
        _rollAnim.SetFloat("_moveSpeed", -_moveSpeed/4);

        //Check for 'close to 0' horizontal movement.
        if (_moveSpeed < -0.01 || _moveSpeed > 0.01)
        {
            _rollAnim.SetBool("_moving", true);
        }
        else
        {
            _rollAnim.SetBool("_moving", false);
        }
    }

    public void JumpStart()
    {
        _squishAnim.SetBool("_jump", true);
    }

    public void JumpLand()
    {
        _squishAnim.SetBool("_jump", false);
    }

    public void DeathAnim()
    {
        _squishAnim.SetBool("_dead", true);
    }

    public void WinAnim()
    {
        _squishAnim.SetBool("_win", true);
    }
}
