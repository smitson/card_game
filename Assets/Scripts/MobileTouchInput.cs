using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private GameObject selectedCard = null;
    
    // Card highlighting
    private Dictionary<GameObject, SpriteRenderer> cardRenderers = new Dictionary<GameObject, SpriteRenderer>();
    private Dictionary<GameObject, Color> originalColors = new Dictionary<GameObject, Color>();

    void Start()
    {
        solitaire = FindObjectOfType<Solitaire>();
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
        
        // Update visual feedback for valid moves
        if (showValidMoves && !solitaire.isGameOver)
        {
            UpdateValidMoveHighlights();
        }
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
            selectedCard = null;
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
                selectedCard = null;
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
        }
    }

    void TapCard(GameObject card)
    {
        if (solitaire == null || solitaire.isGameOver) return;
        
        Debug.Log($"MobileTouchInput: Tapped card {card.name}");
        
        // Check if this card can be removed
        if (IsCardRemovable(card))
        {
            RemoveCard(card);
        }
        else
        {
            Debug.Log($"Card {card.name} cannot be removed (not in valid position or no match)");
            
            // Visual feedback for invalid tap (shake card?)
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
            
            // Destroy the GameObject
            GameObject cardObj = GameObject.Find(cardToRemove);
            if (cardObj != null)
            {
                // Optional: Add fade-out animation here
                Destroy(cardObj);
            }
            
            Debug.Log($"Removed card: {cardToRemove}");
            
            // Update card positions
            solitaire.MoveCards();
            
            // Clear highlighting
            ClearAllHighlights();
        }
    }

    void UpdateValidMoveHighlights()
    {
        // Clear previous highlights
        ClearAllHighlights();
        
        if (solitaire == null || solitaire.dealtCards == null) return;
        
        // Highlight all cards that can be removed
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

    void HighlightCard(GameObject card, Color color)
    {
        SpriteRenderer renderer = card.GetComponent<SpriteRenderer>();
        if (renderer == null) return;
        
        // Store original color if not already stored
        if (!originalColors.ContainsKey(card))
        {
            originalColors[card] = renderer.color;
        }
        
        // Apply highlight color
        renderer.color = color;
        cardRenderers[card] = renderer;
    }

    void ClearAllHighlights()
    {
        foreach (var kvp in cardRenderers)
        {
            if (kvp.Key != null && kvp.Value != null)
            {
                if (originalColors.ContainsKey(kvp.Key))
                {
                    kvp.Value.color = originalColors[kvp.Key];
                }
            }
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
        
        UIButtons uiButtons = FindObjectOfType<UIButtons>();
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

    void OnDisable()
    {
        ClearAllHighlights();
    }
}
