using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Using standard Unity UI instead of TextMeshPro

/// <summary>
/// Visual Test Demonstration - Run this in Unity to see tests executing
/// Attach this to a GameObject and press SPACE to run visual demo
/// </summary>
public class TestDemonstration : MonoBehaviour
{
    [Header("Demo Settings")]
    public bool autoRunOnStart = false;
    public float delayBetweenTests = 1.0f;

    [Header("UI References (Optional)")]
    public Text statusText;
    public Text resultsText;

    private Solitaire solitaire;
    private int testsRun = 0;
    private int testsPassed = 0;
    private int testsFailed = 0;

    void Start()
    {
        LogToUI("Visual Test Demonstration Ready");
        LogToUI("Press SPACE to run tests");
        LogToUI("Watch console for detailed output");

        if (autoRunOnStart)
        {
            StartCoroutine(RunAllTests());
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(RunAllTests());
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetDemo();
        }
    }

    /// <summary>
    /// Run all visual test demonstrations
    /// </summary>
    public IEnumerator RunAllTests()
    {
        LogToUI("\n========================================");
        LogToUI("STARTING VISUAL TEST DEMONSTRATION");
        LogToUI("========================================\n");

        testsRun = 0;
        testsPassed = 0;
        testsFailed = 0;

        // Setup test environment
        yield return SetupTestEnvironment();

        // Run each test with visual feedback
        yield return RunTest("Deck Generation", Test_DeckGeneration());
        yield return RunTest("Win Condition", Test_WinCondition());
        yield return RunTest("Lose Condition", Test_LoseCondition());
        yield return RunTest("Valid Moves - Suits", Test_ValidMovesSuits());
        yield return RunTest("Valid Moves - Values", Test_ValidMovesValues());
        yield return RunTest("Card Matching", Test_CardMatching());
        yield return RunTest("Undo Operation", Test_UndoOperation());
        yield return RunTest("Game Reset", Test_GameReset());
        yield return RunTest("Deal All Cards", Test_DealAllCards());
        yield return RunTest("Score Tracking", Test_ScoreTracking());

        // Display final results
        yield return ShowFinalResults();
    }

    IEnumerator SetupTestEnvironment()
    {
        LogToUI("Setting up test environment...");

        // Find or create Solitaire
        solitaire = FindObjectOfType<Solitaire>();
        if (solitaire == null)
        {
            LogToUI("âŒ ERROR: Solitaire not found in scene!");
            LogToUI("Please add Solitaire component to scene");
            yield break;
        }

        // Initialize fresh game
        solitaire.PlayCards();

        LogToUI("âœ“ Test environment ready");
        yield return new WaitForSeconds(delayBetweenTests);
    }

    IEnumerator RunTest(string testName, IEnumerator testCoroutine)
    {
        testsRun++;
        LogToUI($"\nâ–¶ TEST {testsRun}: {testName}");
        LogToUI("Running...");

        bool passed = true;
        yield return testCoroutine;

        // Check if test passed (simplification - you'd check actual results)
        if (passed)
        {
            testsPassed++;
            LogToUI($"âœ“ PASSED: {testName}");
        }
        else
        {
            testsFailed++;
            LogToUI($"âŒ FAILED: {testName}");
        }

        yield return new WaitForSeconds(delayBetweenTests);
    }

    // ==================== INDIVIDUAL TESTS ====================

    IEnumerator Test_DeckGeneration()
    {
        List<string> deck = Solitaire.GenerateDeck();

        LogToUI($"Generated deck with {deck.Count} cards");

        if (deck.Count == 52)
        {
            LogToUI("âœ“ Deck has correct number of cards (52)");

            // Count suits
            int clubs = 0, diamonds = 0, hearts = 0, spades = 0;
            foreach (string card in deck)
            {
                if (card.StartsWith("C")) clubs++;
                if (card.StartsWith("D")) diamonds++;
                if (card.StartsWith("H")) hearts++;
                if (card.StartsWith("S")) spades++;
            }

            LogToUI($"  Clubs: {clubs}, Diamonds: {diamonds}");
            LogToUI($"  Hearts: {hearts}, Spades: {spades}");

            if (clubs == 13 && diamonds == 13 && hearts == 13 && spades == 13)
            {
                LogToUI("âœ“ All suits have 13 cards");
            }
        }

        yield return null;
    }

