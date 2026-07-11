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
			if (this.m_Updater != null)
			{
				this.m_Updater.Dispose();
			}
			this.m_Panel = null;
		}

		public bool UIREnabled
		{
			set
			{
				if (this.m_Updater != null)
				{
					this.m_Updater.Dispose();
				}
				if (value)
				{
					this.m_Updater = new UIRRepaintUpdater();
				}
				else
				{
					this.m_Updater = new VisualTreeRepaintUpdater();
				}
				this.m_Panel.SetUpdater(this.m_Updater, VisualTreeUpdatePhase.Repaint);
			}
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

		private BaseVisualTreeUpdater m_Updater;
	}
}
