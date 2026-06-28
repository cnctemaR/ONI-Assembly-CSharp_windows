using System;
using UnityEngine;

namespace NodeEditorFramework.Utilities
{
	public class GenericMenu
	{
		public GenericMenu()
		{
			GenericMenu.popup = new PopupMenu();
		}

		public Vector2 Position
		{
			get
			{
				return GenericMenu.popup.Position;
			}
		}

		public void ShowAsContext()
		{
			GenericMenu.popup.Show(GUIScaleUtility.GUIToScreenSpace(Event.current.mousePosition), 40f);
		}

		public void Show(Vector2 pos, float MinWidth = 40f)
		{
			GenericMenu.popup.Show(GUIScaleUtility.GUIToScreenSpace(pos), MinWidth);
		}

		public void AddItem(GUIContent content, bool on, PopupMenu.MenuFunctionData func, object userData)
		{
			GenericMenu.popup.AddItem(content, on, func, userData);
		}

		public void AddItem(GUIContent content, bool on, PopupMenu.MenuFunction func)
		{
			GenericMenu.popup.AddItem(content, on, func);
		}

		public void AddSeparator(string path)
		{
			GenericMenu.popup.AddSeparator(path);
		}

		private static PopupMenu popup;
	}
}