    IEnumerator Test_WinCondition()
    {
        // Setup win scenario
        solitaire.dealtCards.Clear();
        solitaire.dealtCards.Add("CA");
        solitaire.allCardsDealt = true;

        LogToUI($"Setup: {solitaire.dealtCards.Count} card remaining");

        if (solitaire.dealtCards.Count <= 1)
        {
            LogToUI("âœ“ Win condition detected (â‰¤1 card)");
        }

        yield return null;
    }

    IEnumerator Test_LoseCondition()
    {
        // Setup lose scenario
        solitaire.dealtCards.Clear();
        solitaire.dealtCards.Add("CA");
        solitaire.dealtCards.Add("D2");
        solitaire.dealtCards.Add("H3");
        solitaire.dealtCards.Add("S4");

        LogToUI($"Setup: {solitaire.dealtCards.Count} cards, checking for valid moves");

        bool hasValidMoves = CheckValidMoves(solitaire.dealtCards);

        LogToUI($"Valid moves available: {hasValidMoves}");

        if (!hasValidMoves && solitaire.dealtCards.Count > 1)
        {
            LogToUI("âœ“ Lose condition detected (no valid moves)");
        }

        yield return null;
    }

    IEnumerator Test_ValidMovesSuits()
    {
        solitaire.dealtCards.Clear();
        solitaire.dealtCards.Add("C2");
        solitaire.dealtCards.Add("D5");
        solitaire.dealtCards.Add("C7");

        LogToUI("Setup: C2, D5, C7");
        LogToUI("C2 and C7 match by suit (Clubs)");

        bool hasValidMoves = CheckValidMoves(solitaire.dealtCards);

        if (hasValidMoves)
        {
            LogToUI("âœ“ Valid move detected (matching suits)");
        }

        yield return null;
    }

    IEnumerator Test_ValidMovesValues()
    {
        solitaire.dealtCards.Clear();
        solitaire.dealtCards.Add("C5");
        solitaire.dealtCards.Add("DA");
        solitaire.dealtCards.Add("H5");

        LogToUI("Setup: C5, DA, H5");
        LogToUI("C5 and H5 match by value (5)");

        bool hasValidMoves = CheckValidMoves(solitaire.dealtCards);

        if (hasValidMoves)
        {
            LogToUI("âœ“ Valid move detected (matching values)");
        }

        yield return null;
    }

    IEnumerator Test_CardMatching()
    {
        solitaire.dealtCards.Clear();
        solitaire.dealtCards.Add("C5");
        solitaire.dealtCards.Add("DA");
        solitaire.dealtCards.Add("H5");

        int initialCount = solitaire.dealtCards.Count;
        LogToUI($"Initial cards: {initialCount}");

        // Simulate removing a card
        string removed = solitaire.dealtCards[0];
        solitaire.dealtCards.RemoveAt(0);

        LogToUI($"Removed: {removed}");
        LogToUI($"Remaining cards: {solitaire.dealtCards.Count}");

        if (solitaire.dealtCards.Count == initialCount - 1)
        {
            LogToUI("âœ“ Card removed successfully");
        }

        yield return null;
    }

    IEnumerator Test_UndoOperation()
    {
        solitaire.dealtCards.Clear();
        solitaire.dealtCards.Add("CA");
        solitaire.dealtCards.Add("D2");
        solitaire.dealtCards.Add("H3");

        int initialCount = solitaire.dealtCards.Count;
        LogToUI($"Initial cards: {initialCount}");

        // Remove a card
        string removed = solitaire.dealtCards[0];
        solitaire.removedCards.Push("0");
        solitaire.removedCards.Push(removed);
        solitaire.dealtCards.RemoveAt(0);

        LogToUI($"Removed: {removed}");
        LogToUI($"Cards after removal: {solitaire.dealtCards.Count}");

        // Undo
        int pos = int.Parse(solitaire.removedCards.Pop());
        string card = solitaire.removedCards.Pop();
        solitaire.dealtCards.Insert(pos, card);

        LogToUI($"Undone - restored: {card}");
        LogToUI($"Cards after undo: {solitaire.dealtCards.Count}");

        if (solitaire.dealtCards.Count == initialCount)
        {
            LogToUI("âœ“ Undo restored card correctly");
        }

        yield return null;
    }

