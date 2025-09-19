using Unity.VisualScripting;

public record Scores
{
    public int CurrentScore;
    public int HighScore;
    public static Scores Zero => new Scores
    {
        CurrentScore = 0,
        HighScore = 0
    };
    public static Scores operator +(Scores left, Scores right)
    {
        return new Scores() with
        {
            CurrentScore = left.CurrentScore + right.CurrentScore,
            HighScore = left.HighScore + right.HighScore
        };
    }
    public static Scores operator -(Scores left, Scores right)
    {
        return new Scores() with
        {
            CurrentScore = left.CurrentScore - right.CurrentScore,
            HighScore = left.HighScore - right.HighScore
        };
    }
}