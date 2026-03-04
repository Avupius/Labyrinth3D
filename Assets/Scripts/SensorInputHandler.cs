using UnityEngine;
using UnityEngine.InputSystem;

public class SensorInputHandlerOLD : MonoBehaviour
{
    [Header("Sensor Settings")]
    [SerializeField] private float sensitivity = 1.0f;
    [SerializeField] private bool calibrateOnStart = true;
    [SerializeField] private float smoothing = 10f;

    private Vector3 accelerationOffset = Vector3.zero;
    private Vector3 tiltSmoothed = Vector3.zero;

    void OnEnable()
    {
        if (Accelerometer.current != null)
        {
            InputSystem.EnableDevice(Accelerometer.current);
            Debug.Log("Accelerometer enabled: " + Accelerometer.current.enabled);
        }
        else
        {
            Debug.LogWarning("Accelerometer.current ist NULL (kein Device gefunden).");
        }
    }

    void OnDisable()
    {
        if (Accelerometer.current != null)
        {
            InputSystem.DisableDevice(Accelerometer.current);
        }
    }

    void Start()
    {
        Debug.Log("=== SensorInputHandler gestartet ===");
        Debug.Log("Accelerometer supported: " + SystemInfo.supportsAccelerometer);

        
        if (calibrateOnStart)
        {
            Calibrate();
        }
    }

    void Update()
    {
        Vector3 rawAccel = Vector3.zero;

        /*#if UNITY_EDITOR
        // Im Editor: WASD zum Testen (mit neuem Input System)
        var keyboard = Keyboard.current;
        if (keyboard != null)
        {
            if (keyboard.wKey.isPressed) rawAccel.x = 5f;
            if (keyboard.sKey.isPressed) rawAccel.x = -5f;
            if (keyboard.dKey.isPressed) rawAccel.z = 5f;
            if (keyboard.aKey.isPressed) rawAccel.z = -5f;
        }
        #else*/
        // Auf Android: Neue InputSystem Accelerometer API
        if (Accelerometer.current != null)
        {
            rawAccel = Accelerometer.current.acceleration.ReadValue();
        }
        else
        {
            rawAccel = Input.acceleration; // Fallback
        }
        //#endif

        // Kalibrierungs-Offset abziehen
        Vector3 adjustedAccel = rawAccel - accelerationOffset;

        // Optional: Sensitivity wirklich anwenden
        adjustedAccel *= sensitivity;

        // Smoothing
        float t = Mathf.Clamp01(Time.deltaTime * smoothing);
        tiltSmoothed = Vector3.Lerp(tiltSmoothed, adjustedAccel, t);

        if (Time.frameCount % 60 == 0)
        {
            Debug.Log($"Tilt Input: ({tiltSmoothed.x:F2}, {tiltSmoothed.z:F2}) | Raw: {rawAccel}");
        }
    }

    /// Gibt die aktuelle Tilt-Richtung zurueck (X=Links/Rechts, Z=Vor/Zurueck)
    public Vector3 GetTiltDirection()
    {
        return tiltSmoothed;
    }

    /// Kalibriert das aktuelle Handy-Orientation als "neutral" (0,0,0 Input)
    public void Calibrate()
    {
        if (Accelerometer.current != null)
            accelerationOffset = Accelerometer.current.acceleration.ReadValue();
        else
            accelerationOffset = Input.acceleration;

        tiltSmoothed = Vector3.zero;
        Debug.Log("SensorInputHandler kalibriert, Offset: " + accelerationOffset);
    }


    public void SetSensitivity(float newSensitivity)
    {
        sensitivity = Mathf.Clamp(newSensitivity, 0.1f, 5.0f);
        Debug.Log("Sensitivity gesetzt auf: " + sensitivity);
    }
}
