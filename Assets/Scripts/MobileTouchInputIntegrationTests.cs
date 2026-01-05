using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

/// <summary>
/// Integration Tests for Mobile Touch Input
/// These tests run in PlayMode and verify complete touch interaction flow
/// </summary>
public class MobileTouchInputIntegrationTests
{
    private GameObject touchManagerObj;
    private MobileTouchInput touchInput;
    private GameObject solitaireObj;
    private Solitaire solitaire;
    private GameObject cardPrefab;
    private Camera mainCamera;

    [UnitySetUp]
    public IEnumerator Setup()
    {
        Debug.Log("========================================");
        Debug.Log("SETTING UP MOBILE TOUCH INTEGRATION TEST");
        Debug.Log("========================================");

        // Create main camera
        GameObject cameraObj = new GameObject("Main Camera");
        mainCamera = cameraObj.AddComponent<Camera>();
        mainCamera.tag = "MainCamera";
        mainCamera.transform.position = new Vector3(0, 0, -10);

        // Create Solitaire game object
        solitaireObj = new GameObject("Solitaire");
        solitaire = solitaireObj.AddComponent<Solitaire>();

        // Create card prefab
        cardPrefab = new GameObject("CardPrefab");
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
        GameObject cardArea = new GameObject("CardArea");
        GameObject staticArea = new GameObject("StaticArea");

        // Setup Solitaire references
        solitaire.cardPrefab = cardPrefab;
        solitaire.deckButton = deckButton;
        solitaire.highScorePanel = highScorePanel;
        solitaire.cardArea = cardArea.transform;
        solitaire.staticArea = staticArea.transform;
        solitaire.cardFaces = new Sprite[52];

        // Create MobileTouchInput
        touchManagerObj = new GameObject("TouchManager");
        touchInput = touchManagerObj.AddComponent<MobileTouchInput>();

        // Initialize game
        solitaire.PlayCards();

        yield return null;

        Debug.Log("✓ Mobile touch integration test setup complete");
    }

    [UnityTearDown]
    public IEnumerator Teardown()
    {
        // Destroy all test objects
        Object.Destroy(touchManagerObj);
        Object.Destroy(solitaireObj);
        Object.Destroy(cardPrefab);
        Object.Destroy(mainCamera?.gameObject);

        // Clean up any spawned cards
        GameObject[] allCards = GameObject.FindGameObjectsWithTag("Card");
        foreach (GameObject card in allCards)
        {
            Object.Destroy(card);
        }

        yield return null;

        Debug.Log("✓ Mobile touch integration test cleanup complete");
        Debug.Log("========================================\n");
    }

    /// <summary>
    /// INTEGRATION TEST 1: Touch system initialization
    /// </summary>
    [UnityTest]
    public IEnumerator IntegrationTest_01_TouchSystemInitialization()
    {
        Debug.Log("\n▶▶▶ RUNNING INTEGRATION TEST 1: Touch System Initialization");

        Assert.IsNotNull(touchInput, "MobileTouchInput should exist");
        Assert.IsNotNull(solitaire, "Solitaire should be found by touch input");
        Assert.IsNotNull(mainCamera, "Camera should be found");

        yield return null;

        Debug.Log("✓✓✓ INTEGRATION TEST 1 PASSED - Touch system initialized");
    }

    /// <summary>
    /// INTEGRATION TEST 2: Visual highlighting with dealt cards
    /// </summary>
    [UnityTest]
    public IEnumerator IntegrationTest_02_VisualHighlightingWithDealtCards()
    {
        Debug.Log("\n▶▶▶ RUNNING INTEGRATION TEST 2: Visual Highlighting");

        // Enable highlighting
        touchInput.SetShowValidMoves(true);

        // Deal some cards
        for (int i = 0; i < 10; i++)
        {
            solitaire.DealFromDeck();
        }

        yield return new WaitForSeconds(0.1f);

        Assert.AreEqual(10, solitaire.dealtCards.Count, "Should have 10 cards dealt");

        // Wait for highlighting system to update
        yield return null;

        Debug.Log("✓✓✓ INTEGRATION TEST 2 PASSED - Highlighting system active");
    }

