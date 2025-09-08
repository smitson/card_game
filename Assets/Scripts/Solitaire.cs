using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class Solitaire : MonoBehaviour
{
    public Sprite[] cardFaces;
    public GameObject cardPrefab;
    public GameObject deckButton;
    public GameObject Deck;
    public GameObject[] bottomPos;
    public GameObject[] topPos;

    public Camera m_MainCamera;

    public static string[] suits = new string[] { "C", "D", "H", "S" };
    public static string[] values = new string[] { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };

    public static float[] cardXEvenlocs = new float[] {-10.0f, -8.0f, -6.0f, -4.0f, -2.0f, 0.0f, 2.0f, 4.0f, 6.0f};
    public static float[] cardXOddlocs = new float[] {6.0f, 4.0f, 2.0f, 0.0f, -2.0f, -4.0f, -6.0f, -8.0f,-10.0f};

    // TODO 10 card per row public static float[] cardXEvenlocs = new float[] {-10.0f, -8.0f, -6.0f, -4.0f, -2.0f, 0.0f, 2.0f, 4.0f, 6.0f, 8.0f};
    // public static float[] cardXOddlocs = new float[] {8.0f, 6.0f, 4.0f, 2.0f, 0.0f, -2.0f, -4.0f, -6.0f, -8.0f,-10.0f};
    
    public static float[] cardYLocations = new float[] {3.0f, 0.0f, -3.0f, -6.0f, -9.0f, -12.0f, -15.0f, -18.0f, -12.0f};

    public List<string> dealtCards = new List<string>();

    public Stack removedCards = new Stack();

    public List<string> deck;
    public List<string> dealDeck;
    [SerializeField]
    private int cardRow = 9;
    private int deckLocation = 0;
    private int cardDealt = 0;

    public int numRow = 0;

    private float scrollDown = -0.8f;

    public  int currentScore;
    private int bestScore = 52;
    public  int totalGames = 0;
    private int scoreRange1to5 = 0;
    private int scoreRange6to10 = 0;
    private int scoreRange11to15 = 0;
    private int scoreRange16to20 = 0;

    private const string BestScoreKey = "BestScore";
    private const string ScoreRangeKey1to5 = "ScoreRange1to5";
    private const string ScoreRangeKey6to10 = "ScoreRange6to10";
    private const string ScoreRangeKey11to15 = "ScoreRange11to15";
    private const string ScoreRangeKey16to20 = "ScoreRange16to20";

    private const string TotalGames = "TotalGames";

    public TextMeshProUGUI displayScore;

    public bool allCardsDealt;
    public bool isGameOver;




    // Start is called before the first frame update
    void Start()
    {
        displayScore = FindFirstObjectByType<TextMeshProUGUI>();
        bestScore = 52;

        if (PlayerPrefs.HasKey(BestScoreKey))
        {
            bestScore = PlayerPrefs.GetInt(BestScoreKey);
            scoreRange1to5 = PlayerPrefs.GetInt(ScoreRangeKey1to5);
            scoreRange6to10 = PlayerPrefs.GetInt(ScoreRangeKey6to10);
            scoreRange11to15 = PlayerPrefs.GetInt(ScoreRangeKey11to15);
            scoreRange16to20 = PlayerPrefs.GetInt(ScoreRangeKey16to20);
            totalGames = PlayerPrefs.GetInt(TotalGames);
        }

        displayScore.text = bestScore.ToString();

        isGameOver = false;

        PlayCards();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnApplicationQuit()
    {
        EndGame();
    }

    public void updateScores()
    {
        // Update best score if current score is lower
        if (allCardsDealt)
        {
            // Increment total games when a game is completed
            totalGames++;
            
            if (currentScore < bestScore)
            {
                bestScore = currentScore;
                displayScore.text = bestScore.ToString();

                // Save new best score
                PlayerPrefs.SetInt(BestScoreKey, bestScore);


            }

            // Update score ranges
            if (currentScore >= 1 && currentScore <= 5)
            {
                scoreRange1to5++;
                PlayerPrefs.SetInt(ScoreRangeKey1to5, scoreRange1to5);
            }
            else if (currentScore >= 6 && currentScore <= 10)
            {
                scoreRange6to10++;
                PlayerPrefs.SetInt(ScoreRangeKey6to10, scoreRange6to10);
            }
            else if (currentScore >= 11 && currentScore <= 15)
            {
                scoreRange11to15++;
                PlayerPrefs.SetInt(ScoreRangeKey11to15, scoreRange11to15);
            }
            else if (currentScore >= 16 && currentScore <= 20)
            {
                scoreRange16to20++;
                PlayerPrefs.SetInt(ScoreRangeKey16to20, scoreRange16to20);
            }
        }
        PlayerPrefs.Save();
    }

    public void EndGame()
    {
        updateScores();
        PlayerPrefs.SetInt(TotalGames, totalGames);
        PlayerPrefs.Save();    
    }

    public void PlayCards()
    {

        deckButton.GetComponent<Renderer>().enabled = true;
        Camera.main.transform.position = new Vector3(0f, 0f, -1.0f);
        
        float yAxis = Camera.main.transform.position[1];
        Debug.Log("yAxis");
        Debug.Log("yAxis = " + yAxis);
        removedCards.Clear();
        
        deckLocation = 0;
        cardDealt = 0;
        numRow = 2;
        dealtCards = new List<string>();
        allCardsDealt = false;

        deck = GenerateDeck();
        Shuffle(deck);
    }

    public void UndoCards()
    {

        if (removedCards.Count > 1)
        {
            int listPosPop = (int)removedCards.Pop();
            string cardNamePop = (removedCards.Pop() as string);

            Debug.Log(listPosPop);
            Debug.Log(cardNamePop);
            dealtCards.Insert(listPosPop,cardNamePop);
            GameObject newCard = Instantiate(cardPrefab, new Vector3(0, 0, 0), Quaternion.identity, deckButton.transform);
        
            newCard.name = cardNamePop;
        
            currentScore = currentScore + 2;
            MoveCards();
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

    public void CheckRow(int yLoc)
    {
        if (yLoc > numRow)
            {
                numRow = yLoc;
                m_MainCamera = Camera.main;
                m_MainCamera.transform.position = m_MainCamera.transform.position + new Vector3(0, yLoc * scrollDown, 0);
            }
    }
    public void DealFromDeck()
    {
        if (cardDealt < 52)
        { 
            if (cardDealt < 1)
            {
                Debug.Log("new game");
            }    

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
            

            CheckRow(yLoc);

            GameObject newCard = Instantiate(cardPrefab, new Vector3(xOffset, yOffset, zOffset), Quaternion.identity, deckButton.transform);

            newCard.name = card;

            deckLocation++;
            cardDealt++;
            currentScore++;

            if (cardDealt >= 52)
            {
                deckButton.GetComponent<Renderer>().enabled = false;
                allCardsDealt = true;
                Debug.Log("52 cards dealt");
            }
        }
        
        
    }    

    public void MoveCards()
    {
        int yLoc;
        int xLoc;
        float xOffset, yOffset, zOffset = 0.2f;
        int count = 0;
        deckLocation = 0;
        currentScore--;

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

            CheckRow(yLoc);

            nextCard.transform.position = new Vector3( xOffset, yOffset, zOffset);
            
            count++;
            deckLocation++;
        }
    }
}
