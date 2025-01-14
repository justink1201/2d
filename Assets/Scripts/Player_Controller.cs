using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float _jumpForce;
    GameController _gameController;
    public AudioSource _myAudio;
    Rigidbody2D _myBody;
    bool _stop;
    public Transform _feet;
    Player_Animation _playerAnimScript;
    public bool _grounded, _canJump, _hitGround;
    public float _coyoteTime;
    public float _analogueJumpTime;
    float _trackCoyoteTime;
    float _jumpInputTime;
    public float _groundMoveSpeed, _airMoveSpeed;
    public LayerMask _groundLayer;
    bool _invulnerable;
    float _defaultGravity;
    bool _startJumpTimer;
    public float _jumpTimer;
    float _currentJumpTimer;
    public AudioClip _audioJump, _audioDie, _audioWin;
    private BoxCollider2D BoxCollider;
    private float wallJumpCooldown;
    private float horizontalInput;

    // Start is called before the first frame update
    void Start()
    {
        _gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        _myBody = GetComponent<Rigidbody2D>();
        _playerAnimScript = GetComponent<Player_Animation>();
        BoxCollider = GetComponent<BoxCollider2D>();
        _trackCoyoteTime = _coyoteTime;
        _jumpInputTime = _analogueJumpTime;
        _defaultGravity = _myBody.gravityScale;
        _currentJumpTimer = _jumpTimer;
    }


    private void FixedUpdate()
    {
        if (!_stop)
        {
            float _moveSpeed = 0;

            //Change horizontal speed when in the air.
            if (_grounded)
            {
                _moveSpeed = _groundMoveSpeed * Input.GetAxisRaw("Horizontal");
            }
            else
            {
                _moveSpeed = _airMoveSpeed * Input.GetAxisRaw("Horizontal");
            }
            
            Vector2 _moveVector = new Vector2(_moveSpeed, 0);
            _myBody.AddForce(_moveVector);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");

        //flip player left and right
        if (horizontalInput > 0.01f)
            transform.localScale = Vector3.one;
        else if (horizontalInput < -0.01f)
            transform.localScale = new Vector3(-1, 1, 1);
        //JUMP.
        if (Input.GetKeyDown(KeyCode.Space) && _canJump && !_stop && isGrounded())
        {
            PerformJump(_jumpForce, 1);
            _myAudio.PlayOneShot(_audioJump);

        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            StopJump();
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded())
        {
            _myBody.velocity = new Vector2(_myBody.velocity.x, _jumpForce);
        }
        else if (onWall() && !isGrounded())
        {
            if (horizontalInput == 0)
            {
                _myBody.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 10, 0);
                transform.localScale = new Vector3(-Mathf.Sign(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else
            {

            }
            _myBody.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 3, 6);

            wallJumpCooldown = 1;
        }

        //Wall Jump Logic
        if (wallJumpCooldown < 0.2f)
        {


            _myBody.velocity = new Vector2(horizontalInput * _groundMoveSpeed, _myBody.velocity.y);

            if (onWall() && !isGrounded())
            {
                _myBody.velocity = new Vector2(0, _myBody.velocity.y);
                _myBody.gravityScale = 0;
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    PerformJump(_jumpForce, 1);
                    _myAudio.PlayOneShot(_audioJump);
                }
            }
            else
                _myBody.gravityScale = 3;
        }
        else
        {
            wallJumpCooldown += Time.deltaTime;
            _myBody.velocity = new Vector2(Mathf.Sign(transform.localScale.x) * 3, 6);
            wallJumpCooldown = 0f;
        }

        TimeVariableJump();
        CheckGround();
        UpdateCoyoteTime();
        EnemyCheck();
        print(onWall());
    }

    void PerformJump(float _jumpStrength, float _hangMultiplier)
    {
        _trackCoyoteTime = 0;
        _startJumpTimer = true;
        _currentJumpTimer = _jumpTimer * _hangMultiplier;
        _myBody.gravityScale = 0f;
        Vector2 _jumpVector = new Vector2(0, _jumpStrength);
        _myBody.AddForce(_jumpVector, ForceMode2D.Impulse);    
        _playerAnimScript.JumpStart();
    }

    void StopJump()
    {
        _myBody.gravityScale = _defaultGravity;
        _startJumpTimer = false;
        //_currentJumpTimer = _jumpTimer;
    }

    void TimeVariableJump()
    {
        if (_startJumpTimer)
        {
            _currentJumpTimer -= Time.deltaTime;
        }

        if (_currentJumpTimer <= 0)
        {
            StopJump();
        }
    }

    void CheckGround()
    {
        Collider2D[] _colliders = Physics2D.OverlapCircleAll(_feet.position, 0.05f, _groundLayer);

        //Check for first collision.
        if (_colliders.Length > 0 && !_grounded)
        {
            HitGround();
        }

        //Switch bool based on ground.
        if (_colliders.Length > 0)
        {
            _grounded = true;
        }
        else
        {
            _grounded = false;
        }
    }

    void EnemyCheck()
    {
        Collider2D[] _colliders = Physics2D.OverlapCircleAll(_feet.position, 0.4f);

        for (int i = 0; i < _colliders.Length; i++)
        {
            if (_colliders[i].gameObject.CompareTag("Enemy") && !_invulnerable)
            {     
                _invulnerable = true;
                PerformJump(_jumpForce * 0.8f, 0.5f);
                StartCoroutine(InvulnerableReset());

                Enemy_Movement _enemyScript = _colliders[i].GetComponent<Enemy_Movement>();
                _enemyScript.EnemyDie();
            }
        }
    }


    public void HitGround()
    {
        
        _trackCoyoteTime = _coyoteTime;
        _jumpInputTime = _analogueJumpTime;
        _playerAnimScript.JumpLand();
        StartCoroutine(GroundSlamCheck());
        StopJump();
    }

    //Bounces back up after hitting a block from above.
    public void ReboundBounce()
    {
        PerformJump(_jumpForce * 0.8f, 0.5f);
    }

    public void UpdateCoyoteTime()
    {
        //When the player leaves the ground, count 'coyote time'.
        if (!_grounded)
        {
            _trackCoyoteTime -= Time.deltaTime;
        }

        //Enable or disable jumping based on 'coyote time'.
        if (_trackCoyoteTime < 0)
        {
            _canJump = false;
        }
        else
        {
            _canJump = true;
        }
    }

    public void WinState()
    {
        _stop = true;
        _playerAnimScript.WinAnim();
        _gameController.WinScreen();
        _myAudio.PlayOneShot(_audioWin);
    }

    IEnumerator GroundSlamCheck()
    {
        _hitGround = true;
        yield return new WaitForSeconds(0.01f);
        _hitGround = false;
    }

    IEnumerator InvulnerableReset()
    {
        _invulnerable = true;
        yield return new WaitForSeconds(0.2f);
        _invulnerable = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Check for enemy collision.
        if (collision.gameObject.CompareTag("Enemy") && !_invulnerable)
        {
            _gameController.GameOver();
            _stop = true;
            _invulnerable = true;
            _playerAnimScript.DeathAnim();
            _myAudio.PlayOneShot(_audioDie);
        }

       
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Check for pickup collision.
        if (collision.gameObject.CompareTag("Pickup"))
        {
            //Load pickup script.
            Item_Pickup _pickupScript = collision.gameObject.GetComponent<Item_Pickup>();

            //Check if it's already been collected.
            if (!_pickupScript._collected)
            {
                _pickupScript.Pickup();
                _gameController._score++;
            }
        }

        //Check for goal.
        if (collision.gameObject.CompareTag("Goal"))
         {

            WinState();
        }

    }

    private bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(BoxCollider.bounds.center, BoxCollider.bounds.size, 0, Vector2.down, 0.1f, _groundLayer);
        return raycastHit.collider != null;
    }

    private bool onWall()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(BoxCollider.bounds.center, BoxCollider.bounds.size, 0, new Vector2(transform.localScale.x, 0), 0.1f, wallLayer);
        return raycastHit.collider != null;
    }

    
}

