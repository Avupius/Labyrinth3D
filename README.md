# Labyrinth Ball-Rolling Game

Ein 3D Labyrinth-Spiel für Android, das durch Geräteneigung (Accelerometer) gesteuert wird.

## Spielbeschreibung

- **Ziel:** Steuere einen Ball durch ein Labyrinth mit Hilfe der Handy-Neigung
- **Steuerung (auf dem Handy):** Kippe das Gerät, um den Ball zu bewegen
- **Steuerung (im Editor):** Nutze WASD-Tasten
- **Hindernisse:** Vermeide Löcher im Labyrinth
- **Neuer Versuch:** Falls du in ein Loch fällst, startet das Spiel von vorne
- **Ziel erreichen:** Navigiere zum Ausgang des Labyrinths

---

## Features

- Sensor-basierte Steuerung mit Kalibrierung und Smoothing
- Prozedural generiertes Labyrinth (Recursive Backtracking)
- Realistische Physik-Simulation (Rigidbody)
- Persistente Speicherung von Best-Times und Statistiken
- Intuitive UI mit Live-Timer und Feedback-System
- Kompatibilität mit Android 10+

---

## Installation & Nutzung

### Option 1: Fertige APK installieren

**Voraussetzungen:**
- Android-Gerät (mind. Android 10)
- USB-Kabel
- USB-Debugging aktiviert

**Schritte:**

1. APK-Datei auf das Gerät kopieren
2. Auf dem Gerät zur APK-Datei navigieren
3. Datei antippen und "Installieren" wählen
4. Installation bestätigen
5. Spiel öffnen und Sensorzugriff erlauben

---

### Option 2: Aus Quellcode in Unity bauen

**Voraussetzungen:**
- Unity 6 installiert
- JetBrains Rider oder Visual Studio Code (optional)
- 5-10 GB freier Speicherplatz

**Schritte:**

1. **Projekt öffnen:**
   - Unity Hub öffnen
   - "Add" > "Add project from disk" wählen
   - Projekt-Ordner auswählen

2. **Projekt laden lassen:**
   - Unity importiert automatisch (5-10 Minuten)
   - Warten bis die Fortschrittsleiste fertig ist
   - Konsole sollte keine roten Fehler zeigen

3. **Im Editor testen:**
   - Im `Assets/` Ordner die Main-Szene öffnen
   - Play-Button drücken
   - WASD-Tasten zum Testen verwenden

4. **Für Android bauen:**
   - Android-Handy via USB verbinden
   - File > Build Profiles > Android wählen
   - "Switch Platform" klicken
   - Gerät unter "Run Device" auswählen
   - "Build and Run" klicken

---

## Technik-Stack

- **Engine:** Unity 6
- **Sprache:** C#
- **Input System:** Unity Input System API (Accelerometer)
- **Persistenz:** PlayerPrefs
- **Zielplattform:** Android 10+

---

## Projektstruktur

```
├── Assets/
│   ├── Scripts/
│   │   ├── GameManager.cs
│   │   ├── Ball.cs
│   │   ├── SensorInputHandler.cs
│   │   ├── Level.cs
│   │   ├── LabyrinthGenerator.cs
│   │   ├── ScoreManager.cs
│   │   ├── UIManager.cs
│   │   ├── HoleTrigger.cs
│   │   └── MenuManager.cs
│   └── Scenes/
│       └── Main.unity
├── ProjectSettings/
├── Packages/
├── My project.slnx
└── Labyrinth3D_15_03_FINAL.apk
```


