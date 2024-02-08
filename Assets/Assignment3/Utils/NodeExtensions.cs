using System;
using Assignment3.Editor;
using Assignment3.Editor.Nodes;

namespace Assignment3.Utils
{
    public static class NodeExtensions
    {
        public static NodeFramework.SerializableNode ToSerializableNode(this Node node)
        {
            return node switch
            {
                SingleNode sn => new NodeFramework.SerializableSingleNode
                {
                    startNode = sn.IsStart, 
                    name = sn.name, 
                    id = sn.id.ToString(), 
                    title = sn.title,
                    type = sn.GetType().ToString()
                },
                BinaryNode bn => new NodeFramework.SerializableBinaryNode
                {
                    startNode = bn.IsStart,
                    title = bn.title,
                    id = bn.id.ToString(),
                    type = bn.GetType().ToString()
                },
                _ => throw new ArgumentOutOfRangeException(nameof(node))
            };
        }
    }
}