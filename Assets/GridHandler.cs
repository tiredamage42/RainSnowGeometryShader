/*
    script to handle specifying where on a grid the player is
*/
using UnityEngine;
using System;

[ExecuteInEditMode]
public class GridHandler : MonoBehaviour
{
    [Tooltip("How large (in meters) one grid block side is")]
    public float gridSize = 10f;

    [Tooltip("The player's transform to track")]
    public Transform playerTransform;
 
    // a callback to subscribe to when the player grid changes
    public event Action<Vector3Int> onPlayerGridChange;

    Vector3Int lastPlayerGrid = new Vector3Int(-99999,-99999,-99999);
    
    // Update runs once per frame.
    void Update () {
        if (playerTransform == null) {
            Debug.LogWarning("Grid Handler Has No Player Transform!");
            return;
        }

        // calculate the grid coordinate where the player currently is
        Vector3 playerPos = playerTransform.position;
        
        Vector3Int playerGrid = new Vector3Int(
            Mathf.FloorToInt(playerPos.x / gridSize),
            Mathf.FloorToInt(playerPos.y / gridSize),
            Mathf.FloorToInt(playerPos.z / gridSize)
        );
    
        // check if the player changed grid coordinates since the last check
        if (playerGrid != lastPlayerGrid) {
        
            // if it has, then broadcast the new grid coordinates
            // to whoever subscribed to the callback
            if (onPlayerGridChange != null) {
                onPlayerGridChange(playerGrid);
            }
        
            lastPlayerGrid = playerGrid;       
        }
    }    

    // calculate the center position of a certain grid coordinate
    public Vector3 GetGridCenter(Vector3Int grid) {
        float halfGrid = gridSize * .5f;
        return new Vector3(
            grid.x * gridSize + halfGrid, 
            grid.y * gridSize + halfGrid,
            grid.z * gridSize + halfGrid
        );
    }

    // draw gizmo cubes around teh grids where the player is
    // so we can see it in the scene view
    void OnDrawGizmos () {
        // loop in a 3 x 3 x 3 grid
        for (int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++) {
                for (int z = -1; z <= 1; z++) {

                    bool isCenter = x == 0 && y == 0 && z == 0;

                    Vector3 gridCenter = GetGridCenter(lastPlayerGrid + new Vector3Int(x, y, z));
                    
                    // make the center one green and slightly smaller so it stands out visually
                    Gizmos.color = isCenter ? Color.green : Color.red;
                    Gizmos.DrawWireCube(gridCenter, Vector3.one * (gridSize * (isCenter ? .95f : 1.0f)));
                }
            }
        }   
    }
}