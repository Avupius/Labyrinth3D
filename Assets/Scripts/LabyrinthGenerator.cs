using UnityEngine;

public class LabyrinthGenerator : MonoBehaviour
{
    [Header("Labyrinth Einstellungen")]
    [SerializeField] private int width = 11;
    [SerializeField] private int height = 11;
    [SerializeField] private float cellSize = 2f;
    [SerializeField] private float wallHeight = 2f;
    
    [Header("Materialien")]
    [SerializeField] private Material wallMaterial;
    [SerializeField] private Material floorMaterial;
    [SerializeField] private Material exitMaterial;
    
    [Header("Objekt-Referenzen")]
    [SerializeField] private Transform parentTransform;
    [SerializeField] private Transform startPositionTransform; // Referenz zu Level
    
    private bool[,] maze;
    private GameObject startPositionObject;
    private GameObject wallContainer;
    private GameObject floorContainer;

    public void GenerateMaze()
    {
        // Alte Objekte loeschen
        if (wallContainer != null) DestroyImmediate(wallContainer);
        if (floorContainer != null) DestroyImmediate(floorContainer);
        if (startPositionObject != null) DestroyImmediate(startPositionObject);
        
        {
            
        }
        
        // Container erstellen
        wallContainer = new GameObject("Walls");
        wallContainer.transform.parent = parentTransform ?? transform;
        wallContainer.transform.localPosition = Vector3.zero;
        
        floorContainer = new GameObject("Floor");
        floorContainer.transform.parent = parentTransform ?? transform;
        floorContainer.transform.localPosition = Vector3.zero;
        
        // Labyrinth mit Recursive Backtracker generieren
        maze = new bool[width, height];
        InitializeMaze();
        GeneratePathRecursive(1, 1);
        
        // 3D-Objekte erstellen
        CreateFloor();
        CreateWalls();
        CreateExit();
        CreateStartPosition();
    }
    
    
    private void CreateStartPosition()
    {
        startPositionObject = new GameObject("StartPosition");
        startPositionObject.transform.parent = transform;
        startPositionObject.transform.localPosition = new Vector3(
            1 * cellSize,
            1f,
            1 * cellSize
        );
    
        Debug.Log("StartPosition erstellt bei: " + startPositionObject.transform.position);
    }

    
    
    private void InitializeMaze()
    {
        // Alle Zellen sind zuerst Waende
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                maze[x, y] = true; // true = Wand
            }
        }
    }
    
    private void GeneratePathRecursive(int x, int y)
    {
        maze[x, y] = false; // Aktueller Pfad oeffnen
        
        // Vier Richtungen: rechts, unten, links, oben
        int[] dx = { 2, 0, -2, 0 };
        int[] dy = { 0, 2, 0, -2 };
        
        // Richtungen randomizieren
        int[] directions = { 0, 1, 2, 3 };
        ShuffleArray(directions);
        
        foreach (int dir in directions)
        {
            int nx = x + dx[dir];
            int ny = y + dy[dir];
            
            // Grenzpruefung
            if (nx > 0 && nx < width - 1 && ny > 0 && ny < height - 1 && maze[nx, ny])
            {
                // Wand dazwischen oeffnen
                maze[x + dx[dir] / 2, y + dy[dir] / 2] = false;
                GeneratePathRecursive(nx, ny);
            }
        }
    }
    
    private void ShuffleArray(int[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            // Swap
            int temp = array[i];
            array[i] = array[randomIndex];
            array[randomIndex] = temp;
        }
    }
    
    private void CreateFloor()
    {
        // Erstelle einen Cube fuer den Boden
        GameObject floorCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        
        // Entferne den Mesh Collider des Primitives
        Collider defaultCollider = floorCube.GetComponent<Collider>();
        if (defaultCollider != null)
            DestroyImmediate(defaultCollider);
        
        floorCube.name = "Floor";
        floorCube.transform.parent = floorContainer.transform;
        
        float floorWidth = width * cellSize;
        float floorDepth = height * cellSize;
        
        floorCube.transform.localPosition = new Vector3(
            (width - 1) * cellSize * 0.5f,
            -0.5f,
            (height - 1) * cellSize * 0.5f
        );
        
        floorCube.transform.localScale = new Vector3(floorWidth, 1f, floorDepth);
        
        if (floorMaterial != null)
            floorCube.GetComponent<Renderer>().material = floorMaterial;
        
        // BoxCollider fuer Physik
        BoxCollider collider = floorCube.AddComponent<BoxCollider>();
        collider.size = Vector3.one;
        collider.isTrigger = false;
        
        // Rigidbody - WICHTIG: kinematic damit es nicht faellt
        Rigidbody rb = floorCube.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints.FreezeAll;
    }
    
    private void CreateWalls()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (maze[x, y])
                {
                    CreateWallCube(x, y);
                }
            }
        }
    }
    
    private void CreateWallCube(int x, int y)
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        
        // Entferne den default Collider
        Collider defaultCollider = wall.GetComponent<Collider>();
        if (defaultCollider != null)
            DestroyImmediate(defaultCollider);
        
        wall.name = $"Wall_{x}_{y}";
        wall.transform.parent = wallContainer.transform;
        wall.transform.localPosition = new Vector3(
            x * cellSize,
            wallHeight * 0.5f,
            y * cellSize
        );
        wall.transform.localScale = new Vector3(cellSize, wallHeight, cellSize);
        
        if (wallMaterial != null)
            wall.GetComponent<Renderer>().material = wallMaterial;
        
        // BoxCollider fuer Physik - WICHTIG: nicht ist Trigger!
        BoxCollider collider = wall.AddComponent<BoxCollider>();
        collider.size = Vector3.one;
        collider.isTrigger = false;
        
        // Rigidbody - kinematic damit Waende nicht herumfallen
        Rigidbody rb = wall.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints.FreezeAll;
    }
    
    private void CreateExit()
    {
        // Exit in der oberen rechten Ecke
        int exitX = width - 2;
        int exitY = height - 4;
        
        GameObject exit = GameObject.CreatePrimitive(PrimitiveType.Cube);
        
        // Entferne default Collider
        Collider defaultCollider = exit.GetComponent<Collider>();
        if (defaultCollider != null)
            DestroyImmediate(defaultCollider);
        
        exit.name = "Exit";
        exit.transform.parent = transform;
        exit.transform.localPosition = new Vector3(
            exitX * cellSize,
            0.455f,
            exitY * cellSize
        );
        exit.transform.localScale = new Vector3(cellSize * 0.8f, 0.1f, cellSize * 0.8f);
        
        if (exitMaterial != null)
            exit.GetComponent<Renderer>().material = exitMaterial;
        
        // BoxCollider als TRIGGER
        BoxCollider triggerCollider = exit.AddComponent<BoxCollider>();
        triggerCollider.isTrigger = true;
        triggerCollider.size = Vector3.one;
        
        // Script fuer Exit-Logik
        ExitTrigger exitScript = exit.AddComponent<ExitTrigger>();
        
        exit.tag = "Exit";
    }
    
    public Transform GetStartPosition()
    {
        return startPositionObject?.transform;
    }
}



// Einfaches Script fuer Exit-Trigger
public class ExitTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            Debug.Log("Labyrinth geloest!");
            // Hier kannst du deine eigene Logik einfuegen
            // z.B. nächstes Level laden oder Score erhöhen
        }
    }
}