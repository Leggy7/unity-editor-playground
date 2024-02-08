using System.Linq;
using Assignment3.Editor.Nodes;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assignment3.Editor
{
    public partial class NodeFramework
    {
        private OutputPin selectedPin;
        private void OpenContextMenu()
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Create new node", null, ""), false, OnCreateNodeButtonClicked);
            menu.AddItem(new GUIContent("Delete selected node", null, ""), false, DeleteNode);
            menu.ShowAsContext();
        }

        private void OpenPinContextMenu(OutputPin pin)
        {
            var menu = new GenericMenu();

            var pinUsed = _connections.Any(ce => ce.fromPin == pin);

            if (!pinUsed)
            {
                menu.AddDisabledItem(new GUIContent("Remove connection", null, ""), false);
            }
            else
            {
                menu.AddItem(new GUIContent("Remove connection", null, ""), false, OnRemoveNodeConnectionMenuItemClicked);
            }
            
            menu.ShowAsContext();
        }

        private void DeleteNode()
        {
            DeleteNode(_selectedNode);
        }

        private void OnMouseUpOnBoardEventHandler(MouseUpEvent evt)
        {
            if (evt.button != 1) return;
            var ve = evt.target as VisualElement;
            var pin = ve!.GetFirstAncestorOfType<OutputPin>();
            
            if (pin is not null)
            {
                selectedPin = pin;
                OpenPinContextMenu(pin);
            }
            else
            {
                OpenContextMenu();
            }
        }
            
        private void OnRemoveNodeConnectionMenuItemClicked()
        {
            var connection = _connections.First(ce => ce.fromPin == selectedPin);
            connection.fromPin.Q<VisualElement>("UsedFlag").visible = false;
            _connections.Remove(connection);
            selectedPin = null;
        }
    }
}