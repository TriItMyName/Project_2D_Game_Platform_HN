using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [Header("Enemy Settings:")]
    [SerializeField] public float health;
    [SerializeField] public float maxHealth;
    [SerializeField] protected float recoilLength;
    [SerializeField] protected float recoilFactor;
    [SerializeField] protected bool isRecoiling = false;
    [SerializeField] public float speed;
    [SerializeField] protected GameObject Blood;
    [SerializeField] public float damage;
    [SerializeField] protected AudioClip hurtSound,Hitsound, deathSound;
    public float detectDistance;
    public float destroyDelay;
    public int scoreValue;
    //References
    protected float recoilTimer;
    [HideInInspector] public Rigidbody2D enemyRb;
    protected Transform _playerTransform;
    protected Transform _transform;
    [HideInInspector] public Animator _animator;
    protected SpriteRenderer _spriteRenderer;
    protected float _playerEnemyDistance;
    protected AudioSource audioSource;

    public delegate void OnHealthChanged();
    public OnHealthChanged onHealthChangedCallback;
    protected enum EnemyStates
    {
        //Patrol
        Patrol_Idle,
        Patrol_Flip,
        Patrol_Chase,

        //FlyingInsect
        Insect_Idle,
        Insect_Chase,
        Insect_Stunned,
        Insect_Death,

        //Gunner
        Gunner_Idle,
        Gunner_Attack,
        Gunner_Death,

        //Charger
        Charger_Idle,
        Charger_Detect,
        Charger_Charge,
        Charger_Hurt,
        Charger_Death,

        //Shade
        Shade_Idle,
        Shade_Chase,
        Shade_Stunned,
        Shade_Death,

        //FinalBoss
        FinalBoss_Stage1,
        FinalBoss_Stage2,
        FinalBoss_Stage3,
        FinalBoss_Stage4
    }
    protected EnemyStates currentEnemyState;

    protected virtual EnemyStates GetCurrentEnemyState
    {
        get { return currentEnemyState; }
        set
        {
            if (currentEnemyState != value)
            {
                currentEnemyState = value;
                ChangeCurrentAnimation();
            }
        }
    }

    protected virtual void Awake()
    {
        enemyRb = GetComponent<Rigidbody2D>();
        _transform = transform;
        _animator = GetComponentInChildren<Animator>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        if (_spriteRenderer == null)
            Debug.LogError("SpriteRenderer missing on " + gameObject.name);

        if (enemyRb == null)
            Debug.LogError("Rigidbody2D missing on " + gameObject.name);

        if (_animator == null)
            Debug.LogError("Animator missing on " + gameObject.name);
    }
    protected virtual void Start()
    {
        if (GlobalController.instance != null && GlobalController.instance.player != null)
        {
            _playerTransform = GlobalController.instance.player.transform;
        }
        else
        {
            _playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        }
        health = maxHealth;
    }
    public virtual void ResetStats()
    {
        health = maxHealth;
        isRecoiling = false;
        gameObject.layer = LayerMask.NameToLayer("Attackable");
        if (_spriteRenderer != null)
            _spriteRenderer.color = new Color(1, 1, 1, 1);
    }

    protected virtual void Update()
    {
        if (PauseMenuUI.Instance.GameIsPaused) { return; }
        if (isRecoiling)
        {
            if (recoilTimer < recoilLength)
            {
                recoilTimer += Time.deltaTime;
            }
            else
            {
                isRecoiling = false;
                recoilTimer = 0;
            }
        }
        else
        {
            UpdateEnemyStates();
        }
    }
    public float playerEnemyDistance()
    {
        return _playerEnemyDistance;
    }

    public virtual void EnemyGetsHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        health -= _damageDone;
        if (!isRecoiling)
        {
            audioSource.PlayOneShot(hurtSound);
            GameObject _orangeBlood = Instantiate(Blood, transform.position, Quaternion.identity);
            Destroy(_orangeBlood, 2.5f);
            enemyRb.linearVelocity = _hitForce * recoilFactor * _hitDirection;
            isRecoiling = true;
        }
    }


    public virtual void TrapHit(float _damageDone)
    {
        health -= _damageDone;
        audioSource.PlayOneShot(hurtSound);
    }


    protected virtual void OnCollisionStay2D(Collision2D collision)
    {
        string layerName = LayerMask.LayerToName(collision.collider.gameObject.layer);

        if (layerName == "Player" && !PlayerController.Instance.pState.Invincible && !PlayerController.Instance.pState.cutscenes && health > 0)
        {
            AttackPlayer();
            if (PlayerController.Instance.pState.alive)
            {
                PlayerController.Instance.HitStopTime(0f, 8, 0.4f);
            }
        }
        else if (layerName == "Attackable" && health > 0)
        {
            Turn();
        }
    }

    public abstract void Turn();

    public abstract void AttackPlayer();

    protected virtual void UpdateEnemyStates() { }

    protected virtual void ChangeCurrentAnimation() { }

    protected void ChangeState(EnemyStates _newState)
    {
        GetCurrentEnemyState = _newState;
    }

    public bool Event()
    {
        if(health <= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public float Health
    {
        get { return health; }
        set
        {
            if (health != value)
            {
                health = Mathf.Clamp(value, 0, maxHealth);

                if (onHealthChangedCallback != null)
                {
                    onHealthChangedCallback.Invoke();
                }
            }
        }
    }
}
