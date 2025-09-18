public interface IScoreProducer
{
    /// <summary>
    /// ScoreManager will call this function on Start(). 
    /// Use pushUpdateScore delegate for event driven update of scores. 
    /// It is recommended to also set should poll to false if using this delegate.
    /// </summary>
    /// <param name="scoringManager"></param>
    /// <param name="pushUpdateScore_delegate">Cache this delegate for push type updating of scores</param>
    /// <returns>A string key to identify this producer</returns>
    public string RegisterProducer(ScoringManager scoringManager, ScoringManager.PushUpdateScore pushUpdateScore_delegate);
    /// <summary>
    /// Gameover function should return any score that could only be tabulated on a triggered gameover state. Returned value will be added ontop of every 
    /// </summary>
    /// <returns>Any score to be added from this producer that is only calculated at end of round</returns>
    public int GameOver();
    /// <summary>
    /// Implement update score function. All score producer will be polled each graphical frame. Return false in ShouldPollEveryFrame to only utilize Gameover function
    /// </summary>
    /// <returns>A delta score to be added</returns>
    public int UpdateScore();
    public bool ShouldPollEveryFrame();

}