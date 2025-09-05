using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering.Universal;

public class rotator : MonoBehaviour
{
    #if UNITY_EDITOR
        [MenuItem("Game Tools/Rotate 90 Deg")]
        static void rotate() {
            int i, j;
            var ter = FindObjectsByType<Terrain>(FindObjectsSortMode.None)[1];
            var td = ter.terrainData;

            // rotate heightmap
            var hgts = td.GetHeights(0, 0, td.heightmapResolution, td.heightmapResolution);
            var newhgts = new float[hgts.GetLength(0), hgts.GetLength(1)];
            for (j = 0; j < td.heightmapResolution; j++) {
            for (i = 0; i < td.heightmapResolution; i++) {
                newhgts[td.heightmapResolution - 1 - j, i] = hgts[i, j];
            }
            }
            td.SetHeights(0, 0, newhgts);
            ter.Flush();

            // rotate splatmap
            var alpha = td.GetAlphamaps(0, 0, td.alphamapWidth, td.alphamapHeight);
            var newalpha = new float[alpha.GetLength(0), alpha.GetLength(1), alpha.GetLength(2)];
            for (j = 0; j < td.alphamapHeight; j++) {
            for (i = 0; i < td.alphamapWidth; i++) {
                for (int k = 0; k < td.splatPrototypes.Length; k++) {
                newalpha[td.alphamapHeight - 1 - j, i, k] = alpha[i, j, k];
                }
            }
            }
            td.SetAlphamaps(0, 0, newalpha);

            // rotate trees
            var size = td.size;
            var trees = td.treeInstances;
            for (i = 0; i < trees.Length; i++) {
            trees[i].position = new Vector3(1 - trees[i].position.z, 0, trees[i].position.x);
            trees[i].position.y = td.GetInterpolatedHeight(trees[i].position.x, trees[i].position.z) / size.y;
            }
            td.treeInstances = trees;
        }

        [CustomEditor(typeof(rotator))]
        public class RotatorEditor : Editor
        {
            public override void OnInspectorGUI()
            {
            DrawDefaultInspector();

            if (GUILayout.Button("Rotate Terrain 90 Deg"))
            {
                rotate();
            }
            }
        }
    #endif
}
