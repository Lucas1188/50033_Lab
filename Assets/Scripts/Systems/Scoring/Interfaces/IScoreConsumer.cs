public interface IScoreConsumer
{
    public void ScoreChanged(int delta_score, int total_score);
    public int OnScoreManagerRegistration(ScoringManager scoreManager, bool success);
    public string OnRegisterConsumer();
}