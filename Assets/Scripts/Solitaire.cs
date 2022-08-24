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

    public static float[] cardXEvenlocs = new float[] {-4.0f, -2.0f, 0.0f, 2.0f, 4.0f, 6.0f};
    public static float[] cardXOddlocs = new float[] {6.0f, 4.0f, 2.0f, 0.0f, -2.0f, -4.0f};
    
    public static float[] cardYLocations = new float[] {3.0f, 0.0f, -3.0f, -6.0f, -9.0f, -12.0f, -15.0f, -18.0f, -12.0f};

    public List<string> DealtCards = new List<string>();

    public List<string> deck;
    public List<string> dealDeck;

    private int cardRow = 6;
    private int deckLocation = 0;

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

        string card = deck[deckLocation]; 
        {
            print("card");
            
            DealtCards.Add(card);

            
            int xLoc = (deckLocation % cardRow);
            int yLoc = (deckLocation / cardRow);

            float xOffset;

            if(yLoc%2 == 0)
            {
                xOffset = cardXEvenlocs[xLoc];
            }        
            else
            {
                xOffset = cardXOddlocs[xLoc];
            }    
            
            float yOffset = cardYLocations[yLoc];

            float zOffset = 0.2f;

            GameObject newCard = Instantiate(cardPrefab, new Vector3(xOffset, yOffset, zOffset), Quaternion.identity, deckButton.transform);

            newCard.name = card;
        
            //TODO if its new card increment deckLocation - if it is at the end then we need to access endGame option  

            deckLocation++;
        }
    }    
}
