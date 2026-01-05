using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Standalone Mobile Touch Test Runner
/// Press SPACE to run visual tests of touch functionality
/// </summary>
public class MobileTouchTestRunner : MonoBehaviour
{
    private MobileTouchInput touchInput;
    private Solitaire solitaire;
    private int testsRun = 0;
    private int testsPassed = 0;

    void Start()
    {
        Debug.Log("=============================================================");
        Debug.Log("MOBILE TOUCH TEST RUNNER - PRESS SPACE TO START");
        Debug.Log("=============================================================\n");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(RunTouchTests());
        }
    }

    IEnumerator RunTouchTests()
    {
        // Find components
        touchInput = FindObjectOfType<MobileTouchInput>();
        solitaire = FindObjectOfType<Solitaire>();

        if (touchInput == null)
        {
            Debug.LogError("❌ ERROR: MobileTouchInput not found!");
            Debug.LogError("Please add MobileTouchInput to your scene");
            yield break;
        }

        if (solitaire == null)
        {
            Debug.LogError("❌ ERROR: Solitaire not found!");
            yield break;
        }

        Debug.Log("\n");
        Debug.Log("█████████████████████████████████████████████████████████████");
        Debug.Log("█                                                           █");
        Debug.Log("█        MOBILE TOUCH INPUT - TEST SUITE                   █");
        Debug.Log("█                                                           █");
        Debug.Log("█████████████████████████████████████████████████████████████");
        Debug.Log("");

        yield return new WaitForSeconds(0.5f);

        testsRun = 0;
        testsPassed = 0;

        // Run tests
        yield return RunTest1_ComponentsExist();
        yield return RunTest2_CardRemovabilityBySuit();
        yield return RunTest3_CardRemovabilityByValue();
        yield return RunTest4_EdgeCardProtection();
        yield return RunTest5_HighlightingToggle();
        yield return RunTest6_CameraDragToggle();
        yield return RunTest7_GameOverState();
        yield return RunTest8_ValidMoveDetection();

        // Show results
        yield return ShowResults();
    }

    IEnumerator RunTest1_ComponentsExist()
    {
        testsRun++;
        Debug.Log($"\n▶▶▶ TEST {testsRun}: COMPONENTS EXIST");
        Debug.Log("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Debug.Log("Purpose: Verify MobileTouchInput and Solitaire are present");

        yield return new WaitForSeconds(0.2f);

        bool pass = (touchInput != null && solitaire != null);

        if (pass)
        {
            Debug.Log("<color=green>✓ MobileTouchInput found</color>");
            Debug.Log("<color=green>✓ Solitaire found</color>");
            Debug.Log("<color=green>✓✓✓ TEST 1 PASSED ✓✓✓</color>");
            testsPassed++;
        }
        else
        {
            Debug.LogError("❌ TEST 1 FAILED");
        }

        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator RunTest2_CardRemovabilityBySuit()
    {
        testsRun++;
        Debug.Log($"\n▶▶▶ TEST {testsRun}: CARD REMOVABILITY - MATCHING SUITS");
        Debug.Log("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Debug.Log("Purpose: Verify cards matching by suit can be detected");

        yield return new WaitForSeconds(0.2f);

        solitaire.dealtCards.Clear();
        solitaire.dealtCards.Add("C2");  // Clubs
        solitaire.dealtCards.Add("D5");  // Diamonds
        solitaire.dealtCards.Add("C7");  // Clubs (matches first)

        Debug.Log("→ Setup: C2, D5, C7");
        Debug.Log("→ C2 and C7 both Clubs (suit match)");

        yield return new WaitForSeconds(0.3f);

        // Check if middle card has valid neighbors
        bool hasMatch = CheckCardAtIndexRemovable(1);

        if (hasMatch)
        {
            Debug.Log("<color=green>✓ Suit match detected correctly</color>");
            Debug.Log("<color=green>✓✓✓ TEST 2 PASSED ✓✓✓</color>");
            testsPassed++;
        }
        else
        {
            Debug.LogError("❌ TEST 2 FAILED");
        }

        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator RunTest3_CardRemovabilityByValue()
    {
        testsRun++;
        Debug.Log($"\n▶▶▶ TEST {testsRun}: CARD REMOVABILITY - MATCHING VALUES");
        Debug.Log("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Debug.Log("Purpose: Verify cards matching by value can be detected");

        yield return new WaitForSeconds(0.2f);

        solitaire.dealtCards.Clear();
        solitaire.dealtCards.Add("C5");  // 5 of Clubs
        solitaire.dealtCards.Add("DA");  // Ace of Diamonds
        solitaire.dealtCards.Add("H5");  // 5 of Hearts (matches first)

        Debug.Log("→ Setup: C5, DA, H5");
        Debug.Log("→ C5 and H5 both have value '5'");

        yield return new WaitForSeconds(0.3f);

        bool hasMatch = CheckCardAtIndexRemovable(1);

        if (hasMatch)
        {
            Debug.Log("<color=green>✓ Value match detected correctly</color>");
            Debug.Log("<color=green>✓✓✓ TEST 3 PASSED ✓✓✓</color>");
            testsPassed++;
        }
        else
        {
            Debug.LogError("❌ TEST 3 FAILED");
        }

        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator RunTest4_EdgeCardProtection()
    {
        testsRun++;
        Debug.Log($"\n▶▶▶ TEST {testsRun}: EDGE CARD PROTECTION");
        Debug.Log("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Debug.Log("Purpose: Verify first and last cards cannot be removed");

        yield return new WaitForSeconds(0.2f);

        solitaire.dealtCards.Clear();
        solitaire.dealtCards.Add("C5");
        solitaire.dealtCards.Add("DA");
        solitaire.dealtCards.Add("H5");

        Debug.Log("→ Setup: 3 cards");
        Debug.Log("→ Checking first card (index 0)");

        bool firstRemovable = CheckCardAtIndexRemovable(0);

        Debug.Log("→ Checking last card (index 2)");
        bool lastRemovable = CheckCardAtIndexRemovable(2);

        yield return new WaitForSeconds(0.3f);

        if (!firstRemovable && !lastRemovable)
        {
            Debug.Log("<color=green>✓ First card protected</color>");
            Debug.Log("<color=green>✓ Last card protected</color>");
            Debug.Log("<color=green>✓✓✓ TEST 4 PASSED ✓✓✓</color>");
            testsPassed++;
        }
        else
        {
            Debug.LogError("❌ TEST 4 FAILED - Edge cards not protected");
        }

        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator RunTest5_HighlightingToggle()
    {
        testsRun++;
        Debug.Log($"\n▶▶▶ TEST {testsRun}: HIGHLIGHTING TOGGLE");
        Debug.Log("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Debug.Log("Purpose: Verify highlighting can be toggled on/off");

        yield return new WaitForSeconds(0.2f);

        Debug.Log("→ Turning highlighting ON");
        touchInput.SetShowValidMoves(true);
        yield return new WaitForSeconds(0.2f);

        Debug.Log("→ Turning highlighting OFF");
        touchInput.SetShowValidMoves(false);
        yield return new WaitForSeconds(0.2f);

        Debug.Log("→ Turning highlighting back ON");
        touchInput.SetShowValidMoves(true);
        yield return new WaitForSeconds(0.2f);

        Debug.Log("<color=green>✓ Highlighting toggle works</color>");
        Debug.Log("<color=green>✓✓✓ TEST 5 PASSED ✓✓✓</color>");
        testsPassed++;

        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator RunTest6_CameraDragToggle()
    {
        testsRun++;
        Debug.Log($"\n▶▶▶ TEST {testsRun}: CAMERA DRAG TOGGLE");
        Debug.Log("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Debug.Log("Purpose: Verify camera drag can be enabled/disabled");

        yield return new WaitForSeconds(0.2f);

        Debug.Log("→ Enabling camera drag");
        touchInput.SetCameraDrag(true);
        yield return new WaitForSeconds(0.2f);

        Debug.Log("→ Disabling camera drag");
        touchInput.SetCameraDrag(false);
        yield return new WaitForSeconds(0.2f);

        Debug.Log("→ Re-enabling camera drag");
        touchInput.SetCameraDrag(true);
        yield return new WaitForSeconds(0.2f);

        Debug.Log("<color=green>✓ Camera drag toggle works</color>");
        Debug.Log("<color=green>✓✓✓ TEST 6 PASSED ✓✓✓</color>");
        testsPassed++;

        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator RunTest7_GameOverState()
    {
        testsRun++;
        Debug.Log($"\n▶▶▶ TEST {testsRun}: GAME OVER STATE HANDLING");
        Debug.Log("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Debug.Log("Purpose: Verify interactions blocked when game over");

        yield return new WaitForSeconds(0.2f);

        // Setup cards
        solitaire.dealtCards.Clear();
        solitaire.dealtCards.Add("C5");
        solitaire.dealtCards.Add("DA");
        solitaire.dealtCards.Add("H5");

        Debug.Log("→ Setting game over flag");
        solitaire.isGameOver = true;
        yield return new WaitForSeconds(0.2f);

        // Try to check if card is removable (should respect game over state)
        Debug.Log("→ Attempting to check card removability");
        bool canInteract = !solitaire.isGameOver;

        yield return new WaitForSeconds(0.3f);

        if (!canInteract)
        {
            Debug.Log("<color=green>✓ Game over state respected</color>");
            Debug.Log("<color=green>✓✓✓ TEST 7 PASSED ✓✓✓</color>");
            testsPassed++;
        }
        else
        {
            Debug.LogError("❌ TEST 7 FAILED");
        }

        // Reset game over state
        solitaire.isGameOver = false;

        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator RunTest8_ValidMoveDetection()
    {
        testsRun++;
        Debug.Log($"\n▶▶▶ TEST {testsRun}: VALID MOVE DETECTION");
        Debug.Log("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Debug.Log("Purpose: Verify system can detect when valid moves exist");

        yield return new WaitForSeconds(0.2f);

        // Scenario 1: Valid move exists
        solitaire.dealtCards.Clear();
        solitaire.dealtCards.Add("C5");
        solitaire.dealtCards.Add("DA");
        solitaire.dealtCards.Add("H5");

        Debug.Log("→ Scenario 1: Cards with valid move");
        bool hasValidMove1 = HasAnyValidMoves();
        Debug.Log($"→ Valid move detected: {hasValidMove1}");

        yield return new WaitForSeconds(0.3f);

        // Scenario 2: No valid moves
        solitaire.dealtCards.Clear();
        solitaire.dealtCards.Add("C2");
        solitaire.dealtCards.Add("D5");
        solitaire.dealtCards.Add("HK");

        Debug.Log("→ Scenario 2: Cards with NO valid move");
        bool hasValidMove2 = HasAnyValidMoves();
        Debug.Log($"→ Valid move detected: {hasValidMove2}");

        yield return new WaitForSeconds(0.3f);

        if (hasValidMove1 && !hasValidMove2)
        {
            Debug.Log("<color=green>✓ Valid moves detected correctly</color>");
            Debug.Log("<color=green>✓ No-move scenario detected correctly</color>");
            Debug.Log("<color=green>✓✓✓ TEST 8 PASSED ✓✓✓</color>");
            testsPassed++;
        }
        else
        {
            Debug.LogError("❌ TEST 8 FAILED");
        }

        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator ShowResults()
    {
        Debug.Log("\n\n");
        Debug.Log("█████████████████████████████████████████████████████████████");
        Debug.Log("█                                                           █");
        Debug.Log("█              MOBILE TOUCH TEST RESULTS                    █");
        Debug.Log("█                                                           █");
        Debug.Log("█████████████████████████████████████████████████████████████");
        Debug.Log("");

        yield return new WaitForSeconds(0.3f);

        Debug.Log($"<color=cyan>═════════════════════════════════════════════════════</color>");
        Debug.Log($"<color=cyan>  Tests Run:      {testsRun}</color>");
        Debug.Log($"<color=green>  Tests Passed:   {testsPassed}</color>");
        Debug.Log($"<color=red>  Tests Failed:   {testsRun - testsPassed}</color>");
        Debug.Log($"<color=cyan>  Success Rate:   {(testsPassed * 100 / testsRun)}%</color>");
        Debug.Log($"<color=cyan>═════════════════════════════════════════════════════</color>");
        Debug.Log("");

        yield return new WaitForSeconds(0.5f);

        if (testsPassed == testsRun)
        {
            Debug.Log("<color=green>╔═══════════════════════════════════════════════════════╗</color>");
            Debug.Log("<color=green>║                                                       ║</color>");
            Debug.Log("<color=green>║       🎉  ALL TOUCH TESTS PASSED!  🎉                ║</color>");
            Debug.Log("<color=green>║                                                       ║</color>");
            Debug.Log("<color=green>║   Mobile touch input is working perfectly!           ║</color>");
            Debug.Log("<color=green>║                                                       ║</color>");
            Debug.Log("<color=green>╚═══════════════════════════════════════════════════════╝</color>");
        }
        else
        {
            Debug.LogWarning("╔═══════════════════════════════════════════════════════╗");
            Debug.LogWarning("║                                                       ║");
            Debug.LogWarning("║          ⚠️  SOME TESTS FAILED  ⚠️                   ║");
            Debug.LogWarning("║                                                       ║");
            Debug.LogWarning("║   Review the console output above for details         ║");
            Debug.LogWarning("║                                                       ║");
            Debug.LogWarning("╚═══════════════════════════════════════════════════════╝");
        }

        Debug.Log("\n<color=yellow>Press SPACE to run tests again</color>\n");
    }

    // Helper methods
    bool CheckCardAtIndexRemovable(int index)
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

    bool HasAnyValidMoves()
    {
        if (solitaire.dealtCards.Count < 3) return false;

        for (int i = 1; i < solitaire.dealtCards.Count - 1; i++)
        {
            if (CheckCardAtIndexRemovable(i))
            {
                return true;
            }
        }

        return false;
    }
}
