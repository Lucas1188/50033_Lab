using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
#nullable enable
public class GameManager : Singleton<GameManager>
{
    public CameraBounds cameraBounds; 
    private DateTime _startTime;
    private DateTime _endTime = DateTime.MinValue;
    private bool _firstframe = false;
    public delegate void GameRestartHandler();
    public event GameRestartHandler OnGameRestart;
    public delegate void GameStartHandler();
    public event GameStartHandler OnGameStart;
    public delegate void GameOverHandler();
    public event GameOverHandler OnGameOver;
    private int _currentControlling = -1;
    public List<IControllable> controllables =new();
    /// <summary>
    /// Components must register this during the awake call so that it will be ready by Start
    /// </summary>
    /// <param name="controllable"></param>
    public void RegisterControllable(IControllable controllable)
    {
        //ToDo: To add internal states to track current engine execution state
        controllables.Add(controllable);
    }
    public delegate void ControllableCharacterChangedHandler(IControllable controllable);
    public event ControllableCharacterChangedHandler OnControllableCharacterChanged;
    public void SwitchControllable(int idx)
    {
        if (idx >= controllables.Count || idx < 0)
        {
            Debug.LogError("Not a valid controllable");
            return;
        }
        if (_currentControlling >= 0 && _currentControlling < controllables.Count)
        {
            controllables[_currentControlling].OnControlRemoved();    
        }
        controllables[idx].OnTakeControl(); //this can return an error, TODO: Have a default look point or just not have such conditions in prod
        OnControllableCharacterChanged?.Invoke(controllables[idx]);
        _currentControlling = idx;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _firstframe = false;
        SwitchControllable(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (!_firstframe)
        {
            _startTime = DateTime.Now; //Using first frame update so to recover time from load and script execution order
            OnGameStart?.Invoke();
        }
    }
    public void Restart()
    {
        _firstframe = false;
        _endTime = DateTime.MinValue;
        OnGameRestart?.Invoke(); //Should be synchronous
        SceneManager.LoadScene(0); //We do this now but ideally this should be a same scene just that we sandbox the environment
    }
    public void InvokeGameOver(object? caller, string reason)
    {
        _endTime = DateTime.Now;
        OnGameOver?.Invoke(); //Synchronous
    }
    public TimeSpan GetRoundTime()
    {
        if (_startTime > DateTime.Now) return TimeSpan.Zero;
        if (_endTime < _startTime) return DateTime.Now - _startTime;
        return _endTime - _startTime;
    }
}
