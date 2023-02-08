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

        //TODO create Undo process to be linekd to button
        //  GameObject newCard = Instantiate(cardPrefab, new Vector3(xOffset, yOffset, zOffset), Quaternion.identity, deckButton.transform
        //    newCard.name = card;

    }

    public void PlayAgain()
    {
        highScorePanel.SetActive(false);
        ResetScene();
    }

    public void ResetScene()
    {
        // find all the cards and remove them
        UpdateSprite[] cards = FindObjectsOfType<UpdateSprite>();
        foreach (UpdateSprite card in cards)
        {
            Destroy(card.gameObject);
        }

        // deal new cards
        FindObjectOfType<Solitaire>().PlayCards();
    }
}
