/*
    Precipitation Manager
*/
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;

[ExecuteInEditMode] 
public class PrecipitationManager : MonoBehaviour 
{
    // 65536 (256 x 256) vertices is the max per mesh
    [Range(2, 256)] public int meshSubdivisions = 200;

    GridHandler gridHandler;
    Matrix4x4[] renderMatrices = new Matrix4x4[3 * 3 * 3];
		
    Mesh meshToDraw;

    Material rainMaterial, snowMaterial;
    // automatic material creation
    static Material CreateMaterialIfNull(string shaderName, ref Material reference) {
        if (reference == null) {
            reference = new Material(Shader.Find(shaderName));
            reference.hideFlags = HideFlags.HideAndDontSave;
            reference.renderQueue = 3000;
            reference.enableInstancing = true;
        }
        return reference;
    }


    void OnEnable () {
        gridHandler = GetComponent<GridHandler>();
        gridHandler.onPlayerGridChange += OnPlayerGridChange;
    }

    void OnDisable() {
        gridHandler.onPlayerGridChange -= OnPlayerGridChange;
    }
    
    /*
        set all our render matrices to be positioned
        in a 3x3x3 grid around the player
    */
    void OnPlayerGridChange(Vector3Int playerGrid) {

        // index for each individual matrix
        int i = 0;

        // loop in a 3 x 3 x 3 grid
        for (int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++) {
                for (int z = -1; z <= 1; z++) {

                    Vector3Int neighborOffset = new Vector3Int(x, y, z);
                    
                    // adjust the rendering position matrix, leaving rotation and scale alone
                    renderMatrices[i++].SetTRS(
                        gridHandler.GetGridCenter(playerGrid + neighborOffset), 
                        Quaternion.identity, 
                        Vector3.one
                    );
                }
            }
        }
    }

    void Update()
    {
        // update the mesh automatically if it doesnt exist
        if (meshToDraw == null)
            RebuildPrecipitationMesh();


        // render the rain and snow
        RenderEnvironmentParticles(CreateMaterialIfNull("Hidden/Environment/Rain", ref rainMaterial));
        RenderEnvironmentParticles(CreateMaterialIfNull("Hidden/Environment/Snow", ref snowMaterial));
    }

    void RenderEnvironmentParticles(Material material) {
            
        material.SetFloat("_GridSize", gridHandler.gridSize);
     
        Graphics.DrawMeshInstanced(meshToDraw, 0, material, renderMatrices, renderMatrices.Length, null, ShadowCastingMode.Off, true, 0, null, LightProbeUsage.Off);
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