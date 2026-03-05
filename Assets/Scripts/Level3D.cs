using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField] private Transform startPosition;
    [SerializeField] private Transform exitPosition;
    [SerializeField] private Transform holePosition;
    [SerializeField] private float holeRadius = 0.5f;
    [SerializeField] private Vector3 exitSize = new Vector3(1, 0.5f, 1);
    
    void Start()
    {
        if (startPosition == null)
            Debug.LogError("Level3D: Start Position nicht gesetzt!");
        else
        {
            Debug.Log("Level3D: Start Position = " + startPosition.position);
        }
    }
    
    public void SetStartPosition(Transform newStartPos)
    {
        startPosition = newStartPos;
        Debug.Log("StartPosition gesetzt zu: " + newStartPos.position);
    }
    
    public Vector3 GetStartPosition()
    {
        return startPosition != null ? startPosition.position : Vector3.zero;
    }
    
    public bool CheckBallInHole(Vector3 ballPosition)
    {
        if (holePosition == null) return false;
        
        float distance = Vector3.Distance(ballPosition, holePosition.position);
        return distance < holeRadius;
    }
    
    public bool CheckBallAtExit(Vector3 ballPosition)
    {
        if (exitPosition == null) return false;
        
        Vector3 exitPos = exitPosition.position;
        
        bool isInExit =
            Mathf.Abs(ballPosition.x - exitPos.x) < exitSize.x / 2f &&
            Mathf.Abs(ballPosition.y - exitPos.y) < exitSize.y / 2f &&
            Mathf.Abs(ballPosition.z - exitPos.z) < exitSize.z / 2f;
        
        return isInExit;
    }
}