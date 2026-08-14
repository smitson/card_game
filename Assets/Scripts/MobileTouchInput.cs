using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Mobile Touch Input System for Android
/// Handles tap, drag, and card selection with visual feedback
/// </summary>
public class MobileTouchInput : MonoBehaviour
{
    [Header("Touch Settings")]
    [Tooltip("Minimum drag distance to register as a drag (not a tap)")]
    public float dragThreshold = 50f;
    
    [Tooltip("Enable visual highlighting of valid moves")]
    public bool showValidMoves = true;
    
    [Header("Visual Feedback")]
    [Tooltip("Color tint for cards that can be removed")]
    public Color validCardColor = new Color(0.5f, 1f, 0.5f, 1f); // Light green
    
    [Tooltip("Color tint for selected card")]
    public Color selectedCardColor = new Color(1f, 1f, 0.5f, 1f); // Light yellow
    
    [Header("Camera Drag Settings")]
    [Tooltip("Enable dragging to pan the camera")]
    public bool enableCameraDrag = true;
    
    [Tooltip("Camera drag speed")]
    public float cameraDragSpeed = 0.01f;
    
    private Solitaire solitaire;
    private Camera mainCamera;
    
    // Touch tracking
    private Vector2 touchStartPos;
    private Vector2 lastTouchPos;
    private bool isDragging = false;
    
    // Card highlighting
    private Dictionary<GameObject, SpriteRenderer> cardRenderers = new Dictionary<GameObject, SpriteRenderer>();
    private Dictionary<GameObject, Color> originalColors = new Dictionary<GameObject, Color>();
    private Dictionary<GameObject, Coroutine> pulseCoroutines = new Dictionary<GameObject, Coroutine>();

    [Header("Combo Settings")]
    [Tooltip("Time window in seconds between matches to chain a combo")]
    public float comboWindowSeconds = 2.5f;
    public GameObject comboIndicatorPanel;
    public Text comboText;

    [Header("Android")]
    [Tooltip("Optional quit-confirmation panel shown when back button is pressed during a game")]
    public GameObject quitConfirmPanel;

    private int comboCount = 0;
    private float lastMatchTime = -999f;
    private Coroutine comboDisplayCoroutine;

    void Start()
    {
        solitaire = FindFirstObjectByType<Solitaire>();
        mainCamera = Camera.main;
        
        if (solitaire == null)
        {
            Debug.LogError("MobileTouchInput: Solitaire component not found!");
        }
        
        Debug.Log("MobileTouchInput: Android touch controls enabled");
    }

