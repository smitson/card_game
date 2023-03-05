using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UIButtons : MonoBehaviour
{
    public GameObject highScorePanel;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Undo()
    {
        print("Undo");  
        FindObjectOfType<Solitaire>().UndoCards();

    }

    public void PlayAgain()
    {
        highScorePanel.SetActive(false);
        ResetScene();
    }

    public void ResetScene()
    {
        //Remove all active cards 
        UpdateSprite[] cards = FindObjectsOfType<UpdateSprite>();
        foreach (UpdateSprite card in cards)
        {
            Destroy(card.gameObject);
        }

        // deal new cards
        FindObjectOfType<Solitaire>().PlayCards();
    }
}
