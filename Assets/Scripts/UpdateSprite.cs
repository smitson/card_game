using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Updates card sprite based on card name
/// No longer requires UserInput - works with MobileTouchInput
/// </summary>
public class UpdateSprite : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Solitaire solitaire;

    void Start()
    {
        // Get components
        spriteRenderer = GetComponent<SpriteRenderer>();
        solitaire = FindObjectOfType<Solitaire>();

        if (solitaire != null && spriteRenderer != null)
        {
            UpdateCardSprite();
        }
    }

    void UpdateCardSprite()
    {
        // Get card name (e.g., "C5", "DA", "HK")
        string cardName = gameObject.name;

        if (string.IsNullOrEmpty(cardName) || cardName.Length < 2)
        {
            Debug.LogWarning($"Invalid card name: {cardName}");
            return;
        }

        // Parse suit and value
        char suit = cardName[0];
        string value = cardName.Substring(1);

        // Find the correct sprite index
        int spriteIndex = GetSpriteIndex(suit, value);

        if (spriteIndex >= 0 && spriteIndex < solitaire.cardFaces.Length)
        {
            spriteRenderer.sprite = solitaire.cardFaces[spriteIndex];
        }
        else
        {
            Debug.LogWarning($"Could not find sprite for card: {cardName}");
        }
    }

    int GetSpriteIndex(char suit, string value)
    {
        // Sprite array is ordered: Clubs, Diamonds, Hearts, Spades
        // Within each suit: A, 2, 3, 4, 5, 6, 7, 8, 9, 10, J, Q, K

        int suitOffset = 0;
        switch (suit)
        {
            case 'C': suitOffset = 0; break;   // Clubs: 0-12
            case 'D': suitOffset = 13; break;  // Diamonds: 13-25
            case 'H': suitOffset = 26; break;  // Hearts: 26-38
            case 'S': suitOffset = 39; break;  // Spades: 39-51
            default:
                Debug.LogError($"Unknown suit: {suit}");
                return -1;
        }

        int valueIndex = 0;
        switch (value)
        {
            case "A": valueIndex = 0; break;
            case "2": valueIndex = 1; break;
            case "3": valueIndex = 2; break;
            case "4": valueIndex = 3; break;
            case "5": valueIndex = 4; break;
            case "6": valueIndex = 5; break;
            case "7": valueIndex = 6; break;
            case "8": valueIndex = 7; break;
            case "9": valueIndex = 8; break;
            case "10": valueIndex = 9; break;
            case "J": valueIndex = 10; break;
            case "Q": valueIndex = 11; break;
            case "K": valueIndex = 12; break;
            default:
                Debug.LogError($"Unknown value: {value}");
                return -1;
        }

        return suitOffset + valueIndex;
    }

    // Optional: Update sprite if card name changes dynamically
    public void RefreshSprite()
    {
        UpdateCardSprite();
    }
}
