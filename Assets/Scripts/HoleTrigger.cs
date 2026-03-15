using System.Collections;
using UnityEngine;

/// <summary>
/// Wird an jedes generierte Loch angehängt.
/// Erkennt wenn der Ball hinein fällt und teilt das dem GameManager mit.
/// </summary>
public class HoleTrigger : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    private bool hasTriggered = false;

    void Awake()
    {
        // HoleTrigger sollte auf einem GameObject mit Collider (z.B. SphereCollider) liegen, der als Trigger eingestellt ist.
        // Stelle sicher dass der Collider als Trigger eingestellt ist, falls es nicht bereits so ist.
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            Debug.LogError("HoleTrigger: Kein Collider gefunden! Bitte füge einen Collider hinzu.");
        }
        else if (!col.isTrigger)
        {
            col.isTrigger = true;
            Debug.Log("HoleTrigger: Collider wurde auf Trigger gesetzt");
        }
    }
    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("HoleTrigger: GameManager nicht gefunden! Kein GameManager in der Szene vorhanden.");
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        // Prüfe ob der Ball in das Loch fällt
        if (hasTriggered || !other.CompareTag("Ball"))
            return;
        
        
        Debug.Log("HoleTrigger: Ball faellt in Loch bei " + transform.position);
        hasTriggered = true;
        gameManager?.BallFellInHole();
            
        // Reset Flag nach kurzer Zeit für nächsten "Fall"
        StartCoroutine(ResetTrigger(0.5f));
    }
    
    private IEnumerator ResetTrigger(float delay)
    {
        yield return new WaitForSeconds(delay);
        hasTriggered = false;
    }

    /// <summary>
    /// Wird vom Level aufgerufen wenn Ball neu gesetzt wird
    /// </summary>
    public void Reset()
    {
        hasTriggered = false;
    }
}