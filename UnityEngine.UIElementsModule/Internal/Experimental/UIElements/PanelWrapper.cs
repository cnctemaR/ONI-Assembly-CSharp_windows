using System;
using UnityEngine.Experimental.UIElements;

namespace UnityEngine.Internal.Experimental.UIElements
{
	public class PanelWrapper : ScriptableObject
	{
		private void OnEnable()
		{
			this.m_Panel = UIElementsUtility.FindOrCreatePanel(this);
		}

		private void OnDisable()
		{
			this.m_Panel = null;
		}

		public VisualElement visualTree
		{
			get
			{
				return this.m_Panel.visualTree;
			}
		}

		public void Repaint(Event e)
		{
			this.m_Panel.Repaint(e);
		}

		private Panel m_Panel;
	}
}
