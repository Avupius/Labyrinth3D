using UnityEngine;

/// <summary>
/// Verwaltet Best-Times und speichert sie persistent
/// Verwendet PlayerPrefs für einfache Datenspeicherung
/// </summary>
public class ScoreManager : MonoBehaviour
{
    private static ScoreManager instance;
    
    [SerializeField] private string bestTimeKey = "BestTime";
    [SerializeField] private string totalPlaysKey = "TotalPlays";
    [SerializeField] private string totalWinsKey = "TotalWins";
    
    private float currentLevelTime = 0f;
    private float bestTime = float.MaxValue;
    private int totalPlays = 0;
    private int totalWins = 0;
    
    void Awake()
    {
        // Singleton Pattern
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        LoadScores();
    }
    
    /// <summary>
    /// Laden der Best-Time aus PlayerPrefs
    /// </summary>
    public void LoadScores()
    {
        if (PlayerPrefs.HasKey(bestTimeKey))
        {
            bestTime = PlayerPrefs.GetFloat(bestTimeKey);
        }
        else
        {
            bestTime = float.MaxValue;
        }
        
        totalPlays = PlayerPrefs.GetInt(totalPlaysKey, 0);
        totalWins = PlayerPrefs.GetInt(totalWinsKey, 0);
        
        Debug.Log($"ScoreManager: Best Time = {FormatTime(bestTime)}, Wins = {totalWins}, Plays = {totalPlays}");
    }
    
    /// <summary>
    /// Speichert die Best-Time wenn das Level gewonnen wurde
    /// </summary>
    public void SaveLevelTime(float levelTime)
    {
        currentLevelTime = levelTime;
        totalPlays++;
        
        // Prüfe ob neue Best-Time
        if (levelTime < bestTime)
        {
            bestTime = levelTime;
            PlayerPrefs.SetFloat(bestTimeKey, bestTime);
            Debug.Log($"ScoreManager: NEUE BESTE ZEIT! {FormatTime(bestTime)}");
        }
        
        totalWins++;
        PlayerPrefs.SetInt(totalPlaysKey, totalPlays);
        PlayerPrefs.SetInt(totalWinsKey, totalWins);
        PlayerPrefs.Save();
        
        Debug.Log($"ScoreManager: Level-Zeit = {FormatTime(levelTime)}, Beste Zeit = {FormatTime(bestTime)}");
    }
    
    /// <summary>
    /// Formatiert eine Zeit im Format MM:SS.MS
    /// </summary>
    public static string FormatTime(float timeInSeconds)
    {
        if (timeInSeconds >= float.MaxValue)
            return "-- : --.--";
        
        int minutes = (int)(timeInSeconds / 60f);
        int seconds = (int)(timeInSeconds % 60f);
        int milliseconds = (int)((timeInSeconds % 1f) * 100f);
        
        return string.Format("{0:D2}:{1:D2}.{2:D2}", minutes, seconds, milliseconds);
    }
    
    /// <summary>
    /// Gibt die beste Zeit zurück
    /// </summary>
    public float GetBestTime()
    {
        return bestTime;
    }
    
    /// <summary>
    /// Gibt die formattierte beste Zeit zurück
    /// </summary>
    public string GetBestTimeFormatted()
    {
        return FormatTime(bestTime);
    }
    
    /// <summary>
    /// Gibt die aktuelle Level-Zeit zurück
    /// </summary>
    public float GetCurrentLevelTime()
    {
        return currentLevelTime;
    }
    
    /// <summary>
    /// Gibt die Anzahl der Siege zurück
    /// </summary>
    public int GetTotalWins()
    {
        return totalWins;
    }
    
    /// <summary>
    /// Gibt die Anzahl aller Versuche zurück
    /// </summary>
    public int GetTotalPlays()
    {
        return totalPlays;
    }
    
    /// <summary>
    /// Setzt alle Scores zurück
    /// </summary>
    public void ResetAllScores()
    {
        PlayerPrefs.DeleteKey(bestTimeKey);
        PlayerPrefs.DeleteKey(totalPlaysKey);
        PlayerPrefs.DeleteKey(totalWinsKey);
        PlayerPrefs.Save();
        
        bestTime = float.MaxValue;
        totalPlays = 0;
        totalWins = 0;
        
        Debug.Log("ScoreManager: Alle Scores zurückgesetzt!");
    }
}