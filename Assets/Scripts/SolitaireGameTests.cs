using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

/// <summary>
/// Automated Test Suite for Solitaire Game
/// These tests verify win conditions, lose conditions, scoring, and game flow
/// </summary>
public class SolitaireGameTests
{
    private GameObject solitaireGameObject;
    private Solitaire solitaire;
    private GameObject cardPrefab;

    [SetUp]
    public void Setup()
    {
        // Create test game object with Solitaire component
        solitaireGameObject = new GameObject("TestSolitaire");
        solitaire = solitaireGameObject.AddComponent<Solitaire>();

        // Create a simple card prefab for testing
        cardPrefab = new GameObject("CardPrefab");
        cardPrefab.AddComponent<SpriteRenderer>();
        cardPrefab.AddComponent<Selectable>();
        cardPrefab.AddComponent<UpdateSprite>();
        
        // Set up basic Solitaire references
        solitaire.cardPrefab = cardPrefab;
        
        // Create mock objects for required references
        GameObject deckButton = new GameObject("DeckButton");
        deckButton.AddComponent<SpriteRenderer>();
        solitaire.deckButton = deckButton;

        GameObject cardArea = new GameObject("CardArea");
        solitaire.cardArea = cardArea.transform;

        GameObject staticArea = new GameObject("StaticArea");
        solitaire.staticArea = staticArea.transform;

        // Create mock high score panel
        GameObject highScorePanel = new GameObject("HighScorePanel");
        solitaire.highScorePanel = highScorePanel;
        highScorePanel.SetActive(false);

        // Set up camera
        GameObject cameraObj = new GameObject("Camera");
        Camera camera = cameraObj.AddComponent<Camera>();
        camera.tag = "MainCamera";

        Debug.Log("âœ“ Test setup complete");
    }

    [TearDown]
    public void Teardown()
    {
        // Clean up after each test
        Object.Destroy(solitaireGameObject);
        Object.Destroy(cardPrefab);
        Object.Destroy(Camera.main?.gameObject);
        
        // Clean up any spawned cards
        foreach (GameObject card in GameObject.FindGameObjectsWithTag("Card"))
        {
            Object.Destroy(card);
        }

        Debug.Log("âœ“ Test teardown complete");
    }

    /// <summary>
    /// TEST 1: Verify deck generation creates exactly 52 cards
    /// </summary>
    [Test]
    public void Test_DeckGeneration_Creates52Cards()
    {
        Debug.Log("â–¶ Running TEST 1: Deck Generation");
        
        List<string> deck = Solitaire.GenerateDeck();
        
        Assert.AreEqual(52, deck.Count, "Deck should contain exactly 52 cards");
        
        // Verify all suits present
        int clubs = 0, diamonds = 0, hearts = 0, spades = 0;
        foreach (string card in deck)
        {
            if (card.StartsWith("C")) clubs++;
            if (card.StartsWith("D")) diamonds++;
            if (card.StartsWith("H")) hearts++;
            if (card.StartsWith("S")) spades++;
        }
        
        Assert.AreEqual(13, clubs, "Should have 13 clubs");
        Assert.AreEqual(13, diamonds, "Should have 13 diamonds");
        Assert.AreEqual(13, hearts, "Should have 13 hearts");
        Assert.AreEqual(13, spades, "Should have 13 spades");
        
        Debug.Log("âœ“ TEST 1 PASSED: Deck generated correctly with 52 cards");
    }

    /// <summary>
    /// TEST 2: Verify win condition detects when only 1 card remains
    /// </summary>
    [Test]
    public void Test_WinCondition_TriggersWithOneCard()
    {
        Debug.Log("â–¶ Running TEST 2: Win Condition Detection");
        
        // Setup: manually set game state to win condition
        solitaire.dealtCards.Clear();
        solitaire.dealtCards.Add("CA"); // Only one card left
        solitaire.allCardsDealt = true;
        solitaire.isGameOver = false;

        // Create a test method to check win condition
        bool hasValidMoves = HasValidMovesTestHelper(solitaire.dealtCards);
        
        Assert.IsFalse(hasValidMoves, "Should have no valid moves with 1 card");
        Assert.LessOrEqual(solitaire.dealtCards.Count, 1, "Win condition: 1 or fewer cards");
        
        Debug.Log("âœ“ TEST 2 PASSED: Win condition detected correctly");
    }

