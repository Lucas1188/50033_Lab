using UnityEngine;

public class TimeScoring : MonoBehaviour, IScoreProducer
{
    public static readonly string key = "TimeScore";
    public int PointsPerSecond = 1;
    public int GameOver()
    {
        return  PointsPerSecond * (int)GameManager.Instance.GetRoundTime().TotalSeconds;
    }
    private ScoringManager _scoringManager;
    private ScoringManager.PushUpdateScore _pushUpdateScoredelegate;
    public string RegisterProducer(ScoringManager scoringManager, ScoringManager.PushUpdateScore pushUpdateScore_delegate)
    {
        _scoringManager = scoringManager;
        _pushUpdateScoredelegate = pushUpdateScore_delegate;
        return key;
    }

    public bool ShouldPollEveryFrame()
    {
        return false;
    }

    public int UpdateScore()
    {
        return 0;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        ScoringManager.Instance.RegisterProducer(this);
        GameManager.Instance.OnGameOver += ()=>_pushUpdateScoredelegate(key, GameOver());
    }
    void Start()
    {
    }
}
