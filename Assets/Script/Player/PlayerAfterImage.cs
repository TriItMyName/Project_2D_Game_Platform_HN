using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAfterImage : MonoBehaviour
{
    public float afterImagesDelay;
    private float afterImagesSeconds;
    public bool Generate = false;

    private SpriteRenderer sr;

    private void Start()
    {
        // cache SpriteRenderer and validate
        sr = GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            Debug.LogWarning("PlayerAfterImage: no SpriteRenderer found on this GameObject.", this);
        }
        SetDelay();
    }

    private void Update()
    {
        if (!Generate) return;

        if (afterImagesSeconds > 0)
        {
            afterImagesSeconds -= Time.deltaTime;
            return;
        }

        // Validate pool singleton
        var pool = PlayerAfterImagesPool.Instance;
        if (pool == null)
        {
            // try a fallback find so you get a clearer log during debugging
            pool = FindObjectOfType<PlayerAfterImagesPool>();
            if (pool == null)
            {
                Debug.LogError("PlayerAfterImage: PlayerAfterImagesPool instance is null. Make sure the pool exists and its singleton is initialized before after-image generation.", this);
                SetDelay();
                return;
            }
            else
            {
                Debug.LogWarning("PlayerAfterImage: Found PlayerAfterImagesPool via FindObjectOfType (singleton was null). Consider fixing initialization order.", this);
            }
        }

        GameObject afterImage = null;
        try
        {
            afterImage = pool.GetFromPool();
        }
        catch (System.Exception ex)
        {
            Debug.LogError("PlayerAfterImage: exception when getting from pool: " + ex.Message, this);
        }

        if (afterImage == null)
        {
            Debug.LogWarning("PlayerAfterImage: pool returned null afterImage. Check pool configuration.", this);
            SetDelay();
            return;
        }

        // move/rotate pooled object
        afterImage.transform.position = transform.position;
        afterImage.transform.rotation = transform.rotation;

        // validate sprite on player and pooled object
        if (sr == null)
        {
            sr = GetComponent<SpriteRenderer>();
            if (sr == null)
            {
                Debug.LogWarning("PlayerAfterImage: still no SpriteRenderer on player; cannot copy sprite.", this);
                SetDelay();
                return;
            }
        }

        var aiSr = afterImage.GetComponent<SpriteRenderer>();
        if (aiSr == null)
        {
            Debug.LogWarning("PlayerAfterImage: pooled afterImage has no SpriteRenderer. Add one to the prefab.", afterImage);
            SetDelay();
            return;
        }

        aiSr.sprite = sr.sprite;

        StartCoroutine(ReturnAfterImageToPool(afterImage));
        SetDelay();
    }

    private IEnumerator ReturnAfterImageToPool(GameObject afterImage)
    {
        yield return new WaitForSeconds(0.3f);

        if (PlayerAfterImagesPool.Instance == null)
        {
            Debug.LogWarning("PlayerAfterImage: cannot return afterImage to pool because Instance is null. Destroying object instead.", this);
            Destroy(afterImage);
            yield break;
        }

        PlayerAfterImagesPool.Instance.ReturnToPool(afterImage);
    }
    public float SetDelay()
    {
        return afterImagesSeconds = afterImagesDelay;
    }
}
