using UnityEngine;

public class UI_Gameover_TotalScore : UITitleDynamic, IScoreConsumer
{
    public static string key = "totalScoreGameover";
    private ScoringManager scoringManager;
    private int totalScore;
    public string OnRegisterConsumer()
    {
        return key;
    }

    public int OnScoreManagerRegistration(ScoringManager scoreManager, bool success)
    {
        scoringManager = scoreManager;
        return 0;
    }

    public void ScoreChanged(int delta_score, int total_score)
    {
        return; // Not required
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        ScoringManager.Instance.RegisterConsumer(this);
        GameManager.Instance.OnGameOver += HandleGameOver;
        WriteContent("0");
    }
    void HandleGameOver()
    {
        var allscore = scoringManager.GetScore(new string[]{
            TimeScoring.key,
        });
        totalScore = allscore.CurrentScore;
        WriteContent(totalScore.ToString());
    }
    // Update is called once per frame
}
