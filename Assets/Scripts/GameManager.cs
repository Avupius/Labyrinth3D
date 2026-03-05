using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Ball ball;
    [SerializeField] private Level level;
    [SerializeField] private MenuManager menuManager;
    
    private float elapsedTime = 0f;
    private bool isGameRunning = false;
    private bool hasWon = false;
    
    void Start()
    {
        // WICHTIG: Spiel startet NICHT automatisch!
        isGameRunning = false;
        hasWon = false;
        elapsedTime = 0f;
        
        Debug.Log("GameManager: Bereit (wartet auf StartGame() vom Menu)");
    }
    
    /// <summary>
    /// Wird vom MenuManager aufgerufen wenn Spiel gestartet wird
    /// </summary>
    public void StartGame()
    {
        isGameRunning = true;
        hasWon = false;
        elapsedTime = 0f;
        
        if (ball != null && level != null)
        {
            ball.ResetPosition(level.GetStartPosition());
            Debug.Log("Ball zur Startposition gesetzt");
        }
        
        Debug.Log("GameManager: Spiel gestartet!");
    }
    
    void Update()
    {
        if (!isGameRunning || hasWon)
            return;
        
        elapsedTime += Time.deltaTime;
        
        // Pruefen ob Ball in Loch faellt
        if (level != null && level.CheckBallInHole(ball.GetPosition()))
        {
            Debug.Log("Ball faellt in Loch!");
            RestartLevel();
            return;
        }
        
        // Pruefen ob Ball Exit erreicht
        if (level != null && level.CheckBallAtExit(ball.GetPosition()))
        {
            Debug.Log("GEWONNEN! Zeit: " + elapsedTime.ToString("F2") + "s");
            WinLevel();
            return;
        }
    }
    
    void RestartLevel()
    {
        isGameRunning = false;
        Invoke("ResetBall", 1.5f);
    }
    
    void ResetBall()
    {
        if (ball != null && level != null)
        {
            ball.ResetPosition(level.GetStartPosition());
            isGameRunning = true;
            elapsedTime = 0f;
        }
    }
    
    void WinLevel()
    {
        hasWon = true;
        isGameRunning = false;
        
        Debug.Log("Level completed in " + elapsedTime.ToString("F2") + "s");
        
        // Nach 2 Sekunden Menu zeigen
        Invoke("ShowMenuAfterWin", 2f);
    }
    
    void ShowMenuAfterWin()
    {
        if (menuManager != null)
        {
            menuManager.GameOver();
        }
    }
    
    public bool IsGameRunning => isGameRunning;
}