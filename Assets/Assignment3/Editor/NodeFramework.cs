using Assignment3.Editor.Nodes;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assignment3.Editor
{
    public class NodeFramework : EditorWindow
    {
        [SerializeField]
        private VisualTreeAsset m_VisualTreeAsset = default;
        [SerializeField]
        private VisualTreeAsset singleNodeUxml;
        [SerializeField]
        private VisualTreeAsset binaryNodeUxml;

        private VisualElement board;
        private EnumField nodeTypeSelector;
        private Button createNodeButton;

        [MenuItem("Febucci/NodeFramework")]
        public static void ShowExample()
        {
            NodeFramework wnd = GetWindow<NodeFramework>();
            wnd.titleContent = new GUIContent("NodeFramework");
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            var root = rootVisualElement;

            // Instantiate UXML
            VisualElement mainContainer = m_VisualTreeAsset.Instantiate();
            root.Add(mainContainer);
            
            // this is a life saver!
            mainContainer.StretchToParentSize();
            board = mainContainer.Q<VisualElement>("Board");
            
            var toolbar = mainContainer.Q<VisualElement>("BoardToolbar");
            nodeTypeSelector = toolbar.Q<EnumField>();
            createNodeButton = toolbar.Q<Button>();
            createNodeButton.clicked += OnCreateNodeButtonClicked;
        }

        private void OnCreateNodeButtonClicked()
        {
            TemplateContainer instance = null;
            if (nodeTypeSelector.text == NodeType.Single.ToString())
            {
                instance = singleNodeUxml.Instantiate();
            }
            if (nodeTypeSelector.text == NodeType.Binary.ToString())
            {
                instance = binaryNodeUxml.Instantiate();
            }

            if (instance is not null)
            {
                board.Add(instance);
                instance.transform.position = board.transform.position;
                var dragAndDropManipulator = new DragAndDropManipulator(instance);
                var node = instance.Q<Node>();
                Debug.Log($"{node.name} has {node.outputPorts} output ports.");
            }
        }
    }

    public enum NodeType
    {
        Single = 0,
        Binary
    }
}