using System.ComponentModel;
using System.Linq;
using UnityEngine.UIElements;

namespace Assignment3.Editor.Nodes
{
    public abstract class Node : VisualElement
    {
        protected Node()
        {
            style.width = new StyleLength(new Length(200f));
            style.height = new StyleLength(new Length(100f));
            RegisterCallback<AttachToPanelEvent>(Init);
        }
        
        // Add the two custom UXML attributes.
        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription _mTitle = new() { name = "title", defaultValue = $"{nameof(Node)}" };
            private readonly UxmlIntAttributeDescription _mOutputPorts = new() { name = "output-ports", defaultValue = 0 };
            
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var node = ve as Node;
                System.Diagnostics.Debug.Assert(node != null, nameof(node) + " != null");
                
                node.title = _mTitle.GetValueFromBag(bag, cc);
                node.outputPorts = _mOutputPorts.GetValueFromBag(bag, cc);
            }
        }
        
        // Must expose your element class to a { get; set; } property that has the same name 
        // as the name you set in your UXML attribute description with the camel case format
        public string title { get; set; }
        public int outputPorts { get; set; }

        private void Init(AttachToPanelEvent attachToPanelEvent)
        {
            var titleLabel = this.Q<Label>("Title");
            if (titleLabel is null) return;
            var currentType = GetType().ToString().Split('.').Last();
            titleLabel.text = $"{currentType}";
        }
    }
}