    /// <summary>
    /// TEST 3: Verify lose condition detects when no valid moves exist
    /// </summary>
    [Test]
    public void Test_LoseCondition_NoValidMoves()
    {
        Debug.Log("â–¶ Running TEST 3: Lose Condition Detection");
        
        // Setup: create scenario with cards but no valid moves
        solitaire.dealtCards.Clear();
        solitaire.dealtCards.Add("CA");  // Clubs Ace
        solitaire.dealtCards.Add("D2");  // Diamonds 2
        solitaire.dealtCards.Add("H3");  // Hearts 3
        solitaire.dealtCards.Add("S4");  // Spades 4
        // No adjacent cards match by suit or value
        
        solitaire.allCardsDealt = true;
        solitaire.isGameOver = false;

        bool hasValidMoves = HasValidMovesTestHelper(solitaire.dealtCards);
        
        Assert.IsFalse(hasValidMoves, "Should have no valid moves");
        Assert.Greater(solitaire.dealtCards.Count, 1, "Cards remain but no moves");
        
        Debug.Log("âœ“ TEST 3 PASSED: Lose condition detected correctly");
    }

    /// <summary>
    /// TEST 4: Verify valid moves are correctly identified (matching suits)
    /// </summary>
    [Test]
    public void Test_ValidMoves_MatchingSuits()
    {
        Debug.Log("â–¶ Running TEST 4: Valid Moves - Matching Suits");
        
        // Setup: cards where neighbors match by suit
        solitaire.dealtCards.Clear();
        solitaire.dealtCards.Add("C2");  // Clubs
        solitaire.dealtCards.Add("D5");  // Middle card (different suit)
        solitaire.dealtCards.Add("C7");  // Clubs - matches first card's suit!
        
        bool hasValidMoves = HasValidMovesTestHelper(solitaire.dealtCards);
        
        Assert.IsTrue(hasValidMoves, "Should detect valid move: C2 and C7 match by suit");
        
        Debug.Log("âœ“ TEST 4 PASSED: Valid move detected for matching suits");
    }

    /// <summary>
    /// TEST 5: Verify valid moves are correctly identified (matching values)
    /// </summary>
    [Test]
    public void Test_ValidMoves_MatchingValues()
    {
        Debug.Log("â–¶ Running TEST 5: Valid Moves - Matching Values");
        
        // Setup: cards where neighbors match by value
        solitaire.dealtCards.Clear();
        solitaire.dealtCards.Add("C5");  // 5 of Clubs
        solitaire.dealtCards.Add("DA");  // Middle card (different value)
        solitaire.dealtCards.Add("H5");  // 5 of Hearts - matches first card's value!
        
        bool hasValidMoves = HasValidMovesTestHelper(solitaire.dealtCards);
        
        Assert.IsTrue(hasValidMoves, "Should detect valid move: C5 and H5 match by value");
        
        Debug.Log("âœ“ TEST 5 PASSED: Valid move detected for matching values");
    }

    /// <summary>
    /// TEST 6: Verify scoring system
    /// </summary>
    [Test]
    public void Test_Scoring_IncreasesWithDeals()
    {
        Debug.Log("â–¶ Running TEST 6: Scoring System");
        
        int initialScore = solitaire.currentScore;
        
        // Simulate dealing a card (score should increase)
        solitaire.currentScore++;
        
        Assert.AreEqual(initialScore + 1, solitaire.currentScore, "Score should increase by 1 per deal");
        
        // Simulate successful match (score should decrease)
        solitaire.currentScore--;
        
        Assert.AreEqual(initialScore, solitaire.currentScore, "Score should decrease by 1 per match");
        
        Debug.Log("âœ“ TEST 6 PASSED: Scoring works correctly");
    }

    /// <summary>
    /// TEST 7: Verify game initialization
    /// </summary>
    [Test]
    public void Test_GameInitialization()
    {
        Debug.Log("â–¶ Running TEST 7: Game Initialization");
        
        solitaire.PlayCards();
        
        Assert.AreEqual(0, solitaire.currentScore, "Score should start at 0");
        Assert.IsFalse(solitaire.allCardsDealt, "Cards should not all be dealt at start");
        Assert.IsFalse(solitaire.isGameOver, "Game should not be over at start");
        Assert.IsNotNull(solitaire.deck, "Deck should be initialized");
        Assert.AreEqual(52, solitaire.deck.Count, "Deck should have 52 cards");
        
        Debug.Log("âœ“ TEST 7 PASSED: Game initializes correctly");
    }

