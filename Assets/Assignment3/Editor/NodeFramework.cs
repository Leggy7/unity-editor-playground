using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assignment3.Editor.Nodes;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assignment3.Editor
{
    public partial class NodeFramework : EditorWindow
    {
        [SerializeField]
        private VisualTreeAsset m_VisualTreeAsset = default;
        [SerializeField]
        private VisualTreeAsset singleNodeUxml;
        [SerializeField]
        private VisualTreeAsset binaryNodeUxml;

        private VisualElement _board;
        private EnumField _nodeTypeSelector;
        private Button _createNodeButton;
        private Button _exportBoardButton;

        private OutputPin _startingPin;
        private Vector2 _startingPosition;

        private Node _selectedNode;
        private readonly List<VisualElement> _nodes = new();
        private readonly List<(OutputPin fromPin, Node toNode)> _connections = new();

        private readonly Vector2 _pinOriginDisplacement = new(0, -20f);
        private bool _creationBusy;

        public List<(OutputPin fromPin, Node toNode)> Connections => _connections;
        
        [MenuItem("Febucci/NodeFramework")]
        public static void ShowExample()
        {
            var wnd = GetWindow<NodeFramework>();
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
            _board = mainContainer.Q<VisualElement>("Board");
            
            var toolbar = mainContainer.Q<VisualElement>("BoardToolbar");
            _nodeTypeSelector = toolbar.Q<EnumField>();
            _createNodeButton = toolbar.Q<Button>("CreateNodeButton");
            _exportBoardButton = toolbar.Q<Button>("Export");
            _createNodeButton.clicked += OnCreateNodeButtonClicked;
            _exportBoardButton.clicked += OnExportButtonClicked;
            _board.RegisterCallback<MouseUpEvent>(OnMouseUpOnBoardEventHandler);
            wantsMouseMove = true;
            mainContainer.RegisterCallback<MouseUpEvent>(OnMouseUpEventHandler);
        }

        private void OnGUI()
        {
            Handles.BeginGUI();
            if(_startingPin is not null)
            {
                var toPosition = Event.current.mousePosition;
                Handles.color = Color.red;
                Handles.DrawLine(_startingPosition, toPosition);
            }
            RenderNodeConnections();
            Repaint();
            Handles.EndGUI();
            
            HandleKeyboardEvents();
        }

        /// <summary>
        /// This processes all the keyboard input that will be otherwise captured by other editor windows
        /// </summary>
        private void HandleKeyboardEvents()
        {
            if (Event.current != null && Event.current.isKey && Event.current.keyCode == KeyCode.Delete)
            {
                if (_selectedNode is not null && !_selectedNode.IsStart)
                {
                    Event.current.Use();
                    DeleteNode(_selectedNode);
                }
            }
            else if (Event.current != null && Event.current.isKey && Event.current.keyCode == KeyCode.N && Event.current.control)
            {
                Event.current.Use();
                CreateWithDelay().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// This was necessary because <see cref="OnGUI"/> is called more than once per frame.
        /// I'm then processing the event and wait for the next frame.
        /// </summary>
        private async Task CreateWithDelay()
        {
            if (_creationBusy) return;
            _creationBusy = true;
            OnCreateNodeButtonClicked();
            await Task.Delay(TimeSpan.FromSeconds(Time.deltaTime));
            _creationBusy = false;
        }

        private void DeleteNode(Node n)
        {
            var connectionEntriesFrom = _connections.Where(ce => ce.fromPin.GetFirstAncestorOfType<Node>() == n).ToList();
            var connectionEntriesTo = _connections.Where(ce => ce.toNode == n).ToList();
            connectionEntriesFrom.ForEach(ce => _connections.Remove(ce));
            connectionEntriesTo.ForEach(ce => _connections.Remove(ce));
            if (_nodes.Contains(n)) _nodes.Remove(n);

            connectionEntriesTo.ForEach(ce =>
            {
                ce.fromPin.Q<VisualElement>("UsedFlag").visible = false;
            });
            
            _board.Remove(n.parent);
            _selectedNode = null;
        }

        private void RenderNodeConnections()
        {
            foreach (var connection in _connections)
            {
                Handles.color = Color.white;
                var fromPosition = connection.fromPin.worldBound.center + _pinOriginDisplacement;
                var toPosition = connection.toNode.worldBound.center;
                Handles.DrawLine(fromPosition, toPosition);
            }
        }

        private void OnCreateNodeButtonClicked()
        {
            TemplateContainer instance = null;
            if (_nodeTypeSelector.text == NodeType.Single.ToString())
            {
                instance = singleNodeUxml.Instantiate();
            }
            if (_nodeTypeSelector.text == NodeType.Binary.ToString())
            {
                instance = binaryNodeUxml.Instantiate();
            }

            if (instance is not null)
            {
                _board.Add(instance);
                instance.transform.position = _board.transform.position;
                instance.RegisterCallback<MouseDownEvent>(OnMouseDownOnNodeEventHandler);
                var dragAndDropManipulator = new DragAndDropManipulator(instance, this);
                instance.Query<OutputPin>().ToList().ForEach(op =>
                {
                    op.RegisterCallback<MouseDownEvent>(OnMouseDownOnPinEventHandler);
                    op.RegisterCallback<MouseOverEvent>(OnMouseOverOnPinEventHandler);
                    op.RegisterCallback<MouseOutEvent>(OnMouseOutOnPinEventHandler);
                });
                
                if(_nodes.Count <= 0) SetNodeAsStart(instance.Q<Node>());
                _nodes.Add(instance);
            }
        }

        /// <summary>
        /// This is responsible of setting <paramref name="node"/> as start node.
        /// </summary>
        /// <param name="node">The node to be set as start</param>
        private void SetNodeAsStart(Node node)
        {
            _nodes.ForEach(n =>
            {
                var nodeVal =n.GetFirstAncestorOfType<Node>();
                nodeVal.IsStart = false;
                n.RemoveFromClassList("startNode");
            });
            node.IsStart = true;
            node.Q<VisualElement>("Frame").AddToClassList("startNode");
        }

        /// <summary>
        /// If not landing on an <see cref="OutputPin"/> this function sets the target node as selected one.
        /// </summary>
        private void OnMouseDownOnNodeEventHandler(MouseDownEvent evt)
        {
            var ve = evt.target as VisualElement;
            var node = ve!.GetFirstAncestorOfType<Node>();
            var outputPin = ve!.GetFirstAncestorOfType<OutputPin>();

            if (node is null || outputPin is not null)
            {
                return;
            }

            if (_selectedNode is not null)
            {
                _selectedNode.Q<VisualElement>("Frame").RemoveFromClassList("completeSelected");
                _selectedNode.Q<VisualElement>("Frame").AddToClassList("completeUnselected");
            }
            
            _selectedNode = node;
            _selectedNode.Q<VisualElement>("Frame").RemoveFromClassList("completeUnselected");
            _selectedNode.Q<VisualElement>("Frame").AddToClassList("completeSelected");
        }

        private void OnMouseOutOnPinEventHandler(MouseOutEvent evt)
        {
            var ve = evt.target as VisualElement;
            if (ve!.name != "RoundShape") return;
            ve.RemoveFromClassList("hovered");
        }

        private void OnMouseOverOnPinEventHandler(MouseOverEvent evt)
        { 
            var ve = evt.target as VisualElement;
            if (ve!.name != "RoundShape") return;
            ve.AddToClassList("hovered");
        }

        /// <summary>
        /// Called generally on <see cref="NodeFramework"/> editor window.
        /// This is the way in which I detect mouse up event.
        /// </summary>
        private void OnMouseUpEventHandler(MouseUpEvent evt)
        {
            var target = (VisualElement) evt.target;
            if(target.worldBound.Contains(evt.mousePosition))
            {
                var rootNode = target.GetFirstAncestorOfType<Node>();
                if(rootNode is not null && _startingPin.FindCommonAncestor(rootNode) != rootNode)
                {
                    var toAdd = (startingPin: _startingPin, rootNode);
                    if (!_connections.Contains(toAdd))
                    {
                        _connections.Add((_startingPin, rootNode));
                        var pinFlag = _startingPin.Q<VisualElement>("UsedFlag");
                        pinFlag.visible = true;
                    }
                }
            }

            _startingPin = null;
        }
        
        /// <summary>
        /// Handles the tracing route for node connection.
        /// While held down it is than managed on the <see cref="OnGUI"/> function.
        /// The final event catching the release in order to actually connect nodes is demanded to the <see cref="OnMouseUpEventHandler"/> which is global to the main container
        /// </summary>
        /// <param name="evt"></param>
        private void OnMouseDownOnPinEventHandler(MouseDownEvent evt)
        {
            var elementClicked = evt.target as VisualElement;
            _startingPin = elementClicked!.GetFirstAncestorOfType<OutputPin>();
            if (_startingPin is null) return;
            
            _startingPosition = _startingPin.worldBound.center;
            _startingPosition += _pinOriginDisplacement;
        }
    }

    public enum NodeType
    {
        Single = 0,
        Binary
    }
}