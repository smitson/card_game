using UnityEngine;

/// <summary>
/// Adjusts a RectTransform to stay within the screen's safe area.
/// Handles notches, punch-holes, rounded corners, and the Android navigation bar.
///
/// Inspector setup:
///   1. In SolitaireGame.unity, create a child panel on the Canvas called "SafeAreaPanel".
///   2. Set its RectTransform to stretch full-screen (anchors 0,0 → 1,1, offsets all 0).
///   3. Move all HUD UI elements (score, buttons, etc.) under SafeAreaPanel.
///   4. Attach this component to SafeAreaPanel.
///
/// The component re-applies the safe area whenever the screen size or orientation changes.
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class SafeAreaHandler : MonoBehaviour
{
    private RectTransform rectTransform;
    private Rect lastSafeArea = Rect.zero;
    private Vector2Int lastScreenSize = Vector2Int.zero;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        ApplySafeArea();
    }

    void Update()
    {
        // Re-apply if orientation or resolution changes (e.g. on rotation)
        Rect currentSafeArea = Screen.safeArea;
        Vector2Int currentSize = new Vector2Int(Screen.width, Screen.height);

        if (currentSafeArea != lastSafeArea || currentSize != lastScreenSize)
        {
            ApplySafeArea();
        }
    }

    void ApplySafeArea()
    {
        Rect safeArea = Screen.safeArea;
        lastSafeArea = safeArea;
        lastScreenSize = new Vector2Int(Screen.width, Screen.height);

        if (Screen.width == 0 || Screen.height == 0) return;

        // Convert pixel-space safe area to normalized anchor coordinates
        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = safeArea.position + safeArea.size;

        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;

        Debug.Log($"SafeAreaHandler: Applied safe area {safeArea} on {Screen.width}x{Screen.height}");
    }
}
