using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class CardDealPatternTests
{
    private Deck deck;
    private const int CardsPerRow = 9;

    [SetUp]
    public void SetUp()
    {
        deck = new Deck();
        deck.InitializeDeck();
    }

    [Test]
    public void DealCards_CheckRowPatterns()
    {
        List<Vector3> cardPositions = new List<Vector3>();

        // Deal all cards, storing their positions
        for (int i = 0; i < 52; i++)
        {
            Card card = deck.DealCard();
            Vector3 position = card.GetPosition(); // Assuming each card has a position
            cardPositions.Add(position);
        }

        // Check the card positions for the pattern
        for (int i = 0; i < cardPositions.Count; i++)
        {
            int row = i / CardsPerRow;
            int col = i % CardsPerRow;

            if (row % 2 == 0)
            {
                // Left-to-right rows: cards should increase left-to-right
                Assert.Less(cardPositions[i].x, cardPositions[i + 1].x,
                    $"Expected left-to-right order in row {row}, card {i}.");
            }
            else
            {
                // Right-to-left rows: cards should decrease right-to-left
                Assert.Greater(cardPositions[i].x, cardPositions[i + 1].x,
                    $"Expected right-to-left order in row {row}, card {i}.");
            }

            // Check the start of each row is consistent
            if (col == 0)
            {
                Assert.AreEqual(0, cardPositions[i].y, $"Expected all rows to start at y=0 for row {row}.");
            }
        }
    }
}
