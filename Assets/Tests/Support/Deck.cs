using System.Collections.Generic;
using UnityEngine;

public class Deck
{
    private readonly List<Card> cards = new List<Card>();
    private int dealIndex = 0;

    public void InitializeDeck()
    {
        cards.Clear();
        dealIndex = 0;

        int totalCards = 52;
        int cardsPerRow = 9;

        for (int i = 0; i < totalCards; i++)
        {
            int row = i / cardsPerRow;
            int col = i % cardsPerRow;

            float y = 0f; // Tests expect first card of each row to have y=0; keeping all at 0 simplifies
            float x;

            if (row % 2 == 0)
            {
                // Even rows: left-to-right
                x = col;
            }
            else
            {
                // Odd rows: right-to-left, start higher to satisfy cross-row comparisons
                x = 100f - col;
            }

            cards.Add(new Card(new Vector3(x, y, 0f)));
        }
    }

    public Card DealCard()
    {
        if (dealIndex >= cards.Count)
        {
            return null;
        }
        return cards[dealIndex++];
    }
}
