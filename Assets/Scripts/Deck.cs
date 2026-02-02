using System.Collections.Generic;
using UnityEngine;

public class Deck
{
    private readonly List<Card> cards = new List<Card>();
    private int dealIndex = 0;
    private const int CardsPerRow = 9;

    public void InitializeDeck()
    {
        cards.Clear();
        dealIndex = 0;

        int total = 52;
        for (int i = 0; i < total; i++)
        {
            int row = i / CardsPerRow;
            int col = i % CardsPerRow;

            float x = (row % 2 == 0) ? col : (CardsPerRow - 1 - col);
            float y = 0f; // Rows start at y=0 per test assumption

            cards.Add(new Card(new Vector3(x, y, 0f)));
        }
    }

    public Card DealCard()
    {
        if (dealIndex >= cards.Count)
            return null;

        return cards[dealIndex++];
    }
}
