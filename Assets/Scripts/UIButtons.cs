using UnityEngine;
using System.Collections.Generic;

public class UIButtons : MonoBehaviour
{
    public GameObject highScorePanel;
    private Solitaire solitaire;

    void Start()
    {
        solitaire = FindObjectOfType<Solitaire>();
        
        // Make sure high score panel starts hidden
        if (highScorePanel != null)
        {
            highScorePanel.SetActive(false);
        }
    }

    void Update()
    {
        
    }

    public void Undo()
    {
        Debug.Log("Undo button pressed");
        
        if (solitaire != null)
        {
            solitaire.UndoCards();
        }
        else
        {
            Debug.LogError("Solitaire reference not found!");
        }
    }

    public void PlayAgain()
    {
        Debug.Log("Play Again button pressed");
        
        if (highScorePanel != null)
        {
            highScorePanel.SetActive(false);
        }
        
        ResetScene();
    }

    public void ResetScene()
    {
        Debug.Log("Resetting scene");
        
        // Update scores before resetting (if game was in progress)
        if (solitaire != null && solitaire.allCardsDealt)
        {
            solitaire.updateScores();
        }

        // Remove all active cards 
        UpdateSprite[] cards = FindObjectsOfType<UpdateSprite>();
        foreach (UpdateSprite card in cards)
        {
            Destroy(card.gameObject);
        }

        // Start new game
        if (solitaire != null)
        {
            solitaire.PlayCards();
        }
        else
        {
            Debug.LogError("Solitaire reference not found!");
        }
    }
    
    public void ShowBestScore()
    {
        // Optional: Add a button to show best scores/statistics
        Debug.Log("Show Best Score panel");
        
        if (highScorePanel != null)
        {
            highScorePanel.SetActive(true);
        }
    }
}
