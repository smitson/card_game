using UnityEngine;
using UnityEngine.UI;

//TODO use this to generate barcharts re number of games etc 

public class ScoreScreenManager : MonoBehaviour
{
    public GameObject scoreScreen;
    public GameObject barGraphPrefab;
    public Transform barGraphContainer;
    public ScoreData[] scoreData;

    private void Start()
    {
        scoreScreen.SetActive(false); // Hide the score screen initially
    }

    public void ShowScoreScreen()
    {
        scoreScreen.SetActive(true);
        GenerateBarGraphs();
    }

    public void HideScoreScreen()
    {
        scoreScreen.SetActive(false);
    }

    private void GenerateBarGraphs()
    {
        foreach (ScoreData data in scoreData)
        {
            // Instantiate the bar graph prefab
            GameObject barGraphObj = Instantiate(barGraphPrefab, barGraphContainer);
            BarGraph barGraph = barGraphObj.GetComponent<BarGraph>();

            // Set the score value and label
            barGraph.SetValue(data.score);
            barGraph.SetLabel(data.label);
        }
    }
}

public class BarGraph : MonoBehaviour
{
    public Image barImage;
    public Text valueText;
    public Text label;

    public void SetValue(float value)
    {
        // Set the width of the bar based on the value
        barImage.rectTransform.sizeDelta = new Vector2(value, barImage.rectTransform.sizeDelta.y);
        valueText.text = value.ToString();
    }

    public void SetLabel(string labelText)
    {
        label.text = labelText;
    }
}

[System.Serializable]
public class ScoreData
{
    public string label;
    public float score;
}
