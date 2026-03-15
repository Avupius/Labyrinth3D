using UnityEngine;
using System.Collections.Generic;

public class Level : MonoBehaviour
{
    [SerializeField] private Transform startPosition;
    [SerializeField] private Transform exitPosition;
    
    // Neue Hole-Verwaltung
    private List<HoleTrigger> holeTriggers = new List<HoleTrigger>();
    private List<Vector3> holePositions = new List<Vector3>();
    private static readonly Vector3 EXIT_SIZE = new Vector3(1, 0.5f, 1);
    
    void Start()
    {
        if (startPosition == null)
            Debug.LogError("Level: Start Position nicht gesetzt!");
        else
        {
            Debug.Log("Level: Start Position = " + startPosition.position);
        }
    }
    
    /// <summary>
    /// Setzt eine neue Start-Position.
    /// </summary>
    public void SetStartPosition(Transform newStartPos)
    {
        startPosition = newStartPos;
        Debug.Log("StartPosition gesetzt zu: " + newStartPos.position);
    }
    
    /// <summary>
    /// Gibt die Start-Position des Levels zurueck.
    /// </summary>
    public Vector3 GetStartPosition()
    {
        return startPosition != null ? startPosition.position : Vector3.zero;
    }
    
    /// <summary>
    /// Setzt die Hole-Trigger vom LabyrinthGenerator
    /// </summary>
    public void SetHoleTriggers(List<HoleTrigger> triggers)
    {
        holeTriggers = triggers;
        Debug.Log($"Level: {holeTriggers.Count} HoleTriggers empfangen");
    }
    
    /// <summary>
    /// Setzt die Hole-Positionen vom LabyrinthGenerator
    /// </summary>
    public void SetHolePositions(List<Vector3> positions)
    {
        holePositions = positions;
        Debug.Log($"Level: {holePositions.Count} Hole-Positionen empfangen");
    }
    
    /// <summary>
    /// Prüft ob Ball am Exit ist
    /// </summary>
    public bool CheckBallAtExit(Vector3 ballPosition)
    {
        if (exitPosition == null) return false;
        
        Vector3 exitPos = exitPosition.position;
        
        bool isInExit =
            Mathf.Abs(ballPosition.x - exitPos.x) < EXIT_SIZE.x / 2f &&
            Mathf.Abs(ballPosition.y - exitPos.y) < EXIT_SIZE.y / 2f &&
            Mathf.Abs(ballPosition.z - exitPos.z) < EXIT_SIZE.z / 2f;
        
        return isInExit;
    }
    
    /// <summary>
    /// Setzt alle HoleTrigger zurück (wird nach Neustart aufgerufen)
    /// </summary>
    public void ResetHoleTriggers()
    {
        foreach (HoleTrigger trigger in holeTriggers)
        {
            if (trigger != null)
            {
                trigger.Reset();
            }
        }
    }
}