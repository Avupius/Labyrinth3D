using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Ball ball;
    [SerializeField] private Level level;
    [SerializeField] private SensorInputHandler sensorInput;
    
    private float elapsedTime = 0f;
    private bool isGameRunning = false;
    private bool hasWon = false;
    
    void Start()
    {
        isGameRunning = true;
        ball.ResetPosition(level.GetStartPosition());
        Debug.Log("3D Labyrinth Game Started!");
    }
    
    void Update()
    {
        if (isGameRunning && !hasWon)
        {
            elapsedTime += Time.deltaTime;
            
            /*// Check lose condition
            if (level.CheckBallInHole(ball.GetPosition()))
            {
                Debug.Log("Ball fell in hole!");
                RestartLevel();
                return;
            }
            
            // Check win condition
            if (level.CheckBallAtExit(ball.GetPosition()))
            {
                Debug.Log("You won!");
                WinLevel();
                return;
            }*/
        }
    }
    
    void RestartLevel()
    {
        Invoke("ResetBall", 1.5f);
    }
    
    void ResetBall()
    {
        ball.ResetPosition(level.GetStartPosition());
        elapsedTime = 0f;
    }
    
    void WinLevel()
    {
        hasWon = true;
        isGameRunning = false;
        Debug.Log("Level completed in " + elapsedTime.ToString("F2") + "s");
    }
    
    public bool IsGameRunning => isGameRunning;
}