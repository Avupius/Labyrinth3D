using UnityEngine;
using TMPro;

/// <summary>
/// Verwaltet alle UI-Elemente während des Spiels und im Menu
/// Zeigt aktuelle Zeit, Best-Time und Statistiken
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("Game UI References")]
    [SerializeField] private GameObject gameUIPanel;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI bestTimeText;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private CanvasGroup gameUIPanelCanvasGroup;
    
    [Header("Menu UI References")]
    [SerializeField] private GameObject menuUIPanel;
    [SerializeField] private TextMeshProUGUI menuBestTimeText;
    [SerializeField] private TextMeshProUGUI menuStatsText;
    
    [Header("Settings")]
    [SerializeField] private float uiFadeInDuration = 0.5f;

    [Header("Manager References")] 
    [SerializeField] private GameManager gameManager;
    [SerializeField ] private ScoreManager scoreManager;
    
    private float displayTimer = 0f;
    private bool isGameActive = false;
    
    void Start()
    {
        if (gameManager == null)
        {
            Debug.LogError("UIManager: GameManager wurde nicht gesetzt");
            return;
        }

        if (scoreManager == null)
        {
            Debug.LogError("UIManager: ScoreManager wurde nicht gesetzt");
        }
        
        ShowMenuUI();
        displayTimer = 0f;
        isGameActive = false;
        
        Debug.Log("UIManager: Initialisiert");
    }
    
    void Update()
    {
        if (gameManager != null && gameManager.IsGameRunning)
        {
            // Spiel läuft - aktualisiere Timer
            displayTimer += Time.deltaTime;
            UpdateGameTimer(displayTimer);
        }
        else if (isGameActive)
        {
            // Spiel ist gerade zu Ende gegangen
            isGameActive = false;
            displayTimer = 0f;
        }
    }
    
    /// <summary>
    /// Zeigt die Game-UI an wenn Spiel startet
    /// </summary>
    public void ShowGameUI()
    {
        // Verstecke Menu-UI
        if (menuUIPanel != null)
        {
            menuUIPanel.SetActive(false);
        }
        
        // Zeige Game-UI
        if (gameUIPanel != null)
        {
            gameUIPanel.SetActive(true);
        }
        
        // Reset Timer
        displayTimer = 0f;
        isGameActive = true;
        
        // Zeige Best-Time im Game-UI
        if (bestTimeText != null && scoreManager != null)
        {
            bestTimeText.text = "Best: " + scoreManager.GetBestTimeFormatted();
        }
        
        // Status-Text
        if (statusText != null)
        {
            statusText.text = "SPIEL LÄUFT";
            statusText.color = Color.white;
        }
        
        Debug.Log("UIManager: Game-UI angezeigt");
    }
    
    /// <summary>
    /// Zeigt die Menu-UI an
    /// </summary>
    public void ShowMenuUI()
    {
        // Verstecke Game-UI
        if (gameUIPanel != null)
        {
            gameUIPanel.SetActive(false);
        }
        
        // Zeige Menu-UI
        if (menuUIPanel != null)
        {
            menuUIPanel.SetActive(true);
        }
        
        // Aktualisiere Menu-Statistiken IMMER wenn Menu gezeigt wird
        UpdateMenuStatistics();
        
        displayTimer = 0f;
        isGameActive = false;
        
        Debug.Log("UIManager: Menu-UI angezeigt");
    }
    
    /// <summary>
    /// Aktualisiert die Statistiken im Menu
    /// </summary>
    private void UpdateMenuStatistics()
    {
        if (scoreManager != null)
        {
            if (menuBestTimeText != null)
            {
                menuBestTimeText.text = "Beste Zeit: " + scoreManager.GetBestTimeFormatted();
            }
            
            if (menuStatsText != null)
            {
                string stats = string.Format("Spiele: {0} | Siege: {1}", 
                    scoreManager.GetTotalPlays(), 
                    scoreManager.GetTotalWins());
                menuStatsText.text = stats;
            }
        }
    }
    
    /// <summary>
    /// Aktualisiert den Timer im Game-UI
    /// </summary>
    private void UpdateGameTimer(float elapsedTime)
    {
        if (timerText != null)
        {
            int minutes = (int)(elapsedTime / 60f);
            int seconds = (int)(elapsedTime % 60f);
            int milliseconds = (int)((elapsedTime % 1f) * 100f);
            timerText.text = string.Format("Zeit: {0:D2}:{1:D2}.{2:D2}", minutes, seconds, milliseconds);
        }
    }
    
    /// <summary>
    /// Wird vom GameManager aufgerufen wenn Level gewonnen wurde
    /// </summary>
    public void OnLevelWon(float levelTime)
    {
        // Speichere die Zeit
        if (scoreManager != null)
        {
            scoreManager.SaveLevelTime(levelTime);
        }
        
        // Zeige Gewonnen-Nachricht
        if (statusText != null)
        {
            bool isNewBestTime = levelTime < scoreManager.GetBestTime();
            
            if (isNewBestTime)
            {
                statusText.text = "NEUE BESTE ZEIT! " + ScoreManager.FormatTime(levelTime);
                statusText.color = Color.yellow;
            }
            else
            {
                statusText.text = "GEWONNEN! Zeit: " + ScoreManager.FormatTime(levelTime);
                statusText.color = Color.green;
            }
        }
        
        Debug.Log("UIManager: Level gewonnen!");
    }
    
    /// <summary>
    /// Wird vom GameManager aufgerufen wenn Ball in Loch faellt
    /// </summary>
    public void OnBallFellInHole()
    {
        if (statusText != null)
        {
            statusText.text = "BALL FÄLLT!";
            statusText.color = Color.softBlue;
        }
    }
    
    /// <summary>
    /// Zeigt eine beliebige Nachricht an
    /// </summary>
    public void ShowStatus(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
            Debug.Log("Status: " + message);
        }
    }
    
    /// <summary>
    /// Setzt den Timer zurück
    /// </summary>
    public void ResetTimer()
    {
        displayTimer = 0f;
        if (timerText != null)
        {
            timerText.text = "Zeit: 00:00.00";
        }
    }
    
}