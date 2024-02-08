using System;
using System.Collections.Generic;
using System.Linq;
using Assignment3.Editor.Nodes;
using Assignment3.Utils;
using UnityEditor;
using UnityEngine.UIElements;

namespace Assignment3.Editor
{
    public partial class NodeFramework
    {
        private void OnExportButtonClicked()
        {
            ExportToJson();
        }
        
        private void ExportToJson()
        {
            var path = EditorUtility.OpenFolderPanel("Export board to JSON", "", "");
            var serializableBoard = new SerializableBoard {Nodes = new List<SerializableNode>()};

            foreach (var vs in _nodes)
            {
                var node = vs.Q<Node>();
                serializableBoard.Nodes.Add(node.ToSerializableNode());
            }
            
            foreach (var connection in _connections)
            {
                var fromNode = connection.fromPin.GetFirstAncestorOfType<Node>();
                var toNode = connection.toNode;
                var serializableFromNode = serializableBoard.Nodes.First(node => node.id == fromNode.id.ToString());
                var serializableToNode = serializableBoard.Nodes.First(node => node.id == toNode.id.ToString());

                for (var n = 0; n < serializableFromNode.connections.Length; n++)
                {
                    if (serializableFromNode.connections[n] != null) continue;
                    serializableFromNode.connections[n] = new NodeConnection
                        {pinId = connection.fromPin.id.ToString(), nodeId = serializableToNode.id};
                    break;
                }
            }

            var json = EditorJsonUtility.ToJson(serializableBoard, prettyPrint: true);
            System.IO.File.WriteAllText(path + "/board.json", json);
        }

        [Serializable]
        public class SerializableBoard
        {
            public List<SerializableNode> Nodes;
        }
        
        [Serializable]
        public class SerializableNode
        {
            public string id;
            public string title;
            public bool startNode;
            public string type;
            public NodeConnection[] connections;
        }

        [Serializable]
        public class SerializableSingleNode : SerializableNode
        {
            public string name;

            public SerializableSingleNode()
            {
                connections = new NodeConnection[1];
            }
        }

        [Serializable]
        public class SerializableBinaryNode : SerializableNode
        {
            public SerializableBinaryNode()
            {
                connections = new NodeConnection[2];
            }
        }
        
        [Serializable]
        public class NodeConnection
        {
            public string pinId;
            public string nodeId;
        }
    }
}