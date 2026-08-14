using UnityEngine;
using UnityEngine.UI;

public class ScoreScreenManager : MonoBehaviour
{
    public GameObject scoreScreen;
    public GameObject barGraphPrefab;
    public Transform barGraphContainer;

    private void Start()
    {
        scoreScreen.SetActive(false);
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
        // Clear old bars
        foreach (Transform child in barGraphContainer)
            Destroy(child.gameObject);

        var data = new[]
        {
            ("1–5",   PlayerPrefs.GetInt("ScoreRange1to5",   0)),
            ("6–10",  PlayerPrefs.GetInt("ScoreRange6to10",  0)),
            ("11–15", PlayerPrefs.GetInt("ScoreRange11to15", 0)),
            ("16–20", PlayerPrefs.GetInt("ScoreRange16to20", 0)),
        };

        foreach (var (label, count) in data)
        {
            GameObject barObj = Instantiate(barGraphPrefab, barGraphContainer);
            BarGraph bar = barObj.GetComponent<BarGraph>();
            bar.SetValue(count);
            bar.SetLabel(label);
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
        barImage.rectTransform.sizeDelta = new Vector2(value, barImage.rectTransform.sizeDelta.y);
        valueText.text = value.ToString();
    }

    public void SetLabel(string labelText)
    {
        label.text = labelText;
    }
}
