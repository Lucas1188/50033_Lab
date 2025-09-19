using UnityEngine;

public class UI_GameoverScreen : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        GameManager.Instance.OnGameOver -= HandleGameOver;
        GameManager.Instance.OnGameOver += HandleGameOver;
    }
    void HandleGameOver()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
