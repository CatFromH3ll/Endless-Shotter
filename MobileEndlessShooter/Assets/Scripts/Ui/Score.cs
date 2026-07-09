using TMPro;
using UnityEngine;

public class Score : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI  scoreText;
    public int score {get; set;}
    public void UpdateScore(int scoreToAdd)
    {
        score += scoreToAdd;
        scoreText.text = "Score: " + score.ToString();
    }
}
