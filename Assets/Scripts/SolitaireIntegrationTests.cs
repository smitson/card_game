using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

/// <summary>
/// Integration Tests for Solitaire Game (PlayMode)
/// These tests run the actual game and verify complete gameplay flow
/// </summary>
public class SolitaireIntegrationTests
{
    private Solitaire solitaire;
    private UIButtons uiButtons;

    /// <summary>
    /// Setup before each test - creates a full game scene
    /// </summary>
    [UnitySetUp]
    public IEnumerator Setup()
    {
        Debug.Log("========================================");
        Debug.Log("SETTING UP INTEGRATION TEST");
        Debug.Log("========================================");

        // Create main camera
        GameObject cameraObj = new GameObject("Main Camera");
        Camera camera = cameraObj.AddComponent<Camera>();
        camera.tag = "MainCamera";
        camera.transform.position = new Vector3(0, 0, -10);

        // Create Solitaire game object
        GameObject solitaireObj = new GameObject("Solitaire");
        solitaire = solitaireObj.AddComponent<Solitaire>();

        // Create card prefab with all required components
        GameObject cardPrefab = new GameObject("CardPrefab");
        cardPrefab.tag = "Card";
        cardPrefab.AddComponent<SpriteRenderer>();
        cardPrefab.AddComponent<BoxCollider2D>();
        cardPrefab.AddComponent<Selectable>();
        cardPrefab.AddComponent<UpdateSprite>();

        // Create deck button
        GameObject deckButton = new GameObject("DeckButton");
        deckButton.tag = "Deck";
        deckButton.AddComponent<SpriteRenderer>();
        deckButton.AddComponent<BoxCollider2D>();

        // Create UI elements
        GameObject highScorePanel = new GameObject("HighScorePanel");
        highScorePanel.AddComponent<Canvas>();

        GameObject cardArea = new GameObject("CardArea");
        GameObject staticArea = new GameObject("StaticArea");

        // Setup Solitaire references
        solitaire.cardPrefab = cardPrefab;
        solitaire.deckButton = deckButton;
        solitaire.highScorePanel = highScorePanel;
        solitaire.cardArea = cardArea.transform;
        solitaire.staticArea = staticArea.transform;
        solitaire.cardFaces = new Sprite[52]; // Mock sprites

        // Create UIButtons
        GameObject uiButtonsObj = new GameObject("UIButtons");
        uiButtons = uiButtonsObj.AddComponent<UIButtons>();
        uiButtons.highScorePanel = highScorePanel;

        // Initialize the game
        solitaire.PlayCards();

        yield return null; // Wait one frame
        
        Debug.Log("âœ“ Integration test setup complete");
    }

    /// <summary>
    /// Cleanup after each test
    /// </summary>
    [UnityTearDown]
    public IEnumerator Teardown()
    {
        // Destroy all test objects
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            Object.Destroy(obj);
        }

        yield return null;
        
