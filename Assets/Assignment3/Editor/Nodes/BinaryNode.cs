using UnityEngine.UIElements;

namespace Assignment3.Editor.Nodes
{
    public class BinaryNode : Node
    {
        public BinaryNode() : base()
        {
            
        }
        
        public new class UxmlFactory : UxmlFactory<BinaryNode, UxmlTraits> {}
    }
}