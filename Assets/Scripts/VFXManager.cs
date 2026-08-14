using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Singleton VFX manager for particle effects and screen flashes.
///
/// Scene setup:
///   1. Create an empty GameObject named "VFXManager" in SolitaireGame.unity.
///   2. Attach this component to it.
///   3. Assign the particle prefabs (MatchBurstFX, ConfettiFX) in the Inspector.
///   4. Create a full-screen Image (white, alpha 0, Raycast Target = OFF) on the Canvas.
///      Assign it to flashOverlay. Disable it in the Inspector.
///
/// Particle prefabs (create in Unity Editor):
///   MatchBurstFX — 20-particle gold burst, 0.5s, sphere shape, auto-destroy
///   ConfettiFX   — 150 particles, 4s, box emitter, gravity, multi-color
///
/// All calls are null-safe so tests without a scene are unaffected.
/// </summary>
public class VFXManager : MonoBehaviour
{
    public static VFXManager Instance { get; private set; }

    [Header("Particle Prefabs")]
    [Tooltip("Small sparkle/star burst played at a card's world position on match")]
    public GameObject matchBurstPrefab;

    [Tooltip("Full-screen confetti system played when the player wins")]
    public GameObject confettiPrefab;

    [Header("Screen Flash")]
    [Tooltip("Full-screen UI Image used for flash effects. Set Raycast Target = false so it never blocks input.")]
    public Image flashOverlay;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // ---- Public API ----

    /// <summary>
    /// Spawn a small particle burst at the given world position (call on card match).
    /// </summary>
    public void PlayMatchBurst(Vector3 worldPosition)
    {
        if (matchBurstPrefab == null) return;
        GameObject fx = Instantiate(matchBurstPrefab, worldPosition, Quaternion.identity);
        ParticleSystem ps = fx.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            ps.Play();
            Destroy(fx, ps.main.duration + ps.main.startLifetime.constantMax);
        }
        else
        {
            Destroy(fx, 2f);
        }
    }

    /// <summary>
    /// Spawn a confetti rain effect when the player wins.
    /// </summary>
    public void PlayWinConfetti()
    {
        if (confettiPrefab == null) return;
        Camera cam = Camera.main;
        Vector3 spawnPos = cam != null
            ? cam.transform.position + new Vector3(0f, 5f, 5f)
            : new Vector3(0f, 5f, 5f);
        GameObject fx = Instantiate(confettiPrefab, spawnPos, Quaternion.identity);
        Destroy(fx, 5f);
    }

    /// <summary>
    /// Brief gold flash on win.
    /// </summary>
    public void PlayWinFlash()
    {
        if (flashOverlay == null) return;
        StartCoroutine(FlashScreen(new Color(1f, 0.9f, 0f, 0.3f), 0.1f, 0.5f));
    }

    /// <summary>
    /// Brief red flash on loss.
    /// </summary>
    public void PlayLoseFlash()
    {
        if (flashOverlay == null) return;
        StartCoroutine(FlashScreen(new Color(1f, 0f, 0f, 0.35f), 0.15f, 0.4f));
    }

    // ---- Internal ----

    IEnumerator FlashScreen(Color flashColor, float fadeInTime, float fadeOutTime)
    {
        flashOverlay.gameObject.SetActive(true);
        flashOverlay.color = new Color(flashColor.r, flashColor.g, flashColor.b, 0f);

        // Fade in
        float elapsed = 0f;
        while (elapsed < fadeInTime)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, flashColor.a, elapsed / fadeInTime);
            flashOverlay.color = new Color(flashColor.r, flashColor.g, flashColor.b, alpha);
            yield return null;
        }

        // Fade out
        elapsed = 0f;
        while (elapsed < fadeOutTime)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(flashColor.a, 0f, elapsed / fadeOutTime);
            flashOverlay.color = new Color(flashColor.r, flashColor.g, flashColor.b, alpha);
            yield return null;
        }

        flashOverlay.gameObject.SetActive(false);
    }
}
