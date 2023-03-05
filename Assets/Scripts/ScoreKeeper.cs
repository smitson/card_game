using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreKeeper : MonoBehaviour
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

    void Win()
    {
        highScorePanel.SetActive(true);
        print("You have won!");
    }

}
