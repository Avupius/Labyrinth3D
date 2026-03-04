using UnityEngine;
using UnityEngine.InputSystem;

public class SensorInputHandler : MonoBehaviour
{
    [Header("Sensor Settings")]
    [SerializeField] private float sensitivity = 1.0f;
    [SerializeField] private bool calibrateOnStart = true;
    [SerializeField] private float smoothing = 5f;
    [SerializeField] private float deadzone = 0.1f;

    private Vector3 accelerationOffset = Vector3.zero;
    private Vector3 tiltSmoothed = Vector3.zero;
    private bool isCalibrated = false;

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
        if (!isCalibrated)
            return;

        Vector3 rawAccel = Vector3.zero;

        if (Accelerometer.current != null)
        {
            rawAccel = Accelerometer.current.acceleration.ReadValue();
        }
        else
        {
            rawAccel = Input.acceleration;
        }

        // Kalibrierungs-Offset abziehen (entfernt die Gravity-Komponente)
        Vector3 adjustedAccel = rawAccel - accelerationOffset;

        // Deadzone anwenden (ignoriert kleine Werte)
        if (Mathf.Abs(adjustedAccel.x) < deadzone)
            adjustedAccel.x = 0f;
        if (Mathf.Abs(adjustedAccel.z) < deadzone)
            adjustedAccel.z = 0f;
        
        // Z-Achse invertieren (damit negative Neigung nach hinten geht)
        adjustedAccel.z = -adjustedAccel.z;

        // Sensitivity anwenden
        adjustedAccel *= sensitivity;

        // Smoothing
        float t = Mathf.Clamp01(Time.deltaTime * smoothing);
        tiltSmoothed = Vector3.Lerp(tiltSmoothed, adjustedAccel, t);

        if (Time.frameCount % 60 == 0)
        {
            Debug.Log($"Tilt Input: ({tiltSmoothed.x:F2}, {tiltSmoothed.z:F2}) | Raw: {rawAccel} | Offset: {accelerationOffset}");
        }
    }

    public Vector3 GetTiltDirection()
    {
        return tiltSmoothed;
    }

    public void Calibrate()
    {
        Debug.Log("Kalibrierung startet synchron...");
        Vector3 sum = Vector3.zero;
        int samples = 30;

        for (int i = 0; i < samples; i++)
        {
            if (Accelerometer.current != null)
                sum += Accelerometer.current.acceleration.ReadValue();
            else
                sum += Input.acceleration;
        }

        accelerationOffset = sum / samples;
        tiltSmoothed = Vector3.zero;
        isCalibrated = true;

        Debug.Log("Kalibrierung abgeschlossen! Offset: " + accelerationOffset);
    }

    public void SetSensitivity(float newSensitivity)
    {
        sensitivity = Mathf.Clamp(newSensitivity, 0.1f, 5.0f);
        Debug.Log("Sensitivity gesetzt auf: " + sensitivity);
    }

    public void SetDeadzone(float newDeadzone)
    {
        deadzone = Mathf.Clamp(newDeadzone, 0f, 1f);
        Debug.Log("Deadzone gesetzt auf: " + deadzone);
    }
}