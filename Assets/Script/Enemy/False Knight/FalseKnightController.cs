using System.Collections;
using UnityEngine;

public class FKController : Enemy
{
    public enum BossPhase { Phase1, Phase2, Phase3 }

    [Header("Current Status")]
    public BossPhase currentPhase = BossPhase.Phase1;

    [Header("Settings")]
    public float attackRange = 3f;
    public float detectRange = 25f;
    private float currentCooldownTime;

    [Header("Jump Settings")]
    public float jumpForce = 15f;
    public float jumpForwardForce = 12f;

    [Header("Phase 2 & 3")]
    public float madSlamSpeed = 0.2f;

    [Header("Audio")]
    public AudioClip jumpSound;
    public AudioClip slamSound;

    public Transform groundCheckPoint;


    public bool attacking = false;
    private float attackTimer;
    [HideInInspector] public bool facingLeft;

    private BossHitFeedback hitFeedback;
    private BossFireballSpawner fireballSpawner;

    private bool isDead = false;


    protected override void Start()
    {
        base.Start();
        hitFeedback = GetComponent<BossHitFeedback>();
        fireballSpawner = GetComponent<BossFireballSpawner>();

        if (PlayerController.Instance != null)
            _playerTransform = PlayerController.Instance.transform;
    }

    protected override void Update()
    {
        if (isDead) return;
        base.Update();
        if (health <= 0 && !isDead)
        {
            isDead = true;
            StartCoroutine(Dead());
            enemyRb.linearVelocity = Vector2.zero;
            return;
        }
        if (PlayerController.Instance == null) return;

        UpdateBossPhase();

        if (attackTimer > 0)
            attackTimer -= Time.deltaTime;

        if (!attacking && attackTimer <= 0)
        {
            float distance = Vector2.Distance(transform.position, _playerTransform.position);
            Flip();

            if (distance <= attackRange)
                DecideCloseRangeAction();
            else if (distance <= detectRange)
                DecideLongRangeAction();
            else
                StopMoving();
        }
    }

    void UpdateBossPhase()
    {
        float hpPercent = (health / maxHealth) * 100f;

        if (hpPercent > 70f)
        {
            currentPhase = BossPhase.Phase1;
            currentCooldownTime = Random.Range(1.5f, 2f);
        }
        else if (hpPercent > 40f)
        {
            currentPhase = BossPhase.Phase2;
            currentCooldownTime = 1.2f;
        }
        else
        {
            currentPhase = BossPhase.Phase3;
            currentCooldownTime = 0.6f;
        }
    }

    void DecideCloseRangeAction()
    {
        int rand = Random.Range(0, 100);

        if (currentPhase == BossPhase.Phase3 && rand < 30)
            StartCoroutine(Skill5_MadSlam());
        else if (currentPhase != BossPhase.Phase1 && rand < 50)
            StartCoroutine(Skill3_JumpBackAttack());
        else
            StartCoroutine(Skill4_StandingSlash());
    }

    void DecideLongRangeAction()
    {
        if (currentPhase == BossPhase.Phase1)
            StartCoroutine(Random.Range(0, 100) < 50 ? Skill1_BodySlam() : Skill2_JumpAttackSlam());
        else
            StartCoroutine(Skill2_JumpAttackSlam());
    }

    IEnumerator Skill1_BodySlam()
    {
        attacking = true;
        StopMoving();

        _animator.SetTrigger("Jump");
        yield return new WaitForSeconds(0.4f);

        PerformJump(true, 1f);

        yield return new WaitUntil(() => enemyRb.linearVelocity.y <= 0);
        yield return new WaitUntil(Grounded);

        enemyRb.linearVelocity = Vector2.zero;
        _animator.SetTrigger("Land");
        PlaySound(slamSound);

        yield return new WaitForSeconds(0.5f);
        EndAttack();
    }

    IEnumerator Skill2_JumpAttackSlam()
    {
        attacking = true;
        StopMoving();

        _animator.SetTrigger("JumpAttack");
        yield return new WaitForSeconds(0.4f);

        PerformJump(true, 1f);

        yield return new WaitUntil(() => enemyRb.linearVelocity.y <= 0);
        yield return new WaitUntil(Grounded);

        enemyRb.linearVelocity = Vector2.zero;
        _animator.SetTrigger("Attack");
        PlaySound(slamSound);

        if (currentPhase != BossPhase.Phase1 && fireballSpawner != null)
            StartCoroutine(fireballSpawner.SpawnFireballs());

        yield return new WaitForSeconds(0.5f);
        EndAttack();
    }

