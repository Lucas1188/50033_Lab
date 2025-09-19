using UnityEngine;

public class UI_ScoreTitle : UITitleDynamic,IScoreConsumer
{   
    private string score = string.Format("{0:000000}",0);
    private int value_score = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        ScoringManager.Instance.RegisterConsumer(this);// Register on start
        WriteContent("\n" + string.Format("{0:000000}", value_score));
    }
    public void ScoreChanged(int delta_score, int total_score)
    {
        value_score =total_score;
        WriteContent("\n" + string.Format("{0:000000}", value_score));
    }

    public int OnScoreManagerRegistration(ScoringManager scoreManager, bool success)
    {
        if (success)
        {

        }
        return 0;
    }

    public string OnRegisterConsumer()
    {
        return nameof(UI_ScoreTitle);
    }
}
