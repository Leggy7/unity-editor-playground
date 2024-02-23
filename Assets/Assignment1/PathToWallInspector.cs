using UnityEditor;
using UnityEngine;

namespace Assignment1
{
    [CustomEditor(typeof(PathToWall))]
    public class PathToWallInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var rect = EditorGUILayout.GetControlRect();
            var generatePathButtonRect = new Rect(new Vector2(5, rect.position.y), new Vector2(100, 30));
            if (GUI.Button(generatePathButtonRect, new GUIContent("Generate path")))
            {
                (target as PathToWall)!.GenerateNewPath();
                SceneView.RepaintAll();
            }
            
            var generateWallsButtonRect = new Rect(new Vector2(110, rect.position.y), new Vector2(100, 30));
            if (GUI.Button(generateWallsButtonRect, new GUIContent("Generate wall")))
            {
                (target as PathToWall)!.GenerateWallMesh();
                SceneView.RepaintAll();
            }
        }
    }
}
