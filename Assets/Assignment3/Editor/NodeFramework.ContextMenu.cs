using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assignment3.Editor
{
    public partial class NodeFramework
    {
        private void OpenContextMenu()
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Create new node", null, ""), false, OnCreateNodeButtonClicked);
            menu.AddItem(new GUIContent("Delete selected node", null, ""), false, DeleteNode);
            menu.ShowAsContext();
        }

        private void DeleteNode()
        {
            DeleteNode(_selectedNode);
        }

        private void OnMouseUpOnBoardEventHandler(MouseUpEvent evt)
        {
            if (evt.button == 1)
            {
                OpenContextMenu();
            }
        }
    }
}