using UnityEngine;
using System.Collections.Generic;

public class UIButtons : MonoBehaviour
{
    public GameObject highScorePanel;
    private Solitaire solitaire;

    void Start()
    {
        solitaire = FindFirstObjectByType<Solitaire>();
        
        // Make sure high score panel starts hidden
        if (highScorePanel != null)
        {
            highScorePanel.SetActive(false);
        }
    }

    public void Undo()
    {
        Debug.Log("Undo button pressed");
        AudioManager.Instance?.PlayButtonClick();

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
        AudioManager.Instance?.PlayButtonClick();

        if (highScorePanel != null)
        {
            highScorePanel.SetActive(false);
        }

        ResetScene();
    }

    public void ResetScene()
    {
        Debug.Log("Resetting scene");
        AudioManager.Instance?.PlayButtonClick();
        
        // Update scores before resetting (if game was in progress)
        if (solitaire != null && solitaire.allCardsDealt)
        {
            solitaire.updateScores();
        }

        // Remove all active cards 
        UpdateSprite[] cards = FindObjectsByType<UpdateSprite>(FindObjectsSortMode.None);
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
        Debug.Log("Show Best Score panel");
        AudioManager.Instance?.PlayButtonClick();

        if (highScorePanel != null)
        {
            highScorePanel.SetActive(true);
        }
    }

    public void ToggleMusic()
    {
        AudioManager.Instance?.ToggleMusic();
        Debug.Log("Music toggled. Now enabled: " + (AudioManager.Instance?.IsMusicEnabled() ?? false));
    }

    public void ConfirmQuit()
    {
        Application.Quit();
    }

    public void CancelQuit(GameObject quitPanel)
    {
        if (quitPanel != null)
            quitPanel.SetActive(false);
    }
}
