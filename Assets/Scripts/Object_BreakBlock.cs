using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object_BreakBlock : MonoBehaviour
{
    Animator _myAnim;
    public AudioClip _audioBreaking;
    int _break;
    public int _breakThreshold;

    private void Start()
    {
        //Load the Animator.
        _myAnim = GetComponent<Animator>();
        
    }

    void BreakBlock()
    {
        //Increase break amount.
        _break++;
        _myAnim.SetInteger("_break", _break);
        

        //If block is fully broken, remove block.
        if (_break >= _breakThreshold)
        {
            this.gameObject.SetActive(false);
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Check for player.
        if (collision.gameObject.CompareTag("Player"))
        {
            //Load player script.
            Player_Controller _playerScript = collision.gameObject.GetComponent<Player_Controller>();

            //Check for head hit.
            if (!_playerScript._grounded)
            {
                BreakBlock();
                _playerScript._myAudio.PlayOneShot(_audioBreaking);
            }
            

            //Check for ground.
            if (_playerScript._hitGround)
            {
                BreakBlock();
                _playerScript._myAudio.PlayOneShot(_audioBreaking);
            }
        }
    }

}
