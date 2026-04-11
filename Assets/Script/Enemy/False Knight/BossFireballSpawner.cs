using System.Collections;
using UnityEngine;

public class BossFireballSpawner : MonoBehaviour
{
    [Header("Fireball Settings")]
    public GameObject fireballPrefab;
    public BoxCollider2D spawnBallArea;

    public int minFireballs = 4;
    public int maxFireballs = 8;
    public float spawnDelay = 0.1f;

    public Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
    }

    void Start()
    {
        StartCoroutine(EnableGravity());
    }

    public IEnumerator SpawnFireballs(int overrideCount = 0)
    {
        if (fireballPrefab == null || spawnBallArea == null)
            yield break;

        int count = (overrideCount > 0)
            ? overrideCount
            : Random.Range(minFireballs, maxFireballs + 1);

        Bounds bounds = spawnBallArea.bounds;

        for (int i = 0; i < count; i++)
        {
            float randomX = Random.Range(bounds.min.x, bounds.max.x);
            Vector2 spawnPos = new Vector2(randomX, bounds.center.y);

            Instantiate(fireballPrefab, spawnPos, Quaternion.identity);
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    IEnumerator EnableGravity()
        {
            yield return new WaitForSeconds(0.1f);
            rb.gravityScale = 2f; 
        }
}