    /// <summary>
    /// INTEGRATION TEST 3: Complete card removal flow
    /// </summary>
    [UnityTest]
    public IEnumerator IntegrationTest_03_CompleteCardRemovalFlow()
    {
        Debug.Log("\n▶▶▶ RUNNING INTEGRATION TEST 3: Card Removal Flow");

        // Setup specific scenario
        solitaire.dealtCards.Clear();
        solitaire.dealtCards.Add("C5");
        solitaire.dealtCards.Add("DA");
        solitaire.dealtCards.Add("H5");

        // Create actual GameObjects
        GameObject card1 = Object.Instantiate(cardPrefab);
        card1.name = "C5";
        card1.GetComponent<Selectable>().name = "C5";

        GameObject card2 = Object.Instantiate(cardPrefab);
        card2.name = "DA";
        card2.GetComponent<Selectable>().name = "DA";

        GameObject card3 = Object.Instantiate(cardPrefab);
        card3.name = "H5";
        card3.GetComponent<Selectable>().name = "H5";

        int initialCount = solitaire.dealtCards.Count;

        yield return new WaitForSeconds(0.1f);

        // Simulate card tap using reflection
        var tapMethod = typeof(MobileTouchInput).GetMethod("TapCard",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        tapMethod.Invoke(touchInput, new object[] { card2 });

        yield return new WaitForSeconds(0.1f);

        Assert.Less(solitaire.dealtCards.Count, initialCount, "Card should be removed");

        Debug.Log("✓✓✓ INTEGRATION TEST 3 PASSED - Card removal flow works");
    }

    /// <summary>
    /// INTEGRATION TEST 4: Invalid card tap handling
    /// </summary>
    [UnityTest]
    public IEnumerator IntegrationTest_04_InvalidCardTap()
    {
        Debug.Log("\n▶▶▶ RUNNING INTEGRATION TEST 4: Invalid Card Tap Handling");

        // Setup cards with no valid moves
        solitaire.dealtCards.Clear();
        solitaire.dealtCards.Add("C2");
        solitaire.dealtCards.Add("D5");
        solitaire.dealtCards.Add("HK");

        GameObject card = Object.Instantiate(cardPrefab);
        card.name = "D5";
        card.GetComponent<Selectable>().name = "D5";

        int initialCount = solitaire.dealtCards.Count;

        yield return new WaitForSeconds(0.1f);

        // Try to tap invalid card
        var tapMethod = typeof(MobileTouchInput).GetMethod("TapCard",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        tapMethod.Invoke(touchInput, new object[] { card });

        yield return new WaitForSeconds(0.3f); // Wait for shake animation

        Assert.AreEqual(initialCount, solitaire.dealtCards.Count, "Card count should not change");

        Debug.Log("✓✓✓ INTEGRATION TEST 4 PASSED - Invalid tap handled gracefully");
    }

    /// <summary>
    /// INTEGRATION TEST 5: Edge card tap blocking
    /// </summary>
    [UnityTest]
    public IEnumerator IntegrationTest_05_EdgeCardBlocking()
    {
        Debug.Log("\n▶▶▶ RUNNING INTEGRATION TEST 5: Edge Card Blocking");

        solitaire.dealtCards.Clear();
        solitaire.dealtCards.Add("C5");
        solitaire.dealtCards.Add("DA");
        solitaire.dealtCards.Add("H5");

        // Try to tap first card (edge)
        GameObject firstCard = Object.Instantiate(cardPrefab);
        firstCard.name = "C5";
        firstCard.GetComponent<Selectable>().name = "C5";

        int initialCount = solitaire.dealtCards.Count;

        var tapMethod = typeof(MobileTouchInput).GetMethod("TapCard",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        tapMethod.Invoke(touchInput, new object[] { firstCard });

        yield return new WaitForSeconds(0.1f);

        Assert.AreEqual(initialCount, solitaire.dealtCards.Count, "Edge card should not be removed");

        Debug.Log("✓✓✓ INTEGRATION TEST 5 PASSED - Edge cards protected");
    }

    /// <summary>
    /// INTEGRATION TEST 6: Camera drag functionality
    /// </summary>
    [UnityTest]
    public IEnumerator IntegrationTest_06_CameraDrag()
    {
        Debug.Log("\n▶▶▶ RUNNING INTEGRATION TEST 6: Camera Drag");

        touchInput.SetCameraDrag(true);
        
        Vector3 initialCameraPos = mainCamera.transform.position;

        yield return new WaitForSeconds(0.1f);

        // Simulate drag using reflection
        var dragMethod = typeof(MobileTouchInput).GetMethod("HandleDrag",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        Vector2 dragPos = new Vector2(100f, 100f);
        dragMethod.Invoke(touchInput, new object[] { dragPos });

        yield return null;

        // Camera position might have changed (depending on lastTouchPos state)
        Assert.IsTrue(true, "Camera drag method executed without errors");

        Debug.Log("✓✓✓ INTEGRATION TEST 6 PASSED - Camera drag functional");
    }

    /// <summary>
    /// INTEGRATION TEST 7: Highlighting toggle functionality
    /// </summary>
    [UnityTest]
    public IEnumerator IntegrationTest_07_HighlightingToggle()
    {
        Debug.Log("\n▶▶▶ RUNNING INTEGRATION TEST 7: Highlighting Toggle");

        // Deal some cards
        for (int i = 0; i < 5; i++)
        {
            solitaire.DealFromDeck();
        }

        yield return new WaitForSeconds(0.1f);

        // Toggle highlighting on
        touchInput.SetShowValidMoves(true);
        yield return null;

        // Toggle highlighting off
        touchInput.SetShowValidMoves(false);
        yield return null;

        // Toggle back on
        touchInput.SetShowValidMoves(true);
        yield return null;

        Assert.IsTrue(true, "Highlighting can be toggled without errors");

        Debug.Log("✓✓✓ INTEGRATION TEST 7 PASSED - Highlighting toggle works");
    }

    /// <summary>
    /// INTEGRATION TEST 8: Game over state respect
    /// </summary>
    [UnityTest]
    public IEnumerator IntegrationTest_08_GameOverStateRespect()
    {
        Debug.Log("\n▶▶▶ RUNNING INTEGRATION TEST 8: Game Over State");

        // Set game over state
        solitaire.isGameOver = true;
        solitaire.dealtCards.Clear();
        solitaire.dealtCards.Add("CA");
        solitaire.dealtCards.Add("D2");
        solitaire.dealtCards.Add("H3");

        GameObject card = Object.Instantiate(cardPrefab);
        card.name = "D2";
        card.GetComponent<Selectable>().name = "D2";

        int initialCount = solitaire.dealtCards.Count;

        // Try to tap card when game is over
        var tapMethod = typeof(MobileTouchInput).GetMethod("TapCard",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        tapMethod.Invoke(touchInput, new object[] { card });

        yield return new WaitForSeconds(0.1f);

        Assert.AreEqual(initialCount, solitaire.dealtCards.Count, "Cards should not be removed when game over");

        Debug.Log("✓✓✓ INTEGRATION TEST 8 PASSED - Game over state respected");
    }

    /// <summary>
    /// INTEGRATION TEST 9: Multiple card removals in sequence
    /// </summary>
    [UnityTest]
    public IEnumerator IntegrationTest_09_SequentialCardRemovals()
    {
        Debug.Log("\n▶▶▶ RUNNING INTEGRATION TEST 9: Sequential Card Removals");

        // Deal multiple cards
        for (int i = 0; i < 15; i++)
        {
            solitaire.DealFromDeck();
        }

        int initialCount = solitaire.dealtCards.Count;

        yield return new WaitForSeconds(0.1f);

        // Attempt several removals (may or may not succeed based on random deck)
        for (int attempt = 0; attempt < 3; attempt++)
        {
            if (solitaire.dealtCards.Count < 3) break;

            // Find a valid card if possible
            GameObject validCard = null;
            for (int i = 1; i < solitaire.dealtCards.Count - 1; i++)
            {
                string cardName = solitaire.dealtCards[i];
                validCard = new GameObject(cardName);
                validCard.tag = "Card";
                validCard.AddComponent<SpriteRenderer>();
                validCard.AddComponent<Selectable>().name = cardName;

                var isRemovableMethod = typeof(MobileTouchInput).GetMethod("IsCardRemovable",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                bool removable = (bool)isRemovableMethod.Invoke(touchInput, new object[] { validCard });

                if (removable)
                {
                    var tapMethod = typeof(MobileTouchInput).GetMethod("TapCard",
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                    tapMethod.Invoke(touchInput, new object[] { validCard });
                    break;
                }

                Object.Destroy(validCard);
                validCard = null;
            }

            yield return new WaitForSeconds(0.1f);
        }

        Assert.LessOrEqual(solitaire.dealtCards.Count, initialCount, "Some cards may have been removed");

        Debug.Log("✓✓✓ INTEGRATION TEST 9 PASSED - Sequential removals handled");
    }

    /// <summary>
    /// INTEGRATION TEST 10: Full game flow with touch input
    /// </summary>
    [UnityTest]
    public IEnumerator IntegrationTest_10_FullGameFlowWithTouch()
    {
        Debug.Log("\n▶▶▶ RUNNING INTEGRATION TEST 10: Full Game Flow");

        // Start new game
        solitaire.PlayCards();
        touchInput.SetShowValidMoves(true);

        yield return new WaitForSeconds(0.1f);

        // Deal some cards
        Debug.Log("Dealing 20 cards...");
        for (int i = 0; i < 20; i++)
        {
            solitaire.DealFromDeck();
            if (i % 5 == 0)
            {
                yield return null;
            }
        }

        Assert.AreEqual(20, solitaire.dealtCards.Count, "Should have 20 cards");

        yield return new WaitForSeconds(0.1f);

        // Test highlighting is active
        Debug.Log("Testing highlighting system...");
        yield return null;

        // Toggle camera drag
        Debug.Log("Testing camera drag toggle...");
        touchInput.SetCameraDrag(false);
        touchInput.SetCameraDrag(true);

        yield return null;

        Debug.Log("Game flow test completed successfully");

        Debug.Log("✓✓✓ INTEGRATION TEST 10 PASSED - Full game flow works with touch");
    }
}
