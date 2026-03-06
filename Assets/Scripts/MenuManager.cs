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
    
    private ScoreManager scoreManager;
    private bool gameStarted = false;

    void Start()
    {
        // Finde ScoreManager
        scoreManager = FindObjectOfType<ScoreManager>();
        if (scoreManager == null)
        {
            GameObject scoreManagerObj = new GameObject("ScoreManager");
            scoreManager = scoreManagerObj.AddComponent<ScoreManager>();
        }
        
        // Finde UIManager wenn nicht gesetzt
        if (uiManager == null)
        {
            uiManager = FindObjectOfType<UIManager>();
        }
        
        // Beim Start: Menu zeigen, Spiel nicht starten
        ShowMenu();
        gameStarted = false;
        
        // Button-Events verbinden
        if (startGameButton != null)
        {
            startGameButton.onClick.AddListener(StartGameButtonPressed);
        }
        
        if (resetStatsButton != null)
        {
            resetStatsButton.onClick.AddListener(ResetStats);
        }
        
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

    public void QuitGame()
    {
        Debug.Log("Spiel wird beendet...");
    
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }
    
    public void GameOver()
    {
        Debug.Log("Spiel vorbei - Menu wird wieder angezeigt");
        gameStarted = false;
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
}