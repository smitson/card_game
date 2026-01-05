using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI; // Using standard Unity UI instead of TextMeshPro

public class Solitaire : MonoBehaviour
{
    public Transform cardArea; // Scrollable area for dealt cards
    public Transform staticArea; // Static area for pack and buttons
    private Coroutine cameraMoveCoroutine;
    public Sprite[] cardFaces;
    public GameObject cardPrefab;
    public GameObject deckButton;
    public GameObject Deck;
    public GameObject[] bottomPos;
    public GameObject[] topPos;
    
    // High Score Panel Reference
    public GameObject highScorePanel;
    public Text winText;
    public Text finalScoreText;
    public Text bestScoreTextPanel;

    public Camera m_MainCamera;

    public static string[] suits = new string[] { "C", "D", "H", "S" };
    public static string[] values = new string[] { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };

    public static float[] cardXEvenlocs = new float[] {-9.0f, -7.0f, -5.0f, -3.0f, -1.0f, 1.0f, 3.0f, 5.0f, 7.0f};
    public static float[] cardXOddlocs = new float[] {7.0f, 5.0f, 3.0f, 1.0f, -1.0f, -3.0f, -5.0f, -7.0f,-9.0f};
    
    public static float[] cardYLocations = new float[] {3.0f, 0.0f, -3.0f, -6.0f, -9.0f, -12.0f, -15.0f, -18.0f, -12.0f};

    public List<string> dealtCards = new List<string>();
    public Stack<string> removedCards = new Stack<string>();

    public List<string> deck;
    public List<string> dealDeck;
    [SerializeField]
    private int cardRow = 9;
    private int deckLocation = 0;
    private int cardDealt = 0;

    public int numRow = 0;

    public int currentScore;
    private int bestScore = 52;
    public int totalGames = 0;
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

    public Text displayScore;
    public Text currentScoreText;

    public bool allCardsDealt;
    public bool isGameOver;

    // Start is called before the first frame update
    void Start()
    {
        // Find or create StaticArea parent (right side)
        GameObject staticAreaObj = GameObject.Find("StaticArea");
        if (staticAreaObj == null)
        {
            staticAreaObj = new GameObject("StaticArea");
            staticAreaObj.transform.position = new Vector3(8f, 0f, 0f);
        }
        staticArea = staticAreaObj.transform;

        // Move pack and buttons under StaticArea (if needed)
        deckButton.transform.SetParent(staticArea);

        // Find or create CardArea parent (left side)
        GameObject cardAreaObj = GameObject.Find("CardArea");
        if (cardAreaObj == null)
        {
            cardAreaObj = new GameObject("CardArea");
            cardAreaObj.transform.position = new Vector3(-4f, 0f, 0f);
        }
        cardArea = cardAreaObj.transform;
        
        // Find display score text (if it exists)
        // displayScore = FindFirstObjectByType<Text>();
        bestScore = 52;
        Debug.Log("displayScore " + displayScore);

        // Load saved scores
        if (PlayerPrefs.HasKey(BestScoreKey))
        {
            bestScore = PlayerPrefs.GetInt(BestScoreKey);
            scoreRange1to5 = PlayerPrefs.GetInt(ScoreRangeKey1to5);
            scoreRange6to10 = PlayerPrefs.GetInt(ScoreRangeKey6to10);
            scoreRange11to15 = PlayerPrefs.GetInt(ScoreRangeKey11to15);
            scoreRange16to20 = PlayerPrefs.GetInt(ScoreRangeKey16to20);
            totalGames = PlayerPrefs.GetInt(TotalGames);
            Debug.Log("Loaded - Best Score: " + bestScore + " Total Games: " + totalGames);
        }

        if (displayScore != null)
        {
            displayScore.text = "Best: " + bestScore.ToString();
        }

        // Make sure high score panel is hidden at start
        if (highScorePanel != null)
        {
            highScorePanel.SetActive(false);
        }

        isGameOver = false;
        PlayCards();
    }

