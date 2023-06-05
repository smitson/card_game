using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UserInput : MonoBehaviour
{
    public GameObject slot1;
    private Solitaire solitaire;

    // Start is called before the first frame update
    void Start()
    {
        solitaire = FindObjectOfType<Solitaire>();
        slot1 = this.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        GetMouseClick();
    }

    void GetMouseClick()
    {
        if (Input.GetMouseButtonDown(0))
        {

            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -10));
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit)
            {
                
                // what has been hit? Deck/Card/EmptySlot...
                if (hit.collider.CompareTag("Deck"))
                {
                    //clicked deck
                    Deck();
                }
                else if (hit.collider.CompareTag("Card"))
                {
                    // clicked card

                    Card(hit.collider.gameObject);
                }
                else if (hit.collider.CompareTag("Reset"))
                {
                    Reset();
                        
                }
                else if (hit.collider.CompareTag("Undo"))
                {
                    solitaire.UndoCards();

                }
                else if (hit.collider.CompareTag("Best Score"))
                {
                    //TODO add in best score panel 
                    solitaire.UndoCards();

                }
            }
        }
    }

    void Deck()
    {
        // deck click actions
        Debug.Log("Clicked on deck");
        solitaire.DealFromDeck();
    }
    
    void Card(GameObject selected)
    {
        // card click actions
        // if card is selected need to chck it against the second card clicked
        
        Debug.Log("Clicked on Card");

            if (slot1 == this.gameObject) // not null because we pass in this gameObject instead
            {
                slot1 = selected;
                if (Stackable(selected))
                {    
                    Stack(selected);
                }
                else
                {
                    slot1 = this.gameObject;
                }
                
            }
    }

    void Reset()
    {
        //TODO have duplicated this need to look to remove 

        UpdateSprite[] cards = FindObjectsOfType<UpdateSprite>();
        foreach (UpdateSprite card in cards)
        {
            Destroy(card.gameObject);
        }
        FindObjectOfType<Solitaire>().PlayCards();
    }

    bool Stackable(GameObject selected)
    {
        Selectable s1 = selected.GetComponent<Selectable>();
        int posMid;
        string cardOne, cardTwo;

        posMid = solitaire.dealtCards.IndexOf(s1.name); 
        
        if (posMid > 0 && posMid < (solitaire.dealtCards.Count -1)) 
        {
            cardOne = solitaire.dealtCards[posMid - 1];
            cardTwo = solitaire.dealtCards[posMid + 1];

            if (cardOne.Substring(0,1) == cardTwo.Substring(0,1) || cardOne.Substring(1,1) == cardTwo.Substring(1,1))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
             
    }

    void Stack(GameObject selected)
    {
        Selectable s2 = selected.GetComponent<Selectable>();
        
        int listPos = 0;
        
        string cardName = s2.name;

        listPos = solitaire.dealtCards.IndexOf(cardName);  

        if (listPos > 0)
        {
            listPos = listPos - 1;
            cardName = solitaire.dealtCards[listPos];     
                        
            solitaire.removedCards.Push(cardName);
            solitaire.removedCards.Push(listPos);

            solitaire.dealtCards.Remove(cardName);
            
            Destroy(GameObject.Find(cardName));

            solitaire.MoveCards();
        } 

        // after completing move reset slot1 to be essentially null as being null will break the logic
        slot1 = this.gameObject;

    }
    
    
 
}
