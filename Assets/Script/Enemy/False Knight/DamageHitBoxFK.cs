using UnityEngine;

public class DamageHitBoxFK : MonoBehaviour
{
    [Header("Damage Settings")]
    public float damage = 1f; 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.gameObject.name == "Player") 
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }
        }
    }
}
