using UnityEngine;

public class LabyrinthGenerator : MonoBehaviour
{
    [Header("Labyrinth Einstellungen")]
    [SerializeField] private int width = 11;
    [SerializeField] private int height = 11;
    [SerializeField] private float cellSize = 2f;
    [SerializeField] private float wallHeight = 2f;
    
    [Header("Loecher Einstellungen")]
    [SerializeField] private int holeCount = 3;
    [SerializeField] private float holeRadius = 0.8f;
    [SerializeField] private int safeZoneRadiusSpawn = 3;
    [SerializeField] private int safeZoneRadiusExit = 3;
    
    [Header("Materialien")]
    [SerializeField] private Material wallMaterial;
    [SerializeField] private Material floorMaterial;
    [SerializeField] private Material exitMaterial;
    [SerializeField] private Material holeMaterial;
    
    [Header("Objekt-Referenzen")]
    [SerializeField] private Transform parentTransform;
    [SerializeField] private Level level;
    
    private bool[,] maze;
    private GameObject startPositionObject;
    private GameObject exitPositionObject;
    private GameObject wallContainer;
    private GameObject floorContainer;
    private GameObject holesContainer;

    public void GenerateMaze()
    {
        // Alte Objekte loeschen
        if (wallContainer != null) DestroyImmediate(wallContainer);
        if (floorContainer != null) DestroyImmediate(floorContainer);
        if (holesContainer != null) DestroyImmediate(holesContainer);
        if (startPositionObject != null) DestroyImmediate(startPositionObject);
        if (exitPositionObject != null) DestroyImmediate(exitPositionObject);
        
        // Container erstellen
        wallContainer = new GameObject("Walls");
        wallContainer.transform.parent = parentTransform ?? transform;
        wallContainer.transform.localPosition = Vector3.zero;
        
        floorContainer = new GameObject("Floor");
        floorContainer.transform.parent = parentTransform ?? transform;
        floorContainer.transform.localPosition = Vector3.zero;
        
        holesContainer = new GameObject("Holes");
        holesContainer.transform.parent = parentTransform ?? transform;
        holesContainer.transform.localPosition = Vector3.zero;
        
        // Labyrinth generieren
        maze = new bool[width, height];
        InitializeMaze();
        GeneratePathRecursive(1, 1);
        
        // 3D-Objekte erstellen
        CreateFloor();
        CreateWalls();
        CreateStartPosition();
        CreateExit();
        CreateHoles();
    }
    
