using UnityEngine;
using Random = UnityEngine.Random;

namespace Assignment1
{
    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
    public class PathToWall : MonoBehaviour
    {
        // -- path --
        [SerializeField] Vector3[] path = System.Array.Empty<Vector3>();
        
        // -- height --
        const float DefaultHeight = 4;
        [SerializeField] bool useCustomHeight;
        [SerializeField] float customHeight = 4;
        float GetHeight() => useCustomHeight ? customHeight : DefaultHeight;
        
        MeshFilter meshFilter;

        public MeshFilter MeshFilter
        {
            get
            {
                if (meshFilter) return meshFilter;

                if (TryGetComponent(out meshFilter))
                    return meshFilter;
                
                Debug.LogError($"No Mesh Filter Component in GameObject {name}", gameObject);
                return null;
            }
        }

        void Awake()
        {
            GenerateNewPath();
            GenerateWallMesh();
        }
        
        [ContextMenu("Generate Path")]
        void GenerateNewPath()
        {
            path = new Vector3[Random.Range(5, 15)];
            Vector3 startPos = Random.insideUnitSphere * 2;
            startPos.y = 0;
            for (int i = 0; i < path.Length; i++)
            {
                path[i] = startPos;
                startPos += Random.insideUnitSphere * 2;
                startPos.y = 0;
            }
        }

        [ContextMenu("Generate Wall Mesh")]
        void GenerateWallMesh()
        {
            // ASSIGNMENT HERE

            // [uncomment when ready with vertices etc., so that it doesn't throw compilation errors]
            // Mesh mesh = new Mesh();
            // mesh.vertices = vertices;
            // mesh.triangles = triangles;
            // mesh.RecalculateNormals();

            // MeshFilter.mesh = mesh;
        }


#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            // Super quick debug to preview how things should be
            
            
            if(path == null || path.Length == 0) return;
            
            for (var i = 0; i < path.Length; i++)
                UnityEditor.Handles.Label(path[i], $"{i}");

            Gizmos.color = Color.red;
            float height = GetHeight();
            for (var i = 0; i < path.Length-1; i++)
            {
                var btmLeft = path[i];
                var btmRight = path[i+1];
                var topLeft = btmLeft + Vector3.up * height;
                var topRight = btmRight + Vector3.up * height;
                Gizmos.DrawLine(btmLeft, btmRight);
                Gizmos.DrawLine(btmLeft, topLeft);
                Gizmos.DrawLine(btmRight, topRight);
                Gizmos.DrawLine(topLeft, topRight);
                
            }
            Gizmos.color = Color.white;
        }
#endif
    }

}