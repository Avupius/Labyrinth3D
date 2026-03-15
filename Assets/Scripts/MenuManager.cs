using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Verwaltet das Hauptmenu
/// Zeigt Best-Time und Statistiken
/// </summary>
public class MenuManager : MonoBehaviour
{
    [Header("Menu UI")]
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button resetStatsButton;
    [SerializeField] private TextMeshProUGUI bestTimeDisplay;
    [SerializeField] private TextMeshProUGUI statsDisplay;
    
    [Header("Game")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private LabyrinthGenerator labyrinthGenerator;
    [SerializeField] private Level level;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private ScoreManager scoreManager;
    
    void Start()
    {
        if (scoreManager == null)
        {
           Debug.LogError("MenuManager: ScoreManager nicht gesetzt");
        }
        
        if (uiManager == null)
        {
            Debug.LogError("MenuManager: UIManager nicht gesetzt");
        }
        
        // Button-Events verbinden
        if (startGameButton != null)
        {
            startGameButton.onClick.AddListener(StartGameButtonPressed);
        }
        
        if (resetStatsButton != null)
        {
            resetStatsButton.onClick.AddListener(ResetStats);
        }
        
        // Beim Start: Menu zeigen, Spiel nicht starten
        ShowMenu();
        
        
        // Zeige Statistiken
        UpdateStatisticsDisplay();
        
        Debug.Log("MenuManager gestartet - Menu sichtbar");
    }
    
    /// <summary>
    /// Wird periodisch aufgerufen um Statistiken zu aktualisieren
    /// </summary>
    void Update()
    {
        // Aktualisiere Statistiken live im Menu
        if (menuPanel != null && menuPanel.activeInHierarchy)
        {
            UpdateStatisticsDisplay();
        }
    }

    private void ShowMenu()
    {
        if (menuPanel != null)
        {
            menuPanel.SetActive(true);
            Debug.Log("Menu angezeigt");
        }
        
        if (uiManager != null)
        {
            uiManager.ShowMenuUI();
        }
    }

    private void HideMenu()
    {
        if (menuPanel != null)
        {
            menuPanel.SetActive(false);
            Debug.Log("Menu versteckt");
        }
    }

    /// <summary>
    /// Wird aufgerufen, wenn der Spieler "Start" drueckt.
    /// Generiert das Labyrinth und startet das Spiel.
    /// </summary>
    public void StartGameButtonPressed()
    {
        Debug.Log("=== SPIEL WIRD GESTARTET ===");
    
        // 1. Labyrinth generieren
        if (labyrinthGenerator != null)
        {
            labyrinthGenerator.GenerateMaze();
        
            // 2. StartPosition vom Generator bekommen
            Transform startPos = labyrinthGenerator.GetStartPosition();
            if (level != null && startPos != null)
            {
                level.SetStartPosition(startPos);
            }
        }
    
        // 3. Menu verstecken
        HideMenu();
    
        // 4. Spiel starten
        if (gameManager != null)
        {
            gameManager.StartGame();
        }
    }

    
    /// <summary>
    /// Wird vom GameManager aufgerufen, wenn das Spiel vorbei ist (Level gewonnen).
    /// Zeigt das Menu wieder an.
    /// </summary>
    public void GameOver()
    {
        Debug.Log("Spiel vorbei - Menu wird wieder angezeigt");
        ShowMenu();
        
        // Aktualisiere Statistiken
        UpdateStatisticsDisplay();
    }
    
    /// <summary>
    /// Aktualisiert die Statistik-Anzeige im Menu
    /// </summary>
    private void UpdateStatisticsDisplay()
    {
        if (scoreManager != null)
        {
            // Zeige Best-Time
            if (bestTimeDisplay != null)
            {
                bestTimeDisplay.text = "Beste Zeit: " + scoreManager.GetBestTimeFormatted();
            }
            
            // Zeige Statistiken
            if (statsDisplay != null)
            {
                string stats = string.Format(
                    "Spiele: {0} | Siege: {1}",
                    scoreManager.GetTotalPlays(),
                    scoreManager.GetTotalWins()
                );
                statsDisplay.text = stats;
            }
        }
    }
    
    /// <summary>
    /// Setzt alle Statistiken zurück
    /// </summary>
    public void ResetStats()
    {
        if (scoreManager != null)
        {
            scoreManager.ResetAllScores();
            UpdateStatisticsDisplay();
            Debug.Log("Alle Statistiken zurückgesetzt!");
        }
    }
    public void QuitGame()
    {
        Debug.Log("Spiel wird beendet...");
    
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }
}
