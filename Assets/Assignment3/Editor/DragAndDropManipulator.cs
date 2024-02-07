using System.Collections.Generic;
using System.Linq;
using Assignment3.Editor.Nodes;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assignment3.Editor
{
    public class DragAndDropManipulator : PointerManipulator
    {
        private readonly NodeFramework _frameworkReference;
        private List<(Node node, Vector3 startPosition)> nodesToDrag;

        // Write a constructor to set target and store a reference to the
        // root of the visual tree.
        public DragAndDropManipulator(VisualElement target, NodeFramework frameworkReference)
        {
            _frameworkReference = frameworkReference;
            this.target = target;
            root = target.parent;
        }

        protected override void RegisterCallbacksOnTarget()
        {
            // Register the four callbacks on target.
            target.RegisterCallback<PointerDownEvent>(PointerDownHandler);
            target.RegisterCallback<PointerMoveEvent>(PointerMoveHandler);
            target.RegisterCallback<PointerUpEvent>(PointerUpHandler);
            target.RegisterCallback<PointerCaptureOutEvent>(PointerCaptureOutHandler);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            // Un-register the four callbacks from target.
            target.UnregisterCallback<PointerDownEvent>(PointerDownHandler);
            target.UnregisterCallback<PointerMoveEvent>(PointerMoveHandler);
            target.UnregisterCallback<PointerUpEvent>(PointerUpHandler);
            target.UnregisterCallback<PointerCaptureOutEvent>(PointerCaptureOutHandler);
        }

        private Vector2 targetStartPosition { get; set; }

        private Vector3 pointerStartPosition { get; set; }

        private bool enabled { get; set; }

        private VisualElement root { get; }

        // This method stores the starting position of target and the pointer,
        // makes target capture the pointer, and denotes that a drag is now in progress.
        private void PointerDownHandler(PointerDownEvent evt)
        {
            var elementClicked = evt.target as VisualElement;
            if (elementClicked!.GetFirstAncestorOfType<OutputPin>() is not null)
            {
                return;
            }
            
            targetStartPosition = target.transform.position;
            pointerStartPosition = evt.position;
            target.CapturePointer(evt.pointerId);
            
            enabled = true;
        }

        // This method checks whether a drag is in progress and whether target has captured the pointer.
        // If both are true, calculates a new position for target within the bounds of the window.
        private void PointerMoveHandler(PointerMoveEvent evt)
        {
            if (enabled && target.HasPointerCapture(evt.pointerId))
            {
                var elementClicked = evt.target as VisualElement;
                Vector3 pointerDelta = evt.position - pointerStartPosition;

                var position = new Vector2(
                    Mathf.Clamp(targetStartPosition.x + pointerDelta.x, 0, root.worldBound.width),
                    Mathf.Clamp(targetStartPosition.y + pointerDelta.y, 0, root.worldBound.height));
                
                target.transform.position = position;
                
            
                if (evt.shiftKey)
                {
                    var dragNodes = GetConnectedNodes(elementClicked!.Q<Node>());
                    dragNodes.ForEach(dn =>
                    {
                        var nodePosition = new Vector2(
                            Mathf.Clamp(dn.Item2.x + pointerDelta.x, 0, root.worldBound.width),
                            Mathf.Clamp(dn.Item2.y + pointerDelta.y, 0, root.worldBound.height));
                
                        dn.Item1.transform.position = nodePosition;
                    });
                }
            }
        }

        // This method checks whether a drag is in progress and whether target has captured the pointer.
        // If both are true, makes target release the pointer.
        private void PointerUpHandler(PointerUpEvent evt)
        {
            if (enabled && target.HasPointerCapture(evt.pointerId))
            {
                target.ReleasePointer(evt.pointerId);
            }

            nodesToDrag = null;
        }
        
        private void PointerCaptureOutHandler(PointerCaptureOutEvent evt)
        {
            if (enabled)
            {
                enabled = false;
            }
        }

        private List<(Node, Vector3)> GetConnectedNodes(Node node)
        {
            if (nodesToDrag == null)
            {
                var connectedFromMe = _frameworkReference.Connections
                    .Where(c => c.fromPin.GetFirstAncestorOfType<Node>() == node)
                    .Select(c => (c.toNode, c.toNode.transform.position)).ToList();
                
                var connectedWithMe = _frameworkReference.Connections
                    .Where(c => c.toNode == node)
                    .Select(c => (c.fromPin.GetFirstAncestorOfType<Node>(), c.fromPin.GetFirstAncestorOfType<Node>().transform.position))
                    .ToList();

                nodesToDrag = new List<(Node node, Vector3 startPosition)>();
                nodesToDrag.AddRange(connectedWithMe);
                nodesToDrag.AddRange(connectedFromMe);
            }

            return nodesToDrag;
        }
    }
}