using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Movement : MonoBehaviour
{
   
    public float _moveSpeed;
    int _direction = 1;
    Rigidbody2D _myBody;
    Animator _myAnim;
    AudioSource _myAudio;
    bool _stop;

    public LayerMask _groundLayer;
    public Transform _leftWallCheck, _rightWallCheck;
    public Transform _leftFloorCheck, _rightFloorCheck;

    // Start is called before the first frame update
    void Start()
    {
        _myBody = GetComponent<Rigidbody2D>();
        _myAnim = GetComponent<Animator>();
        _myAudio = GetComponent<AudioSource>();
    }


    
    // Update is called once per frame
    void Update()
    {
        WallCheck();
        FloorCheck();
    }

    private void FixedUpdate()
    {
        if (!_stop)
        {
            float _move = _direction * _moveSpeed * Time.fixedDeltaTime;
            Vector2 _moveVector = new Vector2(_move, _myBody.velocity.y);
            _myBody.velocity = _moveVector;
        }
    }

    void WallCheck()
    {
        Collider2D[] _collidersLeftWall = Physics2D.OverlapCircleAll(_leftWallCheck.position, 0.05f, _groundLayer);
        Collider2D[] _collidersRightWall = Physics2D.OverlapCircleAll(_rightWallCheck.position, 0.05f, _groundLayer);

        if (_collidersLeftWall.Length > 0 || _collidersRightWall.Length > 0)
        {
            _direction = _direction * -1;
        }

    }

    void FloorCheck()
    {
        Collider2D[] _collidersLeftFloor = Physics2D.OverlapCircleAll(_leftFloorCheck.position, 0.05f, _groundLayer);
        Collider2D[] _collidersRightFloor = Physics2D.OverlapCircleAll(_rightFloorCheck.position, 0.05f, _groundLayer);

        if (_collidersLeftFloor.Length == 0 || _collidersRightFloor.Length == 0)
        {
            _direction = _direction * -1;
        }
    }

    public void EnemyDie()
    {
        _stop = true;
        _myBody.velocity = Vector2.zero;
        _myBody.isKinematic = true;
        _myAnim.SetBool("_die", true);
        _myAudio.Play();
    }


}
