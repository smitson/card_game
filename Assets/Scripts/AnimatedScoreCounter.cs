using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Attaches to a Text GameObject and animates the score counter
/// from its current displayed value to a new target value using an EaseOutCubic curve.
///
/// Inspector setup:
///   - Attach to the score label GameObject alongside a Text component.
///   - Assign to Solitaire.scoreCounter in the Inspector.
///   - Adjust scorePrefix and animationDuration as desired.
/// </summary>
[RequireComponent(typeof(Text))]
public class AnimatedScoreCounter : MonoBehaviour
{
    [Tooltip("Text shown before the number, e.g. 'Score: '")]
    public string scorePrefix = "Score: ";

    [Tooltip("Duration in seconds for the number-tick animation")]
    public float animationDuration = 0.4f;

    private Text label;
    private int displayedValue = 0;
    private Coroutine activeAnimation;

    void Awake()
    {
        label = GetComponent<Text>();
        label.text = scorePrefix + "0";
    }

    /// <summary>
    /// Animate the score display from its current value to newValue.
    /// Safe to call every frame — cancels and restarts if already animating.
    /// </summary>
    public void UpdateScore(int newValue)
    {
        if (activeAnimation != null)
            StopCoroutine(activeAnimation);
        activeAnimation = StartCoroutine(AnimateScore(displayedValue, newValue));
    }

    /// <summary>
    /// Set the score immediately without animation (used for game resets).
    /// </summary>
    public void SetScoreImmediate(int value)
    {
        if (activeAnimation != null)
        {
            StopCoroutine(activeAnimation);
            activeAnimation = null;
        }
        displayedValue = value;
        label.text = scorePrefix + value.ToString();
    }

    private IEnumerator AnimateScore(int from, int to)
    {
        float elapsed = 0f;

        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / animationDuration);

            // EaseOutCubic: snappy start, decelerates to rest
            t = 1f - Mathf.Pow(1f - t, 3f);

            displayedValue = Mathf.RoundToInt(Mathf.Lerp(from, to, t));
            label.text = scorePrefix + displayedValue.ToString();
            yield return null;
        }

        displayedValue = to;
        label.text = scorePrefix + to.ToString();
        activeAnimation = null;
    }
}
