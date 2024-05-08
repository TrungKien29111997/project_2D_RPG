using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    CapsuleCollider2D _capsuleCollider2D;
    PlayerInputManager _input;
    Rigidbody2D _rigidbody2D;

    [Header("StatusChecking")]
    [SerializeField] bool isGround;
    [SerializeField] bool isAttack;

    [Header("StatesChecking")]
    [SerializeField] bool inOnGroundState;
    [SerializeField] bool inSwimmingState;
    [SerializeField] bool inDivingState;
    [SerializeField] bool inClimbingState;
    [SerializeField] bool inFlyingState;
    [SerializeField] bool inESkillState;
    [SerializeField] bool inQSkillState;

    [Header("GeneralSetting")]
    [SerializeField] GameObject _mainCamera;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float speed;
    [SerializeField] float jumpForce;
    [SerializeField] GameObject kunaiPrefab;
    [SerializeField] Transform kunaiSpawn;
    [SerializeField] GameObject attackPrefab;
    Vector3 savePoint;
    [HideInInspector] public int coin;
    RaycastHit2D hit;

    IStatePlayer currentState;

    // animation IDs
    int animIDIdle;
    int animIDRun;
    int animIDJump;
    int animIDInAir;
    int animIDFly;
    int animIDAttack;
    int animIDShoot;
    int animIDDeath;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        _input = GetComponent<PlayerInputManager>();
        _mainCamera = FindObjectOfType<CameraFollow>().gameObject;
        AssignAnimationIDs();
        SavePoint();
        coin = 0;
        UIManager.instance.SetCoin(coin);
    }

    void FixedUpdate()
    {
        if (isDead)
        {
            OnDead();
            _rigidbody2D.velocity = Vector2.zero;
            return;
        }

        if (currentState != null)
        {
            currentState.OnExecute(this);
        }
    }

    public override void OnInit()
    {
        base.OnInit();
        isAttack = false;
        transform.position = savePoint;
        ChangeAnim(animIDIdle);
        ChangeState(new OnGroundStatePlayer());
        attackPrefab.SetActive(false);
    }

    public override void OnDespawn()
    {
        OnInit();
        base.OnDespawn();
    }

    protected override void OnDead()
    {
        base.OnDead();
        ChangeAnim(animIDDeath);
    }

    public void StartOnGroundState()
    {
        inOnGroundState = true;
    }
    public void UpdateOnGroundState()
    {
        isGround = GroundCheck();

        if (isAttack)
        {
            _rigidbody2D.velocity = Vector2.zero;
            return;
        }

        if (_input.shoot)
        {
            Shoot();
        }

        if (isGround)
        {
            if (_input.jump)
            {
                ChangeAnim(animIDJump);
                _rigidbody2D.velocity = new Vector2(0, jumpForce);
            }
            if (_input.move.x != 0)
            {
                ChangeAnim(animIDRun);
                Move();
            }
            else
            {
                ChangeAnim(animIDIdle);
                _rigidbody2D.velocity = new Vector2(0, _rigidbody2D.velocity.y);
            }

            if (_input.attack)
            {
                Attack();
            }
        }
        else
        {
            ChangeAnim(animIDInAir);
            if (_input.move.x != 0)
            {
                Move();
            }
        }
    }
    public void ExitOnGroundState()
    {
        inOnGroundState = false;
    }
    public void StartSwimmingState()
    {
        inSwimmingState = true;
        _rigidbody2D.gravityScale = 0;
        _rigidbody2D.velocity = Vector2.zero;
    }
    public void UpdateSwimmingState()
    {

    }
    public void ExitSwimmingState()
    {
        inSwimmingState = false;
        _rigidbody2D.gravityScale = 1;
    }

    public void ChangeState(IStatePlayer newState)
    {
        if (currentState != null)
        {
            currentState.OnExit(this);
        }
        currentState = newState;

        if (currentState != null)
        {
            currentState.OnEnter(this);
        }
    }

    void Move()
    {
        _rigidbody2D.velocity = new Vector2(_input.move.x * Time.fixedDeltaTime * 100f * speed , _rigidbody2D.velocity.y);
        transform.localRotation = Quaternion.Euler(0, _input.move.x > 0 ? 0 : 180, 0);
    }

    void Attack()
    {
        isAttack = true;
        ChangeAnim(animIDAttack);
        attackPrefab.SetActive(true);
        Invoke(nameof(ResetAttack), 0.6f);
    }

    void ResetAttack()
    {
        isAttack = false;
        attackPrefab.SetActive(false);
        ChangeAnim(animIDIdle);
    }

    void Shoot()
    {
        isAttack = true;
        ChangeAnim(animIDShoot);
        Invoke(nameof(ResetAttack), 0.6f);
        Instantiate(kunaiPrefab, kunaiSpawn.position, transform.rotation);
    }
    void SavePoint()
    {
        savePoint = transform.position;
    }
    bool GroundCheck()
    {
        hit = Physics2D.Raycast(transform.position, Vector2.down, 1.1f, groundLayer);
        return hit.collider != null;
    }
    void AssignAnimationIDs()
    {
        animIDIdle = Animator.StringToHash("Idle");
        animIDRun = Animator.StringToHash("Run");
        animIDJump = Animator.StringToHash("StartJump");
        animIDFly = Animator.StringToHash("Fly");
        animIDInAir = Animator.StringToHash("InAir");
        animIDAttack = Animator.StringToHash("Attack");
        animIDShoot = Animator.StringToHash("Shoot");
        animIDDeath = Animator.StringToHash("Death");
        currentAnimID = animIDIdle;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Coin")
        {
            coin++;
            UIManager.instance.SetCoin(coin);
            Destroy(collision.gameObject);
        }
        if (collision.tag == "DeathZone")
        {
            hp = 0;
            ChangeAnim(animIDDeath);
            Invoke(nameof(OnInit), 1f);
        }
        if (collision.tag == "SavePoint")
        {
            savePoint = collision.transform.position;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Elevator")
        {
            transform.SetParent(collision.transform);
            _mainCamera.transform.SetParent(collision.transform);
        }
    }
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Elevator")
        {
            transform.SetParent(null);
            _mainCamera.transform.SetParent(null);
        }
    }
}
