using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UserInput : MonoBehaviour
{
    public GameObject slot1;
    private Solitaire solitaire;
    private float timer;
    private float doubleClickTime = 0.3f;
    private int clickCount = 0;


    // Start is called before the first frame update
    void Start()
    {
        solitaire = FindObjectOfType<Solitaire>();
        slot1 = this.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (clickCount == 1)
        {
            timer += Time.deltaTime;
        }
        if (clickCount == 3)
        {
            timer = 0;
            clickCount = 1;
        }
        if (timer > doubleClickTime)
        {
            timer = 0;
            clickCount = 0;
        }

        GetMouseClick();
    }

    void GetMouseClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            clickCount++;

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
            }
        }
    }

    void Deck()
    {
        // deck click actions
        print("Clicked on deck");
        solitaire.DealFromDeck();
        slot1 = this.gameObject;

    }
    
    void Card(GameObject selected)
    {
        // card click actions
        // if card is selected need to chck it against the second card clicked
        
        print("Clicked on Card");

            if (slot1 == this.gameObject) // not null because we pass in this gameObject instead
            {
                slot1 = selected;
            }

            // if there is already a card selected (and it is not the same card)
            else if (slot1 != selected)
            {
                // if the new card is eligable to stack on the old card
                if (Stackable(selected))
                {
                    Stack(selected);
                    //TODO if stacked remove the bottom card from the array 
                }
                else
                {
                    // select the new card
                    slot1 = selected;
                }
            }

            else if (slot1 == selected) // if the same card is clicked twice
            {
                if (DoubleClick())
                {
                    // attempt auto stack
                    AutoStack(selected);
                }
            }

    }

    bool Stackable(GameObject selected)
    {
        Selectable s1 = slot1.GetComponent<Selectable>();
        Selectable s2 = selected.GetComponent<Selectable>();
        // compare them to see if they stack

        if (s1.suit == s2.suit || s1.value == s2.value )
        {
           return true;
        }
        else
        {
           return false;
        }
             
    }

    void Stack(GameObject selected)
    {
        
        Selectable s1 = slot1.GetComponent<Selectable>();
        Selectable s2 = selected.GetComponent<Selectable>();
        
        int listPos = 0;
        
        string cardName = s2.name;

        listPos = solitaire.dealtCards.IndexOf(cardName);   
        solitaire.dealtCards.Remove(cardName);   

        Destroy(GameObject.Find(cardName));        

        cardName = s1.name;
        solitaire.dealtCards.Remove(cardName); 
        solitaire.dealtCards.Insert(listPos,cardName); 

        solitaire.MoveCards();
        

        // after completing move reset slot1 to be essentially null as being null will break the logic
        slot1 = this.gameObject;

    }
    
    
    bool DoubleClick()
    {
        if (timer < doubleClickTime && clickCount == 2)
        {
            print("Double Click");
            return true;
        }
        else
        {
            return false;
        }
    }

    void AutoStack(GameObject selected) //TODO
    {
        for (int i = 0; i < solitaire.topPos.Length; i++)
        {
            Selectable stack = solitaire.topPos[i].GetComponent<Selectable>();
            if (selected.GetComponent<Selectable>().value == 1) // if it is an Ace
            {
                if (solitaire.topPos[i].GetComponent<Selectable>().value == 0) // and the top position is empty
                {
                    slot1 = selected;
                    Stack(stack.gameObject); // stack the ace up top
                    break;                  // in the first empty position found
                }
            }
            else
            {
                if ((solitaire.topPos[i].GetComponent<Selectable>().suit == slot1.GetComponent<Selectable>().suit) && (solitaire.topPos[i].GetComponent<Selectable>().value == slot1.GetComponent<Selectable>().value - 1))
                {
                    // if it is the last card (if it has no children)
                    if (HasNoChildren(slot1))
                    {
                        slot1 = selected;
                        // find a top spot that matches the conditions for auto stacking if it exists
                        string lastCardname = stack.suit + stack.value.ToString();
                        if (stack.value == 1)
                        {
                            lastCardname = stack.suit + "A";
                        }
                        if (stack.value == 11)
                        {
                            lastCardname = stack.suit + "J";
                        }
                        if (stack.value == 12)
                        {
                            lastCardname = stack.suit + "Q";
                        }
                        if (stack.value == 13)
                        {
                            lastCardname = stack.suit + "K";
                        }
                        GameObject lastCard = GameObject.Find(lastCardname);
                        Stack(lastCard);
                        break;
                    }
                }
            }



        }
    }

    bool HasNoChildren(GameObject card)
    {
        int i = 0;
        foreach (Transform child in card.transform)
        {
            i++;
        }
        if (i == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}