    IEnumerator Skill3_JumpBackAttack()
    {
        attacking = true;
        StopMoving();

        _animator.SetTrigger("Jump");
        yield return new WaitForSeconds(0.3f);

        PerformJump(false, 0.8f);

        yield return new WaitUntil(() => enemyRb.linearVelocity.y <= 0);
        yield return new WaitUntil(Grounded);

        enemyRb.linearVelocity = Vector2.zero;
        _animator.SetTrigger("Land");

        yield return new WaitForSeconds(0.4f);
        _animator.SetTrigger("Attack");

        PlaySound(slamSound);

        if (currentPhase != BossPhase.Phase1 && fireballSpawner != null)
            StartCoroutine(fireballSpawner.SpawnFireballs());

        yield return new WaitForSeconds(0.8f);
        EndAttack();
    }


    IEnumerator Skill4_StandingSlash()
    {
        attacking = true;
        StopMoving();

        _animator.SetTrigger("Attack");
        yield return new WaitForSeconds(0.2f);
        PlaySound(slamSound);

        yield return new WaitForSeconds(1f);
        EndAttack();
    }

    IEnumerator Skill5_MadSlam()
    {
        attacking = true;
        StopMoving();

        int totalSlams = Random.Range(4, 8);

        for (int i = 0; i < totalSlams; i++)
        {
            ForceFlip(i % 2 == 0);
            _animator.SetTrigger("Attack");
            PlaySound(slamSound);

            if (fireballSpawner != null)
                StartCoroutine(fireballSpawner.SpawnFireballs(1));

            yield return new WaitForSeconds(madSlamSpeed);
        }

        yield return new WaitForSeconds(1f);
        EndAttack();
    }

    void PerformJump(bool towardPlayer, float mul)
    {
        float dir = towardPlayer
            ? (_playerTransform.position.x > transform.position.x ? 1 : -1)
            : (_playerTransform.position.x > transform.position.x ? -1 : 1);

        enemyRb.linearVelocity = new Vector2(dir * jumpForwardForce * mul, jumpForce);
    }

    void EndAttack()
    {
        attacking = false;
        attackTimer = currentCooldownTime;
        _animator.ResetTrigger("Attack");
        _animator.ResetTrigger("Jump");
        _animator.ResetTrigger("JumpAttack");
        _animator.ResetTrigger("Land");
    }

    void StopMoving()
    {
        enemyRb.linearVelocity = new Vector2(0, enemyRb.linearVelocity.y);
    }

    public void Flip()
    {
        if (attacking) return;

        bool faceLeft = _playerTransform.position.x < transform.position.x;
        transform.rotation = Quaternion.Euler(0, faceLeft ? 180 : 0, 0);
        facingLeft = faceLeft;
    }

    void ForceFlip(bool faceRight)
    {
        transform.rotation = Quaternion.Euler(0, faceRight ? 0 : 180, 0);
        facingLeft = !faceRight;
    }

    public bool Grounded()
    {
        return Physics2D.Raycast(
            groundCheckPoint.position,
            Vector2.down,
            0.2f,
            LayerMask.GetMask("Ground")
        );
    }

    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }

    IEnumerator Dead()
    {
        attacking = true;

        enemyRb.linearVelocity = Vector2.zero;
        enemyRb.bodyType = RigidbodyType2D.Kinematic;

        GetComponent<Collider2D>().enabled = false;

        if (deathSound != null)
            PlaySound(deathSound);

        _animator.SetTrigger("Death");

        yield return new WaitForSeconds(1.2f);

        StartCoroutine(fadeCoroutine());
    }

    private IEnumerator fadeCoroutine()
    {
        if (GlobalController.instance != null)
        {
            GlobalController.instance.playerScore += scoreValue;
        }

        float fadeTime = destroyDelay;
        float t = 0f;
        if (_spriteRenderer != null)
        {
            Color c = _spriteRenderer.color;
            c.a = 1f;
            _spriteRenderer.color = c;
        }

        while (t < fadeTime)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, t / fadeTime);

            if (_spriteRenderer != null)
            {
                Color c = _spriteRenderer.color;
                c.a = alpha;
                _spriteRenderer.color = c;
            }

            yield return null;
        }

        if (_spriteRenderer != null)
        {
            Color c = _spriteRenderer.color;
            c.a = 0f;
            _spriteRenderer.color = c;
        }

        Destroy(gameObject);
    }
    public override void AttackPlayer() { if (PlayerController.Instance != null) PlayerController.Instance.TakeDamage(damage); }
    public override void Turn() { }
}