    void Update()
    {
        // Update current score display
        if (currentScoreText != null)
        {
            currentScoreText.text = "Score: " + currentScore.ToString();
        }
        
        // Check for win/lose conditions after all cards dealt
        if (allCardsDealt && !isGameOver)
        {
            CheckGameEnd();
        }
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt(TotalGames, totalGames);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Check if the game has ended (win or lose)
    /// </summary>
    private void CheckGameEnd()
    {
        // WIN CONDITION: Only 1 card left
        if (dealtCards.Count <= 1)
        {
            Debug.Log("WIN! Only " + dealtCards.Count + " card(s) remaining!");
            GameWin();
            return;
        }

        // LOSE CONDITION: No more valid moves available
        if (!HasValidMoves())
        {
            Debug.Log("LOSE! No more valid moves. Cards remaining: " + dealtCards.Count);
            GameLose();
            return;
        }
    }

    /// <summary>
    /// Check if there are any valid moves left
    /// </summary>
    private bool HasValidMoves()
    {
        // Need at least 3 cards to make a move (card in middle to remove, plus one on each side)
        if (dealtCards.Count < 3)
        {
            return false;
        }

        // Check each card (except first and last) to see if it can be stacked
        for (int i = 1; i < dealtCards.Count - 1; i++)
        {
            string currentCard = dealtCards[i];
            string leftCard = dealtCards[i - 1];
            string rightCard = dealtCards[i + 1];

            // Check if left and right cards match by suit or value
            if (leftCard.Substring(0, 1) == rightCard.Substring(0, 1) || // Same suit
                leftCard.Substring(1) == rightCard.Substring(1))          // Same value
            {
                return true; // Found a valid move
            }
        }

        return false; // No valid moves found
    }

    /// <summary>
    /// Handle game win
    /// </summary>
    private void GameWin()
    {
        isGameOver = true;
        updateScores();
        ShowHighScorePanel(true);
    }

    /// <summary>
    /// Handle game loss
    /// </summary>
    private void GameLose()
    {
        isGameOver = true;
        updateScores();
        ShowHighScorePanel(false);
    }

    /// <summary>
    /// Display the high score panel with results
    /// </summary>
    private void ShowHighScorePanel(bool won)
    {
        if (highScorePanel == null)
        {
            Debug.LogWarning("High Score Panel is not assigned!");
            return;
        }

        highScorePanel.SetActive(true);

        // Update win/lose text
        if (winText != null)
        {
            if (won)
            {
                winText.text = "YOU WIN!";
                winText.color = Color.green;
            }
            else
            {
                winText.text = "GAME OVER";
                winText.color = Color.red;
            }
        }

        // Update final score
        if (finalScoreText != null)
        {
            finalScoreText.text = "Final Score: " + currentScore;
        }

        // Update best score on panel
        if (bestScoreTextPanel != null)
        {
            bestScoreTextPanel.text = "Best Score: " + bestScore;
        }

        Debug.Log("High Score Panel Shown - Won: " + won + " Score: " + currentScore);
    }

    public void updateScores()
    {
        // Update best score if current score is lower (lower is better in this game)
        if (allCardsDealt)
        {
            Debug.Log("Updating scores - Current: " + currentScore + " Best: " + bestScore);

            if (currentScore < bestScore)
            {
                bestScore = currentScore;
                if (displayScore != null)
                {
                    displayScore.text = "Best: " + bestScore.ToString();
                }
                Debug.Log("New best score: " + bestScore);

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

        removedCards.Clear();

        deckLocation = 0;
        cardDealt = 0;
        currentScore = 0;
        numRow = 2;
        dealtCards = new List<string>();
        allCardsDealt = false;
        isGameOver = false;

        deck = GenerateDeck();
        Shuffle(deck);

        Debug.Log("New game started!");
    }

    public void UndoCards()
    {
        if (removedCards.Count > 1)
        {
            int listPosPop = int.Parse((string)removedCards.Pop());
            string cardNamePop = (removedCards.Pop() as string);

            Debug.Log("Undo - Position: " + listPosPop + " Card: " + cardNamePop);
            dealtCards.Insert(listPosPop, cardNamePop);
            GameObject newCard = Instantiate(cardPrefab, new Vector3(0, 0, 0), Quaternion.identity, deckButton.transform);

            newCard.name = cardNamePop;

            currentScore = currentScore + 2; // Penalty for undo
            MoveCards();
        }
    }

    public void MoveCards()
    {
        int yLoc;
        int xLoc;
        float xOffset, yOffset, zOffset = 0.2f;
        int count = 0;
        deckLocation = 0;
        currentScore--; // Reduce score for successful move

        Debug.Log("MoveCards - Current score: " + currentScore);

        GameObject nextCard;

        foreach (string card in dealtCards)
        {
            nextCard = GameObject.Find(card);
            if (nextCard == null)
            {
                Debug.LogWarning("Card not found: " + card);
                continue;
            }

            xLoc = (count % cardRow);
            yLoc = (count / cardRow);

            if (yLoc % 2 == 0)
            {
                xOffset = cardXEvenlocs[xLoc];
            }
            else
            {
                xOffset = cardXOddlocs[xLoc];
            }

            yOffset = cardYLocations[yLoc];

            CheckRow(yLoc);

            nextCard.transform.position = new Vector3(xOffset, yOffset, zOffset);

            count++;
            deckLocation++;
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
        Debug.Log("DealFromDeck - Card #" + cardDealt);
        if (cardDealt < 52)
        {
            if (cardDealt < 1)
            {
                totalGames++;
                Debug.Log("Starting new game #" + totalGames);
            }
            
            string card;
            int yLoc;
            int xLoc;
            float xOffset, yOffset, zOffset = 0.2f;
            
            card = deck[cardDealt];
            xLoc = (deckLocation % cardRow);
            yLoc = (deckLocation / cardRow);
            dealtCards.Add(card);
            
            if (yLoc % 2 == 0)
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
            currentScore++;
            
            Debug.Log("Dealt card: " + card + " | Current score: " + currentScore);
            
            if (cardDealt >= 52)
            {
                deckButton.GetComponent<Renderer>().enabled = false;
                allCardsDealt = true;
                Debug.Log("All 52 cards dealt! Game checking for win/lose...");
            }
        }
    }

    public void CheckRow(int yLoc)
    {
        // Move CardArea vertically to keep new cards in view
        if (yLoc > 4)
        {
            float scrollOffset = -(yLoc - 4) * 3.0f;
            cardArea.position = new Vector3(cardArea.position.x, scrollOffset, cardArea.position.z);
        }
        else
        {
            cardArea.position = new Vector3(cardArea.position.x, 0f, cardArea.position.z);
        }
    }
}
