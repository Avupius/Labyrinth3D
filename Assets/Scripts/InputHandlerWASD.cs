using UnityEngine;
using UnityEngine.InputSystem;

public class SensorInputHandler : MonoBehaviour
{
    [Header("Sensor Settings")]
    [SerializeField] private float sensitivity = 1.0f;
    [SerializeField] private bool calibrateOnStart = true;
    [SerializeField] private float smoothing = 5f;
    [SerializeField] private float deadzonePositive = 0.1f;  // Für positive Werte (vorwärts)
    [SerializeField] private float deadzoneNegative = 0.3f;  // Für negative Werte (rückwärts) - leichter zu triggern
    [SerializeField] private float zAxisOffset = -0.5f;  // Verschiebt den Nullpunkt der Z-Achse nach vorne (positiv)

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
        
        // FALLBACK: Wenn Kalibrierung fehlschlaegt, manuell setzen
        if (Mathf.Approximately(accelerationOffset.magnitude, 0f))
        {
            Debug.LogWarning("Kalibrierung fehlgeschlagen! Setze Fallback-Offset");
            accelerationOffset = new Vector3(0f, -9.81f, 0f);
            isCalibrated = true;
        }
    }

    void Update()
    {
        if (!isCalibrated)
            return;

        Vector3 rawAccel = Vector3.zero;
        
        
        // Auf Android: Neue InputSystem Accelerometer API
        if (Accelerometer.current != null)
        {
            rawAccel = Accelerometer.current.acceleration.ReadValue();
        }
        else
        {
            var keyboard = Keyboard.current;
            if (keyboard != null)
            {
                if (keyboard.wKey.isPressed) rawAccel.z = 1f;  // W = vorne (positiv Z)
                if (keyboard.sKey.isPressed) rawAccel.z = -1f; // S = hinten (negativ Z)
                if (keyboard.dKey.isPressed) rawAccel.x = 1f;  // D = rechts (positiv X)
                if (keyboard.aKey.isPressed) rawAccel.x = -1f; // A = links (negativ X)
            }
        }
        
        // Im Editor: WASD zum Testen (mit neuem Input System)

        // Kalibrierungs-Offset abziehen (entfernt die Gravity-Komponente)
        Vector3 adjustedAccel = rawAccel - accelerationOffset;

        // Deadzone anwenden - asymmetrisch fuer Z (rueckwaerts leichter)
        if (Mathf.Abs(adjustedAccel.x) < deadzonePositive)
            adjustedAccel.x = 0f;
        
        // Z-Achse: unterschiedliche Deadzone fuer positive und negative Werte
        if (adjustedAccel.z > 0 && adjustedAccel.z < deadzonePositive)
            adjustedAccel.z = 0f;
        else if (adjustedAccel.z < 0 && adjustedAccel.z > -deadzoneNegative)
            adjustedAccel.z = 0f;
        
        // Z-Achse invertieren (damit negative Neigung nach hinten geht)
        adjustedAccel.z = -adjustedAccel.z;
        
        // Z-Achsen Offset anwenden (verschiebt den Nullpunkt nach vorne/positiv)
        adjustedAccel.z += -zAxisOffset;

        // Sensitivity anwenden
        adjustedAccel *= sensitivity;

        // Smoothingw
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
            // Im Editor: keine echte Kalibrierung nötig
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

    public void SetDeadzonePositive(float newDeadzone)
    {
        deadzonePositive = Mathf.Clamp(newDeadzone, 0f, 1f);
        Debug.Log("Deadzone Positive gesetzt auf: " + deadzonePositive);
    }

    public void SetDeadzoneNegative(float newDeadzone)
    {
        deadzoneNegative = Mathf.Clamp(newDeadzone, 0f, 1f);
        Debug.Log("Deadzone Negative gesetzt auf: " + deadzoneNegative);
    }

    public void SetZAxisOffset(float newOffset)
    {
        zAxisOffset = newOffset;
        Debug.Log("Z-Achsen Offset gesetzt auf: " + zAxisOffset);
    }
}