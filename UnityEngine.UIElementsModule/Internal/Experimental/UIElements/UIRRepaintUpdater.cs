using System;
using System.Collections.Generic;
using UnityEngine.Experimental.UIElements;

namespace UnityEngine.Internal.Experimental.UIElements
{
	internal class UIRRepaintUpdater : BaseVisualTreeUpdater
	{
		public override string description
		{
			get
			{
				return "UIRRepaintUpdater";
			}
		}

		protected override void Dispose(bool disposing)
		{
			this.m_Painter.Dispose(disposing);
		}

		public override void OnVersionChanged(VisualElement ve, VersionChangeType versionChangeType)
		{
			if ((versionChangeType & VersionChangeType.Repaint) == VersionChangeType.Repaint)
			{
				if (!this.m_Elements.Contains(ve))
				{
					this.m_Elements.Add(ve);
				}
			}
		}

		public override void Update()
		{
			if (this.m_Elements.Count > 0)
			{
				foreach (VisualElement visualElement in this.m_Elements)
				{
					this.m_Painter.currentElement = visualElement;
					visualElement.Repaint(this.m_Painter);
				}
				this.m_Elements.Clear();
			}
			this.m_Painter.Draw();
		}

		private UIRPainter m_Painter = new UIRPainter();

		private List<VisualElement> m_Elements = new List<VisualElement>();
	}
}
