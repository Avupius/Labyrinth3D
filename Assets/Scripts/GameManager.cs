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
    
    private float elapsedTime = 0f;
    private bool isGameRunning = false;
    private bool hasWon = false;
    
    void Start()
    {
        // Finde UIManager wenn nicht gesetzt
        if (uiManager == null)
        {
            uiManager = FindObjectOfType<UIManager>();
        }
        
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
            level.ResetHoleTriggers();
            Debug.Log("Ball zur Startposition gesetzt");
        }
        
        // Zeige Game-UI
        if (uiManager != null)
        {
            uiManager.ShowGameUI();
            uiManager.ResetTimer();
        }
        
        Debug.Log("GameManager: Spiel gestartet!");
    }
    
    void Update()
    {
        if (!isGameRunning || hasWon)
            return;
        
        elapsedTime += Time.deltaTime;
        
        // Pruefen ob Ball Exit erreicht
        if (level != null && level.CheckBallAtExit(ball.GetPosition()))
        {
            Debug.Log("GEWONNEN! Zeit: " + elapsedTime.ToString("F2") + "s");
            WinLevel();
            return;
        }
    }
    
    /// <summary>
    /// Wird vom HoleTrigger aufgerufen wenn Ball in Loch faellt
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
        Debug.Log("GameManager: Level wird neu gestartet...");
        Invoke("ResetBall", 1.5f);
    }
    
    void ResetBall()
    {
        if (ball != null && level != null)
        {
            ball.ResetPosition(level.GetStartPosition());
            level.ResetHoleTriggers();
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
    }
    
    public void WinLevel()
    {
        hasWon = true;
        isGameRunning = false;
        
        Debug.Log("Level completed in " + elapsedTime.ToString("F2") + "s");
        
        // Benachrichtige UI
        if (uiManager != null)
        {
            uiManager.OnLevelWon(elapsedTime);
        }
        
        // Nach 2 Sekunden Menu zeigen
        Invoke("ShowMenuAfterWin", 2f);
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
    
    public bool IsGameRunning => isGameRunning;
}