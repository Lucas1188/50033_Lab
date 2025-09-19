using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
public class ScoringManager : Singleton<ScoringManager>
{
    public delegate void PushUpdateScore(string sender_key, int delta_score);
    public ConcurrentDictionary<string,IScoreConsumer> ScoreConsumers = new();
    public List<(string,IScoreProducer)> ScoreProducers = new();
    private ConcurrentDictionary<string, Scores> _score_bag = new();
    void Start()
    {
        
    }
    public void RegisterConsumer(IScoreConsumer consumer)
    {
        var key = consumer.OnRegisterConsumer();
        ScoreConsumers.TryAdd(key, consumer);
    }
    public void RegisterProducer(IScoreProducer producer)
    {
        var key = producer.RegisterProducer(this, OnPushUpdateScore);
        var need_polling = producer.ShouldPollEveryFrame();
        if (need_polling)
        {
            ScoreProducers.Add((key, producer));
        }

    }
    void Update()
    {
        foreach (var producer in ScoreProducers)
        {
            var delta_score = producer.Item2.UpdateScore();
            if (_score_bag.ContainsKey(producer.Item1))
            {
                _score_bag[producer.Item1] += Scores.Zero with { CurrentScore = delta_score };
            }
        }
    }
    private void OnPushUpdateScore(string key, int delta_score)
    {
        if (_score_bag.ContainsKey(key))
        {
            var old_score = _score_bag[key];
            _score_bag[key] = old_score with
            {
                CurrentScore = old_score.CurrentScore + delta_score
            };
        }
    }
    public void AddScore(string key, Scores score)
    {
        if (_score_bag.ContainsKey(key))
        {
            _score_bag[key] += score;
        }
    }
    public Scores GetScore(string[] keys)
    {
        var retval = Scores.Zero;
        foreach (var k in keys)
        {
            var s = GetScore(k);
            if (s != null)
            {
                retval += s;
            }
        }
        return retval;
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