using System;
using NaughtyAttributes;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

namespace Assignment2
{
    public class LucasCube : MonoBehaviour
    {
        public Vector3 position = Vector3.zero;
        public Vector3 rotation = Vector3.zero;
        public Vector3 scale = new(1, 1, 1);

        private Vector3[] _vertices = Array.Empty<Vector3>();
        private MeshFilter _meshFilter;

        private MeshFilter MeshFilter
        {
            get
            {
                if (_meshFilter) return _meshFilter;

                if (TryGetComponent(out _meshFilter))
                    return _meshFilter;
                
                Debug.LogError($"No Mesh Filter Component in GameObject {name}", gameObject);
                return null;
            }
        }

        [Button]
        public void CreateCube()
        {
            int[] tris =
            {
                6, 5, 4, //face front
                5, 6, 7,
                5, 7, 1, //face top
                3, 1, 7,
                6, 2, 7, //face right
                7, 2, 3,
                0, 4, 1, //face left
                5, 1, 4,
                2, 0, 1, //face rear
                2, 1, 3,
                2, 4, 0, //face bottom
                6, 4, 2
            };
        
            SetVertices();
            var mesh = new Mesh
            {
                vertices = _vertices,
                triangles = tris
            };

            mesh.RecalculateNormals();
            MeshFilter.mesh = mesh;

            Debug.Log($"Pressed Shit");
            // lucasCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            // lucasCube.transform.position = position;
            // lucasCube.transform.rotation = Quaternion.Euler(rotation);
            // lucasCube.transform.localScale = scale;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            SetVertices();
            
            Gizmos.color = Color.red;
            
            Gizmos.DrawLine(_vertices[0], _vertices[1]);
            Gizmos.DrawLine(_vertices[0], _vertices[2]);
            Gizmos.DrawLine(_vertices[2], _vertices[3]);
            Gizmos.DrawLine(_vertices[1], _vertices[3]);
            
            Gizmos.DrawLine(_vertices[4], _vertices[5]);
            Gizmos.DrawLine(_vertices[4], _vertices[6]);
            Gizmos.DrawLine(_vertices[6], _vertices[7]);
            Gizmos.DrawLine(_vertices[5], _vertices[7]);
            
            Gizmos.DrawLine(_vertices[1], _vertices[5]);
            Gizmos.DrawLine(_vertices[3], _vertices[7]);
            Gizmos.DrawLine(_vertices[0], _vertices[4]);
            Gizmos.DrawLine(_vertices[2], _vertices[6]);
            
            Gizmos.color = Color.white;
        }
#endif

        private void SetVertices()
        {
            var leftBottomRear = Quaternion.Euler(rotation) * new Vector3(- scale.x * 0.5f, - scale.y * 0.5f, - scale.z * 0.5f) + position;
            var leftTopRear = Quaternion.Euler(rotation) * new Vector3(- scale.x * 0.5f, scale.y * 0.5f, - scale.z * 0.5f) + position;
            var rightBottomRear = Quaternion.Euler(rotation) * new Vector3(scale.x * 0.5f, - scale.y * 0.5f, - scale.z * 0.5f) + position;
            var rightTopRear = Quaternion.Euler(rotation) * new Vector3(scale.x * 0.5f, scale.y * 0.5f, - scale.z * 0.5f) + position;
            var leftBottomFront = Quaternion.Euler(rotation) * new Vector3(- scale.x * 0.5f, - scale.y * 0.5f, scale.z * 0.5f) + position;
            var leftTopFront = Quaternion.Euler(rotation) * new Vector3(- scale.x * 0.5f, scale.y * 0.5f, scale.z * 0.5f) + position;
            var rightBottomFront = Quaternion.Euler(rotation) * new Vector3(scale.x * 0.5f, - scale.y * 0.5f, scale.z * 0.5f) + position;
            var rightTopFront = Quaternion.Euler(rotation) * new Vector3(scale.x * 0.5f, scale.y * 0.5f, scale.z * 0.5f) + position;

            _vertices = new[]
            {
                leftBottomRear, //left, bottom, rear 0
                leftTopRear, // left, top, rear 1
                rightBottomRear, // right, bottom, rear 2
                rightTopRear, // right, top, rear 3
                leftBottomFront, // left, bottom, front 4
                leftTopFront, // left, top, front 5
                rightBottomFront, // right, bottom, front 6
                rightTopFront // right, top, front 7
            };
        }
    }

    [EditorTool("Cube manager Tool", typeof(LucasCube))]
    public class CubeTool : EditorTool
    {
        public override void OnActivated()
        {
            SceneView.lastActiveSceneView.ShowNotification(new GUIContent("Creating cube..."), .1f);
        }

        public override void OnWillBeDeactivated()
        {
            SceneView.lastActiveSceneView.ShowNotification(new GUIContent("Stopping cube magic..."), .1f);
        }

        public override void OnToolGUI(EditorWindow window)
        {
            foreach (var obj in targets)
            {
                if (obj is not LucasCube cube)
                    continue;

                EditorGUI.BeginChangeCheck();
                var positionHandle = Handles.PositionHandle(cube.position, Quaternion.identity);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(cube, "Set Cube Position");
                    cube.position = positionHandle;
                }

                EditorGUI.BeginChangeCheck();
                var rotationHandle = Handles.RotationHandle(Quaternion.Euler(cube.rotation), cube.position);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(cube, "Set Cube Rotation");
                    cube.rotation = rotationHandle.eulerAngles;
                }

                EditorGUI.BeginChangeCheck();
                var sizeHandle = Handles.ScaleHandle(cube.scale, cube.position, Quaternion.Euler(cube.rotation));
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(cube, "Set Cube Scale");
                    cube.scale = sizeHandle;
                }
            }
        }
    }
}