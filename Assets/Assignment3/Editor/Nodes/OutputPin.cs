using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assignment3.Editor.Nodes
{
    public class OutputPin : VisualElement
    {
        private bool _used;
        private VisualElement _usedFlag;
        
        public OutputPin()
        {
            style.width = new StyleLength(new Length(10f));
            style.height = new StyleLength(new Length(10f));
            RegisterCallback<AttachToPanelEvent>(Init);
        }

        public new class UxmlFactory : UxmlFactory<OutputPin, UxmlTraits> {}
        
        // Add the two custom UXML attributes.
        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            private readonly UxmlBoolAttributeDescription _mUsed = new() { name = "used", defaultValue = false };
            
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var outputPin = ve as OutputPin;
                System.Diagnostics.Debug.Assert(outputPin != null, nameof(outputPin) + " != null");
                outputPin.used = _mUsed.GetValueFromBag(bag, cc);
            }
        }

        public bool used
        {
            get => _used;
            set
            {
                _used = value;
                if(_usedFlag is not null)
                {
                    _usedFlag.visible = _used;
                }
            }
        }

        private void Init(AttachToPanelEvent evt)
        {
            
            if (_usedFlag is not null) return;
            
            var roundShape = this.Q<VisualElement>("RoundShape");
            var usedFlag = roundShape.Q<VisualElement>("UsedFlag");
            _usedFlag = usedFlag;
        }
    }
}
