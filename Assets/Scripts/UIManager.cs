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
    
    void Start()
    {
        if (gameManager == null)
            Debug.LogError("UIManager3D: GameManager nicht zugewiesen!");
        
        Debug.Log("UIManager3D: Initialisiert");
    }
    
    void Update()
    {
        if (gameManager != null && gameManager.IsGameRunning)
        {
            displayTimer += Time.deltaTime;
            UpdateTimer(displayTimer);
        }
    }
    
    public void UpdateTimer(float elapsedTime)
    {
        if (timerText != null)
        {
            int minutes = (int)(elapsedTime / 60f);
            int seconds = (int)(elapsedTime % 60f);
            timerText.text = string.Format("Zeit: {0:D2}:{1:D2}", minutes, seconds);
        }
    }
    
    public void ShowStatus(string message)
    {
        if (statusText != null)
            statusText.text = message;
    }
}