    void Update()
    {
        HandleTouchInput();

#if UNITY_ANDROID
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HandleBackButton();
        }
#endif
    }

    void HandleTouchInput()
    {
        // Handle both touch (mobile) and mouse (editor testing)
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            HandleTouch(touch.position, touch.phase);
        }
        else if (Input.GetMouseButtonDown(0))
        {
            touchStartPos = Input.mousePosition;
            lastTouchPos = Input.mousePosition;
            isDragging = false;
        }
        else if (Input.GetMouseButton(0))
        {
            Vector2 currentPos = Input.mousePosition;
            float dragDistance = Vector2.Distance(touchStartPos, currentPos);
            
            if (dragDistance > dragThreshold)
            {
                isDragging = true;
                HandleDrag(currentPos);
            }
            
            lastTouchPos = currentPos;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (!isDragging)
            {
                HandleTap(Input.mousePosition);
            }
            isDragging = false;
        }
    }

    void HandleTouch(Vector2 position, TouchPhase phase)
    {
        switch (phase)
        {
            case TouchPhase.Began:
                touchStartPos = position;
                lastTouchPos = position;
                isDragging = false;
                break;
                
            case TouchPhase.Moved:
                float dragDistance = Vector2.Distance(touchStartPos, position);
                
                if (dragDistance > dragThreshold)
                {
                    isDragging = true;
                    HandleDrag(position);
                }
                
                lastTouchPos = position;
                break;
                
            case TouchPhase.Ended:
                if (!isDragging)
                {
                    HandleTap(position);
                }
                isDragging = false;
                break;
        }
    }

    void HandleTap(Vector2 screenPosition)
    {
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(screenPosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);
        
        if (hit.collider != null)
        {
            string tag = hit.collider.tag;
            
            Debug.Log($"Tapped on: {hit.collider.name} (Tag: {tag})");
            
            if (tag == "Deck")
            {
                TapDeck();
            }
            else if (tag == "Card")
            {
                TapCard(hit.collider.gameObject);
            }
            else if (tag == "Reset")
            {
                ResetGame();
            }
            else if (tag == "Undo")
            {
                UndoMove();
            }
        }
    }

    void HandleDrag(Vector2 currentPosition)
    {
        if (!enableCameraDrag) return;
        
        Vector2 delta = currentPosition - lastTouchPos;
        
        // Pan the camera (inverted for natural feel)
        Vector3 newPos = mainCamera.transform.position;
        newPos.x -= delta.x * cameraDragSpeed;
        newPos.y -= delta.y * cameraDragSpeed;
        
        // Optional: Clamp camera bounds
        // newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
        // newPos.y = Mathf.Clamp(newPos.y, minY, maxY);
        
        mainCamera.transform.position = newPos;
    }

    void TapDeck()
    {
        Debug.Log("MobileTouchInput: Dealing card from deck");

        if (solitaire != null && !solitaire.isGameOver)
        {
            solitaire.DealFromDeck();
            // Refresh valid move highlights after each deal
            if (showValidMoves)
                UpdateValidMoveHighlights();
        }
    }

    void TapCard(GameObject card)
    {
        if (solitaire == null || solitaire.isGameOver) return;
        
        Debug.Log($"MobileTouchInput: Tapped card {card.name}");
        
        // Check if this card can be removed
        if (IsCardRemovable(card))
        {
            TriggerHaptic(isHeavy: true);
            AudioManager.Instance?.PlayMatch();
            RemoveCard(card);
        }
        else
        {
            Debug.Log($"Card {card.name} cannot be removed (not in valid position or no match)");
            TriggerHaptic(isHeavy: false);
            AudioManager.Instance?.PlayInvalidTap();
            StartCoroutine(ShakeCard(card));
        }
    }

    bool IsCardRemovable(GameObject card)
    {
        Selectable selectable = card.GetComponent<Selectable>();
        if (selectable == null) return false;
        
        int cardIndex = solitaire.dealtCards.IndexOf(selectable.name);
        
        // Check if card is in the middle (not first or last)
        if (cardIndex <= 0 || cardIndex >= solitaire.dealtCards.Count - 1)
        {
            return false;
        }
        
        string leftCard = solitaire.dealtCards[cardIndex - 1];
        string rightCard = solitaire.dealtCards[cardIndex + 1];
        
        // Check if neighbors match by suit OR value
        bool suitMatch = leftCard.Substring(0, 1) == rightCard.Substring(0, 1);
        bool valueMatch = leftCard.Substring(1) == rightCard.Substring(1);
        
        return suitMatch || valueMatch;
    }

    void RemoveCard(GameObject card)
    {
        Selectable selectable = card.GetComponent<Selectable>();
        if (selectable == null) return;
        
        int cardIndex = solitaire.dealtCards.IndexOf(selectable.name);
        
        if (cardIndex > 0)
        {
            // Remove the LEFT neighbor card (game rule)
            int removeIndex = cardIndex - 1;
            string cardToRemove = solitaire.dealtCards[removeIndex];
            
            // Store for undo
            solitaire.removedCards.Push(cardToRemove);
            solitaire.removedCards.Push(removeIndex.ToString());
            
            // Remove from list
            solitaire.dealtCards.Remove(cardToRemove);

            // Trigger combo tracking
            TriggerCombo();

            // Destroy / animate the removed card GameObject
            GameObject cardObj = GameObject.Find(cardToRemove);
            if (cardObj != null)
            {
                CardAnimator animator = cardObj.GetComponent<CardAnimator>();
                if (animator != null)
                {
                    // VFX before the animated removal
                    VFXManager.Instance?.PlayMatchBurst(cardObj.transform.position);
                    // Defer MoveCards until animation finishes
                    animator.PlayRemoveAnimation(onComplete: () =>
                    {
                        solitaire.MoveCards();
                        RefreshValidMoveHighlights();
                    });
                }
                else
                {
                    VFXManager.Instance?.PlayMatchBurst(cardObj.transform.position);
                    Destroy(cardObj);
                    solitaire.MoveCards();
                    RefreshValidMoveHighlights();
                }
            }
            else
            {
                solitaire.MoveCards();
                RefreshValidMoveHighlights();
            }

            Debug.Log($"Removed card: {cardToRemove}");

            // Clear highlighting
            ClearAllHighlights();
        }
    }

    void UpdateValidMoveHighlights()
    {
        // Clear previous highlights
        ClearAllHighlights();

        if (!showValidMoves || solitaire == null || solitaire.dealtCards == null || solitaire.isGameOver) return;

        // Start pulsing coroutines for all removable cards
        for (int i = 1; i < solitaire.dealtCards.Count - 1; i++)
        {
            string cardName = solitaire.dealtCards[i];
            GameObject cardObj = GameObject.Find(cardName);

            if (cardObj != null && IsCardAtIndexRemovable(i))
            {
                HighlightCard(cardObj, validCardColor);
            }
        }
    }

    /// <summary>
    /// Re-run highlight pass after a card removal or move (called after MoveCards completes).
    /// </summary>
    void RefreshValidMoveHighlights()
    {
        if (showValidMoves)
            UpdateValidMoveHighlights();
    }

    bool IsCardAtIndexRemovable(int index)
    {
        if (index <= 0 || index >= solitaire.dealtCards.Count - 1)
        {
            return false;
        }
        
        string leftCard = solitaire.dealtCards[index - 1];
        string rightCard = solitaire.dealtCards[index + 1];
        
        bool suitMatch = leftCard.Substring(0, 1) == rightCard.Substring(0, 1);
        bool valueMatch = leftCard.Substring(1) == rightCard.Substring(1);
        
        return suitMatch || valueMatch;
    }

    void HighlightCard(GameObject card, Color highlightColor)
    {
        SpriteRenderer renderer = card.GetComponent<SpriteRenderer>();
        if (renderer == null) return;

        if (!originalColors.ContainsKey(card))
            originalColors[card] = renderer.color;

        cardRenderers[card] = renderer;

        // Start a pulse coroutine if one isn't already running for this card
        if (!pulseCoroutines.ContainsKey(card))
            pulseCoroutines[card] = StartCoroutine(PulseCard(card, renderer, highlightColor));
    }

    IEnumerator PulseCard(GameObject card, SpriteRenderer renderer, Color highlightColor)
    {
        Color baseColor = originalColors.ContainsKey(card) ? originalColors[card] : Color.white;
        float pulseSpeed = 2.5f; // Hz

        while (card != null && renderer != null && cardRenderers.ContainsKey(card))
        {
            float t = (Mathf.Sin(Time.time * pulseSpeed * Mathf.PI * 2f) + 1f) * 0.5f;
            renderer.color = Color.Lerp(baseColor, highlightColor, t * 0.75f);
            yield return null;
        }
    }

    void ClearAllHighlights()
    {
        // Stop all running pulse coroutines
        foreach (var kvp in pulseCoroutines)
        {
            if (kvp.Value != null)
                StopCoroutine(kvp.Value);
        }
        pulseCoroutines.Clear();

        // Restore original colors
        foreach (var kvp in cardRenderers)
        {
            if (kvp.Key != null && kvp.Value != null && originalColors.ContainsKey(kvp.Key))
                kvp.Value.color = originalColors[kvp.Key];
        }

        cardRenderers.Clear();
        originalColors.Clear();
    }

    IEnumerator ShakeCard(GameObject card)
    {
        if (card == null) yield break;
        
        Vector3 originalPos = card.transform.position;
        float shakeDuration = 0.2f;
        float shakeAmount = 0.1f;
        float elapsed = 0f;
        
        while (elapsed < shakeDuration)
        {
            float x = originalPos.x + Random.Range(-shakeAmount, shakeAmount);
            float y = originalPos.y + Random.Range(-shakeAmount, shakeAmount);
            
            card.transform.position = new Vector3(x, y, originalPos.z);
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        card.transform.position = originalPos;
    }

    void ResetGame()
    {
        Debug.Log("MobileTouchInput: Reset button pressed");
        ResetCombo();
        ClearAllHighlights();

        UIButtons uiButtons = FindFirstObjectByType<UIButtons>();
        if (uiButtons != null)
        {
            uiButtons.ResetScene();
        }
    }

    void UndoMove()
    {
        Debug.Log("MobileTouchInput: Undo button pressed");

        if (solitaire != null)
        {
            solitaire.UndoCards();
            ClearAllHighlights();
            if (showValidMoves)
                UpdateValidMoveHighlights();
        }
    }

    // Enable/disable features at runtime
    public void SetShowValidMoves(bool enabled)
    {
        showValidMoves = enabled;
        if (!enabled)
        {
            ClearAllHighlights();
        }
    }

    public void SetCameraDrag(bool enabled)
    {
        enableCameraDrag = enabled;
    }

    // ---- Combo Tracking ----

    void TriggerCombo()
    {
        float timeSinceLast = Time.time - lastMatchTime;
        comboCount = timeSinceLast <= comboWindowSeconds ? comboCount + 1 : 1;
        lastMatchTime = Time.time;

        if (comboCount >= 2)
            ShowComboIndicator(comboCount);
    }

    void ShowComboIndicator(int count)
    {
        if (comboIndicatorPanel == null || comboText == null) return;

        if (comboDisplayCoroutine != null)
            StopCoroutine(comboDisplayCoroutine);
        comboDisplayCoroutine = StartCoroutine(AnimateComboIndicator(count));
    }

    IEnumerator AnimateComboIndicator(int count)
    {
        if (comboIndicatorPanel == null || comboText == null) yield break;

        comboText.text = "COMBO x" + count + "!";
        comboIndicatorPanel.SetActive(true);

        // Pop-in: scale from 0 → 1.2 → 1
        Transform panel = comboIndicatorPanel.transform;
        panel.localScale = Vector3.zero;
        float popDuration = 0.2f;
        float elapsed = 0f;

        while (elapsed < popDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / popDuration);
            float scale = Mathf.Sin(t * Mathf.PI * 0.5f) * 1.25f;
            panel.localScale = Vector3.one * Mathf.Min(scale, 1.2f);
            yield return null;
        }
        panel.localScale = Vector3.one;

        // Hold
        yield return new WaitForSeconds(1.2f);

        // Fade out via CanvasGroup
        CanvasGroup cg = comboIndicatorPanel.GetComponent<CanvasGroup>();
        if (cg == null) cg = comboIndicatorPanel.AddComponent<CanvasGroup>();

        float fadeDuration = 0.3f;
        elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = 1f - (elapsed / fadeDuration);
            yield return null;
        }

        cg.alpha = 1f;
        comboIndicatorPanel.SetActive(false);
        comboDisplayCoroutine = null;
    }

    public void ResetCombo()
    {
        comboCount = 0;
        lastMatchTime = -999f;
        if (comboDisplayCoroutine != null)
        {
            StopCoroutine(comboDisplayCoroutine);
            comboDisplayCoroutine = null;
        }
        if (comboIndicatorPanel != null)
            comboIndicatorPanel.SetActive(false);
    }

    // ---- Haptic Feedback ----

    void TriggerHaptic(bool isHeavy = false)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            if (isHeavy)
            {
                using (AndroidJavaClass player = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                using (AndroidJavaObject activity = player.GetStatic<AndroidJavaObject>("currentActivity"))
                using (AndroidJavaObject vibrator = activity.Call<AndroidJavaObject>("getSystemService", "vibrator"))
                using (AndroidJavaClass vibrationEffect = new AndroidJavaClass("android.os.VibrationEffect"))
                {
                    AndroidJavaObject effect = vibrationEffect.CallStatic<AndroidJavaObject>(
                        "createOneShot", 80L, 200);
                    vibrator.Call("vibrate", effect);
                }
            }
            else
            {
                Handheld.Vibrate();
            }
        }
        catch (System.Exception)
        {
            Handheld.Vibrate(); // Fallback for older devices
        }
#endif
    }

    // ---- Android Back Button ----

    void HandleBackButton()
    {
        if (solitaire != null && !solitaire.isGameOver && solitaire.allCardsDealt)
        {
            // Game is in progress — show quit confirmation
            if (quitConfirmPanel != null)
                quitConfirmPanel.SetActive(true);
        }
        else if (solitaire != null && solitaire.isGameOver)
        {
            // Game over panel visible — treat back as "Play Again"
            UIButtons uiButtons = FindFirstObjectByType<UIButtons>();
            uiButtons?.PlayAgain();
        }
        else
        {
            Application.Quit();
        }
    }

    void OnDisable()
    {
        ClearAllHighlights();
        ResetCombo();
    }
}
