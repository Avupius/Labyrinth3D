using UnityEngine;

/// <summary>
/// Verwaltet den Spielfluss und die Spiellogik
/// Kommuniziert mit UIManager für UI-Updates
/// </summary>
public class GameManager : MonoBehaviour
{
    [SerializeField] private Ball ball;
    [SerializeField] private Level level;
    [SerializeField] private MenuManager menuManager;
    [SerializeField] private UIManager uiManager;

    private const float HOLE_RESTART_DELAY = 1.5f;
    private const float WIN_MENU_DELAY = 1.5f;
    
    private float elapsedTime = 0f;
    private bool isGameRunning = false;
    private bool hasWon = false;
    
    public bool IsGameRunning => isGameRunning;
    public float ElapsedTime => elapsedTime;
    

    void Start()
    {   
        ResetGameState();

        if (uiManager == null)
        {
            Debug.LogError("GameManager: UIManager nicht gesetzt");
            return;
        }
        
    }
    void Update()
    {
        if (!isGameRunning || hasWon)
            return;

        UpdateGameTime();
        CheckLevelCompletion();
    }
    
    /// <summary>
    /// Wird vom MenuManager aufgerufen wenn Spiel gestartet wird
    /// </summary>
    public void StartGame()
    {
        
        if (ball == null || level == null)
        {
            Debug.Log("GameManager: Ball oder Level nicht gesetzt");
            return;
        }
        
        isGameRunning = true;
        hasWon = false;
        elapsedTime = 0f;
        
        InitializeLevel();
        UpdateGameUI();
        
        Debug.Log("GameManager: Spiel gestartet!");
    }

    private void UpdateGameUI()
    {
        if (uiManager != null)
        {
            uiManager.ShowGameUI();
            uiManager.ResetTimer();
        }
    }
    
    private void InitializeLevel()
    {
        ball.ResetPosition(level.GetStartPosition());
        level.ResetHoleTriggers();
    }
    
    /// <summary>
    /// Wird vom HoleTrigger aufgerufen, wenn der Ball in Loch fällt
    /// </summary>
    public void BallFellInHole()
    {
        Debug.Log("GameManager: Ball ist in Loch gefallen!");
        
        // Zeige UI-Feedback
        if (uiManager != null)
        {
            uiManager.OnBallFellInHole();
            
        }
        
        RestartLevel();
        
    }
    
    void RestartLevel()
    {
        isGameRunning = false;
        // Verzögerung um UI-Animation zu zeigen
        Invoke("ResetBall", HOLE_RESTART_DELAY);
    }
    
    void ResetBall()
    {
        if (ball == null || level == null)
        {
            Debug.Log("GameManager: Der Ball oder Level kann nicht zurückgesetzt werden");
            return;
        }
            
        InitializeLevel();
        isGameRunning = true;
        elapsedTime = 0f;
        
        // Reset UI
        if (uiManager != null)
        {
            uiManager.ResetTimer();
            uiManager.ShowStatus("SPIEL LÄUFT");
        }
        
        Debug.Log("Ball zurückgesetzt, Spiel läuft weiter");
    }
    
    public void CompletedLevel()
    {
        // Verhindere doppelte Aufrufe
        if (hasWon)
            return;

        hasWon = true;
        isGameRunning = false;


        // Benachrichtige UI
        if (uiManager != null)
        {
            uiManager.OnLevelWon(elapsedTime);
        }

        // Nach 2 Sekunden Menu zeigen
        Invoke("ShowMenuAfterWin", WIN_MENU_DELAY);
    }
    
    void ShowMenuAfterWin()
    {
        if (menuManager != null)
        {
            menuManager.GameOver();
        }
        
        // Aktualisiere UI Statistiken
        if (uiManager != null)
        {
            uiManager.ShowMenuUI();
        }
    }
    
    private void ResetGameState()
    {
        isGameRunning = false;
        hasWon = false;
        elapsedTime = 0f;
    }

    private void UpdateGameTime()
    {
        elapsedTime += Time.deltaTime;
    }

    private void CheckLevelCompletion()
    {
        if (level.CheckBallAtExit(ball.GetPosition()))
        {
            CompletedLevel();
        }
    }
    
}