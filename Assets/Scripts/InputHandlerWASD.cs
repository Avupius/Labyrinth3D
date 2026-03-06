using UnityEngine;
using UnityEngine.InputSystem;

public class SensorInputHandler : MonoBehaviour
{
    [Header("Sensor Settings")]
    [SerializeField] private float sensitivity = 1.0f;
    [SerializeField] private bool calibrateOnStart = true;
    [SerializeField] private float smoothing = 10f;

    private Vector3 accelerationOffset = Vector3.zero;
    private Vector3 tiltSmoothed = Vector3.zero;
    private bool isEditorMode = false;

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

        #if UNITY_EDITOR
        isEditorMode = true;
        Debug.Log("Editor-Modus aktiviert - verwende WASD zum Testen");
        // Im Editor: accelerationOffset auf Null lassen, damit WASD direkt verwendet wird
        accelerationOffset = Vector3.zero;
        #else
        isEditorMode = false;
        if (calibrateOnStart)
        {
            Calibrate();
        }
        #endif
    }

    void Update()
    {
        Vector3 rawAccel = Vector3.zero;

        #if UNITY_EDITOR
        // Im Editor: WASD zum Testen (mit neuem Input System)
        var keyboard = Keyboard.current;
        if (keyboard != null)
        {
            if (keyboard.wKey.isPressed) rawAccel.z = 1f;  // W = vorne (positiv Z)
            if (keyboard.sKey.isPressed) rawAccel.z = -1f; // S = hinten (negativ Z)
            if (keyboard.dKey.isPressed) rawAccel.x = 1f;  // D = rechts (positiv X)
            if (keyboard.aKey.isPressed) rawAccel.x = -1f; // A = links (negativ X)
        }
        #else
        // Auf Android: Neue InputSystem Accelerometer API
        if (Accelerometer.current != null)
        {
            rawAccel = Accelerometer.current.acceleration.ReadValue();
        }
        else
        {
            rawAccel = Input.acceleration; // Fallback zur alten API
        }
        #endif

        // Nur im Device-Modus Kalibrierungs-Offset abziehen
        Vector3 adjustedAccel = rawAccel;
        if (!isEditorMode)
        {
            adjustedAccel -= accelerationOffset;
        }

        // Sensitivity anwenden
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