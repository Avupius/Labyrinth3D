using UnityEngine;
using System.Collections.Generic;

public class Level : MonoBehaviour
{
    [SerializeField] private Transform startPosition;
    [SerializeField] private Transform exitPosition;
    [SerializeField] private float exitSize = new Vector3(1, 0.5f, 1).x;
    
    // Neue Hole-Verwaltung
    private List<HoleTrigger> holeTriggers = new List<HoleTrigger>();
    private List<Vector3> holePositions = new List<Vector3>();
    private float holeRadius = 0.8f;
    
    void Start()
    {
        if (startPosition == null)
            Debug.LogError("Level3D: Start Position nicht gesetzt!");
        else
        {
            Debug.Log("Level3D: Start Position = " + startPosition.position);
        }
    }
    
    public void SetStartPosition(Transform newStartPos)
    {
        startPosition = newStartPos;
        Debug.Log("StartPosition gesetzt zu: " + newStartPos.position);
    }
    
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
    /// Prüft ob Ball in eines der generierten Löcher faellt (nur visuell/Debug)
    /// Die echte Erkennung erfolgt über HoleTrigger Collider!
    /// </summary>
    public bool CheckBallInHole(Vector3 ballPosition)
    {
        // Prüfe gegen alle Hole-Positionen
        foreach (Vector3 holePos in holePositions)
        {
            float distance = Vector3.Distance(ballPosition, holePos);
            if (distance < holeRadius)
            {
                Debug.Log($"Ball ist nahe bei Loch: {distance}m");
                return true;
            }
        }
        
        return false;
    }
    
    /// <summary>
    /// Prüft ob Ball am Exit ist
    /// </summary>
    public bool CheckBallAtExit(Vector3 ballPosition)
    {
        if (exitPosition == null) return false;
        
        Vector3 exitPos = exitPosition.position;
        Vector3 exitSize3D = new Vector3(1, 0.5f, 1);
        
        bool isInExit =
            Mathf.Abs(ballPosition.x - exitPos.x) < exitSize3D.x / 2f &&
            Mathf.Abs(ballPosition.y - exitPos.y) < exitSize3D.y / 2f &&
            Mathf.Abs(ballPosition.z - exitPos.z) < exitSize3D.z / 2f;
        
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