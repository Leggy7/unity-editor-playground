using UnityEditor;
using UnityEngine;

namespace Assignment2
{
    [CustomEditor(typeof(LucasCube))]
    public class LucasCubeInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var rect = EditorGUILayout.GetControlRect();
            var createCubeButtonRect = new Rect(new Vector2(5, rect.position.y), new Vector2(100, 30));
            if (GUI.Button(createCubeButtonRect, new GUIContent("Shape cube")))
            {
                (target as LucasCube)!.CreateCube();
                SceneView.RepaintAll();
            }
        }
    }
}
