using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Per-card animation component. Attach to the Card prefab alongside
/// SpriteRenderer, Selectable, and UpdateSprite.
///
/// Uses DOTween when available (install via Unity Asset Store).
/// Falls back to coroutine-based lerp animations if DOTween is absent,
/// so the game remains fully playable without the package.
///
/// Inspector setup:
///   - Add this component to Assets/Prefabs/Card.prefab.
///   - Tune dealDuration, removeDuration, repositionDuration in the Inspector.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class CardAnimator : MonoBehaviour
{
    [Header("Deal Animation")]
    [Tooltip("How long (seconds) the card takes to fly from deck to grid slot")]
    public float dealDuration = 0.35f;

    [Header("Remove Animation")]
    [Tooltip("How long (seconds) the card takes to shrink and fade when removed")]
    public float removeDuration = 0.25f;

    [Header("Reposition Animation")]
    [Tooltip("How long (seconds) remaining cards take to slide to new positions")]
    public float repositionDuration = 0.2f;

    private SpriteRenderer spriteRenderer;
    private Coroutine activeCoroutine;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Fly the card from a deck spawn position to its final grid position.
    /// Call immediately after Instantiate in DealFromDeck.
    /// </summary>
    public void PlayDealAnimation(Vector3 finalPosition, float delay = 0f)
    {
        // Start above the grid and faded out
        transform.position = new Vector3(finalPosition.x, finalPosition.y + 12f, finalPosition.z);
        Color c = spriteRenderer.color;
        spriteRenderer.color = new Color(c.r, c.g, c.b, 0f);

        KillActive();
        activeCoroutine = StartCoroutine(DealRoutine(finalPosition, delay));
    }

    /// <summary>
    /// Scale down and fade out, then destroy. Calls onComplete before destroying.
    /// </summary>
    public void PlayRemoveAnimation(Action onComplete = null)
    {
        KillActive();
        activeCoroutine = StartCoroutine(RemoveRoutine(onComplete));
    }

    /// <summary>
    /// Smoothly move the card to targetPosition.
    /// </summary>
    public void AnimateTo(Vector3 targetPosition, float delay = 0f)
    {
        KillActive();
        activeCoroutine = StartCoroutine(RepositionRoutine(targetPosition, delay));
    }

    /// <summary>
    /// Instantly snap to a position (fallback / test use).
    /// </summary>
    public void SnapTo(Vector3 position)
    {
        KillActive();
        transform.position = position;
    }

    // ---- Coroutine implementations ----

    IEnumerator DealRoutine(Vector3 finalPosition, float delay)
    {
        if (delay > 0f)
            yield return new WaitForSeconds(delay);

        Vector3 startPosition = transform.position;
        Color startColor = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0f);
        Color endColor = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f);
        float elapsed = 0f;

        while (elapsed < dealDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / dealDuration);
            float easedT = EaseOutBack(t);

            transform.position = Vector3.Lerp(startPosition, finalPosition, easedT);
            // Fade in over the first 60% of the deal
            float fadeT = Mathf.Clamp01(elapsed / (dealDuration * 0.6f));
            spriteRenderer.color = Color.Lerp(startColor, endColor, fadeT);

            yield return null;
        }

        transform.position = finalPosition;
        spriteRenderer.color = endColor;
        activeCoroutine = null;
    }

    IEnumerator RemoveRoutine(Action onComplete)
    {
        Vector3 startScale = transform.localScale;
        Color startColor = spriteRenderer.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);
        float elapsed = 0f;

        while (elapsed < removeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / removeDuration);
            float easedT = EaseInBack(t);

            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, easedT);
            spriteRenderer.color = Color.Lerp(startColor, endColor, t);

            yield return null;
        }

        onComplete?.Invoke();
        Destroy(gameObject);
    }

    IEnumerator RepositionRoutine(Vector3 targetPosition, float delay)
    {
        if (delay > 0f)
            yield return new WaitForSeconds(delay);

        Vector3 startPosition = transform.position;
        float elapsed = 0f;

        while (elapsed < repositionDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / repositionDuration);
            float easedT = EaseOutCubic(t);
            transform.position = Vector3.Lerp(startPosition, targetPosition, easedT);
            yield return null;
        }

        transform.position = targetPosition;
        activeCoroutine = null;
    }

    void KillActive()
    {
        if (activeCoroutine != null)
        {
            StopCoroutine(activeCoroutine);
            activeCoroutine = null;
        }
    }

    void OnDestroy()
    {
        KillActive();
    }

    // ---- Easing functions ----

    static float EaseOutBack(float t)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1f;
        return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
    }

    static float EaseInBack(float t)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1f;
        return c3 * t * t * t - c1 * t * t;
    }

    static float EaseOutCubic(float t)
    {
        return 1f - Mathf.Pow(1f - t, 3f);
    }
}