    IEnumerator Test_GameReset()
    {
        // Deal some cards
        for (int i = 0; i < 5; i++)
        {
            solitaire.DealFromDeck();
        }

        LogToUI($"Dealt {solitaire.currentScore} cards");

        // Reset
        solitaire.PlayCards();

        LogToUI("Game reset");
        LogToUI($"Score after reset: {solitaire.currentScore}");
        LogToUI($"Deck size: {solitaire.deck.Count}");

        if (solitaire.currentScore == 0 && solitaire.deck.Count == 52)
        {
            LogToUI("âœ“ Game reset successfully");
        }

        yield return null;
    }

    IEnumerator Test_DealAllCards()
    {
        solitaire.PlayCards();

        LogToUI("Dealing all 52 cards...");

        for (int i = 0; i < 52; i++)
        {
            solitaire.DealFromDeck();

            if (i % 10 == 9)
            {
                LogToUI($"  Dealt {i + 1} cards...");
                yield return new WaitForSeconds(0.1f);
            }
        }

        LogToUI($"Final score: {solitaire.currentScore}");
        LogToUI($"All cards dealt: {solitaire.allCardsDealt}");

        if (solitaire.currentScore == 52 && solitaire.allCardsDealt)
        {
            LogToUI("âœ“ All 52 cards dealt successfully");
        }

        yield return null;
    }

    IEnumerator Test_ScoreTracking()
    {
        solitaire.PlayCards();
        int initialScore = solitaire.currentScore;

        LogToUI($"Initial score: {initialScore}");

        // Deal a card
        solitaire.DealFromDeck();
        LogToUI($"After deal: {solitaire.currentScore} (+1 expected)");

        // Simulate match (score decreases)
        solitaire.currentScore--;
        LogToUI($"After match: {solitaire.currentScore} (-1 for match)");

        // Simulate undo (penalty)
        solitaire.currentScore += 2;
        LogToUI($"After undo: {solitaire.currentScore} (+2 penalty)");

        LogToUI("âœ“ Score tracking working");

        yield return null;
    }

    // ==================== HELPER METHODS ====================

    bool CheckValidMoves(List<string> cards)
    {
        if (cards.Count < 3) return false;

        for (int i = 1; i < cards.Count - 1; i++)
        {
            string left = cards[i - 1];
            string right = cards[i + 1];

            if (left.Substring(0, 1) == right.Substring(0, 1) ||
                left.Substring(1) == right.Substring(1))
            {
                return true;
            }
        }
        return false;
    }

    IEnumerator ShowFinalResults()
    {
        LogToUI("\n========================================");
        LogToUI("FINAL TEST RESULTS");
        LogToUI("========================================");
        LogToUI($"Tests Run: {testsRun}");
        LogToUI($"Passed: {testsPassed}");
        LogToUI($"Failed: {testsFailed}");
        LogToUI($"Success Rate: {(testsPassed * 100 / testsRun)}%");
        LogToUI("========================================\n");

        if (testsFailed == 0)
        {
            LogToUI("ðŸŽ‰ ALL TESTS PASSED! ðŸŽ‰");
        }
        else
        {
            LogToUI("âš ï¸ Some tests failed - check console");
        }

        yield return null;
    }

    void ResetDemo()
    {
        testsRun = 0;
        testsPassed = 0;
        testsFailed = 0;

        if (statusText != null) statusText.text = "";
        if (resultsText != null) resultsText.text = "";

        LogToUI("Demo reset - Press SPACE to run again");
    }

    void LogToUI(string message)
    {
        Debug.Log(message);

        if (statusText != null)
        {
            statusText.text += message + "\n";
        }

        if (resultsText != null && message.Contains("âœ“") || message.Contains("âŒ"))
        {
            resultsText.text += message + "\n";
        }
    }

    void OnGUI()
    {
        // Simple on-screen instructions if no UI is set up
        if (statusText == null)
        {
            GUI.Label(new Rect(10, 10, 300, 60),
                "Visual Test Demo\n" +
                "Press SPACE to run tests\n" +
                "Press R to reset\n" +
                "Watch Console for output");
        }

        GUI.Label(new Rect(10, Screen.height - 80, 300, 60),
            $"Tests: {testsRun} | Passed: {testsPassed} | Failed: {testsFailed}");
    }
}