    private void InitializeMaze()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                maze[x, y] = true;
            }
        }
    }
    
    private void GeneratePathRecursive(int x, int y)
    {
        maze[x, y] = false;
        
        int[] dx = { 2, 0, -2, 0 };
        int[] dy = { 0, 2, 0, -2 };
        
        int[] directions = { 0, 1, 2, 3 };
        ShuffleArray(directions);
        
        foreach (int dir in directions)
        {
            int nx = x + dx[dir];
            int ny = y + dy[dir];
            
            if (nx > 0 && nx < width - 1 && ny > 0 && ny < height - 1 && maze[nx, ny])
            {
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
            int temp = array[i];
            array[i] = array[randomIndex];
            array[randomIndex] = temp;
        }
    }
    
    private void CreateFloor()
    {
        GameObject floorCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        
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
        
        BoxCollider collider = floorCube.AddComponent<BoxCollider>();
        collider.size = Vector3.one;
        collider.isTrigger = false;
        
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
        
        BoxCollider collider = wall.AddComponent<BoxCollider>();
        collider.size = Vector3.one;
        collider.isTrigger = false;
        
        Rigidbody rb = wall.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints.FreezeAll;
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
    
    private void CreateExit()
    {
        int exitX = width - 2;
        int exitY = height - 4;
        
        GameObject exit = GameObject.CreatePrimitive(PrimitiveType.Cube);
        
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
        
        BoxCollider triggerCollider = exit.AddComponent<BoxCollider>();
        triggerCollider.isTrigger = true;
        triggerCollider.size = Vector3.one;
        
        ExitTrigger exitScript = exit.AddComponent<ExitTrigger>();
        exit.tag = "Exit";
        
        // Exit Position speichern
        exitPositionObject = new GameObject("ExitPosition");
        exitPositionObject.transform.parent = transform;
        exitPositionObject.transform.localPosition = exit.transform.position;
    }
    
    private void CreateHoles()
    {
        int holesCreated = 0;
        int attempts = 0;
        int maxAttempts = 200;
        
        while (holesCreated < holeCount && attempts < maxAttempts)
        {
            attempts++;
            
            int randomX = Random.Range(2, width - 2);
            int randomY = Random.Range(2, height - 2);
            
            if (!maze[randomX, randomY])
            {
                // Pruefe Safe Zones
                float distToSpawn = Vector3.Distance(
                    new Vector3(randomX * cellSize, 0, randomY * cellSize),
                    new Vector3(1 * cellSize, 0, 1 * cellSize)
                );
                
                float distToExit = Vector3.Distance(
                    new Vector3(randomX * cellSize, 0, randomY * cellSize),
                    exitPositionObject.transform.position
                );
                
                if (distToSpawn > safeZoneRadiusSpawn * cellSize && 
                    distToExit > safeZoneRadiusExit * cellSize)
                {
                    CreateHole(randomX, randomY);
                    holesCreated++;
                }
            }
        }
        
        Debug.Log($"Loecher erstellt: {holesCreated}/{holeCount}");
    }
    
    private void CreateHole(int x, int y)
    {
        // Bestimme Offset-Richtung zur Wand
        Vector3 offsetDirection = GetOffsetTowardsNearestWall(x, y);
        
        // Verschiebe das Loch zur Wand hin
        float offsetAmount = 0.4f;
        Vector3 cellCenter = new Vector3(x * cellSize, 0.1f, y * cellSize);
        Vector3 offsetPosition = cellCenter + offsetDirection * cellSize * offsetAmount;
        
        GameObject hole = new GameObject($"Hole_{x}_{y}");
        hole.transform.parent = holesContainer.transform;
        hole.transform.localPosition = offsetPosition;
        
        // Visualisierung
        GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        DestroyImmediate(visual.GetComponent<Collider>());
        visual.transform.parent = hole.transform;
        visual.transform.localPosition = Vector3.zero;
        visual.transform.localScale = new Vector3(holeRadius * 2, 0.1f, holeRadius * 2);
        
        if (holeMaterial != null)
            visual.GetComponent<Renderer>().material = holeMaterial;
        
        // Trigger Collider
        SphereCollider collider = hole.AddComponent<SphereCollider>();
        collider.radius = holeRadius;
        collider.isTrigger = true;
    }

    private Vector3 GetOffsetTowardsNearestWall(int x, int y)
    {
        int leftWallCount = 0;
        int rightWallCount = 0;
        int topWallCount = 0;
        int bottomWallCount = 0;
        
        // Zaehle Waende in direkter Umgebung
        for (int dx = -1; dx <= 1; dx++)
        {
            if (x + dx >= 0 && x + dx < width && maze[x + dx, y])
            {
                if (dx < 0) leftWallCount++;
                else if (dx > 0) rightWallCount++;
            }
        }
        
        for (int dy = -1; dy <= 1; dy++)
        {
            if (y + dy >= 0 && y + dy < height && maze[x, y + dy])
            {
                if (dy < 0) topWallCount++;
                else if (dy > 0) bottomWallCount++;
            }
        }
        
        // Finde Hauptrichtung mit den meisten Waenden
        int maxCount = Mathf.Max(leftWallCount, rightWallCount, topWallCount, bottomWallCount);
        
        if (maxCount == 0)
        {
            int rand = Random.Range(0, 4);
            return rand switch
            {
                0 => Vector3.left,
                1 => Vector3.right,
                2 => Vector3.back,
                _ => Vector3.forward,
            };
        }
        
        if (leftWallCount == maxCount) return Vector3.left;
        if (rightWallCount == maxCount) return Vector3.right;
        if (topWallCount == maxCount) return Vector3.back;
        if (bottomWallCount == maxCount) return Vector3.forward;
        
        return Vector3.zero;
    }
    
    public Transform GetStartPosition()
    {
        return startPositionObject?.transform;
    }
    
    public Transform GetExitPosition()
    {
        return exitPositionObject?.transform;
    }
}

public class ExitTrigger : MonoBehaviour
{
    private GameManager gameManager;
    
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball") && gameManager != null)
        {
            Debug.Log("Exit erreicht!");
            gameManager.WinLevel();
        }
    }
}