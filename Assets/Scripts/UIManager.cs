using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameManager gameManager;
    
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI statusText;
    
    private float displayTimer = 0f;
    private bool isGameActive = false;
    
    void Start()
    {
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
            if (gameManager == null)
                Debug.LogError("UIManager: GameManager nicht gefunden!");
        }
        
        // Starte Timer auf 0
        displayTimer = 0f;
        UpdateTimer(0f);
        ShowStatus("Drücke 'Spiel Starten' um zu beginnen");
        
        Debug.Log("UIManager: Initialisiert");
    }
    
    void Update()
    {
        if (gameManager != null && gameManager.IsGameRunning)
        {
            displayTimer += Time.deltaTime;
            UpdateTimer(displayTimer);
        }
        else
        {
            // Wenn Spiel nicht läuft, Timer zurücksetzen
            if (isGameActive)
            {
                displayTimer = 0f;
                isGameActive = false;
            }
        }
    }
    
    public void UpdateTimer(float elapsedTime)
    {
        isGameActive = true;
        
        if (timerText != null)
        {
            int minutes = (int)(elapsedTime / 60f);
            int seconds = (int)(elapsedTime % 60f);
            int milliseconds = (int)((elapsedTime % 1f) * 100f);
            timerText.text = string.Format("Zeit: {0:D2}:{1:D2}.{2:D2}", minutes, seconds, milliseconds);
        }
    }
    
    public void ShowStatus(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
            Debug.Log("Status: " + message);
        }
    }
    
    public void ResetTimer()
    {
        displayTimer = 0f;
        UpdateTimer(0f);
    }
}