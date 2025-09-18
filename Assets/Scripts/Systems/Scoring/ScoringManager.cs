using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
public class ScoringManager : Singleton<ScoringManager>
{
    public delegate void PushUpdateScore(string sender_key, int delta_score);
    public List<IScoreConsumer> ScoreConsumers = new();
    public List<IScoreProducer> ScoreProducers = new();
    private ConcurrentDictionary<string, Scores> _score_bag = new();
    void Start()
    {
        foreach (var s_producer in ScoreProducers)
        {
            var k = s_producer.RegisterProducer(this, OnPushUpdateScore);
            _score_bag.TryAdd(k, new()
            {
                CurrentScore = 0,
                HighScore = 0, //This should change once we implement the serealization manager 
            });
        }
        foreach (var s_consumer in ScoreConsumers)
        {
            var consumer_key = s_consumer.OnRegisterConsumer();
            if (_score_bag.ContainsKey(consumer_key))
            {
                var initscore = s_consumer.OnScoreManagerRegistration(this, true);
                _score_bag[consumer_key] = new()
                {
                    CurrentScore = initscore,
                    HighScore = initscore//See above
                };
            }
            else
            {
                s_consumer.OnScoreManagerRegistration(this, false);
            }
        }
    }
    private void OnPushUpdateScore(string key, int delta_score)
    {
        if (_score_bag.ContainsKey(key))
        {
            var old_score = _score_bag[key];
            _score_bag[key] =  old_score with
            {
                CurrentScore = old_score.CurrentScore + delta_score
            };
        }
    }
    public Scores GetScore(string key)
    {
        if (_score_bag.ContainsKey(key))
        {
            return _score_bag[key];
        }
        return null;
    }
}