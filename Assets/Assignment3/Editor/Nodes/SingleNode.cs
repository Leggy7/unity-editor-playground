using UnityEditor;
using UnityEngine.UIElements;

namespace Assignment3.Editor.Nodes
{
    public class SingleNode : Node
    {
        public SingleNode() : base()
        {
            
        }
        
        public new class UxmlFactory : UxmlFactory<SingleNode, UxmlTraits> {}
    }
}