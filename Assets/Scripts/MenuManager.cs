using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [Header("Menu UI")]
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private Button startGameButton;
    
    [Header("Game")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private LabyrinthGenerator labyrinthGenerator;
    [SerializeField] private Level level; 
    
    private bool gameStarted = false;

    void Start()
    {
        // Beim Start: Menu zeigen, Spiel nicht starten
        ShowMenu();
        gameStarted = false;
        
        // Button verbinden
        if (startGameButton != null)
        {
            startGameButton.onClick.AddListener(StartGameButtonPressed);
        }
        
        Debug.Log("MenuManager gestartet - Menu sichtbar");
    }

    private void ShowMenu()
    {
        if (menuPanel != null)
        {
            menuPanel.SetActive(true);
            Debug.Log("Menu angezeigt");
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

    public void GameOver()
    {
        Debug.Log("Spiel vorbei - Menu wird wieder angezeigt");
        gameStarted = false;
        ShowMenu();
    }
}