using UnityEngine;

/// <summary>
/// Wird an jedes generierte Loch angehängt.
/// Erkennt wenn der Ball hinein fällt und teilt das dem GameManager mit.
/// </summary>
public class HoleTrigger : MonoBehaviour
{
    private GameManager gameManager;
    private bool hasTriggered = false;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        
        if (gameManager == null)
        {
            Debug.LogError("HoleTrigger: GameManager nicht gefunden!");
        }
        
        // Stelle sicher dass dieser Collider ein Trigger ist
        Collider col = GetComponent<Collider>();
        if (col != null && !col.isTrigger)
        {
            col.isTrigger = true;
            Debug.Log("HoleTrigger: Collider wurde auf Trigger gesetzt");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Nur einmal pro "Fall" triggern
        if (hasTriggered)
            return;

        // Prüfe ob der Ball in das Loch fällt
        if (other.CompareTag("Ball"))
        {
            hasTriggered = true;
            Debug.Log("HoleTrigger: Ball faellt in Loch bei " + transform.position);
            
            if (gameManager != null)
            {
                gameManager.BallFellInHole();
            }
            
            // Reset Flag nach kurzer Zeit für nächsten "Fall"
            Invoke("ResetTrigger", 0.5f);
        }
    }

    private void ResetTrigger()
    {
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