using System;
using System.Collections.Generic;

namespace UnityEngine.Experimental.UIElements
{
	internal class VisualTreeStyleUpdater : BaseVisualTreeUpdater
	{
		public override string description
		{
			get
			{
				return "Update Style";
			}
		}

		public void DirtyStyleSheets()
		{
			StyleCache.ClearStyleCache();
			VisualTreeStyleUpdater.PropagateDirtyStyleSheets(base.visualTree);
			base.visualTree.IncrementVersion(VersionChangeType.StyleSheet);
		}

		private static void PropagateDirtyStyleSheets(VisualElement element)
		{
			if (element != null)
			{
				if (element.styleSheets != null)
				{
					element.LoadStyleSheetsFromPaths();
				}
				foreach (VisualElement visualElement in element.shadow.Children())
				{
					VisualTreeStyleUpdater.PropagateDirtyStyleSheets(visualElement);
				}
			}
		}

		public override void OnVersionChanged(VisualElement ve, VersionChangeType versionChangeType)
		{
			if ((versionChangeType & VersionChangeType.StyleSheet) == VersionChangeType.StyleSheet)
			{
				this.m_Version += 1U;
				if (this.m_IsApplyingStyles)
				{
					this.m_ApplyStyleUpdateList.Add(ve);
				}
				else
				{
					this.m_StyleContextHierarchyTraversal.AddChangedElement(ve);
				}
			}
		}

		public override void Update()
		{
			if (this.m_Version != this.m_LastVersion)
			{
				this.m_LastVersion = this.m_Version;
				this.ApplyStyles();
				this.m_StyleContextHierarchyTraversal.Clear();
				foreach (VisualElement visualElement in this.m_ApplyStyleUpdateList)
				{
					this.m_StyleContextHierarchyTraversal.AddChangedElement(visualElement);
				}
				this.m_ApplyStyleUpdateList.Clear();
			}
		}

		private void ApplyStyles()
		{
			Debug.Assert(base.visualTree.panel != null);
			this.m_IsApplyingStyles = true;
			this.m_StyleContextHierarchyTraversal.currentPixelsPerPoint = base.panel.currentPixelsPerPoint;
			this.m_StyleContextHierarchyTraversal.Traverse(base.visualTree);
			this.m_IsApplyingStyles = false;
		}

		private HashSet<VisualElement> m_ApplyStyleUpdateList = new HashSet<VisualElement>();

		private bool m_IsApplyingStyles = false;

		private uint m_Version = 0U;

		private uint m_LastVersion = 0U;

		private VisualTreeStyleUpdaterTraversal m_StyleContextHierarchyTraversal = new VisualTreeStyleUpdaterTraversal();
	}
}