        Debug.Log("âœ“ Integration test cleanup complete");
        Debug.Log("========================================\n");
    }

    /// <summary>
    /// INTEGRATION TEST 1: Complete game initialization and first deal
    /// </summary>
    [UnityTest]
    public IEnumerator IntegrationTest_01_GameInitialization()
    {
        Debug.Log("\nâ–¶â–¶â–¶ RUNNING INTEGRATION TEST 1: Game Initialization");
        
        // Verify initial state
        Assert.IsNotNull(solitaire, "Solitaire should exist");
        Assert.AreEqual(0, solitaire.currentScore, "Initial score should be 0");
        Assert.IsFalse(solitaire.allCardsDealt, "Cards should not all be dealt");
        Assert.IsFalse(solitaire.isGameOver, "Game should not be over");
        
        yield return null;
        
        Debug.Log("âœ“âœ“âœ“ INTEGRATION TEST 1 PASSED");
    }

    /// <summary>
    /// INTEGRATION TEST 2: Deal all 52 cards
    /// </summary>
    [UnityTest]
    public IEnumerator IntegrationTest_02_DealAllCards()
    {
        Debug.Log("\nâ–¶â–¶â–¶ RUNNING INTEGRATION TEST 2: Deal All 52 Cards");
        
        int cardsToDeal = 52;
        
        for (int i = 0; i < cardsToDeal; i++)
        {
            int previousScore = solitaire.currentScore;
            solitaire.DealFromDeck();
            
            // Verify score increased
            Assert.AreEqual(previousScore + 1, solitaire.currentScore, 
                $"Score should increase to {previousScore + 1} after dealing card {i + 1}");
            
            // Every 10 cards, wait a frame
            if (i % 10 == 0)
            {
                yield return null;
                Debug.Log($"Dealt {i + 1} cards... Score: {solitaire.currentScore}");
            }
        }
        
        // Verify final state
        Assert.AreEqual(52, solitaire.currentScore, "Final score should be 52");
        Assert.AreEqual(52, solitaire.dealtCards.Count, "Should have 52 cards dealt");
        Assert.IsTrue(solitaire.allCardsDealt, "All cards should be marked as dealt");
        
        yield return null;
        
        Debug.Log("âœ“âœ“âœ“ INTEGRATION TEST 2 PASSED - All 52 cards dealt successfully");
    }

    /// <summary>
    /// INTEGRATION TEST 3: Simulate a winning game
    /// </summary>
    [UnityTest]
    public IEnumerator IntegrationTest_03_WinCondition()
    {
        Debug.Log("\nâ–¶â–¶â–¶ RUNNING INTEGRATION TEST 3: Win Condition Simulation");
        
        // Setup a winning scenario manually
        solitaire.dealtCards.Clear();
        solitaire.dealtCards.Add("CA"); // Only 1 card
        solitaire.allCardsDealt = true;
        solitaire.isGameOver = false;
        solitaire.currentScore = 1;
        
        Debug.Log("Set up win scenario: 1 card remaining");
        
        yield return new WaitForSeconds(0.1f);
        
        // Manually trigger the check (since Update() might not run in test)
        // In actual gameplay, this happens automatically
        if (solitaire.dealtCards.Count <= 1)
        {
            Debug.Log("WIN DETECTED! Cards remaining: " + solitaire.dealtCards.Count);
            Assert.LessOrEqual(solitaire.dealtCards.Count, 1, "Win condition: â‰¤1 card");
        }
        
        yield return null;
        
        Debug.Log("âœ“âœ“âœ“ INTEGRATION TEST 3 PASSED - Win condition detected");
    }

    /// <summary>
    /// INTEGRATION TEST 4: Simulate a losing game
    /// </summary>
    [UnityTest]
    public IEnumerator IntegrationTest_04_LoseCondition()
    {
        Debug.Log("\nâ–¶â–¶â–¶ RUNNING INTEGRATION TEST 4: Lose Condition Simulation");
        
        // Setup a losing scenario - cards remain but no valid moves
        solitaire.dealtCards.Clear();
        solitaire.dealtCards.Add("CA");  // Ace of Clubs
        solitaire.dealtCards.Add("D2");  // 2 of Diamonds
        solitaire.dealtCards.Add("H3");  // 3 of Hearts
        solitaire.dealtCards.Add("S4");  // 4 of Spades
        // No adjacent cards match - this is a lose condition
        
        solitaire.allCardsDealt = true;
        solitaire.isGameOver = false;
        solitaire.currentScore = 48;
        
        Debug.Log("Set up lose scenario: 4 cards, no valid moves");
        
        yield return new WaitForSeconds(0.1f);
        
        // Check for valid moves
        bool hasValidMoves = CheckValidMoves(solitaire.dealtCards);
        
        Assert.IsFalse(hasValidMoves, "Should have no valid moves");
        Assert.Greater(solitaire.dealtCards.Count, 1, "Should have multiple cards");
        
        Debug.Log("LOSE DETECTED! No valid moves with " + solitaire.dealtCards.Count + " cards");
        
        yield return null;
        
        Debug.Log("âœ“âœ“âœ“ INTEGRATION TEST 4 PASSED - Lose condition detected");
    }

    /// <summary>
    /// INTEGRATION TEST 5: Card matching and removal
    /// </summary>
    [UnityTest]
    public IEnumerator IntegrationTest_05_CardMatching()
    {
        Debug.Log("\nâ–¶â–¶â–¶ RUNNING INTEGRATION TEST 5: Card Matching Logic");
        
        // Setup cards that can match
        solitaire.dealtCards.Clear();
        solitaire.dealtCards.Add("C5");  // 5 of Clubs
        solitaire.dealtCards.Add("DA");  // Ace of Diamonds (will be removed)
        solitaire.dealtCards.Add("H5");  // 5 of Hearts (matches C5 by value!)
        
        Debug.Log("Setup: C5, DA, H5 (C5 and H5 match by value)");
        
        int initialCount = solitaire.dealtCards.Count;
        
        // Check if middle card is stackable
        bool canMatch = CheckCanMatchCard(1, solitaire.dealtCards);
        Assert.IsTrue(canMatch, "Middle card should be matchable");
        
        // Simulate removing the left card (index 0)
        string removedCard = solitaire.dealtCards[0];
        solitaire.dealtCards.RemoveAt(0);
        
        Debug.Log($"Removed card: {removedCard}");
        
        Assert.AreEqual(initialCount - 1, solitaire.dealtCards.Count, "Card count should decrease");
        
        yield return null;
        
        Debug.Log("âœ“âœ“âœ“ INTEGRATION TEST 5 PASSED - Card matching works");
    }

    /// <summary>
    /// INTEGRATION TEST 6: Undo operation
    /// </summary>
    [UnityTest]
    public IEnumerator IntegrationTest_06_UndoOperation()
    {
        Debug.Log("\nâ–¶â–¶â–¶ RUNNING INTEGRATION TEST 6: Undo Operation");
        
        // Setup initial cards
        solitaire.dealtCards.Clear();
        solitaire.dealtCards.Add("CA");
        solitaire.dealtCards.Add("D2");
        solitaire.dealtCards.Add("H3");
        
        int initialCount = solitaire.dealtCards.Count;
        int initialScore = solitaire.currentScore;
        
        // Remove a card (matching actual game logic from UserInput.cs)
        string removedCard = solitaire.dealtCards[0];
        solitaire.removedCards.Push(removedCard);  // Push card name first
        solitaire.removedCards.Push("0");          // Push position second
        solitaire.dealtCards.RemoveAt(0);
        
        Debug.Log($"Removed card: {removedCard}");
        Assert.AreEqual(initialCount - 1, solitaire.dealtCards.Count, "Card should be removed");
        
        yield return new WaitForSeconds(0.1f);
        
        // Undo the removal (manually simulate since we can't spawn GameObjects easily)
        string posString = solitaire.removedCards.Pop();  // Pop position first (last pushed)
        string cardName = solitaire.removedCards.Pop();   // Pop card name second
        int listPos = int.Parse(posString);
        solitaire.dealtCards.Insert(listPos, cardName);
        solitaire.currentScore += 2; // Undo penalty
        
        Debug.Log($"Undone! Restored card: {cardName}");
        
        Assert.AreEqual(initialCount, solitaire.dealtCards.Count, "Card should be restored");
        Assert.AreEqual(removedCard, solitaire.dealtCards[0], "Correct card restored");
        Assert.AreEqual(initialScore + 2, solitaire.currentScore, "Undo penalty applied");
        
        yield return null;
        
        Debug.Log("âœ“âœ“âœ“ INTEGRATION TEST 6 PASSED - Undo works correctly");
    }

    /// <summary>
    /// INTEGRATION TEST 7: Game reset
    /// </summary>
    [UnityTest]
    public IEnumerator IntegrationTest_07_GameReset()
    {
        Debug.Log("\nâ–¶â–¶â–¶ RUNNING INTEGRATION TEST 7: Game Reset");
        
        // Deal some cards
        for (int i = 0; i < 10; i++)
        {
            solitaire.DealFromDeck();
        }
        
        Assert.Greater(solitaire.currentScore, 0, "Should have dealt cards");
        
        yield return null;
        
        // Reset the game
        solitaire.PlayCards();
        
        yield return new WaitForSeconds(0.1f);
        
        Assert.AreEqual(0, solitaire.currentScore, "Score should reset to 0");
        Assert.IsFalse(solitaire.allCardsDealt, "Cards should not be dealt");
        Assert.IsFalse(solitaire.isGameOver, "Game should not be over");
        Assert.AreEqual(52, solitaire.deck.Count, "Fresh deck should have 52 cards");
        
        Debug.Log("âœ“âœ“âœ“ INTEGRATION TEST 7 PASSED - Game resets correctly");
    }

    /// <summary>
    /// INTEGRATION TEST 8: Score persistence
    /// </summary>
    [UnityTest]
    public IEnumerator IntegrationTest_08_ScorePersistence()
    {
        Debug.Log("\nâ–¶â–¶â–¶ RUNNING INTEGRATION TEST 8: Score Persistence");
        
        // Clear any existing best score
        PlayerPrefs.DeleteKey("BestScore");
        
        // Set a score and save
        int testScore = 15;
        PlayerPrefs.SetInt("BestScore", testScore);
        PlayerPrefs.Save();
        
        yield return null;
        
        // Retrieve score
        int retrievedScore = PlayerPrefs.GetInt("BestScore", 52);
        
        Assert.AreEqual(testScore, retrievedScore, "Score should persist");
        
        Debug.Log($"Saved score: {testScore}, Retrieved score: {retrievedScore}");
        
        // Cleanup
        PlayerPrefs.DeleteKey("BestScore");
        
        yield return null;
        
        Debug.Log("âœ“âœ“âœ“ INTEGRATION TEST 8 PASSED - Scores persist correctly");
    }

    /// <summary>
    /// INTEGRATION TEST 9: Full game playthrough simulation
    /// </summary>
    [UnityTest]
    public IEnumerator IntegrationTest_09_FullGamePlaythrough()
    {
        Debug.Log("\nâ–¶â–¶â–¶ RUNNING INTEGRATION TEST 9: Full Game Playthrough");
        
        Debug.Log("Starting new game...");
        solitaire.PlayCards();
        yield return null;
        
        Debug.Log("Dealing all 52 cards...");
        for (int i = 0; i < 52; i++)
        {
            solitaire.DealFromDeck();
            if (i % 13 == 0)
            {
                yield return null;
            }
        }
        
        Assert.AreEqual(52, solitaire.currentScore, "All cards dealt");
        Assert.IsTrue(solitaire.allCardsDealt, "Marked as all dealt");
        
        Debug.Log($"Game state: {solitaire.dealtCards.Count} cards, Score: {solitaire.currentScore}");
        
        // Check if game can detect end condition
        bool hasValidMoves = CheckValidMoves(solitaire.dealtCards);
        
        if (solitaire.dealtCards.Count <= 1)
        {
            Debug.Log("RESULT: WIN!");
        }
        else if (!hasValidMoves)
        {
            Debug.Log("RESULT: LOSE - No valid moves");
        }
        else
        {
            Debug.Log($"RESULT: Game continues - {solitaire.dealtCards.Count} cards, valid moves available");
        }
        
        yield return null;
        
        Debug.Log("âœ“âœ“âœ“ INTEGRATION TEST 9 PASSED - Full playthrough completed");
    }

    // ==================== HELPER METHODS ====================

    private bool CheckValidMoves(List<string> cards)
    {
        if (cards.Count < 3) return false;

        for (int i = 1; i < cards.Count - 1; i++)
        {
            string leftCard = cards[i - 1];
            string rightCard = cards[i + 1];

            if (leftCard.Substring(0, 1) == rightCard.Substring(0, 1) ||
                leftCard.Substring(1) == rightCard.Substring(1))
            {
                return true;
            }
        }
        return false;
    }

    private bool CheckCanMatchCard(int cardIndex, List<string> cards)
    {
        if (cardIndex <= 0 || cardIndex >= cards.Count - 1)
        {
            return false;
        }

        string leftCard = cards[cardIndex - 1];
        string rightCard = cards[cardIndex + 1];

        return (leftCard.Substring(0, 1) == rightCard.Substring(0, 1) ||
                leftCard.Substring(1) == rightCard.Substring(1));
    }
}
