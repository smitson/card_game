using UnityEngine;
using System.Collections.Generic;

public class UIButtons : MonoBehaviour
{
    public GameObject highScorePanel;
    private Solitaire solitaire;

    // Start is called before the first frame update
    void Start()
    {
        solitaire = FindFirstObjectByType<Solitaire>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Undo()
    {
        print("Undo");  
        if (solitaire == null)
        {
            solitaire = FindFirstObjectByType<Solitaire>();
        }
        solitaire.UndoCards();

    }

    public void PlayAgain()
    {
        highScorePanel.SetActive(false);
        ResetScene();
    }

    public void ResetScene()
    {
        //Remove all active cards 
        UpdateSprite[] cards = FindObjectsByType<UpdateSprite>(FindObjectsSortMode.None);
        foreach (UpdateSprite card in cards)
        {
            Destroy(card.gameObject);
        }

        //TODO Is this where we store the score ?
     
        if (solitaire == null)
        {
            solitaire = FindFirstObjectByType<Solitaire>();
        }
        solitaire.PlayCards();
    }
}
