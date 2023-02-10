using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Solitaire : MonoBehaviour
{
    public Sprite[] cardFaces;
    public GameObject cardPrefab;
    public GameObject deckButton;
    public GameObject Deck;
    public GameObject[] bottomPos;
    public GameObject[] topPos;

    public static string[] suits = new string[] { "C", "D", "H", "S" };
    public static string[] values = new string[] { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };

    public static float[] cardXEvenlocs = new float[] {-10.0f, -8.0f, -6.0f, -4.0f, -2.0f, 0.0f, 2.0f, 4.0f, 6.0f, 8.0f};
    public static float[] cardXOddlocs = new float[] {8.0f, 6.0f, 4.0f, 2.0f, 0.0f, -2.0f, -4.0f, -6.0f, -8.0f,-10.0f};
    
    public static float[] cardYLocations = new float[] {3.0f, 0.0f, -3.0f, -6.0f, -9.0f, -12.0f, -15.0f, -18.0f, -12.0f};

    public List<string> dealtCards = new List<string>();

    public Dictionary<string, int> removedCards = new Dictionary<string, int>();

    public List<string> deck;
    public List<string> dealDeck;

    private int cardRow = 10;
    private int deckLocation = 0;
    private int cardDealt = 0;

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

        deckButton.GetComponent<Renderer>().enabled = true;
        removedCards.Clear();
        
        deckLocation = 0;
        cardDealt = 0;
        dealtCards = new List<string>();

        deck = GenerateDeck();
        Shuffle(deck);


        
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
        if (cardDealt < 52)
        { 
            string card; 
            int yLoc;
            int xLoc;
            float xOffset, yOffset, zOffset = 0.2f;
            
            card = deck[cardDealt];   
        
            xLoc = (deckLocation % cardRow);
            yLoc = (deckLocation / cardRow);
            
            dealtCards.Add(card);     
        

            if(yLoc%2 == 0)
            {
                xOffset = cardXEvenlocs[xLoc];
            }        
            else
            {
                xOffset = cardXOddlocs[xLoc];
            }    
            
            yOffset = cardYLocations[yLoc];


            GameObject newCard = Instantiate(cardPrefab, new Vector3(xOffset, yOffset, zOffset), Quaternion.identity, deckButton.transform);

            newCard.name = card;

            deckLocation++;
            cardDealt++;
        }
        else
        {
            deckButton.GetComponent<Renderer>().enabled = false;
            //print("More than 52 cards dealt");    
        }
        
    }    

    public void MoveCards()
    {
        int yLoc;
        int xLoc;
        float xOffset, yOffset, zOffset = 0.2f;
        int count = 0;
        deckLocation = 0;
        GameObject nextCard;          
       
            
        foreach (string card in dealtCards)
        {
            nextCard = GameObject.Find(card);
            xLoc = (count % cardRow);
            yLoc = (count / cardRow);
            
            if(yLoc%2 == 0)
            {
                xOffset = cardXEvenlocs[xLoc];
            }        
            else
            {
            xOffset = cardXOddlocs[xLoc];
            }    
            
            yOffset = cardYLocations[yLoc];
            nextCard.transform.position = new Vector3( xOffset, yOffset, zOffset);
            
            count++;
            deckLocation++;
        }
    }
}
