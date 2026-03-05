using UnityEngine;

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
    
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        
        if (rb == null)
        {
            Debug.LogError("Ball3D: Rigidbody nicht gefunden!");
            enabled = false;
        }
    }
    
    void Start()
    {
        if (sensorInput == null)
        {
            sensorInput = FindObjectOfType<SensorInputHandler>();
            if (sensorInput == null)
            {
                Debug.LogError("Ball3D: SensorInputHandler nicht gefunden!");
            }
        }
        
        // Physik-Settings
        rb.linearDamping = dragFriction;
        rb.angularDamping = dragFriction;
        
        Debug.Log("Ball3D initialisiert");
    }
    
   void FixedUpdate()
    {
        if (sensorInput == null)
            return;
        
        Vector3 tiltInput = sensorInput.GetTiltDirection();
        
        // Direkt X und Z verwenden
        Vector3 force = new Vector3(tiltInput.x, 0f, tiltInput.z) * forceMultiplier;
        
        rb.AddForce(force, ForceMode.Acceleration);
        
        if (rb.linearVelocity.magnitude > maxVelocity)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxVelocity;
        }
        
        // Debug
        if (Time.frameCount % 60 == 0)
        {
            Debug.Log($"Ball Velocity: {rb.linearVelocity.magnitude:F2} | Tilt: {tiltInput}");
        }
    }
    
    /// <summary>
    /// Gibt die aktuelle Position des Balls zurueck
    /// </summary>
    public Vector3 GetPosition()
    {
        return transform.position;
    }
   
    /// <summary>
    /// Setzt den Ball zur Startposition zurueck
    /// </summary>
    public void ResetPosition(Vector3 startPos)
    {
        transform.position = startPos;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        Debug.Log("Ball zurueckgesetzt zu: " + startPos);
    }
}