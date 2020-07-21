using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode] 
public class PrecipitationManager : MonoBehaviour 
{
    // 65536 (256 x 256) vertices is the max per mesh
    [Range(2, 256)] public int meshSubdivisions = 200;

    GridHandler gridHandler;
    Mesh meshToDraw;

    void OnEnable () {
        gridHandler = GetComponent<GridHandler>();
        gridHandler.onPlayerGridChange += OnPlayerGridChange;
    }

    void OnDisable() {
        gridHandler.onPlayerGridChange -= OnPlayerGridChange;
    }
    
    void OnPlayerGridChange(Vector3Int playerGrid) {
        
    }

    void Update()
    {
        // update the mesh automatically if it doesnt exist
        if (meshToDraw == null)
            RebuildPrecipitationMesh();

    }

    // the mesh created has a 
    // center at [0,0], 
    // min at [-.5, -.5] 
    // max at [.5, .5]
    public void RebuildPrecipitationMesh() {
        Mesh mesh = new Mesh ();
        List<int> indicies = new List<int>();
        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> uvs = new List<Vector3>();
        
        // use 0 - 100 range instead of 0 to 1
        // to avoid precision errors when subdivisions
        // are to high
        float f = 100f / meshSubdivisions;
        int i  = 0;
        for (float x = 0.0f; x <= 100f; x += f) {
            for (float y = 0.0f; y <= 100f; y += f) {

                // normalize x and y to a value between 0 and 1
                float x01 = x / 100.0f;
                float y01 = y / 100.0f;

                vertices.Add(new Vector3(x01 - .5f, 0, y01 - .5f));

                uvs.Add(new Vector3(x01, y01, 0.0f));

                indicies.Add(i++);
            }    
        }
        
        mesh.SetVertices(vertices);
        mesh.SetUVs(0,uvs);
        mesh.SetIndices(indicies.ToArray(), MeshTopology.Points, 0);

        // give a large bounds so it's always visible, we'll handle culling manually
        mesh.bounds = new Bounds(Vector3.zero, new Vector3(500, 500, 500));

        // dont save as an asset
        mesh.hideFlags = HideFlags.HideAndDontSave;

        meshToDraw = mesh;
    } 
}

#if UNITY_EDITOR
// create a custom editor with a button
// to trigger rebuilding of the render mesh
[CustomEditor(typeof(PrecipitationManager))] 
public class PrecipitationManagerEditor : Editor {

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        
        if (GUILayout.Button("Rebuild Precipitation Mesh")) {
            (target as PrecipitationManager).RebuildPrecipitationMesh();

            // set dirty to make sure the editor updates
            EditorUtility.SetDirty(target);
        }
    }
}
#endif