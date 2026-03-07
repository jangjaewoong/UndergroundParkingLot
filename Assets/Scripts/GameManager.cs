using UnityEngine;


class GameManager : MonoBehaviour
{
    public enum GameState
    {
        
    }

    public static GameState CurrentState { get; private set; }

    public static void SetState(GameState newState)
    {
        CurrentState = newState;
    }
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
