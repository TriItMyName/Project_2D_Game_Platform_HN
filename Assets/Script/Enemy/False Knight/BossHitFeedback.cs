using UnityEngine;
using System.Collections;

public class BossHitFeedback : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Material defaultMaterial; // original material
    private Color defaultColor;       // original tint color (if any)

    [Header("Flash Settings")]
    public Material flashMaterial; // assign FlashMaterial in Inspector
    public float flashDuration = 0.1f;
    public float invulnerabilityTime = 1f;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        if (spriteRenderer != null)
        {
            defaultMaterial = spriteRenderer.material;
            defaultColor = spriteRenderer.color;       
        }
        else
        {
            Debug.LogWarning($"{gameObject.name}: No SpriteRenderer found for BossHitFeedback.");
        }
    }

    public void StartHitFlash()
    {
        if (spriteRenderer == null) return;
        StopAllCoroutines();
        StartCoroutine(HitFlashCoroutine());
    }

    private IEnumerator HitFlashCoroutine()
    {
        float timer = 0f;

        while (timer < invulnerabilityTime)
        {
            if (flashMaterial != null)
            {
                spriteRenderer.material = flashMaterial;
                spriteRenderer.color = UnityEngine.Color.white;
            }

            yield return new WaitForSeconds(flashDuration);

            // restore original material and color
            spriteRenderer.material = defaultMaterial;
            spriteRenderer.color = defaultColor;

            yield return new WaitForSeconds(flashDuration);
            timer += flashDuration * 2;
        }

        spriteRenderer.material = defaultMaterial;
        spriteRenderer.color = defaultColor;
    }
}