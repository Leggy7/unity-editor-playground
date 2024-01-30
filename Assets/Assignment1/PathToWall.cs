using JetBrains.Annotations;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assignment1
{
    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
    public class PathToWall : MonoBehaviour
    {
        [Foldout("Extras")]
        // -- path --
        [SerializeField] Vector3[] path = System.Array.Empty<Vector3>();
        
        // -- height --
        const float DefaultHeight = 4;
        [SerializeField] bool useCustomHeight;
        [SerializeField][ShowIf(nameof(useCustomHeight))] float customHeight = 4;

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
        [Button]
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
        [Button][EnableIf("CanGenerateWallMesh")]
        void GenerateWallMesh()
        {
            if (path.Length < 2) return;
            
            var vertices = GetVertices();
            var tris = GetTriangles(vertices);
            
            var mesh = new Mesh
            {
                vertices = vertices,
                triangles = tris
            };
            mesh.RecalculateNormals();

            MeshFilter.mesh = mesh;
        }

        private Vector3[] GetVertices()
        {
            if (path is {Length: <= 0}) return System.Array.Empty<Vector3>();

            var height = GetHeight();
            var vertices = new Vector3[path.Length * 2];
            for (var i = 0; i < path.Length; i++)
            {
                vertices[2*i] = path[i];
                vertices[2*i + 1] = path[i] + new Vector3(0, height, 0);
                
            }

            return vertices;
        }

        private int[] GetTriangles(Vector3[] vertices)
        {
            if (vertices is {Length: <= 2}) return System.Array.Empty<int>();
            
            var tris = new int[vertices.Length * 3];
            for (var i = 0; i < vertices.Length - 2; i++)
            { // quick n dirty hack to get the triangles drawn clockwise
                tris[i * 3] = i % 2 == 0 ? i : i + 1;
                tris[i * 3 + 1] = i % 2 == 0 ? i + 1 : i;
                tris[i * 3 + 2] = i + 2;
            }

            return tris;
        }

        [UsedImplicitly]
        private bool CanGenerateWallMesh()
        {
            return path is {Length: > 0};
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