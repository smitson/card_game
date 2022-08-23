using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Solitaire : MonoBehaviour
{
    public Sprite[] cardFaces;
    public GameObject cardPrefab;
    public GameObject deckButton;
    public GameObject[] bottomPos;
    public GameObject[] topPos;

    public static string[] suits = new string[] { "C", "D", "H", "S" };
    public static string[] values = new string[] { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };

    public List<string> DealtCards = new List<string>();

    public List<string> deck;
    public List<string> dealDeck;

    private int deckLocation;
    private int cardXLocation = 0;
    private int cardYLocation = 0;
    private int cardRow = 6;
    private int trips;
    private int tripsRemainder;



    // Start is called before the first frame update
    void Start()
    {
        PlayCards();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayCards()
    {
        deck = GenerateDeck();
        Shuffle(deck);

        //test the cards in the deck:
        foreach (string card in deck)
        {
            print(card);
        }
    }


    public static List<string> GenerateDeck()
    {
        List<string> newDeck = new List<string>();
        foreach (string s in suits)
        {
            foreach (string v in values)
            {
                newDeck.Add(s + v);
            }
        }
        return newDeck;
    }

    void Shuffle<T>(List<T> list)
    {
        System.Random random = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            int k = random.Next(n);
            n--;
            T temp = list[k];
            list[k] = list[n];
            list[n] = temp;
        }
    }

   
    
    public void DealFromDeck()
    {
        //if (NewDeck.Count == 0)
        //{
        //    dealDeck == deck;        
        //} 
        //else
        //{
        //    dealDeck == NewDeck;
        // }
        
        string card = deck[deckLocation]; 
        {
            print("card");

            float xOffset = -4.0f + ((cardXLocation * 2) + 0.01f);
            float yOffset = 3.0f + -(cardYLocation * 3);
            float zOffset = 0.2f;

            GameObject newCard = Instantiate(cardPrefab, new Vector3(xOffset, yOffset, zOffset), Quaternion.identity, deckButton.transform);

            newCard.name = card;
            DealtCards.Add(card);
            
            cardXLocation ++;
            if (cardXLocation > cardRow)
            {
                cardXLocation = 0;
                cardYLocation++;
            }
        }
        deckLocation++;

        
    }
}
