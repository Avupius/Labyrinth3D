using UnityEngine;

/// <summary>
/// Verwaltet die Physik und Bewegung des Balls basierend auf Sensor-Input.
/// Kommuniziert mit SensorInputHandler fuer Beschleunigungsdaten.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Ball : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float forceMultiplier = 20f;
    [SerializeField] private float maxVelocity = 15f;
    [SerializeField] private float dragFriction = 2f;
    
    [Header("References")]
    [SerializeField] private SensorInputHandler sensorInput;
    
    private Rigidbody rb;
    private bool isInitialized = false;
    
    void Awake()
    {
        InitializeRigidbody();
    }
    
    void Start()
    {
        if (sensorInput == null)
        {
            Debug.LogError("Ball: SensorInputHandler nicht gesetzt");
            return;
        }
        
        if (rb == null)
        {
            Debug.LogError("Ball: Rigidbody nicht gefunden!");
            return;
        }
        
        ConfigurePhysics();
        isInitialized = true;
        
        Debug.Log("Ball: Ball initialisiert");
    }
    
   void FixedUpdate()
    {
        if (!isInitialized || sensorInput == null)
            return;
        
        ApplyTiltForce();
        LimitVelocity();
    }

    private void ApplyTiltForce()
    {
        Vector3 tiltInput = sensorInput.GetTiltDirection();
        // Wende nur horizontale Kraft an (X und Z). Y ist 0, da Gravität die vertikale Bewegung steuert
        Vector3 force = new Vector3(tiltInput.x, 0f, tiltInput.z) * forceMultiplier;
        
        rb.AddForce(force, ForceMode.Acceleration);
    }

    private void LimitVelocity()
    {
        float currentVelocity = rb.linearVelocity.magnitude;
        
        if (currentVelocity > maxVelocity)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxVelocity;
        }
    }
    private void InitializeRigidbody()
    {
        rb = GetComponent<Rigidbody>();
        
        if (rb == null)
        {
            Debug.LogError("Ball: Rigidbody nicht gefunden!");
            enabled = false;
        }
    }
    
    private void ConfigurePhysics()
    {
        // Physik-Settings
        rb.linearDamping = dragFriction;
        rb.angularDamping = dragFriction;
    }
   
    /// <summary>
    /// Gibt die aktuelle Position des Balls zurück
    /// </summary>
    /// <returns>Welt-Position des Balls</returns>
    public Vector3 GetPosition()
    {
        return transform.position;
    }
   
    /// <summary>
    /// Setzt den Ball zur Startposition zurück
    /// </summary>
    /// <param name="startPos">Zielposition für Reset</param>
    public void ResetPosition(Vector3 startPos)
    {
        transform.position = startPos;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        Debug.Log("Ball zurückgesetzt zu: " + startPos);
    }
}