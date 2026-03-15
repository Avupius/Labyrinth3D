using UnityEngine;

public class ExitTrigger : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    
    void Start()
    {
        gameManager =  FindFirstObjectByType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("ExitTrigger: GameManager nicht gefunden! Kein GameManager in der Szene vorhanden.");
        }
        
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball") && gameManager != null)
        {
            Debug.Log("Exit erreicht!");
            gameManager.CompletedLevel();
        }
    }
}