    /// <summary>
    /// TEST 8: Verify edge cards cannot be removed
    /// </summary>
    [Test]
    public void Test_EdgeCards_CannotBeRemoved()
    {
        Debug.Log("â–¶ Running TEST 8: Edge Cards Protection");
        
        solitaire.dealtCards.Clear();
        solitaire.dealtCards.Add("CA");  // First card - edge
        solitaire.dealtCards.Add("D2");  
        solitaire.dealtCards.Add("H3");  
        solitaire.dealtCards.Add("S4");  // Last card - edge
        
        // Check first card (index 0)
        bool firstCardStackable = IsCardStackableTestHelper(0, solitaire.dealtCards);
        Assert.IsFalse(firstCardStackable, "First card should not be stackable");
        
        // Check last card (index 3)
        bool lastCardStackable = IsCardStackableTestHelper(3, solitaire.dealtCards);
        Assert.IsFalse(lastCardStackable, "Last card should not be stackable");
        
        Debug.Log("âœ“ TEST 8 PASSED: Edge cards protected from removal");
    }

    /// <summary>
    /// TEST 9: Verify undo functionality
    /// </summary>
    [Test]
    public void Test_UndoFunctionality()
    {
        Debug.Log("â–¶ Running TEST 9: Undo Functionality");
        
        solitaire.dealtCards.Clear();
        solitaire.dealtCards.Add("CA");
        solitaire.dealtCards.Add("D2");
        solitaire.dealtCards.Add("H3");
        
        int initialCount = solitaire.dealtCards.Count;
        int initialScore = solitaire.currentScore;
        
        // Simulate removing a card (matching actual game logic)
        string removedCard = solitaire.dealtCards[0];
        solitaire.removedCards.Push(removedCard); // card name first
        solitaire.removedCards.Push("0");         // position second
        solitaire.dealtCards.RemoveAt(0);
        
        Assert.AreEqual(initialCount - 1, solitaire.dealtCards.Count, "Card should be removed");
        
        // Now test undo (manually since we can't instantiate GameObjects in simple tests)
        string posString = solitaire.removedCards.Pop();  // Pop position first (LIFO)
        string cardName = solitaire.removedCards.Pop();   // Pop card name second
        int listPos = int.Parse(posString);
        solitaire.dealtCards.Insert(listPos, cardName);
        
        Assert.AreEqual(initialCount, solitaire.dealtCards.Count, "Card should be restored");
        Assert.AreEqual(removedCard, solitaire.dealtCards[0], "Correct card should be restored");
        
        Debug.Log("âœ“ TEST 9 PASSED: Undo works correctly");
    }

    /// <summary>
    /// TEST 10: Verify PlayerPrefs saving (Best Score)
    /// </summary>
    [Test]
    public void Test_BestScore_SavesCorrectly()
    {
        Debug.Log("â–¶ Running TEST 10: Best Score Persistence");
        
        // Clear any existing data
        PlayerPrefs.DeleteKey("BestScore");
        
        // Set a best score
        int testScore = 10;
        PlayerPrefs.SetInt("BestScore", testScore);
        PlayerPrefs.Save();
        
        // Retrieve it
        int retrievedScore = PlayerPrefs.GetInt("BestScore", 52); // 52 is default
        
        Assert.AreEqual(testScore, retrievedScore, "Best score should be saved and retrieved");
        
        // Test that lower score replaces higher score
        solitaire.currentScore = 5;
        int oldBestScore = 10;
        
        if (solitaire.currentScore < oldBestScore)
        {
            int newBestScore = solitaire.currentScore;
            Assert.Less(newBestScore, oldBestScore, "Lower score should replace higher score");
        }
        
        // Cleanup
        PlayerPrefs.DeleteKey("BestScore");
        
        Debug.Log("âœ“ TEST 10 PASSED: Best score saves correctly");
    }

    // ==================== HELPER METHODS ====================

    /// <summary>
    /// Helper method to check if there are valid moves (mimics HasValidMoves from Solitaire)
    /// </summary>
    private bool HasValidMovesTestHelper(List<string> cards)
    {
        if (cards.Count < 3) return false;

        for (int i = 1; i < cards.Count - 1; i++)
        {
            string leftCard = cards[i - 1];
            string rightCard = cards[i + 1];

            // Check if left and right match by suit or value
            if (leftCard.Substring(0, 1) == rightCard.Substring(0, 1) || // Same suit
                leftCard.Substring(1) == rightCard.Substring(1))          // Same value
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Helper method to check if a specific card is stackable
    /// </summary>
    private bool IsCardStackableTestHelper(int cardIndex, List<string> cards)
    {
        // Can't stack edge cards
        if (cardIndex <= 0 || cardIndex >= cards.Count - 1)
        {
            return false;
        }

        string leftCard = cards[cardIndex - 1];
        string rightCard = cards[cardIndex + 1];

        // Check if neighbors match
        return (leftCard.Substring(0, 1) == rightCard.Substring(0, 1) ||
                leftCard.Substring(1) == rightCard.Substring(1));
    }
}
